using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BoardGameFontier.Repostiory.Entity
{
    /// <summary>
    /// 網站公告資料模型
    /// 這個類別對應到資料庫中的 Announcements 表格
    /// 負責人：王品瑄 (根據 ApplicationDbContext 中的註解)
    /// </summary>
    [Table("Announcements")]
    public class Announcement
    {
        /// <summary>
        /// 公告的唯一識別碼（主鍵）
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 公告標題
        /// </summary>
        [Required(ErrorMessage = "公告標題是必填的")]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// 公告內容
        /// </summary>
        [Required(ErrorMessage = "公告內容是必填的")]
        [StringLength(4000)]
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// 發布者ID (可以是管理員)
        /// 關聯到自訂的 UserProfile
        /// </summary>
        [Required]
        [StringLength(450)] // 與 UserProfile.Id 的長度保持一致
        public string AuthorId { get; set; } = string.Empty; // 新增，記錄誰發布的公告

        /// <summary>
        /// 發布時間
        /// </summary>
        public DateTime PublishedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// 公告是否啟用 (顯示或隱藏)
        /// </summary>
        public bool IsActive { get; set; } = true; // 新增

        /// <summary>
        /// 公告過期時間（可選）
        /// </summary>
        public DateTime? ExpiresAt { get; set; } // 新增

        // ===== 導航屬性 =====

        /// <summary>
        /// 導航屬性：發布公告的使用者 (通常是管理員)
        /// </summary>
        [ForeignKey("AuthorId")]
        public virtual UserProfile Author { get; set; } = null!;
    }
}