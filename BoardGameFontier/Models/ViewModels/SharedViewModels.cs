using BoardGameFontier.Repostiory.Entity.Social;

namespace BoardGameFontier.Models.ViewModels
{
    /// <summary>
    /// 分頁 ViewModel
    /// 提供統一的分頁資訊處理
    /// </summary>
    public class PaginationViewModel
    {
        /// <summary>
        /// 當前頁碼
        /// </summary>
        public int CurrentPage { get; set; } = 1;

        /// <summary>
        /// 總頁數
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// 總項目數
        /// </summary>
        public int TotalItems { get; set; }

        /// <summary>
        /// 每頁顯示項目數
        /// </summary>
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// 是否有上一頁
        /// </summary>
        public bool HasPreviousPage { get; set; }

        /// <summary>
        /// 是否有下一頁
        /// </summary>
        public bool HasNextPage { get; set; }

        /// <summary>
        /// 開始項目編號
        /// </summary>
        public int StartItem => (CurrentPage - 1) * PageSize + 1;

        /// <summary>
        /// 結束項目編號
        /// </summary>
        public int EndItem => Math.Min(CurrentPage * PageSize, TotalItems);

        /// <summary>
        /// 建立分頁資訊
        /// </summary>
        /// <param name="currentPage">當前頁碼</param>
        /// <param name="pageSize">每頁大小</param>
        /// <param name="totalItems">總項目數</param>
        /// <returns>分頁 ViewModel</returns>
        public static PaginationViewModel Create(int currentPage, int pageSize, int totalItems)
        {
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            
            return new PaginationViewModel
            {
                CurrentPage = Math.Max(1, Math.Min(currentPage, totalPages)),
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages
            };
        }

        /// <summary>
        /// 取得頁碼範圍（用於分頁導航）
        /// </summary>
        /// <param name="range">顯示範圍</param>
        /// <returns>頁碼列表</returns>
        public List<int> GetPageRange(int range = 5)
        {
            var start = Math.Max(1, CurrentPage - range / 2);
            var end = Math.Min(TotalPages, start + range - 1);
            
            // 調整開始位置，確保總是顯示指定數量的頁碼
            start = Math.Max(1, end - range + 1);
            
            return Enumerable.Range(start, end - start + 1).ToList();
        }
    }

    /// <summary>
    /// 統一的作者資訊 ViewModel
    /// 用於貼文、留言等所有需要顯示作者資訊的地方
    /// </summary>
    public class AuthorViewModel
    {
        /// <summary>
        /// 使用者 ID
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// 使用者 ID（別名，向後相容）
        /// </summary>
        public string UserId 
        { 
            get => Id; 
            set => Id = value; 
        }

        /// <summary>
        /// 使用者名稱
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// 顯示名稱
        /// </summary>
        public string DisplayName { get; set; } = string.Empty;

        /// <summary>
        /// 頭像 URL
        /// </summary>
        public string? ProfilePictureUrl { get; set; }

        /// <summary>
        /// 使用者等級
        /// </summary>
        public int Level { get; set; } = 1;

        /// <summary>
        /// 是否為驗證用戶
        /// </summary>
        public bool IsVerified { get; set; }

        /// <summary>
        /// 當前用戶是否已追蹤此作者
        /// </summary>
        public bool IsFollowed { get; set; } = false;

        /// <summary>
        /// 等級顯示文字
        /// </summary>
        public string LevelDisplay => $"Lv.{Level}";

        /// <summary>
        /// 頭像 URL（含預設值）
        /// </summary>
        public string AvatarUrl => !string.IsNullOrEmpty(ProfilePictureUrl) 
            ? ProfilePictureUrl 
            : "/img/noPortrait.png";

        /// <summary>
        /// 使用者連結
        /// </summary>
        public string UserProfileUrl => $"/Member/Profile/{Id}";
    }


    // 注意：ApiResponseViewModel<T> 已被移除
    // 所有 API 現在統一使用 BoardGameFontier.Models.Common.ApiResponse<T>
    // 這提供了更完整的功能和標準化的錯誤處理格式

    /// <summary>
    /// 排序選項 ViewModel
    /// 通用的排序選項處理
    /// </summary>
    public class SortOptionViewModel
    {
        /// <summary>
        /// 排序欄位
        /// </summary>
        public string Field { get; set; } = string.Empty;

        /// <summary>
        /// 排序方向
        /// </summary>
        public SortDirection Direction { get; set; } = SortDirection.Descending;

        /// <summary>
        /// 顯示名稱
        /// </summary>
        public string DisplayName { get; set; } = string.Empty;

        /// <summary>
        /// 是否為當前排序
        /// </summary>
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// 排序方向枚舉
    /// </summary>
    public enum SortDirection
    {
        /// <summary>
        /// 升序
        /// </summary>
        Ascending = 0,

        /// <summary>
        /// 降序
        /// </summary>
        Descending = 1
    }

    /// <summary>
    /// 操作回應 ViewModel
    /// 用於 AJAX 操作的標準回應
    /// </summary>
    public class OperationResultViewModel
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 訊息
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// 重新導向 URL
        /// </summary>
        public string? RedirectUrl { get; set; }

        /// <summary>
        /// 額外資料
        /// </summary>
        public object? Data { get; set; }

        /// <summary>
        /// 建立成功結果
        /// </summary>
        public static OperationResultViewModel CreateSuccess(string message, object? data = null, string? redirectUrl = null)
        {
            return new OperationResultViewModel
            {
                Success = true,
                Message = message,
                Data = data,
                RedirectUrl = redirectUrl
            };
        }

        /// <summary>
        /// 建立失敗結果
        /// </summary>
        public static OperationResultViewModel CreateFailure(string message)
        {
            return new OperationResultViewModel
            {
                Success = false,
                Message = message
            };
        }
    }
}