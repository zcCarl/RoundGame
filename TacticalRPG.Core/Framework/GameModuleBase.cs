using System.Threading.Tasks;

namespace TacticalRPG.Core.Framework
{
    /// <summary>
    /// 游戏模块基类，实现IGameModule基本功能
    /// </summary>
    public abstract class GameModuleBase : IGameModule
    {
        /// <summary>
        /// 模块名称
        /// </summary>
        public abstract string ModuleName { get; }

        /// <summary>
        /// 模块优先级
        /// </summary>
        public abstract int Priority { get; }

        /// <summary>
        /// 初始化模块
        /// </summary>
        public virtual Task Initialize()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// 启动模块
        /// </summary>
        public virtual Task Start()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// 暂停模块
        /// </summary>
        public virtual Task Pause()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// 恢复模块
        /// </summary>
        public virtual Task Resume()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// 停止模块
        /// </summary>
        public virtual Task Stop()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// 卸载模块
        /// </summary>
        public virtual Task Unload()
        {
            return Task.CompletedTask;
        }
    }
}