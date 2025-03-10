using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using TacticalRPG.Core.Modules.Config;

namespace TacticalRPG.Implementation.Modules.Config
{
    /// <summary>
    /// 配置工厂实现类，负责创建和初始化各种配置对象
    /// </summary>
    public class ConfigFactory : IConfigFactory
    {
        private readonly IConfigManager _configManager;
        private readonly ILogger<ConfigFactory> _logger;
        private readonly Dictionary<Type, Delegate> _configCreators = new Dictionary<Type, Delegate>();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="configManager">配置管理器</param>
        /// <param name="logger">日志器</param>
        public ConfigFactory(IConfigManager configManager, ILogger<ConfigFactory> logger)
        {
            _configManager = configManager ?? throw new ArgumentNullException(nameof(configManager));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// 创建配置对象
        /// </summary>
        /// <typeparam name="T">配置类型</typeparam>
        /// <param name="moduleId">模块ID</param>
        /// <returns>初始化的配置对象</returns>
        public T CreateConfig<T>(string moduleId) where T : class, IConfig, new()
        {
            if (string.IsNullOrEmpty(moduleId))
            {
                _logger.LogError("创建配置失败：模块ID不能为空");
                throw new ArgumentException("模块ID不能为空", nameof(moduleId));
            }

            try
            {
                // 检查是否有自定义创建器
                if (_configCreators.TryGetValue(typeof(T), out var creator))
                {
                    var customCreator = creator as Func<string, T>;
                    if (customCreator != null)
                    {
                        _logger.LogInformation($"使用自定义创建器创建配置 {typeof(T).Name} 模块ID: {moduleId}");
                        return customCreator(moduleId);
                    }
                }

                // 使用默认方式创建配置
                var config = new T();
                if (config.ModuleId != moduleId)
                {
                    _logger.LogWarning($"配置对象的ModuleId({config.ModuleId})与请求的模块ID({moduleId})不匹配");
                }

                // 应用默认值
                config.ResetToDefault();

                _logger.LogInformation($"创建配置 {typeof(T).Name} 模块ID: {moduleId}");
                return config;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"创建配置 {typeof(T).Name} 时发生错误, 模块ID: {moduleId}");
                throw;
            }
        }

        /// <summary>
        /// 使用自定义初始化逻辑创建配置对象
        /// </summary>
        /// <typeparam name="T">配置类型</typeparam>
        /// <param name="moduleId">模块ID</param>
        /// <param name="initializer">初始化委托</param>
        /// <returns>初始化的配置对象</returns>
        public T CreateConfig<T>(string moduleId, Action<T> initializer) where T : class, IConfig, new()
        {
            if (initializer == null)
            {
                _logger.LogError("创建配置失败：初始化委托不能为空");
                throw new ArgumentNullException(nameof(initializer));
            }

            var config = CreateConfig<T>(moduleId);
            initializer(config);

            // 验证配置
            var (isValid, errorMessage) = config.Validate();
            if (!isValid)
            {
                _logger.LogError($"配置验证失败: {errorMessage}, 模块ID: {moduleId}");
                throw new InvalidOperationException($"配置验证失败: {errorMessage}");
            }

            return config;
        }

        /// <summary>
        /// 注册自定义配置创建器
        /// </summary>
        /// <typeparam name="T">配置类型</typeparam>
        /// <param name="creator">创建器委托</param>
        public void RegisterConfigCreator<T>(Func<string, T> creator) where T : class, IConfig
        {
            if (creator == null)
            {
                _logger.LogError($"注册配置创建器失败：创建器不能为空, 类型: {typeof(T).Name}");
                throw new ArgumentNullException(nameof(creator));
            }

            _configCreators[typeof(T)] = creator;
            _logger.LogInformation($"注册配置创建器: {typeof(T).Name}");
        }

        /// <summary>
        /// 创建并注册配置对象
        /// </summary>
        /// <typeparam name="T">配置类型</typeparam>
        /// <param name="moduleId">模块ID</param>
        /// <returns>初始化并注册的配置对象</returns>
        public T CreateAndRegisterConfig<T>(string moduleId) where T : class, IConfig, new()
        {
            var config = CreateConfig<T>(moduleId);

            // 验证配置
            var (isValid, errorMessage) = config.Validate();
            if (!isValid)
            {
                _logger.LogError($"配置验证失败: {errorMessage}, 模块ID: {moduleId}");
                throw new InvalidOperationException($"配置验证失败: {errorMessage}");
            }

            // 注册到配置管理器
            if (_configManager.RegisterConfig(config))
            {
                _logger.LogInformation($"配置注册成功: {typeof(T).Name}, 模块ID: {moduleId}");
                return config;
            }
            else
            {
                _logger.LogError($"配置注册失败: {typeof(T).Name}, 模块ID: {moduleId}");
                throw new InvalidOperationException($"配置注册失败: {typeof(T).Name}, 模块ID: {moduleId}");
            }
        }
    }
}