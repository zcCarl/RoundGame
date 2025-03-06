using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TacticalRPG.Core.Framework.Events;

namespace TacticalRPG.Core.Framework
{
    /// <summary>
    /// 游戏系统接口，作为整个游戏的主入口
    /// </summary>
    public interface IGameSystem
    {
        /// <summary>
        /// 获取模块管理器
        /// </summary>
        IGameModuleManager ModuleManager { get; }

        /// <summary>
        /// 获取事件管理器
        /// </summary>
        IEventManager EventManager { get; }

        /// <summary>
        /// 获取服务提供者
        /// </summary>
        IServiceProvider ServiceProvider { get; }

        /// <summary>
        /// 初始化游戏系统
        /// </summary>
        Task Initialize();

        /// <summary>
        /// 启动游戏系统
        /// </summary>
        Task Start();

        /// <summary>
        /// 暂停游戏系统
        /// </summary>
        Task Pause();

        /// <summary>
        /// 恢复游戏系统
        /// </summary>
        Task Resume();

        /// <summary>
        /// 停止游戏系统
        /// </summary>
        Task Stop();

        /// <summary>
        /// 卸载游戏系统
        /// </summary>
        Task Unload();
    }
}
