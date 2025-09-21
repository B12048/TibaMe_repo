using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BoardGameFontier.Services;
using BoardGameFontier.Models.ViewModels;
using BoardGameFontier.Models.Common;
using BoardGameFontier.Repostiory.DTOS.Social;
using BoardGameFontier.Extensions;

namespace BoardGameFontier.Controllers.API
{
    /// <summary>
    /// 留言 API 控制器
    /// 提供留言相關的 RESTful API
    /// </summary>
    [Route("api/comments")]
    [ApiController]
    public class CommentsAPIController : ControllerBase
    {
        private readonly ICommentService _commentService;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<CommentsAPIController> _logger;

        public CommentsAPIController(
            ICommentService commentService, 
            ICurrentUserService currentUserService,
            ILogger<CommentsAPIController> logger)
        {
            _commentService = commentService;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        /// <summary>
        /// 取得指定貼文的所有留言
        /// GET: api/comments/post/{postId}
        /// </summary>
        /// <param name="postId">貼文 ID</param>
        /// <returns>留言列表</returns>
        [HttpGet("post/{postId}")]
        public async Task<ActionResult<ApiResponse<List<CommentResponseDto>>>> GetPostComments(int postId)
        {
            try
            {
                if (postId <= 0)
                {
                    _logger.LogWarning("無效的貼文 ID: {PostId}", postId);
                    return BadRequest(ApiResponse<List<CommentResponseDto>>.Error("無效的貼文 ID", 400));
                }

                var comments = await _commentService.GetPostCommentsAsync(postId);
                
                _logger.LogInformation("成功載入貼文 {PostId} 的 {CommentCount} 則留言", postId, comments.Count);
                
                return Ok(ApiResponse<List<CommentResponseDto>>.Ok(comments, "留言載入成功"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得貼文留言時發生錯誤，PostId: {PostId}", postId);
                return StatusCode(500, ApiResponse<List<CommentResponseDto>>.InternalServerError("載入留言失敗，請稍後再試"));
            }
        }

        /// <summary>
        /// 建立新留言
        /// POST: api/comments
        /// </summary>
        /// <param name="createDto">留言建立資料</param>
        /// <returns>建立的留言</returns>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ApiResponse<CommentResponseDto>>> CreateComment([FromBody] CreateCommentRequestDto createDto)
        {
            try
            {
                // 驗證 Model 狀態
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("建立留言請求格式不正確，ModelState: {ModelState}", ModelState);
                    return BadRequest(ApiResponse<CommentResponseDto>.Error("請求格式不正確", 400));
                }

                // 檢查使用者是否已登入
                var currentUserId = _currentUserService.UserId;
                if (string.IsNullOrEmpty(currentUserId))
                {
                    _logger.LogWarning("未登入使用者嘗試建立留言");
                    return Unauthorized(ApiResponse<CommentResponseDto>.Unauthorized("請先登入"));
                }

                // 驗證留言內容
                if (string.IsNullOrWhiteSpace(createDto.Content))
                {
                    _logger.LogWarning("使用者 {UserId} 嘗試建立空白留言", currentUserId);
                    return BadRequest(ApiResponse<CommentResponseDto>.Error("留言內容不能為空", 400));
                }

                if (createDto.Content.Length > 1000)
                {
                    _logger.LogWarning("使用者 {UserId} 嘗試建立過長留言，長度: {Length}", currentUserId, createDto.Content.Length);
                    return BadRequest(ApiResponse<CommentResponseDto>.Error("留言內容不能超過 1000 字", 400));
                }

                var comment = await _commentService.CreateCommentAsync(createDto, currentUserId);
                
                _logger.LogInformation("用戶 {UserId} 在貼文 {PostId} 成功新增留言 {CommentId}", 
                    currentUserId, createDto.PostId, comment.Id);
                    
                return CreatedAtAction(nameof(GetComment), new { id = comment.Id }, ApiResponse<CommentResponseDto>.Ok(comment, "留言建立成功"));
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("建立留言失敗 - 參數錯誤: {Message}, PostId: {PostId}, UserId: {UserId}", 
                    ex.Message, createDto?.PostId, _currentUserService.UserId);
                return BadRequest(ApiResponse<CommentResponseDto>.Error(ex.Message, 400));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "建立留言時發生錯誤，PostId: {PostId}, UserId: {UserId}", 
                    createDto?.PostId, _currentUserService.UserId);
                return StatusCode(500, ApiResponse<CommentResponseDto>.InternalServerError("建立留言失敗，請稍後再試"));
            }
        }

        /// <summary>
        /// 取得單一留言詳細資料
        /// GET: api/comments/{id}
        /// </summary>
        /// <param name="id">留言 ID</param>
        /// <returns>留言詳細資料</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<CommentResponseDto>>> GetComment(int id)
        {
            try
            {
                var comment = await _commentService.GetCommentByIdAsync(id);
                if (comment == null)
                {
                    return NotFound(ApiResponse<CommentResponseDto>.NotFound("留言不存在"));
                }

                return Ok(ApiResponse<CommentResponseDto>.Ok(comment, "留言載入成功"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<CommentResponseDto>.InternalServerError($"載入留言失敗：{ex.Message}"));
            }
        }

        /// <summary>
        /// 刪除留言
        /// DELETE: api/comments/{id}
        /// </summary>
        /// <param name="id">留言 ID</param>
        /// <returns>刪除結果</returns>
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteComment(int id)
        {
            try
            {
                if (id <= 0)
                {
                    _logger.LogWarning("無效的留言 ID: {CommentId}", id);
                    return BadRequest(ApiResponse<bool>.Error("無效的留言 ID", 400));
                }

                // 檢查使用者是否已登入
                var currentUserId = _currentUserService.UserId;
                if (string.IsNullOrEmpty(currentUserId))
                {
                    _logger.LogWarning("未登入使用者嘗試刪除留言 {CommentId}", id);
                    return Unauthorized(ApiResponse<bool>.Unauthorized("請先登入"));
                }

                var success = await _commentService.DeleteCommentAsync(id, currentUserId);
                if (!success)
                {
                    _logger.LogWarning("使用者 {UserId} 無權限刪除留言 {CommentId}", currentUserId, id);
                    return BadRequest(ApiResponse<bool>.Error("無法刪除留言，可能留言不存在或您沒有權限", 400));
                }

                _logger.LogInformation("用戶 {UserId} 成功刪除留言 {CommentId}", currentUserId, id);
                
                return Ok(ApiResponse<bool>.Ok(true, "留言刪除成功"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "刪除留言時發生錯誤，CommentId: {CommentId}, UserId: {UserId}", 
                    id, _currentUserService.UserId);
                return StatusCode(500, ApiResponse<bool>.InternalServerError("刪除留言失敗，請稍後再試"));
            }
        }
    }

}