using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TacticalRPG.Core.Framework.Events;

namespace TacticalRPG.Core.Framework
{
    /// <summary>
    /// 基础游戏模块类，作为所有游戏模块的基类
    /// </summary>
    public abstract class BaseGameModule : IGameModule
    {
        /// <summary>
        /// 游戏系统引用
        /// </summary>
        protected readonly IGameSystem GameSystem;

        /// <summary>
        /// 日志记录器
        /// </summary>
        protected readonly ILogger Logger;

        /// <summary>
        /// 事件管理器
        /// </summary>
        protected readonly IEventManager EventManager;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="gameSystem">游戏系统</param>
        /// <param name="logger">日志记录器</param>
        protected BaseGameModule(
            IGameSystem gameSystem,
            ILogger logger)
        {
            GameSystem = gameSystem ?? throw new ArgumentNullException(nameof(gameSystem));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            EventManager = gameSystem.EventManager;
        }

        /// <summary>
        /// 获取模块名称
        /// </summary>
        public abstract string ModuleName { get; }

        /// <summary>
        /// 获取模块优先级，优先级高的模块会先初始化
        /// </summary>
        public virtual int Priority => 0;

        /// <summary>
        /// 模块初始化
        /// </summary>
        public virtual Task Initialize()
        {
            Logger.LogInformation($"模块 {ModuleName} 初始化中...");
            return Task.CompletedTask;
        }

        /// <summary>
        /// 模块启动
        /// </summary>
        public virtual Task Start()
        {
            Logger.LogInformation($"模块 {ModuleName} 启动中...");
            return Task.CompletedTask;
        }

        /// <summary>
        /// 模块暂停
        /// </summary>
        public virtual Task Pause()
        {
            Logger.LogInformation($"模块 {ModuleName} 暂停中...");
            return Task.CompletedTask;
        }

        /// <summary>
        /// 模块恢复
        /// </summary>
        public virtual Task Resume()
        {
            Logger.LogInformation($"模块 {ModuleName} 恢复中...");
            return Task.CompletedTask;
        }

        /// <summary>
        /// 模块停止
        /// </summary>
        public virtual Task Stop()
        {
            Logger.LogInformation($"模块 {ModuleName} 停止中...");
            return Task.CompletedTask;
        }

        /// <summary>
        /// 模块卸载
        /// </summary>
        public virtual Task Unload()
        {
            Logger.LogInformation($"模块 {ModuleName} 卸载中...");
            return Task.CompletedTask;
        }
    }
}
