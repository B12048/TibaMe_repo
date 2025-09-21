namespace BoardGameFontier.Repostiory.DTOS.Social
{
    /// <summary>
    /// 通知回應資料傳輸物件
    /// 用於通知系統的 API 回應
    /// 負責人：廖昊威
    /// </summary>
    public class NotificationResponseDto
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 觸發通知的使用者資訊（可選）
        /// </summary>
        public AuthorDto? Trigger { get; set; }

        /// <summary>
        /// 相關項目資訊（可選）
        /// </summary>
        public RelatedItemDto? RelatedItem { get; set; }
    }

    /// <summary>
    /// 相關項目資訊 DTO
    /// </summary>
    public class RelatedItemDto
    {
        /// <summary>
        /// 項目類型（Post、Comment、TradeItem等）
        /// </summary>
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// 項目 ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 項目標題或簡要描述
        /// </summary>
        public string? Title { get; set; }
    }

    /// <summary>
    /// 通知統計資料 DTO
    /// </summary>
    public class NotificationStatsDto
    {
        /// <summary>
        /// 未讀通知數量
        /// </summary>
        public int UnreadCount { get; set; }

        /// <summary>
        /// 總通知數量
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// 最新通知時間
        /// </summary>
        public DateTime? LatestNotificationTime { get; set; }
    }

    /// <summary>
    /// 標記通知為已讀請求 DTO
    /// </summary>
    public class MarkNotificationReadRequestDto
    {
        /// <summary>
        /// 通知 ID 列表
        /// </summary>
        public List<int> NotificationIds { get; set; } = new List<int>();
    }
}