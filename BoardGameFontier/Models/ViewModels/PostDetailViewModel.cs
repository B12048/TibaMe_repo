using BoardGameFontier.Repostiory.Entity.Social;

namespace BoardGameFontier.Models.ViewModels
{
    /// <summary>
    /// 貼文詳細頁面 ViewModel
    /// 用於貼文詳細檢視的資料綁定
    /// </summary>
    public class PostDetailViewModel
    {
        /// <summary>
        /// 貼文資料
        /// </summary>
        public PostViewModel Post { get; set; } = new();

        /// <summary>
        /// 當前使用者資訊
        /// </summary>
        public CurrentUserViewModel CurrentUser { get; set; } = new();

        /// <summary>
        /// 留言列表
        /// </summary>
        public List<CommentViewModel> Comments { get; set; } = new();

        /// <summary>
        /// 權限設定
        /// </summary>
        public PostPermissionViewModel Permissions { get; set; } = new();

        /// <summary>
        /// 相關貼文推薦
        /// </summary>
        public List<RelatedPostViewModel> RelatedPosts { get; set; } = new();
    }

    /// <summary>
    /// 貼文 ViewModel
    /// </summary>
    public class PostViewModel
    {
        /// <summary>
        /// 貼文 ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 貼文標題
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// 貼文內容
        /// </summary>
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// 貼文類型
        /// </summary>
        public PostType Type { get; set; }

        /// <summary>
        /// 作者資訊
        /// </summary>
        public AuthorViewModel Author { get; set; } = new();

        /// <summary>
        /// 相關遊戲
        /// </summary>
        public RelatedGameViewModel? RelatedGame { get; set; }

        /// <summary>
        /// 統計資訊
        /// </summary>
        public PostStatsViewModel Stats { get; set; } = new();

        /// <summary>
        /// 時間資訊
        /// </summary>
        public PostTimeViewModel Time { get; set; } = new();

        /// <summary>
        /// 交易資訊（僅交易類型貼文）
        /// </summary>
        public TradeInfoViewModel? TradeInfo { get; set; }

        /// <summary>
        /// 圖片列表
        /// </summary>
        public List<string> ImageUrls { get; set; } = new();

        /// <summary>
        /// 標籤列表
        /// </summary>
        public List<PostTagViewModel> Tags { get; set; } = new();

        /// <summary>
        /// 貼文狀態
        /// </summary>
        public PostStatusViewModel Status { get; set; } = new();

        /// <summary>
        /// 是否已被當前用戶按讚
        /// </summary>
        public bool IsLikedByCurrentUser { get; set; }
    }


    /// <summary>
    /// 相關遊戲 ViewModel
    /// </summary>
    public class RelatedGameViewModel
    {
        /// <summary>
        /// 遊戲 ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 遊戲名稱
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 遊戲封面圖
        /// </summary>
        public string? ImageUrl { get; set; }

        /// <summary>
        /// 遊戲類型
        /// </summary>
        public string Category { get; set; } = string.Empty;
    }

    /// <summary>
    /// 貼文統計資訊 ViewModel
    /// </summary>
    public class PostStatsViewModel
    {
        /// <summary>
        /// 按讚數
        /// </summary>
        public int LikeCount { get; set; }

        /// <summary>
        /// 留言數
        /// </summary>
        public int CommentCount { get; set; }

        /// <summary>
        /// 瀏覽數
        /// </summary>
        public int ViewCount { get; set; }

        /// <summary>
        /// 分享數
        /// </summary>
        public int ShareCount { get; set; }

        /// <summary>
        /// 收藏數
        /// </summary>
        public int FavoriteCount { get; set; }
    }

    /// <summary>
    /// 貼文時間資訊 ViewModel
    /// </summary>
    public class PostTimeViewModel
    {
        /// <summary>
        /// 建立時間
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 更新時間
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// 是否已編輯
        /// </summary>
        public bool IsEdited => UpdatedAt > CreatedAt.AddMinutes(1);

        /// <summary>
        /// 相對時間顯示
        /// </summary>
        public string RelativeTime
        {
            get
            {
                var timeSpan = DateTime.Now - CreatedAt;
                return timeSpan.TotalDays >= 1 
                    ? CreatedAt.ToString("yyyy/MM/dd HH:mm")
                    : timeSpan.TotalHours >= 1 
                        ? $"{(int)timeSpan.TotalHours} 小時前"
                        : $"{(int)timeSpan.TotalMinutes} 分鐘前";
            }
        }

        /// <summary>
        /// 建立時間相對顯示（向後相容）
        /// </summary>
        public string CreatedAgo { get; set; } = string.Empty;

        /// <summary>
        /// 更新時間相對顯示（向後相容）
        /// </summary>
        public string UpdatedAgo { get; set; } = string.Empty;

        /// <summary>
        /// 格式化建立時間（向後相容）
        /// </summary>
        public string FormattedCreated { get; set; } = string.Empty;

        /// <summary>
        /// 格式化更新時間（向後相容）
        /// </summary>
        public string FormattedUpdated { get; set; } = string.Empty;
    }

    /// <summary>
    /// 交易資訊 ViewModel
    /// </summary>
    public class TradeInfoViewModel
    {
        /// <summary>
        /// 交易價格
        /// </summary>
        public decimal? Price { get; set; }

        /// <summary>
        /// 貨幣單位
        /// </summary>
        public string Currency { get; set; } = "NT$";

        /// <summary>
        /// 交易狀態
        /// </summary>
        public TradeStatus Status { get; set; } = TradeStatus.Available;

        /// <summary>
        /// 交易地點
        /// </summary>
        public string? Location { get; set; }

        /// <summary>
        /// 交易備註
        /// </summary>
        public string? Notes { get; set; }
    }

    /// <summary>
    /// 交易狀態枚舉
    /// </summary>
    public enum TradeStatus
    {
        /// <summary>
        /// 可交易
        /// </summary>
        Available = 0,

        /// <summary>
        /// 交易中
        /// </summary>
        InProgress = 1,

        /// <summary>
        /// 已售出
        /// </summary>
        Sold = 2,

        /// <summary>
        /// 已下架
        /// </summary>
        Removed = 3
    }

    /// <summary>
    /// 貼文標籤 ViewModel
    /// </summary>
    public class PostTagViewModel
    {
        /// <summary>
        /// 標籤 ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 標籤名稱
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 標籤顏色
        /// </summary>
        public string Color { get; set; } = "#007bff";

        /// <summary>
        /// 使用次數
        /// </summary>
        public int UsageCount { get; set; }
    }

    /// <summary>
    /// 貼文狀態 ViewModel
    /// </summary>
    public class PostStatusViewModel
    {
        /// <summary>
        /// 是否為熱門貼文
        /// </summary>
        public bool IsPopular { get; set; }

        /// <summary>
        /// 是否被置頂
        /// </summary>
        public bool IsPinned { get; set; }

        /// <summary>
        /// 是否為精選貼文
        /// </summary>
        public bool IsFeatured { get; set; }

        /// <summary>
        /// 是否被鎖定
        /// </summary>
        public bool IsLocked { get; set; }
    }

    /// <summary>
    /// 留言 ViewModel
    /// </summary>
    public class CommentViewModel
    {
        /// <summary>
        /// 留言 ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 留言內容
        /// </summary>
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// 作者資訊
        /// </summary>
        public AuthorViewModel Author { get; set; } = new();

        /// <summary>
        /// 父留言 ID
        /// </summary>
        public int? ParentCommentId { get; set; }

        /// <summary>
        /// 子留言列表
        /// </summary>
        public List<CommentViewModel> Replies { get; set; } = new();

        /// <summary>
        /// 按讚數
        /// </summary>
        public int LikeCount { get; set; }

        /// <summary>
        /// 建立時間
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 是否已被當前用戶按讚
        /// </summary>
        public bool IsLikedByCurrentUser { get; set; }
    }

    /// <summary>
    /// 貼文權限 ViewModel
    /// </summary>
    public class PostPermissionViewModel
    {
        /// <summary>
        /// 是否可以編輯
        /// </summary>
        public bool CanEdit { get; set; }

        /// <summary>
        /// 是否可以刪除
        /// </summary>
        public bool CanDelete { get; set; }

        /// <summary>
        /// 是否可以留言
        /// </summary>
        public bool CanComment { get; set; }

        /// <summary>
        /// 是否可以按讚
        /// </summary>
        public bool CanLike { get; set; }

        /// <summary>
        /// 是否可以分享
        /// </summary>
        public bool CanShare { get; set; } = true;

        /// <summary>
        /// 是否可以檢舉
        /// </summary>
        public bool CanReport { get; set; } = true;
    }

    /// <summary>
    /// 相關貼文 ViewModel
    /// </summary>
    public class RelatedPostViewModel
    {
        /// <summary>
        /// 貼文 ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 貼文標題
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// 作者名稱
        /// </summary>
        public string AuthorName { get; set; } = string.Empty;

        /// <summary>
        /// 建立時間
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 按讚數
        /// </summary>
        public int LikeCount { get; set; }

        /// <summary>
        /// 留言數
        /// </summary>
        public int CommentCount { get; set; }
    }
}