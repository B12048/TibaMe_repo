using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
// 移除對 Microsoft.AspNetCore.Identity 的引用
// using Microsoft.AspNetCore.Identity;

namespace BoardGameFontier.Repostiory.Entity
{
    /// <summary>
    /// 交易商品資料模型
    /// 這個類別對應到資料庫中的 TradeItems 表格
    /// 負責人：洪苡芯
    /// </summary>
    [Table("TradeItems")]
    public class TradeItem
    {
        /// <summary>
        /// 交易商品的唯一識別碼（主鍵）
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 商品名稱
        /// </summary>
        [Required(ErrorMessage = "商品名稱是必填的")]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 商品描述
        /// </summary>
        [StringLength(2000)]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// 商品價格
        /// Column: 指定資料庫的精確資料類型
        /// decimal(10,2) 表示總共10位數，小數點後2位
        /// </summary>
        [Required(ErrorMessage = "商品價格是必填的")]
        [Column(TypeName = "decimal(10,2)")]
        [Range(0.0, 999999.99)] // 確保價格在合理範圍內
        public decimal Price { get; set; }

        /// <summary>
        /// 商品圖片URL，以分號分隔多個URL
        /// </summary>
        [StringLength(2000)]
        public string? ImageUrls { get; set; } // 新增，用於存放多張圖片URL

        /// <summary>
        /// 庫存數量
        /// </summary>
        [Required]
        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; } = 1; // 新增，預設為1

        [Range(0, int.MaxValue)]
        public int Category { get; set; } = 1;

        /// <summary>
        /// 商品狀態 (例如：Available, SoldOut, Draft, Hidden)
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Available"; // 新增，預設為可販售

        /// <summary>
        /// 賣家ID（外鍵）
        /// 關聯到自訂的 UserProfile
        /// </summary>
        [Required]
        [StringLength(450)] // 與 UserProfile.Id 的長度保持一致
        public string SellerId { get; set; } = string.Empty;

        /// <summary>
        /// 建立時間
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// 更新時間
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // ===== 導航屬性 =====

        /// <summary>
        /// 導航屬性：商品賣家
        /// 關聯到自訂的 UserProfile 實體
        /// </summary>
        [ForeignKey("SellerId")]
        public virtual UserProfile Seller { get; set; } = null!;

        /// <summary>
        /// 導航屬性：所有關於此商品的問答
        /// </summary>
        public virtual ICollection<QuestionAnswer> QuestionAnswers { get; set; } = new List<QuestionAnswer>();

        /// <summary>
        /// 導航屬性：包含此商品的所有購物車項目
        /// </summary>
        public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

        /// <summary>
        /// 導航屬性：所有關於此商品的交易記錄 (作為被購買的商品)
        /// </summary>
        public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }

    /// <summary>
    /// 商品問答資料模型
    /// 負責人：洪苡芯
    /// </summary>
    [Table("QuestionAnswers")]
    public class QuestionAnswer
    {
        /// <summary>
        /// 問答的唯一識別碼（主鍵）
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 相關的交易商品ID（外鍵）
        /// </summary>
        [Required]
        public int TradeItemId { get; set; }

        /// <summary>
        /// 提問內容
        /// </summary>
        [Required]
        [StringLength(500)]
        public string Question { get; set; } = string.Empty;

        /// <summary>
        /// 回答內容
        /// </summary>
        [StringLength(1000)]
        public string? Answer { get; set; } // 可以為空，直到賣家回答

        /// <summary>
        /// 提問者ID（外鍵）
        /// 關聯到自訂的 UserProfile
        /// </summary>
        [Required]
        [StringLength(450)] // 與 UserProfile.Id 的長度保持一致
        public string QuestionerId { get; set; } = string.Empty;

        /// <summary>
        /// 回答者ID（外鍵，通常是賣家，可選）
        /// 關聯到自訂的 UserProfile
        /// </summary>
        [StringLength(450)] // 與 UserProfile.Id 的長度保持一致
        public string? AnswererId { get; set; }

        /// <summary>
        /// 是否已回答
        /// </summary>
        public bool IsAnswered { get; set; } = false; // 新增預設值

        /// <summary>
        /// 問題建立時間
        /// </summary>
        public DateTime QuestionCreatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// 回答建立時間
        /// </summary>
        public DateTime? AnswerCreatedAt { get; set; }

        // ===== 導航屬性 =====

        /// <summary>
        /// 導航屬性：相關的交易商品
        /// </summary>
        [ForeignKey("TradeItemId")]
        public virtual TradeItem TradeItem { get; set; } = null!;

        /// <summary>
        /// 導航屬性：提問者
        /// </summary>
        [ForeignKey("QuestionerId")]
        // 使用 InverseProperty 來明確定義多對多關係的兩端
        [InverseProperty("AskedQuestions")]
        public virtual UserProfile Questioner { get; set; } = null!;

        /// <summary>
        /// 導航屬性：回答者（可為空）
        /// </summary>
        [ForeignKey("AnswererId")]
        // 使用 InverseProperty 來明確定義多對多關係的兩端
        [InverseProperty("AnsweredQuestions")]
        public virtual UserProfile? Answerer { get; set; }
    }
}