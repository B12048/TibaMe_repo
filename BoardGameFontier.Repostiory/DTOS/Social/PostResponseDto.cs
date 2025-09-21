using BoardGameFontier.Repostiory.Entity.Social;

namespace BoardGameFontier.Repostiory.DTOS.Social
{
    /// <summary>
    /// 貼文回應資料傳輸物件
    /// 用於 API 回應，控制對外暴露的欄位
    /// 負責人：廖昊威
    /// </summary>
    public class PostResponseDto
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string Content { get; set; } = string.Empty;
        public PostType Type { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int LikeCount { get; set; }
        public int CommentCount { get; set; }
        public bool IsLiked { get; set; }

        /// <summary>
        /// 作者資訊
        /// </summary>
        public AuthorDto Author { get; set; } = null!;

        /// <summary>
        /// 相關遊戲資訊（可選）
        /// </summary>
        public RelatedGameDto? RelatedGame { get; set; }

        /// <summary>
        /// 交易資訊（僅交易類型貼文）
        /// </summary>
        public TradeInfoDto? TradeInfo { get; set; }

        /// <summary>
        /// 圖片 URLs（逗號分隔）
        /// </summary>
        public string? ImageUrls { get; set; }
    }

    /// <summary>
    /// 作者資訊 DTO
    /// 僅暴露公開資訊，保護使用者隱私
    /// </summary>
    public class AuthorDto
    {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string? ProfilePictureUrl { get; set; }
        public bool IsFollowed { get; set; }
    }

    /// <summary>
    /// 相關遊戲資訊 DTO
    /// </summary>
    public class RelatedGameDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    /// <summary>
    /// 交易資訊 DTO
    /// </summary>
    public class TradeInfoDto
    {
        public decimal? Price { get; set; }
        public string Currency { get; set; } = "NT$";
        public string? Location { get; set; }
        public string? Notes { get; set; }
    }
}