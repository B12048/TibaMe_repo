using BoardGameFontier.Models;

namespace BoardGameFontier.Services
{
    /// <summary>
    /// 內容驗證和過濾服務介面
    /// 提供 XSS 防護、內容過濾、輸入驗證等功能
    /// </summary>
    public interface IContentValidationService
    {
        /// <summary>
        /// 清理 HTML 內容，防止 XSS 攻擊
        /// </summary>
        /// <param name="htmlContent">原始 HTML 內容</param>
        /// <returns>清理後的安全 HTML 內容</returns>
        string SanitizeHtml(string htmlContent);

        /// <summary>
        /// 驗證貼文內容
        /// </summary>
        /// <param name="content">貼文內容</param>
        /// <returns>驗證結果</returns>
        ServiceResult<string> ValidatePostContent(string content);

        /// <summary>
        /// 驗證留言內容
        /// </summary>
        /// <param name="content">留言內容</param>
        /// <returns>驗證結果</returns>
        ServiceResult<string> ValidateCommentContent(string content);

        /// <summary>
        /// 檢查內容是否包含敏感詞彙
        /// </summary>
        /// <param name="content">內容</param>
        /// <returns>檢查結果</returns>
        ServiceResult<(bool HasSensitiveWords, List<string> SensitiveWords)> CheckSensitiveWords(string content);

        /// <summary>
        /// 驗證使用者輸入的通用方法
        /// </summary>
        /// <param name="input">使用者輸入</param>
        /// <param name="maxLength">最大長度</param>
        /// <param name="allowHtml">是否允許 HTML</param>
        /// <returns>驗證結果</returns>
        ServiceResult<string> ValidateUserInput(string input, int maxLength, bool allowHtml = false);
    }
}