namespace BoardGameFontier.Services
{
    /// <summary>
    /// 快取服務介面
    /// 提供統一的快取操作
    /// </summary>
    public interface ICacheService
    {
        /// <summary>
        /// 取得快取資料
        /// </summary>
        /// <typeparam name="T">資料類型</typeparam>
        /// <param name="key">快取鍵值</param>
        /// <returns>快取資料，不存在時回傳 null</returns>
        Task<T?> GetAsync<T>(string key) where T : class;

        /// <summary>
        /// 設定快取資料
        /// </summary>
        /// <typeparam name="T">資料類型</typeparam>
        /// <param name="key">快取鍵值</param>
        /// <param name="value">要快取的資料</param>
        /// <param name="expiry">過期時間，null 表示使用預設過期時間</param>
        Task SetAsync<T>(string key, T value, TimeSpan? expiry = null) where T : class;

        /// <summary>
        /// 移除快取資料
        /// </summary>
        /// <param name="key">快取鍵值</param>
        Task RemoveAsync(string key);

        /// <summary>
        /// 移除指定模式的快取資料
        /// </summary>
        /// <param name="pattern">模式，支援萬用字元 *</param>
        Task RemoveByPatternAsync(string pattern);

        /// <summary>
        /// 檢查快取是否存在
        /// </summary>
        /// <param name="key">快取鍵值</param>
        /// <returns>存在回傳 true</returns>
        Task<bool> ExistsAsync(string key);
    }
}