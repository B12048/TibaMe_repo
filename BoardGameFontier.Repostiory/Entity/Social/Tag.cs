using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BoardGameFontier.Repostiory.Entity.Social
{
    /// <summary>
    /// 標籤資料模型
    /// 負責人：廖昊威
    /// </summary>
    [Table("Tags")]
    public class Tag
    {
        /// <summary>
        /// 標籤的唯一識別碼（主鍵）
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 標籤名稱
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 標籤顏色（用於前端顯示）
        /// </summary>
        [StringLength(20)]
        public string Color { get; set; } = "#007bff";

        /// <summary>
        /// 使用次數（有多少貼文使用此標籤）
        /// </summary>
        public int UsageCount { get; set; }

        /// <summary>
        /// 建立時間
        /// </summary>
        public DateTime CreatedAt { get; set; }

        // ===== 導航屬性 =====

        /// <summary>
        /// 導航屬性：使用此標籤的所有貼文
        /// 多對多關聯
        /// </summary>
        public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
    }
}