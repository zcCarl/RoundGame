using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using TacticalRPG.Core.Modules.Config;
using TacticalRPG.Core.Modules.Inventory;

namespace TacticalRPG.Implementation.Modules.Inventory
{
    /// <summary>
    /// 物品配置管理器，负责物品配置的加载和管理
    /// </summary>
    public class ItemConfigManager
    {
        private readonly ILogger<ItemConfigManager> _logger;
        private readonly IConfigManager _configManager;
        private readonly Dictionary<string, ItemConfig> _itemConfigs = new Dictionary<string, ItemConfig>();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="logger">日志记录器</param>
        /// <param name="configManager">配置管理器</param>
        public ItemConfigManager(ILogger<ItemConfigManager> logger, IConfigManager configManager)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configManager = configManager ?? throw new ArgumentNullException(nameof(configManager));
        }

        /// <summary>
        /// 创建物品配置
        /// </summary>
        /// <param name="itemId">物品ID</param>
        /// <param name="name">物品名称</param>
        /// <returns>物品配置</returns>
        public ItemConfig CreateItemConfig(string itemId, string name = "")
        {
            if (string.IsNullOrEmpty(itemId))
                throw new ArgumentException("物品ID不能为空", nameof(itemId));

            if (_itemConfigs.TryGetValue(itemId, out var existingConfig))
            {
                _logger.LogWarning($"物品配置 {itemId} 已存在，返回现有配置");
                return existingConfig;
            }

            var config = new ItemConfig(itemId, name);
            RegisterItemConfig(config);
            return config;
        }

        /// <summary>
        /// 注册物品配置
        /// </summary>
        /// <param name="config">物品配置</param>
        /// <returns>是否成功注册</returns>
        public bool RegisterItemConfig(ItemConfig config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            string itemId = ExtractItemId(config.ModuleId);
            if (string.IsNullOrEmpty(itemId))
            {
                _logger.LogError($"无法从ModuleId '{config.ModuleId}'中提取物品ID");
                return false;
            }

            if (_itemConfigs.ContainsKey(itemId))
            {
                _logger.LogWarning($"物品配置 {itemId} 已存在，更新现有配置");
                _itemConfigs[itemId] = config;
            }
            else
            {
                _itemConfigs.Add(itemId, config);
            }

            // 注册到配置管理器
            bool registered = _configManager.RegisterConfig(config);
            if (!registered)
            {
                _logger.LogError($"物品配置 {itemId} 注册到配置管理器失败");
                return false;
            }

            _logger.LogInformation($"物品配置 {itemId} 注册成功");
            return true;
        }

        /// <summary>
        /// 获取物品配置
        /// </summary>
        /// <param name="itemId">物品ID</param>
        /// <returns>物品配置，如不存在则返回null</returns>
        public ItemConfig GetItemConfig(string itemId)
        {
            if (string.IsNullOrEmpty(itemId))
                return null;

            if (_itemConfigs.TryGetValue(itemId, out var config))
                return config;

            // 尝试从配置管理器获取
            var moduleId = ItemConfig.GetModuleId(itemId);
            config = _configManager.GetConfig<ItemConfig>(moduleId);
            if (config != null)
            {
                _itemConfigs[itemId] = config;
                return config;
            }

            return null;
        }

        /// <summary>
        /// 更新物品配置
        /// </summary>
        /// <param name="config">物品配置</param>
        /// <returns>是否成功更新</returns>
        public bool UpdateItemConfig(ItemConfig config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            string itemId = ExtractItemId(config.ModuleId);
            if (string.IsNullOrEmpty(itemId))
            {
                _logger.LogError($"无法从ModuleId '{config.ModuleId}'中提取物品ID");
                return false;
            }

            _itemConfigs[itemId] = config;

            // 更新到配置管理器
            bool updated = _configManager.UpdateConfig(config);
            if (!updated)
            {
                _logger.LogError($"物品配置 {itemId} 更新到配置管理器失败");
                return false;
            }

            _logger.LogInformation($"物品配置 {itemId} 更新成功");
            return true;
        }

        /// <summary>
        /// 移除物品配置
        /// </summary>
        /// <param name="itemId">物品ID</param>
        /// <returns>是否成功移除</returns>
        public bool RemoveItemConfig(string itemId)
        {
            if (string.IsNullOrEmpty(itemId))
                return false;

            if (!_itemConfigs.Remove(itemId))
                return false;

            _logger.LogInformation($"物品配置 {itemId} 移除成功");
            return true;
        }

        /// <summary>
        /// 获取所有物品配置
        /// </summary>
        /// <returns>物品配置列表</returns>
        public IReadOnlyList<ItemConfig> GetAllItemConfigs()
        {
            return _itemConfigs.Values.ToList();
        }

        /// <summary>
        /// 获取所有物品ID
        /// </summary>
        /// <returns>物品ID列表</returns>
        public IReadOnlyList<string> GetAllItemIds()
        {
            return _itemConfigs.Keys.ToList();
        }

        /// <summary>
        /// 根据类型过滤物品配置
        /// </summary>
        /// <param name="type">物品类型</param>
        /// <returns>物品配置列表</returns>
        public IReadOnlyList<ItemConfig> GetItemConfigsByType(ItemType type)
        {
            return _itemConfigs.Values.Where(c => c.Type == type).ToList();
        }

        /// <summary>
        /// 根据稀有度过滤物品配置
        /// </summary>
        /// <param name="rarity">物品稀有度</param>
        /// <returns>物品配置列表</returns>
        public IReadOnlyList<ItemConfig> GetItemConfigsByRarity(ItemRarity rarity)
        {
            return _itemConfigs.Values.Where(c => c.Rarity == rarity).ToList();
        }

        /// <summary>
        /// 加载物品配置
        /// </summary>
        /// <param name="configFolderPath">配置文件夹路径</param>
        /// <returns>是否成功加载</returns>
        public bool LoadItemConfigs(string configFolderPath)
        {
            try
            {
                // 从配置管理器加载
                var allModuleIds = _configManager.GetAllConfigModuleIds();
                foreach (var moduleId in allModuleIds)
                {
                    if (moduleId.StartsWith(ItemConfig.MODULE_ID_PREFIX))
                    {
                        var config = _configManager.GetConfig<ItemConfig>(moduleId);
                        if (config != null)
                        {
                            string itemId = ExtractItemId(moduleId);
                            if (!string.IsNullOrEmpty(itemId))
                            {
                                _itemConfigs[itemId] = config;
                            }
                        }
                    }
                }

                _logger.LogInformation($"已加载 {_itemConfigs.Count} 个物品配置");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "加载物品配置失败");
                return false;
            }
        }

        /// <summary>
        /// 保存物品配置
        /// </summary>
        /// <param name="configFolderPath">配置文件夹路径</param>
        /// <returns>是否成功保存</returns>
        public bool SaveItemConfigs(string configFolderPath)
        {
            try
            {
                // 先注册所有配置
                foreach (var config in _itemConfigs.Values)
                {
                    _configManager.RegisterConfig(config);
                }

                // 保存到配置管理器
                bool saved = _configManager.SaveAllConfigs(configFolderPath);
                if (!saved)
                {
                    _logger.LogError("保存物品配置到配置管理器失败");
                    return false;
                }

                _logger.LogInformation($"已保存 {_itemConfigs.Count} 个物品配置");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "保存物品配置失败");
                return false;
            }
        }

        /// <summary>
        /// 从ModuleId提取物品ID
        /// </summary>
        /// <param name="moduleId">模块ID</param>
        /// <returns>物品ID</returns>
        private string ExtractItemId(string moduleId)
        {
            if (string.IsNullOrEmpty(moduleId) || !moduleId.StartsWith(ItemConfig.MODULE_ID_PREFIX))
                return null;

            string prefix = $"{ItemConfig.MODULE_ID_PREFIX}.";
            return moduleId.Substring(prefix.Length);
        }
    }
}