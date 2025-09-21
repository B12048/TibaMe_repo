using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoardGameFontier.Repostiory.Entity
{
    /// <summary>
    /// 收藏記錄資料模型
    /// 這個類別對應到資料庫中的 Favorites 表格
    /// 用於記錄使用者收藏的貼文、商品等
    /// 負責人：陳建宇 (根據 ApplicationDbContext 中的註解)
    /// </summary>
    [Table("Favorites")]
    public class Favorite
    {
        /// <summary>
        /// 收藏記錄的唯一識別碼（主鍵）
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 收藏者ID（外鍵）
        /// 關聯到自訂的 UserProfile
        /// </summary>
        [Required]
        [StringLength(450)] // 與 UserProfile.Id 的長度保持一致
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// 被收藏的項目類型（Post、TradeItem、GameDetail 等）
        /// </summary>
        [Required]
        [StringLength(50)]
        public string ItemType { get; set; } = string.Empty;

        /// <summary>
        /// 被收藏的項目ID
        /// </summary>
        [Required]
        public int ItemId { get; set; }

        /// <summary>
        /// 收藏時間
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // ===== 導航屬性 =====

        /// <summary>
        /// 導航屬性：收藏者
        /// </summary>
        [ForeignKey("UserId")]
        public virtual UserProfile User { get; set; } = null!;

        // 由於 ItemType 和 ItemId 可以指向不同實體，
        // 這裡不直接建立導航屬性，而是在應用層邏輯中處理多態性關聯。
    }
}
