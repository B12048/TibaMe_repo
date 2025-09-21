using System.ComponentModel.DataAnnotations;
using BoardGameFontier.Repostiory.Entity.Social;

namespace BoardGameFontier.Models.ViewModels
{
    /// <summary>
    /// 貼文更新 DTO
    /// 用於編輯現有貼文的資料傳輸
    /// </summary>
    public class PostUpdateDto
    {
        /// <summary>
        /// 貼文標題（可選，部分貼文類型不需要標題）
        /// </summary>
        [StringLength(200, ErrorMessage = "標題最多 200 個字元")]
        public string? Title { get; set; }

        /// <summary>
        /// 貼文內容
        /// </summary>
        [Required(ErrorMessage = "內容不能為空")]
        [StringLength(5000, ErrorMessage = "內容最多 5000 個字元")]
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// 貼文類型（編輯時不允許修改）
        /// </summary>
        public PostType Type { get; set; }

        /// <summary>
        /// 關聯的桌遊 ID（可選）
        /// </summary>
        public int? GameDetailId { get; set; }

        // ===== 交易相關欄位（僅交易類型貼文） =====

        /// <summary>
        /// 交易價格（僅交易類型）
        /// </summary>
        [Range(0, 999999, ErrorMessage = "價格必須在 0-999999 之間")]
        public decimal? Price { get; set; }

        /// <summary>
        /// 交易地點（僅交易類型）
        /// </summary>
        [StringLength(100, ErrorMessage = "交易地點最多 100 個字元")]
        public string? TradeLocation { get; set; }

        /// <summary>
        /// 交易備註（僅交易類型）
        /// </summary>
        [StringLength(500, ErrorMessage = "交易備註最多 500 個字元")]
        public string? TradeNotes { get; set; }

        /// <summary>
        /// 圖片URL字串，以分號分隔多個URL（與Entity保持一致）
        /// </summary>
        [StringLength(2000, ErrorMessage = "圖片URL最多 2000 個字元")]
        public string? ImageUrls { get; set; }

        /// <summary>
        /// 驗證交易欄位
        /// 如果是交易類型貼文，確保必要欄位不為空
        /// </summary>
        public bool ValidateTradeFields()
        {
            if (Type == PostType.Trade)
            {
                return !string.IsNullOrWhiteSpace(TradeLocation);
            }
            return true;
        }
    }
}