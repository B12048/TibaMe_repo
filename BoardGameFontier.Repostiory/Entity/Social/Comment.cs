using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
// 移除對 Microsoft.AspNetCore.Identity 的引用
// using Microsoft.AspNetCore.Identity;

namespace BoardGameFontier.Repostiory.Entity.Social
{
    /// <summary>
    /// 留言資料模型
    /// 負責人：廖昊威
    /// </summary>
    [Table("Comments")]
    public class Comment
    {
        /// <summary>
        /// 留言的唯一識別碼（主鍵）
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 留言內容
        /// </summary>
        [Required(ErrorMessage = "留言內容是必填的")]
        [StringLength(1000)]
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// 留言者ID（外鍵）
        /// 現在關聯到自訂的 UserProfile
        /// </summary>
        [Required]
        [StringLength(450)] // 與 UserProfile.Id 的長度保持一致
        public string AuthorId { get; set; } = string.Empty;

        /// <summary>
        /// 所屬貼文ID（外鍵）
        /// </summary>
        [Required]
        public int PostId { get; set; }

        /// <summary>
        /// 父留言ID（用於回覆功能，如果是回覆別人的留言）
        /// 如果是 null，表示這是直接對貼文的留言
        /// </summary>
        public int? ParentCommentId { get; set; }

        /// <summary>
        /// 按讚數量
        /// </summary>
        public int LikeCount { get; set; }

        /// <summary>
        /// 建立時間
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 更新時間
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// 樂觀鎖定版本控制欄位
        /// 用於防止並發修改衝突
        /// </summary>
        [Timestamp]
        public byte[] RowVersion { get; set; } = Array.Empty<byte>();

        // ===== 導航屬性 =====

        /// <summary>
        /// 導航屬性：留言者
        /// 關聯到自訂的 UserProfile 實體
        /// </summary>
        [ForeignKey("AuthorId")]
        public virtual UserProfile Author { get; set; } = null!;

        /// <summary>
        /// 導航屬性：所屬貼文
        /// </summary>
        [ForeignKey("PostId")]
        public virtual Post Post { get; set; } = null!;

        /// <summary>
        /// 導航屬性：父留言（如果是回覆）
        /// </summary>
        [ForeignKey("ParentCommentId")]
        public virtual Comment? ParentComment { get; set; }

        /// <summary>
        /// 導航屬性：此留言的所有回覆
        /// </summary>
        public virtual ICollection<Comment> Replies { get; set; } = new List<Comment>();

        /// <summary>
        /// 導航屬性：所有對此留言的按讚記錄
        /// </summary>
        public virtual ICollection<Like> Likes { get; set; } = new List<Like>();
    }
}