using BoardGameFontier.DTOs;
using BoardGameFontier.Models;
using BoardGameFontier.Repostiory.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BoardGameFontier.Models.ViewModels
{
    public class HomeViewModel
    {
        public List<HomeHottestGameViewModel> HottestGame { get; set; } // 熱門遊戲排行榜
        public List<HomeReelViewModel> Reels { get; set; }// 限時動態資料
        public List<HomeNewsViewModel> News { get; set; } // 最新消息資料
    }
    public class HomeHottestGameViewModel
    {
        public int Id { get; set; }
        public string Cover { get; set; } // 封面圖
        public string engTitle { get; set; }
        public string zhtTitle { get; set; }
    }
    public class HomeReelViewModel
    {   
        public string UserName { get; set; } = string.Empty;
        public string ImageURL { get; set; } = string.Empty; 
       
    }
    public class HomeNewsViewModel
    {       
        public int Id { get; set; }       
        public string Title { get; set; }        
        public string Content { get; set; }       
        public string CoverURL { get; set; }
       
    }
}
