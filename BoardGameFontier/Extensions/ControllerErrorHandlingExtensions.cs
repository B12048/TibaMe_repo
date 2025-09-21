using BoardGameFontier.Models.Common;
using Microsoft.AspNetCore.Mvc;

namespace BoardGameFontier.Extensions
{
    /// <summary>
    /// Controller 統一錯誤處理擴充方法
    /// 提供標準化的錯誤回應格式
    /// </summary>
    public static class ControllerErrorHandlingExtensions
    {
        /// <summary>
        /// 建立統一的成功回應
        /// </summary>
        /// <typeparam name="T">資料類型</typeparam>
        /// <param name="controller">控制器實例</param>
        /// <param name="data">回應資料</param>
        /// <param name="message">成功訊息</param>
        /// <returns>標準化成功回應</returns>
        public static IActionResult ApiSuccess<T>(this ControllerBase controller, T data, string message = "操作成功")
        {
            return controller.Ok(ApiResponse<T>.Ok(data, message));
        }

        /// <summary>
        /// 建立統一的成功回應（無資料）
        /// </summary>
        /// <param name="controller">控制器實例</param>
        /// <param name="message">成功訊息</param>
        /// <returns>標準化成功回應</returns>
        public static IActionResult ApiSuccess(this ControllerBase controller, string message = "操作成功")
        {
            return controller.Ok(ApiResponse<object>.Ok(message));
        }

        /// <summary>
        /// 建立統一的錯誤回應
        /// </summary>
        /// <typeparam name="T">資料類型</typeparam>
        /// <param name="controller">控制器實例</param>
        /// <param name="message">錯誤訊息</param>
        /// <param name="statusCode">HTTP 狀態碼</param>
        /// <returns>標準化錯誤回應</returns>
        public static IActionResult ApiError<T>(this ControllerBase controller, string message, int statusCode = 400)
        {
            return controller.StatusCode(statusCode, ApiResponse<T>.Error(message, statusCode));
        }

        /// <summary>
        /// 建立統一的錯誤回應（無資料）
        /// </summary>
        /// <param name="controller">控制器實例</param>
        /// <param name="message">錯誤訊息</param>
        /// <param name="statusCode">HTTP 狀態碼</param>
        /// <returns>標準化錯誤回應</returns>
        public static IActionResult ApiError(this ControllerBase controller, string message, int statusCode = 400)
        {
            return controller.StatusCode(statusCode, ApiResponse<object>.Error(message, statusCode));
        }

        /// <summary>
        /// 建立統一的未授權回應
        /// </summary>
        /// <typeparam name="T">資料類型</typeparam>
        /// <param name="controller">控制器實例</param>
        /// <param name="message">錯誤訊息</param>
        /// <returns>標準化未授權回應</returns>
        public static IActionResult ApiUnauthorized<T>(this ControllerBase controller, string message = "請先登入")
        {
            return controller.Unauthorized(ApiResponse<T>.Unauthorized(message));
        }

        /// <summary>
        /// 建立統一的禁止存取回應
        /// </summary>
        /// <typeparam name="T">資料類型</typeparam>
        /// <param name="controller">控制器實例</param>
        /// <param name="message">錯誤訊息</param>
        /// <returns>標準化禁止存取回應</returns>
        public static IActionResult ApiForbidden<T>(this ControllerBase controller, string message = "沒有權限執行此操作")
        {
            return controller.StatusCode(403, ApiResponse<T>.Forbidden(message));
        }

        /// <summary>
        /// 建立統一的資源不存在回應
        /// </summary>
        /// <typeparam name="T">資料類型</typeparam>
        /// <param name="controller">控制器實例</param>
        /// <param name="message">錯誤訊息</param>
        /// <returns>標準化資源不存在回應</returns>
        public static IActionResult ApiNotFound<T>(this ControllerBase controller, string message = "找不到指定的資源")
        {
            return controller.NotFound(ApiResponse<T>.NotFound(message));
        }

        /// <summary>
        /// 建立統一的驗證錯誤回應
        /// </summary>
        /// <typeparam name="T">資料類型</typeparam>
        /// <param name="controller">控制器實例</param>
        /// <param name="errors">錯誤列表</param>
        /// <param name="message">主要錯誤訊息</param>
        /// <returns>標準化驗證錯誤回應</returns>
        public static IActionResult ApiValidationError<T>(this ControllerBase controller, List<string> errors, string message = "資料驗證失敗")
        {
            return controller.StatusCode(422, ApiResponse<T>.ValidationError(errors, message));
        }

        /// <summary>
        /// 建立統一的伺服器錯誤回應
        /// </summary>
        /// <typeparam name="T">資料類型</typeparam>
        /// <param name="controller">控制器實例</param>
        /// <param name="message">錯誤訊息</param>
        /// <returns>標準化伺服器錯誤回應</returns>
        public static IActionResult ApiInternalServerError<T>(this ControllerBase controller, string message = "伺服器內部錯誤")
        {
            return controller.StatusCode(500, ApiResponse<T>.InternalServerError(message));
        }

        /// <summary>
        /// 建立統一的模型驗證錯誤回應
        /// </summary>
        /// <typeparam name="T">資料類型</typeparam>
        /// <param name="controller">控制器實例</param>
        /// <param name="message">主要錯誤訊息</param>
        /// <returns>標準化模型驗證錯誤回應</returns>
        public static IActionResult ApiModelStateError<T>(this ControllerBase controller, string message = "請求參數無效")
        {
            var errors = controller.ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .SelectMany(x => x.Value.Errors.Select(e => $"{x.Key}: {e.ErrorMessage}"))
                .ToList();

            return controller.ApiValidationError<T>(errors, message);
        }

        /// <summary>
        /// 處理異常並建立統一的錯誤回應
        /// </summary>
        /// <typeparam name="T">資料類型</typeparam>
        /// <param name="controller">控制器實例</param>
        /// <param name="ex">異常</param>
        /// <param name="logger">Logger 實例</param>
        /// <param name="context">操作上下文</param>
        /// <param name="customMessage">自定義錯誤訊息</param>
        /// <returns>標準化錯誤回應</returns>
        public static IActionResult ApiHandleException<T>(
            this ControllerBase controller, 
            Exception ex, 
            ILogger logger, 
            string context, 
            string? customMessage = null)
        {
            logger.LogError(ex, "{Context} 時發生錯誤", context);

            var message = customMessage ?? "操作失敗，請稍後再試";
            return controller.ApiInternalServerError<T>(message);
        }
    }

    /// <summary>
    /// 常用的 API 回應擴充方法（無型別參數）
    /// </summary>
    public static class ApiResponseExtensions
    {
        /// <summary>
        /// 建立統一的成功回應（object 型別）
        /// </summary>
        public static IActionResult ApiSuccess(this ControllerBase controller, object? data, string message = "操作成功")
        {
            return controller.Ok(ApiResponse<object>.Ok(data, message));
        }

        /// <summary>
        /// 建立統一的錯誤回應（object 型別）
        /// </summary>
        public static IActionResult ApiError(this ControllerBase controller, string message, int statusCode = 400)
        {
            return controller.StatusCode(statusCode, ApiResponse<object>.Error(message, statusCode));
        }

        /// <summary>
        /// 建立統一的未授權回應（object 型別）
        /// </summary>
        public static IActionResult ApiUnauthorized(this ControllerBase controller, string message = "請先登入")
        {
            return controller.Unauthorized(ApiResponse<object>.Unauthorized(message));
        }

        /// <summary>
        /// 建立統一的資源不存在回應（object 型別）
        /// </summary>
        public static IActionResult ApiNotFound(this ControllerBase controller, string message = "找不到指定的資源")
        {
            return controller.NotFound(ApiResponse<object>.NotFound(message));
        }

        /// <summary>
        /// 建立統一的伺服器錯誤回應（object 型別）
        /// </summary>
        public static IActionResult ApiInternalServerError(this ControllerBase controller, string message = "伺服器內部錯誤")
        {
            return controller.StatusCode(500, ApiResponse<object>.InternalServerError(message));
        }
    }
}