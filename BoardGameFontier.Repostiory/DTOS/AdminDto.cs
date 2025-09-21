using System.ComponentModel.DataAnnotations;

namespace BoardGameFontier.Repostiory.DTOS
{
    //會員管理
    public class GetAllUsersDto
    {
        public string? SearchText { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
    }
    public class ContactAppDto
    {
        /// <summary>
        /// 使用者名稱（帳號）
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// Email
        /// </summary>
        public string Display { get; set; } = string.Empty;

        /// <summary>
        /// 帳戶是否可被鎖定
        /// </summary>
        public bool LockoutEnabled { get; set; }

        /// <summary>
        /// 使用者角色
        /// </summary>
        public string Roles { get; set; }

        public string Id { get; set; }
    }
    //鎖帳號
    public class UpdateLockoutDto
    {
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// 帳號是否可被鎖定（對應 IdentityUser.LockoutEnabled）
        /// </summary>
        public bool LockoutEnabled { get; set; }
    }

    //檢舉用
    #region DTO
    public class ReportRequest
    {
        [Required]
        [RegularExpression("^(Post|Comment|User)$")]
        public string ReportedType { get; set; } = string.Empty;

        [Required]
        [Range(1, int.MaxValue)]
        public int ReportedId { get; set; }

        [Required]
        [StringLength(100)]
        public string Reason { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Description { get; set; } // 對應前端備註
    }

    public class ReportDto
    {
        public int Id { get; set; }

        /// <summary>
        /// 檢舉人 ID
        /// </summary>
        public string ReporterId { get; set; } = string.Empty;

        /// <summary>
        /// 被檢舉的類型 (Post / Comment / User)
        /// </summary>
        public string ReportedType { get; set; } = string.Empty;

        /// <summary>
        /// 被檢舉目標 ID
        /// </summary>
        public int ReportedId { get; set; }

        /// <summary>
        /// 檢舉原因
        /// </summary>
        public string Reason { get; set; } = string.Empty;

        /// <summary>
        /// 檢舉補充描述（可選）
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// 檢舉狀態 (Pending / Resolved / Rejected)
        /// </summary>
        public string Status { get; set; } = "Pending";

        /// <summary>
        /// 審核備註
        /// </summary>
        public string? ResolutionNotes { get; set; }

        /// <summary>
        /// 審核完成時間（台灣時間）
        /// </summary>
        public DateTime? ResolvedAt { get; set; }

        /// <summary>
        /// 建立時間（台灣時間）
        /// </summary>
        public DateTime CreatedAt { get; set; }

        public string? ReportedTitle { get; set; }
        public string? ReportedContent { get; set; }
        public bool ReportedContentIsHtml { get; set; }  
        public string? ReportedUrl { get; set; }
    }

    #endregion

    //圖表的
    public class SummaryCardDto
    {
        public string totalNews { get; set; }
        public string monthlyNews { get; set; }
        public string totalWatch { get; set; }

    }

    public class ChartDto
    {
        public List<string> Labels { get; set; }
        public List<int> Data { get; set; }
    }
    //升身分
    public class UpdateRoleDto
    {
        public string Id { get; set; }
        public string Role { get; set; }
    }
}



