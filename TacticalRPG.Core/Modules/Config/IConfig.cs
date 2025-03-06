using System;

namespace TacticalRPG.Core.Modules.Config
{
    /// <summary>
    /// 配置接口，所有配置对象都应实现此接口
    /// </summary>
    public interface IConfig
    {
        /// <summary>
        /// 配置模块的唯一标识符
        /// </summary>
        string ModuleId { get; }

        /// <summary>
        /// 配置的名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 配置的版本
        /// </summary>
        Version Version { get; }

        /// <summary>
        /// 验证配置是否有效
        /// </summary>
        /// <returns>验证结果，如果有错误则包含错误信息</returns>
        (bool IsValid, string ErrorMessage) Validate();

        /// <summary>
        /// 重置配置为默认值
        /// </summary>
        void ResetToDefault();
    }
}