using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BoardGameFontier.Repostiory.Entity
{
    public class Reel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // 自動生成主鍵值
        public int Id { get; set; } // 主鍵，Reel的唯一識別碼
        public string UserId { get; set; } //外鍵

        [Required(ErrorMessage = "請選擇上傳的圖片")]
        public string ImageURL { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public virtual UserProfile User { get; set; } //導覽屬性
    }
}