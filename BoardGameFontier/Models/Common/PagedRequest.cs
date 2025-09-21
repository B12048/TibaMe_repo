using System.ComponentModel.DataAnnotations;

namespace BoardGameFontier.Models.Common
{
    /// <summary>
    /// 統一分頁請求參數
    /// 標準化所有分頁查詢的輸入格式
    /// </summary>
    public class PagedRequest
    {
        /// <summary>
        /// 頁碼（從 1 開始）
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "頁碼必須大於 0")]
        public int Page { get; set; } = 1;

        /// <summary>
        /// 每頁筆數
        /// </summary>
        [Range(1, 100, ErrorMessage = "每頁筆數必須在 1 到 100 之間")]
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// 計算跳過的記錄數
        /// </summary>
        public int Skip => (Page - 1) * PageSize;

        /// <summary>
        /// 取得每頁筆數（別名，方便使用）
        /// </summary>
        public int Take => PageSize;

        /// <summary>
        /// 驗證分頁參數是否有效
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            return Page > 0 && PageSize > 0 && PageSize <= 100;
        }

        /// <summary>
        /// 建立安全的分頁參數（確保參數在合理範圍內）
        /// </summary>
        /// <param name="page">頁碼</param>
        /// <param name="pageSize">每頁筆數</param>
        /// <returns>安全的分頁參數</returns>
        public static PagedRequest CreateSafe(int page, int pageSize)
        {
            return new PagedRequest
            {
                Page = Math.Max(1, page),
                PageSize = Math.Max(1, Math.Min(100, pageSize))
            };
        }
    }
}