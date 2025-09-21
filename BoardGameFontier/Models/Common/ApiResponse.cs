namespace BoardGameFontier.Models.Common
{
    /// <summary>
    /// 統一 API 回應格式
    /// 標準化所有 API 的回傳結構，提供一致的錯誤處理和成功回應
    /// </summary>
    /// <typeparam name="T">回應資料的類型</typeparam>
    public class ApiResponse<T>
    {
        /// <summary>
        /// 操作是否成功
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 回應資料
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        /// 回應訊息（成功或錯誤訊息）
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// 錯誤詳細列表（用於表單驗證錯誤等）
        /// </summary>
        public List<string> Errors { get; set; } = new List<string>();

        /// <summary>
        /// 額外的中繼資料（如分頁資訊、時間戳記等）
        /// </summary>
        public object? Metadata { get; set; }

        /// <summary>
        /// HTTP 狀態碼（可選）
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// 伺服器時間戳記
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.Now;

        /// <summary>
        /// 建構函式
        /// </summary>
        public ApiResponse()
        {
        }

        /// <summary>
        /// 建構函式 - 完整參數
        /// </summary>
        /// <param name="success">是否成功</param>
        /// <param name="data">回應資料</param>
        /// <param name="message">訊息</param>
        /// <param name="statusCode">狀態碼</param>
        public ApiResponse(bool success, T? data, string message, int statusCode = 200)
        {
            Success = success;
            Data = data;
            Message = message;
            StatusCode = statusCode;
        }

        /// <summary>
        /// 建立成功回應
        /// </summary>
        /// <param name="data">回應資料</param>
        /// <param name="message">成功訊息</param>
        /// <returns>成功的 API 回應</returns>
        public static ApiResponse<T> Ok(T data, string message = "操作成功")
        {
            return new ApiResponse<T>
            {
                Success = true,
                Data = data,
                Message = message,
                StatusCode = 200
            };
        }

        /// <summary>
        /// 建立成功回應（無資料）
        /// </summary>
        /// <param name="message">成功訊息</param>
        /// <returns>成功的 API 回應</returns>
        public static ApiResponse<T> Ok(string message = "操作成功")
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message,
                StatusCode = 200
            };
        }

        /// <summary>
        /// 建立錯誤回應
        /// </summary>
        /// <param name="message">錯誤訊息</param>
        /// <param name="statusCode">HTTP 狀態碼</param>
        /// <returns>錯誤的 API 回應</returns>
        public static ApiResponse<T> Error(string message, int statusCode = 400)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                StatusCode = statusCode
            };
        }

        /// <summary>
        /// 建立驗證錯誤回應
        /// </summary>
        /// <param name="errors">錯誤列表</param>
        /// <param name="message">主要錯誤訊息</param>
        /// <returns>驗證錯誤的 API 回應</returns>
        public static ApiResponse<T> ValidationError(List<string> errors, string message = "資料驗證失敗")
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Errors = errors,
                StatusCode = 422
            };
        }

        /// <summary>
        /// 建立未授權回應
        /// </summary>
        /// <param name="message">錯誤訊息</param>
        /// <returns>未授權的 API 回應</returns>
        public static ApiResponse<T> Unauthorized(string message = "未授權存取")
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                StatusCode = 401
            };
        }

        /// <summary>
        /// 建立禁止存取回應
        /// </summary>
        /// <param name="message">錯誤訊息</param>
        /// <returns>禁止存取的 API 回應</returns>
        public static ApiResponse<T> Forbidden(string message = "禁止存取")
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                StatusCode = 403
            };
        }

        /// <summary>
        /// 建立資源不存在回應
        /// </summary>
        /// <param name="message">錯誤訊息</param>
        /// <returns>資源不存在的 API 回應</returns>
        public static ApiResponse<T> NotFound(string message = "資源不存在")
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                StatusCode = 404
            };
        }

        /// <summary>
        /// 建立伺服器錯誤回應
        /// </summary>
        /// <param name="message">錯誤訊息</param>
        /// <returns>伺服器錯誤的 API 回應</returns>
        public static ApiResponse<T> InternalServerError(string message = "伺服器內部錯誤")
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                StatusCode = 500
            };
        }

        /// <summary>
        /// 建立樂觀鎖定衝突回應
        /// </summary>
        /// <param name="message">錯誤訊息</param>
        /// <returns>衝突的 API 回應</returns>
        public static ApiResponse<T> Conflict(string message = "資料已被其他使用者修改，請重新載入後再試")
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                StatusCode = 409
            };
        }
    }

    /// <summary>
    /// 無型別參數的 API 回應（用於不需要回傳資料的操作）
    /// </summary>
    public class ApiResponse : ApiResponse<object>
    {
        /// <summary>
        /// 建立成功回應（無資料）
        /// </summary>
        /// <param name="message">成功訊息</param>
        /// <returns>成功的 API 回應</returns>
        public static new ApiResponse Ok(string message = "操作成功")
        {
            return new ApiResponse
            {
                Success = true,
                Message = message,
                StatusCode = 200
            };
        }

        /// <summary>
        /// 建立錯誤回應
        /// </summary>
        /// <param name="message">錯誤訊息</param>
        /// <param name="statusCode">HTTP 狀態碼</param>
        /// <returns>錯誤的 API 回應</returns>
        public static new ApiResponse Error(string message, int statusCode = 400)
        {
            return new ApiResponse
            {
                Success = false,
                Message = message,
                StatusCode = statusCode
            };
        }
    }
}