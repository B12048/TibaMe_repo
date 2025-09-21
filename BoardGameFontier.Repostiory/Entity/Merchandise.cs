using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoardGameFontier.Repostiory.Entity
{
    public class Merchandise
    {
            [Key]
            public int Id { get; set; } // 商品唯一識別碼
            public int GameDetailId { get; set; }  //外鍵
            [Required]
            public string CoverURL { get; set; } // 封面圖片 URL
            public string IndexBannerURL { get; set; } // 顯示在首頁旋轉木馬的URL
            public string ImageGalleryJson { get; set; } // 其他商品圖片（用 JSON 儲存陣列）
            [Required]
            public string Description { get; set; } // 商品描述（詳細說明）
            [Required]
            [MaxLength(50)]
            public string Brand { get; set; } // 品牌
            [Required]
            public int Price { get; set; } // 售價
            public int? DiscountPrice { get; set; } // 折扣價（可選）
            public int Stock { get; set; } // 庫存量
            [Required]
            public string Category { get; set; } // 分類（或 CategoryId FK）
            public bool IsActive { get; set; } = true; // 是否上架
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // 建立時間
            public DateTime? UpdatedAt { get; set; } // 更新時間
            public GameDetail GameDetail { get; set; }// 導覽屬性
    }
}
