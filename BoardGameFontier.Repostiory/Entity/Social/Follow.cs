using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BoardGameFontier.Repostiory.Entity.Social
{
    /// <summary>
    /// 追蹤關係資料模型
    /// 這個類別對應到資料庫中的 Follows 表格
    /// 處理使用者之間的追蹤關係
    /// 負責人：廖昊威
    /// </summary>
    [Table("Follows")]
    public class Follow
    {
        /// <summary>
        /// 追蹤關係的唯一識別碼（主鍵）
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 追蹤者ID（外鍵）
        /// 誰在追蹤別人，關聯到自訂的 UserProfile
        /// </summary>
        [Required]
        [StringLength(450)] // 與 UserProfile.Id 的長度保持一致
        public string FollowerId { get; set; } = string.Empty;

        /// <summary>
        /// 被追蹤者ID（外鍵）
        /// 被誰追蹤，關聯到自訂的 UserProfile
        /// </summary>
        [Required]
        [StringLength(450)] // 與 UserProfile.Id 的長度保持一致
        public string FolloweeId { get; set; } = string.Empty;

        /// <summary>
        /// 追蹤時間
        /// </summary>
        public DateTime CreatedAt { get; set; }

        // ===== 導航屬性 =====

        /// <summary>
        /// 導航屬性：追蹤者（誰在追蹤）
        /// 
        /// 關聯到 UserProfile.Following 集合
        /// EF Core 會自動建立關聯，無需手動設定外鍵值
        /// </summary>
        [ForeignKey("FollowerId")]
        public virtual UserProfile Follower { get; set; } = null!;

        /// <summary>
        /// 導航屬性：被追蹤者（被誰追蹤）
        /// 
        /// 關聯到 UserProfile.Followers 集合
        /// 透過 ApplicationDbContext.OnModelCreating 設定關聯規則
        /// </summary>
        [ForeignKey("FolloweeId")]
        public virtual UserProfile Followee { get; set; } = null!;
    }
}