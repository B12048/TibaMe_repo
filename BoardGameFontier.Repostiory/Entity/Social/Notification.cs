using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
// 移除對 Microsoft.AspNetCore.Identity 的引用
// using Microsoft.AspNetCore.Identity;

namespace BoardGameFontier.Repostiory.Entity.Social
{
    /// <summary>
    /// 通知資料模型
    /// 用於系統通知功能（按讚、留言、追蹤等）
    /// 負責人：廖昊威
    /// </summary>
    [Table("Notifications")]
    public class Notification
    {
        /// <summary>
        /// 通知的唯一識別碼（主鍵）
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 接收通知的使用者ID（外鍵）
        /// 關聯到自訂的 UserProfile
        /// </summary>
        [Required]
        [StringLength(450)] // 與 UserProfile.Id 的長度保持一致
        public string ReceiverId { get; set; } = string.Empty;

        /// <summary>
        /// 通知類型（Like、Comment、Follow、TradeUpdate等）
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// 通知內容
        /// </summary>
        [Required]
        [StringLength(1000)]
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// 是否已讀
        /// </summary>
        public bool IsRead { get; set; } = false; // 新增預設值

        /// <summary>
        /// 觸發通知的使用者ID（例如：誰按了讚、誰留了言）
        /// 可選，因為有些通知是系統自動產生的
        /// 關聯到自訂的 UserProfile
        /// </summary>
        [StringLength(450)] // 與 UserProfile.Id 的長度保持一致
        public string? TriggerId { get; set; }

        /// <summary>
        /// 相關項目類型（Post、Comment、TradeItem等）
        /// </summary>
        [StringLength(50)]
        public string? RelatedItemType { get; set; }

        /// <summary>
        /// 相關項目ID
        /// </summary>
        public int? RelatedItemId { get; set; }

        /// <summary>
        /// 建立時間
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // ===== 導航屬性 =====

        /// <summary>
        /// 導航屬性：接收通知的使用者
        /// </summary>
        [ForeignKey("ReceiverId")]
        public virtual UserProfile Receiver { get; set; } = null!;

        /// <summary>
        /// 導航屬性：觸發通知的使用者（可為空）
        /// </summary>
        [ForeignKey("TriggerId")]
        public virtual UserProfile? Trigger { get; set; }
    }
}