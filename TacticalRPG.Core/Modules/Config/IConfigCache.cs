using System;

namespace TacticalRPG.Core.Modules.Config
{
    /// <summary>
    /// 配置缓存接口，用于缓存频繁访问的配置
    /// </summary>
    public interface IConfigCache
    {
        /// <summary>
        /// 获取缓存中的配置
        /// </summary>
        /// <typeparam name="T">配置类型</typeparam>
        /// <param name="moduleId">模块ID</param>
        /// <returns>配置对象，如不存在返回null</returns>
        T GetConfig<T>(string moduleId) where T : class, IConfig;

        /// <summary>
        /// 将配置添加到缓存
        /// </summary>
        /// <typeparam name="T">配置类型</typeparam>
        /// <param name="moduleId">模块ID</param>
        /// <param name="config">配置对象</param>
        /// <param name="expirationTime">缓存过期时间，如为null则永久缓存</param>
        void SetConfig<T>(string moduleId, T config, TimeSpan? expirationTime = null) where T : class, IConfig;

        /// <summary>
        /// 从缓存中移除配置
        /// </summary>
        /// <param name="moduleId">模块ID</param>
        /// <returns>是否成功移除</returns>
        bool RemoveConfig(string moduleId);

        /// <summary>
        /// 清空所有缓存
        /// </summary>
        void ClearCache();

        /// <summary>
        /// 获取缓存统计信息
        /// </summary>
        /// <returns>缓存统计信息</returns>
        (int CacheCount, int HitCount, int MissCount) GetCacheStats();

        /// <summary>
        /// 重置缓存统计信息
        /// </summary>
        void ResetCacheStats();
    }
}