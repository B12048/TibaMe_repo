using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BoardGameFontier.Repostiory.Entity.Social
{
    /// <summary>
    /// 貼文與標籤的關聯資料模型
    /// 這是用於實現 Post 和 Tag 之間多對多關係的聯結表 (Junction Table)
    /// 負責人：廖昊威 (補充)
    /// </summary>
    [Table("PostTags")]
    public class PostTag
    {
        /// <summary>
        /// 所屬貼文ID（複合主鍵的一部分，外鍵）
        /// </summary>
        [Required]
        public int PostId { get; set; }

        /// <summary>
        /// 所屬標籤ID（複合主鍵的一部分，外鍵）
        /// </summary>
        [Required]
        public int TagId { get; set; }

        // ===== 導航屬性 =====

        /// <summary>
        /// 導航屬性：所屬貼文
        /// </summary>
        [ForeignKey("PostId")]
        public virtual Post Post { get; set; } = null!;

        /// <summary>
        /// 導航屬性：所屬標籤
        /// </summary>
        [ForeignKey("TagId")]
        public virtual Tag Tag { get; set; } = null!;
    }
}