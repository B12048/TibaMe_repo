using BoardGameFontier.Models.Common;
using BoardGameFontier.Repostiory.DTOS.Social;
using BoardGameFontier.Extensions;
using BoardGameFontier.Services;
using Microsoft.AspNetCore.Mvc;

namespace BoardGameFontier.Controllers.API
{
    /// <summary>
    /// 追蹤關係 API Controller - RESTful 資源導向設計
    /// 提供用戶追蹤相關的標準 REST API
    /// 路由格式：/api/follows
    /// </summary>
    [Route("api/follows")]
    [ApiController]
    public class FollowAPIController : ControllerBase
    {
        private readonly IFollowService _followService;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<FollowAPIController> _logger;

        public FollowAPIController(
            IFollowService followService,
            ICurrentUserService currentUserService,
            ILogger<FollowAPIController> logger)
        {
            _followService = followService;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        /// <summary>
        /// 建立追蹤關係 - RESTful 資源建立
        /// POST /api/follows
        /// Body: { "followeeId": "targetUserId" }
        /// </summary>
        /// <param name="request">追蹤請求</param>
        /// <returns>操作結果</returns>
        [HttpPost]
        public async Task<IActionResult> CreateFollow([FromBody] CreateFollowRequest request)
        {
            var currentUserId = _currentUserService.UserId;
            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized(ApiResponse<object>.Unauthorized("請先登入"));
            }

            try
            {
                if (request == null || string.IsNullOrEmpty(request.FolloweeId))
                {
                    return BadRequest(ApiResponse<object>.Error("用戶ID不能為空", 400));
                }

                if (currentUserId == request.FolloweeId)
                {
                    return BadRequest(ApiResponse<object>.Error("不能追蹤自己", 400));
                }

                var result = await _followService.FollowUserAsync(currentUserId, request.FolloweeId);
                
                if (result)
                {
                    return Ok(ApiResponse<object>.Ok(null, "追蹤成功"));
                }
                else
                {
                    return BadRequest(ApiResponse<object>.Error("追蹤失敗，請稍後再試", 400));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "追蹤用戶時發生錯誤，當前用戶：{CurrentUserId}，被追蹤者：{FolloweeId}", 
                    currentUserId, request?.FolloweeId);
                return StatusCode(500, ApiResponse<object>.InternalServerError("追蹤時發生錯誤"));
            }
        }

        /// <summary>
        /// 刪除追蹤關係 - RESTful 資源刪除
        /// DELETE /api/follows/{followeeId}
        /// </summary>
        /// <param name="followeeId">被追蹤者ID</param>
        /// <returns>操作結果</returns>
        [HttpDelete("{followeeId}")]
        public async Task<IActionResult> DeleteFollow(string followeeId)
        {
            var currentUserId = _currentUserService.UserId;
            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized(ApiResponse<object>.Unauthorized("請先登入"));
            }

            try
            {
                if (string.IsNullOrEmpty(followeeId))
                {
                    return BadRequest(ApiResponse<object>.Error("用戶ID不能為空", 400));
                }

                var result = await _followService.UnfollowUserAsync(currentUserId, followeeId);
                
                if (result)
                {
                    return Ok(ApiResponse<object>.Ok(null, "取消追蹤成功"));
                }
                else
                {
                    return BadRequest(ApiResponse<object>.Error("取消追蹤失敗，請稍後再試", 400));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取消追蹤時發生錯誤，當前用戶：{CurrentUserId}，被追蹤者：{FolloweeId}", 
                    currentUserId, followeeId);
                return StatusCode(500, ApiResponse<object>.InternalServerError("取消追蹤時發生錯誤"));
            }
        }

        /// <summary>
        /// 檢查是否已追蹤指定用戶
        /// </summary>
        /// <param name="followeeId">被追蹤者ID</param>
        /// <returns>追蹤狀態</returns>
        [HttpGet("status/{followeeId}")]
        public async Task<IActionResult> GetFollowStatus(string followeeId)
        {
            var currentUserId = _currentUserService.UserId;
            if (string.IsNullOrEmpty(currentUserId))
            {
                return Ok(ApiResponse<object>.Ok(new { isFollowing = false }, "未登入狀態"));
            }

            try
            {
                if (string.IsNullOrEmpty(followeeId))
                {
                    return BadRequest(ApiResponse<object>.Error("用戶ID不能為空", 400));
                }

                var isFollowing = await _followService.IsFollowingAsync(currentUserId, followeeId);
                
                return Ok(ApiResponse<object>.Ok(new { isFollowing }, "查詢成功"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "查詢追蹤狀態時發生錯誤，當前用戶：{CurrentUserId}，查詢對象：{FolloweeId}", 
                    currentUserId, followeeId);
                return StatusCode(500, ApiResponse<object>.InternalServerError("查詢追蹤狀態時發生錯誤"));
            }
        }

        /// <summary>
        /// 批次檢查追蹤狀態
        /// </summary>
        /// <param name="request">用戶ID列表</param>
        /// <returns>追蹤狀態字典</returns>
        [HttpPost("status/batch")]
        public async Task<IActionResult> GetFollowStatusBatch([FromBody] FollowStatusBatchRequestDto request)
        {
            var currentUserId = _currentUserService.UserId;
            if (string.IsNullOrEmpty(currentUserId))
            {
                var emptyResult = request.UserIds.ToDictionary(id => id, _ => false);
                return Ok(ApiResponse<object>.Ok(emptyResult, "未登入狀態"));
            }

            try
            {
                if (request == null || request.UserIds == null || !request.UserIds.Any())
                {
                    return BadRequest(ApiResponse<object>.Error("用戶ID列表不能為空", 400));
                }

                var followStatus = await _followService.GetFollowStatusBatchAsync(currentUserId, request.UserIds);
                
                return Ok(ApiResponse<object>.Ok(followStatus, "批次查詢成功"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "批次查詢追蹤狀態時發生錯誤，當前用戶：{CurrentUserId}", currentUserId);
                return StatusCode(500, ApiResponse<object>.InternalServerError("批次查詢追蹤狀態時發生錯誤"));
            }
        }

        /// <summary>
        /// 取得用戶的追蹤者列表
        /// </summary>
        /// <param name="userId">用戶ID</param>
        /// <param name="page">頁碼</param>
        /// <param name="pageSize">每頁筆數</param>
        /// <returns>追蹤者列表</returns>
        [HttpGet("followers/{userId}")]
        public async Task<IActionResult> GetFollowers(string userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest(ApiResponse<object>.Error("用戶ID不能為空", 400));
                }

                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 20;

                var (followers, totalCount) = await _followService.GetFollowersAsync(userId, page, pageSize);

                var followersData = followers.Select(user => user.ToUserBasicInfoDto()).ToList();

                var responseData = new FollowListResponseDto
                {
                    Users = followersData,
                    Pagination = Extensions.DtoMappingExtensions.ToPaginationDto(page, pageSize, totalCount)
                };

                return Ok(ApiResponse<object>.Ok(responseData, "追蹤者列表載入成功"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得追蹤者列表時發生錯誤，用戶ID：{UserId}", userId);
                return StatusCode(500, ApiResponse<object>.InternalServerError("載入追蹤者列表時發生錯誤"));
            }
        }

        /// <summary>
        /// 取得用戶的追蹤列表
        /// </summary>
        /// <param name="userId">用戶ID</param>
        /// <param name="page">頁碼</param>
        /// <param name="pageSize">每頁筆數</param>
        /// <returns>追蹤列表</returns>
        [HttpGet("following/{userId}")]
        public async Task<IActionResult> GetFollowing(string userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest(ApiResponse<object>.Error("用戶ID不能為空", 400));
                }

                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 20;

                var (following, totalCount) = await _followService.GetFollowingAsync(userId, page, pageSize);

                var followingData = following.Select(user => user.ToUserBasicInfoDto()).ToList();

                var responseData = new FollowListResponseDto
                {
                    Users = followingData,
                    Pagination = Extensions.DtoMappingExtensions.ToPaginationDto(page, pageSize, totalCount)
                };

                return Ok(ApiResponse<object>.Ok(responseData, "追蹤列表載入成功"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得追蹤列表時發生錯誤，用戶ID：{UserId}", userId);
                return StatusCode(500, ApiResponse<object>.InternalServerError("載入追蹤列表時發生錯誤"));
            }
        }

        /// <summary>
        /// 取得用戶的追蹤統計
        /// </summary>
        /// <param name="userId">用戶ID</param>
        /// <returns>追蹤統計</returns>
        [HttpGet("stats/{userId}")]
        public async Task<IActionResult> GetFollowStats(string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest(ApiResponse<object>.Error("用戶ID不能為空", 400));
                }

                var (followersCount, followingCount) = await _followService.GetFollowCountsAsync(userId);

                var statsData = new FollowStatsDto
                {
                    FollowersCount = followersCount,
                    FollowingCount = followingCount,
                    UserId = userId,
                    Timestamp = DateTime.Now
                };

                _logger.LogInformation("返回追蹤統計，用戶ID：{UserId}，粉絲數：{FollowersCount}，追蹤數：{FollowingCount}", 
                    userId, followersCount, followingCount);

                return Ok(ApiResponse<object>.Ok(statsData, "追蹤統計載入成功"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得追蹤統計時發生錯誤，用戶ID：{UserId}", userId);
                return StatusCode(500, ApiResponse<object>.InternalServerError("載入追蹤統計時發生錯誤"));
            }
        }
    }

    // 注意：追蹤相關的 DTOs 已移動到 BoardGameFontier.Repository.DTOS.Social
    // FollowStatusBatchRequestDto、FollowResponseDto 等類別現在統一管理
}