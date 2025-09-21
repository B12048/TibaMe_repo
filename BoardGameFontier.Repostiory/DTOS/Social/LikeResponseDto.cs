using System.ComponentModel.DataAnnotations;

namespace BoardGameFontier.Repostiory.DTOS.Social
{
    /// <summary>
    /// 按讚回應資料傳輸物件
    /// 用於統一按讚相關的 API 回應
    /// 負責人：廖昊威
    /// </summary>
    public class LikeResponseDto
    {
        /// <summary>
        /// 是否已按讚
        /// </summary>
        public bool IsLiked { get; set; }

        /// <summary>
        /// 總按讚數
        /// </summary>
        public int LikeCount { get; set; }

        /// <summary>
        /// 項目類型（Post 或 Comment）
        /// </summary>
        public string ItemType { get; set; } = string.Empty;

        /// <summary>
        /// 項目 ID
        /// </summary>
        public int ItemId { get; set; }
    }

    /// <summary>
    /// 建立按讚請求 DTO - RESTful API 用
    /// </summary>
    public class CreateLikeRequest
    {
        /// <summary>
        /// 項目類型（Post 或 Comment）
        /// </summary>
        [Required(ErrorMessage = "項目類型是必填的")]
        public string ItemType { get; set; } = string.Empty;

        /// <summary>
        /// 項目 ID
        /// </summary>
        [Required(ErrorMessage = "項目 ID 是必填的")]
        public int ItemId { get; set; }
    }

    /// <summary>
    /// 按讚切換請求 DTO (保留向後相容性)
    /// </summary>
    [Obsolete("建議使用 CreateLikeRequest，此類別將在未來版本移除")]
    public class LikeToggleRequestDto
    {
        /// <summary>
        /// 項目類型（Post 或 Comment）
        /// </summary>
        public string ItemType { get; set; } = string.Empty;

        /// <summary>
        /// 項目 ID
        /// </summary>
        public int ItemId { get; set; }
    }

    /// <summary>
    /// 批量按讚狀態查詢請求 DTO
    /// </summary>
    public class LikeStatusBatchRequestDto
    {
        /// <summary>
        /// 要查詢的項目列表
        /// </summary>
        public List<LikeItemDto> Items { get; set; } = new List<LikeItemDto>();
    }

    /// <summary>
    /// 按讚項目 DTO
    /// </summary>
    public class LikeItemDto
    {
        public string ItemType { get; set; } = string.Empty;
        public int ItemId { get; set; }
    }

    /// <summary>
    /// 使用者按讚統計 DTO
    /// </summary>
    public class UserLikeStatsDto
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
}