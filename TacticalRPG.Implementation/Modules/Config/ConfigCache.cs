using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Extensions.Logging;
using TacticalRPG.Core.Modules.Config;

namespace TacticalRPG.Implementation.Modules.Config
{
    /// <summary>
    /// 配置缓存实现类，使用内存缓存频繁访问的配置
    /// </summary>
    public class ConfigCache : IConfigCache
    {
        private class CacheEntry
        {
            public IConfig Config { get; set; }
            public DateTime? ExpirationTime { get; set; }
            public bool IsExpired => ExpirationTime.HasValue && DateTime.Now > ExpirationTime.Value;
        }

        private readonly Dictionary<string, CacheEntry> _cache = new Dictionary<string, CacheEntry>();
        private readonly object _cacheLock = new object();
        private readonly ILogger<ConfigCache> _logger;
        private readonly Timer _cleanupTimer;

        // 缓存统计信息
        private int _hitCount = 0;
        private int _missCount = 0;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="logger">日志器</param>
        public ConfigCache(ILogger<ConfigCache> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            // 创建定期清理过期缓存的定时器，每分钟执行一次
            _cleanupTimer = new Timer(CleanupExpiredEntries, null, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));

            _logger.LogInformation("配置缓存系统已初始化");
        }

        /// <summary>
        /// 获取缓存中的配置
        /// </summary>
        /// <typeparam name="T">配置类型</typeparam>
        /// <param name="moduleId">模块ID</param>
        /// <returns>配置对象，如不存在返回null</returns>
        public T GetConfig<T>(string moduleId) where T : class, IConfig
        {
            if (string.IsNullOrEmpty(moduleId))
                return null;

            lock (_cacheLock)
            {
                if (_cache.TryGetValue(moduleId, out var entry))
                {
                    // 检查是否过期
                    if (entry.IsExpired)
                    {
                        _cache.Remove(moduleId);
                        Interlocked.Increment(ref _missCount);
                        _logger.LogDebug($"缓存项已过期: {moduleId}");
                        return null;
                    }

                    // 类型检查
                    if (entry.Config is T typedConfig)
                    {
                        Interlocked.Increment(ref _hitCount);
                        _logger.LogDebug($"缓存命中: {moduleId}");
                        return typedConfig;
                    }
                    else
                    {
                        Interlocked.Increment(ref _missCount);
                        _logger.LogWarning($"缓存类型不匹配. 期望: {typeof(T).Name}, 实际: {entry.Config.GetType().Name}");
                        return null;
                    }
                }

                Interlocked.Increment(ref _missCount);
                _logger.LogDebug($"缓存未命中: {moduleId}");
                return null;
            }
        }

        /// <summary>
        /// 将配置添加到缓存
        /// </summary>
        /// <typeparam name="T">配置类型</typeparam>
        /// <param name="moduleId">模块ID</param>
        /// <param name="config">配置对象</param>
        /// <param name="expirationTime">缓存过期时间，如为null则永久缓存</param>
        public void SetConfig<T>(string moduleId, T config, TimeSpan? expirationTime = null) where T : class, IConfig
        {
            if (string.IsNullOrEmpty(moduleId) || config == null)
                return;

            DateTime? expiration = expirationTime.HasValue ? DateTime.Now.Add(expirationTime.Value) : null;

            lock (_cacheLock)
            {
                _cache[moduleId] = new CacheEntry
                {
                    Config = config,
                    ExpirationTime = expiration
                };

                _logger.LogDebug($"缓存已添加: {moduleId}, 过期时间: {(expiration.HasValue ? expiration.Value.ToString() : "永不过期")}");
            }
        }

        /// <summary>
        /// 从缓存中移除配置
        /// </summary>
        /// <param name="moduleId">模块ID</param>
        /// <returns>是否成功移除</returns>
        public bool RemoveConfig(string moduleId)
        {
            if (string.IsNullOrEmpty(moduleId))
                return false;

            lock (_cacheLock)
            {
                bool result = _cache.Remove(moduleId);
                if (result)
                {
                    _logger.LogDebug($"缓存已移除: {moduleId}");
                }
                return result;
            }
        }

        /// <summary>
        /// 清空所有缓存
        /// </summary>
        public void ClearCache()
        {
            lock (_cacheLock)
            {
                int count = _cache.Count;
                _cache.Clear();
                _logger.LogInformation($"已清空所有缓存，共 {count} 项");
            }
        }

        /// <summary>
        /// 获取缓存统计信息
        /// </summary>
        /// <returns>缓存统计信息</returns>
        public (int CacheCount, int HitCount, int MissCount) GetCacheStats()
        {
            lock (_cacheLock)
            {
                return (_cache.Count, _hitCount, _missCount);
            }
        }

        /// <summary>
        /// 重置缓存统计信息
        /// </summary>
        public void ResetCacheStats()
        {
            Interlocked.Exchange(ref _hitCount, 0);
            Interlocked.Exchange(ref _missCount, 0);
            _logger.LogInformation("缓存统计信息已重置");
        }

        /// <summary>
        /// 清理过期的缓存项
        /// </summary>
        private void CleanupExpiredEntries(object state)
        {
            int cleanedCount = 0;

            lock (_cacheLock)
            {
                var keysToRemove = new List<string>();

                foreach (var kvp in _cache)
                {
                    if (kvp.Value.IsExpired)
                    {
                        keysToRemove.Add(kvp.Key);
                    }
                }

                foreach (var key in keysToRemove)
                {
                    _cache.Remove(key);
                    cleanedCount++;
                }
            }

            if (cleanedCount > 0)
            {
                _logger.LogInformation($"已清理 {cleanedCount} 个过期的缓存项");
            }
        }

        /// <summary>
        /// 析构函数，释放资源
        /// </summary>
        ~ConfigCache()
        {
            _cleanupTimer?.Dispose();
        }
    }
}