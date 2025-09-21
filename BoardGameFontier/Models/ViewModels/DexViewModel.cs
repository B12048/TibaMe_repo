using BoardGameFontier.Models;
using BoardGameFontier.Repostiory.Entity;

namespace BoardGameFontier.Models.ViewModels
{
    public class DexViewModel
    {
        public GameDetail GameDetails { get; set; } // 桌遊詳細資料
        public List<Rating> Ratings { get; set; } // 評分資料
    }
}
