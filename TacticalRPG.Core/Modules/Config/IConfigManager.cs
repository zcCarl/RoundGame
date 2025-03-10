using System;
using System.Collections.Generic;

namespace TacticalRPG.Core.Modules.Config
{
    /// <summary>
    /// 配置管理器接口，负责管理所有模块的配置
    /// </summary>
    public interface IConfigManager
    {
        /// <summary>
        /// 配置变更事件委托
        /// </summary>
        /// <param name="moduleId">配置模块ID</param>
        /// <param name="config">配置对象</param>
        delegate void ConfigChangedEventHandler(string moduleId, IConfig config);

        /// <summary>
        /// 配置变更事件
        /// </summary>
        event ConfigChangedEventHandler ConfigChanged;

        /// <summary>
        /// 注册配置
        /// </summary>
        /// <param name="config">配置对象</param>
        /// <returns>是否成功注册</returns>
        bool RegisterConfig(IConfig config);

        /// <summary>
        /// 获取配置
        /// </summary>
        /// <typeparam name="T">配置类型</typeparam>
        /// <param name="moduleId">模块ID</param>
        /// <returns>配置对象，如不存在则返回默认值</returns>
        T GetConfig<T>(string moduleId) where T : class, IConfig;

        /// <summary>
        /// 更新配置
        /// </summary>
        /// <param name="config">配置对象</param>
        /// <returns>是否成功更新</returns>
        bool UpdateConfig(IConfig config);

        /// <summary>
        /// 加载所有配置
        /// </summary>
        /// <param name="configFolderPath">配置文件夹路径</param>
        /// <returns>是否成功加载</returns>
        bool LoadAllConfigs(string configFolderPath);

        /// <summary>
        /// 保存所有配置
        /// </summary>
        /// <param name="configFolderPath">配置文件夹路径</param>
        /// <returns>是否成功保存</returns>
        bool SaveAllConfigs(string configFolderPath);

        /// <summary>
        /// 加载指定模块的配置
        /// </summary>
        /// <param name="moduleId">模块ID</param>
        /// <param name="configFolderPath">配置文件夹路径</param>
        /// <returns>是否成功加载</returns>
        bool LoadConfig(string moduleId, string configFolderPath);

        /// <summary>
        /// 保存指定模块的配置
        /// </summary>
        /// <param name="moduleId">模块ID</param>
        /// <param name="configFolderPath">配置文件夹路径</param>
        /// <returns>是否成功保存</returns>
        bool SaveConfig(string moduleId, string configFolderPath);

        /// <summary>
        /// 重置指定模块的配置为默认值
        /// </summary>
        /// <param name="moduleId">模块ID</param>
        /// <returns>是否成功重置</returns>
        bool ResetConfig(string moduleId);

        /// <summary>
        /// 获取所有已注册的配置模块ID
        /// </summary>
        /// <returns>模块ID列表</returns>
        IReadOnlyList<string> GetAllConfigModuleIds();

        /// <summary>
        /// 验证配置是否有效
        /// </summary>
        /// <param name="moduleId">模块ID</param>
        /// <returns>验证结果，包含是否有效和错误信息</returns>
        (bool IsValid, string ErrorMessage) ValidateConfig(string moduleId);

        /// <summary>
        /// 验证所有配置
        /// </summary>
        /// <returns>验证结果，包含无效配置的模块ID和错误信息</returns>
        Dictionary<string, string> ValidateAllConfigs();

        /// <summary>
        /// 注册配置，并进行验证
        /// </summary>
        /// <param name="config">配置对象</param>
        /// <param name="skipValidation">是否跳过验证</param>
        /// <returns>是否成功注册</returns>
        bool RegisterConfig(IConfig config, bool skipValidation = false);
    }
}