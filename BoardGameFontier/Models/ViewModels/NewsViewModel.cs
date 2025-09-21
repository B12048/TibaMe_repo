using BoardGameFontier.Models;
using BoardGameFontier.Repostiory.Entity;

namespace BoardGameFontier.Models.ViewModels
{
    public class NewsViewModel
    {
        public News news { get; set; } // 桌遊詳細資料
        //public GameDetail RelatedGames { get; set; } // 關聯桌遊
        public List<NewsComment> NewsComment { get; set; }
    }
}
