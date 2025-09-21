using BoardGameFontier.Repostiory.Entity;
using BoardGameFontier.Repostiory.Entity.Social;

namespace BoardGameFontier.Services
{
    /// <summary>
    /// 追蹤服務介面
    /// 負責處理用戶之間的追蹤關係
    /// </summary>
    public interface IFollowService
    {
        /// <summary>
        /// 追蹤指定用戶
        /// </summary>
        /// <param name="followerId">追蹤者ID</param>
        /// <param name="followeeId">被追蹤者ID</param>
        /// <returns>操作結果</returns>
        Task<bool> FollowUserAsync(string followerId, string followeeId);

        /// <summary>
        /// 取消追蹤指定用戶
        /// </summary>
        /// <param name="followerId">追蹤者ID</param>
        /// <param name="followeeId">被追蹤者ID</param>
        /// <returns>操作結果</returns>
        Task<bool> UnfollowUserAsync(string followerId, string followeeId);

        /// <summary>
        /// 檢查是否已追蹤指定用戶
        /// </summary>
        /// <param name="followerId">追蹤者ID</param>
        /// <param name="followeeId">被追蹤者ID</param>
        /// <returns>追蹤狀態</returns>
        Task<bool> IsFollowingAsync(string followerId, string followeeId);

        /// <summary>
        /// 批次檢查追蹤狀態
        /// </summary>
        /// <param name="followerId">追蹤者ID</param>
        /// <param name="followeeIds">被追蹤者ID列表</param>
        /// <returns>追蹤狀態字典</returns>
        Task<Dictionary<string, bool>> GetFollowStatusBatchAsync(string followerId, List<string> followeeIds);

        /// <summary>
        /// 取得用戶的追蹤者列表
        /// </summary>
        /// <param name="userId">用戶ID</param>
        /// <param name="page">頁碼</param>
        /// <param name="pageSize">每頁筆數</param>
        /// <returns>追蹤者列表</returns>
        Task<(List<UserProfile> Followers, int TotalCount)> GetFollowersAsync(string userId, int page = 1, int pageSize = 20);

        /// <summary>
        /// 取得用戶的追蹤列表
        /// </summary>
        /// <param name="userId">用戶ID</param>
        /// <param name="page">頁碼</param>
        /// <param name="pageSize">每頁筆數</param>
        /// <returns>追蹤列表</returns>
        Task<(List<UserProfile> Following, int TotalCount)> GetFollowingAsync(string userId, int page = 1, int pageSize = 20);

        /// <summary>
        /// 取得用戶的追蹤統計
        /// </summary>
        /// <param name="userId">用戶ID</param>
        /// <returns>追蹤統計</returns>
        Task<(int FollowersCount, int FollowingCount)> GetFollowCountsAsync(string userId);
    }
}