using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TacticalRPG.Core.Framework;

namespace TacticalRPG.Core.Modules.Config
{
    /// <summary>
    /// 配置模块接口，继承自游戏模块接口，负责配置系统的生命周期管理和高层操作
    /// </summary>
    public interface IConfigModule : IGameModule
    {
        /// <summary>
        /// 获取配置
        /// </summary>
        /// <typeparam name="T">配置类型</typeparam>
        /// <param name="moduleId">模块ID</param>
        /// <returns>配置对象</returns>
        Task<T> GetConfigAsync<T>(string moduleId) where T : class, IConfig;

        /// <summary>
        /// 注册配置
        /// </summary>
        /// <param name="config">配置对象</param>
        /// <returns>操作结果</returns>
        Task<(bool Success, string Message)> RegisterConfigAsync(IConfig config);

        /// <summary>
        /// 更新配置
        /// </summary>
        /// <param name="config">配置对象</param>
        /// <returns>操作结果</returns>
        Task<(bool Success, string Message)> UpdateConfigAsync(IConfig config);

        /// <summary>
        /// 重置配置
        /// </summary>
        /// <param name="moduleId">模块ID</param>
        /// <returns>操作结果</returns>
        Task<(bool Success, string Message)> ResetConfigAsync(string moduleId);

        /// <summary>
        /// 验证配置
        /// </summary>
        /// <param name="moduleId">模块ID</param>
        /// <returns>验证结果</returns>
        Task<(bool IsValid, string ErrorMessage)> ValidateConfigAsync(string moduleId);

        /// <summary>
        /// 验证所有配置
        /// </summary>
        /// <returns>包含无效配置的模块ID和错误信息的字典</returns>
        Task<Dictionary<string, string>> ValidateAllConfigsAsync();

        /// <summary>
        /// 加载指定模块的配置
        /// </summary>
        /// <param name="moduleId">模块ID</param>
        /// <returns>操作结果</returns>
        Task<(bool Success, string Message)> LoadConfigAsync(string moduleId);

        /// <summary>
        /// 保存指定模块的配置
        /// </summary>
        /// <param name="moduleId">模块ID</param>
        /// <returns>操作结果</returns>
        Task<(bool Success, string Message)> SaveConfigAsync(string moduleId);

        /// <summary>
        /// 加载所有配置
        /// </summary>
        /// <returns>操作结果</returns>
        Task<(bool Success, string Message)> LoadAllConfigsAsync();

        /// <summary>
        /// 保存所有配置
        /// </summary>
        /// <returns>操作结果</returns>
        Task<(bool Success, string Message)> SaveAllConfigsAsync();

        /// <summary>
        /// 获取配置工厂
        /// </summary>
        /// <returns>配置工厂实例</returns>
        IConfigFactory GetConfigFactory();

        /// <summary>
        /// 获取配置管理器
        /// </summary>
        /// <returns>配置管理器实例</returns>
        IConfigManager GetConfigManager();

        /// <summary>
        /// 获取配置缓存
        /// </summary>
        /// <returns>配置缓存实例</returns>
        IConfigCache GetConfigCache();

        /// <summary>
        /// 配置变更事件
        /// </summary>
        event Action<string, IConfig> ConfigChanged;
    }
}