using BoardGameFontier.Models;
using BoardGameFontier.Repostiory;
using BoardGameFontier.Repostiory.Entity.Social;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using BoardGameFontier.Constants;

namespace BoardGameFontier.Services
{
    /// <summary>
    /// 按讚服務實作 - 重構優化版本
    /// 統一處理 Post 和 Comment 按讚，記憶體快取 + 背景同步
    /// </summary>
    public class LikeService : ILikeService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<LikeService> _logger;
        private readonly IMemoryCache _cache;
        private readonly IBackgroundJobClient _backgroundJobs;
        private readonly IServiceProvider _serviceProvider;

        // 快取鍵格式
        private const string USER_LIKES_CACHE_KEY = "user_likes_{0}";
        private const string ITEM_LIKE_COUNT_KEY = "item_likes_{0}_{1}";
        private const string USER_STATS_CACHE_KEY = "user_stats_{0}";

        // 使用統一的常數定義

        public LikeService(
            ApplicationDbContext context,
            ILogger<LikeService> logger,
            IMemoryCache cache,
            IBackgroundJobClient backgroundJobs,
            IServiceProvider serviceProvider)
        {
            _context = context;
            _logger = logger;
            _cache = cache;
            _backgroundJobs = backgroundJobs;
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// 統一按讚切換功能 - 樂觀更新 + 背景同步版本
        /// 🚀 立即更新快取並回應，背景同步資料庫
        /// </summary>
        public async Task<ServiceResult<LikeToggleResult>> ToggleLikeAsync(string itemType, int itemId, string userId)
        {
            try
            {
                // 驗證輸入參數
                if (!IsValidItemType(itemType))
                {
                    return ServiceResult<LikeToggleResult>.Failure($"不支援的項目類型：{itemType}");
                }

                // 1. 檢查項目是否存在（快速檢查）
                var itemExists = await CheckItemExistsAsync(itemType, itemId);
                if (!itemExists)
                {
                    return ServiceResult<LikeToggleResult>.Failure($"{GetItemTypeDisplayName(itemType)}不存在");
                }

                // 2. 🚀 快速查詢當前按讚狀態（優先使用快取）
                var userLikesKey = string.Format(USER_LIKES_CACHE_KEY, userId);
                var likeKey = $"{itemType}_{itemId}";
                bool isCurrentlyLiked;

                if (_cache.TryGetValue(userLikesKey, out HashSet<string>? userLikes) && userLikes != null)
                {
                    // 從快取獲取狀態
                    isCurrentlyLiked = userLikes.Contains(likeKey);
                }
                else
                {
                    // 快取未命中，快速查詢資料庫
                    var existingLike = await _context.Likes
                        .AsNoTracking()
                        .FirstOrDefaultAsync(l => l.UserId == userId && 
                                                l.ItemType == itemType && 
                                                l.ItemId == itemId);
                    isCurrentlyLiked = existingLike != null;
                }

                bool newLikedState = !isCurrentlyLiked;

                // 3. 🚀 立即更新快取（樂觀更新）
                var currentCount = await GetItemLikeCountFromCacheAsync(itemType, itemId);
                var optimisticNewCount = currentCount + (newLikedState ? 1 : -1);
                optimisticNewCount = Math.Max(0, optimisticNewCount); // 確保不為負數

                // 立即更新用戶按讚狀態快取
                await UpdateCacheAfterDatabaseChangeAsync(userId, itemType, itemId, newLikedState, optimisticNewCount);

                // 4. 🔥 背景執行資料庫同步（Hangfire）
                _backgroundJobs.Enqueue(() => SyncLikeToDatabase(itemType, itemId, userId, newLikedState));

                // 5. 清除用戶統計快取（延遲更新）
                _cache.Remove(string.Format(USER_STATS_CACHE_KEY, userId));

                // 6. 🚀 立即回傳樂觀結果
                var result = new LikeToggleResult
                {
                    IsLiked = newLikedState,
                    LikeCount = optimisticNewCount
                };

                _logger.LogInformation("用戶 {UserId} 樂觀 {Action} {ItemType} {ItemId}，預期按讚數：{Count}",
                    userId, newLikedState ? "按讚" : "取消按讚", itemType, itemId, optimisticNewCount);

                return ServiceResult<LikeToggleResult>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "按讚切換失敗：{ItemType} {ItemId}, 用戶：{UserId}", itemType, itemId, userId);
                return ServiceResult<LikeToggleResult>.Failure("操作失敗，請稍後再試");
            }
        }

        /// <summary>
        /// 批量查詢用戶按讚狀態 - 解決 N+1 問題，性能優化版本
        /// </summary>
        public async Task<Dictionary<string, bool>> GetLikeStatusBatchAsync(List<LikeItem> items, string userId)
        {
            var result = new Dictionary<string, bool>();

            try
            {
                // ✅ 效能優化：如果沒有項目，直接返回空字典
                if (!items.Any())
                {
                    return result;
                }

                // ✅ 效能優化：嘗試從快取獲取，若失敗則直接查詢資料庫
                var userLikesKey = string.Format(USER_LIKES_CACHE_KEY, userId);
                HashSet<string>? userLikes = null;

                if (_cache.TryGetValue(userLikesKey, out HashSet<string>? cached) && cached != null)
                {
                    userLikes = cached;
                }
                else
                {
                    // ✅ 快取策略優化：載入用戶全部按讚記錄，提升後續查詢效率
                    var allUserLikes = await _context.Likes
                        .AsNoTracking()
                        .Where(l => l.UserId == userId)
                        .Select(l => $"{l.ItemType}_{l.ItemId}")
                        .ToListAsync();

                    userLikes = new HashSet<string>(allUserLikes);

                    // ✅ 快取用戶全部按讚記錄，提升命中率
                    _cache.Set(userLikesKey, userLikes, TimeSpan.FromMinutes(30));
                }

                // ✅ 批量處理結果組裝
                foreach (var item in items)
                {
                    var key = $"{item.ItemType}_{item.ItemId}";
                    result[key] = userLikes.Contains(key);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "批量查詢按讚狀態失敗，用戶：{UserId}", userId);
                
                // 降級：回傳全部 false
                foreach (var item in items)
                {
                    result[$"{item.ItemType}_{item.ItemId}"] = false;
                }
                return result;
            }
        }

        /// <summary>
        /// 批量查詢項目按讚數量 - 性能優化版本
        /// </summary>
        public async Task<Dictionary<string, int>> GetLikeCountsBatchAsync(List<LikeItem> items)
        {
            var result = new Dictionary<string, int>();

            try
            {
                // ✅ 效能優化：如果沒有項目，直接返回空字典
                if (!items.Any())
                {
                    return result;
                }

                var cachedItems = new List<string>();
                var uncachedItems = new List<LikeItem>();

                // ✅ 效能優化：批量檢查快取狀態
                foreach (var item in items)
                {
                    var cacheKey = string.Format(ITEM_LIKE_COUNT_KEY, item.ItemType, item.ItemId);
                    var itemKey = $"{item.ItemType}_{item.ItemId}";

                    if (_cache.TryGetValue(cacheKey, out int cached))
                    {
                        result[itemKey] = cached;
                        cachedItems.Add(itemKey);
                    }
                    else
                    {
                        uncachedItems.Add(item);
                    }
                }

                // ✅ 效能優化：只對快取未命中的項目查詢資料庫，並且一次查詢所有
                if (uncachedItems.Any())
                {
                    var uncachedCounts = await GetLikeCountsBatchFromDatabaseOptimizedAsync(uncachedItems);
                    
                    // 合併結果並更新快取
                    foreach (var kvp in uncachedCounts)
                    {
                        result[kvp.Key] = kvp.Value;
                        
                        // 更新快取
                        var parts = kvp.Key.Split('_');
                        if (parts.Length == 2)
                        {
                            var cacheKey = string.Format(ITEM_LIKE_COUNT_KEY, parts[0], parts[1]);
                            _cache.Set(cacheKey, kvp.Value, TimeSpan.FromMinutes(15));
                        }
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "批量查詢按讚數失敗");
                
                // 降級：從資料庫查詢
                return await GetLikeCountsBatchFromDatabaseAsync(items);
            }
        }

        /// <summary>
        /// 獲取用戶按讚統計
        /// </summary>
        public async Task<UserLikeStats> GetUserLikeStatsAsync(string userId)
        {
            var cacheKey = string.Format(USER_STATS_CACHE_KEY, userId);
            
            if (_cache.TryGetValue(cacheKey, out UserLikeStats? cachedStats) && cachedStats != null)
            {
                return cachedStats;
            }

            try
            {
                // 序列化查詢以避免 DbContext 線程安全問題
                var postLikesReceived = await GetUserPostLikesReceivedAsync(userId);
                var commentLikesReceived = await GetUserCommentLikesReceivedAsync(userId);
                var totalLikesGiven = await GetUserTotalLikesGivenAsync(userId);

                var stats = new UserLikeStats
                {
                    PostLikesReceived = postLikesReceived,
                    CommentLikesReceived = commentLikesReceived,
                    TotalLikesReceived = postLikesReceived + commentLikesReceived,
                    TotalLikesGiven = totalLikesGiven
                };

                // 快取統計結果（10分鐘）
                _cache.Set(cacheKey, stats, TimeSpan.FromMinutes(10));

                return stats;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取用戶統計失敗，用戶：{UserId}", userId);
                return new UserLikeStats();
            }
        }

        /// <summary>
        /// 檢查單個項目的按讚狀態
        /// </summary>
        public async Task<ServiceResult<bool>> HasLikedAsync(string itemType, int itemId, string userId)
        {
            try
            {
                var items = new List<LikeItem> { new LikeItem { ItemType = itemType, ItemId = itemId } };
                var results = await GetLikeStatusBatchAsync(items, userId);
                var key = $"{itemType}_{itemId}";
                return ServiceResult<bool>.Success(results.GetValueOrDefault(key, false));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "檢查按讚狀態失敗：{ItemType} {ItemId}, 用戶：{UserId}", itemType, itemId, userId);
                return ServiceResult<bool>.Failure("查詢失敗");
            }
        }

        #region Hangfire 背景同步方法

        /// <summary>
        /// 背景同步按讚狀態到資料庫 - 增強版本
        /// 🔥 支援自動修正快取不一致問題
        /// </summary>
        [AutomaticRetry(Attempts = 3)]
        public async Task SyncLikeToDatabase(string itemType, int itemId, string userId, bool shouldLike)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var cache = scope.ServiceProvider.GetRequiredService<IMemoryCache>();

            try
            {
                // 查詢現有的按讚記錄
                var existingLike = await context.Likes
                    .FirstOrDefaultAsync(l => l.UserId == userId && 
                                            l.ItemType == itemType && 
                                            l.ItemId == itemId);

                bool actualDbState = existingLike != null;
                bool needsDbUpdate = false;

                if (shouldLike && !actualDbState)
                {
                    // 新增按讚記錄
                    context.Likes.Add(new Like
                    {
                        UserId = userId,
                        ItemType = itemType,
                        ItemId = itemId,
                        CreatedAt = DateTime.Now
                    });

                    // 更新項目按讚數
                    await UpdateItemLikeCountAsync(context, itemType, itemId, 1);
                    needsDbUpdate = true;
                }
                else if (!shouldLike && actualDbState)
                {
                    // 移除按讚記錄
                    context.Likes.Remove(existingLike!);

                    // 更新項目按讚數
                    await UpdateItemLikeCountAsync(context, itemType, itemId, -1);
                    needsDbUpdate = true;
                }
                else
                {
                    // 狀態已一致，可能是重複請求或快取問題
                    _logger.LogWarning("背景同步發現狀態不一致：預期 {Expected}，實際 {Actual}，項目：{ItemType} {ItemId}，用戶：{UserId}",
                        shouldLike, actualDbState, itemType, itemId, userId);
                }

                if (needsDbUpdate)
                {
                    await context.SaveChangesAsync();
                    _logger.LogDebug("✅ 背景同步資料庫成功：{ItemType} {ItemId}, 用戶：{UserId}, 狀態：{ShouldLike}",
                        itemType, itemId, userId, shouldLike);
                }

                // 🔧 同步後修正快取，確保與資料庫一致
                var actualCount = await GetActualLikeCountFromDatabaseAsync(context, itemType, itemId);
                await UpdateCacheWithActualDataAsync(cache, userId, itemType, itemId, shouldLike, actualCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ 背景同步資料庫失敗：{ItemType} {ItemId}, 用戶：{UserId}", itemType, itemId, userId);
                throw; // 讓 Hangfire 重試
            }
        }

        #endregion

        #region 私有輔助方法

        private static bool IsValidItemType(string itemType)
        {
            return LikeConstants.IsValidItemType(itemType);
        }

        private static string GetItemTypeDisplayName(string itemType)
        {
            return itemType switch
            {
                LikeConstants.ItemTypes.Post => "貼文",
                LikeConstants.ItemTypes.Comment => "留言",
                _ => "項目"
            };
        }

        private async Task<bool> CheckItemExistsAsync(string itemType, int itemId)
        {
            return itemType switch
            {
                LikeConstants.ItemTypes.Post => await _context.Posts.AnyAsync(p => p.Id == itemId),
                LikeConstants.ItemTypes.Comment => await _context.Comments.AnyAsync(c => c.Id == itemId),
                _ => false
            };
        }

        private async Task<HashSet<string>> GetUserLikesFromCacheAsync(string cacheKey, string userId)
        {
            if (_cache.TryGetValue(cacheKey, out HashSet<string>? cached) && cached != null)
            {
                return cached;
            }

            // 從資料庫載入用戶按讚記錄
            var likeKeys = await _context.Likes
                .Where(l => l.UserId == userId)
                .Select(l => $"{l.ItemType}_{l.ItemId}")
                .ToListAsync();

            var userLikes = new HashSet<string>(likeKeys);
            _cache.Set(cacheKey, userLikes, TimeSpan.FromMinutes(30));

            return userLikes;
        }

        private async Task<int> GetItemLikeCountFromCacheAsync(string itemType, int itemId)
        {
            var cacheKey = string.Format(ITEM_LIKE_COUNT_KEY, itemType, itemId);
            
            if (_cache.TryGetValue(cacheKey, out int cached))
            {
                return cached;
            }

            // 從資料庫查詢實際按讚數
            int actualCount = itemType switch
            {
                LikeConstants.ItemTypes.Post => await _context.Posts
                    .Where(p => p.Id == itemId)
                    .Select(p => p.LikeCount)
                    .FirstOrDefaultAsync(),
                LikeConstants.ItemTypes.Comment => await _context.Comments
                    .Where(c => c.Id == itemId)
                    .Select(c => c.LikeCount)
                    .FirstOrDefaultAsync(),
                _ => 0
            };

            _cache.Set(cacheKey, actualCount, TimeSpan.FromMinutes(15));
            return actualCount;
        }

        /// <summary>
        /// ✅ 線程安全版本：序列化查詢資料庫按讚數
        /// </summary>
        private async Task<Dictionary<string, int>> GetLikeCountsBatchFromDatabaseOptimizedAsync(List<LikeItem> items)
        {
            var result = new Dictionary<string, int>();

            var postIds = items.Where(i => i.ItemType == LikeConstants.ItemTypes.Post).Select(i => i.ItemId).ToList();
            var commentIds = items.Where(i => i.ItemType == LikeConstants.ItemTypes.Comment).Select(i => i.ItemId).ToList();

            // ✅ 序列化查詢確保線程安全
            if (postIds.Any())
            {
                var postCounts = await _context.Posts
                    .AsNoTracking()
                    .Where(p => postIds.Contains(p.Id))
                    .Select(p => new { p.Id, p.LikeCount })
                    .ToListAsync();

                foreach (var post in postCounts)
                {
                    result[$"Post_{post.Id}"] = post.LikeCount;
                }
            }

            if (commentIds.Any())
            {
                var commentCounts = await _context.Comments
                    .AsNoTracking()
                    .Where(c => commentIds.Contains(c.Id))
                    .Select(c => new { c.Id, c.LikeCount })
                    .ToListAsync();

                foreach (var comment in commentCounts)
                {
                    result[$"Comment_{comment.Id}"] = comment.LikeCount;
                }
            }

            return result;
        }

        private async Task<Dictionary<string, int>> GetLikeCountsBatchFromDatabaseAsync(List<LikeItem> items)
        {
            var result = new Dictionary<string, int>();

            var postIds = items.Where(i => i.ItemType == LikeConstants.ItemTypes.Post).Select(i => i.ItemId).ToList();
            var commentIds = items.Where(i => i.ItemType == LikeConstants.ItemTypes.Comment).Select(i => i.ItemId).ToList();

            if (postIds.Any())
            {
                var postCounts = await _context.Posts
                    .Where(p => postIds.Contains(p.Id))
                    .Select(p => new { p.Id, p.LikeCount })
                    .ToListAsync();

                foreach (var post in postCounts)
                {
                    result[$"Post_{post.Id}"] = post.LikeCount;
                }
            }

            if (commentIds.Any())
            {
                var commentCounts = await _context.Comments
                    .Where(c => commentIds.Contains(c.Id))
                    .Select(c => new { c.Id, c.LikeCount })
                    .ToListAsync();

                foreach (var comment in commentCounts)
                {
                    result[$"Comment_{comment.Id}"] = comment.LikeCount;
                }
            }

            return result;
        }

        private async Task UpdateItemLikeCountAsync(ApplicationDbContext context, string itemType, int itemId, int increment)
        {
            if (itemType == LikeConstants.ItemTypes.Post)
            {
                // 使用條件表達式替代 Math.Max，EF Core 可以翻譯成 SQL
                await context.Posts
                    .Where(p => p.Id == itemId)
                    .ExecuteUpdateAsync(p => p.SetProperty(x => x.LikeCount, 
                        x => (x.LikeCount + increment) < 0 ? 0 : (x.LikeCount + increment)));
            }
            else if (itemType == LikeConstants.ItemTypes.Comment)
            {
                // 使用條件表達式替代 Math.Max，EF Core 可以翻譯成 SQL
                await context.Comments
                    .Where(c => c.Id == itemId)
                    .ExecuteUpdateAsync(c => c.SetProperty(x => x.LikeCount, 
                        x => (x.LikeCount + increment) < 0 ? 0 : (x.LikeCount + increment)));
            }
        }

        private async Task RefreshItemCacheAsync(string itemType, int itemId)
        {
            var cacheKey = string.Format(ITEM_LIKE_COUNT_KEY, itemType, itemId);
            _cache.Remove(cacheKey);
            
            // 預載入新的計數
            await GetItemLikeCountFromCacheAsync(itemType, itemId);
        }

        private async Task<int> GetUserPostLikesReceivedAsync(string userId)
        {
            return await _context.Posts
                .Where(p => p.AuthorId == userId)
                .SumAsync(p => p.LikeCount);
        }

        private async Task<int> GetUserCommentLikesReceivedAsync(string userId)
        {
            return await _context.Comments
                .Where(c => c.AuthorId == userId)
                .SumAsync(c => c.LikeCount);
        }

        private async Task<int> GetUserTotalLikesGivenAsync(string userId)
        {
            return await _context.Likes
                .Where(l => l.UserId == userId)
                .CountAsync();
        }

        /// <summary>
        /// 從資料庫取得項目的實際按讚數
        /// </summary>
        private async Task<int> GetActualLikeCountFromDatabaseAsync(string itemType, int itemId)
        {
            return itemType switch
            {
                LikeConstants.ItemTypes.Post => await _context.Posts
                    .Where(p => p.Id == itemId)
                    .Select(p => p.LikeCount)
                    .FirstOrDefaultAsync(),
                LikeConstants.ItemTypes.Comment => await _context.Comments
                    .Where(c => c.Id == itemId)
                    .Select(c => c.LikeCount)
                    .FirstOrDefaultAsync(),
                _ => 0
            };
        }

        /// <summary>
        /// 從指定的 DbContext 取得項目的實際按讚數（用於背景任務）
        /// </summary>
        private async Task<int> GetActualLikeCountFromDatabaseAsync(ApplicationDbContext context, string itemType, int itemId)
        {
            return itemType switch
            {
                LikeConstants.ItemTypes.Post => await context.Posts
                    .Where(p => p.Id == itemId)
                    .Select(p => p.LikeCount)
                    .FirstOrDefaultAsync(),
                LikeConstants.ItemTypes.Comment => await context.Comments
                    .Where(c => c.Id == itemId)
                    .Select(c => c.LikeCount)
                    .FirstOrDefaultAsync(),
                _ => 0
            };
        }

        /// <summary>
        /// 使用實際資料庫資料更新快取（用於背景同步修正）
        /// </summary>
        private async Task UpdateCacheWithActualDataAsync(IMemoryCache cache, string userId, string itemType, int itemId, bool isLiked, int actualCount)
        {
            try
            {
                // 更新用戶按讚狀態快取
                var userLikesKey = string.Format(USER_LIKES_CACHE_KEY, userId);
                var userLikes = await GetUserLikesFromCacheAsync(userLikesKey, userId);
                var likeKey = $"{itemType}_{itemId}";

                if (isLiked && !userLikes.Contains(likeKey))
                {
                    userLikes.Add(likeKey);
                }
                else if (!isLiked && userLikes.Contains(likeKey))
                {
                    userLikes.Remove(likeKey);
                }

                cache.Set(userLikesKey, userLikes, TimeSpan.FromMinutes(30));

                // 更新項目按讚數快取
                var countKey = string.Format(ITEM_LIKE_COUNT_KEY, itemType, itemId);
                cache.Set(countKey, actualCount, TimeSpan.FromMinutes(15));

                _logger.LogDebug("🔧 背景任務已修正快取：{ItemType} {ItemId}，實際按讚數：{ActualCount}", itemType, itemId, actualCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "背景更新快取失敗：{ItemType} {ItemId}", itemType, itemId);
            }
        }

        /// <summary>
        /// 資料庫變更後更新快取
        /// </summary>
        private async Task UpdateCacheAfterDatabaseChangeAsync(string userId, string itemType, int itemId, bool isLiked, int newCount)
        {
            // 更新用戶按讚狀態快取
            var userLikesKey = string.Format(USER_LIKES_CACHE_KEY, userId);
            var userLikes = await GetUserLikesFromCacheAsync(userLikesKey, userId);
            var likeKey = $"{itemType}_{itemId}";

            if (isLiked)
            {
                userLikes.Add(likeKey);
            }
            else
            {
                userLikes.Remove(likeKey);
            }

            _cache.Set(userLikesKey, userLikes, TimeSpan.FromMinutes(30));

            // 更新項目按讚數快取
            var countKey = string.Format(ITEM_LIKE_COUNT_KEY, itemType, itemId);
            _cache.Set(countKey, newCount, TimeSpan.FromMinutes(15));
        }

        #endregion

        #region RESTful API Methods

        /// <summary>
        /// 建立按讚 - RESTful API 專用
        /// </summary>
        public async Task<ServiceResult<LikeToggleResult>> LikeAsync(string itemType, int itemId, string userId)
        {
            try
            {
                // 先檢查是否已經按讚
                var existingResult = await HasLikedAsync(itemType, itemId, userId);
                if (!existingResult.IsSuccess)
                {
                    return ServiceResult<LikeToggleResult>.Failure(existingResult.ErrorMessage);
                }

                if (existingResult.Data)
                {
                    // 已經按讚，回傳錯誤
                    return ServiceResult<LikeToggleResult>.Failure("已經按過讚了");
                }

                // 執行按讚操作（重用 ToggleLikeAsync 的邏輯）
                return await ToggleLikeAsync(itemType, itemId, userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "建立按讚失敗：{ItemType} {ItemId}, 用戶：{UserId}", itemType, itemId, userId);
                return ServiceResult<LikeToggleResult>.Failure("按讚操作失敗");
            }
        }

        /// <summary>
        /// 取消按讚 - RESTful API 專用
        /// </summary>
        public async Task<ServiceResult<LikeToggleResult>> UnlikeAsync(string itemType, int itemId, string userId)
        {
            try
            {
                // 先檢查是否已經按讚
                var existingResult = await HasLikedAsync(itemType, itemId, userId);
                if (!existingResult.IsSuccess)
                {
                    return ServiceResult<LikeToggleResult>.Failure(existingResult.ErrorMessage);
                }

                if (!existingResult.Data)
                {
                    // 尚未按讚，回傳錯誤
                    return ServiceResult<LikeToggleResult>.Failure("尚未按讚");
                }

                // 執行取消按讚操作（重用 ToggleLikeAsync 的邏輯）
                return await ToggleLikeAsync(itemType, itemId, userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取消按讚失敗：{ItemType} {ItemId}, 用戶：{UserId}", itemType, itemId, userId);
                return ServiceResult<LikeToggleResult>.Failure("取消按讚操作失敗");
            }
        }

        /// <summary>
        /// 取得單一項目按讚數量 - RESTful API 專用
        /// </summary>
        public async Task<ServiceResult<int>> GetLikeCountAsync(string itemType, int itemId)
        {
            try
            {
                var cacheKey = string.Format(ITEM_LIKE_COUNT_KEY, itemType, itemId);
                
                // 先檢查快取
                if (_cache.TryGetValue(cacheKey, out int cachedCount))
                {
                    return ServiceResult<int>.Success(cachedCount);
                }

                // 從資料庫查詢
                int actualCount = 0;
                
                if (itemType == LikeConstants.ItemTypes.Post)
                {
                    var post = await _context.Posts
                        .AsNoTracking()
                        .Where(p => p.Id == itemId)
                        .Select(p => new { p.LikeCount })
                        .FirstOrDefaultAsync();
                    
                    actualCount = post?.LikeCount ?? 0;
                }
                else if (itemType == LikeConstants.ItemTypes.Comment)
                {
                    var comment = await _context.Comments
                        .AsNoTracking()
                        .Where(c => c.Id == itemId)
                        .Select(c => new { c.LikeCount })
                        .FirstOrDefaultAsync();
                    
                    actualCount = comment?.LikeCount ?? 0;
                }

                // 更新快取
                _cache.Set(cacheKey, actualCount, TimeSpan.FromMinutes(15));

                return ServiceResult<int>.Success(actualCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "查詢按讚數失敗：{ItemType} {ItemId}", itemType, itemId);
                return ServiceResult<int>.Failure("查詢按讚數失敗");
            }
        }

        #endregion
    }
}