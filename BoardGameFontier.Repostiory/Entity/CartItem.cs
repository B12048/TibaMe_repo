using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BoardGameFontier.Repostiory.Entity
{
    /// <summary>
    /// 購物車項目資料模型
    /// 這個類別對應到資料庫中的 CartItems 表格
    /// 代表購物車中的單個商品及其數量
    /// 負責人：洪苡芯 (根據 ApplicationDbContext 中的註解)
    /// </summary>
    [Table("CartItems")]
    public class CartItem
    {
        /// <summary>
        /// 購物車項目的唯一識別碼（主鍵）
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 所屬購物車ID（外鍵）
        /// </summary>
        [Required]
        public int CartId { get; set; }

        /// <summary>
        /// 交易商品ID（外鍵）
        /// </summary>
        [Required]
        public int TradeItemId { get; set; }

        /// <summary>
        /// 商品數量
        /// </summary>
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "商品數量必須大於0")]
        public int Quantity { get; set; }

        /// <summary>
        /// 商品加入購物車的時間
        /// </summary>
        public DateTime AddedAt { get; set; } = DateTime.Now;

        // ===== 導航屬性 =====

        /// <summary>
        /// 導航屬性：所屬購物車
        /// </summary>
        [ForeignKey("CartId")]
        public virtual Cart Cart { get; set; } = null!;

        /// <summary>
        /// 導航屬性：購物車中的交易商品
        /// </summary>
        [ForeignKey("TradeItemId")]
        public virtual TradeItem TradeItem { get; set; } = null!;
    }
}