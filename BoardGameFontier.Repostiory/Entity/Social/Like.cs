using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
// 移除對 Microsoft.AspNetCore.Identity 的引用
// using Microsoft.AspNetCore.Identity;

namespace BoardGameFontier.Repostiory.Entity.Social
{
    /// <summary>
    /// 按讚記錄資料模型
    /// 使用複合主鍵防止重複按讚
    /// 負責人：廖昊威
    /// </summary>
    [Table("Likes")]
    public class Like
    {
        /// <summary>
        /// 按讚者ID（複合主鍵的一部分，外鍵）
        /// 現在關聯到自訂的 UserProfile
        /// </summary>
        [Required]
        [StringLength(450)] // 與 UserProfile.Id 的長度保持一致
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// 被按讚的項目類型（複合主鍵的一部分）
        /// Post 或 Comment
        /// </summary>
        [Required]
        [StringLength(20)]
        public string ItemType { get; set; } = string.Empty;

        /// <summary>
        /// 被按讚的項目ID（複合主鍵的一部分）
        /// </summary>
        [Required]
        public int ItemId { get; set; }

        /// <summary>
        /// 按讚時間
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // ===== 導航屬性 =====

        /// <summary>
        /// 導航屬性：按讚者
        /// 關聯到自訂的 UserProfile 實體
        /// </summary>
        [ForeignKey("UserId")]
        public virtual UserProfile User { get; set; } = null!;

        // 由於 ItemType 和 ItemId 可以指向 Post 或 Comment
        // 這裡不直接建立導航屬性，而是在應用層邏輯中處理多態性關聯。
        // 如果未來需要強類型導航，可以考慮 Table-per-Hierarchy 或 Table-per-Type 策略。
    }
}