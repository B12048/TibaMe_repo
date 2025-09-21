using BoardGameFontier.Repostiory;
using BoardGameFontier.Repostiory.Entity.Social;
using BoardGameFontier.Repostiory.Entity;
using BoardGameFontier.Models.ViewModels;
using BoardGameFontier.Models.Common;
using Microsoft.EntityFrameworkCore;

namespace BoardGameFontier.Services
{
    /// <summary>
    /// 貼文服務實作 - 優化版本：整合快取失效機制
    /// </summary>
    public class PostService : IPostService
    {
        private readonly ApplicationDbContext _context;
        private readonly ICacheService _cacheService;

        public PostService(ApplicationDbContext context, ICacheService cacheService)
        {
            _context = context;
            _cacheService = cacheService;
        }

        /// <summary>
        /// 清除使用者相關快取
        /// </summary>
        private async Task InvalidateUserCacheAsync(string userId)
        {
            await _cacheService.RemoveAsync($"user_stats_{userId}");
            await _cacheService.RemoveAsync($"user_profile_{userId}");
        }

        /// <summary>
        /// 根據 ID 取得貼文詳細資料
        /// </summary>
        public async Task<Post?> GetPostByIdAsync(int postId)
        {
            return await _context.Set<Post>()
                .Include(p => p.Author)
                .Include(p => p.Comments)
                    .ThenInclude(c => c.Author)
                .Include(p => p.Likes)
                .Include(p => p.GameDetail)
                .FirstOrDefaultAsync(p => p.Id == postId);
        }

        /// <summary>
        /// 取得貼文列表（支援分頁）- 線程安全版本
        /// </summary>
        public async Task<(IEnumerable<Post> Posts, int TotalCount)> GetPostsAsync(int page = 1, int pageSize = 9, PostType? postType = null, string? searchKeyword = null)
        {
            // ✅ 效能優化：使用 AsNoTracking 提升查詢速度
            var baseQuery = _context.Set<Post>()
                .AsNoTracking()
                .AsQueryable();

            // 類型篩選
            if (postType.HasValue)
            {
                baseQuery = baseQuery.Where(p => p.Type == postType.Value);
            }

            // 關鍵字搜尋
            if (!string.IsNullOrWhiteSpace(searchKeyword))
            {
                baseQuery = baseQuery.Where(p => 
                    (p.Title != null && p.Title.Contains(searchKeyword)) ||
                    p.Content.Contains(searchKeyword));
            }

            // 排序：最新的在前面
            baseQuery = baseQuery.OrderByDescending(p => p.CreatedAt);

            // ✅ 線程安全修復：先計算總數，再查詢分頁資料
            var totalCount = await baseQuery.CountAsync();
            
            var posts = await baseQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new
                {
                    // ✅ 效能優化：使用匿名類型避免EF Core的複雜物件映射
                    Id = p.Id,
                    Title = p.Title,
                    Content = p.Content,
                    AuthorId = p.AuthorId,
                    CreatedAt = p.CreatedAt,
                    Type = p.Type,
                    LikeCount = p.LikeCount,
                    CommentCount = p.CommentCount,
                    // ✅ 移除 ViewCount 欄位
                    GameDetailId = p.GameDetailId,
                    // 🖼️ 新增：圖片URL欄位
                    ImageUrls = p.ImageUrls,
                    // 交易相關欄位
                    Price = p.Price,
                    TradeLocation = p.TradeLocation,
                    TradeNotes = p.TradeNotes,
                    // ✅ 關聯資料平鋪，避免複雜的導航屬性
                    Author_Id = p.Author.Id,
                    Author_UserName = p.Author.UserName,
                    Author_DisplayName = p.Author.DisplayName,
                    Author_ProfilePictureUrl = p.Author.ProfilePictureUrl,
                    GameDetail_Id = p.GameDetail != null ? p.GameDetail.Id : (int?)null,
                    GameDetail_EngTitle = p.GameDetail != null ? p.GameDetail.engTitle : null,
                    GameDetail_ZhtTitle = p.GameDetail != null ? p.GameDetail.zhtTitle : null
                })
                .ToListAsync();

            // ✅ 在記憶體中重組為Post物件，避免EF Core查詢開銷
            var result = posts.Select(p => new Post
            {
                Id = p.Id,
                Title = p.Title,
                Content = p.Content,
                AuthorId = p.AuthorId,
                CreatedAt = p.CreatedAt,
                Type = p.Type,
                LikeCount = p.LikeCount,
                CommentCount = p.CommentCount,
                // ✅ 移除 ViewCount 欄位
                GameDetailId = p.GameDetailId,
                // 🖼️ 新增：圖片URL欄位
                ImageUrls = p.ImageUrls,
                // 交易相關欄位
                Price = p.Price,
                TradeLocation = p.TradeLocation,
                TradeNotes = p.TradeNotes,
                Author = new UserProfile
                {
                    Id = p.Author_Id,
                    UserName = p.Author_UserName,
                    DisplayName = p.Author_DisplayName,
                    ProfilePictureUrl = p.Author_ProfilePictureUrl
                },
                GameDetail = p.GameDetail_Id.HasValue ? new GameDetail
                {
                    Id = p.GameDetail_Id.Value,
                    engTitle = p.GameDetail_EngTitle,
                    zhtTitle = p.GameDetail_ZhtTitle
                } : null
            }).ToList();

            return (result, totalCount);
        }

        /// <summary>
        /// 取得貼文列表（支援分頁）- 新版使用統一分頁模型
        /// </summary>
        public async Task<PagedResult<Post>> GetPostsPagedAsync(PagedRequest request, PostType? postType = null, string? searchKeyword = null)
        {
            // 驗證分頁參數
            if (!request.IsValid())
            {
                return PagedResult<Post>.Empty(request);
            }

            // ✅ 重用現有的查詢邏輯，避免程式碼重複
            var (posts, totalCount) = await GetPostsAsync(request.Page, request.PageSize, postType, searchKeyword);

            // 轉換為統一分頁格式
            return new PagedResult<Post>(posts.ToList(), totalCount, request);
        }

        /// <summary>
        /// 建立新貼文 - 優化版本：加入快取失效
        /// </summary>
        public async Task<Post> CreatePostAsync(Post post)
        {
            post.CreatedAt = DateTime.Now;
            post.UpdatedAt = DateTime.Now;

            _context.Set<Post>().Add(post);
            await _context.SaveChangesAsync();

            // ✅ 清除使用者統計快取
            await InvalidateUserCacheAsync(post.AuthorId);

            return post;
        }

        /// <summary>
        /// 檢查貼文是否存在
        /// </summary>
        public async Task<bool> PostExistsAsync(int postId)
        {
            return await _context.Set<Post>()
                .AnyAsync(p => p.Id == postId);
        }

        /// <summary>
        /// 更新貼文（僅作者可編輯）
        /// </summary>
        public async Task<PostUpdateResult> UpdatePostAsync(int postId, PostUpdateDto updateDto, string userId)
        {
            try
            {
                // 1. 檢查貼文是否存在
                var post = await _context.Set<Post>()
                    .FirstOrDefaultAsync(p => p.Id == postId);

                if (post == null)
                {
                    return PostUpdateResult.CreateError("貼文不存在");
                }

                // 2. 權限檢查：僅作者可編輯
                if (post.AuthorId != userId)
                {
                    return PostUpdateResult.CreateError("您沒有權限編輯此貼文");
                }

                // 3. 作者可以隨時編輯自己的貼文
                // 不設置時間限制

                // 4. 檢查回覆數限制（有回覆的貼文限制編輯範圍）
                var hasComments = await _context.Set<Comment>().AnyAsync(c => c.PostId == postId);
                if (hasComments && post.Type != updateDto.Type)
                {
                    return PostUpdateResult.CreateError("已有回覆的貼文無法修改類型");
                }

                // 5. 更新貼文資料
                post.Title = updateDto.Title;
                post.Content = updateDto.Content;
                post.GameDetailId = updateDto.GameDetailId;
                post.UpdatedAt = DateTime.Now;
                post.ImageUrls = updateDto.ImageUrls; // 補上這行，更新圖片URL

                // 6. 交易相關欄位更新（僅交易類型）
                if (updateDto.Type == PostType.Trade)
                {
                    post.Price = updateDto.Price;
                    post.TradeLocation = updateDto.TradeLocation;
                    post.TradeNotes = updateDto.TradeNotes;
                }
                else
                {
                    // 非交易類型清空交易資料
                    post.Price = null;
                    post.TradeLocation = null;
                    post.TradeNotes = null;
                }

                // 7. 儲存變更
                await _context.SaveChangesAsync();

                // ✅ 清除使用者統計快取
                await InvalidateUserCacheAsync(post.AuthorId);

                // 8. 回傳更新後的資料
                var updatedPost = new
                {
                    post.Id,
                    post.Title,
                    post.Content,
                    post.Type,
                    post.UpdatedAt,
                    TradeInfo = post.Type == PostType.Trade ? new 
                    {
                        Price = post.Price,
                        Currency = "NT$",
                        Location = post.TradeLocation,
                        Notes = post.TradeNotes
                    } : null
                };

                return PostUpdateResult.CreateSuccess(updatedPost, "貼文更新成功");
            }
            catch (Exception ex)
            {
                return PostUpdateResult.CreateError($"更新失敗：{ex.Message}");
            }
        }

        /// <summary>
        /// 刪除貼文（僅作者可刪除）
        /// 處理級聯刪除：留言、按讚記錄、標籤關聯
        /// </summary>
        // 舊簽章：向下相容，預設不是 Admin
        public Task<PostDeleteResult> DeletePostAsync(int postId, string userId)
            => DeletePostAsync(postId, userId, isAdmin: false);

        // 新簽章：作者或 Admin 皆可刪
        public async Task<PostDeleteResult> DeletePostAsync(int postId, string userId, bool isAdmin)
        {
            try
            {
                var post = await _context.Set<Post>()
                    .Include(p => p.Comments)
                    .Include(p => p.Likes)
                    .Include(p => p.PostTags)
                    .FirstOrDefaultAsync(p => p.Id == postId);

                if (post == null)
                {
                    // 保留「找不到」關鍵字，跟 Controller 既有判斷相容
                    return PostDeleteResult.CreateError("找不到貼文");
                }

                // 核心：作者 or Admin 放行
                if (post.AuthorId != userId && !isAdmin)
                {
                    // 保留「權限」關鍵字，跟 Controller 既有判斷相容
                    return PostDeleteResult.CreateError("沒有權限刪除");
                }

                // ===== 以下沿用你原本的刪除流程 =====
                var commentIds = post.Comments.Select(c => c.Id).ToList();
                if (commentIds.Any())
                {
                    var commentLikes = await _context.Set<Like>()
                        .Where(l => l.ItemType == "Comment" && commentIds.Contains(l.ItemId))
                        .ToListAsync();
                    _context.Set<Like>().RemoveRange(commentLikes);
                }

                _context.Set<Like>().RemoveRange(post.Likes);
                _context.Set<Comment>().RemoveRange(post.Comments);
                _context.Set<PostTag>().RemoveRange(post.PostTags);
                _context.Set<Post>().Remove(post);

                await _context.SaveChangesAsync();

                await InvalidateUserCacheAsync(post.AuthorId);
                return PostDeleteResult.CreateSuccess("貼文已成功刪除");
            }
            catch (Exception ex)
            {
                return PostDeleteResult.CreateError($"刪除失敗：{ex.Message}");
            }
        }
    }
}