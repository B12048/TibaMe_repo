using BoardGameFontier.Repostiory;
using BoardGameFontier.Repostiory.Entity;
using BoardGameFontier.Repostiory.Entity.Social;
using BoardGameFontier.Repostiory.DTOS.Social;
using BoardGameFontier.Models;
using Microsoft.EntityFrameworkCore;
using BoardGameFontier.Models.ViewModels;
using BoardGameFontier.Constants;

namespace BoardGameFontier.Services
{
    /// <summary>
    /// 社群服務實作
    /// 負責協調社群頁面相關的業務邏輯
    /// </summary>
    public class CommunityService(
        ApplicationDbContext context,
        ICurrentUserService currentUserService,
        ILogger<CommunityService> logger,
        ICacheService cacheService,
        IFollowService followService) : ICommunityService
    {
        private readonly ApplicationDbContext _context = context;
        private readonly ICurrentUserService _currentUserService = currentUserService;
        private readonly ILogger<CommunityService> _logger = logger;
        private readonly ICacheService _cacheService = cacheService;
        private readonly IFollowService _followService = followService;

        /// <summary>
        /// 取得社群主頁資料
        /// </summary>
        public async Task<CommunityViewModel> GetCommunityPageDataAsync()
        {
            // 重構後：使用核心方法載入社群主頁完整資料
            var options = new CommunityDataLoadOptions
            {
                IncludeGameSelection = true,  // 社群主頁需要遊戲選擇列表（發表貼文用）
                IncludeCarousel = true,       // 社群主頁需要輪播資料
                IncludeAllGames = false       // 社群主頁熱門遊戲限制5個即可
            };

            return await GetBaseCommunityDataAsync(options);
        }

        /// <summary>
        /// 取得側邊欄資料（輕量級版本）
        /// 僅包含使用者資訊、熱門文章、熱門遊戲，不包含輪播資料
        /// 用於非社群頁面的側邊欄快速載入
        /// </summary>
        public async Task<CommunityViewModel> GetSidebarDataAsync()
        {
            // 重構後：使用核心方法載入側邊欄輕量級資料
            var options = new CommunityDataLoadOptions
            {
                IncludeGameSelection = false, // 側邊欄不需要遊戲選擇列表
                IncludeCarousel = false,      // 側邊欄不需要輪播資料
                IncludeAllGames = true        // 側邊欄載入全部熱門遊戲
            };

            return await GetBaseCommunityDataAsync(options);
        }

        /// <summary>
        /// 取得基礎社群資料的核心方法（重構後的共用邏輯）
        /// 避免程式碼重複，提升維護性
        /// </summary>
        /// <param name="options">資料載入選項</param>
        /// <returns>社群頁面 ViewModel</returns>
        private async Task<CommunityViewModel> GetBaseCommunityDataAsync(CommunityDataLoadOptions options)
        {
            try
            {
                // 1. 載入使用者資訊（所有頁面都需要）
                var currentUser = await GetCurrentUserViewModelAsync(_currentUserService.UserId);
                
                // 2. 載入熱門貼文（所有頁面都需要）
                var hotPosts = await GetHotPostsAsync();
                
                // 3. 載入熱門遊戲（依據選項決定數量）
                var hotGames = options.IncludeAllGames 
                    ? await GetHotGamesAsync()  // 側邊欄用：載入全部
                    : await GetHotGamesAsync(5);  // 社群主頁用：限制5個
                
                // 4. 載入追蹤者資料（所有頁面都需要，僅登入用戶）
                var recentFollowers = new List<FollowerViewModel>();
                if (currentUser.IsAuthenticated && !string.IsNullOrEmpty(_currentUserService.UserId))
                {
                    recentFollowers = await GetRecentFollowersAsync(_currentUserService.UserId);
                }
                
                // 5. 載入完整遊戲列表（僅社群主頁需要）
                List<GameSelectionDto> allGames = new();
                if (options.IncludeGameSelection)
                {
                    allGames = await GetAllGamesForSelectionAsync();
                }
                
                // 6. 載入輪播資料（僅社群主頁需要）
                List<CarouselItemViewModel> carouselItems = new();
                if (options.IncludeCarousel)
                {
                    carouselItems = await GetCarouselItemsAsync();
                }

                // 7. 建構並回傳 ViewModel
                return new CommunityViewModel
                {
                    CurrentUser = currentUser,
                    HotPosts = hotPosts,
                    HotGames = hotGames,
                    RecentFollowers = recentFollowers,
                    AllGames = allGames,
                    PageSettings = new CommunityPageSettings
                    {
                        Title = "社群討論",
                        DefaultPageSize = 9,
                        EnableRealTimeUpdates = false,
                        CarouselItems = carouselItems
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "載入社群基礎資料時發生錯誤，選項：{@Options}", options);
                throw;
            }
        }

        /// <summary>
        /// 社群資料載入選項類別
        /// 用於控制 GetBaseCommunityDataAsync 載入不同類型的資料
        /// </summary>
        private class CommunityDataLoadOptions
        {
            /// <summary>
            /// 是否載入完整遊戲選擇列表（發表貼文用）
            /// </summary>
            public bool IncludeGameSelection { get; set; } = false;
            
            /// <summary>
            /// 是否載入輪播資料
            /// </summary>
            public bool IncludeCarousel { get; set; } = false;
            
            /// <summary>
            /// 是否載入全部熱門遊戲（false=限制5個）
            /// </summary>
            public bool IncludeAllGames { get; set; } = true;

            public override string ToString()
            {
                return $"GameSelection: {IncludeGameSelection}, Carousel: {IncludeCarousel}, AllGames: {IncludeAllGames}";
            }
        }

        /// <summary>
        /// 取得熱門貼文排行
        /// </summary>
        public async Task<List<HotPostViewModel>> GetHotPostsAsync(int count = 5)
        {
            try
            {
                // 檢查快取
                var cacheKey = $"hot_posts_{count}";
                var cachedPosts = await _cacheService.GetAsync<List<HotPostViewModel>>(cacheKey);
                
                if (cachedPosts != null)
                {
                    return cachedPosts;
                }

                // 效能優化：使用預計算欄位 + 複合索引友善的查詢
                var thirtyDaysAgo = DateTime.Now.AddDays(-30);
                var postData = await _context.Posts
                    .Where(p => p.CreatedAt >= thirtyDaysAgo && p.Author != null) // 使用變數避免重複計算
                    .OrderByDescending(p => p.LikeCount) // 主要排序：按讚數 (需要複合索引: CreatedAt, LikeCount)
                    .ThenByDescending(p => p.CommentCount) // 次要排序：留言數
                    .ThenByDescending(p => p.CreatedAt) // 第三排序：建立時間
                    .Take(count) // 先篩選 TOP N，減少資料傳輸
                    .Select(p => new 
                    {
                        p.Id,
                        p.Title,
                        p.Content,
                        p.LikeCount,     // 使用預計算欄位，避免子查詢
                        p.CommentCount,  // 使用預計算欄位，避免子查詢
                        p.CreatedAt,
                        p.Type,
                        // 直接選取需要的作者欄位，避免載入整個 Author 物件
                        AuthorDisplayName = p.Author.DisplayName,
                        AuthorUserName = p.Author.UserName
                    })
                    .AsNoTracking() // 不需要變更追蹤，提升查詢效能
                    .ToListAsync();

                var hotPosts = postData.Select((p, index) => new HotPostViewModel
                {
                    Id = p.Id,
                    Rank = index + 1,
                    Title = !string.IsNullOrEmpty(p.Title) ? p.Title : string.Concat(p.Content.AsSpan(0, Math.Min(20, p.Content.Length)), "..."),
                    AuthorName = p.AuthorDisplayName ?? p.AuthorUserName,
                    HotScore = p.LikeCount, // 熱度分數只依照按讚數
                    LikeCount = p.LikeCount,
                    CommentCount = p.CommentCount,
                    CreatedAt = p.CreatedAt,
                    IsFeatured = p.LikeCount > 100, // 暫時邏輯：按讚數超過 100 就是精選
                    Type = p.Type,
                    MetaInfo = $"♥ {p.LikeCount} 讚 • {p.CommentCount} 留言" + (p.LikeCount > 100 ? " • 精選" : "")
                }).ToList();

                // 快取結果（10 分鐘）
                await _cacheService.SetAsync(cacheKey, hotPosts, TimeSpan.FromMinutes(10));

                return hotPosts;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得熱門貼文排行時發生錯誤");
                // 回傳空列表而不是拋出異常，保持頁面可用性
                return [];
            }
        }

        /// <summary>
        /// 取得熱門遊戲排行
        /// </summary>
        public async Task<List<HotGameViewModel>> GetHotGamesAsync(int count = 5)
        {
            try
            {
                // 檢查快取
                var cacheKey = $"hot_games_{count}";
                var cachedGames = await _cacheService.GetAsync<List<HotGameViewModel>>(cacheKey);
                
                if (cachedGames != null)
                {
                    return cachedGames;
                }

                // 優先使用點擊統計排名，確保能顯示足夠數量的遊戲
                var clickBasedRanking = await GetRealHotGamesFromDatabaseAsync(count);
                
                // 如果點擊統計有足夠資料，直接使用
                if (clickBasedRanking.Count >= count)
                {
                    await _cacheService.SetAsync(cacheKey, clickBasedRanking, TimeSpan.FromMinutes(15));
                    return clickBasedRanking;
                }

                // 如果點擊統計不足，補充用社群討論統計
                var thirtyDaysAgo = DateTime.Now.AddDays(-30);
                
                var gameStats = await _context.Posts
                    .Where(p => p.GameDetailId.HasValue && p.CreatedAt >= thirtyDaysAgo)
                    .GroupBy(p => p.GameDetailId!.Value)
                    .Select(g => new
                    {
                        GameDetailId = g.Key,
                        DiscussionCount = g.Count(),
                        HotScore = g.Sum(p => p.LikeCount + p.CommentCount), // 使用預計算欄位
                        LastDiscussionAt = g.Max(p => p.CreatedAt)
                    })
                    .OrderByDescending(g => g.HotScore)
                    .ThenByDescending(g => g.LastDiscussionAt)
                    .Take(count)
                    .AsNoTracking() // 新增：不需要變更追蹤
                    .ToListAsync();

                // 如果社群統計也沒有足夠資料，返回點擊統計結果
                if (gameStats.Count == 0)
                {
                    await _cacheService.SetAsync(cacheKey, clickBasedRanking, TimeSpan.FromMinutes(15));
                    return clickBasedRanking;
                }

                var gameDetailIds = gameStats.Select(g => g.GameDetailId).ToList();
                
                // 使用 Dictionary 提升查詢效率，避免多次 FirstOrDefault 調用
                var gameDetailsDict = await _context.GameDetails
                    .Where(gd => gameDetailIds.Contains(gd.Id))
                    .ToDictionaryAsync(gd => gd.Id, gd => gd);

                // 3. 高效組合結果，使用 Dictionary 查詢取代 FirstOrDefault
                var result = gameStats.Select((stat, index) =>
                {
                    // 使用 TryGetValue 避免 KeyNotFoundException 並提升效能
                    gameDetailsDict.TryGetValue(stat.GameDetailId, out var gameDetail);
                    
                    return new HotGameViewModel
                    {
                        Id = stat.GameDetailId,
                        Rank = index + 1,
                        Name = GetGameDisplayName(gameDetail),
                        Category = GetGameCategory(gameDetail), // 優化分類邏輯
                        DiscussionCount = stat.DiscussionCount,
                        HotScore = stat.HotScore,
                        ImageUrl = gameDetail?.Cover, // 使用遊戲封面
                        MetaInfo = GenerateGameMetaInfo(gameDetail, stat.DiscussionCount, stat.LastDiscussionAt)
                    };
                }).ToList();

                // 快取結果（15 分鐘）
                await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(15));

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得熱門遊戲排行時發生錯誤");
                // 回傳真實遊戲資料而不是拋出異常
                return await GetRealHotGamesFromDatabaseAsync(count);
            }
        }

        /// <summary>
        /// 取得貼文詳細資料 ViewModel - 修正版本，避免 DbContext 並行問題
        /// </summary>
        public async Task<PostDetailViewModel?> GetPostDetailViewModelAsync(int postId, string? currentUserId = null)
        {
            try
            {
                // 第一次查詢：取得貼文基本資料
                var post = await _context.Posts
                    .AsNoTracking()
                    .Include(p => p.Author)
                    .Include(p => p.GameDetail)
                    .FirstOrDefaultAsync(p => p.Id == postId);

                if (post == null)
                {
                    return null;
                }
                
                // 第二次查詢：取得留言資料
                var comments = await _context.Comments
                    .AsNoTracking()
                    .Include(c => c.Author)
                    .Where(c => c.PostId == postId && c.ParentCommentId == null)
                    .OrderBy(c => c.CreatedAt)
                    .ToListAsync();
                
                // 移除未使用的標籤查詢
                var postTags = new List<BoardGameFontier.Repostiory.Entity.Social.PostTag>();
                    
                // 建立簡化的用戶資訊（不查詢資料庫）
                var currentUser = !string.IsNullOrEmpty(currentUserId) ? 
                    new CurrentUserViewModel 
                    {
                        UserId = currentUserId,
                        IsAuthenticated = true,
                        DisplayName = "用戶", // 簡化，不查詢用戶資料
                        UserName = currentUserId,
                        Level = 1,
                        PostsCount = 0,
                        LikesCount = 0,
                        FollowersCount = 0,
                        ProfilePictureUrl = "/img/default-avatar.png"
                    } : 
                    new CurrentUserViewModel { IsAuthenticated = false, DisplayName = "訪客" };

                // 重用已查詢的post資料
                var isAuthor = !string.IsNullOrEmpty(currentUserId) && post.AuthorId == currentUserId;
                var isAuthenticated = !string.IsNullOrEmpty(currentUserId);

                var permissions = new PostPermissionViewModel
                {
                    CanEdit = isAuthor,
                    CanDelete = isAuthor,
                    CanComment = isAuthenticated,
                    CanLike = isAuthenticated,
                    CanShare = true,
                    CanReport = isAuthenticated && !isAuthor
                };

                // 相關文章推薦較耗時，如需要可以異步載入
                List<RelatedPostViewModel> relatedPosts = [];

                // 建立 PostViewModel
                var postViewModel = MapToPostViewModel(post, postTags);
                
                // 查詢當前用戶對作者的追蹤狀態
                if (!string.IsNullOrEmpty(currentUserId) && currentUserId != post.AuthorId)
                {
                    var isFollowing = await _context.Set<Follow>()
                        .AsNoTracking()
                        .AnyAsync(f => f.FollowerId == currentUserId && f.FolloweeId == post.AuthorId);
                    postViewModel.Author.IsFollowed = isFollowing;
                }

                List<CommentViewModel> commentViewModels;

                // 批次查詢所有按讚狀態（單一查詢）
                if (!string.IsNullOrEmpty(currentUserId))
                {
                    var commentIds = comments.Select(c => c.Id).ToList();
                    var allItemIds = new List<int> { postId };
                    allItemIds.AddRange(commentIds);
                    
                    // 單一查詢取得所有按讚狀態
                    var likedItems = await _context.Likes
                        .AsNoTracking()
                        .Where(l => l.UserId == currentUserId && allItemIds.Contains(l.ItemId))
                        .Select(l => new { l.ItemId, l.ItemType })
                        .ToListAsync();
                        
                    var likedPostIds = likedItems.Where(l => l.ItemType == LikeConstants.ItemTypes.Post).Select(l => l.ItemId).ToHashSet();
                    var likedCommentIds = likedItems.Where(l => l.ItemType == LikeConstants.ItemTypes.Comment).Select(l => l.ItemId).ToHashSet();
                    
                    postViewModel.IsLikedByCurrentUser = likedPostIds.Contains(postId);
                    commentViewModels = MapToCommentViewModelsWithLikeStatus(comments, likedCommentIds);
                }
                else
                {
                    commentViewModels = MapToCommentViewModels(comments);
                }

                return new PostDetailViewModel
                {
                    Post = postViewModel,
                    Comments = commentViewModels,
                    RelatedPosts = relatedPosts,
                    CurrentUser = currentUser,
                    Permissions = permissions,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得貼文詳細資料時發生錯誤，PostId: {PostId}", postId);
                throw;
            }
        }

        /// <summary>
        /// 取得當前使用者資訊 ViewModel
        /// 修正：認證狀態不快取，避免登入登出狀態延遲
        /// </summary>
        public async Task<CurrentUserViewModel> GetCurrentUserViewModelAsync(string? userId = null)
        {
            // 總是使用 CurrentUserService 取得最新的認證狀態，不快取
            var isAuthenticated = _currentUserService.IsAuthenticated;
            var currentUserId = _currentUserService.UserId;
            
            // 如果傳入的 userId 為空或使用者未認證，回傳訪客資訊
            if (string.IsNullOrEmpty(currentUserId) || !isAuthenticated)
            {
                return new CurrentUserViewModel
                {
                    IsAuthenticated = false,
                    DisplayName = "訪客"
                };
            }

            try
            {
                // 檢查使用者統計資料的快取（不包含認證狀態）
                var cacheKey = $"user_stats_{currentUserId}";
                var cachedStats = await _cacheService.GetAsync<UserStatsCache>(cacheKey);
                
                // 如果有快取的統計資料，直接使用
                if (cachedStats != null)
                {
                    return new CurrentUserViewModel
                    {
                        UserId = currentUserId,
                        IsAuthenticated = isAuthenticated, // 總是使用最新的認證狀態
                        DisplayName = cachedStats.DisplayName,
                        UserName = cachedStats.UserName,
                        Level = cachedStats.Level,
                        PostsCount = cachedStats.PostsCount,
                        LikesCount = cachedStats.LikesCount,
                        FollowersCount = cachedStats.FollowersCount,
                        ProfilePictureUrl = cachedStats.ProfilePictureUrl
                    };
                }

                var userProfile = await _context.UserProfiles
                    .FirstOrDefaultAsync(u => u.Id == currentUserId);

                if (userProfile == null)
                {
                    return new CurrentUserViewModel
                    {
                        IsAuthenticated = false,
                        DisplayName = "訪客"
                    };
                }

                // 優化統計查詢：使用更直接的查詢避免複雜 Join
                var postsCount = await _context.Posts.CountAsync(p => p.AuthorId == currentUserId);
                
                // 直接統計該使用者貼文的總讚數，避免複雜的 Join
                var likesCount = await _context.Posts
                    .Where(p => p.AuthorId == currentUserId)
                    .SumAsync(p => p.LikeCount);

                // 取得追蹤統計
                var followStats = await _followService.GetFollowCountsAsync(currentUserId);

                // 快取統計資料（不包含認證狀態）
                var statsToCache = new UserStatsCache
                {
                    DisplayName = userProfile.DisplayName ?? userProfile.UserName,
                    UserName = userProfile.UserName,
                    Level = 1, // TODO: 實作使用者等級系統
                    PostsCount = postsCount,
                    LikesCount = likesCount,
                    FollowersCount = followStats.FollowersCount, // 使用 FollowService 取得真實數據
                    ProfilePictureUrl = userProfile.ProfilePictureUrl
                };

                // 快取統計資料（5 分鐘）
                await _cacheService.SetAsync(cacheKey, statsToCache, TimeSpan.FromMinutes(5));

                return new CurrentUserViewModel
                {
                    UserId = currentUserId,
                    IsAuthenticated = isAuthenticated, // 總是使用最新的認證狀態
                    DisplayName = statsToCache.DisplayName,
                    UserName = statsToCache.UserName,
                    Level = statsToCache.Level,
                    PostsCount = statsToCache.PostsCount,
                    LikesCount = statsToCache.LikesCount,
                    FollowersCount = statsToCache.FollowersCount,
                    ProfilePictureUrl = statsToCache.ProfilePictureUrl
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得使用者資訊時發生錯誤，UserId: {UserId}", currentUserId);
                return new CurrentUserViewModel
                {
                    IsAuthenticated = false,
                    DisplayName = "訪客"
                };
            }
        }

        /// <summary>
        /// 使用者統計資料快取模型（不包含認證狀態）
        /// </summary>
        private class UserStatsCache
        {
            public string DisplayName { get; set; } = "";
            public string UserName { get; set; } = "";
            public int Level { get; set; }
            public int PostsCount { get; set; }
            public int LikesCount { get; set; }
            public int FollowersCount { get; set; }
            public string? ProfilePictureUrl { get; set; }
        }

        /// <summary>
        /// 取得輪播設定
        /// </summary>
        public Task<List<CarouselItemViewModel>> GetCarouselItemsAsync()
        {
            // TODO: 實作動態輪播設定，目前回傳靜態資料
            List<CarouselItemViewModel> carouselItems = [
                new()
                {
                    ImageUrl = "/img/arkham-horror.jpg",
                    Title = "阿卡姆驚魂",
                    Description = "探索洛夫克拉夫特式恐怖世界",
                    LinkUrl = "https://www.arkhamhorror.com/games/arkham-horror-the-card-game/",
                    OpenInNewTab = true
                },
                new()
                {
                    ImageUrl = "/img/compania.jpg",
                    Title = "Compania",
                    Description = "建立你的工業帝國",
                    LinkUrl = "https://www.kickstarter.com/projects/asobition/compania",
                    OpenInNewTab = true
                },
                new()
                {
                    ImageUrl = "/img/stitch-the-fix.jpg",
                    Title = "Stitch The Fix",
                    Description = "迪士尼史迪奇主題拼圖遊戲",
                    LinkUrl = "https://www.zmangames.com/game/the-fix-for-626/",
                    OpenInNewTab = true
                }
            ];

            return Task.FromResult(carouselItems);
        }

        #region Private Helper Methods

        /// <summary>
        /// 將 Post 實體轉換為 PostViewModel
        /// </summary>
        private static PostViewModel MapToPostViewModel(Post post)
        {
            return MapToPostViewModel(post, post.PostTags?.ToList() ?? []);
        }

        private static PostViewModel MapToPostViewModel(Post post, List<PostTag> postTags)
        {
            return new PostViewModel
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                Type = post.Type,
                Author = new AuthorViewModel
                {
                    Id = post.Author.Id,
                    UserName = post.Author.UserName,
                    DisplayName = post.Author.DisplayName ?? post.Author.UserName,
                    ProfilePictureUrl = post.Author.ProfilePictureUrl,
                    Level = 1, // TODO: 實作使用者等級
                    IsVerified = false // TODO: 實作驗證系統
                },
                RelatedGame = post.GameDetail == null ? null : new RelatedGameViewModel
                {
                    Id = post.GameDetail.Id,
                    Name = !string.IsNullOrEmpty(post.GameDetail.zhtTitle) ? post.GameDetail.zhtTitle : post.GameDetail.engTitle,
                    Category = "策略" // TODO: 從 GameDetail 取得真實分類
                },
                Stats = new PostStatsViewModel
                {
                    LikeCount = post.LikeCount,
                    CommentCount = post.CommentCount,
                    ShareCount = 0, // TODO: 實作分享統計
                    FavoriteCount = 0 // TODO: 實作收藏統計
                },
                Time = new PostTimeViewModel
                {
                    CreatedAt = post.CreatedAt,
                    UpdatedAt = post.UpdatedAt
                },
                TradeInfo = post.Type == PostType.Trade ? new TradeInfoViewModel
                {
                    Price = post.Price,
                    Currency = "NT$",
                    Status = TradeStatus.Available, // TODO: 實作交易狀態
                    Location = post.TradeLocation,
                    Notes = post.TradeNotes
                } : null,
                ImageUrls = string.IsNullOrEmpty(post.ImageUrls) 
                    ? [] 
                    : [.. post.ImageUrls.Split(';', StringSplitOptions.RemoveEmptyEntries)],
                Tags = [.. postTags.Select(pt => new PostTagViewModel
                {
                    Id = pt.Tag.Id,
                    Name = pt.Tag.Name,
                    Color = pt.Tag.Color,
                    UsageCount = pt.Tag.UsageCount
                })],
                Status = new PostStatusViewModel
                {
                    IsPopular = post.IsPopular,
                    IsPinned = post.IsPinned,
                    IsFeatured = post.LikeCount > 100, // 暫時邏輯
                    IsLocked = false // TODO: 實作鎖定功能
                },
                IsLikedByCurrentUser = false // 預設值，需要在調用時設定
            };
        }

        /// <summary>
        /// 將 Comment 實體列表轉換為 CommentViewModel 列表
        /// </summary>
        private static List<CommentViewModel> MapToCommentViewModels(List<Comment> comments)
        {
            return [.. comments.Select(c => new CommentViewModel
            {
                Id = c.Id,
                Content = c.Content,
                Author = new AuthorViewModel
                {
                    Id = c.Author.Id,
                    UserName = c.Author.UserName,
                    DisplayName = c.Author.DisplayName ?? c.Author.UserName,
                    ProfilePictureUrl = c.Author.ProfilePictureUrl,
                    Level = 1,
                    IsVerified = false
                },
                ParentCommentId = c.ParentCommentId,
                Replies = [.. c.Replies?.Select(r => new CommentViewModel
                {
                    Id = r.Id,
                    Content = r.Content,
                    Author = new AuthorViewModel
                    {
                        Id = r.Author.Id,
                        UserName = r.Author.UserName,
                        DisplayName = r.Author.DisplayName ?? r.Author.UserName,
                        ProfilePictureUrl = r.Author.ProfilePictureUrl,
                        Level = 1,
                        IsVerified = false
                    },
                    ParentCommentId = r.ParentCommentId,
                    LikeCount = r.LikeCount,
                    CreatedAt = r.CreatedAt,
                    IsLikedByCurrentUser = false 
                }) ?? []],
                LikeCount = c.LikeCount,
                CreatedAt = c.CreatedAt,
                IsLikedByCurrentUser = false
            })];
        }

        /// <summary>
        /// 將 Comment 實體列表轉換為 CommentViewModel 列表（包含按讚狀態）- 使用 HashSet 版本
        /// </summary>
        private static List<CommentViewModel> MapToCommentViewModelsWithLikeStatus(List<Comment> comments, HashSet<int> likedCommentIds)
        {
            return comments.Select(c => new CommentViewModel
            {
                Id = c.Id,
                Content = c.Content,
                Author = new AuthorViewModel
                {
                    Id = c.Author.Id,
                    UserName = c.Author.UserName,
                    DisplayName = c.Author.DisplayName ?? c.Author.UserName,
                    ProfilePictureUrl = c.Author.ProfilePictureUrl,
                    Level = 1,
                    IsVerified = false
                },
                ParentCommentId = c.ParentCommentId,
                CreatedAt = c.CreatedAt,
                LikeCount = c.LikeCount,
                IsLikedByCurrentUser = likedCommentIds.Contains(c.Id),
                Replies = [] // 簡化，不載入回覆
            }).ToList();
        }


        /// <summary>
        /// 從資料庫取得真實的熱門遊戲資料（基於圖鑑點擊統計）
        /// 使用 GameClickLog 的點擊統計來排序熱門遊戲
        /// </summary>
        private async Task<List<HotGameViewModel>> GetRealHotGamesFromDatabaseAsync(int count)
        {
            try
            {
                // 計算最近30天的點擊統計
                var thirtyDaysAgo = DateTime.Now.AddDays(-30);
                
                // 統計各遊戲的點擊次數
                var gameClickStats = await _context.GameClickLog
                    .Where(log => log.ClickedTime >= thirtyDaysAgo)
                    .GroupBy(log => log.GameDetailId)
                    .Select(g => new
                    {
                        GameDetailId = g.Key,
                        ClickCount = g.Count(),
                        LastClickTime = g.Max(log => log.ClickedTime)
                    })
                    .OrderByDescending(stat => stat.ClickCount)
                    .ThenByDescending(stat => stat.LastClickTime) // 相同點擊數時按最後點擊時間排序
                    .Take(count)
                    .ToListAsync();

                if (gameClickStats.Count == 0)
                {
                    // 如果沒有點擊統計，直接從資料庫取得真實遊戲（按ID排序）
                    var realGames = await _context.GameDetails
                        .Where(g => !string.IsNullOrEmpty(g.zhtTitle) || !string.IsNullOrEmpty(g.engTitle))
                        .OrderBy(g => g.Id) // 按ID排序，確保順序一致
                        .Take(count)
                        .ToListAsync();

                    _logger.LogInformation("沒有找到點擊統計資料，返回 {DefaultGameCount} 個預設遊戲", realGames.Count);

                    return realGames.Select((game, index) => new HotGameViewModel
                    {
                        Id = game.Id,
                        Rank = index + 1,
                        Name = GetGameDisplayName(game),
                        Category = GetGameCategory(game),
                        DiscussionCount = 0, // 沒有點擊統計時為0
                        HotScore = 0, // 沒有點擊統計時為0
                        ImageUrl = game.Cover,
                        MetaInfo = $"{GetGameCategory(game)} • 等待點擊統計"
                    }).ToList();
                }

                // 批次載入遊戲詳細資料
                var gameDetailIds = gameClickStats.Select(stat => stat.GameDetailId).ToList();
                var gameDetailsDict = await _context.GameDetails
                    .Where(gd => gameDetailIds.Contains(gd.Id))
                    .ToDictionaryAsync(gd => gd.Id, gd => gd);

                // 建立熱門遊戲視圖模型
                var result = gameClickStats.Select((stat, index) =>
                {
                    gameDetailsDict.TryGetValue(stat.GameDetailId, out var gameDetail);
                    
                    return new HotGameViewModel
                    {
                        Id = stat.GameDetailId,
                        Rank = index + 1,
                        Name = GetGameDisplayName(gameDetail),
                        Category = GetGameCategory(gameDetail),
                        DiscussionCount = stat.ClickCount, // 顯示點擊次數
                        HotScore = stat.ClickCount, // 使用點擊次數作為熱度分數
                        ImageUrl = gameDetail?.Cover,
                        MetaInfo = $"{GetGameCategory(gameDetail)} • {stat.ClickCount}次點擊 • {CalculateTimeAgo(stat.LastClickTime)}"
                    };
                }).ToList();

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "從資料庫取得真實遊戲資料時發生錯誤");
                // 最後的備援方案才使用假資料
                return GetDefaultHotGames(count);
            }
        }

        /// <summary>
        /// 取得預設熱門遊戲資料（當資料庫查詢失敗時使用）
        /// </summary>
        private static List<HotGameViewModel> GetDefaultHotGames(int count)
        {
            HotGameViewModel[] defaultGames = [
                new() { Id = 1, Rank = 1, Name = "璀璨寶石", Category = "策略", DiscussionCount = 45, HotScore = 120, MetaInfo = "策略 • 45篇討論" },
                new() { Id = 2, Rank = 2, Name = "卡坦島", Category = "經典", DiscussionCount = 32, HotScore = 89, MetaInfo = "經典 • 32篇討論" },
                new() { Id = 3, Rank = 3, Name = "七大奇蹟", Category = "文明", DiscussionCount = 28, HotScore = 76, MetaInfo = "文明 • 28篇討論" },
                new() { Id = 4, Rank = 4, Name = "翼展", Category = "引擎", DiscussionCount = 24, HotScore = 65, MetaInfo = "引擎 • 24篇討論" },
                new() { Id = 5, Rank = 5, Name = "農家樂", Category = "工人擺放", DiscussionCount = 19, HotScore = 52, MetaInfo = "工人擺放 • 19篇討論" }
            ];

            return [.. defaultGames.Take(count)];
        }

        #endregion

        #region 個人貼文管理功能

        /// <summary>
        /// 取得使用者的個人貼文管理頁面資料 - 優化版本：單一查詢合併所有資料
        /// </summary>
        public async Task<MyPostsViewModel> GetMyPostsPageDataAsync(string userId, PostFilterOptions? filterOptions = null, int page = 1)
        {
            try
            {
                filterOptions ??= new();
                var pageSize = filterOptions.PageSize > 0 ? filterOptions.PageSize : 9;

                // 優化：單一查詢取得貼文、統計和使用者資料
                var combinedQuery = from p in _context.Posts.AsNoTracking()
                                      .Include(p => p.Author)
                                      .Include(p => p.GameDetail)
                                  where p.AuthorId == userId
                                  select new
                                  {
                                      // 貼文基本資料
                                      Post = p,
                                      Author = p.Author,
                                      GameDetail = p.GameDetail,
                                      // 統計資料（每行計算一次）
                                      TotalLikes = p.LikeCount,
                                      TotalComments = p.CommentCount
                                  };

                // 套用篩選條件
                if (filterOptions.FilterType.HasValue)
                {
                    combinedQuery = combinedQuery.Where(x => x.Post.Type == filterOptions.FilterType.Value);
                }

                if (!string.IsNullOrWhiteSpace(filterOptions.SearchKeyword))
                {
                    var keyword = filterOptions.SearchKeyword;
                    combinedQuery = combinedQuery.Where(x => 
                        (x.Post.Title != null && x.Post.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase)) || 
                        x.Post.Content.Contains(keyword, StringComparison.OrdinalIgnoreCase));
                }

                // 套用排序
                combinedQuery = filterOptions.SortOrder switch
                {
                    PostSortOrder.CreatedAsc => combinedQuery.OrderBy(x => x.Post.CreatedAt),
                    PostSortOrder.UpdatedDesc => combinedQuery.OrderByDescending(x => x.Post.UpdatedAt),
                    PostSortOrder.PopularityDesc => combinedQuery.OrderByDescending(x => x.Post.LikeCount + x.Post.CommentCount),
                    PostSortOrder.CommentsDesc => combinedQuery.OrderByDescending(x => x.Post.CommentCount),
                    _ => combinedQuery.OrderByDescending(x => x.Post.CreatedAt)
                };

                // 改為順序執行避免 DbContext 併發錯誤
                var totalCount = await combinedQuery.CountAsync();
                
                var pagedData = await combinedQuery
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                // 暫時禁用快取 - 如需要快取功能可在此處實作

                // 取得使用者資訊
                var currentUser = await _context.UserProfiles
                    .AsNoTracking()
                    .Where(u => u.Id == userId)
                    .Select(u => new CurrentUserViewModel
                    {
                        UserId = u.Id,
                        UserName = u.UserName,
                        DisplayName = u.DisplayName ?? u.UserName,
                        ProfilePictureUrl = u.ProfilePictureUrl,
                        IsAuthenticated = true
                    })
                    .FirstOrDefaultAsync() ?? new CurrentUserViewModel();

                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                // 如果有貼文，批次載入留言資料
                Dictionary<int, List<Comment>> commentsDict = new();
                if (pagedData.Any())
                {
                    var postIds = pagedData.Select(x => x.Post.Id).ToList();
                    commentsDict = await _context.Comments
                        .AsNoTracking()
                        .Include(c => c.Author)
                        .Where(c => postIds.Contains(c.PostId))
                        .GroupBy(c => c.PostId)
                        .ToDictionaryAsync(g => g.Key, g => g.OrderByDescending(c => c.CreatedAt).Take(3).ToList());
                }

                // 優化：在記憶體中轉換為 ViewModel，避免額外查詢
                var userPosts = new List<UserPostViewModel>();
                foreach (var item in pagedData)
                {
                    var post = item.Post;
                    var comments = commentsDict.GetValueOrDefault(post.Id, []);
                    
                    // 直接建立 ViewModel 避免呼叫 ConvertToUserPostViewModelAsync
                    var userPost = new UserPostViewModel
                    {
                        Id = post.Id,
                        Title = post.Title ?? string.Empty,
                        ContentPreview = post.Content.Length > 100 
                            ? string.Concat(post.Content.AsSpan(0, 100), "...") 
                            : post.Content,
                        FullContent = post.Content,
                        Type = post.Type,
                        Price = post.Price,
                        TradeLocation = post.TradeLocation,
                        TradeNotes = post.TradeNotes,
                        RelatedGame = item.GameDetail != null ? new RelatedGameViewModel
                        {
                            Id = item.GameDetail.Id,
                            Name = item.GameDetail.zhtTitle ?? item.GameDetail.engTitle ?? "未知遊戲"
                        } : null,
                        CreatedAt = post.CreatedAt,
                        UpdatedAt = post.UpdatedAt,
                        TimeDisplay = new PostTimeViewModel
                        {
                            CreatedAgo = CalculateTimeAgo(post.CreatedAt),
                            UpdatedAgo = CalculateTimeAgo(post.UpdatedAt),
                            FormattedCreated = post.CreatedAt.ToString("yyyy/MM/dd HH:mm"),
                            FormattedUpdated = post.UpdatedAt.ToString("yyyy/MM/dd HH:mm")
                        },
                        Stats = new PostStatsViewModel
                        {
                            LikeCount = post.LikeCount,
                            CommentCount = post.CommentCount,
                            ShareCount = 0
                        },
                        RecentResponses = comments.Select(c => new RecentResponseViewModel
                        {
                            Id = c.Id,
                            ContentPreview = c.Content.Length > 50 
                                ? string.Concat(c.Content.AsSpan(0, 50), "...") 
                                : c.Content,
                            Author = new AuthorViewModel
                            {
                                Id = c.Author.Id,
                                UserName = c.Author.UserName,
                                DisplayName = c.Author.DisplayName ?? c.Author.UserName,
                                ProfilePictureUrl = c.Author.ProfilePictureUrl
                            },
                            CreatedAt = c.CreatedAt,
                            TimeAgo = CalculateTimeAgo(c.CreatedAt),
                            IsNew = false
                        }).ToList(),
                        HasNewResponses = false,
                        NewResponseCount = 0,
                        Permissions = new PostActionPermissions
                        {
                            CanEdit = true,    // 自己的貼文都可編輯
                            CanDelete = true,  // 自己的貼文都可刪除
                            CanView = true
                        }
                    };
                    userPosts.Add(userPost);
                }

                // 暫時計算統計資料（不使用快取）
                var tempStats = await _context.Posts
                    .Where(p => p.AuthorId == userId)
                    .GroupBy(p => p.AuthorId)
                    .Select(g => new
                    {
                        TotalPosts = g.Count(),
                        TotalLikes = g.Sum(p => p.LikeCount),
                        TotalComments = g.Sum(p => p.CommentCount),
                        PostsThisMonth = g.Count(p => p.CreatedAt >= DateTime.Now.AddDays(-30))
                    })
                    .FirstOrDefaultAsync();

                var stats = new UserPostStatsViewModel
                {
                    TotalPosts = tempStats?.TotalPosts ?? 0,
                    TotalLikes = tempStats?.TotalLikes ?? 0,
                    TotalComments = tempStats?.TotalComments ?? 0,
                    PostsThisMonth = tempStats?.PostsThisMonth ?? 0,
                    PostsByType = new Dictionary<PostType, int>(), // 暫時空的
                    PostsWithNewResponses = 0
                };

                // 優化後的除錯資訊
                _logger.LogInformation("GetMyPostsPageDataAsync 優化版本: userId={UserId}, PostsCount={PostsCount}, TotalPosts={TotalPosts}, TotalLikes={TotalLikes}, 查詢數量: 4個並行查詢",
                    userId, userPosts.Count, stats.TotalPosts, stats.TotalLikes);

                return new MyPostsViewModel
                {
                    CurrentUser = currentUser,
                    MyPosts = userPosts,
                    Pagination = new PaginationViewModel
                    {
                        CurrentPage = page,
                        PageSize = pageSize,
                        TotalItems = totalCount,
                        TotalPages = totalPages,
                        HasPreviousPage = page > 1,
                        HasNextPage = page < totalPages
                    },
                    FilterOptions = filterOptions,
                    Stats = stats
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得個人貼文管理頁面資料時發生錯誤，使用者ID: {UserId}", userId);
                throw;
            }
        }

        /// <summary>
        /// 取得使用者的貼文統計資訊
        /// </summary>
        public async Task<UserPostStatsViewModel> GetUserPostStatsAsync(string userId)
        {
            try
            {
                var posts = await _context.Posts
                    .Where(p => p.AuthorId == userId)
                    .ToListAsync();

                var stats = new UserPostStatsViewModel
                {
                    TotalPosts = posts.Count,
                    PostsByType = posts.GroupBy(p => p.Type)
                         .ToDictionary(g => g.Key, g => g.Count()),
                    TotalLikes = posts.Sum(p => p.LikeCount),
                    TotalComments = posts.Sum(p => p.CommentCount),
                    PostsThisMonth = posts.Count(p => p.CreatedAt >= DateTime.Now.AddDays(-30)),
                    PostsWithNewResponses = 0
                };

                _logger.LogInformation("GetUserPostStatsAsync: userId={UserId}, TotalPosts={TotalPosts}, TotalLikes={TotalLikes}, TotalComments={TotalComments}",
                    userId, stats.TotalPosts, stats.TotalLikes, stats.TotalComments); 

                return stats;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得使用者貼文統計時發生錯誤，使用者ID: {UserId}", userId);
                throw;
            }
        }

        /// <summary>
        /// 取得單一使用者貼文的詳細資訊（用於編輯）
        /// </summary>
        public async Task<UserPostViewModel?> GetUserPostForEditAsync(int postId, string userId)
        {
            try
            {
                var post = await _context.Posts
                    .Include(p => p.Author)
                    .Include(p => p.GameDetail)
                    .Include(p => p.Comments)
                        .ThenInclude(c => c.Author)
                    .FirstOrDefaultAsync(p => p.Id == postId && p.AuthorId == userId);

                if (post == null)
                {
                    return null;
                }

                return await ConvertToUserPostViewModelAsync(post);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得使用者貼文編輯資料時發生錯誤，貼文ID: {PostId}，使用者ID: {UserId}", postId, userId);
                throw;
            }
        }

        /// <summary>
        /// 取得貼文的最近回應列表
        /// </summary>
        public async Task<List<RecentResponseViewModel>> GetPostRecentResponsesAsync(int postId, string userId, int count = 10)
        {
            try
            {
                // 先確認貼文屬於該使用者
                var postExists = await _context.Posts
                    .AnyAsync(p => p.Id == postId && p.AuthorId == userId);

                if (!postExists)
                {
                    return [];
                }

                var comments = await _context.Comments
                    .Include(c => c.Author)
                    .Where(c => c.PostId == postId)
                    .OrderByDescending(c => c.CreatedAt)
                    .Take(count)
                    .ToListAsync();

                return [.. comments.Select(c => new RecentResponseViewModel
                {
                    Id = c.Id,
                    ContentPreview = c.Content.Length > 100 ? string.Concat(c.Content.AsSpan(0, 100), "...") : c.Content,
                    Author = new AuthorViewModel
                    {
                        Id = c.Author.Id,
                        UserName = c.Author.UserName,
                        DisplayName = c.Author.DisplayName ?? c.Author.UserName,
                        ProfilePictureUrl = c.Author.ProfilePictureUrl
                    },
                    CreatedAt = c.CreatedAt,
                    TimeAgo = CalculateTimeAgo(c.CreatedAt),
                    IsNew = false // TODO: 實作新回應判斷邏輯
                })];
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得貼文最近回應時發生錯誤，貼文ID: {PostId}，使用者ID: {UserId}", postId, userId);
                throw;
            }
        }

        /// <summary>
        /// 檢查貼文是否有新回應
        /// </summary>
        public async Task<int> GetNewResponseCountAsync(int postId, string userId, DateTime? lastViewTime = null)
        {
            try
            {
                // 先確認貼文屬於該使用者
                var postExists = await _context.Posts
                    .AnyAsync(p => p.Id == postId && p.AuthorId == userId);

                if (!postExists)
                {
                    return 0;
                }

                // 如果沒有提供最後檢視時間，使用24小時前作為預設
                lastViewTime ??= DateTime.Now.AddDays(-1);

                var newCommentCount = await _context.Comments
                    .Where(c => c.PostId == postId && 
                              c.CreatedAt > lastViewTime.Value &&
                              c.AuthorId != userId) // 排除自己的回應
                    .CountAsync();

                return newCommentCount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "檢查新回應時發生錯誤，貼文ID: {PostId}，使用者ID: {UserId}", postId, userId);
                return 0;
            }
        }

        /// <summary>
        /// 標記貼文回應為已讀
        /// </summary>
        public Task<bool> MarkPostResponsesAsReadAsync(int postId, string userId)
        {
            try
            {
                // TODO: 實作已讀狀態追蹤
                // 可以考慮在資料庫中新增 UserPostViewHistory 表格來追蹤已讀狀態
                _logger.LogInformation("標記貼文回應為已讀，貼文ID: {PostId}，使用者ID: {UserId}", postId, userId);
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "標記貼文回應為已讀時發生錯誤，貼文ID: {PostId}，使用者ID: {UserId}", postId, userId);
                return Task.FromResult(false);
            }
        }

        /// <summary>
        /// 軟刪除使用者貼文
        /// </summary>
        public async Task<ServiceResult<bool>> SoftDeleteUserPostAsync(int postId, string userId)
        {
            try
            {
                var post = await _context.Posts
                    .FirstOrDefaultAsync(p => p.Id == postId && p.AuthorId == userId);

                if (post == null)
                {
                    return ServiceResult<bool>.Failure("找不到指定的貼文或您沒有權限刪除");
                }

                // 檢查是否可以刪除
                var (canDelete, reason) = await CanUserDeletePostAsync(postId, userId);
                if (!canDelete)
                {
                    return ServiceResult<bool>.Failure(reason ?? "無法刪除此貼文");
                }

                // 軟刪除：暫時移除貼文（可以考慮新增 IsDeleted 欄位）
                _context.Posts.Remove(post);
                await _context.SaveChangesAsync();

                _logger.LogInformation("成功刪除貼文，貼文ID: {PostId}，使用者ID: {UserId}", postId, userId);
                return ServiceResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "刪除貼文時發生錯誤，貼文ID: {PostId}，使用者ID: {UserId}", postId, userId);
                return ServiceResult<bool>.Failure("刪除貼文時發生錯誤，請稍後再試");
            }
        }

        /// <summary>
        /// 檢查使用者是否可以編輯指定貼文
        /// </summary>
        public async Task<(bool CanEdit, string? Reason)> CanUserEditPostAsync(int postId, string userId)
        {
            try
            {
                var post = await _context.Posts
                    .Include(p => p.Comments)
                    .FirstOrDefaultAsync(p => p.Id == postId);

                if (post == null)
                {
                    return (false, "貼文不存在");
                }

                if (post.AuthorId != userId)
                {
                    return (false, "您不是此貼文的作者");
                }

                // 作者可以隨時編輯自己的貼文
                // 不設置時間限制和回應數量限制

                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "檢查編輯權限時發生錯誤，貼文ID: {PostId}，使用者ID: {UserId}", postId, userId);
                return (false, "檢查權限時發生錯誤");
            }
        }

        /// <summary>
        /// 檢查使用者是否可以刪除指定貼文
        /// </summary>
        public async Task<(bool CanDelete, string? Reason)> CanUserDeletePostAsync(int postId, string userId)
        {
            try
            {
                var post = await _context.Posts
                    .Include(p => p.Comments)
                    .FirstOrDefaultAsync(p => p.Id == postId);

                if (post == null)
                {
                    return (false, "貼文不存在");
                }

                if (post.AuthorId != userId)
                {
                    return (false, "您不是此貼文的作者");
                }

                // 社群頁面改的：移除不合理的48小時刪除限制
                // 使用者應該可以隨時刪除自己的貼文（除非有其他業務邏輯限制）
                // var deleteTimeLimit = TimeSpan.FromHours(48);
                // if (DateTime.Now - post.CreatedAt > deleteTimeLimit)
                // {
                //     return (false, "貼文發布超過48小時，無法刪除");
                // }

                // 交易型貼文如果有回應則不可刪除
                if (post.Type == PostType.Trade && post.Comments.Count > 0)
                {
                    return (false, "交易貼文已有回應，無法刪除");
                }

                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "檢查刪除權限時發生錯誤，貼文ID: {PostId}，使用者ID: {UserId}", postId, userId);
                return (false, "檢查權限時發生錯誤");
            }
        }

        /// <summary>
        /// 轉換 Post 實體為 UserPostViewModel
        /// </summary>
        private async Task<UserPostViewModel> ConvertToUserPostViewModelAsync(Post post, List<Comment>? comments = null)
        {
            // 使用傳入的留言列表，如果沒有則從 post.Comments 取得（向後相容）
            var postComments = comments ?? post.Comments?.ToList() ?? [];
            
            var recentResponses = postComments
                .Select(c => new RecentResponseViewModel
                {
                    Id = c.Id,
                    ContentPreview = c.Content.Length > 100 ? string.Concat(c.Content.AsSpan(0, 100), "...") : c.Content,
                    Author = new AuthorViewModel
                    {
                        Id = c.Author.Id,
                        UserName = c.Author.UserName,
                        DisplayName = c.Author.DisplayName ?? c.Author.UserName,
                        ProfilePictureUrl = c.Author.ProfilePictureUrl
                    },
                    CreatedAt = c.CreatedAt,
                    TimeAgo = CalculateTimeAgo(c.CreatedAt),
                    IsNew = false // TODO: 實作新回應判斷邏輯
                }).ToList();

            var (canEdit, editReason) = await CanUserEditPostAsync(post.Id, post.AuthorId);
            var (canDelete, deleteReason) = await CanUserDeletePostAsync(post.Id, post.AuthorId);

            return new UserPostViewModel
            {
                Id = post.Id,
                Title = post.Title ?? string.Empty,
                ContentPreview = post.Content.Length > 100 ? string.Concat(post.Content.AsSpan(0, 100), "...") : post.Content,
                FullContent = post.Content,
                Type = post.Type,
                Price = post.Price,
                TradeLocation = post.TradeLocation,
                TradeNotes = post.TradeNotes,
                RelatedGame = post.GameDetail != null ? new RelatedGameViewModel
                {
                    Id = post.GameDetail.Id,
                    Name = !string.IsNullOrEmpty(post.GameDetail.zhtTitle) ? 
                           post.GameDetail.zhtTitle : post.GameDetail.engTitle
                } : null,
                CreatedAt = post.CreatedAt,
                UpdatedAt = post.UpdatedAt,
                TimeDisplay = new PostTimeViewModel
                {
                    CreatedAgo = CalculateTimeAgo(post.CreatedAt),
                    UpdatedAgo = CalculateTimeAgo(post.UpdatedAt),
                    FormattedCreated = post.CreatedAt.ToString("yyyy/MM/dd HH:mm"),
                    FormattedUpdated = post.UpdatedAt.ToString("yyyy/MM/dd HH:mm")
                },
                Stats = new PostStatsViewModel
                {
                    LikeCount = post.LikeCount,
                    CommentCount = post.CommentCount,
                    ShareCount = 0 // TODO: 實作分享功能
                },
                RecentResponses = recentResponses,
                HasNewResponses = false, // TODO: 實作新回應檢查
                NewResponseCount = 0, // TODO: 實作新回應計數
                Permissions = new PostActionPermissions
                {
                    CanEdit = canEdit,
                    CanDelete = canDelete,
                    CanView = true,
                    EditRestrictionReason = editReason,
                    DeleteRestrictionReason = deleteReason
                }
            };
        }

        /// <summary>
        /// 計算時間差距的友善顯示
        /// </summary>
        private static string CalculateTimeAgo(DateTime dateTime)
        {
            var timeSpan = DateTime.Now - dateTime;

            if (timeSpan.TotalMinutes < 1)
                return "剛剛";
            
            if (timeSpan.TotalHours < 1)
                return $"{(int)timeSpan.TotalMinutes} 分鐘前";
            
            if (timeSpan.TotalDays < 1)
                return $"{(int)timeSpan.TotalHours} 小時前";
            
            if (timeSpan.TotalDays < 7)
                return $"{(int)timeSpan.TotalDays} 天前";
            
            return dateTime.ToString("yyyy/MM/dd");
        }

        /// <summary>
        /// 清除使用者快取資料（登入/登出時使用）
        /// </summary>
        /// <param name="userId">使用者ID</param>
        public async Task ClearUserCacheAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId)) return;

            try
            {
                // 清除使用者基本資訊快取
                var userInfoCacheKey = $"user_info_{userId}";
                await _cacheService.RemoveAsync(userInfoCacheKey);

                // 清除使用者按讚相關快取（使用模式匹配）
                await _cacheService.RemoveByPatternAsync($"user_likes_{userId}");
                await _cacheService.RemoveByPatternAsync($"user_stats_{userId}");

                _logger.LogInformation("已清除所有使用者快取，UserId: {UserId}", userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "清除使用者快取時發生錯誤，UserId: {UserId}", userId);
            }
        }

        #endregion

        #region 追蹤功能相關方法

        /// <summary>
        /// 取得最近的追蹤者列表
        /// </summary>
        /// <param name="userId">用戶ID</param>
        /// <param name="count">返回數量</param>
        /// <returns>追蹤者列表</returns>
        private async Task<List<FollowerViewModel>> GetRecentFollowersAsync(string userId, int count = 5)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                    return new List<FollowerViewModel>();

                // 檢查快取
                var cacheKey = $"recent_followers_{userId}_{count}";
                var cachedFollowers = await _cacheService.GetAsync<List<FollowerViewModel>>(cacheKey);

                if (cachedFollowers != null)
                {
                    return cachedFollowers;
                }

                // 取得最近的追蹤者，包含互相追蹤狀態檢查
                var (followers, _) = await _followService.GetFollowersAsync(userId, 1, count);

                // 批次檢查互相追蹤狀態
                var followerIds = followers.Select(f => f.Id).ToList();
                var mutualFollowStatus = await _followService.GetFollowStatusBatchAsync(userId, followerIds);

                // 取得追蹤關係建立時間
                var followRelations = await _context.Set<Follow>()
                    .Where(f => f.FolloweeId == userId && followerIds.Contains(f.FollowerId))
                    .Select(f => new { f.FollowerId, f.CreatedAt })
                    .ToListAsync();

                var result = followers.Select(follower =>
                {
                    var followRelation = followRelations.FirstOrDefault(fr => fr.FollowerId == follower.Id);
                    var followedAt = followRelation?.CreatedAt ?? DateTime.Now;

                    return new FollowerViewModel
                    {
                        Id = follower.Id,
                        UserName = follower.UserName,
                        DisplayName = follower.DisplayName ?? follower.UserName,
                        ProfilePictureUrl = follower.ProfilePictureUrl,
                        FollowedAt = followedAt,
                        FollowedTimeAgo = CalculateTimeAgo(followedAt),
                        IsMutualFollow = mutualFollowStatus.GetValueOrDefault(follower.Id, false)
                    };
                }).ToList();

                // 快取結果 5 分鐘
                await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(5));

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得最近追蹤者列表時發生錯誤，UserId: {UserId}", userId);
                return new List<FollowerViewModel>();
            }
        }

        #endregion

        #region 遊戲資料處理輔助方法

        /// <summary>
        /// 取得遊戲顯示名稱，優先使用中文標題
        /// </summary>
        /// <param name="gameDetail">遊戲詳細資料</param>
        /// <returns>顯示用的遊戲名稱</returns>
        private static string GetGameDisplayName(GameDetail? gameDetail)
        {
            if (gameDetail == null) return "未知遊戲";
            
            return !string.IsNullOrEmpty(gameDetail.zhtTitle) 
                ? gameDetail.zhtTitle 
                : gameDetail.engTitle ?? "未知遊戲";
        }

        /// <summary>
        /// 取得遊戲分類，基於遊戲資料推斷分類
        /// </summary>
        /// <param name="gameDetail">遊戲詳細資料</param>
        /// <returns>遊戲分類</returns>
        private static string GetGameCategory(GameDetail? gameDetail)
        {
            if (gameDetail == null) return "未分類";
            
            // 基於遊戲重量和玩家數量推斷分類
            return gameDetail.Weight switch
            {
                >= 3.5f => "重度策略",
                >= 2.5f => "中度策略", 
                >= 1.5f => "輕度策略",
                _ => "家庭遊戲"
            };
        }

        /// <summary>
        /// 產生遊戲元資訊字串
        /// </summary>
        /// <param name="gameDetail">遊戲詳細資料</param>
        /// <param name="discussionCount">討論數量</param>
        /// <param name="lastDiscussionAt">最後討論時間</param>
        /// <returns>格式化的元資訊</returns>
        private static string GenerateGameMetaInfo(GameDetail? gameDetail, int discussionCount, DateTime lastDiscussionAt)
        {
            var category = GetGameCategory(gameDetail);
            var timeAgo = CalculateTimeAgo(lastDiscussionAt);
            
            return $"{category} • {discussionCount}篇討論 • {timeAgo}";
        }

        /// <summary>
        /// 取得所有遊戲供下拉選單使用
        /// 參照遊戲圖鑑頁面的資料來源，提供完整的遊戲列表
        /// </summary>
        private async Task<List<GameSelectionDto>> GetAllGamesForSelectionAsync()
        {
            try
            {
                // 參照 DexController.Index() 的實作：_db.GameDetails.ToList()
                var allGames = await _context.GameDetails
                    .AsNoTracking()
                    .OrderBy(g => g.zhtTitle) // 按中文名稱排序
                    .ThenBy(g => g.engTitle) // 中文名稱相同時按英文名稱排序
                    .Select(g => new GameSelectionDto
                    {
                        Id = g.Id,
                        ZhtTitle = g.zhtTitle,
                        EngTitle = g.engTitle,
                        DisplayName = !string.IsNullOrEmpty(g.zhtTitle) ? g.zhtTitle : g.engTitle
                    })
                    .ToListAsync();

                return allGames;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得遊戲選擇列表時發生錯誤");
                // 發生錯誤時回傳空列表，不中斷頁面載入
                return new List<GameSelectionDto>();
            }
        }

        #endregion
    }
}