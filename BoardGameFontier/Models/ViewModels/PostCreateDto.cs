using BoardGameFontier.Repostiory.Entity.Social;
using System.ComponentModel.DataAnnotations;

namespace BoardGameFontier.Models.ViewModels
{
    /// <summary>
    /// 用於接收前端建立貼文請求的資料傳輸物件 (DTO)
    /// </summary>
    public class PostCreateDto
    {
        [StringLength(200)]
        public string? Title { get; set; }

        [Required(ErrorMessage = "貼文內容是必填的")]
        [StringLength(5000)]
        public string Content { get; set; } = string.Empty;

        public PostType Type { get; set; } = PostType.Review;

        public decimal? Price { get; set; }

        [StringLength(200)]
        public string? TradeLocation { get; set; }

        [StringLength(1000)]
        public string? TradeNotes { get; set; }

        /// <summary>
        /// 貼文圖片URL，以分號分隔多個URL（最多5張圖片）
        /// </summary>
        [StringLength(2000)]
        public string? ImageUrls { get; set; }

        public int? GameDetailId { get; set; }
    }
}
