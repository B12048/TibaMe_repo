using Microsoft.AspNetCore.Mvc;
using BoardGameFontier.Models.Common;

namespace BoardGameFontier.Models
{
    /// <summary>
    /// 標準化錯誤處理基礎類別
    /// 提供統一的錯誤處理機制
    /// </summary>
    public static class ErrorHandler
    {
        /// <summary>
        /// 處理 API 錯誤並回傳統一格式
        /// </summary>
        /// <typeparam name="T">資料類型</typeparam>
        /// <param name="ex">異常物件</param>
        /// <param name="logger">日誌記錄器</param>
        /// <param name="operation">操作名稱</param>
        /// <param name="context">錯誤上下文資訊</param>
        /// <returns>標準錯誤回應</returns>
        public static ObjectResult HandleApiError<T>(
            Exception ex, 
            ILogger logger, 
            string operation, 
            object? context = null)
        {
            var errorId = Guid.NewGuid().ToString("N")[..8];
            var errorMessage = GetUserFriendlyMessage(ex);
            
            // 記錄詳細錯誤到日誌
            logger.LogError(ex, 
                "API 錯誤 [ID: {ErrorId}] 操作: {Operation} 上下文: {@Context}", 
                errorId, operation, context);

            // 根據異常類型決定 HTTP 狀態碼
            var statusCode = GetStatusCodeFromException(ex);
            
            var result = ApiResponse<T>.Error($"{errorMessage} (錯誤ID: {errorId})", statusCode);
            
            // 將錯誤ID加入到Metadata中
            result.Metadata = new { ErrorId = errorId };
            
            return new ObjectResult(result);
        }

        /// <summary>
        /// 處理 Controller 錯誤並回傳適當的 ActionResult
        /// </summary>
        /// <param name="ex">異常物件</param>
        /// <param name="logger">日誌記錄器</param>
        /// <param name="operation">操作名稱</param>
        /// <param name="context">錯誤上下文資訊</param>
        /// <returns>錯誤 ActionResult</returns>
        public static IActionResult HandleControllerError(
            Exception ex, 
            ILogger logger, 
            string operation, 
            object? context = null)
        {
            var errorId = Guid.NewGuid().ToString("N")[..8];
            
            // 記錄詳細錯誤到日誌
            logger.LogError(ex, 
                "Controller 錯誤 [ID: {ErrorId}] 操作: {Operation} 上下文: {@Context}", 
                errorId, operation, context);

            // 根據異常類型決定回應方式
            return ex switch
            {
                NotFoundException => new NotFoundResult(),
                UnauthorizedException => new UnauthorizedResult(),
                ForbiddenException => new ForbidResult(),
                ValidationException validationEx => new BadRequestObjectResult(new
                {
                    error = "驗證失敗",
                    details = validationEx.ValidationErrors,
                    errorId
                }),
                _ => new StatusCodeResult(500)
            };
        }

        /// <summary>
        /// 根據異常類型取得使用者友善的錯誤訊息
        /// </summary>
        public static string GetUserFriendlyMessage(Exception ex)
        {
            return ex switch
            {
                NotFoundException => "找不到指定的資源",
                UnauthorizedException => "請先登入",
                ForbiddenException => "您沒有權限執行此操作",
                ValidationException => "輸入的資料不正確",
                TimeoutException => "操作超時，請稍後再試",
                InvalidOperationException => "操作無效，請檢查當前狀態",
                _ => "系統暫時發生錯誤，請稍後再試"
            };
        }

        /// <summary>
        /// 根據異常類型取得對應的 HTTP 狀態碼
        /// </summary>
        private static int GetStatusCodeFromException(Exception ex)
        {
            return ex switch
            {
                NotFoundException => 404,
                UnauthorizedException => 401,
                ForbiddenException => 403,
                ValidationException => 400,
                TimeoutException => 408,
                InvalidOperationException => 409,
                _ => 500
            };
        }
    }

    /// <summary>
    /// 資源未找到異常
    /// </summary>
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) { }
        public NotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }

    /// <summary>
    /// 未授權異常
    /// </summary>
    public class UnauthorizedException : Exception
    {
        public UnauthorizedException(string message) : base(message) { }
        public UnauthorizedException(string message, Exception innerException) : base(message, innerException) { }
    }

    /// <summary>
    /// 禁止存取異常
    /// </summary>
    public class ForbiddenException : Exception
    {
        public ForbiddenException(string message) : base(message) { }
        public ForbiddenException(string message, Exception innerException) : base(message, innerException) { }
    }

    /// <summary>
    /// 驗證異常
    /// </summary>
    public class ValidationException : Exception
    {
        public Dictionary<string, string[]> ValidationErrors { get; }

        public ValidationException(string message) : base(message)
        {
            ValidationErrors = new Dictionary<string, string[]>();
        }

        public ValidationException(string message, Dictionary<string, string[]> validationErrors) : base(message)
        {
            ValidationErrors = validationErrors;
        }
    }

    /// <summary>
    /// 業務邏輯異常
    /// </summary>
    public class BusinessLogicException : Exception
    {
        public string ErrorCode { get; }

        public BusinessLogicException(string errorCode, string message) : base(message)
        {
            ErrorCode = errorCode;
        }

        public BusinessLogicException(string errorCode, string message, Exception innerException) 
            : base(message, innerException)
        {
            ErrorCode = errorCode;
        }
    }

    /// <summary>
    /// 結果包裝器，用於統一處理成功與失敗的結果
    /// </summary>
    /// <typeparam name="T">結果資料類型</typeparam>
    public class ServiceResult<T>
    {
        public bool IsSuccess { get; set; }
        public T? Data { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public string ErrorCode { get; set; } = string.Empty;
        public Exception? Exception { get; set; }

        /// <summary>
        /// 建立成功結果
        /// </summary>
        public static ServiceResult<T> Success(T data)
        {
            return new ServiceResult<T>
            {
                IsSuccess = true,
                Data = data
            };
        }

        /// <summary>
        /// 建立失敗結果
        /// </summary>
        public static ServiceResult<T> Failure(string errorMessage, string errorCode = "", Exception? exception = null)
        {
            return new ServiceResult<T>
            {
                IsSuccess = false,
                ErrorMessage = errorMessage,
                ErrorCode = errorCode,
                Exception = exception
            };
        }

        /// <summary>
        /// 轉換為 ApiResponse
        /// </summary>
        public ApiResponse<T> ToApiResponse()
        {
            return IsSuccess 
                ? ApiResponse<T>.Ok(Data!, "操作成功")
                : ApiResponse<T>.Error(ErrorMessage);
        }
    }

    /// <summary>
    /// 擴展方法，讓 Service 層更容易使用錯誤處理
    /// </summary>
    public static class ServiceExtensions
    {
        /// <summary>
        /// 安全執行異步操作，自動處理異常
        /// </summary>
        public static async Task<ServiceResult<T>> SafeExecuteAsync<T>(
            this Task<T> task, 
            ILogger logger, 
            string operation)
        {
            try
            {
                var result = await task;
                return ServiceResult<T>.Success(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Service 操作失敗: {Operation}", operation);
                return ServiceResult<T>.Failure(
                    ErrorHandler.GetUserFriendlyMessage(ex), 
                    ex.GetType().Name, 
                    ex);
            }
        }

        /// <summary>
        /// 安全執行同步操作，自動處理異常
        /// </summary>
        public static ServiceResult<T> SafeExecute<T>(
            Func<T> func, 
            ILogger logger, 
            string operation)
        {
            try
            {
                var result = func();
                return ServiceResult<T>.Success(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Service 操作失敗: {Operation}", operation);
                return ServiceResult<T>.Failure(
                    ErrorHandler.GetUserFriendlyMessage(ex), 
                    ex.GetType().Name, 
                    ex);
            }
        }
    }
}