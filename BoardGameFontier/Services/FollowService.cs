using BoardGameFontier.Repostiory;
using BoardGameFontier.Repostiory.Entity;
using BoardGameFontier.Repostiory.Entity.Social;
using Microsoft.EntityFrameworkCore;

namespace BoardGameFontier.Services
{
    /// <summary>
    /// 追蹤服務實作
    /// 負責處理用戶之間的追蹤關係
    /// </summary>
    public class FollowService : IFollowService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        private readonly ILogger<FollowService> _logger;
        private readonly ICacheService _cacheService;

        public FollowService(
            IDbContextFactory<ApplicationDbContext> contextFactory, 
            ILogger<FollowService> logger,
            ICacheService cacheService)
        {
            _contextFactory = contextFactory;
            _logger = logger;
            _cacheService = cacheService;
        }

        /// <summary>
        /// 追蹤指定用戶
        /// </summary>
        public async Task<bool> FollowUserAsync(string followerId, string followeeId)
        {
            try
            {
                // 驗證參數
                if (string.IsNullOrEmpty(followerId) || string.IsNullOrEmpty(followeeId))
                    return false;

                // 不能追蹤自己
                if (followerId == followeeId)
                    return false;

                using var context = _contextFactory.CreateDbContext();

                // 檢查是否已經追蹤
                var existingFollow = await context.Set<Follow>()
                    .FirstOrDefaultAsync(f => f.FollowerId == followerId && f.FolloweeId == followeeId);

                if (existingFollow != null)
                    return true; // 已經追蹤，返回成功

                // 驗證用戶是否存在
                var follower = await context.Set<UserProfile>().FindAsync(followerId);
                var followee = await context.Set<UserProfile>().FindAsync(followeeId);

                if (follower == null || followee == null)
                {
                    _logger.LogWarning("追蹤操作失敗：用戶不存在。FollowerId: {FollowerId}, FolloweeId: {FolloweeId}", 
                        followerId, followeeId);
                    return false;
                }

                // 建立追蹤關係
                var follow = new Follow
                {
                    FollowerId = followerId,
                    FolloweeId = followeeId,
                    CreatedAt = DateTime.UtcNow  // 使用 UTC 時間確保跨時區一致性
                };

                context.Set<Follow>().Add(follow);
                var result = await context.SaveChangesAsync();

                if (result > 0)
                {
                    // 清除相關的快取
                    await ClearFollowRelatedCacheAsync(followerId, followeeId);
                }

                _logger.LogInformation("用戶追蹤成功：{FollowerId} 追蹤 {FolloweeId}", followerId, followeeId);
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "追蹤用戶時發生錯誤：{FollowerId} 追蹤 {FolloweeId}", followerId, followeeId);
                return false;
            }
        }

        /// <summary>
        /// 取消追蹤指定用戶
        /// </summary>
        public async Task<bool> UnfollowUserAsync(string followerId, string followeeId)
        {
            try
            {
                // 驗證參數
                if (string.IsNullOrEmpty(followerId) || string.IsNullOrEmpty(followeeId))
                    return false;

                using var context = _contextFactory.CreateDbContext();

                // 查找追蹤關係
                var follow = await context.Set<Follow>()
                    .FirstOrDefaultAsync(f => f.FollowerId == followerId && f.FolloweeId == followeeId);

                if (follow == null)
                    return true; // 沒有追蹤關係，返回成功

                // 刪除追蹤關係
                context.Set<Follow>().Remove(follow);
                var result = await context.SaveChangesAsync();

                if (result > 0)
                {
                    // 清除相關的快取
                    await ClearFollowRelatedCacheAsync(followerId, followeeId);
                }

                _logger.LogInformation("取消追蹤成功：{FollowerId} 取消追蹤 {FolloweeId}", followerId, followeeId);
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取消追蹤時發生錯誤：{FollowerId} 取消追蹤 {FolloweeId}", followerId, followeeId);
                return false;
            }
        }

        /// <summary>
        /// 檢查是否已追蹤指定用戶
        /// </summary>
        public async Task<bool> IsFollowingAsync(string followerId, string followeeId)
        {
            try
            {
                if (string.IsNullOrEmpty(followerId) || string.IsNullOrEmpty(followeeId))
                    return false;

                using var context = _contextFactory.CreateDbContext();
                return await context.Set<Follow>()
                    .AnyAsync(f => f.FollowerId == followerId && f.FolloweeId == followeeId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "檢查追蹤狀態時發生錯誤：{FollowerId}, {FolloweeId}", followerId, followeeId);
                return false;
            }
        }

        /// <summary>
        /// 批次檢查追蹤狀態
        /// </summary>
        public async Task<Dictionary<string, bool>> GetFollowStatusBatchAsync(string followerId, List<string> followeeIds)
        {
            var result = new Dictionary<string, bool>();

            try
            {
                if (string.IsNullOrEmpty(followerId) || followeeIds == null || !followeeIds.Any())
                    return result;

                using var context = _contextFactory.CreateDbContext();

                // 查詢該用戶的所有追蹤關係
                var follows = await context.Set<Follow>()
                    .Where(f => f.FollowerId == followerId && followeeIds.Contains(f.FolloweeId))
                    .Select(f => f.FolloweeId)
                    .ToListAsync();

                // 建立結果字典
                foreach (var followeeId in followeeIds)
                {
                    result[followeeId] = follows.Contains(followeeId);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "批次檢查追蹤狀態時發生錯誤：{FollowerId}", followerId);
                return result;
            }
        }

        /// <summary>
        /// 取得用戶的追蹤者列表
        /// </summary>
        public async Task<(List<UserProfile> Followers, int TotalCount)> GetFollowersAsync(string userId, int page = 1, int pageSize = 20)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                    return (new List<UserProfile>(), 0);

                using var context = _contextFactory.CreateDbContext();

                var query = context.Set<Follow>()
                    .Where(f => f.FolloweeId == userId)
                    .Include(f => f.Follower);

                var totalCount = await query.CountAsync();

                var followers = await query
                    .OrderByDescending(f => f.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(f => f.Follower)
                    .ToListAsync();

                return (followers, totalCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得追蹤者列表時發生錯誤：{UserId}", userId);
                return (new List<UserProfile>(), 0);
            }
        }

        /// <summary>
        /// 取得用戶的追蹤列表
        /// </summary>
        public async Task<(List<UserProfile> Following, int TotalCount)> GetFollowingAsync(string userId, int page = 1, int pageSize = 20)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                    return (new List<UserProfile>(), 0);

                using var context = _contextFactory.CreateDbContext();

                var query = context.Set<Follow>()
                    .Where(f => f.FollowerId == userId)
                    .Include(f => f.Followee);

                var totalCount = await query.CountAsync();

                var following = await query
                    .OrderByDescending(f => f.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(f => f.Followee)
                    .ToListAsync();

                return (following, totalCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得追蹤列表時發生錯誤：{UserId}", userId);
                return (new List<UserProfile>(), 0);
            }
        }

        /// <summary>
        /// 取得用戶的追蹤統計
        /// </summary>
        public async Task<(int FollowersCount, int FollowingCount)> GetFollowCountsAsync(string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                    return (0, 0);

                /*
                 * 使用並行查詢優化效能：
                 * 
                 * 1. 使用兩個獨立的 DbContext 實例避免並發衝突
                 * 2. 透過 Task.WhenAll 同時執行追蹤者和被追蹤者統計查詢
                 * 3. 解決 Partial View 多次渲染時的 DbContext 重複存取問題
                 * 
                 * 技術原理：
                 * - Factory 模式確保每個 DbContext 實例獨立運作
                 * - 並行查詢提升統計數據獲取效能
                 */
                using var context1 = _contextFactory.CreateDbContext();
                using var context2 = _contextFactory.CreateDbContext();

                var followersTask = context1.Set<Follow>()
                    .CountAsync(f => f.FolloweeId == userId);  // 統計有多少人追蹤此用戶

                var followingTask = context2.Set<Follow>()
                    .CountAsync(f => f.FollowerId == userId);  // 統計此用戶追蹤多少人

                // 並行執行兩個查詢，提升效能
                var results = await Task.WhenAll(followersTask, followingTask);
                
                return (results[0], results[1]);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得追蹤統計時發生錯誤：{UserId}", userId);
                return (0, 0);
            }
        }

        /// <summary>
        /// 清除追蹤相關的快取
        /// 確保追蹤關係變更後，相關統計數據能正確更新
        /// </summary>
        /// <param name="followerId">追蹤者ID</param>
        /// <param name="followeeId">被追蹤者ID</param>
        private async Task ClearFollowRelatedCacheAsync(string followerId, string followeeId)
        {
            try
            {
                /*
                 * 快取清理策略：
                 * 
                 * 追蹤關係異動時，需要清除相關的快取資料以確保數據一致性：
                 * 1. 被追蹤者的統計數據（粉絲數變化）
                 * 2. 追蹤者的統計數據（追蹤數變化）
                 * 3. 兩者間的具體追蹤狀態
                 */
                var cacheKeys = new[]
                {
                    $"user_stats_{followeeId}",   // 被追蹤者統計（粉絲數）
                    $"user_stats_{followerId}",   // 追蹤者統計（追蹤數）
                    $"follow_status_{followerId}_{followeeId}"  // 追蹤關係狀態
                };

                // 並行清除多個快取，提升清理效能
                var clearTasks = cacheKeys.Select(key => _cacheService.RemoveAsync(key));
                await Task.WhenAll(clearTasks);

                _logger.LogInformation("已清除追蹤相關快取，追蹤者: {FollowerId}，被追蹤者: {FolloweeId}", 
                    followerId, followeeId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "清除快取時發生錯誤，追蹤者: {FollowerId}，被追蹤者: {FolloweeId}", 
                    followerId, followeeId);
            }
        }
    }
}