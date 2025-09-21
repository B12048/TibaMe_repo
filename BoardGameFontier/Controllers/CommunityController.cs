using Microsoft.AspNetCore.Mvc;
using BoardGameFontier.Services;
using BoardGameFontier.Models.ViewModels;
using BoardGameFontier.Models;
using BoardGameFontier.Repostiory.Entity.Social;

namespace BoardGameFontier.Controllers
{
    /// <summary>
    /// 社群討論控制器
    /// 處理社群頁面的使用者介面與基本資料傳遞
    /// 重構後使用強型別 ViewModel 和 Service 層協調
    /// 遵循 SOLID 原則：僅依賴 Service 層，不直接存取資料層
    /// </summary>
    public class CommunityController(
        ICommunityService communityService,
        ICurrentUserService currentUserService,
        IPostService postService,
        ILogger<CommunityController> logger) : Controller
    {
        private readonly ICommunityService _communityService = communityService;
        private readonly ICurrentUserService _currentUserService = currentUserService;
        private readonly IPostService _postService = postService;
        private readonly ILogger<CommunityController> _logger = logger;

        /// <summary>
        /// 社群首頁
        /// 展示貼文列表、熱門遊戲、使用者介面
        /// 重構後使用強型別 ViewModel
        /// </summary>
        /// <param name="postId">可選的貼文 ID，用於直接顯示特定貼文</param>
        /// <returns>社群主頁檢視</returns>
        public async Task<IActionResult> Index(int? postId = null)
        {
            try
            {
                // 使用 ICommunityService 取得完整的頁面資料
                var viewModel = await _communityService.GetCommunityPageDataAsync();
                
                // 設定頁面標題
                ViewData["Title"] = viewModel.PageSettings.Title;
                
                // 如果有 postId 參數，傳遞給 ViewData 讓前端自動載入該貼文
                if (postId.HasValue)
                {
                    ViewData["AutoLoadPostId"] = postId.Value;
                    _logger.LogInformation("社群主頁載入成功並將自動顯示貼文 {PostId}，使用者: {UserId}", postId.Value, _currentUserService.UserId ?? "訪客");
                }
                else
                {
                    _logger.LogInformation("社群主頁載入成功，使用者: {UserId}", _currentUserService.UserId ?? "訪客");
                }
                
                return View(viewModel);
            }
            catch (Exception ex)
            {
                return ErrorHandler.HandleControllerError(ex, _logger, "社群主頁載入", new { userId = _currentUserService.UserId });
            }
        }


        /// <summary>
        /// 貼文詳細頁面 - 右側面板顯示
        /// 用於無縫切換的貼文詳情檢視
        /// 重構後使用 ICommunityService 和強型別 ViewModel
        /// </summary>
        /// <param name="id">貼文 ID</param>
        /// <returns>Partial View 用於 AJAX 載入</returns>
        [HttpGet("Community/Post/{id}")]
        public async Task<IActionResult> PostDetail(int id)
        {
            try 
            {
                // 使用 ICommunityService 取得完整的貼文詳細資料
                var viewModel = await _communityService.GetPostDetailViewModelAsync(id, _currentUserService.UserId);
                
                if (viewModel == null)
                {
                    _logger.LogWarning("貼文不存在，PostId: {PostId}，使用者: {UserId}", id, _currentUserService.UserId ?? "訪客");
                    ViewBag.ErrorMessage = "找不到指定的貼文";
                    return PartialView("~/Views/Shared/Partials/_PostDetailError.cshtml");
                }

                _logger.LogInformation("貼文詳情載入成功，PostId: {PostId}，使用者: {UserId}", id, _currentUserService.UserId ?? "訪客");
                
                return PartialView("~/Views/Shared/Partials/_PostDetailPanel.cshtml", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "載入貼文詳情時發生錯誤，PostId: {PostId}，使用者: {UserId}", id, _currentUserService.UserId ?? "訪客");
                
                ViewBag.ErrorMessage = "載入貼文失敗，請稍後再試";
                return PartialView("~/Views/Shared/Partials/_PostDetailError.cshtml");
            }
        }

        // ✅ 已移除重複的 GetPostsApi 方法
        // 前端現在統一使用 /api/PostsAPI 標準 RESTful API
        // 參考：PostsAPIController.GetPosts() 方法
    }
}
