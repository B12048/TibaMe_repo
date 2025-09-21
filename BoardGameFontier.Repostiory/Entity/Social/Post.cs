using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
// 移除對 Microsoft.AspNetCore.Identity 的引用
// using Microsoft.AspNetCore.Identity;

namespace BoardGameFontier.Repostiory.Entity.Social
{
    /// <summary>
    /// 貼文資料模型
    /// 這個類別對應到資料庫中的 Posts 表格
    /// 負責人：廖昊威
    /// </summary>
    [Table("Posts")]
    public class Post
    {
        /// <summary>
        /// 貼文的唯一識別碼（主鍵）
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 貼文標題（可選）
        /// </summary>
        [StringLength(200)]
        public string? Title { get; set; }

        /// <summary>
        /// 貼文內容
        /// </summary>
        [Required(ErrorMessage = "貼文內容是必填的")]
        [StringLength(5000)]
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// 貼文類型（心得、詢問、交易、創作）
        /// </summary>
        public PostType Type { get; set; } = PostType.Review;

        /// <summary>
        /// 交易價格（僅交易型貼文使用）
        /// Column: 指定資料庫的精確資料類型
        /// decimal(18,2) 表示總共18位數，小數點後2位
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        [Range(0.0, 9999999999.99)] // 確保價格在合理範圍內
        public decimal? Price { get; set; }

        /// <summary>
        /// 交易地點（僅交易型貼文使用）
        /// </summary>
        [StringLength(200)]
        public string? TradeLocation { get; set; }

        /// <summary>
        /// 交易備註（僅交易型貼文使用）
        /// </summary>
        [StringLength(1000)]
        public string? TradeNotes { get; set; }

        /// <summary>
        /// 貼文圖片URL，以分號分隔多個URL
        /// </summary>
        [StringLength(2000)]
        public string? ImageUrls { get; set; }

        /// <summary>
        /// 貼文者ID（外鍵）
        /// 關聯到自訂的 UserProfile
        /// </summary>
        [Required]
        [StringLength(450)] // 與 UserProfile.Id 的長度保持一致
        public string AuthorId { get; set; } = string.Empty;

        /// <summary>
        /// 相關桌遊ID（外鍵，可選）
        /// </summary>
        public int? GameDetailId { get; set; }

        /// <summary>
        /// 按讚數量
        /// </summary>
        public int LikeCount { get; set; }

        /// <summary>
        /// 留言數量
        /// </summary>
        public int CommentCount { get; set; }

        /// <summary>
        /// 瀏覽次數
        /// </summary>
        public int ViewCount { get; set; }

        /// <summary>
        /// 是否為熱門貼文
        /// </summary>
        public bool IsPopular { get; set; }

        /// <summary>
        /// 是否被置頂
        /// </summary>
        public bool IsPinned { get; set; }

        /// <summary>
        /// 建立時間
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// 更新時間
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// 樂觀鎖定版本控制欄位
        /// 用於防止並發修改衝突
        /// </summary>
        [Timestamp]
        public byte[] RowVersion { get; set; } = Array.Empty<byte>();

        // ===== 導航屬性 =====

        /// <summary>
        /// 導航屬性：發佈者
        /// 關聯到自訂的 UserProfile 實體
        /// </summary>
        [ForeignKey("AuthorId")]
        public virtual UserProfile Author { get; set; } = null!;

        /// <summary>
        /// 導航屬性：相關的桌遊
        /// </summary>
        [ForeignKey("GameDetailId")]
        public virtual GameDetail? GameDetail { get; set; }

        /// <summary>
        /// 導航屬性：貼文的所有留言
        /// </summary>
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

        /// <summary>
        /// 導航屬性：貼文的所有按讚記錄
        /// </summary>
        public virtual ICollection<Like> Likes { get; set; } = new List<Like>();

        /// <summary>
        /// 導航屬性：貼文所使用的標籤 (多對多關係)
        /// </summary>
        public virtual ICollection<PostTag> PostTags { get; set; } = new List<PostTag>();
    }
}