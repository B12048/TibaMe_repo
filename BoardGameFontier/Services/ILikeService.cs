using BoardGameFontier.Models;

namespace BoardGameFontier.Services
{
    /// <summary>
    /// 按讚服務介面 - 簡化重構版本
    /// 統一處理 Post 和 Comment 的按讚邏輯
    /// </summary>
    public interface ILikeService
    {
        /// <summary>
        /// 統一按讚切換功能
        /// 支援 Post 和 Comment 類型
        /// </summary>
        /// <param name="itemType">項目類型：Post 或 Comment</param>
        /// <param name="itemId">項目ID</param>
        /// <param name="userId">用戶ID</param>
        /// <returns>切換結果，包含新的按讚狀態和計數</returns>
        Task<ServiceResult<LikeToggleResult>> ToggleLikeAsync(string itemType, int itemId, string userId);

        /// <summary>
        /// 批量查詢用戶按讚狀態
        /// 解決 N+1 查詢問題
        /// </summary>
        /// <param name="items">要查詢的項目列表</param>
        /// <param name="userId">用戶ID</param>
        /// <returns>按讚狀態字典，Key 格式：ItemType_ItemId</returns>
        Task<Dictionary<string, bool>> GetLikeStatusBatchAsync(List<LikeItem> items, string userId);

        /// <summary>
        /// 批量查詢項目按讚數量
        /// </summary>
        /// <param name="items">要查詢的項目列表</param>
        /// <returns>按讚數量字典，Key 格式：ItemType_ItemId</returns>
        Task<Dictionary<string, int>> GetLikeCountsBatchAsync(List<LikeItem> items);

        /// <summary>
        /// 獲取用戶按讚統計
        /// 用於側邊欄顯示
        /// </summary>
        /// <param name="userId">用戶ID</param>
        /// <returns>用戶按讚統計資料</returns>
        Task<UserLikeStats> GetUserLikeStatsAsync(string userId);

        /// <summary>
        /// 檢查單個項目的按讚狀態
        /// 保留此方法以向後兼容
        /// </summary>
        Task<ServiceResult<bool>> HasLikedAsync(string itemType, int itemId, string userId);

        /// <summary>
        /// 建立按讚 - RESTful API 用
        /// </summary>
        /// <param name="itemType">項目類型：Post 或 Comment</param>
        /// <param name="itemId">項目ID</param>
        /// <param name="userId">用戶ID</param>
        /// <returns>按讚結果</returns>
        Task<ServiceResult<LikeToggleResult>> LikeAsync(string itemType, int itemId, string userId);

        /// <summary>
        /// 取消按讚 - RESTful API 用
        /// </summary>
        /// <param name="itemType">項目類型：Post 或 Comment</param>
        /// <param name="itemId">項目ID</param>
        /// <param name="userId">用戶ID</param>
        /// <returns>取消按讚結果</returns>
        Task<ServiceResult<LikeToggleResult>> UnlikeAsync(string itemType, int itemId, string userId);

        /// <summary>
        /// 取得單一項目按讚數量 - RESTful API 用
        /// </summary>
        /// <param name="itemType">項目類型：Post 或 Comment</param>
        /// <param name="itemId">項目ID</param>
        /// <returns>按讚數量</returns>
        Task<ServiceResult<int>> GetLikeCountAsync(string itemType, int itemId);
    }

    #region Result Classes

    /// <summary>
    /// 按讚切換結果
    /// </summary>
    public class LikeToggleResult
    {
        public bool IsLiked { get; set; }
        public int LikeCount { get; set; }
    }

    /// <summary>
    /// 用戶按讚統計
    /// </summary>
    public class UserLikeStats
    {
        /// <summary>
        /// 總獲讚數（貼文 + 留言）
        /// </summary>
        public int TotalLikesReceived { get; set; }

        /// <summary>
        /// 總給讚數
        /// </summary>
        public int TotalLikesGiven { get; set; }

        /// <summary>
        /// 貼文獲讚數
        /// </summary>
        public int PostLikesReceived { get; set; }

        /// <summary>
        /// 留言獲讚數
        /// </summary>
        public int CommentLikesReceived { get; set; }
    }

    /// <summary>
    /// 按讚項目
    /// </summary>
    public class LikeItem
    {
        public string ItemType { get; set; } = string.Empty;
        public int ItemId { get; set; }
    }

    #endregion
}