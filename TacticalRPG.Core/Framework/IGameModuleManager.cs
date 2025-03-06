using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TacticalRPG.Core.Framework
{
    /// <summary>
    /// 游戏模块管理器接口，负责管理所有游戏模块的生命周期
    /// </summary>
    public interface IGameModuleManager
    {
        /// <summary>
        /// 注册模块
        /// </summary>
        /// <param name="module">要注册的模块</param>
        void RegisterModule(IGameModule module);

        /// <summary>
        /// 取消注册模块
        /// </summary>
        /// <param name="moduleName">要取消注册的模块名称</param>
        void UnregisterModule(string moduleName);

        /// <summary>
        /// 获取模块
        /// </summary>
        /// <typeparam name="T">模块类型</typeparam>
        /// <returns>模块实例</returns>
        T GetModule<T>() where T : IGameModule;

        /// <summary>
        /// 根据名称获取模块
        /// </summary>
        /// <param name="moduleName">模块名称</param>
        /// <returns>模块实例</returns>
        IGameModule GetModule(string moduleName);

        /// <summary>
        /// 获取所有已注册的模块
        /// </summary>
        /// <returns>所有模块的列表</returns>
        IReadOnlyList<IGameModule> GetAllModules();

        /// <summary>
        /// 初始化所有模块
        /// </summary>
        Task InitializeAllModules();

        /// <summary>
        /// 启动所有模块
        /// </summary>
        Task StartAllModules();

        /// <summary>
        /// 暂停所有模块
        /// </summary>
        Task PauseAllModules();

        /// <summary>
        /// 恢复所有模块
        /// </summary>
        Task ResumeAllModules();

        /// <summary>
        /// 停止所有模块
        /// </summary>
        Task StopAllModules();

        /// <summary>
        /// 卸载所有模块
        /// </summary>
        Task UnloadAllModules();
    }
}
