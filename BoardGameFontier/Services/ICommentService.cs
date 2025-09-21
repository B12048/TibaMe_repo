using BoardGameFontier.Models.ViewModels;
using BoardGameFontier.Repostiory.DTOS.Social;

namespace BoardGameFontier.Services
{
    /// <summary>
    /// 留言服務介面
    /// 處理留言相關的業務邏輯
    /// </summary>
    public interface ICommentService
    {
        /// <summary>
        /// 取得指定貼文的所有留言
        /// </summary>
        /// <param name="postId">貼文 ID</param>
        /// <returns>留言列表，按建立時間排序</returns>
        Task<List<CommentResponseDto>> GetPostCommentsAsync(int postId);

        /// <summary>
        /// 建立新留言
        /// </summary>
        /// <param name="createDto">留言建立資料</param>
        /// <param name="authorId">留言者 ID</param>
        /// <returns>建立的留言資料</returns>
        Task<CommentResponseDto> CreateCommentAsync(CreateCommentRequestDto createDto, string authorId);

        /// <summary>
        /// 刪除留言（貼文作者、或 Admin 可刪除）
        /// </summary>
        /// <param name="commentId">留言 ID</param>
        /// <param name="userId">操作者 ID</param>
        /// <returns>是否刪除成功</returns>

        Task<bool> DeleteCommentAsync(int commentId, string userId);


        /// <summary>
        /// 取得留言詳細資料
        /// </summary>
        /// <param name="commentId">留言 ID</param>
        /// <returns>留言詳細資料，不存在時回傳 null</returns>
        Task<CommentResponseDto?> GetCommentByIdAsync(int commentId);

        /// <summary>
        /// 檢查使用者是否為留言作者
        /// </summary>
        /// <param name="commentId">留言 ID</param>
        /// <param name="userId">使用者 ID</param>
        /// <returns>是作者回傳 true</returns>
        Task<bool> IsCommentAuthorAsync(int commentId, string userId);
    }

    // 注意：CommentDto 和 CreateCommentDto 已移動到 BoardGameFontier.Repository.DTOS.Social
    // 這些類別現在統一管理在 Repository 層
}