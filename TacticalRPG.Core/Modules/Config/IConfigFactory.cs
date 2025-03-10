using System;

namespace TacticalRPG.Core.Modules.Config
{
    /// <summary>
    /// 配置工厂接口，负责创建和初始化各种配置对象
    /// </summary>
    public interface IConfigFactory
    {
        /// <summary>
        /// 创建配置对象
        /// </summary>
        /// <typeparam name="T">配置类型</typeparam>
        /// <param name="moduleId">模块ID</param>
        /// <returns>初始化的配置对象</returns>
        T CreateConfig<T>(string moduleId) where T : class, IConfig, new();

        /// <summary>
        /// 使用自定义初始化逻辑创建配置对象
        /// </summary>
        /// <typeparam name="T">配置类型</typeparam>
        /// <param name="moduleId">模块ID</param>
        /// <param name="initializer">初始化委托</param>
        /// <returns>初始化的配置对象</returns>
        T CreateConfig<T>(string moduleId, Action<T> initializer) where T : class, IConfig, new();

        /// <summary>
        /// 注册自定义配置创建器
        /// </summary>
        /// <typeparam name="T">配置类型</typeparam>
        /// <param name="creator">创建器委托</param>
        void RegisterConfigCreator<T>(Func<string, T> creator) where T : class, IConfig;

        /// <summary>
        /// 创建并注册配置对象
        /// </summary>
        /// <typeparam name="T">配置类型</typeparam>
        /// <param name="moduleId">模块ID</param>
        /// <returns>初始化并注册的配置对象</returns>
        T CreateAndRegisterConfig<T>(string moduleId) where T : class, IConfig, new();
    }
}