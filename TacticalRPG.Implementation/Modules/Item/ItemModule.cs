using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TacticalRPG.Core.Framework;
using TacticalRPG.Core.Modules.Item;
using TacticalRPG.Core.Modules.Config;
using System.IO;

namespace TacticalRPG.Implementation.Modules.Item
{
    /// <summary>
    /// 物品模块实现类 - 负责作为游戏模块与系统集成，并提供异步API
    /// </summary>
    public class ItemModule : BaseGameModule, IItemModule
    {
        private readonly IConfigManager _configManager;
        private readonly IItemManager _itemManager;
        private readonly IItemConfigManager _itemConfigManager;
        private readonly Dictionary<string, IItemTemplate> _itemTemplates = new Dictionary<string, IItemTemplate>();
        private bool _isInitialized = false;

        // 事件处理委托，用于模块间通信
        private readonly Dictionary<string, Func<object, Task>> _eventHandlers = new Dictionary<string, Func<object, Task>>();

        /// <summary>
        /// 模块名称
        /// </summary>
        public override string ModuleName => "物品管理模块";

        /// <summary>
        /// 模块优先级
        /// </summary>
        public override int Priority => 40; // 物品模块优先级高于背包模块

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="gameSystem">游戏系统</param>
        /// <param name="logger">日志记录器</param>
        /// <param name="configManager">配置管理器</param>
        /// <param name="itemManager">物品管理器</param>
        /// <param name="itemConfigManager">物品配置管理器</param>
        public ItemModule(
            IGameSystem gameSystem,
            ILogger<ItemModule> logger,
            IConfigManager configManager,
            IItemManager itemManager,
            IItemConfigManager itemConfigManager) : base(gameSystem, logger)
        {
            _configManager = configManager ?? throw new ArgumentNullException(nameof(configManager));
            _itemManager = itemManager ?? throw new ArgumentNullException(nameof(itemManager));
            _itemConfigManager = itemConfigManager ?? throw new ArgumentNullException(nameof(itemConfigManager));

            // 注册事件处理器
            RegisterEventHandlers();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public override async Task Initialize()
        {
            if (_isInitialized)
            {
                return;
            }

            Logger.LogInformation("物品模块初始化中...");

            try
            {
                // 加载物品配置
                string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Configs", "Items");
                Directory.CreateDirectory(configPath); // 确保配置目录存在

                bool configLoaded = _itemConfigManager.LoadItemConfigs(configPath);
                if (configLoaded)
                {
                    Logger.LogInformation($"物品配置加载成功，路径：{configPath}");
                }
                else
                {
                    Logger.LogWarning($"物品配置加载失败或无配置文件，将使用默认配置，路径：{configPath}");
                }

                // 初始化物品模板
                await InitializeItemTemplates();

                // 注册事件处理
                // EventManager.Subscribe<CharacterCreatedEvent>(OnCharacterCreated);

                _isInitialized = true;
                await base.Initialize();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "物品模块初始化失败");
                throw;
            }
        }

        /// <summary>
        /// 注册事件处理器
        /// </summary>
        private void RegisterEventHandlers()
        {
            // 例如：_eventHandlers["ItemCreated"] = HandleItemCreatedEvent;
        }

        /// <summary>
        /// 初始化物品模板
        /// </summary>
        private async Task InitializeItemTemplates()
        {
            // 这里可以从配置文件或数据库加载默认的物品模板
            Logger.LogInformation("加载物品模板...");

            try
            {
                // 加载全局物品配置
                var globalConfig = _itemConfigManager.GlobalConfig;
                Logger.LogInformation($"全局物品配置加载完成，默认最大堆叠数量：{globalConfig.DefaultMaxStackSize}");

                // 加载所有物品配置
                var itemConfigs = _itemConfigManager.GetAllItemConfigs();
                Logger.LogInformation($"成功加载 {itemConfigs.Count} 个物品配置");

                // 根据物品配置创建物品模板
                foreach (var config in itemConfigs)
                {
                    var templateId = config.TemplateId;
                    var template = new ItemTemplate(templateId, config.Name);

                    // 使用反射或属性设置方法设置模板属性
                    // 基本属性（描述，图标，类型等）
                    SetTemplateProperties(template, config);

                    // 注册模板
                    RegisterItemTemplate(templateId, template);
                    Logger.LogDebug($"创建物品模板：{templateId} - {template.Name}");
                }

                Logger.LogInformation($"成功创建 {_itemTemplates.Count} 个物品模板");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "加载物品模板失败");
            }
        }

        /// <summary>
        /// 设置模板属性
        /// </summary>
        /// <param name="template">物品模板</param>
        /// <param name="config">物品配置</param>
        private void SetTemplateProperties(ItemTemplate template, ItemConfig config)
        {
            // 设置基本属性
            SetAttributeValue(template, "Description", config.Description);
            SetAttributeValue(template, "IconPath", config.IconPath);
            SetAttributeValue(template, "Type", config.Type);
            SetAttributeValue(template, "Rarity", config.Rarity);
            SetAttributeValue(template, "Value", config.Value);
            SetAttributeValue(template, "Weight", config.Weight);

            // 设置堆叠属性
            SetAttributeValue(template, "IsStackable", config.IsStackable);
            SetAttributeValue(template, "MaxStackSize", config.MaxStackSize);

            // 设置使用和装备属性
            SetAttributeValue(template, "IsUsable", config.IsUsable);
            SetAttributeValue(template, "IsEquippable", config.IsEquippable);

            if (config.IsEquippable)
            {
                SetAttributeValue(template, "EquipSlot", config.EquipSlot);

                // 设置装备属性
                foreach (var stat in config.EquipmentStats)
                {
                    template.SetAttribute($"Stat.{stat.Key}", stat.Value);
                }
            }

            // 设置其他自定义属性
            foreach (var key in config.GetCustomPropertyKeys())
            {
                var value = config.GetCustomProperty(key);
                if (value != null)
                {
                    template.SetAttribute(key, value);
                }
            }
        }

        /// <summary>
        /// 设置属性值，使用反射或直接设置
        /// </summary>
        /// <param name="template">物品模板</param>
        /// <param name="propertyName">属性名</param>
        /// <param name="value">属性值</param>
        private void SetAttributeValue(ItemTemplate template, string propertyName, object value)
        {
            var property = typeof(ItemTemplate).GetProperty(propertyName);
            if (property != null && property.CanWrite)
            {
                property.SetValue(template, value);
            }
            else
            {
                // 如果无法直接设置属性，使用SetAttribute方法
                template.SetAttribute(propertyName, value);
            }
        }

        /// <summary>
        /// 创建物品
        /// </summary>
        /// <param name="templateId">物品模板ID</param>
        /// <param name="stackSize">堆叠数量</param>
        /// <returns>物品ID</returns>
        public async Task<Guid> CreateItemAsync(string templateId, int stackSize = 1)
        {
            try
            {
                var item = _itemManager.CreateItem(templateId, stackSize);
                if (item == null)
                {
                    Logger.LogWarning($"创建物品失败：无法基于模板 {templateId} 创建物品");
                    return Guid.Empty;
                }

                // 这里可以发布物品创建事件
                // await EventManager.PublishAsync(new ItemCreatedEvent(item.Id, templateId, stackSize));

                return item.Id;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"创建物品异常：{ex.Message}");
                return Guid.Empty;
            }
        }

        /// <summary>
        /// 获取物品
        /// </summary>
        /// <param name="itemId">物品ID</param>
        /// <returns>物品实例</returns>
        public Task<IItem> GetItemAsync(Guid itemId)
        {
            return Task.FromResult(_itemManager.FindItem(itemId));
        }

        /// <summary>
        /// 获取物品位置
        /// </summary>
        /// <param name="itemId">物品ID</param>
        /// <returns>物品位置</returns>
        public Task<ItemLocation> GetItemLocationAsync(Guid itemId)
        {
            return Task.FromResult(_itemManager.GetItemLocation(itemId));
        }

        /// <summary>
        /// 更新物品位置
        /// </summary>
        /// <param name="itemId">物品ID</param>
        /// <param name="location">新位置</param>
        /// <returns>是否更新成功</returns>
        public Task<bool> UpdateItemLocationAsync(Guid itemId, ItemLocation location)
        {
            bool result = _itemManager.UpdateItemLocation(itemId, location);

            if (result)
            {
                // 可以发布位置更新事件
                // await EventManager.PublishAsync(new ItemLocationChangedEvent(itemId, location));
            }

            return Task.FromResult(result);
        }

        /// <summary>
        /// 设置物品堆叠数量
        /// </summary>
        /// <param name="itemId">物品ID</param>
        /// <param name="newStackSize">新的堆叠数量</param>
        /// <returns>是否设置成功</returns>
        public Task<bool> SetItemStackSizeAsync(Guid itemId, int newStackSize)
        {
            return Task.FromResult(_itemManager.SetItemStackSize(itemId, newStackSize));
        }

        /// <summary>
        /// 拆分物品堆叠
        /// </summary>
        /// <param name="itemId">物品ID</param>
        /// <param name="amount">拆分数量</param>
        /// <returns>新物品ID</returns>
        public Task<Guid> SplitItemStackAsync(Guid itemId, int amount)
        {
            return Task.FromResult(_itemManager.SplitItemStack(itemId, amount));
        }

        /// <summary>
        /// 合并物品堆叠
        /// </summary>
        /// <param name="sourceItemId">源物品ID</param>
        /// <param name="targetItemId">目标物品ID</param>
        /// <returns>是否合并成功</returns>
        public Task<bool> MergeItemStacksAsync(Guid sourceItemId, Guid targetItemId)
        {
            return Task.FromResult(_itemManager.MergeItemStacks(sourceItemId, targetItemId));
        }

        /// <summary>
        /// 删除物品
        /// </summary>
        /// <param name="itemId">物品ID</param>
        /// <param name="reason">删除原因</param>
        /// <returns>是否删除成功</returns>
        public Task<bool> DeleteItemAsync(Guid itemId, string reason = "")
        {
            bool result = _itemManager.DeleteItem(itemId);

            if (result)
            {
                Logger.LogInformation($"删除物品 {itemId} 成功，原因：{reason}");
                // 可以发布物品删除事件
                // await EventManager.PublishAsync(new ItemDeletedEvent(itemId, reason));
            }

            return Task.FromResult(result);
        }

        /// <summary>
        /// 使用物品
        /// </summary>
        /// <param name="itemId">物品ID</param>
        /// <param name="characterId">角色ID</param>
        /// <param name="targetId">目标ID</param>
        /// <returns>使用结果</returns>
        public async Task<(bool success, string message)> UseItemAsync(Guid itemId, Guid characterId, Guid? targetId = null)
        {
            try
            {
                bool success = _itemManager.UseItem(itemId, characterId, targetId);
                string message = success ? "物品使用成功" : "物品使用失败";

                if (success)
                {
                    // 可以发布物品使用事件
                    // await EventManager.PublishAsync(new ItemUsedEvent(itemId, characterId, targetId));
                }

                return (success, message);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"使用物品 {itemId} 异常");
                return (false, $"物品使用出错：{ex.Message}");
            }
        }

        /// <summary>
        /// 设置物品属性
        /// </summary>
        /// <param name="itemId">物品ID</param>
        /// <param name="key">属性键</param>
        /// <param name="value">属性值</param>
        /// <returns>是否设置成功</returns>
        public Task<bool> SetItemAttributeAsync(Guid itemId, string key, object value)
        {
            return Task.FromResult(_itemManager.SetItemAttribute(itemId, key, value));
        }

        /// <summary>
        /// 获取物品属性
        /// </summary>
        /// <param name="itemId">物品ID</param>
        /// <param name="key">属性键</param>
        /// <returns>属性值</returns>
        public Task<object> GetItemAttributeAsync(Guid itemId, string key)
        {
            return Task.FromResult(_itemManager.GetItemAttribute(itemId, key));
        }

        /// <summary>
        /// 获取角色物品
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <returns>物品ID列表</returns>
        public Task<IReadOnlyList<Guid>> GetCharacterItemsAsync(Guid characterId)
        {
            var items = _itemManager.GetItemsByOwner(characterId);
            return Task.FromResult<IReadOnlyList<Guid>>(items);
        }

        /// <summary>
        /// 保存物品数据
        /// </summary>
        /// <returns>序列化的数据</returns>
        public Task<string> SaveItemDataAsync()
        {
            try
            {
                // 实际实现应该调用 ItemManager 的批量保存方法
                return Task.FromResult("物品数据序列化示例");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "保存物品数据异常");
                return Task.FromResult(string.Empty);
            }
        }

        /// <summary>
        /// 加载物品数据
        /// </summary>
        /// <param name="data">序列化的数据</param>
        /// <returns>是否加载成功</returns>
        public Task<bool> LoadItemDataAsync(string data)
        {
            try
            {
                if (string.IsNullOrEmpty(data))
                {
                    Logger.LogWarning("物品数据为空，无法加载");
                    return Task.FromResult(false);
                }

                // 实际实现应该调用 ItemManager 的批量加载方法
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "加载物品数据异常");
                return Task.FromResult(false);
            }
        }

        /// <summary>
        /// 根据模板ID获取物品ID列表
        /// </summary>
        /// <param name="characterId">角色ID（可选）</param>
        /// <param name="templateId">模板ID</param>
        /// <returns>物品ID列表</returns>
        public Task<IReadOnlyList<Guid>> GetItemsByTemplateIdAsync(Guid characterId, string templateId)
        {
            var items = _itemManager.FindItemsByTemplate(templateId)
                .Where(item => _itemManager.GetItemLocation(item.Id).OwnerId == characterId)
                .Select(item => item.Id)
                .ToList();

            return Task.FromResult<IReadOnlyList<Guid>>(items);
        }

        /// <summary>
        /// 注册物品模板
        /// </summary>
        /// <param name="templateId">模板ID</param>
        /// <param name="template">模板实例</param>
        public void RegisterItemTemplate(string templateId, IItemTemplate template)
        {
            _itemManager.RegisterItemTemplate(templateId, template);
        }

        /// <summary>
        /// 注册多个物品模板
        /// </summary>
        /// <param name="templates">模板字典</param>
        public void RegisterItemTemplates(IDictionary<string, IItemTemplate> templates)
        {
            if (templates == null)
            {
                throw new ArgumentNullException(nameof(templates));
            }

            foreach (var template in templates)
            {
                RegisterItemTemplate(template.Key, template.Value);
            }
        }

        /// <summary>
        /// 创建物品模板
        /// </summary>
        /// <param name="templateId">模板ID</param>
        /// <param name="name">模板名称</param>
        /// <returns>是否创建成功</returns>
        public Task<bool> CreateItemTemplateAsync(string templateId, string name)
        {
            try
            {
                if (string.IsNullOrEmpty(templateId) || string.IsNullOrEmpty(name))
                {
                    Logger.LogWarning("创建物品模板失败：模板ID或名称为空");
                    return Task.FromResult(false);
                }

                var template = new ItemTemplate(templateId, name);
                _itemManager.RegisterItemTemplate(templateId, template);

                Logger.LogInformation($"创建物品模板成功：{templateId}, {name}");
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"创建物品模板异常：{templateId}");
                return Task.FromResult(false);
            }
        }

        /// <summary>
        /// 获取物品模板
        /// </summary>
        /// <param name="templateId">模板ID</param>
        /// <returns>物品模板</returns>
        public Task<IItemTemplate> GetItemTemplateAsync(string templateId)
        {
            return Task.FromResult(_itemManager.GetItemTemplate(templateId));
        }

        /// <summary>
        /// 设置模板属性
        /// </summary>
        /// <param name="templateId">模板ID</param>
        /// <param name="key">属性键</param>
        /// <param name="value">属性值</param>
        /// <returns>是否设置成功</returns>
        public Task<bool> SetTemplateAttributeAsync(string templateId, string key, object value)
        {
            var template = _itemManager.GetItemTemplate(templateId);
            if (template == null)
            {
                Logger.LogWarning($"设置模板属性失败：找不到模板 {templateId}");
                return Task.FromResult(false);
            }

            try
            {

                template.SetAttribute(key, value);

                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"设置模板 {templateId} 的属性 {key} 时出错");
                return Task.FromResult(false);
            }
        }

        /// <summary>
        /// 获取模板属性
        /// </summary>
        /// <param name="templateId">模板ID</param>
        /// <param name="key">属性键</param>
        /// <returns>属性值</returns>
        public Task<object> GetTemplateAttributeAsync(string templateId, string key)
        {
            var template = _itemManager.GetItemTemplate(templateId);
            if (template == null)
            {
                Logger.LogWarning($"获取模板属性失败：找不到模板 {templateId}");
                return Task.FromResult<object>(null);
            }

            try
            {
                // 获取自定义属性
                return Task.FromResult(template.GetAttribute(key));
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"获取模板 {templateId} 的属性 {key} 时出错");
                return Task.FromResult<object>(null);
            }
        }
    }
}