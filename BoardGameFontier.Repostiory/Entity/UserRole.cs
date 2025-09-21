using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BoardGameFontier.Repostiory.Entity
{
    /// <summary>
    /// 自訂使用者角色資料模型
    /// 這個類別對應到資料庫中的 CustomUserRoles 表格
    /// 用於定義使用者與角色之間的關係（多對多）
    /// 負責人：王品瑄 (根據 ApplicationDbContext 中的註解)
    /// </summary>
    [Table("CustomUserRoles")]
    public class UserRole
    {
        /// <summary>
        /// 使用者角色關聯的唯一識別碼（主鍵）
        /// </summary>
        [Key]
        public int Id { get; set; } // 新增一個獨立的主鍵，方便管理

        /// <summary>
        /// 使用者ID（外鍵）
        /// 關聯到自訂的 UserProfile
        /// </summary>
        [Required]
        [StringLength(450)] // 與 UserProfile.Id 的長度保持一致
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// 角色名稱 (例如: Admin, Member, Moderator)
        /// 這裡直接使用字串表示角色，如果需要更複雜的角色管理，可以考慮建立獨立的 Role 實體。
        /// </summary>
        [Required]
        [StringLength(50)]
        public string RoleName { get; set; } = string.Empty;

        /// <summary>
        /// 建立時間
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // ===== 導航屬性 =====

        /// <summary>
        /// 導航屬性：所屬使用者
        /// </summary>
        [ForeignKey("UserId")]
        public virtual UserProfile User { get; set; } = null!;

        // 如果您未來建立一個獨立的 Role 實體 (例如: RoleId, RoleName, Description)，
        // 則這裡可以添加一個 RoleId 和相應的導航屬性。
        /*
        [Required]
        public int RoleId { get; set; }
        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; } = null!;
        */
    }
}