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
    /// 交易記錄資料模型
    /// 這個類別對應到資料庫中的 Transactions 表格
    /// 記錄商品買賣的詳細資訊
    /// 負責人：陳建宇 (根據 ApplicationDbContext 中的註解)
    /// </summary>
    [Table("Transactions")]
    public class Transaction
    {
        /// <summary>
        /// 交易記錄的唯一識別碼（主鍵）
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 購買者ID（外鍵）
        /// 關聯到自訂的 UserProfile
        /// </summary>
        [Required]
        [StringLength(450)] // 與 UserProfile.Id 的長度保持一致
        public string BuyerId { get; set; } = string.Empty;

        /// <summary>
        /// 賣家ID（外鍵）
        /// 關聯到自訂的 UserProfile
        /// </summary>
        [Required]
        [StringLength(450)] // 與 UserProfile.Id 的長度保持一致
        public string SellerId { get; set; } = string.Empty;

        /// <summary>
        /// 交易商品ID（外鍵）
        /// </summary>
        [Required]
        public int TradeItemId { get; set; }

        /// <summary>
        /// 購買數量
        /// </summary>
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "購買數量必須大於0")]
        public int Quantity { get; set; }

        /// <summary>
        /// 交易金額
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        [Range(0.0, 999999.99)]
        public decimal Amount { get; set; }

        /// <summary>
        /// 交易狀態 (例如：Pending, Completed, Cancelled)
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Pending"; // 新增預設值

        /// <summary>
        /// 交易建立時間
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// 交易完成時間（如果適用）
        /// </summary>
        public DateTime? CompletedAt { get; set; }

        /// <summary>
        /// 買家地址（用於寄送）
        /// </summary>
        [StringLength(500)]
        public string? ShippingAddress { get; set; } // 新增

        /// <summary>
        /// 買家聯絡電話
        /// </summary>
        [StringLength(50)]
        public string? ContactPhone { get; set; } // 新增

        // ===== 導航屬性 =====

        /// <summary>
        /// 導航屬性：購買者
        /// </summary>
        [ForeignKey("BuyerId")]
        [InverseProperty("BoughtTransactions")]
        public virtual UserProfile Buyer { get; set; } = null!;

        /// <summary>
        /// 導航屬性：賣家
        /// </summary>
        [ForeignKey("SellerId")]
        [InverseProperty("SoldTransactions")]
        public virtual UserProfile Seller { get; set; } = null!;

        /// <summary>
        /// 導航屬性：交易商品
        /// </summary>
        [ForeignKey("TradeItemId")]
        public virtual TradeItem TradeItem { get; set; } = null!;

        //-----------下面暫時放假資料欄位，不會存到資料庫--------
        [NotMapped]
        public string? TransactionNumber { get; set; }
        [NotMapped]
        public string? TradeItemName { get; set; }
        [NotMapped]
        public int? SellerRating { get; set; }
        [NotMapped]
        public int? BuyerRating { get; set; }
    }
}
