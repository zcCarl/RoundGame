using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TacticalRPG.Core.Framework;
using TacticalRPG.Core.Framework.Events;

namespace TacticalRPG.Implementation.Framework
{
    /// <summary>
    /// 游戏系统实现类
    /// </summary>
    public class GameSystem : IGameSystem
    {
        private readonly ILogger<GameSystem> _logger;
        private bool _isInitialized;

        /// <summary>
        /// 获取模块管理器
        /// </summary>
        public IGameModuleManager ModuleManager { get; }

        /// <summary>
        /// 获取事件管理器
        /// </summary>
        public IEventManager EventManager { get; }

        /// <summary>
        /// 获取服务提供者
        /// </summary>
        public IServiceProvider ServiceProvider { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="serviceProvider">服务提供者</param>
        /// <param name="moduleManager">模块管理器</param>
        /// <param name="eventManager">事件管理器</param>
        /// <param name="logger">日志记录器</param>
        public GameSystem(
            IServiceProvider serviceProvider,
            IGameModuleManager moduleManager,
            IEventManager eventManager,
            ILogger<GameSystem> logger)
        {
            ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            ModuleManager = moduleManager ?? throw new ArgumentNullException(nameof(moduleManager));
            EventManager = eventManager ?? throw new ArgumentNullException(nameof(eventManager));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// 初始化游戏系统
        /// </summary>
        public async Task Initialize()
        {
            if (_isInitialized)
            {
                _logger.LogWarning("游戏系统已经初始化，无需重复初始化");
                return;
            }

            _logger.LogInformation("开始初始化游戏系统...");

            // 初始化所有已注册模块
            await ModuleManager.InitializeAllModules();

            _isInitialized = true;
            _logger.LogInformation("游戏系统初始化完成");
        }

        /// <summary>
        /// 启动游戏系统
        /// </summary>
        public async Task Start()
        {
            if (!_isInitialized)
            {
                _logger.LogWarning("游戏系统尚未初始化，无法启动");
                await Initialize();
            }

            _logger.LogInformation("开始启动游戏系统...");

            // 启动所有模块
            await ModuleManager.StartAllModules();

            _logger.LogInformation("游戏系统启动完成");
        }

        /// <summary>
        /// 暂停游戏系统
        /// </summary>
        public async Task Pause()
        {
            if (!_isInitialized)
            {
                _logger.LogWarning("游戏系统尚未初始化，无法暂停");
                return;
            }

            _logger.LogInformation("开始暂停游戏系统...");

            // 暂停所有模块
            await ModuleManager.PauseAllModules();

            _logger.LogInformation("游戏系统暂停完成");
        }

        /// <summary>
        /// 恢复游戏系统
        /// </summary>
        public async Task Resume()
        {
            if (!_isInitialized)
            {
                _logger.LogWarning("游戏系统尚未初始化，无法恢复");
                return;
            }

            _logger.LogInformation("开始恢复游戏系统...");

            // 恢复所有模块
            await ModuleManager.ResumeAllModules();

            _logger.LogInformation("游戏系统恢复完成");
        }

        /// <summary>
        /// 停止游戏系统
        /// </summary>
        public async Task Stop()
        {
            if (!_isInitialized)
            {
                _logger.LogWarning("游戏系统尚未初始化，无法停止");
                return;
            }

            _logger.LogInformation("开始停止游戏系统...");

            // 停止所有模块
            await ModuleManager.StopAllModules();

            _logger.LogInformation("游戏系统停止完成");
        }

        /// <summary>
        /// 卸载游戏系统
        /// </summary>
        public async Task Unload()
        {
            if (!_isInitialized)
            {
                _logger.LogWarning("游戏系统尚未初始化，无法卸载");
                return;
            }

            _logger.LogInformation("开始卸载游戏系统...");

            // 卸载所有模块
            await ModuleManager.UnloadAllModules();

            _isInitialized = false;
            _logger.LogInformation("游戏系统卸载完成");
        }
    }
}