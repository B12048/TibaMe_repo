using BoardGameFontier.Models.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BoardGameFontier.Filters
{
    /// <summary>
    /// 統一處理模型驗證錯誤的 Action Filter
    /// 自動將 ModelState 錯誤轉換為 ApiResponse 格式
    /// </summary>
    public class ValidationActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .SelectMany(x => x.Value.Errors.Select(e => 
                        string.IsNullOrEmpty(e.ErrorMessage) ? e.Exception?.Message ?? "未知錯誤" : e.ErrorMessage))
                    .Where(msg => !string.IsNullOrEmpty(msg))
                    .ToList();

                var apiResponse = ApiResponse<object>.ValidationError(errors, "資料驗證失敗");
                
                context.Result = new BadRequestObjectResult(apiResponse);
            }
        }
    }
}