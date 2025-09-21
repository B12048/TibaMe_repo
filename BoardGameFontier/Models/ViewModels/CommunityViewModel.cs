using BoardGameFontier.Repostiory.Entity.Social;
using BoardGameFontier.Repostiory.DTOS.Social;

namespace BoardGameFontier.Models.ViewModels
{
    /// <summary>
    /// 社群主頁 ViewModel
    /// 包含社群頁面所需的所有資料，取代 ViewBag 的使用
    /// </summary>
    public class CommunityViewModel
    {
        /// <summary>
        /// 當前使用者資訊
        /// </summary>
        public CurrentUserViewModel CurrentUser { get; set; } = new();

        /// <summary>
        /// 熱門貼文排行
        /// </summary>
        public List<HotPostViewModel> HotPosts { get; set; } = new();

        /// <summary>
        /// 熱門遊戲排行
        /// </summary>
        public List<HotGameViewModel> HotGames { get; set; } = new();

        /// <summary>
        /// 最近的追蹤者列表（顯示前5名）
        /// </summary>
        public List<FollowerViewModel> RecentFollowers { get; set; } = new();

        /// <summary>
        /// 頁面設定
        /// </summary>
        public CommunityPageSettings PageSettings { get; set; } = new();

        /// <summary>
        /// 所有可選遊戲列表（用於發表貼文下拉選單）
        /// </summary>
        public List<GameSelectionDto> AllGames { get; set; } = new();
    }

    /// <summary>
    /// 當前使用者資訊 ViewModel
    /// </summary>
    public class CurrentUserViewModel
    {
        /// <summary>
        /// 使用者 ID
        /// </summary>
        public string? UserId { get; set; }

        /// <summary>
        /// 是否已認證
        /// </summary>
        public bool IsAuthenticated { get; set; }

        /// <summary>
        /// 顯示名稱
        /// </summary>
        public string DisplayName { get; set; } = "訪客";

        /// <summary>
        /// 使用者名稱
        /// </summary>
        public string? UserName { get; set; }

        /// <summary>
        /// 使用者等級
        /// </summary>
        public int Level { get; set; } = 1;

        /// <summary>
        /// 貼文數量
        /// </summary>
        public int PostsCount { get; set; }

        /// <summary>
        /// 獲得讚數
        /// </summary>
        public int LikesCount { get; set; }

        /// <summary>
        /// 粉絲數量
        /// </summary>
        public int FollowersCount { get; set; }

        /// <summary>
        /// 是否可以建立貼文
        /// </summary>
        public bool CanCreatePost => IsAuthenticated;

        /// <summary>
        /// 頭像 URL
        /// </summary>
        public string? ProfilePictureUrl { get; set; }
    }

    /// <summary>
    /// 熱門貼文 ViewModel
    /// </summary>
    public class HotPostViewModel
    {
        /// <summary>
        /// 貼文 ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 排名
        /// </summary>
        public int Rank { get; set; }

        /// <summary>
        /// 貼文標題
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// 作者名稱
        /// </summary>
        public string AuthorName { get; set; } = string.Empty;

        /// <summary>
        /// 熱度分數
        /// </summary>
        public int HotScore { get; set; }

        /// <summary>
        /// 按讚數
        /// </summary>
        public int LikeCount { get; set; }

        /// <summary>
        /// 留言數
        /// </summary>
        public int CommentCount { get; set; }

        /// <summary>
        /// 建立時間
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 是否為精選貼文
        /// </summary>
        public bool IsFeatured { get; set; }

        /// <summary>
        /// 貼文類型
        /// </summary>
        public PostType Type { get; set; }

        /// <summary>
        /// 顯示的中繼資訊
        /// </summary>
        public string MetaInfo { get; set; } = string.Empty;
    }

    /// <summary>
    /// 熱門遊戲 ViewModel
    /// </summary>
    public class HotGameViewModel
    {
        /// <summary>
        /// 遊戲 ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 排名
        /// </summary>
        public int Rank { get; set; }

        /// <summary>
        /// 遊戲名稱
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 遊戲類型
        /// </summary>
        public string Category { get; set; } = string.Empty;

        /// <summary>
        /// 討論數量
        /// </summary>
        public int DiscussionCount { get; set; }

        /// <summary>
        /// 熱度分數
        /// </summary>
        public int HotScore { get; set; }

        /// <summary>
        /// 遊戲封面圖 URL
        /// </summary>
        public string? ImageUrl { get; set; }

        /// <summary>
        /// 顯示的中繼資訊
        /// </summary>
        public string MetaInfo { get; set; } = string.Empty;
    }

    /// <summary>
    /// 社群頁面設定
    /// </summary>
    public class CommunityPageSettings
    {
        /// <summary>
        /// 頁面標題
        /// </summary>
        public string Title { get; set; } = "社群討論";

        /// <summary>
        /// 預設分頁大小
        /// </summary>
        public int DefaultPageSize { get; set; } = 9;

        /// <summary>
        /// 是否啟用即時更新
        /// </summary>
        public bool EnableRealTimeUpdates { get; set; } = false;

        /// <summary>
        /// 輪播圖片設定
        /// </summary>
        public List<CarouselItemViewModel> CarouselItems { get; set; } = new();
    }

    /// <summary>
    /// 輪播項目 ViewModel
    /// </summary>
    public class CarouselItemViewModel
    {
        /// <summary>
        /// 圖片 URL
        /// </summary>
        public string ImageUrl { get; set; } = string.Empty;

        /// <summary>
        /// 標題
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// 連結 URL
        /// </summary>
        public string? LinkUrl { get; set; }

        /// <summary>
        /// 是否在新視窗開啟
        /// </summary>
        public bool OpenInNewTab { get; set; } = true;
    }

    /// <summary>
    /// 追蹤者資訊 ViewModel
    /// </summary>
    public class FollowerViewModel
    {
        /// <summary>
        /// 用戶 ID
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// 用戶名稱
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// 顯示名稱
        /// </summary>
        public string DisplayName { get; set; } = string.Empty;

        /// <summary>
        /// 頭像 URL
        /// </summary>
        public string? ProfilePictureUrl { get; set; }

        /// <summary>
        /// 追蹤時間
        /// </summary>
        public DateTime FollowedAt { get; set; }

        /// <summary>
        /// 追蹤時間文字顯示
        /// </summary>
        public string FollowedTimeAgo { get; set; } = string.Empty;

        /// <summary>
        /// 是否互相追蹤
        /// </summary>
        public bool IsMutualFollow { get; set; }
    }
}