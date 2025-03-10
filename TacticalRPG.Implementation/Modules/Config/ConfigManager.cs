using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using TacticalRPG.Core.Modules.Config;
using Microsoft.Extensions.Logging;

namespace TacticalRPG.Implementation.Modules.Config
{
    /// <summary>
    /// 配置管理器实现
    /// </summary>
    public class ConfigManager : IConfigManager
    {
        private readonly Dictionary<string, IConfig> _configs = new Dictionary<string, IConfig>();
        private readonly object _lockObj = new object();
        private readonly ILogger<ConfigManager> _logger;
        private readonly IConfigCache _configCache;

        /// <summary>
        /// 配置变更事件
        /// </summary>
        public event IConfigManager.ConfigChangedEventHandler ConfigChanged;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="logger">日志记录器</param>
        /// <param name="configCache">配置缓存</param>
        public ConfigManager(ILogger<ConfigManager> logger, IConfigCache configCache = null)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configCache = configCache; // 允许为null，表示不使用缓存

            if (_configCache != null)
            {
                _logger.LogInformation("配置管理器已启用缓存");
            }
        }

        /// <summary>
        /// 注册配置
        /// </summary>
        /// <param name="config">配置对象</param>
        /// <returns>是否成功注册</returns>
        public bool RegisterConfig(IConfig config)
        {
            return RegisterConfig(config, false);
        }

        /// <summary>
        /// 注册配置，并进行验证
        /// </summary>
        /// <param name="config">配置对象</param>
        /// <param name="skipValidation">是否跳过验证</param>
        /// <returns>是否成功注册</returns>
        public bool RegisterConfig(IConfig config, bool skipValidation = false)
        {
            if (config == null)
                return false;

            string moduleId = config.ModuleId;
            if (string.IsNullOrEmpty(moduleId))
                return false;

            // 验证配置
            if (!skipValidation)
            {
                var (isValid, errorMessage) = config.Validate();
                if (!isValid)
                {
                    _logger.LogError($"配置验证失败: {errorMessage}, 模块ID: {moduleId}");
                    return false;
                }
            }

            lock (_lockObj)
            {
                // 检查是否存在同名配置
                if (_configs.ContainsKey(moduleId))
                {
                    _logger.LogWarning($"配置已存在，将被覆盖. 模块ID: {moduleId}");
                }

                // 注册配置
                _configs[moduleId] = config;
                _logger.LogInformation($"注册配置成功. 模块ID: {moduleId}");

                // 触发配置变更事件
                OnConfigChanged(moduleId, config);

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

            // 如果启用了缓存，首先尝试从缓存获取
            if (_configCache != null)
            {
                var cachedConfig = _configCache.GetConfig<T>(moduleId);
                if (cachedConfig != null)
                {
                    return cachedConfig;
                }
            }

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
                OnConfigChanged(config.ModuleId, config);

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
                    OnConfigChanged(moduleId, loadedConfig);

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
                OnConfigChanged(moduleId, config);

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

        /// <summary>
        /// 验证配置是否有效
        /// </summary>
        /// <param name="moduleId">模块ID</param>
        /// <returns>验证结果，包含是否有效和错误信息</returns>
        public (bool IsValid, string ErrorMessage) ValidateConfig(string moduleId)
        {
            if (string.IsNullOrEmpty(moduleId))
                return (false, "模块ID不能为空");

            lock (_lockObj)
            {
                if (!_configs.TryGetValue(moduleId, out var config))
                    return (false, $"找不到模块 {moduleId} 的配置");

                try
                {
                    var result = config.Validate();
                    if (!result.IsValid)
                    {
                        _logger.LogWarning($"配置验证失败. 模块ID: {moduleId}, 错误: {result.ErrorMessage}");
                    }
                    return result;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"验证配置时发生异常. 模块ID: {moduleId}");
                    return (false, $"验证异常: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// 验证所有配置
        /// </summary>
        /// <returns>验证结果，包含无效配置的模块ID和错误信息</returns>
        public Dictionary<string, string> ValidateAllConfigs()
        {
            var invalidConfigs = new Dictionary<string, string>();

            lock (_lockObj)
            {
                foreach (var kvp in _configs)
                {
                    var moduleId = kvp.Key;
                    var config = kvp.Value;

                    try
                    {
                        var (isValid, errorMessage) = config.Validate();
                        if (!isValid)
                        {
                            invalidConfigs.Add(moduleId, errorMessage);
                            _logger.LogWarning($"配置验证失败. 模块ID: {moduleId}, 错误: {errorMessage}");
                        }
                    }
                    catch (Exception ex)
                    {
                        invalidConfigs.Add(moduleId, $"验证异常: {ex.Message}");
                        _logger.LogError(ex, $"验证配置时发生异常. 模块ID: {moduleId}");
                    }
                }
            }

            if (invalidConfigs.Count > 0)
            {
                _logger.LogWarning($"共有 {invalidConfigs.Count} 个配置验证失败");
            }
            else
            {
                _logger.LogInformation("所有配置验证通过");
            }

            return invalidConfigs;
        }

        /// <summary>
        /// 配置变更事件触发器
        /// </summary>
        /// <param name="moduleId">模块ID</param>
        /// <param name="config">配置对象</param>
        protected virtual void OnConfigChanged(string moduleId, IConfig config)
        {
            ConfigChanged?.Invoke(moduleId, config);

            // 如果启用了缓存，更新缓存
            if (_configCache != null)
            {
                _configCache.RemoveConfig(moduleId);
                _logger.LogDebug($"已从缓存中移除变更的配置: {moduleId}");
            }
        }
    }
}