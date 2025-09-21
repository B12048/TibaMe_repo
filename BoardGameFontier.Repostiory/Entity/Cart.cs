using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BoardGameFontier.Repostiory.Entity
{
    /// <summary>
    /// 購物車資料模型
    /// 這個類別對應到資料庫中的 Carts 表格
    /// 每個使用者有一個購物車
    /// 負責人：洪苡芯 (根據 ApplicationDbContext 中的註解)
    /// </summary>
    [Table("Carts")]
    public class Cart
    {
        /// <summary>
        /// 購物車的唯一識別碼（主鍵）
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 所屬使用者ID（外鍵）
        /// 關聯到自訂的 UserProfile，每個使用者一個購物車，因此這裡應該是唯一且必須的
        /// </summary>
        [Required]
        [StringLength(450)] // 與 UserProfile.Id 的長度保持一致
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// 購物車建立時間
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// 購物車最後更新時間
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // ===== 導航屬性 =====

        /// <summary>
        /// 導航屬性：購物車所屬使用者
        /// </summary>
        [ForeignKey("UserId")]
        public virtual UserProfile User { get; set; } = null!;

        /// <summary>
        /// 導航屬性：購物車中的所有商品項目
        /// </summary>
        public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}