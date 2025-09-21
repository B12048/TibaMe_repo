using BoardGameFontier.Repostiory;
using BoardGameFontier.Repostiory.Entity.Social;
using BoardGameFontier.Repostiory.DTOS.Social;
using BoardGameFontier.Models.ViewModels;
using BoardGameFontier.Extensions;
using Microsoft.EntityFrameworkCore;

namespace BoardGameFontier.Services
{
    /// <summary>
    /// 留言服務實作
    /// 處理留言相關的業務邏輯與資料操作
    /// </summary>
    public class CommentService(ApplicationDbContext context, ICurrentUserService currentUserService) : ICommentService
    {
        private readonly ApplicationDbContext _context = context;
        private readonly ICurrentUserService _currentUser = currentUserService;

        /// <summary>
        /// 取得指定貼文的所有留言
        /// </summary>
        public async Task<List<CommentResponseDto>> GetPostCommentsAsync(int postId)
        {
            var commentsEntities = await _context.Comments
                .Include(c => c.Author)
                .Where(c => c.PostId == postId)
                .OrderBy(c => c.CreatedAt) // 按時間順序排列
                .ToListAsync();

            // 在記憶體中使用 Extension 方法轉換
            return commentsEntities.Select(c => c.ToCommentResponseDto()).ToList();
        }

        /// <summary>
        /// 建立新留言
        /// </summary>
        public async Task<CommentResponseDto> CreateCommentAsync(CreateCommentRequestDto createDto, string authorId)
        {
            // 驗證貼文是否存在
            var postExists = await _context.Posts.AnyAsync(p => p.Id == createDto.PostId);
            if (!postExists)
            {
                throw new ArgumentException("指定的貼文不存在");
            }

            // 如果是回覆留言，驗證父留言是否存在
            if (createDto.ParentCommentId.HasValue)
            {
                var parentExists = await _context.Comments.AnyAsync(c => c.Id == createDto.ParentCommentId.Value);
                if (!parentExists)
                {
                    throw new ArgumentException("指定的父留言不存在");
                }
            }

            // 建立留言
            var comment = new Comment
            {
                Content = createDto.Content.Trim(),
                PostId = createDto.PostId,
                ParentCommentId = createDto.ParentCommentId,
                AuthorId = authorId,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                LikeCount = 0
            };

            _context.Comments.Add(comment);

            // 更新貼文的留言數量
            var post = await _context.Posts.FindAsync(createDto.PostId);
            if (post != null)
            {
                post.CommentCount++;
                _context.Posts.Update(post);
            }

            await _context.SaveChangesAsync();

            // 取得完整的留言資料（包含作者資訊）
            var createdComment = await _context.Comments
                .Include(c => c.Author)
                .FirstAsync(c => c.Id == comment.Id);

            return createdComment.ToCommentResponseDto();
        }

        /// <summary>
        /// 刪除留言（僅作者可刪除）
        /// </summary>
        // 假設在 CommentService 類別中先加上：
        // private readonly ICurrentUserService _currentUser;
        // public CommentService(ApplicationDbContext context, ICurrentUserService currentUser) {
        //     _context = context;
        //     _currentUser = currentUser;
        // }

        public async Task<bool> DeleteCommentAsync(int commentId, string userId)
        {
            var comment = await _context.Comments.FindAsync(commentId);
            if (comment == null) return false;

            // 取出該留言所屬貼文的作者 Id（用投影避免載入整個 Post）
            var postAuthorId = await _context.Posts
                .Where(p => p.Id == comment.PostId)
                .Select(p => p.AuthorId)
                .FirstOrDefaultAsync();

            // 判斷是否 Admin（用你專案已在 Controller 使用的同一個 ICurrentUserService）
            var isAdmin = await _currentUser.IsInRoleAsync("Admin");


            // 三種身分其中之一即可刪除：留言作者 / 貼文作者 / Admin
            var canDelete =
                comment.AuthorId == userId ||
                postAuthorId == userId ||
                isAdmin;

            if (!canDelete) return false;

            // 刪除留言
            _context.Comments.Remove(comment);

            // 更新貼文的留言數量（保護一下下限）
            var post = await _context.Posts.FindAsync(comment.PostId);
            if (post != null && post.CommentCount > 0)
            {
                post.CommentCount--;
                _context.Posts.Update(post);
            }

            await _context.SaveChangesAsync();
            return true;
        }


        /// <summary>
        /// 取得留言詳細資料
        /// </summary>
        public async Task<CommentResponseDto?> GetCommentByIdAsync(int commentId)
        {
            var comment = await _context.Comments
                .Include(c => c.Author)
                .FirstOrDefaultAsync(c => c.Id == commentId);

            if (comment == null) return null;

            return comment.ToCommentResponseDto();
        }

        /// <summary>
        /// 檢查使用者是否為留言作者
        /// </summary>
        public async Task<bool> IsCommentAuthorAsync(int commentId, string userId)
        {
            var comment = await _context.Comments.FindAsync(commentId);
            return comment?.AuthorId == userId;
        }
    }
}