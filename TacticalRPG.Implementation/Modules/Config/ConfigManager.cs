using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using TacticalRPG.Core.Modules.Config;

namespace TacticalRPG.Implementation.Modules.Config
{
    /// <summary>
    /// 配置管理器实现
    /// </summary>
    public class ConfigManager : IConfigManager
    {
        private readonly Dictionary<string, IConfig> _configs = new Dictionary<string, IConfig>();
        private readonly object _lockObj = new object();

        /// <summary>
        /// 配置变更事件
        /// </summary>
        public event IConfigManager.ConfigChangedEventHandler ConfigChanged;

        /// <summary>
        /// 构造函数
        /// </summary>
        public ConfigManager()
        {
        }

        /// <summary>
        /// 注册配置
        /// </summary>
        /// <param name="config">配置对象</param>
        /// <returns>是否成功注册</returns>
        public bool RegisterConfig(IConfig config)
        {
            if (config == null)
                return false;

            lock (_lockObj)
            {
                if (_configs.ContainsKey(config.ModuleId))
                    return false;

                _configs[config.ModuleId] = config;
                return true;
            }
        }

        /// <summary>
        /// 获取配置
        /// </summary>
        /// <typeparam name="T">配置类型</typeparam>
        /// <param name="moduleId">模块ID</param>
        /// <returns>配置对象，如不存在则返回null</returns>
        public T GetConfig<T>(string moduleId) where T : class, IConfig
        {
            if (string.IsNullOrEmpty(moduleId))
                return null;

            lock (_lockObj)
            {
                if (!_configs.TryGetValue(moduleId, out var config))
                    return null;

                return config as T;
            }
        }

        /// <summary>
        /// 更新配置
        /// </summary>
        /// <param name="config">配置对象</param>
        /// <returns>是否成功更新</returns>
        public bool UpdateConfig(IConfig config)
        {
            if (config == null)
                return false;

            lock (_lockObj)
            {
                if (!_configs.ContainsKey(config.ModuleId))
                    return false;

                // 验证配置
                var validation = config.Validate();
                if (!validation.IsValid)
                {
                    Console.WriteLine($"配置验证失败: {validation.ErrorMessage}");
                    return false;
                }

                _configs[config.ModuleId] = config;

                // 触发配置变更事件
                ConfigChanged?.Invoke(config.ModuleId, config);

                return true;
            }
        }

        /// <summary>
        /// 加载所有配置
        /// </summary>
        /// <param name="configFolderPath">配置文件夹路径</param>
        /// <returns>是否成功加载</returns>
        public bool LoadAllConfigs(string configFolderPath)
        {
            if (string.IsNullOrEmpty(configFolderPath) || !Directory.Exists(configFolderPath))
                return false;

            bool allSuccess = true;

            lock (_lockObj)
            {
                foreach (var moduleId in _configs.Keys.ToList())
                {
                    if (!LoadConfig(moduleId, configFolderPath))
                        allSuccess = false;
                }
            }

            return allSuccess;
        }

        /// <summary>
        /// 保存所有配置
        /// </summary>
        /// <param name="configFolderPath">配置文件夹路径</param>
        /// <returns>是否成功保存</returns>
        public bool SaveAllConfigs(string configFolderPath)
        {
            if (string.IsNullOrEmpty(configFolderPath))
                return false;

            // 确保配置目录存在
            if (!Directory.Exists(configFolderPath))
                Directory.CreateDirectory(configFolderPath);

            bool allSuccess = true;

            lock (_lockObj)
            {
                foreach (var moduleId in _configs.Keys.ToList())
                {
                    if (!SaveConfig(moduleId, configFolderPath))
                        allSuccess = false;
                }
            }

            return allSuccess;
        }

        /// <summary>
        /// 加载指定模块的配置
        /// </summary>
        /// <param name="moduleId">模块ID</param>
        /// <param name="configFolderPath">配置文件夹路径</param>
        /// <returns>是否成功加载</returns>
        public bool LoadConfig(string moduleId, string configFolderPath)
        {
            if (string.IsNullOrEmpty(moduleId) || string.IsNullOrEmpty(configFolderPath))
                return false;

            lock (_lockObj)
            {
                if (!_configs.TryGetValue(moduleId, out var config))
                    return false;

                string configFilePath = Path.Combine(configFolderPath, $"{moduleId}.json");

                if (!File.Exists(configFilePath))
                    return false;

                try
                {
                    string json = File.ReadAllText(configFilePath);

                    // 使用当前配置的类型进行反序列化
                    var loadedConfig = JsonSerializer.Deserialize<IConfig>(json); //json.DeserializeObject(json, config.GetType()) as IConfig;

                    if (loadedConfig == null)
                        return false;

                    // 验证配置
                    var validation = loadedConfig.Validate();
                    if (!validation.IsValid)
                    {
                        Console.WriteLine($"配置验证失败: {validation.ErrorMessage}");
                        return false;
                    }

                    _configs[moduleId] = loadedConfig;

                    // 触发配置变更事件
                    ConfigChanged?.Invoke(moduleId, loadedConfig);

                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"加载配置异常: {ex.Message}");
                    return false;
                }
            }
        }

        /// <summary>
        /// 保存指定模块的配置
        /// </summary>
        /// <param name="moduleId">模块ID</param>
        /// <param name="configFolderPath">配置文件夹路径</param>
        /// <returns>是否成功保存</returns>
        public bool SaveConfig(string moduleId, string configFolderPath)
        {
            if (string.IsNullOrEmpty(moduleId) || string.IsNullOrEmpty(configFolderPath))
                return false;

            lock (_lockObj)
            {
                if (!_configs.TryGetValue(moduleId, out var config))
                    return false;

                // 确保配置目录存在
                if (!Directory.Exists(configFolderPath))
                    Directory.CreateDirectory(configFolderPath);

                string configFilePath = Path.Combine(configFolderPath, $"{moduleId}.json");

                try
                {
                    string json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true }); //JsonConvert.SerializeObject(config, Formatting.Indented);
                    File.WriteAllText(configFilePath, json);
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"保存配置异常: {ex.Message}");
                    return false;
                }
            }
        }

        /// <summary>
        /// 重置指定模块的配置为默认值
        /// </summary>
        /// <param name="moduleId">模块ID</param>
        /// <returns>是否成功重置</returns>
        public bool ResetConfig(string moduleId)
        {
            if (string.IsNullOrEmpty(moduleId))
                return false;

            lock (_lockObj)
            {
                if (!_configs.TryGetValue(moduleId, out var config))
                    return false;

                config.ResetToDefault();

                // 触发配置变更事件
                ConfigChanged?.Invoke(moduleId, config);

                return true;
            }
        }

        /// <summary>
        /// 获取所有已注册的配置模块ID
        /// </summary>
        /// <returns>模块ID列表</returns>
        public IReadOnlyList<string> GetAllConfigModuleIds()
        {
            lock (_lockObj)
            {
                return _configs.Keys.ToList();
            }
        }
    }
}