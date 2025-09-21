using Microsoft.EntityFrameworkCore;
using BoardGameFontier.Repostiory.Entity;
using BoardGameFontier.Repostiory.Entity.Social;

namespace BoardGameFontier.Repostiory
{
    /// <summary>
    /// 應用程式資料庫上下文
    /// 這是整個應用程式與資料庫溝通的橋樑
    /// 繼承自 IdentityDbContext 以支援 ASP.NET Core Identity
    /// </summary>
    public class ApplicationDbContext : DbContext // Note: Changed from DbContext to IdentityDbContext if it's handling Identity tables
    {
        /// <summary>
        /// 建構函式
        /// 接收資料庫連接設定
        /// </summary>
        /// <param name="options">資料庫連接選項</param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // ===== DbSet 屬性定義 =====
        // 每個 DbSet 對應到資料庫中的一個表格
        // Entity Framework 會使用這些屬性來生成和操作資料庫


        #region 劉益辰負責的部分
        public DbSet<GameDetail> GameDetails { get; set; }
        public DbSet<GameClickLog> GameClickLog { get; set; }
        public DbSet<Reel> Reel { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<ChatMsa> ChatMsaaaaaa { get; set; } //公共聊天室的資料庫
        public DbSet<ChatRoom> ChatRoom { get; set; } //私人聊天室房間(暫緩開發
        public DbSet<PrivateMessages> PrivateMessages { get; set; } //私訊的資料庫
        public DbSet<NewsComment> NewsComment { get; set; }
        public DbSet<Merchandise> Merchandise { get; set; }


        #endregion

        #region 廖昊威負責的部分
        /// <summary>
        /// 貼文資料表
        /// 負責人：廖昊威
        /// </summary>
        public DbSet<Post> Posts { get; set; }

        /// <summary>
        /// 留言資料表
        /// 負責人：廖昊威
        /// </summary>
        public DbSet<Comment> Comments { get; set; }

        /// <summary>
        /// 按讚記錄資料表
        /// 負責人：廖昊威
        /// </summary>
        public DbSet<Like> Likes { get; set; }

        /// <summary>
        /// 標籤資料表
        /// 負責人：廖昊威
        /// </summary>
        public DbSet<Tag> Tags { get; set; }

        /// <summary>
        /// 貼文標籤關聯表
        /// 負責人：廖昊威
        /// </summary>
        public DbSet<PostTag> PostTags { get; set; } // **ADDED**

        /// <summary>
        /// 追蹤關係資料表
        /// 負責人：廖昊威
        /// </summary>
        public DbSet<Follow> Follows { get; set; }

        /// <summary>
        /// 通知資料表
        /// 負責人：廖昊威
        /// </summary>
        public DbSet<Notification> Notifications { get; set; }
        #endregion

        #region 洪苡芯負責的部分
        /// <summary>
        /// 交易商品資料表
        /// 負責人：洪苡芯
        /// </summary>
        public DbSet<TradeItem> TradeItems { get; set; }

        /// <summary>
        /// 購物車資料表
        /// 負責人：洪苡芯
        /// </summary>
        public DbSet<Cart> Carts { get; set; }

        /// <summary>
        /// 購物車項目資料表
        /// 負責人：洪苡芯
        /// </summary>
        public DbSet<CartItem> CartItems { get; set; }

        /// <summary>
        /// 商品問答資料表
        /// 負責人：洪苡芯
        /// </summary>
        public DbSet<QuestionAnswer> QuestionAnswers { get; set; }
        #endregion

        #region 陳建宇負責的部分
        /// <summary>
        /// 使用者詳細資料表
        /// 負責人：陳建宇
        /// </summary>
        public DbSet<UserProfile> UserProfiles { get; set; }

        /// <summary>
        /// 收藏記錄資料表
        /// 負責人：陳建宇
        /// </summary>
        public DbSet<Favorite> Favorites { get; set; }

        /// <summary>
        /// 交易記錄資料表
        /// 負責人：陳建宇
        /// </summary>
        public DbSet<Transaction> Transactions { get; set; }

        //使用者帳號站存區
        public DbSet<TempUser> TempUsers { get; set; }

        //廠商訂單
        public DbSet<Order> Orders { get; set; }

        #endregion

        #region 王品瑄負責的部分
        /// <summary>
        /// 檢舉記錄資料表
        /// 負責人：王品瑄
        /// </summary>
        public DbSet<Report> Reports { get; set; }

        /// <summary>
        /// 網站公告資料表
        /// 負責人：王品瑄
        /// </summary>
        public DbSet<Announcement> Announcements { get; set; }

        /// <summary>
        /// 自訂使用者角色資料表
        /// 負責人：王品瑄
        /// </summary>
        public DbSet<UserRole> CustomUserRoles { get; set; }

        /// <summary>
        /// 系統記錄資料表
        /// 負責人：王品瑄
        /// </summary>
        public DbSet<SystemLog> SystemLogs { get; set; }
        #endregion

        /// <summary>
        /// 模型建立時的設定
        /// 這個方法用來設定實體之間的關聯關係、索引、約束等
        /// </summary>
        /// <param name="modelBuilder">模型建構器</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 必須先呼叫基底類別的方法，以正確設定 Identity 相關表格
            base.OnModelCreating(modelBuilder);

            // ===================================================================
            //                          複合主鍵設定 (Composite Primary Keys)
            // ===================================================================

            /// <summary>
            /// 設定 PostTag 的複合主鍵。
            /// 這是 Post 和 Tag 之間多對多關聯的聯結表，其主鍵由 PostId 和 TagId 共同組成。
            /// </summary>
            modelBuilder.Entity<PostTag>()
                .HasKey(pt => new { pt.PostId, pt.TagId });

            /// <summary>
            /// 設定 Like 的複合主鍵。
            /// 防止同一使用者對同一項目重複按讚，主鍵由 UserId、ItemType、ItemId 共同組成。
            /// </summary>
            modelBuilder.Entity<Like>()
                .HasKey(l => new { l.UserId, l.ItemType, l.ItemId });

            // ===================================================================
            //                          效能索引設定 (Performance Indexes)
            // ===================================================================

            /// <summary>
            /// 為 Like 表格添加複合索引，優化按讚查詢效能
            /// 用於快速查詢特定項目的所有按讚記錄
            /// 例如：取得某個貼文的所有按讚、計算按讚數量等
            /// </summary>
            modelBuilder.Entity<Like>()
                .HasIndex(l => new { l.ItemType, l.ItemId })
                .HasDatabaseName("IX_Likes_ItemType_ItemId");

            /// <summary>
            /// 為 Like 表格添加使用者索引，優化用戶相關查詢效能
            /// 用於快速查詢特定用戶的所有按讚記錄
            /// 例如：取得用戶按讚統計、檢查用戶按讚狀態等
            /// </summary>
            modelBuilder.Entity<Like>()
                .HasIndex(l => l.UserId)
                .HasDatabaseName("IX_Likes_UserId");

            /// <summary>
            /// 為 Like 表格添加時間索引，優化時間範圍查詢效能
            /// 用於統計分析和時間相關的按讚數據查詢
            /// 例如：取得某時間段的按讚趨勢、熱門內容分析等
            /// </summary>
            modelBuilder.Entity<Like>()
                .HasIndex(l => l.CreatedAt)
                .HasDatabaseName("IX_Likes_CreatedAt");


            // ===================================================================
            //                  解決多重串聯刪除路徑問題 (Multiple Cascade Paths)
            // 核心問題：當一個實體 (尤其是 UserProfile) 可以透過多條路徑刪除另一個實體時，
            // SQL Server 會禁止這種模棱兩可的串聯刪除規則。
            // 解決方法：我們手動將其中一條路徑的刪除行為從預設的 Cascade (串聯) 改為 Restrict (限制)。
            // 這表示您在刪除主體前，需要先在應用程式邏輯中手動處理關聯的子實體。
            // ===================================================================

            /// <summary>
            /// 處理 Follows 關聯：
            /// UserProfile 透過 FollowerId 和 FolloweeId 兩個外鍵都關聯到 Follows。
            /// 當刪除一個 UserProfile 時，為避免衝突，我們禁止任何一方直接觸發串聯刪除。
            /// </summary>
            modelBuilder.Entity<Follow>()
                .HasOne(f => f.Follower)
                .WithMany(u => u.Following)
                .HasForeignKey(f => f.FollowerId)
                .OnDelete(DeleteBehavior.Restrict); // 禁止串聯刪除

            modelBuilder.Entity<Follow>()
                .HasOne(f => f.Followee)
                .WithMany(u => u.Followers)
                .HasForeignKey(f => f.FolloweeId)
                .OnDelete(DeleteBehavior.Restrict); // 禁止串聯刪除

            /// <summary>
            /// 處理 Comments 的自我參照關聯 (回覆功能)：
            /// 一個留言 (ParentComment) 被刪除時，為避免產生無限循環，禁止其自動串聯刪除其下的所有回覆 (Replies)。
            /// </summary>
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.ParentComment)
                .WithMany(c => c.Replies)
                .HasForeignKey(c => c.ParentCommentId)
                .OnDelete(DeleteBehavior.Restrict); // 禁止串聯刪除

            /// <summary>
            /// 處理 Comments 與 UserProfile 的關聯：
            /// 刪除 UserProfile 時，存在兩條刪除路徑：
            /// 1. UserProfile -> Posts -> Comments
            /// 2. UserProfile -> Comments (直接)
            /// 我們保留路徑1的串聯刪除，並禁止路徑2，以解決衝突。
            /// </summary>
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Author)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.AuthorId)
                .OnDelete(DeleteBehavior.Restrict); // 禁止串聯刪除

            /// <summary>
            /// 處理 CartItems 與 TradeItems 的關聯：
            /// 刪除 UserProfile (賣家) 時，存在兩條刪除 CartItems 的路徑：
            /// 1. UserProfile (買家) -> Carts -> CartItems
            /// 2. UserProfile (賣家) -> TradeItems -> CartItems
            /// 我們禁止路徑2，避免一個商品下架時，自動從所有人的購物車中移除。
            /// </summary>
            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.TradeItem)
                .WithMany(ti => ti.CartItems)
                .HasForeignKey(ci => ci.TradeItemId)
                .OnDelete(DeleteBehavior.Restrict); // 禁止串聯刪除

            /// <summary>
            /// 處理 Transactions 關聯：
            /// UserProfile 透過 BuyerId 和 SellerId 兩個外鍵都關聯到 Transactions。
            /// 情況與 Follows 類似，刪除 UserProfile 時需禁止雙方的串聯刪除。
            /// </summary>
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Buyer)
                .WithMany(u => u.BoughtTransactions)
                .HasForeignKey(t => t.BuyerId)
                .OnDelete(DeleteBehavior.Restrict); // 禁止串聯刪除

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Seller)
                .WithMany(u => u.SoldTransactions)
                .HasForeignKey(t => t.SellerId)
                .OnDelete(DeleteBehavior.Restrict); // 禁止串聯刪除

            /// <summary>
            /// 處理 QuestionAnswers 關聯：
            /// UserProfile 透過 QuestionerId 和 AnswererId 兩個外鍵都關聯到 QuestionAnswers。
            /// 情況與 Follows 類似，刪除 UserProfile 時需禁止雙方的串聯刪除。
            /// </summary>
            modelBuilder.Entity<QuestionAnswer>()
                .HasOne(qa => qa.Questioner)
                .WithMany(u => u.AskedQuestions)
                .HasForeignKey(qa => qa.QuestionerId)
                .OnDelete(DeleteBehavior.Restrict); // 禁止串聯刪除

            modelBuilder.Entity<QuestionAnswer>()
                .HasOne(qa => qa.Answerer)
                .WithMany(u => u.AnsweredQuestions)
                .HasForeignKey(qa => qa.AnswererId)
                .OnDelete(DeleteBehavior.Restrict); // 禁止串聯刪除

            //設定聊天訊息與使用者帳號的資料綁定
            modelBuilder.Entity<ChatMsa>()
                .HasOne(m => m.User)
                .WithMany()
                .HasForeignKey(m => m.UserId);

            //設定限時動態與使用者帳號的資料綁定
            modelBuilder.Entity<Reel>()
                .HasOne(m => m.User)
                .WithMany(u=>u.Reels)
                .HasForeignKey(m => m.UserId);

            modelBuilder.Entity<PrivateMessages>()
                .HasOne(m=>m.Sender)
                .WithMany(u=>u.SentMessages)
                .HasForeignKey(p => p.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PrivateMessages>()
                .HasOne(m => m.Receiver)
                .WithMany(u=>u.ReceivedMessages)
                .HasForeignKey(p => p.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<NewsComment>()
                .HasOne(m => m.Commenter)
                .WithMany(u => u.CommentContent)
                .HasForeignKey(p => p.CommenterId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<NewsComment>()
                .HasOne(m => m.News)
                .WithMany(n => n.Comments)
                .HasForeignKey(p => p.NewsId);

           
            //軟刪除帳號
            modelBuilder.Entity<UserProfile>()
                .HasIndex(u => u.UserName)
                .HasDatabaseName("IX_UserProfiles_UserName_Active") //篩選式唯一索引，
                                                                    //用在釋放這個帳號讓使用者可再次註冊
                .IsUnique() //唯一性，只對0(未刪除)生效，用來判斷若帳號已刪除(1)
                            //，那使用者可以再創一個一樣的帳號
                .HasFilter("[IsDeleted] = 0"); 
            #region 新聞評論Seed資料
            modelBuilder.Entity<NewsComment>().HasData(
                new NewsComment
                {
                    Id = 1,
                    Content = "太棒了，這個我喜歡",
                    CommenterId = "82926659-b4d5-4a60-9f22-da872ccaf083",
                    NewsId = 12,
                },
                new NewsComment
                {
                    Id = 2,
                    Content = "我可是格鬥天王呢!",
                    CommenterId = "b654683c-9175-4666-b886-f12071b85683",
                    NewsId = 12,
                },
                new NewsComment
                {
                    Id = 3,
                    Content = "中文版代理希望!",
                    CommenterId = "d4662b9b-5651-4843-ba08-9ca121cfcf05",
                    NewsId = 12,
                });
            #endregion
            #region 圖鑑Seed資料
            modelBuilder.Entity<GameDetail>().HasData(
                new GameDetail
                {
                    Id = 1,
                    zhtTitle = "Brass: 伯明翰",
                    engTitle = "Brass: Birmingham",
                    Cover = "https://cf.geekdo-images.com/x3zxjr-Vw5iU4yDPg70Jgw__imagepagezoom/img/7a0LOL48K-7JNIOSGtcsNsIxkN0=/fit-in/1200x900/filters:no_upscale():strip_icc()/pic3490053.jpg",
                    PictureURL1 = "https://cf.geekdo-images.com/e2e0wdO8oVNzD0ZAhzi6MQ__imagepagezoom/img/Bi2I5xGnWdGy8aL247j-BjIYTWE=/fit-in/1200x900/filters:no_upscale():strip_icc()/pic3509697.jpg",
                    PictureURL2 = "https://cf.geekdo-images.com/7xNx9k_R_Hr3bp6LMbSJcA__imagepagezoom/img/IUmfX5VU270hz8Wq7fcGtGg4i5o=/fit-in/1200x900/filters:no_upscale():strip_icc()/pic3509698.jpg",
                    PictureURL3 = "https://cf.geekdo-images.com/zfnOi94URpuyNs_cRy7ESw__imagepagezoom/img/-2KRNQcyH22Gkdz0Nt3zwmy-LPU=/fit-in/1200x900/filters:no_upscale():strip_icc()/pic3500869.jpg",
                    YoutubeURL = "https://www.youtube.com/embed/KSYcQu6BMBA?si=kR87UgCkyow93Tc7",
                    Intro = "建造運輸網並發展工業，引領世界邁向工業革命。",
                    YearReleased = 2018,
                    OverallRank = 1,
                    BGFRank = 1,
                    MinPlayers = 2,
                    MaxPlayers = 4,
                    MinTime = 60,
                    MaxTime = 120,
                    Age = 14,
                    Weight = 3.87f,
                    Designer = "Martin Wallace",
                    Artist = "Gavan Brown",
                    Description = "《Brass：伯明翰》是馬丁華萊士2007年傑作《工業革命》的續作，是一款經濟策略遊戲。 《Brass：伯明翰》講述了1770年至1870年工業革命期間，伯明翰企業家們相互競爭的故事。\r\n\r\n            它提供了與前作截然不同的故事線和體驗。與前作一樣，你必須發展、建造和建立你的產業和網絡，以充分利用低或高的市場需求。遊戲分為兩個階段：運河時代（1770-1830年）和鐵路時代（1830-1870年）。獲得最多勝利點數（VP）即可獲勝。運河、鐵路和已建成（已翻轉）的產業板塊的勝利點數在每個階段結束時計算。\r\n\r\n            每輪，玩家根據回合順序軌跡輪流行動，並獲得兩個行動​​來執行以下任意行動（在原版遊戲中）：\r\n\r\n            1) 建造 - 支付所需資源並放置一個工業板塊。\r\n            2) 網路 - 鋪設鐵路/運河，擴展您的網路。\r\n            3) 發展 - 增加一個工業板塊的勝利點數。\r\n            4) 出售 - 出售您的棉花、製成品和陶器。\r\n            5) 貸款 - 貸款 30 英鎊並減少您的收入。\r\n\r\n            《Brass：伯明翰》也新增了第六個行動：\r\n\r\n            6) 偵察 - 棄掉三張牌，並取得一個隨機地點和隨機工業牌。 （此行動取代原版《工業革命》中的「雙重行動建造」。）"
                },
                new GameDetail
                {
                    Id = 2,
                    zhtTitle = "瘟疫危機傳承：第一季",
                    engTitle = "Pandemic Legacy: Season 1",
                    Cover = "https://cf.geekdo-images.com/-Qer2BBPG7qGGDu6KcVDIw__imagepagezoom/img/PCn9lAknaUcpXQ3U7Z3rxuNOqDM=/fit-in/1200x900/filters:no_upscale():strip_icc()/pic2452831.png",
                    PictureURL1 = "https://dailyworkerplacement.com/wp-content/uploads/2015/12/pleg1a-1541x1140.jpg",
                    PictureURL2 = "https://nerdologists.com/wp-content/uploads/2015/11/Pandemic.png",
                    PictureURL3 = "https://files.rebel.pl/products/100/302/_98794/rebel-pandemic-legacy-sezon-1-edycja-niebieska-foto2.jpg",
                    YoutubeURL = "https://www.youtube.com/embed/g5AfK-QwDaY?si=s-doaJBQpOIxYQ5I",
                    Intro = "突來的疾病正在世界大量散播中，你的團隊能拯救人類嗎？",
                    YearReleased = 2015,
                    OverallRank = 2,
                    BGFRank = 2,
                    MinPlayers = 2,
                    MaxPlayers = 4,
                    MinTime = 60,
                    MaxTime = 60,
                    Age = 13,
                    Weight = 2.83f,
                    Designer = "Matt Leacock",
                    Artist = "Chris Quilliams",
                    Description = "《瘟疫危機：傳承版》是一款合作式的劇情戰役遊戲，擁有貫穿整個劇情的主線故事，遊戲會依照玩家表現進行 12 到 24 場遊戲。\r\n\r\n遊戲一開始的機制與經典版《瘟疫危機》非常相似，你們將扮演一組對抗疾病的專家團隊，與時間賽跑，在世界各地移動，處理爆發的疫情熱點，同時研究四種致命疾病的解藥，防止情勢失控。\r\n\r\n每位玩家在自己的回合中可以進行四個動作，包括以不同方式移動（有時需棄掉手牌），建立研究站等建築物、治療疾病（從地圖上移除一顆疾病方塊；若某個顏色的疾病方塊全數清除，該疾病將被根除）、與其他玩家交換卡片，或是在研究站內棄掉五張同色卡片以研發該色疾病的解藥。每位玩家都擁有一個獨特角色，並具備能幫助完成任務的特殊能力。\r\n\r\n在玩家進行完動作後，需抽取兩張牌。這些牌可能包含「疫情」牌，會使地圖上新增疾病方塊，甚至引發「爆發」——使疾病蔓延至鄰近城市。若發生爆發，城市的恐慌等級會上升，使該城市的移動成本提高。\r\n\r\n每個月份，你們有兩次機會完成該月的任務目標。若第一次嘗試成功，即可直接進入下一個月份。若失敗，則會獲得第二次機會，並獲得更多資助（用來取得強力的事件卡）。\r\n\r\n在整個戰役過程中，會不斷加入新的規則與遊戲元件。有時你需要永久變更遊戲內容，例如在卡片上書寫、撕毀卡片，或是在元件上貼上永久貼紙。你的角色可能獲得新技能，也可能產生負面效果，甚至完全失去該角色，使其無法再被使用。"
                },
                new GameDetail
                {
                    Id = 3,
                    zhtTitle = "方舟動物園",
                    engTitle = "Ark Nova",
                    Cover = "https://cf.geekdo-images.com/k8F-Cp18G2zj6bTcH-a8IA__imagepagezoom/img/Vtxqxe4Y9OT9XGXh6TQxfzeldVM=/fit-in/1200x900/filters:no_upscale():strip_icc()/pic7385009.jpg",
                    PictureURL1 = "https://cf.geekdo-images.com/FmxTr5KHI7-TdSt23beK7A__imagepagezoom/img/W3tgiW45vMocYjbtkBLXsAbzrYc=/fit-in/1200x900/filters:no_upscale():strip_icc()/pic6465445.jpg",
                    PictureURL2 = "https://cf.geekdo-images.com/JAAD2e3Lrh8xa-neieiaJA__imagepagezoom/img/EJh24JC2WlGTSxbLk93ri_mNmvY=/fit-in/1200x900/filters:no_upscale():strip_icc()/pic6465443.jpg",
                    PictureURL3 = "https://cf.geekdo-images.com/L4Df_3VQig-lY8nLR7hVxA__imagepagezoom/img/JRMQ2Ohz3gfn98DG3v5SCNP5eHw=/fit-in/1200x900/filters:no_upscale():strip_icc()/pic6461338.jpg",
                    YoutubeURL = "https://www.youtube.com/embed/n9_yGqR6-0s?si=u8M7a9_x2D1rD_tI",
                    Intro = "打造世界級的現代化動物園，推動保育計畫，支援全球的動物園。",
                    YearReleased = 2021,
                    OverallRank = 3,
                    BGFRank = 3,
                    MinPlayers = 1,
                    MaxPlayers = 4,
                    MinTime = 90,
                    MaxTime = 150,
                    Age = 14,
                    Weight = 3.86f,
                    Designer = "Mathias Wigge",
                    Artist = "Loïc Billiau, Dennis Lohausen, Oliver Schlemmer",
                    Description = "《方舟動物園》是一款高度策略性的桌遊，玩家在遊戲中建造並管理自己的動物園，目標是打造出全球最成功、最受認可的科學化管理設施。遊戲結合了卡牌驅動、板塊放置、集合收集和目標達成等多種機制，提供了豐富的策略深度和高重玩價值。\r\n\r\n            遊戲的核心是行動點數系統，每位玩家擁有五個行動版塊，分別對應不同的行動：建造、動物、卡牌、合作夥伴和贊助者。每個行動版塊有其獨特的強度等級（1到5），使用後會移動到行動槽的最低位置，而其他版塊則向上移動並增強其效果。這種機制使得玩家需要仔細規劃行動順序，以最大化效率。\r\n\r\n            遊戲目標是同時提升「吸引力（Appeal）」和「保育分數（Conservation）」，當兩者在分數軌上相遇或超越時觸發遊戲結束。遊戲結束時，兩條分數軌差距最大者獲勝。這意味著玩家不僅要吸引遊客（透過引進多樣的動物和建造設施），還要積極參與全球保育計畫，釋放動物到野外，並與世界各地的動物園合作。\r\n\r\n            遊戲中包含大量的動物、保育計畫和贊助者卡牌，提供了豐富的選擇和組合，確保每場遊戲都有不同的策略路線。玩家需要管理動物、棲息地、建築物、大學合作夥伴和獨特的特殊建築，同時利用贊助者的支持來獲得額外效益。此外，遊戲還設有單人模式，讓玩家可以獨自挑戰，測試不同的策略。這些機制共同創造了一個引人入勝的遊戲體驗，讓玩家在管理動物園的同時，也能體驗到保育的樂趣與挑戰。"
                }
            );
            #endregion
            #region 評論Seed資料
            modelBuilder.Entity<Rating>().HasData(
            new Rating
            {
                Id = 1,
                GameId = 1, // 這裡假設遊戲ID為1，對應到Brass: 伯明翰
                Stars = 5,
                Title = "桌遊玩家一生必玩",
                Comments = "超喜歡這款遊戲的互動。每次決策將取決於對手的行動。行動通常不會帶來負面影響。對手的行動會為你提供（或取消）一些選項。這款遊戲在長期決策和短期決策之間取得了很好的平衡。你的8張牌限制了你的行動或去向，但你仍然可以用8張牌制定相當長遠的計劃。",
                CreatedAt = new DateTime(2025, 7, 7)
            },
            new Rating
            {
                Id = 2,
                GameId = 1, // 這裡假設遊戲ID為1，對應到Brass: 伯明翰
                Stars = 4,
                Title = "Masterpiece",
                Comments = "廢話不多說，傑作!",
                CreatedAt = new DateTime(2025, 7, 1)
            },
            new Rating
            {
                Id = 3,
                GameId = 1, // 這裡假設遊戲ID為1，對應到Brass: 伯明翰
                Stars = 3,
                Title = "推坑給女朋友玩",
                Comments = "今天第一次推坑給女朋友玩這款遊戲，對方除了能理解遊戲的玩法外，也非常投入整個遊戲過程。沒想到重度策略遊戲竟然能如此成功，雖然最後我不小心把她的關鍵酒桶搶走了，導致她生氣了，所以先給這個遊戲3分，之後再回來修改。",
                CreatedAt = new DateTime(2025, 6, 4)
            }
            );
            #endregion
            #region 新聞Seed資料
            modelBuilder.Entity<News>().HasData(
                new News
                {
                    Id = 1,
                    CoverURL = "https://i0.wp.com/mugandmeeple.co.uk/wp-content/uploads/2022/06/cropped-Meeple-Website-Icon.png?fit=512%2C512&ssl=1",
                    category = "公告",
                    Title = "Board Game Frontier 上線啦！",
                    Content = "歡迎來到 Board Game Frontier！這裡是桌遊愛好者的天堂，快來探索我們的遊戲庫和社群吧！",
                    Created = new DateTime(2025, 7, 1),
                    PageView = 0,
                    status = "已發佈",
                    Claps = 0
                },
                new News
                {
                    Id = 2,
                    CoverURL = "https://blogger.googleusercontent.com/img/b/R29vZ2xl/AVvXsEjPqFnJyF-TbnxVipI66deT4Q_aDbL2EpdWcSFNDg-DTfRZhOVEE1i_OuStHHmWSRr6a2ORBqXp77C8TYuWjSZK9Szjemm-9GWXSN0gpRL4479XPG4XaT0bikwIaWI1G9b1CCtgWFLyMw4hxwPxBzi6ATCVfVNtecvWxDzfVRWmwS55_hQpgBVYaVksNbg/s3308/DSC05330.JPG",
                    Title = "年度合作遊戲大賞得獎作品出爐。",
                    category = "新聞",
                    Content = "《Bomb Busters 炸彈剋星》是一款多人合作的拆炸彈遊戲，遊戲的基本規則就已經很有趣，但更驚人的是，後面居然還有６６道關卡等著玩家們挑戰！類承傳的遊戲體驗，還有很多貼心的小趣味設計～總之先一起來看看開箱吧～",
                    Created = new DateTime(2025, 7, 2),
                    status = "已發佈",
                    PageView = 0,
                    Claps = 0,
                }, new News
                {
                    Id = 3,
                    CoverURL = "https://scontent-tpe1-1.xx.fbcdn.net/v/t39.30808-6/476136865_1172660778195976_3361811058424119193_n.jpg?_nc_cat=102&ccb=1-7&_nc_sid=6ee11a&_nc_ohc=OPDeSSEznn4Q7kNvwErTFVg&_nc_oc=AdkOq-bxGIPWMMoQ4JdFrQmS_rMlS2ImKsn3d9_R6BreeSt_AhA3PDIqqGZdcxsOuhE&_nc_zt=23&_nc_ht=scontent-tpe1-1.xx&_nc_gid=vuvLK-r39w3PXUWCc5CS-A&oh=00_AfQyo2GONPPD-p9PU-cbwCvkufoX_Wic3rPMG_yLQcHBAQ&oe=6883FBB5",
                    Title = "[新銳25]台灣最盛大的桌上遊戲展覽，將於9月24日於松菸舉辦",
                    category = "活動",
                    Content = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec vitae ex a enim aliquam placerat vitae tempor odio. Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos himenaeos. Praesent eu mauris nec nisi tempor aliquam. Integer id cursus neque. Morbi convallis eget nisi sed ullamcorper. Praesent mollis sapien quis vehicula luctus. Curabitur fringilla, risus nec efficitur fringilla, diam tellus maximus nibh, et posuere odio massa in arcu. Vestibulum faucibus eros sollicitudin lacus mollis, a tempus lacus egestas. Nam vitae lectus ipsum. Phasellus eget condimentum leo, quis suscipit massa. Cras scelerisque lorem quis nulla egestas, feugiat gravida magna vulputate. Nunc dapibus dapibus volutpat. Fusce aliquet fringilla felis id mattis. Donec finibus auctor felis. Nulla ac efficitur eros. Aenean lectus urna, pretium non luctus nec, suscipit in lorem.\r\n\r\nFusce non consequat quam, bibendum sodales quam. Ut venenatis nulla at metus facilisis malesuada. Vivamus vitae enim dui. Etiam eu convallis est, ornare aliquam augue. Etiam tincidunt nisl ac ultrices imperdiet. Donec tempus purus nec tincidunt facilisis. Nulla vehicula leo in eleifend suscipit. Duis congue lacus non vulputate sollicitudin. Duis hendrerit eleifend eros at bibendum. In pellentesque fringilla diam, eget porttitor leo sollicitudin vel. Nam sed nulla purus. Pellentesque auctor nec eros ac iaculis. Aliquam sed lorem scelerisque, scelerisque ligula vel, faucibus dui.\r\n\r\nMaecenas et gravida magna, sit amet placerat nisl. Ut porta elit ac gravida vulputate. Aliquam eu bibendum est. Phasellus orci tortor, malesuada id enim sit amet, molestie mattis ante. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia curae; Integer placerat velit nulla. Mauris suscipit dui at porttitor interdum. Nunc dignissim ultrices faucibus. Nunc bibendum sit amet turpis eu aliquet. Duis ac lectus consectetur, pulvinar justo vel, pretium elit. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Praesent vitae ipsum porttitor, malesuada augue non, mollis magna. Donec eleifend pretium semper. Aliquam facilisis sagittis nisl, nec suscipit tellus accumsan eget. Sed mollis volutpat tincidunt. Nullam sed auctor dolor, in interdum sapien.\r\n\r\nMauris maximus quis elit non egestas. Pellentesque accumsan erat et turpis mattis volutpat. Aliquam convallis ipsum ac sapien commodo commodo ac in quam. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Aliquam id ante est. Sed dignissim ac urna auctor luctus. Quisque non porta massa. Sed nibh quam, posuere ut viverra et, egestas egestas neque. Nulla condimentum turpis ac lobortis convallis. Fusce risus dolor, vestibulum at massa mattis, malesuada consequat lectus. Donec maximus ligula quis urna iaculis laoreet. Nunc sed rhoncus magna. Praesent quis sollicitudin dui.\r\n\r\nDonec luctus diam nisi. Morbi ultricies ipsum sapien, et molestie risus sagittis et. Nunc bibendum vitae nulla et posuere. Nulla facilisi. Integer pharetra, augue sed scelerisque vehicula, mauris mi scelerisque orci, eget varius felis quam ullamcorper lectus. Nunc rutrum metus sit amet diam porta facilisis. Quisque pellentesque diam lectus, vitae aliquet metus tincidunt vitae. Maecenas congue leo eget magna luctus, ac tempor massa fringilla. Integer euismod magna eu erat venenatis, eget pharetra justo sagittis. Aenean laoreet ultrices molestie. Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos himenaeos. In eget leo non purus convallis scelerisque at et ligula.",
                    Created = new DateTime(2025, 7, 3),
                    status = "已發佈",
                    PageView = 0,
                    Claps = 0,
                }
                );
            #endregion
            #region 限時動態Seed資料
            modelBuilder.Entity<Reel>().HasData(
            new Reel
            {
                Id = 1,
                UserId = "2abcd73f-e5ee-45ec-b831-e3a8e44b7f1f",
                ImageURL = "https://cf.geekdo-images.com/TMVDTy12ItHsaZ-JAYGpxg__imagepage/img/tFZjDb0JcpmEMADhCrFAzTBNriI=/fit-in/900x600/filters:no_upscale():strip_icc()/pic7744212.jpg"
            },
            new Reel
            {
                Id = 2,
                UserId = "82926659-b4d5-4a60-9f22-da872ccaf083",
                ImageURL = "https://cf.geekdo-images.com/c0PuKfxG50cj5ckVDhzjPw__imagepage/img/tv_s02vyRBmG3TBWszeMNjp3tSY=/fit-in/900x600/filters:no_upscale():strip_icc()/pic6559921.jpg"
            },
            new Reel
            {
                Id = 3,
                UserId = "b654683c-9175-4666-b886-f12071b85683",
                ImageURL = "https://cf.geekdo-images.com/klFnibADgbKMTmf8IUUksQ__imagepagezoom/img/yylBySya3gsdkJqNkV8a08SvKK4=/fit-in/1200x900/filters:no_upscale():strip_icc()/pic4250196.jpg"
            },
            new Reel
            {
                Id = 4,
                UserId = "d4662b9b-5651-4843-ba08-9ca121cfcf05",
                ImageURL = "https://cf.geekdo-images.com/FmxTr5KHI7-TdSt23beK7A__imagepagezoom/img/W3tgiW45vMocYjbtkBLXsAbzrYc=/fit-in/1200x900/filters:no_upscale():strip_icc()/pic6465445.jpg"
            },
            new Reel
            {
                Id = 5,
                UserId = "test-user-001",
                ImageURL = "https://cf.geekdo-images.com/Kl7Ozg_a4GLkwE2kg9pAzQ__imagepagezoom/img/SsINuYi9NetbxGVVqN6iyoaaGFM=/fit-in/1200x900/filters:no_upscale():strip_icc()/pic8967559.jpg"
            });
            #endregion
            #region 聊天室seed資料
            modelBuilder.Entity<ChatMsa>().HasData(
                new ChatMsa
                {
                    Id = 1,
                    UserId = "d4662b9b-5651-4843-ba08-9ca121cfcf05",
                    Message = "麥當勞葛格請幫我買麥當勞",
                    CreatedAt = new DateTime(2025, 7, 23, 11, 49, 48)
                }, 
                new ChatMsa
                {
                    Id = 2,
                    UserId = "b654683c-9175-4666-b886-f12071b85683",
                    Message = "母湯啊老師",
                    CreatedAt = new DateTime(2025, 7, 23, 11, 50, 10)
                }, 
                new ChatMsa
                {
                    Id = 3,
                    UserId = "82926659-b4d5-4a60-9f22-da872ccaf083",
                    Message = "就是說啊老師",
                    CreatedAt = new DateTime(2025, 7, 23, 11, 50, 48)
                },
                new ChatMsa
                {
                    Id = 4,
                    UserId = "82926659-b4d5-4a60-9f22-da872ccaf083",
                    Message = "推薦老師吃蛋白盒子",
                    CreatedAt = new DateTime(2025, 7, 23, 11, 50, 59)
                },
                new ChatMsa
                {
                    Id = 5,
                    UserId = "b654683c-9175-4666-b886-f12071b85683",
                    Message = "老師，蛋白盒子真的好吃推薦",
                    CreatedAt = new DateTime(2025, 7, 23, 11, 51, 58)
                },
                new ChatMsa
                {
                    Id = 6,
                    UserId = "d4662b9b-5651-4843-ba08-9ca121cfcf05",
                    Message = "好吧，那我決定吃Subway",
                    CreatedAt = new DateTime(2025, 7, 23, 11, 52, 15)
                },
                new ChatMsa
                {
                    Id = 7,
                    UserId = "b654683c-9175-4666-b886-f12071b85683",
                    Message = "昏倒！",
                    CreatedAt = new DateTime(2025, 7, 23, 11, 52, 29)
                },
                new ChatMsa
                {
                    Id = 8,
                    UserId = "d4662b9b-5651-4843-ba08-9ca121cfcf05",
                    Message = "感謝麥當勞葛格，我想要10層牛肉＋10倍起司",
                    CreatedAt = new DateTime(2025, 7, 23, 11, 53, 02)
                },
                new ChatMsa
                {
                    Id = 9,
                    UserId = "d4662b9b-5651-4843-ba08-9ca121cfcf05",
                    Message = "飲料要可樂，特大杯",
                    CreatedAt = new DateTime(2025, 7, 23, 11, 54, 12)
                },
                new ChatMsa
                {
                    Id = 10,
                    UserId = "d4662b9b-5651-4843-ba08-9ca121cfcf05",
                    Message = "順便有什麼可以加購都買一買",
                    CreatedAt = new DateTime(2025, 7, 23, 11, 54, 21)
                },
                new ChatMsa
                {
                    Id = 11,
                    UserId = "82926659-b4d5-4a60-9f22-da872ccaf083",
                    Message = "好喔",
                    CreatedAt = new DateTime(2025, 7, 23, 11, 54, 38)
                }
            );
            #endregion
            #region 私訊seed資料
            modelBuilder.Entity<PrivateMessages>().HasData(
                new PrivateMessages
                {
                    Id = 1,
                    SenderId = "d4662b9b-5651-4843-ba08-9ca121cfcf05",  //帳號: 001@gmail.com
                    ReceiverId = "b654683c-9175-4666-b886-f12071b85683", //帳號: 002@gmail.com
                    Message = "Tank你今天中午要吃麥當勞嗎?",
                    CreatedAt = new DateTime(2025, 7, 23, 11, 49, 48),
                    IsRead = true,
                    ReadAt = new DateTime(2025, 7, 23, 11, 50, 00),
                },
                new PrivateMessages
                {
                    Id = 2,
                    SenderId = "b654683c-9175-4666-b886-f12071b85683",  //帳號: 002@gmail.com
                    ReceiverId = "d4662b9b-5651-4843-ba08-9ca121cfcf05", //帳號: 001@gmail.com
                    Message = "又吃麥當勞!",
                    CreatedAt = new DateTime(2025, 7, 23, 11, 50, 20),
                    IsRead = true,
                    ReadAt = new DateTime(2025, 7, 23, 11, 50, 30),
                }, 
                new PrivateMessages
                {
                    Id = 3,
                    SenderId = "b654683c-9175-4666-b886-f12071b85683",  //帳號: 002@gmail.com
                    ReceiverId = "d4662b9b-5651-4843-ba08-9ca121cfcf05", //帳號: 001@gmail.com
                    Message = "好吧 算我一份!",
                    CreatedAt = new DateTime(2025, 7, 23, 11, 50, 38),
                    IsRead = true,
                    ReadAt = new DateTime(2025, 7, 23, 11, 50, 46),
                });
            #endregion
            #region 商城seed資料
            modelBuilder.Entity<Merchandise>().HasData(
                new Merchandise
                {
                    Id = 1,
                    GameDetailId = 4,  //併購風雲
                    IndexBannerURL= "https://img.cashier.ecpay.com.tw/image/2023/11/8/0_eff20c85280d2dff97cbc09da624142359960c96.png",
                    CoverURL = "https://img.cashier.ecpay.com.tw/image/2023/11/8/225885_dcaeb9229cf0bd2cec50445b2f3105b53df36f43.jpg", //帳號: 002@gmail.com
                    ImageGalleryJson = "",
                    Description = "經典經濟股票遊戲《Acquire》在歷經多年的絕版與再版後，於今年推出最新版本，同場加映前所未有的繁體中文版。支援2~6人遊戲，每位玩家從持有一筆小資金開始遊戲，藉由自己獨特的投資眼光在一系列的公司併購案中，脫穎而出吧。",
                    Brand = "彌生戲夢",
                    Price = 1680,
                    DiscountPrice = 1200,
                    Stock = 100,
                    Category = "家庭遊戲",
                    IsActive = true,
                },
                new Merchandise
                {
                    Id = 2,
                    GameDetailId = 1005,  //殺戮尖塔
                    IndexBannerURL = "https://img.cashier.ecpay.com.tw/image/2023/11/8/0_09c76e998d199d42398128fd9e37fb7405b31cd4.png",
                    CoverURL = "https://img.cashier.ecpay.com.tw/image/2023/11/8/225885_dcaeb9229cf0bd2cec50445b2f3105b53df36f43.jpg", //帳號: 002@gmail.com
                    ImageGalleryJson = "",
                    Description = "與最多三名朋友一起登上這座尖塔，擊敗邪惡的高塔之心。本遊戲為繁體中文版，附贈集資解鎖獎勵盒",
                    Brand = "彌生戲夢",
                    Price = 5000,
                    DiscountPrice = 4500,
                    Stock = 20,
                    Category = "玩家遊戲",
                    IsActive = true,
                });
            #endregion

        }
    }
}