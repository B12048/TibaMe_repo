namespace BoardGameFontier.Models.Common
{
    /// <summary>
    /// 統一分頁回應結果
    /// 標準化所有分頁查詢的回傳格式
    /// </summary>
    /// <typeparam name="T">資料項目類型</typeparam>
    public class PagedResult<T>
    {
        /// <summary>
        /// 當前頁面的資料項目列表
        /// </summary>
        public List<T> Items { get; set; } = new List<T>();

        /// <summary>
        /// 總記錄數
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// 總頁數
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// 當前頁碼
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// 每頁筆數
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 是否有上一頁
        /// </summary>
        public bool HasPreviousPage => CurrentPage > 1;

        /// <summary>
        /// 是否有下一頁
        /// </summary>
        public bool HasNextPage => CurrentPage < TotalPages;

        /// <summary>
        /// 當前頁面的起始記錄編號（用於顯示 "顯示第 X 到 Y 筆"）
        /// </summary>
        public int StartRecord => TotalCount == 0 ? 0 : (CurrentPage - 1) * PageSize + 1;

        /// <summary>
        /// 當前頁面的結束記錄編號
        /// </summary>
        public int EndRecord => Math.Min(CurrentPage * PageSize, TotalCount);

        /// <summary>
        /// 是否為空結果
        /// </summary>
        public bool IsEmpty => !Items.Any();

        /// <summary>
        /// 建構函式 - 空結果
        /// </summary>
        public PagedResult()
        {
        }

        /// <summary>
        /// 建構函式 - 完整參數
        /// </summary>
        /// <param name="items">資料項目</param>
        /// <param name="totalCount">總記錄數</param>
        /// <param name="request">分頁請求參數</param>
        public PagedResult(List<T> items, int totalCount, PagedRequest request)
        {
            Items = items ?? new List<T>();
            TotalCount = totalCount;
            CurrentPage = request.Page;
            PageSize = request.PageSize;
            TotalPages = totalCount > 0 ? (int)Math.Ceiling((double)totalCount / request.PageSize) : 0;
        }

        /// <summary>
        /// 建立空的分頁結果
        /// </summary>
        /// <param name="request">分頁請求參數</param>
        /// <returns>空的分頁結果</returns>
        public static PagedResult<T> Empty(PagedRequest request)
        {
            return new PagedResult<T>(new List<T>(), 0, request);
        }

        /// <summary>
        /// 從現有資料建立分頁結果（用於記憶體中分頁）
        /// </summary>
        /// <param name="allItems">所有資料</param>
        /// <param name="request">分頁請求參數</param>
        /// <returns>分頁結果</returns>
        public static PagedResult<T> FromMemory(IEnumerable<T> allItems, PagedRequest request)
        {
            var itemsList = allItems.ToList();
            var totalCount = itemsList.Count;
            var pageItems = itemsList.Skip(request.Skip).Take(request.Take).ToList();
            
            return new PagedResult<T>(pageItems, totalCount, request);
        }
    }
}