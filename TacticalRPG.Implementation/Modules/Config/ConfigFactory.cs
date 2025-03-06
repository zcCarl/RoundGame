using System;
using System.IO;
using TacticalRPG.Core.Modules.Config;

namespace TacticalRPG.Implementation.Modules.Config
{
    /// <summary>
    /// 配置工厂，用于创建和获取配置管理器
    /// </summary>
    public static class ConfigFactory
    {
        private static readonly object _lockObj = new object();
        private static IConfigManager _instance;

        /// <summary>
        /// 获取配置管理器实例（单例模式）
        /// </summary>
        /// <param name="configFolderPath">配置文件夹路径</param>
        /// <returns>配置管理器实例</returns>
        public static IConfigManager GetConfigManager(string configFolderPath = null)
        {
            if (_instance != null)
                return _instance;

            lock (_lockObj)
            {
                if (_instance != null)
                    return _instance;

                // 创建配置管理器
                _instance = new ConfigManager();

                // 初始化配置
                ConfigInitializer.InitializeAllConfigs(_instance, configFolderPath);

                return _instance;
            }
        }

        /// <summary>
        /// 重新加载所有配置
        /// </summary>
        /// <param name="configFolderPath">配置文件夹路径</param>
        /// <returns>是否成功重新加载</returns>
        public static bool ReloadAllConfigs(string configFolderPath)
        {
            if (_instance == null || string.IsNullOrEmpty(configFolderPath))
                return false;

            return _instance.LoadAllConfigs(configFolderPath);
        }

        /// <summary>
        /// 保存所有配置
        /// </summary>
        /// <param name="configFolderPath">配置文件夹路径</param>
        /// <returns>是否成功保存</returns>
        public static bool SaveAllConfigs(string configFolderPath)
        {
            if (_instance == null || string.IsNullOrEmpty(configFolderPath))
                return false;

            return ConfigInitializer.SaveAllConfigs(_instance, configFolderPath);
        }

        /// <summary>
        /// 获取特定模块的配置
        /// </summary>
        /// <typeparam name="T">配置类型</typeparam>
        /// <param name="moduleId">模块ID</param>
        /// <returns>模块配置</returns>
        public static T GetModuleConfig<T>(string moduleId) where T : class, IConfig
        {
            if (_instance == null || string.IsNullOrEmpty(moduleId))
                return null;

            return _instance.GetConfig<T>(moduleId);
        }

        /// <summary>
        /// 重置特定模块的配置
        /// </summary>
        /// <param name="moduleId">模块ID</param>
        /// <returns>是否成功重置</returns>
        public static bool ResetModuleConfig(string moduleId)
        {
            if (_instance == null || string.IsNullOrEmpty(moduleId))
                return false;

            return _instance.ResetConfig(moduleId);
        }
    }
}