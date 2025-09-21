using BoardGameFontier.Services;
using BoardGameFontier.Models.Common;
using BoardGameFontier.Repostiory.DTOS.Social;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Collections.Concurrent;

namespace BoardGameFontier.Controllers.API
{
    /// <summary>
    /// 按讚功能專用 API Controller - RESTful 資源導向設計
    /// 支援 Post 和 Comment 的按讚操作
    /// 路由格式：/api/likes
    /// </summary>
    [Route("api/likes")]
    [ApiController]
    public class LikesAPIController : ControllerBase
    {
        private readonly ILikeService _likeService;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<LikesAPIController> _logger;
        
        // 併發控制：每個用戶+項目組合的信號量
        private static readonly ConcurrentDictionary<string, SemaphoreSlim> _userItemLocks = new();

        public LikesAPIController(ILikeService likeService, ICurrentUserService currentUserService, ILogger<LikesAPIController> logger)
        {
            _likeService = likeService;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        /// <summary>
        /// 建立按讚 - RESTful 標準 POST 操作
        /// POST /api/likes
        /// Body: { "itemType": "Post", "itemId": 123 }
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateLike([FromBody] CreateLikeRequest request)
        {
            var userId = _currentUserService.UserId;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ApiResponse<LikeResponseDto>.Unauthorized("請先登入"));
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<LikeResponseDto>.Error("請求參數無效", 400));
            }

            // 建立用戶+項目的唯一鎖定鍵
            var lockKey = $"{userId}_{request.ItemType}_{request.ItemId}";
            var semaphore = _userItemLocks.GetOrAdd(lockKey, _ => new SemaphoreSlim(1, 1));

            try
            {
                var acquired = await semaphore.WaitAsync(TimeSpan.FromSeconds(10));
                if (!acquired)
                {
                    return StatusCode(408, ApiResponse<LikeResponseDto>.Error("操作超時，請稍後再試", 408));
                }

                try
                {
                    // 檢查是否已經按讚
                    var currentStatus = await _likeService.HasLikedAsync(request.ItemType, request.ItemId, userId);
                    if (currentStatus.IsSuccess && currentStatus.Data)
                    {
                        return Conflict(ApiResponse<LikeResponseDto>.Error("已經按過讚了", 409));
                    }

                    // 執行按讚
                    var result = await _likeService.LikeAsync(request.ItemType, request.ItemId, userId);
                    if (!result.IsSuccess)
                    {
                        return BadRequest(ApiResponse<LikeResponseDto>.Error(result.ErrorMessage, 400));
                    }

                    var response = new LikeResponseDto
                    {
                        ItemType = request.ItemType,
                        ItemId = request.ItemId,
                        IsLiked = true,
                        LikeCount = result.Data.LikeCount
                    };

                    return CreatedAtAction(nameof(GetLikeStatus), new { itemType = request.ItemType, itemId = request.ItemId }, 
                        ApiResponse<LikeResponseDto>.Ok(response, "按讚成功"));
                }
                finally
                {
                    semaphore.Release();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "按讚操作失敗：{ItemType} {ItemId}, 用戶：{UserId}", request.ItemType, request.ItemId, userId);
                return StatusCode(500, ApiResponse<LikeResponseDto>.InternalServerError("操作失敗，請稍後再試"));
            }
        }

        /// <summary>
        /// 取消按讚 - RESTful 標準 DELETE 操作
        /// DELETE /api/likes/{itemType}/{itemId}
        /// </summary>
        [HttpDelete("{itemType}/{itemId}")]
        public async Task<IActionResult> DeleteLike(string itemType, int itemId)
        {
            var userId = _currentUserService.UserId;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ApiResponse<LikeResponseDto>.Unauthorized("請先登入"));
            }

            if (string.IsNullOrEmpty(itemType) || itemId <= 0)
            {
                return BadRequest(ApiResponse<LikeResponseDto>.Error("請求參數無效", 400));
            }

            // 建立用戶+項目的唯一鎖定鍵
            var lockKey = $"{userId}_{itemType}_{itemId}";
            var semaphore = _userItemLocks.GetOrAdd(lockKey, _ => new SemaphoreSlim(1, 1));

            try
            {
                var acquired = await semaphore.WaitAsync(TimeSpan.FromSeconds(10));
                if (!acquired)
                {
                    return StatusCode(408, ApiResponse<LikeResponseDto>.Error("操作超時，請稍後再試", 408));
                }

                try
                {
                    // 檢查是否已經按讚
                    var currentStatus = await _likeService.HasLikedAsync(itemType, itemId, userId);
                    if (!currentStatus.IsSuccess || !currentStatus.Data)
                    {
                        return NotFound(ApiResponse<LikeResponseDto>.NotFound("尚未按讚"));
                    }

                    // 執行取消按讚
                    var result = await _likeService.UnlikeAsync(itemType, itemId, userId);
                    if (!result.IsSuccess)
                    {
                        return BadRequest(ApiResponse<LikeResponseDto>.Error(result.ErrorMessage, 400));
                    }

                    var response = new LikeResponseDto
                    {
                        ItemType = itemType,
                        ItemId = itemId,
                        IsLiked = false,
                        LikeCount = result.Data.LikeCount
                    };

                    return Ok(ApiResponse<LikeResponseDto>.Ok(response, "取消按讚成功"));
                }
                finally
                {
                    semaphore.Release();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取消按讚操作失敗：{ItemType} {ItemId}, 用戶：{UserId}", itemType, itemId, userId);
                return StatusCode(500, ApiResponse<LikeResponseDto>.InternalServerError("操作失敗，請稍後再試"));
            }
        }

        /// <summary>
        /// 取得按讚狀態 - RESTful 標準 GET 操作
        /// GET /api/likes/status/{itemType}/{itemId}
        /// </summary>
        [HttpGet("status/{itemType}/{itemId}")]
        public async Task<IActionResult> GetLikeStatus(string itemType, int itemId)
        {
            var userId = _currentUserService.UserId;
            
            if (string.IsNullOrEmpty(itemType) || itemId <= 0)
            {
                return BadRequest(ApiResponse<LikeResponseDto>.Error("請求參數無效", 400));
            }

            try
            {
                bool isLiked = false;
                if (!string.IsNullOrEmpty(userId))
                {
                    var result = await _likeService.HasLikedAsync(itemType, itemId, userId);
                    if (result.IsSuccess)
                    {
                        isLiked = result.Data;
                    }
                }

                // 取得總按讚數
                var countResult = await _likeService.GetLikeCountAsync(itemType, itemId);
                var likeCount = countResult.IsSuccess ? countResult.Data : 0;

                var response = new LikeResponseDto
                {
                    ItemType = itemType,
                    ItemId = itemId,
                    IsLiked = isLiked,
                    LikeCount = likeCount
                };

                return Ok(ApiResponse<LikeResponseDto>.Ok(response, "查詢成功"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "查詢按讚狀態失敗：{ItemType} {ItemId}, 用戶：{UserId}", itemType, itemId, userId);
                return StatusCode(500, ApiResponse<LikeResponseDto>.InternalServerError("查詢失敗，請稍後再試"));
            }
        }

        /// <summary>
        /// 統一按讚切換 API - 使用新的 DTO 回應格式 (向後相容性)
        /// 支援 Post 和 Comment 類型
        /// 加入併發控制避免競態條件
        /// </summary>
        [HttpPost("toggle")]
        [Obsolete("建議使用標準 RESTful API：POST /api/likes 和 DELETE /api/likes")]
        public async Task<IActionResult> ToggleLike([FromBody] LikeToggleRequestDto request)
        {
            // DEBUG LOGS
            _logger.LogInformation("DEBUG: LikesAPIController.ToggleLike received request. ItemType: {ItemType}, ItemId: {ItemId}",
                request?.ItemType ?? "NULL", request?.ItemId ?? 0);
            _logger.LogInformation("DEBUG: LikesAPIController.ToggleLike ModelState.IsValid: {IsValid}", ModelState.IsValid);
            
            if (!ModelState.IsValid)
            {
                foreach (var modelStateEntry in ModelState.Values)
                {
                    foreach (var error in modelStateEntry.Errors)
                    {
                        _logger.LogError("DEBUG: LikesAPIController.ToggleLike ModelState Error: {ErrorMessage}", error.ErrorMessage);
                    }
                }
            }

            var userId = _currentUserService.UserId;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ApiResponse<LikeResponseDto>.Unauthorized("請先登入"));
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<LikeResponseDto>.Error("請求參數無效", 400));
            }

            // 建立用戶+項目的唯一鎖定鍵
            var lockKey = $"{userId}_{request.ItemType}_{request.ItemId}";
            var semaphore = _userItemLocks.GetOrAdd(lockKey, _ => new SemaphoreSlim(1, 1));

            try
            {
                // 等待鎖定（最多 10 秒）
                var acquired = await semaphore.WaitAsync(TimeSpan.FromSeconds(10));
                if (!acquired)
                {
                    _logger.LogWarning("用戶 {UserId} 對 {ItemType} {ItemId} 的操作超時", userId, request.ItemType, request.ItemId);
                    return StatusCode(408, ApiResponse<LikeResponseDto>.Error("操作超時，請稍後再試", 408));
                }

                try
                {
                    var result = await _likeService.ToggleLikeAsync(request.ItemType, request.ItemId, userId);

                    if (!result.IsSuccess)
                    {
                        return BadRequest(ApiResponse<LikeResponseDto>.Error(result.ErrorMessage, 400));
                    }

                    _logger.LogInformation("用戶 {UserId} 成功切換 {ItemType} {ItemId} 按讚狀態至 {IsLiked}", 
                        userId, request.ItemType, request.ItemId, result.Data.IsLiked);

                    var response = new LikeResponseDto
                    {
                        ItemType = request.ItemType,
                        ItemId = request.ItemId,
                        IsLiked = result.Data.IsLiked,
                        LikeCount = result.Data.LikeCount
                    };
                    
                    return Ok(ApiResponse<LikeResponseDto>.Ok(response, result.Data.IsLiked ? "按讚成功" : "取消按讚成功"));
                }
                finally
                {
                    semaphore.Release();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "按讚操作失敗：{ItemType} {ItemId}, 用戶：{UserId}", request.ItemType, request.ItemId, userId);
                return StatusCode(500, ApiResponse<LikeResponseDto>.InternalServerError("操作失敗，請稍後再試"));
            }
            finally
            {
                // 清理舊的信號量（避免記憶體洩漏）
                _ = Task.Run(async () => 
                {
                    await Task.Delay(TimeSpan.FromMinutes(5)); // 延遲 5 分鐘後清理
                    if (semaphore.CurrentCount == 1) // 如果沒有等待中的請求
                    {
                        _userItemLocks.TryRemove(lockKey, out _);
                        semaphore.Dispose();
                    }
                });
            }
        }

        /// <summary>
        /// 批量查詢按讚狀態 - 使用新的 DTO
        /// 解決 N+1 查詢問題
        /// </summary>
        [HttpPost("status/batch")]
        public async Task<IActionResult> GetLikeStatusBatch([FromBody] LikeStatusBatchRequestDto request)
        {
            var userId = _currentUserService.UserId;
            if (string.IsNullOrEmpty(userId))
            {
                // 未登入用戶返回全部 false
                var emptyResult = request.Items.ToDictionary(
                    item => $"{item.ItemType}_{item.ItemId}", 
                    item => false);
                return Ok(ApiResponse<Dictionary<string, bool>>.Ok(emptyResult, "查詢成功"));
            }

            try
            {
                // 轉換 DTO 到 Service 需要的格式
                var serviceItems = request.Items.Select(dto => new Services.LikeItem
                {
                    ItemType = dto.ItemType,
                    ItemId = dto.ItemId
                }).ToList();

                var result = await _likeService.GetLikeStatusBatchAsync(serviceItems, userId);
                return Ok(ApiResponse<Dictionary<string, bool>>.Ok(result, "查詢成功"));
            }
            catch (Exception)
            {
                return StatusCode(500, ApiResponse<Dictionary<string, bool>>.InternalServerError("查詢失敗"));
            }
        }

        /// <summary>
        /// 獲取用戶按讚統計 - 使用新的 DTO
        /// 用於側邊欄顯示總獲讚數
        /// </summary>
        [HttpGet("stats")]
        public async Task<IActionResult> GetUserLikeStats()
        {
            var userId = _currentUserService.UserId;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ApiResponse<UserLikeStatsDto>.Unauthorized("請先登入"));
            }

            try
            {
                var stats = await _likeService.GetUserLikeStatsAsync(userId);
                
                var statsDto = new UserLikeStatsDto
                {
                    TotalLikesReceived = stats.TotalLikesReceived,
                    TotalLikesGiven = stats.TotalLikesGiven,
                    PostLikesReceived = stats.PostLikesReceived,
                    CommentLikesReceived = stats.CommentLikesReceived
                };
                
                return Ok(ApiResponse<UserLikeStatsDto>.Ok(statsDto, "統計載入成功"));
            }
            catch (Exception)
            {
                return StatusCode(500, ApiResponse<UserLikeStatsDto>.InternalServerError("獲取統計失敗"));
            }
        }
    }
}