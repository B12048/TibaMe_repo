using BoardGameFontier.Models.ViewModels;
using BoardGameFontier.Models;

namespace BoardGameFontier.Services
{
    /// <summary>
    /// 社群服務介面
    /// 負責協調社群頁面相關的業務邏輯
    /// </summary>
    public interface ICommunityService
    {
        /// <summary>
        /// 取得社群主頁資料
        /// 整合使用者資訊、熱門內容、頁面設定等
        /// </summary>
        /// <returns>社群主頁 ViewModel</returns>
        Task<CommunityViewModel> GetCommunityPageDataAsync();

        /// <summary>
        /// 取得側邊欄資料（輕量級版本）
        /// 僅包含使用者資訊、熱門文章、熱門遊戲，不包含輪播資料
        /// 用於非社群頁面的側邊欄快速載入
        /// </summary>
        /// <returns>側邊欄專用的 CommunityViewModel</returns>
        Task<CommunityViewModel> GetSidebarDataAsync();

        /// <summary>
        /// 取得熱門貼文排行
        /// </summary>
        /// <param name="count">取得數量，預設 5 筆</param>
        /// <returns>熱門貼文列表</returns>
        Task<List<HotPostViewModel>> GetHotPostsAsync(int count = 5);

        /// <summary>
        /// 取得熱門遊戲排行
        /// </summary>
        /// <param name="count">取得數量，預設 5 筆</param>
        /// <returns>熱門遊戲列表</returns>
        Task<List<HotGameViewModel>> GetHotGamesAsync(int count = 5);

        /// <summary>
        /// 取得貼文詳細資料 ViewModel
        /// 整合貼文、留言、權限、相關推薦等資訊
        /// </summary>
        /// <param name="postId">貼文 ID</param>
        /// <param name="currentUserId">當前使用者 ID</param>
        /// <returns>貼文詳細 ViewModel，找不到時回傳 null</returns>
        Task<PostDetailViewModel?> GetPostDetailViewModelAsync(int postId, string? currentUserId = null);

        /// <summary>
        /// 取得當前使用者資訊 ViewModel
        /// </summary>
        /// <param name="userId">使用者 ID，null 時代表訪客</param>
        /// <returns>使用者資訊 ViewModel</returns>
        Task<CurrentUserViewModel> GetCurrentUserViewModelAsync(string? userId = null);

        // ✅ 已移除未使用的方法：
        // - CalculatePostHotScoreAsync (未實際調用)
        // - GetRelatedPostsAsync (未實際調用) 
        // - GetPostPermissionsAsync (未實際調用)

        /// <summary>
        /// 取得輪播設定
        /// </summary>
        /// <returns>輪播項目列表</returns>
        Task<List<CarouselItemViewModel>> GetCarouselItemsAsync();

        // ✅ 已移除 IncrementPostViewCountAsync 方法，因為沒有實際使用

        // ===== 個人貼文管理功能 =====

        /// <summary>
        /// 取得使用者的個人貼文管理頁面資料
        /// 包含貼文列表、統計資訊、篩選選項等
        /// </summary>
        /// <param name="userId">使用者 ID</param>
        /// <param name="filterOptions">篩選選項</param>
        /// <param name="page">頁碼，預設 1</param>
        /// <returns>個人貼文管理 ViewModel</returns>
        Task<MyPostsViewModel> GetMyPostsPageDataAsync(string userId, PostFilterOptions? filterOptions = null, int page = 1);

        /// <summary>
        /// 取得使用者的貼文統計資訊
        /// </summary>
        /// <param name="userId">使用者 ID</param>
        /// <returns>使用者貼文統計</returns>
        Task<UserPostStatsViewModel> GetUserPostStatsAsync(string userId);

        /// <summary>
        /// 取得單一使用者貼文的詳細資訊（用於編輯）
        /// </summary>
        /// <param name="postId">貼文 ID</param>
        /// <param name="userId">使用者 ID</param>
        /// <returns>貼文詳細資訊，如果非本人或不存在則回傳 null</returns>
        Task<UserPostViewModel?> GetUserPostForEditAsync(int postId, string userId);

        /// <summary>
        /// 取得貼文的最近回應列表
        /// </summary>
        /// <param name="postId">貼文 ID</param>
        /// <param name="userId">使用者 ID</param>
        /// <param name="count">取得數量，預設 10 筆</param>
        /// <returns>最近回應列表</returns>
        Task<List<RecentResponseViewModel>> GetPostRecentResponsesAsync(int postId, string userId, int count = 10);

        /// <summary>
        /// 檢查貼文是否有新回應（自上次檢視後）
        /// </summary>
        /// <param name="postId">貼文 ID</param>
        /// <param name="userId">使用者 ID</param>
        /// <param name="lastViewTime">上次檢視時間</param>
        /// <returns>新回應數量</returns>
        Task<int> GetNewResponseCountAsync(int postId, string userId, DateTime? lastViewTime = null);

        /// <summary>
        /// 標記貼文回應為已讀
        /// </summary>
        /// <param name="postId">貼文 ID</param>
        /// <param name="userId">使用者 ID</param>
        /// <returns>是否成功</returns>
        Task<bool> MarkPostResponsesAsReadAsync(int postId, string userId);

        /// <summary>
        /// 軟刪除使用者貼文
        /// </summary>
        /// <param name="postId">貼文 ID</param>
        /// <param name="userId">使用者 ID</param>
        /// <returns>ServiceResult 包含操作結果</returns>
        Task<ServiceResult<bool>> SoftDeleteUserPostAsync(int postId, string userId);

        /// <summary>
        /// 檢查使用者是否可以編輯指定貼文
        /// </summary>
        /// <param name="postId">貼文 ID</param>
        /// <param name="userId">使用者 ID</param>
        /// <returns>是否可以編輯和限制原因</returns>
        Task<(bool CanEdit, string? Reason)> CanUserEditPostAsync(int postId, string userId);

        /// <summary>
        /// 檢查使用者是否可以刪除指定貼文
        /// </summary>
        /// <param name="postId">貼文 ID</param>
        /// <param name="userId">使用者 ID</param>
        /// <returns>是否可以刪除和限制原因</returns>
        Task<(bool CanDelete, string? Reason)> CanUserDeletePostAsync(int postId, string userId);

        /// <summary>
        /// 清除使用者快取資料（登入/登出時使用）
        /// </summary>
        /// <param name="userId">使用者 ID</param>
        Task ClearUserCacheAsync(string userId);
    }
}