using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore; // 為了 [Index] 屬性

namespace BoardGameFontier.Repostiory.Entity
{
    /// <summary>
    /// 檢舉資料模型（對應資料表 Reports）
    /// </summary>
    [Table("Reports")]
    // 這個索引可以讓我們用 TargetUserId + Status 快速統計「已核准(Resolved)的次數」
    [Index(nameof(TargetUserId), nameof(Status))]
    public class Report
    {
        [Key]
        public int Id { get; set; }

        /// <summary>檢舉者（UserProfile.Id）</summary>
        [Required, StringLength(450)]
        public string ReporterId { get; set; } = string.Empty;

        /// <summary>被檢舉的類型（User、Post、Comment、TradeItem）</summary>
        [Required, StringLength(20)]
        public string ReportedType { get; set; } = string.Empty;

        /// <summary>
        /// 被檢舉的項目 ID（為相容 GUID/int，這裡用 string 存）
        /// </summary>
        [Required, StringLength(450)]
        public string ReportedId { get; set; } = string.Empty;

        /// <summary>檢舉原因</summary>
        [Required, StringLength(500)]
        public string Reason { get; set; } = string.Empty;

        /// <summary>
        /// 被檢舉者的 UserId（例如貼文作者/留言作者）
        /// 在核准檢舉時由 ModerationService 解析並寫入，
        /// 之後「警告次數」就直接用 Reports 表計算，不受刪文影響。
        /// </summary>
        [StringLength(450)]
        public string? TargetUserId { get; set; }

        /// <summary>檢舉描述（可空）</summary>
        [StringLength(2000)]
        public string? Description { get; set; }

        /// <summary>
        /// 處理狀態：建議統一用 Pending / Resolved / Rejected
        /// （你的 ModerationService 使用的是 Resolved/Rejected）
        /// </summary>
        [Required, StringLength(20)]
        public string Status { get; set; } = "Pending";

        /// <summary>處理備註</summary>
        [StringLength(1000)]
        public string? ResolutionNotes { get; set; }

        /// <summary>處理時間</summary>
        public DateTime? ResolvedAt { get; set; }

        /// <summary>建立時間</summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // ===== 導航屬性 =====

        [ForeignKey(nameof(ReporterId))]
        public virtual UserProfile Reporter { get; set; } = null!;
    }
}
