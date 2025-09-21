using System.ComponentModel.DataAnnotations;

namespace BoardGameFontier.Repostiory.Entity
{
    public class News
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string category { get; set; } //分類 : 公告/新聞/活動
        [Required]
        public string Content { get; set; }
        [Required]
        public string CoverURL {  get; set; }  //封面圖片 (用URL呈現)
        [Required]
        public string status { get; set; } //狀態
        public int PageView { get; set; } = 0;
        public int Claps { get; set; } = 0;
        public DateTime Created { get; set; } = DateTime.Now;
        public bool pinTop { get; set; } = false; //置頂此文章
        public ICollection<NewsComment>? Comments { get; set; }
    }
}


