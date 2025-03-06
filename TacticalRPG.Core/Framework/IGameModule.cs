using System;
using System.Threading.Tasks;

namespace TacticalRPG.Core.Framework
{
    /// <summary>
    /// 游戏模块接口，定义所有模块通用的生命周期方法
    /// </summary>
    public interface IGameModule
    {
        /// <summary>
        /// 获取模块名称
        /// </summary>
        string ModuleName { get; }

        /// <summary>
        /// 获取模块优先级，优先级高的模块会先初始化
        /// </summary>
        int Priority { get; }

        /// <summary>
        /// 模块初始化
        /// </summary>
        Task Initialize();

        /// <summary>
        /// 模块启动
        /// </summary>
        Task Start();

        /// <summary>
        /// 模块暂停
        /// </summary>
        Task Pause();

        /// <summary>
        /// 模块恢复
        /// </summary>
        Task Resume();

        /// <summary>
        /// 模块停止
        /// </summary>
        Task Stop();

        /// <summary>
        /// 模块卸载
        /// </summary>
        Task Unload();
    }
}