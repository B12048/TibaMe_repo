using System.ComponentModel.DataAnnotations;

namespace BoardGameFontier.Repostiory.DTOS.Social
{
    /// <summary>
    /// 追蹤回應資料傳輸物件
    /// 用於追蹤相關的 API 回應
    /// 負責人：廖昊威
    /// </summary>
    public class FollowResponseDto
    {
        /// <summary>
        /// 是否正在追蹤
        /// </summary>
        public bool IsFollowing { get; set; }

        /// <summary>
        /// 被追蹤者 ID
        /// </summary>
        public string FolloweeId { get; set; } = string.Empty;

        /// <summary>
        /// 追蹤時間（如果有追蹤的話）
        /// </summary>
        public DateTime? FollowedAt { get; set; }
    }

    /// <summary>
    /// 使用者基本資訊 DTO（用於追蹤列表）
    /// </summary>
    public class UserBasicInfoDto
    {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string? ProfilePictureUrl { get; set; }
        public string? Bio { get; set; }
        public bool IsFollowed { get; set; }
        public DateTime? FollowedAt { get; set; }
    }

    /// <summary>
    /// 追蹤統計資料 DTO
    /// </summary>
    public class FollowStatsDto
    {
        /// <summary>
        /// 追蹤者數量（粉絲數）
        /// </summary>
        public int FollowersCount { get; set; }

        /// <summary>
        /// 追蹤中數量
        /// </summary>
        public int FollowingCount { get; set; }

        /// <summary>
        /// 使用者 ID
        /// </summary>
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// 資料更新時間
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }

    /// <summary>
    /// 分頁追蹤列表回應 DTO
    /// </summary>
    public class FollowListResponseDto
    {
        /// <summary>
        /// 使用者列表
        /// </summary>
        public List<UserBasicInfoDto> Users { get; set; } = new List<UserBasicInfoDto>();

        /// <summary>
        /// 分頁資訊
        /// </summary>
        public PaginationDto Pagination { get; set; } = null!;
    }

    /// <summary>
    /// 建立追蹤關係請求 DTO - RESTful API 用
    /// </summary>
    public class CreateFollowRequest
    {
        /// <summary>
        /// 要追蹤的使用者 ID
        /// </summary>
        [Required(ErrorMessage = "被追蹤者 ID 是必填的")]
        public string FolloweeId { get; set; } = string.Empty;
    }

    /// <summary>
    /// 批量追蹤狀態查詢請求 DTO
    /// </summary>
    public class FollowStatusBatchRequestDto
    {
        /// <summary>
        /// 要查詢的使用者 ID 列表
        /// </summary>
        public List<string> UserIds { get; set; } = new List<string>();
    }

    /// <summary>
    /// 分頁資訊 DTO
    /// </summary>
    public class PaginationDto
    {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public bool HasNext { get; set; }
        public bool HasPrevious { get; set; }
    }
}