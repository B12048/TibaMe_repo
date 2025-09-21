using BoardGameFontier.Models.ViewModels;
using BoardGameFontier.Models.Common;
using BoardGameFontier.Repostiory.Entity.Social;

namespace BoardGameFontier.Services
{
    /// <summary>
    /// 貼文服務介面
    /// 處理貼文相關的業務邏輯
    /// </summary>
    public interface IPostService
    {
        /// <summary>
        /// 根據 ID 取得貼文詳細資料
        /// </summary>
        /// <param name="postId">貼文 ID</param>
        /// <returns>貼文實體，找不到時回傳 null</returns>
        Task<Post?> GetPostByIdAsync(int postId);

        /// <summary>
        /// 取得貼文列表（支援分頁）- 新版使用統一分頁模型
        /// </summary>
        /// <param name="request">分頁請求參數</param>
        /// <param name="postType">貼文類型篩選</param>
        /// <param name="searchKeyword">搜尋關鍵字</param>
        /// <returns>分頁貼文結果</returns>
        Task<PagedResult<Post>> GetPostsPagedAsync(PagedRequest request, PostType? postType = null, string? searchKeyword = null);

        /// <summary>
        /// 取得貼文列表（支援分頁）- 保留向後相容的舊版方法
        /// </summary>
        /// <param name="page">頁數</param>
        /// <param name="pageSize">每頁大小</param>
        /// <param name="postType">貼文類型篩選</param>
        /// <param name="searchKeyword">搜尋關鍵字</param>
        /// <returns>貼文列表</returns>
        [Obsolete("請使用 GetPostsPagedAsync(PagedRequest, PostType?, string?) 方法")]
        Task<(IEnumerable<Post> Posts, int TotalCount)> GetPostsAsync(int page = 1, int pageSize = 9, PostType? postType = null, string? searchKeyword = null);

        /// <summary>
        /// 建立新貼文
        /// </summary>
        /// <param name="post">貼文資料</param>
        /// <returns>建立成功的貼文</returns>
        Task<Post> CreatePostAsync(Post post);

        /// <summary>
        /// 檢查貼文是否存在
        /// </summary>
        /// <param name="postId">貼文 ID</param>
        /// <returns>存在回傳 true</returns>
        Task<bool> PostExistsAsync(int postId);

        /// <summary>
        /// 更新貼文（僅作者可編輯）
        /// </summary>
        /// <param name="postId">貼文 ID</param>
        /// <param name="updateDto">更新資料</param>
        /// <param name="userId">執行更新的使用者 ID</param>
        /// <returns>更新結果</returns>
        Task<PostUpdateResult> UpdatePostAsync(int postId, PostUpdateDto updateDto, string userId);

        /// <summary>
        /// 刪除貼文（僅作者可刪除）
        /// </summary>
        /// <param name="postId">貼文 ID</param>
        /// <param name="userId">執行刪除的使用者 ID</param>
        /// <returns>刪除結果</returns>
        Task<PostDeleteResult> DeletePostAsync(int postId, string userId, bool isAdmin);

    }

    /// <summary>
    /// 貼文更新操作結果
    /// </summary>
    public class PostUpdateResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public object? Data { get; set; }
        
        public static PostUpdateResult CreateSuccess(object? data = null, string message = "更新成功")
        {
            return new PostUpdateResult { Success = true, Message = message, Data = data };
        }
        
        public static PostUpdateResult CreateError(string message)
        {
            return new PostUpdateResult { Success = false, Message = message };
        }
    }

    /// <summary>
    /// 貼文刪除操作結果
    /// </summary>
    public class PostDeleteResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        
        public static PostDeleteResult CreateSuccess(string message = "刪除成功")
        {
            return new PostDeleteResult { Success = true, Message = message };
        }
        
        public static PostDeleteResult CreateError(string message)
        {
            return new PostDeleteResult { Success = false, Message = message };
        }
    }
}