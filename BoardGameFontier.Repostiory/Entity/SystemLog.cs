using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BoardGameFontier.Repostiory.Entity
{
    /// <summary>
    /// 系統記錄資料模型
    /// 這個類別對應到資料庫中的 SystemLogs 表格
    /// 用於記錄應用程式的運行日誌、錯誤、重要事件等
    /// 負責人：王品瑄 (根據 ApplicationDbContext 中的註解)
    /// </summary>
    [Table("SystemLogs")]
    public class SystemLog
    {
        /// <summary>
        /// 記錄的唯一識別碼（主鍵）
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 記錄時間
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.Now;

        /// <summary>
        /// 記錄級別（例如：Information, Warning, Error, Critical）
        /// </summary>
        [Required]
        [StringLength(20)]
        public string Level { get; set; } = "Information"; // 新增預設值

        /// <summary>
        /// 記錄來源或類別（例如：ControllerName, ServiceName, DataAccess）
        /// </summary>
        [Required]
        [StringLength(100)]
        public string Category { get; set; } = string.Empty;

        /// <summary>
        /// 記錄訊息
        /// </summary>
        [Required]
        [StringLength(2000)]
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// 例外詳細資訊（如果有錯誤）
        /// </summary>
        [StringLength(5000)]
        public string? Exception { get; set; }

        /// <summary>
        /// 相關使用者ID（如果記錄與特定使用者相關）
        /// 關聯到自訂的 UserProfile，可為空
        /// </summary>
        [StringLength(450)] // 與 UserProfile.Id 的長度保持一致
        public string? UserId { get; set; }

        /// <summary>
        /// 使用者IP位址（如果適用）
        /// </summary>
        [StringLength(50)]
        public string? IpAddress { get; set; }

        /// <summary>
        /// 使用者代理（瀏覽器資訊，如果適用）
        /// </summary>
        [StringLength(500)]
        public string? UserAgent { get; set; }

        /// <summary>
        /// 請求的URL路徑（如果適用）
        /// </summary>
        [StringLength(500)]
        public string? RequestPath { get; set; }

        /// <summary>
        /// HTTP方法（GET、POST等，如果適用）
        /// </summary>
        [StringLength(10)]
        public string? HttpMethod { get; set; }

        // ===== 導航屬性 =====

        /// <summary>
        /// 導航屬性：相關使用者（可為空）
        /// </summary>
        [ForeignKey("UserId")]
        public virtual UserProfile? User { get; set; }
    }
}