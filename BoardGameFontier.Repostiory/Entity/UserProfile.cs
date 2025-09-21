using BoardGameFontier.Repostiory.Entity.Social;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BoardGameFontier.Repostiory.Entity
{
    /// <summary>
    /// 使用者詳細資料模型       
    /// 負責人：陳建宇
    /// </summary>
    [Table("UserProfiles")]
    public class UserProfile
    {
        /// <summary>
        /// 使用者ID（主鍵）       
        /// 建議使用 GUID 或 string，以便未來擴展或整合，這裡沿用 Identity 的 string 型別。
        /// </summary>
        [Key]
        [StringLength(450)] // 與 IdentityUser.Id 的長度保持一致，或根據需要調整
        public string Id { get; set; } = Guid.NewGuid().ToString(); // 新增預設值，確保每次建立都有唯一ID

        /// <summary>
        /// 使用者帳號 (登入用)
        /// 原 IdentityUser.UserName，用於登入。
        /// </summary>
        [Required(ErrorMessage = "使用者帳號是必填的")]
        [StringLength(256)]
        public string UserName { get; set; } = string.Empty;
        

        /// <summary>
        /// 電子郵件是否已驗證
        /// 原 IdentityUser.EmailConfirmed。
        /// </summary>
        public bool EmailConfirmed { get; set; } = false;

        /// <summary>
        /// 密碼雜湊值
        /// 原 IdentityUser.PasswordHash，存放加密後的密碼。
        /// </summary>
        [StringLength(256)] // 根據密碼雜湊算法的輸出長度調整
        public string? PasswordHash { get; set; }

        
        //密碼加鹽
        [StringLength(64)]
        public string? PasswordSalt { get; set; }

        [StringLength(128)]
        public string? ResetPwdToken { get; set; }

        public DateTime? ResetPwdTokenExpires { get; set; } = DateTime.Now.AddMinutes(5); // 有效期限

        //軟刪除帳號欄位        
        public bool IsDeleted { get; set; } = false; // 是否軟刪
        public DateTime? DeletedAt { get; set; } // 什麼時候刪



        /// <summary>
        /// 安全戳記，用於指示使用者憑證在資料庫中何時被修改（例如，密碼更改、角色更改）
        /// 原 IdentityUser.SecurityStamp。
        /// </summary>
        [StringLength(256)]
        public string? SecurityStamp { get; set; }

        /// <summary>
        /// 並發戳記，用於樂觀並發控制
        /// 原 IdentityUser.ConcurrencyStamp。
        /// </summary>
        [ConcurrencyCheck]
        [StringLength(256)]
        public string? ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString(); // 提供預設值

        

        /// <summary>
        /// 鎖定結束時間（如果帳戶被鎖定）
        /// 原 IdentityUser.LockoutEnd。
        /// </summary>
        public DateTimeOffset? LockoutEnd { get; set; }

        /// <summary>
        /// 帳戶是否可被鎖定
        /// 原 IdentityUser.LockoutEnabled。
        /// </summary>
        public bool LockoutEnabled { get; set; } = true;

       

        /// <summary>
        /// 顯示名稱（暱稱）
        /// </summary>
        [StringLength(100)]
        public string DisplayName { get; set; } = string.Empty;

        /// <summary>
        /// 生日
        /// </summary>
        public DateTime? Birthday { get; set; }

        /// <summary>
        /// 性別 (0: 未知, 1: 男性, 2: 女性)
        /// </summary>
        [Range(0, 2)]
        public int Gender { get; set; } = 0; // 新增，預設未知

        //桌遊偏好標籤
        [Column(TypeName = "nvarchar(max)")]
        public string? BoardGameTags { get; set; } = string.Empty; // 存 JSON 陣列字串

        /// <summary>
        /// 自我介紹
        /// </summary>
        [StringLength(500)]
        public string? Bio { get; set; } = string.Empty;

        /// <summary>
        /// 頭像圖片URL
        /// </summary>
        [StringLength(500)]
        public string? ProfilePictureUrl { get; set; } = string.Empty;

        //居住城市
        public string? City { get; set; } = string.Empty; // 新增，存放使用者所在城市

        /// <summary>
        /// 註冊時間
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// 最後更新時間
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // 以下為原 UserPrivacy 相關欄位，為了簡化並將隱私設定直接整合到 UserProfile 中


        //是否公開所在城市
        public bool IsCityHide { get; set; } = false;

        /// <summary>
        /// 是否公開個人檔案
        /// </summary>
        public bool IsProfileHide { get; set; } = false;

        /// <summary>
        /// 是否允許他人搜尋
        /// </summary>
        public bool AllowSearch { get; set; } = true;

       

        /// <summary>
        /// 是否顯示生日
        /// </summary>
        public bool ShowBirthday { get; set; } = false;

        /// <summary>
        /// 是否顯示性別
        /// </summary>
        public bool ShowGender { get; set; } = false;

        /// <summary>
        /// 是否顯示追蹤者清單
        /// </summary>
        public bool ShowFollowers { get; set; } = true;

        /// <summary>
        /// 是否顯示追蹤中清單
        /// </summary>
        public bool ShowFollowing { get; set; } = true;

        /// <summary>
        /// 是否顯示收藏清單
        /// </summary>
        public bool ShowFavorites { get; set; } = true;

        /// <summary>
        /// 是否顯示交易記錄
        /// </summary>
        public bool ShowTradeHistory { get; set; } = false;

      

        /// <summary>
        /// 是否接收追蹤通知
        /// </summary>
        public bool AllowFollowNotifications { get; set; } = true;

        /// <summary>
        /// 是否接收留言通知
        /// </summary>
        public bool AllowCommentNotifications { get; set; } = true;

        /// <summary>
        /// 是否接收按讚通知
        /// </summary>
        public bool AllowLikeNotifications { get; set; } = true;

        /// <summary>
        /// 是否接收交易通知
        /// </summary>
        public bool AllowTradeNotifications { get; set; } = true;

        // ===== 導航屬性 =====

        /// <summary>
        /// 導航屬性：此使用者發布的所有貼文
        /// </summary>
        public virtual ICollection<Post> Posts { get; set; } = new List<Post>();

        /// <summary>
        /// 導航屬性：此使用者發布的所有留言
        /// </summary>
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

        /// <summary>
        /// 導航屬性：此使用者按過的所有讚
        /// </summary>
        public virtual ICollection<Like> Likes { get; set; } = new List<Like>();

        /// <summary>
        /// 導航屬性：此使用者收到的所有通知
        /// </summary>
        [InverseProperty("Receiver")]
        public virtual ICollection<Notification> ReceivedNotifications { get; set; } = new List<Notification>();

        /// <summary>
        /// 導航屬性：此使用者觸發的所有通知
        /// </summary>
        [InverseProperty("Trigger")]
        public virtual ICollection<Notification> TriggeredNotifications { get; set; } = new List<Notification>();

        /// <summary>
        /// 導航屬性：此使用者作為追蹤者，追蹤了誰
        /// </summary>
        [InverseProperty("Follower")]
        public virtual ICollection<Follow> Following { get; set; } = new List<Follow>();

        /// <summary>
        /// 導航屬性：此使用者作為被追蹤者，被誰追蹤
        /// </summary>
        [InverseProperty("Followee")]
        public virtual ICollection<Follow> Followers { get; set; } = new List<Follow>();

        /// <summary>
        /// 導航屬性：此使用者收藏的所有項目
        /// </summary>
        public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();

        /// <summary>
        /// 導航屬性：此使用者發布的所有交易商品
        /// </summary>
        public virtual ICollection<TradeItem> TradeItems { get; set; } = new List<TradeItem>();

        /// <summary>
        /// 導航屬性：此使用者作為賣家，收到的所有商品問答
        /// </summary>
        [InverseProperty("Answerer")]
        public virtual ICollection<QuestionAnswer> AnsweredQuestions { get; set; } = new List<QuestionAnswer>();

        /// <summary>
        /// 導航屬性：此使用者作為買家，提出的所有商品問答
        /// </summary>
        [InverseProperty("Questioner")]
        public virtual ICollection<QuestionAnswer> AskedQuestions { get; set; } = new List<QuestionAnswer>();

        /// <summary>
        /// 導航屬性：此使用者參與的所有交易 (作為買家)
        /// </summary>
        [InverseProperty("Buyer")] //用來彼此對應
        public virtual ICollection<Transaction> BoughtTransactions { get; set; } = new List<Transaction>();

        /// <summary>
        /// 導航屬性：此使用者參與的所有交易 (作為賣家)
        /// </summary>
        [InverseProperty("Seller")]
        public virtual ICollection<Transaction> SoldTransactions { get; set; } = new List<Transaction>();

        /// <summary>
        /// 導航屬性：此使用者建立的所有檢舉
        /// </summary>
        public virtual ICollection<Report> ReportsMade { get; set; } = new List<Report>();

        /// <summary>
        /// 導航屬性：所有與此使用者相關的系統日誌
        /// </summary>
        public virtual ICollection<SystemLog> UserSystemLogs { get; set; } = new List<SystemLog>();

        /// <summary>
        /// 導航屬性：此使用者被分配到的角色
        /// </summary>
        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

        public virtual ICollection<Reel> Reels { get; set; } // 導航屬性：此使用者曾經發布過的限時動態

        public virtual ICollection<PrivateMessages> SentMessages { get; set; } // 導航屬性：此使用者曾發過的私訊
        public virtual ICollection<PrivateMessages> ReceivedMessages { get; set; } // 導航屬性：此使用者曾收過的私訊
        public ICollection<NewsComment> CommentContent { get; set; } // 導航屬性：此使用者曾發過的新聞評論

    }

}