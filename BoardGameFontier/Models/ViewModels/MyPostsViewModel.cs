using BoardGameFontier.Repostiory.Entity.Social;

namespace BoardGameFontier.Models.ViewModels
{
    /// <summary>
    /// 個人貼文管理頁面 ViewModel
    /// 用於 Member/_MyPost.cshtml 個人文章管理功能
    /// </summary>
    public class MyPostsViewModel
    {
        /// <summary>
        /// 使用者基本資訊
        /// </summary>
        public CurrentUserViewModel CurrentUser { get; set; } = new();

        /// <summary>
        /// 我的貼文列表
        /// </summary>
        public List<UserPostViewModel> MyPosts { get; set; } = new();

        /// <summary>
        /// 分頁資訊
        /// </summary>
        public PaginationViewModel Pagination { get; set; } = new();

        /// <summary>
        /// 篩選選項
        /// </summary>
        public PostFilterOptions FilterOptions { get; set; } = new();

        /// <summary>
        /// 統計資訊
        /// </summary>
        public UserPostStatsViewModel Stats { get; set; } = new();
    }

    /// <summary>
    /// 個人貼文項目 ViewModel
    /// 包含貼文基本資訊、統計數據和操作權限
    /// </summary>
    public class UserPostViewModel
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
        /// 貼文內容預覽（前100字）
        /// </summary>
        public string ContentPreview { get; set; } = string.Empty;

        /// <summary>
        /// 完整內容（用於編輯模式）
        /// </summary>
        public string FullContent { get; set; } = string.Empty;

        /// <summary>
        /// 貼文類型
        /// </summary>
        public PostType Type { get; set; }

        /// <summary>
        /// 貼文類型顯示名稱
        /// </summary>
        public string TypeDisplayName => Type switch
        {
            PostType.Review => "心得分享",
            PostType.Question => "詢問求助",
            PostType.Trade => "二手交易",
            PostType.Creation => "創作展示",
            _ => "未分類"
        };

        /// <summary>
        /// 交易價格（僅交易型貼文）
        /// </summary>
        public decimal? Price { get; set; }

        /// <summary>
        /// 交易地點（僅交易型貼文）
        /// </summary>
        public string? TradeLocation { get; set; }

        /// <summary>
        /// 交易備註（僅交易型貼文）
        /// </summary>
        public string? TradeNotes { get; set; }

        /// <summary>
        /// 相關桌遊資訊
        /// </summary>
        public RelatedGameViewModel? RelatedGame { get; set; }

        /// <summary>
        /// 發布時間
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 最後更新時間
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// 時間顯示格式
        /// </summary>
        public PostTimeViewModel TimeDisplay { get; set; } = new();

        /// <summary>
        /// 互動統計
        /// </summary>
        public PostStatsViewModel Stats { get; set; } = new();

        /// <summary>
        /// 最近的回應列表（最多5筆）
        /// </summary>
        public List<RecentResponseViewModel> RecentResponses { get; set; } = new();

        /// <summary>
        /// 是否有新回應（自上次檢視後）
        /// </summary>
        public bool HasNewResponses { get; set; }

        /// <summary>
        /// 新回應數量
        /// </summary>
        public int NewResponseCount { get; set; }

        /// <summary>
        /// 操作權限
        /// </summary>
        public PostActionPermissions Permissions { get; set; } = new();
    }

    /// <summary>
    /// 最近回應 ViewModel
    /// </summary>
    public class RecentResponseViewModel
    {
        /// <summary>
        /// 回應 ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 回應內容預覽
        /// </summary>
        public string ContentPreview { get; set; } = string.Empty;

        /// <summary>
        /// 回應者資訊
        /// </summary>
        public AuthorViewModel Author { get; set; } = new();

        /// <summary>
        /// 回應時間
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 時間顯示格式
        /// </summary>
        public string TimeAgo { get; set; } = string.Empty;

        /// <summary>
        /// 是否為新回應
        /// </summary>
        public bool IsNew { get; set; }
    }

    /// <summary>
    /// 使用者貼文統計 ViewModel
    /// </summary>
    public class UserPostStatsViewModel
    {
        /// <summary>
        /// 總貼文數
        /// </summary>
        public int TotalPosts { get; set; }

        /// <summary>
        /// 各類型貼文數量
        /// </summary>
        public Dictionary<PostType, int> PostsByType { get; set; } = new();

        /// <summary>
        /// 總獲讚數
        /// </summary>
        public int TotalLikes { get; set; }

        /// <summary>
        /// 總回應數
        /// </summary>
        public int TotalComments { get; set; }

        // ✅ 移除 TotalViews 欄位，因為沒有實際記數功能

        /// <summary>
        /// 本月發文數
        /// </summary>
        public int PostsThisMonth { get; set; }

        /// <summary>
        /// 待回覆的貼文數
        /// </summary>
        public int PostsWithNewResponses { get; set; }
    }

    /// <summary>
    /// 貼文篩選選項
    /// </summary>
    public class PostFilterOptions
    {
        /// <summary>
        /// 篩選的貼文類型
        /// </summary>
        public PostType? FilterType { get; set; }

        /// <summary>
        /// 排序方式
        /// </summary>
        public PostSortOrder SortOrder { get; set; } = PostSortOrder.CreatedDesc;

        /// <summary>
        /// 是否只顯示有新回應的貼文
        /// </summary>
        public bool OnlyWithNewResponses { get; set; }

        /// <summary>
        /// 搜尋關鍵字
        /// </summary>
        public string? SearchKeyword { get; set; }

        /// <summary>
        /// 每頁顯示數量
        /// </summary>
        public int PageSize { get; set; } = 10;
    }

    /// <summary>
    /// 貼文排序方式
    /// </summary>
    public enum PostSortOrder
    {
        /// <summary>
        /// 建立時間降序（最新優先）
        /// </summary>
        CreatedDesc,
        
        /// <summary>
        /// 建立時間升序（最舊優先）
        /// </summary>
        CreatedAsc,
        
        /// <summary>
        /// 更新時間降序（最近更新優先）
        /// </summary>
        UpdatedDesc,
        
        /// <summary>
        /// 互動數量降序（最熱門優先）
        /// </summary>
        PopularityDesc,
        
        /// <summary>
        /// 回應數量降序（最多回應優先）
        /// </summary>
        CommentsDesc
    }

    /// <summary>
    /// 貼文操作權限
    /// </summary>
    public class PostActionPermissions
    {
        /// <summary>
        /// 是否可以編輯
        /// </summary>
        public bool CanEdit { get; set; } = true;

        /// <summary>
        /// 是否可以刪除
        /// </summary>
        public bool CanDelete { get; set; } = true;

        /// <summary>
        /// 是否可以查看詳情
        /// </summary>
        public bool CanView { get; set; } = true;

        /// <summary>
        /// 編輯限制原因（如果不能編輯）
        /// </summary>
        public string? EditRestrictionReason { get; set; }

        /// <summary>
        /// 刪除限制原因（如果不能刪除）
        /// </summary>
        public string? DeleteRestrictionReason { get; set; }
    }
}