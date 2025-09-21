using System.ComponentModel.DataAnnotations;

namespace BoardGameFontier.Repostiory.Entity
{
    public class GameDetail
    {
        [Key]
        public int Id { get; set; }
        [StringLength(20)]
        public string zhtTitle { get; set; } = "暫無中文名稱"; //中文名稱
        [Required(ErrorMessage ="請輸入英文標題")]
        [StringLength(30)]
        public string engTitle { get; set; } = string.Empty; //英文名稱
        public string Cover { get; set; } = string.Empty; //封面圖(來原先用bgg)
        [Required(ErrorMessage ="請輸入簡介")]
        [StringLength(40, MinimumLength = 10)]
        public string Intro { get; set; } = string.Empty; //簡介
        [Required(ErrorMessage = "請輸入上市年份")]
        public int YearReleased { get; set; } //上市年份
        public int OverallRank { get; set; } //全體排名
        public int BGFRank { get; set; } //本站排名
        [Required(ErrorMessage = "請輸入最少玩家人數")]
        public int MinPlayers { get; set; } //最少人數
        [Required(ErrorMessage = "請輸入最多玩家人數")]
        public int MaxPlayers { get; set; } //最多人數
        [Required(ErrorMessage = "請輸入最短遊戲時間")]
        public int MinTime { get; set; } //最短遊戲時間
        [Required(ErrorMessage = "請輸入最長遊戲時間")]
        public int MaxTime { get; set; } //最長遊戲時間
        [Required(ErrorMessage = "請輸入適用年齡")]
        public int Age {  get; set; } //適用年齡
        [Required(ErrorMessage = "請輸入簡介")]
        public float Weight { get; set; } //重度
        [Required(ErrorMessage = "請輸入設計師")]
        public string Designer { get; set; } = string.Empty; //設計師
        [Required(ErrorMessage = "請輸入美術繪師")]
        public string Artist { get; set; } = string.Empty; //美術
        [Required(ErrorMessage = "請輸入內容")]
        public string Description { get; set; } = string.Empty; //詳細介紹
        [Required]
        public string PictureURL1 { set; get; } = String.Empty;
        public string PictureURL2 { set; get; } = String.Empty;
        public string PictureURL3 { set; get; } = String.Empty;
        public string YoutubeURL { set; get; } = string.Empty;
    }
}