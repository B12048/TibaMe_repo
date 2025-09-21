using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace BoardGameFontier.Services
{
    /// <summary>
    /// 記憶體快取服務實作
    /// 使用 IMemoryCache 提供基本的快取功能
    /// </summary>
    public class MemoryCacheService(IMemoryCache memoryCache, ILogger<MemoryCacheService> logger) : ICacheService
    {
        private readonly IMemoryCache _memoryCache = memoryCache;
        private readonly ILogger<MemoryCacheService> _logger = logger;
        private readonly TimeSpan _defaultExpiry = TimeSpan.FromMinutes(30); // 預設 30 分鐘過期
        private readonly HashSet<string> _cacheKeys = []; // 追蹤所有快取鍵值

        /// <summary>
        /// 取得快取資料
        /// </summary>
        public Task<T?> GetAsync<T>(string key) where T : class
        {
            try
            {
                if (_memoryCache.TryGetValue(key, out var cachedValue))
                {
                    if (cachedValue is string jsonString)
                    {
                        var result = JsonSerializer.Deserialize<T>(jsonString);
                        _logger.LogDebug("快取命中: {CacheKey}", key);
                        return Task.FromResult(result);
                    }
                    
                    if (cachedValue is T directValue)
                    {
                        _logger.LogDebug("快取命中: {CacheKey}", key);
                        return Task.FromResult<T?>(directValue);
                    }
                }

                _logger.LogDebug("快取未命中: {CacheKey}", key);
                return Task.FromResult<T?>(null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得快取資料時發生錯誤: {CacheKey}", key);
                return Task.FromResult<T?>(null);
            }
        }

        /// <summary>
        /// 設定快取資料
        /// </summary>
        public Task SetAsync<T>(string key, T value, TimeSpan? expiry = null) where T : class
        {
            try
            {
                var expiryTime = expiry ?? _defaultExpiry;
                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = expiryTime,
                    SlidingExpiration = TimeSpan.FromMinutes(5), // 5 分鐘滑動過期
                    Priority = CacheItemPriority.Normal
                };

                // 序列化為 JSON 以保持一致性
                var jsonString = JsonSerializer.Serialize(value);
                
                _memoryCache.Set(key, jsonString, cacheOptions);
                
                // 追蹤快取鍵值
                lock (_cacheKeys)
                {
                    _cacheKeys.Add(key);
                }

                _logger.LogDebug("快取設定成功: {CacheKey}, 過期時間: {Expiry}", key, expiryTime);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "設定快取資料時發生錯誤: {CacheKey}", key);
                return Task.CompletedTask;
            }
        }

        /// <summary>
        /// 移除快取資料
        /// </summary>
        public Task RemoveAsync(string key)
        {
            try
            {
                _memoryCache.Remove(key);
                
                // 從追蹤清單中移除
                lock (_cacheKeys)
                {
                    _cacheKeys.Remove(key);
                }

                _logger.LogDebug("快取移除成功: {CacheKey}", key);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "移除快取資料時發生錯誤: {CacheKey}", key);
                return Task.CompletedTask;
            }
        }

        /// <summary>
        /// 移除指定模式的快取資料
        /// </summary>
        public async Task RemoveByPatternAsync(string pattern)
        {
            try
            {
                var keysToRemove = new List<string>();
                
                lock (_cacheKeys)
                {
                    var regex = new System.Text.RegularExpressions.Regex(
                        pattern.Replace("*", ".*"), 
                        System.Text.RegularExpressions.RegexOptions.IgnoreCase
                    );
                    
                    keysToRemove.AddRange(_cacheKeys.Where(key => regex.IsMatch(key)));
                }

                // 這裡保留 async/await，因為需要循環呼叫其他異步方法
                foreach (var key in keysToRemove)
                {
                    await RemoveAsync(key);
                }

                _logger.LogDebug("移除模式快取成功: {Pattern}, 移除數量: {Count}", pattern, keysToRemove.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "移除模式快取時發生錯誤: {Pattern}", pattern);
            }
        }

        /// <summary>
        /// 檢查快取是否存在
        /// </summary>
        public Task<bool> ExistsAsync(string key)
        {
            try
            {
                var exists = _memoryCache.TryGetValue(key, out _);
                return Task.FromResult(exists);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "檢查快取存在時發生錯誤: {CacheKey}", key);
                return Task.FromResult(false);
            }
        }
    }
}