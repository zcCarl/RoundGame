using System;
using System.Collections.Generic;
using TacticalRPG.Core.Modules.Config;

namespace TacticalRPG.Implementation.Modules.Config
{
    /// <summary>
    /// 配置基类，提供IConfig接口的基本实现
    /// </summary>
    public abstract class ConfigBase : IConfig
    {
        /// <summary>
        /// 配置模块的唯一标识符
        /// </summary>
        public string ModuleId { get; protected set; }

        /// <summary>
        /// 配置的名称
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// 配置的版本
        /// </summary>
        public Version Version { get; protected set; }

        /// <summary>
        /// 配置项字典，用于存储自定义配置
        /// </summary>
        protected Dictionary<string, object> ConfigItems { get; private set; } = new Dictionary<string, object>();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="moduleId">模块ID</param>
        /// <param name="name">配置名称</param>
        /// <param name="version">配置版本</param>
        protected ConfigBase(string moduleId, string name, Version version)
        {
            ModuleId = moduleId ?? throw new ArgumentNullException(nameof(moduleId));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Version = version ?? throw new ArgumentNullException(nameof(version));

            InitDefaultValues();
        }

        /// <summary>
        /// 初始化默认值
        /// </summary>
        protected abstract void InitDefaultValues();

        /// <summary>
        /// 验证配置是否有效
        /// </summary>
        /// <returns>验证结果，如果有错误则包含错误信息</returns>
        public virtual (bool IsValid, string ErrorMessage) Validate()
        {
            if (string.IsNullOrEmpty(ModuleId))
                return (false, "ModuleId不能为空");

            if (string.IsNullOrEmpty(Name))
                return (false, "Name不能为空");

            if (Version == null)
                return (false, "Version不能为空");

            return (true, string.Empty);
        }

        /// <summary>
        /// 重置配置为默认值
        /// </summary>
        public virtual void ResetToDefault()
        {
            ConfigItems.Clear();
            InitDefaultValues();
        }

        /// <summary>
        /// 获取配置项值
        /// </summary>
        /// <typeparam name="T">值类型</typeparam>
        /// <param name="key">配置项键</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>配置项值，如不存在则返回默认值</returns>
        public T GetValue<T>(string key, T defaultValue = default)
        {
            if (string.IsNullOrEmpty(key) || !ConfigItems.ContainsKey(key))
                return defaultValue;

            if (ConfigItems[key] is T value)
                return value;

            try
            {
                return (T)Convert.ChangeType(ConfigItems[key], typeof(T));
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// 设置配置项值
        /// </summary>
        /// <typeparam name="T">值类型</typeparam>
        /// <param name="key">配置项键</param>
        /// <param name="value">配置项值</param>
        public void SetValue<T>(string key, T value)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            ConfigItems[key] = value;
        }

        /// <summary>
        /// 检查配置项是否存在
        /// </summary>
        /// <param name="key">配置项键</param>
        /// <returns>是否存在</returns>
        public bool HasValue(string key)
        {
            return !string.IsNullOrEmpty(key) && ConfigItems.ContainsKey(key);
        }
    }
}