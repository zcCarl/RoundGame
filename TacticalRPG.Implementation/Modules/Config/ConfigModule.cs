using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TacticalRPG.Core.Framework;
using TacticalRPG.Core.Modules.Config;
using TacticalRPG.Implementation.Framework;

namespace TacticalRPG.Implementation.Modules.Config
{
    /// <summary>
    /// 配置模块实现类，负责管理游戏配置的全生命周期
    /// </summary>
    public class ConfigModule : BaseGameModule, IConfigModule
    {
        private readonly ILogger<ConfigModule> _logger;
        private readonly IConfigManager _configManager;
        private readonly IConfigFactory _configFactory;
        private readonly IConfigCache _configCache;
        private string _configFolderPath;
        private bool _isInitialized = false;

        /// <summary>
        /// 模块名称
        /// </summary>
        public override string ModuleName => "ConfigModule";

        /// <summary>
        /// 配置变更事件
        /// </summary>
        public event Action<string, IConfig> ConfigChanged;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="gameSystem">游戏系统</param>
        /// <param name="logger">日志记录器</param>
        public ConfigModule(GameSystem gameSystem, ILogger<ConfigModule> logger)
            : base(gameSystem, logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            // 创建缓存
            var cacheLogger = gameSystem.ServiceProvider.GetService(typeof(ILogger<ConfigCache>)) as ILogger<ConfigCache>;
            _configCache = new ConfigCache(cacheLogger);

            // 创建配置管理器
            var managerLogger = gameSystem.ServiceProvider.GetService(typeof(ILogger<ConfigManager>)) as ILogger<ConfigManager>;
            _configManager = new ConfigManager(managerLogger, _configCache);

            // 订阅配置变更事件
            _configManager.ConfigChanged += OnConfigChanged;

            // 创建配置工厂
            var factoryLogger = gameSystem.ServiceProvider.GetService(typeof(ILogger<ConfigFactory>)) as ILogger<ConfigFactory>;
            _configFactory = new ConfigFactory(_configManager, factoryLogger);

            _logger.LogInformation("配置模块构造完成");
        }

        /// <summary>
        /// 配置变更处理
        /// </summary>
        /// <param name="moduleId">模块ID</param>
        /// <param name="config">配置对象</param>
        private void OnConfigChanged(string moduleId, IConfig config)
        {
            ConfigChanged?.Invoke(moduleId, config);
        }

        #region 游戏模块生命周期

        /// <summary>
        /// 初始化模块
        /// </summary>
        /// <returns>是否成功初始化</returns>
        public override async Task Initialize()
        {
            if (_isInitialized)
            {
                _logger.LogWarning("配置模块已经初始化");
                await base.Initialize();
                return;
            }

            try
            {
                _logger.LogInformation("正在初始化配置模块...");

                // 设置配置文件夹路径
                string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                _configFolderPath = Path.Combine(appDataPath, "TacticalRPG", "Configs");

                // 确保配置目录存在
                if (!Directory.Exists(_configFolderPath))
                {
                    Directory.CreateDirectory(_configFolderPath);
                    _logger.LogInformation($"创建配置目录: {_configFolderPath}");
                }

                // 尝试加载所有配置
                var loadResult = await LoadAllConfigsAsync();
                if (!loadResult.Success)
                {
                    _logger.LogWarning($"加载配置失败: {loadResult.Message}，将使用默认配置");
                }

                // 标记为已初始化
                _isInitialized = true;
                _logger.LogInformation("配置模块初始化完成");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "初始化配置模块时发生错误");
            }

            // 调用基类方法
            await base.Initialize();
        }

        /// <summary>
        /// 启动模块
        /// </summary>
        /// <returns>是否成功启动</returns>
        public override async Task Start()
        {
            if (!_isInitialized)
            {
                _logger.LogError("配置模块尚未初始化，无法启动");
                await base.Start();
                return;
            }

            try
            {
                _logger.LogInformation("正在启动配置模块...");

                // 验证所有配置
                var invalidConfigs = await ValidateAllConfigsAsync();
                if (invalidConfigs.Count > 0)
                {
                    _logger.LogWarning($"检测到 {invalidConfigs.Count} 个无效配置");
                    foreach (var kvp in invalidConfigs)
                    {
                        _logger.LogWarning($"无效配置 - 模块: {kvp.Key}, 错误: {kvp.Value}");
                    }
                }

                _logger.LogInformation("配置模块启动完成");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "启动配置模块时发生错误");
            }

            // 调用基类方法
            await base.Start();
        }

        /// <summary>
        /// 停止模块
        /// </summary>
        /// <returns>是否成功停止</returns>
        public override async Task Stop()
        {
            if (!_isInitialized)
            {
                _logger.LogWarning("配置模块尚未初始化，无需停止");
                await base.Stop();
                return;
            }

            try
            {
                _logger.LogInformation("正在停止配置模块...");

                // 保存所有配置
                var saveResult = await SaveAllConfigsAsync();
                if (!saveResult.Success)
                {
                    _logger.LogError($"保存配置时发生错误: {saveResult.Message}");
                }

                // 清空缓存
                _configCache.ClearCache();

                _logger.LogInformation("配置模块停止完成");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "停止配置模块时发生错误");
            }

            // 调用基类方法
            await base.Stop();
        }

        #endregion

        #region 配置模块接口实现

        /// <summary>
        /// 获取配置
        /// </summary>
        /// <typeparam name="T">配置类型</typeparam>
        /// <param name="moduleId">模块ID</param>
        /// <returns>配置对象</returns>
        public async Task<T> GetConfigAsync<T>(string moduleId) where T : class, IConfig
        {
            if (string.IsNullOrEmpty(moduleId))
                return null;

            return await Task.FromResult(_configManager.GetConfig<T>(moduleId));
        }

        /// <summary>
        /// 注册配置
        /// </summary>
        /// <param name="config">配置对象</param>
        /// <returns>操作结果</returns>
        public async Task<(bool Success, string Message)> RegisterConfigAsync(IConfig config)
        {
            if (config == null)
                return (false, "配置对象不能为空");

            bool success = _configManager.RegisterConfig(config);
            string message = success ? "注册成功" : "注册失败";
            return await Task.FromResult((success, message));
        }

        /// <summary>
        /// 更新配置
        /// </summary>
        /// <param name="config">配置对象</param>
        /// <returns>操作结果</returns>
        public async Task<(bool Success, string Message)> UpdateConfigAsync(IConfig config)
        {
            if (config == null)
                return (false, "配置对象不能为空");

            bool success = _configManager.UpdateConfig(config);
            string message = success ? "更新成功" : "更新失败";
            return await Task.FromResult((success, message));
        }

        /// <summary>
        /// 重置配置
        /// </summary>
        /// <param name="moduleId">模块ID</param>
        /// <returns>操作结果</returns>
        public async Task<(bool Success, string Message)> ResetConfigAsync(string moduleId)
        {
            if (string.IsNullOrEmpty(moduleId))
                return (false, "模块ID不能为空");

            bool success = _configManager.ResetConfig(moduleId);
            string message = success ? "重置成功" : "重置失败";
            return await Task.FromResult((success, message));
        }

        /// <summary>
        /// 验证配置
        /// </summary>
        /// <param name="moduleId">模块ID</param>
        /// <returns>验证结果</returns>
        public async Task<(bool IsValid, string ErrorMessage)> ValidateConfigAsync(string moduleId)
        {
            if (string.IsNullOrEmpty(moduleId))
                return (false, "模块ID不能为空");

            return await Task.FromResult(_configManager.ValidateConfig(moduleId));
        }

        /// <summary>
        /// 验证所有配置
        /// </summary>
        /// <returns>包含无效配置的模块ID和错误信息的字典</returns>
        public async Task<Dictionary<string, string>> ValidateAllConfigsAsync()
        {
            return await Task.FromResult(_configManager.ValidateAllConfigs());
        }

        /// <summary>
        /// 加载指定模块的配置
        /// </summary>
        /// <param name="moduleId">模块ID</param>
        /// <returns>操作结果</returns>
        public async Task<(bool Success, string Message)> LoadConfigAsync(string moduleId)
        {
            if (string.IsNullOrEmpty(moduleId))
                return (false, "模块ID不能为空");

            bool success = _configManager.LoadConfig(moduleId, _configFolderPath);
            string message = success ? "加载成功" : "加载失败";
            return await Task.FromResult((success, message));
        }

        /// <summary>
        /// 保存指定模块的配置
        /// </summary>
        /// <param name="moduleId">模块ID</param>
        /// <returns>操作结果</returns>
        public async Task<(bool Success, string Message)> SaveConfigAsync(string moduleId)
        {
            if (string.IsNullOrEmpty(moduleId))
                return (false, "模块ID不能为空");

            bool success = _configManager.SaveConfig(moduleId, _configFolderPath);
            string message = success ? "保存成功" : "保存失败";
            return await Task.FromResult((success, message));
        }

        /// <summary>
        /// 加载所有配置
        /// </summary>
        /// <returns>操作结果</returns>
        public async Task<(bool Success, string Message)> LoadAllConfigsAsync()
        {
            bool success = _configManager.LoadAllConfigs(_configFolderPath);
            string message = success ? "加载成功" : "加载失败";
            return await Task.FromResult((success, message));
        }

        /// <summary>
        /// 保存所有配置
        /// </summary>
        /// <returns>操作结果</returns>
        public async Task<(bool Success, string Message)> SaveAllConfigsAsync()
        {
            bool success = _configManager.SaveAllConfigs(_configFolderPath);
            string message = success ? "保存成功" : "保存失败";
            return await Task.FromResult((success, message));
        }

        /// <summary>
        /// 获取配置工厂
        /// </summary>
        /// <returns>配置工厂实例</returns>
        public IConfigFactory GetConfigFactory()
        {
            return _configFactory;
        }

        /// <summary>
        /// 获取配置管理器
        /// </summary>
        /// <returns>配置管理器实例</returns>
        public IConfigManager GetConfigManager()
        {
            return _configManager;
        }

        /// <summary>
        /// 获取配置缓存
        /// </summary>
        /// <returns>配置缓存实例</returns>
        public IConfigCache GetConfigCache()
        {
            return _configCache;
        }

        #endregion
    }
}