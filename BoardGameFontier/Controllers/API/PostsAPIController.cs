using BoardGameFontier.Models.ViewModels;
using BoardGameFontier.Models.Common;
using BoardGameFontier.Repostiory.Entity.Social;
using BoardGameFontier.Repostiory.DTOS.Social;
using BoardGameFontier.Services;
using BoardGameFontier.Extensions;
using Hangfire;
using Microsoft.AspNetCore.Mvc;

namespace BoardGameFontier.Controllers.API
{
    /// <summary>
    /// 貼文相關 API Controller - 支援 AJAX 動態載入
    /// 
    /// 筆記：
    /// - 這是社群功能的核心 API，處理所有貼文相關的前端請求
    /// - [ApiController] 自動啟用模型驗證和統一錯誤回應
    /// - 使用依賴注入模式，讓程式碼更好測試和維護
    /// - 統一使用 ApiResponse<T> 格式，確保前端處理一致性
    /// </summary>
    [Route("api/posts")]
    [ApiController]
    public class PostsAPIController : ControllerBase
    {
        // 依賴注入：把需要的服務都注入進來，而不是在方法內 new 物件
        private readonly IPostService _postService;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILikeService _likeService;
        private readonly ICommunityService _communityService;
        private readonly IFollowService _followService;
        private readonly ILogger<PostsAPIController> _logger;

        public PostsAPIController(IPostService postService,
            ICurrentUserService currentUserService,
            ILikeService likeService,
            ICommunityService communityService,
            IFollowService followService,
            ILogger<PostsAPIController> logger)
        {
            _postService = postService;
            _currentUserService = currentUserService;
            _likeService = likeService;
            _communityService = communityService;
            _followService = followService;
            _logger = logger;
        }

        /// <summary>
        /// 取得貼文列表 (支援分頁、篩選、搜尋) - 使用統一 API 回應格式
        /// 
        /// 筆記：
        /// - [FromQuery] 自動把 URL 參數 ?page=1&pageSize=6 綁定到方法參數
        /// - 使用批次查詢避免 N+1 查詢問題，大幅提升效能
        /// - 支援按貼文類型篩選和關鍵字搜尋
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetPosts([FromQuery] int page = 1, [FromQuery] int pageSize = 6, [FromQuery] PostType? postType = null, [FromQuery] string? searchKeyword = null)
        {
            try
            {
                // 建立安全的分頁參數，避免惡意的超大數字
                var pagedRequest = PagedRequest.CreateSafe(page, pageSize);

                // 從資料庫取得分頁貼文
                var pagedResult = await _postService.GetPostsPagedAsync(pagedRequest, postType, searchKeyword);
                var currentUserId = _currentUserService.UserId;

                // 效能優化：批次查詢按讚狀態和追蹤狀態，避免 N+1 查詢問題
                Dictionary<string, bool> likeStatuses = new();
                Dictionary<string, bool> followStatuses = new();

                if (pagedResult.Items.Any() && !string.IsNullOrEmpty(currentUserId))
                {
                    var postIds = pagedResult.Items.Select(p => p.Id).ToArray();
                    var likeItems = postIds.Select(id => new Services.LikeItem { ItemType = "Post", ItemId = id }).ToList();
                    var authorIds = pagedResult.Items.Select(p => p.AuthorId).Distinct().ToList();

                    // 並行處理：兩個查詢同時進行，不用等第一個完成才開始第二個
                    var likeStatusTask = _likeService.GetLikeStatusBatchAsync(likeItems, currentUserId);
                    var followStatusTask = _followService.GetFollowStatusBatchAsync(currentUserId, authorIds);

                    await Task.WhenAll(likeStatusTask, followStatusTask);  // 等所有查詢完成

                    likeStatuses = await likeStatusTask;
                    followStatuses = await followStatusTask;
                }

                // 使用新的 DTO 轉換方法，取代匿名物件
                var postsData = pagedResult.Items.ToPostResponseDtos(likeStatuses, followStatuses);

                // 使用統一分頁格式包裝回應資料
                var responseData = new
                {
                    Posts = postsData,
                    Pagination = new
                    {
                        pagedResult.CurrentPage,
                        pagedResult.PageSize,
                        pagedResult.TotalCount,
                        pagedResult.TotalPages,
                        pagedResult.HasPreviousPage,
                        pagedResult.HasNextPage,
                        pagedResult.StartRecord,
                        pagedResult.EndRecord
                    }
                };

                // 統一回應格式：所有 API 都用 ApiResponse 包裝
                return Ok(ApiResponse<object>.Ok(responseData, "貼文列表載入成功"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "載入貼文列表時發生錯誤");
                return StatusCode(500, ApiResponse<object>.InternalServerError("載入貼文列表時發生錯誤"));
            }
        }

        /// <summary>
        /// 取得當前使用者的貼文列表、統計數據和分頁資訊
        /// </summary>
        [HttpGet("MyPosts")]
        public async Task<IActionResult> GetMyPosts(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 9,
            [FromQuery] PostType? filterType = null,
            [FromQuery] string? searchKeyword = null,
            [FromQuery] PostSortOrder sortOrder = PostSortOrder.CreatedDesc,
            [FromQuery] bool onlyWithNewResponses = false)
        {
            var currentUserId = _currentUserService.UserId;
            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized(ApiResponse<object>.Unauthorized("用戶未登入"));
            }

            try
            {
                var filterOptions = new PostFilterOptions
                {
                    FilterType = filterType,
                    SearchKeyword = searchKeyword,
                    SortOrder = sortOrder,
                    OnlyWithNewResponses = onlyWithNewResponses,
                    PageSize = pageSize // Ensure pageSize is passed to the service
                };

                var myPostsViewModel = await _communityService.GetMyPostsPageDataAsync(currentUserId, filterOptions, page);

                // 將 MyPostsViewModel 轉換為適合 API 回傳的 JSON 格式
                var responseData = new
                {
                    myPosts = myPostsViewModel.MyPosts.Select(p => new
                    {
                        p.Id,
                        p.Title,
                        p.ContentPreview,
                        p.FullContent,
                        p.Type,
                        p.Price,
                        p.TradeLocation,
                        p.TradeNotes,
                        RelatedGame = p.RelatedGame != null ? new { p.RelatedGame.Id, p.RelatedGame.Name } : null,
                        p.CreatedAt,
                        p.UpdatedAt,
                        TimeDisplay = new
                        {
                            p.TimeDisplay.CreatedAgo,
                            p.TimeDisplay.UpdatedAgo,
                            p.TimeDisplay.FormattedCreated,
                            p.TimeDisplay.FormattedUpdated
                        },
                        Stats = new
                        {
                            p.Stats.LikeCount,
                            p.Stats.CommentCount,
                            // ✅ 移除 ViewCount 統計
                            p.Stats.ShareCount
                        },
                        RecentResponses = p.RecentResponses.Select(r => new
                        {
                            r.Id,
                            r.ContentPreview,
                            Author = new { r.Author.Id, r.Author.UserName, r.Author.DisplayName, r.Author.ProfilePictureUrl },
                            r.CreatedAt,
                            r.TimeAgo,
                            r.IsNew
                        }),
                        p.HasNewResponses,
                        p.NewResponseCount,
                        Permissions = new
                        {
                            p.Permissions.CanEdit,
                            p.Permissions.CanDelete,
                            p.Permissions.CanView,
                            p.Permissions.EditRestrictionReason,
                            p.Permissions.DeleteRestrictionReason
                        }
                    }),
                    stats = myPostsViewModel.Stats,
                    pagination = myPostsViewModel.Pagination,
                    filterOptions = myPostsViewModel.FilterOptions,
                    hasData = myPostsViewModel.MyPosts.Any()
                };

                return Ok(ApiResponse<object>.Ok(responseData, "個人貼文載入成功"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "載入個人貼文時發生錯誤");
                return StatusCode(500, ApiResponse<object>.InternalServerError("載入個人貼文時發生錯誤"));
            }
        }

        /// <summary>
        /// 建立新貼文
        /// 
        /// 筆記：
        /// - [HttpPost] 處理 POST 請求，用來建立新資料
        /// - [FromBody] 表示資料從 request body 來（通常是 JSON）
        /// - 展示完整的「驗證→處理→回應」流程
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreatePost([FromBody] PostCreateDto createDto)
        {
            // 第一關：檢查使用者有沒有登入
            var authorId = _currentUserService.UserId;
            if (string.IsNullOrEmpty(authorId))
            {
                return Unauthorized(ApiResponse<object>.Unauthorized("請先登入後再發布貼文"));
            }

            // 第二關：檢查資料格式有沒有錯
            // ModelState 會根據 PostCreateDto 上的 [Required]、[StringLength] 等註解自動驗證
            if (!ModelState.IsValid)
            {
                var validationErrors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .SelectMany(x => x.Value.Errors.Select(e => $"{x.Key}: {e.ErrorMessage}"))
                    .ToList();

                return BadRequest(ApiResponse<object>.ValidationError(validationErrors, "貼文資料驗證失敗，請檢查輸入內容"));
            }

            try
            {
                // 第三步：建立貼文物件
                // 這裡展示如何從 DTO 轉換成 Entity
                var post = new Post
                {
                    Title = createDto.Title,
                    Content = createDto.Content,
                    Type = createDto.Type,
                    GameDetailId = createDto.GameDetailId,
                    Price = createDto.Price,
                    TradeLocation = createDto.TradeLocation,
                    TradeNotes = createDto.TradeNotes,
                    // 🖼️ 新增：圖片URLs欄位
                    ImageUrls = createDto.ImageUrls,
                    AuthorId = authorId,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                // 第四步：呼叫 Service 層處理業務邏輯
                // Controller 不直接操作資料庫，而是透過 Service
                var createdPost = await _postService.CreatePostAsync(post);
                return Ok(ApiResponse<object>.Ok(new { id = createdPost.Id, title = createdPost.Title, createdAt = createdPost.CreatedAt }, "貼文發布成功"));
            }
            catch (Exception ex)
            {
                // 錯誤處理：記錄詳細錯誤（包含使用者ID），但只回傳安全的訊息給前端
                _logger.LogError(ex, "建立貼文時發生錯誤，使用者: {UserId}", authorId);
                return StatusCode(500, ApiResponse<object>.InternalServerError("系統暫時無法處理您的請求，請稍後再試"));
            }
        }

        /// <summary>
        /// 取得貼文詳細資料
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetPostDetail(int id)
        {
            try
            {
                var post = await _postService.GetPostByIdAsync(id);
                if (post == null) return NotFound(ApiResponse<object>.NotFound("找不到指定的貼文"));

                // 檢查目前使用者的按讚和追蹤狀態
                var currentUserId = _currentUserService.UserId;
                var isLiked = false;
                var isFollowed = false;

                if (!string.IsNullOrEmpty(currentUserId))
                {
                    var likeStatus = await _likeService.HasLikedAsync("Post", post.Id, currentUserId);
                    isLiked = likeStatus.IsSuccess && likeStatus.Data;

                    isFollowed = await _followService.IsFollowingAsync(currentUserId, post.AuthorId);
                }

                // 使用 DTO 轉換方法
                var postData = post.ToPostResponseDto(isLiked, isFollowed);

                return Ok(ApiResponse<object>.Ok(postData, "貼文載入成功"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "載入貼文詳情時發生錯誤，PostId: {PostId}", id);
                return StatusCode(500, ApiResponse<object>.InternalServerError("載入貼文時發生錯誤，請稍後再試"));
            }
        }

        /// <summary>
        /// 更新貼文（僅作者可編輯）
        /// </summary>
        /// <param name="id">貼文 ID</param>
        /// <param name="updateDto">更新資料</param>
        /// <returns>更新結果</returns>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdatePost(int id, [FromBody] PostUpdateDto updateDto)
        {
            try
            {
                // 檢查使用者是否已登入
                var currentUserId = _currentUserService.UserId;
                if (string.IsNullOrEmpty(currentUserId))
                {
                    return Unauthorized(ApiResponse<object>.Unauthorized("請先登入後再進行編輯"));
                }

                // 驗證模型
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(x => x.Value.Errors.Count > 0)
                        .SelectMany(x => x.Value.Errors.Select(e => $"{x.Key}: {e.ErrorMessage}"))
                        .ToList();

                    return BadRequest(ApiResponse<object>.ValidationError(errors, "貼文更新資料驗證失敗"));
                }

                // 驗證交易欄位（如果是交易類型）
                if (!updateDto.ValidateTradeFields())
                {
                    return BadRequest(ApiResponse<object>.Error("交易類型貼文必須填寫完整的交易資訊", 422));
                }

                // 執行更新操作（包含權限驗證）
                var result = await _postService.UpdatePostAsync(id, updateDto, currentUserId);

                if (!result.Success)
                {
                    return result.Message.Contains("權限") ?
                        StatusCode(403, ApiResponse<object>.Forbidden(result.Message)) :
                        BadRequest(ApiResponse<object>.Error(result.Message, 422));
                }

                return Ok(ApiResponse<object>.Ok(result.Data ?? new object(), "貼文更新成功"));
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, ApiResponse<object>.Forbidden(ex.Message));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<object>.Error(ex.Message, 422));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "貼文更新時發生未預期錯誤，PostId: {PostId}", id);
                return StatusCode(500, ApiResponse<object>.InternalServerError("系統暫時無法處理您的請求，請稍後再試"));
            }
        }

        /// <summary>
        /// 刪除貼文（僅作者可刪除）
        /// </summary>
        /// <param name="id">貼文 ID</param>
        /// <returns>刪除結果</returns>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeletePost(int id)
        {
            try
            {
                var currentUserId = _currentUserService.UserId;
                if (string.IsNullOrEmpty(currentUserId))
                {
                    return Unauthorized(ApiResponse<object>.Unauthorized("請先登入後再進行刪除"));
                }

                // 新增：判斷是否為管理員
                var isAdmin = User.IsInRole("Admin");

                // 改用新簽章，把 isAdmin 傳進去
                var result = await _postService.DeletePostAsync(id, currentUserId, isAdmin);

                if (!result.Success)
                {
                    if (result.Message.Contains("找不到"))
                    {
                        return NotFound(ApiResponse<object>.NotFound(result.Message));
                    }
                    if (result.Message.Contains("權限"))
                    {
                        return StatusCode(403, ApiResponse<object>.Forbidden(result.Message));
                    }
                    return BadRequest(ApiResponse<object>.Error(result.Message, 422));
                }

                return Ok(ApiResponse<object>.Ok(null, "貼文刪除成功"));
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, ApiResponse<object>.Forbidden(ex.Message));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<object>.Error(ex.Message, 422));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "貼文刪除時發生未預期錯誤，PostId: {PostId}", id);
                return StatusCode(500, ApiResponse<object>.InternalServerError("系統暫時無法處理您的請求，請稍後再試"));
            }
        }
    }
}
