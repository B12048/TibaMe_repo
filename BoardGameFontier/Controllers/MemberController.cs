using BoardGameFontier.Extensions;
using BoardGameFontier.Models.ViewModels;
using BoardGameFontier.Repostiory;
using BoardGameFontier.Repostiory.Entity;
using BoardGameFontier.Repostiory.Entity.Social;
using BoardGameFontier.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;


namespace BoardGameFontier.Controllers
{
    public class MemberController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;
        private readonly IMemberProfileService _memberProfile;
        private readonly IPasswordService _pwdService;
        private readonly ICommunityService _communityService;

        public MemberController(ApplicationDbContext context, ICurrentUserService currentUser, IMemberProfileService memberProfile, IPasswordService pwdService, ICommunityService communityService)
        {
            _context = context;
            _currentUser = currentUser;
            _memberProfile = memberProfile;
            _pwdService = pwdService;
            _communityService = communityService;
        }
        [HttpGet]
        public async Task<IActionResult> IndexMember()
        {
            //這邊為了前面的JS加的，不加這邊會導致出現主頁因Model.BoardGameTags Null而炸掉
            var userProfile = await _currentUser.GetUserProfileAsync();
            var memberVm = _memberProfile.GetMemberProfile(userProfile);
            
            // 取得追蹤者資料供右側邊欄使用
            var sidebarData = await _communityService.GetSidebarDataAsync();
            ViewBag.FollowersData = sidebarData;
            
            return View(memberVm);
           
        }
        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var user = await _currentUser.GetUserProfileAsync();
            var editVm = _memberProfile.GetEditProfile(user);
            return PartialView("_EditProfile",editVm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(EditProfileViewModel model)
        {
           
            var user = await _context.UserProfiles
                .WhereActive() //要撈的是IsDeleted=false的
                .FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);
            if (user == null) //理論上這不太可能發生，但還是寫
            {
                return RedirectToAction("IndexLogin", "Login");
            }
            //更新使用者資料
            var result = _memberProfile.UpdateProfile(user, model, _pwdService);

            //如果更新失敗，回傳錯誤訊息
            if (!result.Success)
            {
                ModelState.AddModelError("OldPassword", result.ErrorMessage);
                return PartialView("_EditProfile", model);
            }

            await _context.SaveChangesAsync(); //儲存變更到資料庫

            var newvm = _memberProfile.GetMemberProfile(user);
            return PartialView("_Profile", newvm); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePrivacy(string field, bool value)
        {
            var user = await _context.UserProfiles
                .WhereActive() //要撈的是IsDeleted=false的
                .FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);
            if (user == null) //理論上這應該不可能發生
                return Json(new { success = false, message = "使用者不存在" });

            var result = _memberProfile.UpdatePrivacy(user, field, value);
            if (!result.Success)
                return Json(new { success = false, message = result.ErrorMessage });

            await _context.SaveChangesAsync();
            return Json(new { success = true, field, value });
        }

        [HttpGet]
        public async Task<IActionResult>PlayerProfile(string? id,string? userName) //點其他使用者的Action
        {
            var vm = await _memberProfile.GetPlayerProfileAsync(id, userName);
            if (vm == null) return NotFound();
            var isDeleted = await _context.UserProfiles //這邊判斷是不是已刪除的帳號
               .AsNoTracking()
               .AnyAsync(u => u.Id == vm.UserId && u.IsDeleted);
            if (isDeleted) return View("PrivacyBlocked"); 

            //這邊做判斷是否是本人
            var currentUserId = _currentUser.UserId;
            var isOwner = !string.IsNullOrEmpty(_currentUser.UserId) &&
                string.Equals(vm.UserId, currentUserId, StringComparison.Ordinal);
                                            //StringComparison.Ordinal做最安全穩定的比對!

            //判斷若那個人不是自己，就攔截!
            if (vm.IsProfileHide && !isOwner)
            {
                return View("PrivacyBlocked"); 

            }
            return View("PlayerProfile",vm);
        }

        public async Task<IActionResult> LoadSection(string section)
        {
            var userProfile = await _currentUser.GetUserProfileAsync();

            if (section == "History")
            {
                // 建立一筆測試用的 UserProfile 假資料

                return PartialView("_History");
            }
            switch (section)
            {
                case "Profile":
                    var memberVm = _memberProfile.GetMemberProfile(userProfile);
                    return PartialView("_Profile", memberVm);

                case "EditProfile":
                    var EditVm = _memberProfile.GetEditProfile(userProfile);
                    return PartialView("_EditProfile", EditVm);

                case "History": return PartialView("_History");
                case "Events": return PartialView("_Events");
                case "Market": return PartialView("_Market");
                case "Settings":
                    var Setvm = _memberProfile.GetPrivacySettings(userProfile);
                    return PartialView("_Settings", Setvm);
                case "MyPost":
                    return await LoadMyPostData();
                    
                default: return Content("找不到內容");
            }

        }

        /// <summary>
        /// 載入個人貼文資料 - 統一的資料載入方法
        /// </summary>
        private async Task<IActionResult> LoadMyPostData()
        {
            try
            {
                var userId = _currentUser.UserId;
                if (string.IsNullOrEmpty(userId))
                {
                    return RedirectToAction("IndexLogin", "Login");
                }

                // 建立預設篩選選項
                var filterOptions = new PostFilterOptions
                {
                    FilterType = null,
                    SortOrder = PostSortOrder.CreatedDesc,
                    SearchKeyword = null,
                    PageSize = 5
                };

                // 使用 ICommunityService 取得個人貼文資料
                var viewModel = await _communityService.GetMyPostsPageDataAsync(userId, filterOptions, 1);

                // 準備 JSON 格式的初始資料供前端 Vue.js 使用
                var initialData = new
                {
                    myPosts = viewModel.MyPosts.Select(p => new
                    {
                        id = p.Id,
                        title = p.Title,
                        contentPreview = p.ContentPreview,
                        type = (int)p.Type,
                        price = p.Price,
                        tradeLocation = p.TradeLocation,
                        tradeNotes = p.TradeNotes,
                        relatedGame = p.RelatedGame != null ? new { 
                            id = p.RelatedGame.Id, 
                            name = p.RelatedGame.Name 
                        } : null,
                        createdAt = p.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ss"),
                        updatedAt = p.UpdatedAt.ToString("yyyy-MM-ddTHH:mm:ss"),
                        stats = new
                        {
                            likeCount = p.Stats.LikeCount,
                            commentCount = p.Stats.CommentCount,
                            shareCount = p.Stats.ShareCount
                        },
                        recentResponses = p.RecentResponses.Select(r => new
                        {
                            id = r.Id,
                            contentPreview = r.ContentPreview,
                            author = new
                            {
                                id = r.Author.Id,
                                userName = r.Author.UserName,
                                displayName = r.Author.DisplayName,
                                profilePictureUrl = r.Author.ProfilePictureUrl
                            },
                            createdAt = r.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ss"),
                            timeAgo = r.TimeAgo,
                            isNew = r.IsNew
                        }),
                        hasNewResponses = p.HasNewResponses,
                        newResponseCount = p.NewResponseCount,
                        permissions = new
                        {
                            canEdit = p.Permissions.CanEdit,
                            canDelete = p.Permissions.CanDelete,
                            canView = p.Permissions.CanView,
                            editRestrictionReason = p.Permissions.EditRestrictionReason,
                            deleteRestrictionReason = p.Permissions.DeleteRestrictionReason
                        }
                    }),
                    stats = new
                    {
                        totalPosts = viewModel.Stats.TotalPosts,
                        totalLikes = viewModel.Stats.TotalLikes,
                        totalComments = viewModel.Stats.TotalComments,
                        postsWithNewResponses = viewModel.Stats.PostsWithNewResponses
                    },
                    pagination = new
                    {
                        currentPage = viewModel.Pagination.CurrentPage,
                        pageSize = viewModel.Pagination.PageSize,
                        totalItems = viewModel.Pagination.TotalItems,
                        totalPages = viewModel.Pagination.TotalPages,
                        hasPreviousPage = viewModel.Pagination.HasPreviousPage,
                        hasNextPage = viewModel.Pagination.HasNextPage
                    },
                    filterOptions = new
                    {
                        filterType = filterOptions.FilterType?.ToString() ?? "",
                        sortOrder = filterOptions.SortOrder.ToString(),
                        searchKeyword = filterOptions.SearchKeyword ?? "",
                        onlyWithNewResponses = filterOptions.OnlyWithNewResponses
                    }
                };

                // 將初始資料序列化為 JSON 傳遞給 View
                var jsonData = JsonConvert.SerializeObject(initialData, new JsonSerializerSettings
                {
                    DateFormatString = "yyyy-MM-ddTHH:mm:ss",
                    NullValueHandling = NullValueHandling.Ignore
                });
                ViewBag.InitialData = jsonData;

                return PartialView("_MyPost", viewModel);
            }
            catch (Exception ex)
            {
                // 即使發生錯誤，也提供空的初始資料避免前端錯誤
                ViewBag.InitialData = JsonConvert.SerializeObject(new
                {
                    myPosts = new object[0],
                    stats = new { totalPosts = 0, totalLikes = 0, totalComments = 0, postsWithNewResponses = 0 },
                    pagination = new { currentPage = 1, pageSize = 5, totalItems = 0, totalPages = 0, hasPreviousPage = false, hasNextPage = false },
                    filterOptions = new { filterType = "", sortOrder = "CreatedDesc", searchKeyword = "", onlyWithNewResponses = false }
                });
                
                ViewBag.ErrorMessage = $"載入個人貼文失敗，請稍後再試 (錯誤: {ex.GetType().Name})";
                return PartialView("_MyPost");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAccount() //刪帳號
        {
            var userName = User.Identity.Name;

            var user = await _context.UserProfiles.FirstOrDefaultAsync(u => u.UserName == userName);
            if (user == null)
            {
                return RedirectToAction("IndexMember");//理論上這行應該不用?只是還是寫出來好
            }

            var (ok, err) = await _memberProfile.SoftDeleteAccountAsync(userName);
            if (!ok)
            {
                TempData["Error"] = err ?? "刪除失敗";
                return RedirectToAction("IndexMember");
            }


            //清除Cookie登出! 這邊因為Login就是用Cookie....Scheme這概念，所以登出也要
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);


            return RedirectToAction("Index", "Home");
        }


    }
}

