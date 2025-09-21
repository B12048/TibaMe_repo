using System.ComponentModel.DataAnnotations;

namespace BoardGameFontier.Repostiory.Entity
{
    public class Rating
    {
        [Key]
        public int Id { get; set; } // 主鍵
        public int GameId { get; set; } = 1; // 外鍵，指向GameDetail的Id
        [Required]
        public int Stars { get; set; } // 評分星數，範圍1-5
        public string Title { get; set; } = string.Empty; // 評論標題
        public string Comments { get; set; } = string.Empty; // 評論內容
        public DateTime CreatedAt { get; set; } = DateTime.Now; // 建立時間
    }
}
