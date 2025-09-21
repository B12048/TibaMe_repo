using System.Text.RegularExpressions;
using System.Web;
using BoardGameFontier.Models;

namespace BoardGameFontier.Services
{
    /// <summary>
    /// 內容驗證和過濾服務實作
    /// 提供完整的輸入驗證、XSS 防護和敏感詞過濾功能
    /// </summary>
    public class ContentValidationService : IContentValidationService
    {
        private readonly ILogger<ContentValidationService> _logger;
        private readonly HashSet<string> _sensitiveWords;
        private readonly HashSet<string> _allowedHtmlTags;

        public ContentValidationService(ILogger<ContentValidationService> logger)
        {
            _logger = logger;
            
            // 初始化敏感詞清單（可考慮從資料庫或設定檔載入）
            _sensitiveWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                // 政治敏感詞
                "政治敏感詞1", "政治敏感詞2",
                
                // 髒話和不當言論
                "垃圾", "廢物", "白癡", "智障",
                
                // 歧視性語言
                "歧視詞1", "歧視詞2",
                
                // 可以根據需要擴展
            };

            // 允許的 HTML 標籤（用於富文本編輯）
            _allowedHtmlTags = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "p", "br", "strong", "b", "em", "i", "u", "span", "div",
                "ul", "ol", "li", "h1", "h2", "h3", "h4", "h5", "h6",
                "blockquote", "code", "pre"
            };
        }

        /// <summary>
        /// 清理 HTML 內容，防止 XSS 攻擊
        /// </summary>
        public string SanitizeHtml(string htmlContent)
        {
            if (string.IsNullOrWhiteSpace(htmlContent))
                return string.Empty;

            try
            {
                // 基本的 HTML 編碼
                var sanitized = HttpUtility.HtmlEncode(htmlContent);

                // 移除潛在的 JavaScript 代碼
                sanitized = RemoveJavaScript(sanitized);

                // 移除危險的 HTML 屬性
                sanitized = RemoveDangerousAttributes(sanitized);

                _logger.LogDebug("HTML 內容已清理：原始長度 {OriginalLength}，清理後長度 {CleanedLength}",
                    htmlContent.Length, sanitized.Length);

                return sanitized;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "清理 HTML 內容時發生錯誤");
                // 如果清理失敗，回傳純文字版本
                return HttpUtility.HtmlEncode(StripHtml(htmlContent));
            }
        }

        /// <summary>
        /// 驗證貼文內容
        /// </summary>
        public ServiceResult<string> ValidatePostContent(string content)
        {
            try
            {
                // 基本驗證
                var basicValidation = ValidateUserInput(content, 5000, allowHtml: true);
                if (!basicValidation.IsSuccess)
                    return basicValidation;

                var cleanedContent = basicValidation.Data!;

                // 檢查敏感詞
                var sensitiveCheck = CheckSensitiveWords(cleanedContent);
                if (!sensitiveCheck.IsSuccess)
                    return ServiceResult<string>.Failure("敏感詞檢查失敗");

                if (sensitiveCheck.Data!.HasSensitiveWords)
                {
                    _logger.LogWarning("貼文內容包含敏感詞：{SensitiveWords}", 
                        string.Join(", ", sensitiveCheck.Data.SensitiveWords));
                    return ServiceResult<string>.Failure(
                        $"內容包含不當詞彙：{string.Join(", ", sensitiveCheck.Data.SensitiveWords)}");
                }

                // 檢查內容長度（至少 10 個字元）
                var plainText = StripHtml(cleanedContent);
                if (plainText.Length < 10)
                {
                    return ServiceResult<string>.Failure("貼文內容過短，至少需要 10 個字元");
                }

                return ServiceResult<string>.Success(cleanedContent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "驗證貼文內容時發生錯誤");
                return ServiceResult<string>.Failure("內容驗證失敗");
            }
        }

        /// <summary>
        /// 驗證留言內容
        /// </summary>
        public ServiceResult<string> ValidateCommentContent(string content)
        {
            try
            {
                // 基本驗證
                var basicValidation = ValidateUserInput(content, 1000, allowHtml: false);
                if (!basicValidation.IsSuccess)
                    return basicValidation;

                var cleanedContent = basicValidation.Data!;

                // 檢查敏感詞
                var sensitiveCheck = CheckSensitiveWords(cleanedContent);
                if (!sensitiveCheck.IsSuccess)
                    return ServiceResult<string>.Failure("敏感詞檢查失敗");

                if (sensitiveCheck.Data!.HasSensitiveWords)
                {
                    _logger.LogWarning("留言內容包含敏感詞：{SensitiveWords}", 
                        string.Join(", ", sensitiveCheck.Data.SensitiveWords));
                    return ServiceResult<string>.Failure(
                        $"內容包含不當詞彙：{string.Join(", ", sensitiveCheck.Data.SensitiveWords)}");
                }

                // 檢查內容長度（至少 1 個字元）
                if (cleanedContent.Trim().Length < 1)
                {
                    return ServiceResult<string>.Failure("留言內容不能為空");
                }

                return ServiceResult<string>.Success(cleanedContent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "驗證留言內容時發生錯誤");
                return ServiceResult<string>.Failure("內容驗證失敗");
            }
        }

        /// <summary>
        /// 檢查內容是否包含敏感詞彙
        /// </summary>
        public ServiceResult<(bool HasSensitiveWords, List<string> SensitiveWords)> CheckSensitiveWords(string content)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(content))
                    return ServiceResult<(bool, List<string>)>.Success((false, new List<string>()));

                var foundSensitiveWords = new List<string>();
                var contentLower = content.ToLowerInvariant();

                foreach (var sensitiveWord in _sensitiveWords)
                {
                    if (contentLower.Contains(sensitiveWord.ToLowerInvariant()))
                    {
                        foundSensitiveWords.Add(sensitiveWord);
                    }
                }

                var hasSensitiveWords = foundSensitiveWords.Count > 0;
                
                if (hasSensitiveWords)
                {
                    _logger.LogInformation("發現敏感詞彙：{Count} 個", foundSensitiveWords.Count);
                }

                return ServiceResult<(bool, List<string>)>.Success((hasSensitiveWords, foundSensitiveWords));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "檢查敏感詞時發生錯誤");
                return ServiceResult<(bool, List<string>)>.Failure("敏感詞檢查失敗");
            }
        }

        /// <summary>
        /// 驗證使用者輸入的通用方法
        /// </summary>
        public ServiceResult<string> ValidateUserInput(string input, int maxLength, bool allowHtml = false)
        {
            try
            {
                // 空值檢查
                if (input == null)
                    return ServiceResult<string>.Failure("輸入不能為 null");

                // 長度檢查
                if (input.Length > maxLength)
                    return ServiceResult<string>.Failure($"內容過長，最多允許 {maxLength} 個字元");

                // HTML 處理
                string cleanedInput;
                if (allowHtml)
                {
                    cleanedInput = SanitizeHtml(input);
                }
                else
                {
                    cleanedInput = HttpUtility.HtmlEncode(input);
                }

                // 移除多餘的空白字元
                cleanedInput = NormalizeWhitespace(cleanedInput);

                return ServiceResult<string>.Success(cleanedInput);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "驗證使用者輸入時發生錯誤");
                return ServiceResult<string>.Failure("輸入驗證失敗");
            }
        }

        #region 私有輔助方法

        /// <summary>
        /// 移除 JavaScript 相關內容
        /// </summary>
        private string RemoveJavaScript(string content)
        {
            var patterns = new[]
            {
                @"<script\b[^<]*(?:(?!<\/script>)<[^<]*)*<\/script>",
                @"javascript:",
                @"on\w+\s*=",
                @"expression\s*\(",
                @"vbscript:",
                @"data:text/html"
            };

            foreach (var pattern in patterns)
            {
                content = Regex.Replace(content, pattern, "", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            }

            return content;
        }

        /// <summary>
        /// 移除危險的 HTML 屬性
        /// </summary>
        private string RemoveDangerousAttributes(string content)
        {
            var dangerousAttributes = new[]
            {
                "onclick", "onload", "onerror", "onmouseover", "onmouseout",
                "onchange", "onblur", "onfocus", "onsubmit", "onreset",
                "style", "class", "id"
            };

            foreach (var attr in dangerousAttributes)
            {
                var pattern = $@"\s{attr}\s*=\s*[""'][^""']*[""']";
                content = Regex.Replace(content, pattern, "", RegexOptions.IgnoreCase);
            }

            return content;
        }

        /// <summary>
        /// 移除所有 HTML 標籤，保留純文字
        /// </summary>
        private string StripHtml(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return string.Empty;

            return Regex.Replace(content, @"<[^>]+>", "").Trim();
        }

        /// <summary>
        /// 正規化空白字元
        /// </summary>
        private string NormalizeWhitespace(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return string.Empty;

            // 移除開頭和結尾的空白
            content = content.Trim();

            // 將多個連續空白替換為單一空白
            content = Regex.Replace(content, @"\s+", " ");

            return content;
        }

        #endregion
    }
}