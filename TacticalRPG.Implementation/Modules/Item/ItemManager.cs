using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using TacticalRPG.Core.Modules.Item;

namespace TacticalRPG.Implementation.Modules.Item
{
    /// <summary>
    /// 物品管理器实现类 - 提供中层物品业务逻辑
    /// </summary>
    public class ItemManager : IItemManager
    {
        private readonly Dictionary<Guid, IItemTemplate> _itemTemplates = new Dictionary<Guid, IItemTemplate>();
        private readonly ILogger<ItemManager> _logger;
        private readonly IItemFactory _itemFactory;
        private readonly ItemRegistry _itemRegistry;

        /// <summary>
        /// 物品工厂实例
        /// </summary>
        public IItemFactory ItemFactory => _itemFactory;

        /// <summary>
        /// 注册的物品模板
        /// </summary>
        public Dictionary<Guid, IItemTemplate> ItemTemplates => _itemTemplates;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="itemFactory">物品工厂</param>
        /// <param name="logger">日志记录器</param>
        public ItemManager(IItemFactory itemFactory, ILogger<ItemManager> logger)
        {
            _itemFactory = itemFactory ?? throw new ArgumentNullException(nameof(itemFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _itemRegistry = new ItemRegistry(logger);
        }

        /// <summary>
        /// 注册物品模板
        /// </summary>
        /// <param name="templateId">模板ID</param>
        /// <param name="itemTemplate">物品模板</param>
        public void RegisterItemTemplate(Guid templateId, IItemTemplate itemTemplate)
        {
            if (templateId == Guid.Empty)
            {
                _logger.LogWarning("注册物品模板失败：模板ID为空");
                return;
            }

            if (itemTemplate == null)
            {
                _logger.LogWarning($"注册物品模板失败：模板为空，模板ID：{templateId}");
                return;
            }

            if (_itemTemplates.ContainsKey(templateId))
            {
                _logger.LogWarning($"物品模板已存在，将被覆盖：{templateId}");
            }

            _itemTemplates[templateId] = itemTemplate;
            _logger.LogInformation($"注册物品模板成功：{templateId}，名称：{itemTemplate.Name}");
        }

        /// <summary>
        /// 获取物品模板
        /// </summary>
        /// <param name="templateId">模板ID</param>
        /// <returns>物品模板</returns>
        public IItemTemplate GetItemTemplate(Guid templateId)
        {
            if (templateId == Guid.Empty)
            {
                _logger.LogWarning("获取物品模板失败：模板ID为空");
                return null;
            }

            if (_itemTemplates.TryGetValue(templateId, out var template))
            {
                return template;
            }

            _logger.LogWarning($"物品模板 {templateId} 不存在");
            return null;
        }

        /// <summary>
        /// 创建物品实例
        /// </summary>
        /// <param name="templateId">模板ID</param>
        /// <param name="stackSize">堆叠数量</param>
        /// <returns>创建的物品实例</returns>
        public IItem CreateItem(Guid templateId, int stackSize = 1)
        {
            var template = GetItemTemplate(templateId);
            if (template == null)
            {
                _logger.LogWarning($"创建物品失败：找不到模板 {templateId}");
                return null;
            }

            var item = _itemFactory.CreateFromTemplate(templateId, stackSize);
            if (item != null)
            {
                // 注册物品到注册表
                _itemRegistry.RegisterItem(item);

                // 设置初始位置为未分配
                _itemRegistry.UpdateItemLocation(item.Id, ItemLocation.Unassigned);

                _logger.LogDebug($"创建物品成功：{item.Name}，ID：{item.Id}，堆叠数量：{stackSize}");
            }

            return item;
        }

        /// <summary>
        /// 查找物品
        /// </summary>
        /// <param name="itemId">物品ID</param>
        /// <returns>物品实例</returns>
        public IItem FindItem(Guid itemId)
        {
            return _itemRegistry.GetItem(itemId);
        }

        /// <summary>
        /// 根据模板ID查找物品
        /// </summary>
        /// <param name="templateId">模板ID</param>
        /// <returns>物品实例列表</returns>
        public List<IItem> FindItemsByTemplate(Guid templateId)
        {
            var result = new List<IItem>();

            foreach (var item in _itemRegistry.GetItems().Values)
            {
                if (item.TemplateId == templateId)
                {
                    result.Add(item);
                }
            }

            return result;
        }

        /// <summary>
        /// 根据所有者ID查找物品
        /// </summary>
        /// <param name="ownerId">所有者ID</param>
        /// <returns>物品ID列表</returns>
        public List<Guid> GetItemsByOwner(Guid ownerId)
        {
            return _itemRegistry.GetItemsByOwner(ownerId).ToList();
        }

        /// <summary>
        /// 使用物品
        /// </summary>
        /// <param name="itemId">物品ID</param>
        /// <param name="characterId">角色ID</param>
        /// <param name="targetId">目标ID</param>
        /// <returns>是否使用成功</returns>
        public bool UseItem(Guid itemId, Guid characterId, Guid? targetId = null)
        {
            var item = _itemRegistry.GetItem(itemId);
            if (item == null)
            {
                _logger.LogWarning($"使用物品失败：找不到物品 {itemId}");
                return false;
            }

            // 验证物品是否属于角色
            var location = _itemRegistry.GetItemLocation(itemId);
            if (location.OwnerId != characterId)
            {
                _logger.LogWarning($"使用物品失败：物品 {itemId} 不属于角色 {characterId}");
                return false;
            }

            // 检查物品是否可用
            if (!item.IsUsable)
            {
                _logger.LogWarning($"使用物品失败：物品 {item.Name} 不可使用");
                return false;
            }

            // 使用物品
            var (success, message) = item.Use(characterId, targetId);

            // 如果使用成功且是消耗品，减少堆叠数量或删除
            if (success && item.IsConsumable)
            {
                if (item.StackSize > 1)
                {
                    item.StackSize -= 1;
                    _logger.LogDebug($"物品 {item.Name} 使用后剩余数量：{item.StackSize}");
                }
                else
                {
                    // 物品已用完，删除它
                    _itemRegistry.RemoveItem(itemId);
                    _logger.LogDebug($"物品 {item.Name} 使用后已消耗完");
                }
            }

            _logger.LogInformation($"物品使用{(success ? "成功" : "失败")}：{message}");
            return success;
        }

        /// <summary>
        /// 加载物品数据
        /// </summary>
        /// <param name="itemData">物品数据</param>
        /// <returns>加载的物品</returns>
        public IItem LoadItemData(string itemData)
        {
            if (string.IsNullOrEmpty(itemData))
            {
                _logger.LogWarning("加载物品数据失败：数据为空");
                return null;
            }

            try
            {
                var itemInfo = JsonSerializer.Deserialize<ItemSerializationInfo>(itemData);
                if (itemInfo == null)
                {
                    _logger.LogWarning("加载物品数据失败：反序列化结果为空");
                    return null;
                }

                // 尝试从模板创建物品
                if (itemInfo.TemplateId != Guid.Empty)
                {
                    var item = _itemFactory.CreateFromTemplate(itemInfo.TemplateId, itemInfo.StackSize);
                    if (item != null)
                    {
                        // 恢复自定义属性
                        if (itemInfo.Properties != null)
                        {
                            foreach (var pair in itemInfo.Properties)
                            {
                                item.SetProperty(pair.Key, pair.Value);
                            }
                        }

                        // 注册物品到注册表
                        _itemRegistry.RegisterItem(item);

                        return item;
                    }
                }

                // 如果无法从模板创建，则创建基本物品
                var config = new ItemConfig(
                    itemInfo.Id,
                    itemInfo.Name
                );

                config.SetName(itemInfo.Name);
                config.SetDescription(itemInfo.Description);
                config.SetType((ItemType)itemInfo.Type);
                config.SetRarity((ItemRarity)itemInfo.Rarity);
                config.SetWeight(itemInfo.Weight);
                config.SetValue((int)itemInfo.Value);
                config.SetIsStackable(itemInfo.IsStackable);

                var basicItem = new Item(config, itemInfo.StackSize);

                // 设置自定义属性
                if (itemInfo.Properties != null)
                {
                    foreach (var pair in itemInfo.Properties)
                    {
                        basicItem.SetProperty(pair.Key, pair.Value);
                    }
                }

                // 注册物品到注册表
                _itemRegistry.RegisterItem(basicItem);

                return basicItem;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "加载物品数据异常");
                return null;
            }
        }

        /// <summary>
        /// 保存物品数据
        /// </summary>
        /// <param name="item">物品实例</param>
        /// <returns>保存的物品数据</returns>
        public string SaveItemData(IItem item)
        {
            if (item == null)
            {
                _logger.LogWarning("保存物品数据失败：物品为空");
                return null;
            }

            try
            {
                var itemInfo = new ItemSerializationInfo
                {
                    Id = item.Id,
                    TemplateId = item.TemplateId,
                    Name = item.Name,
                    Description = item.Description,
                    Type = (int)item.Type,
                    Rarity = (int)item.Rarity,
                    Weight = item.Weight,
                    Value = item.Value,
                    IsStackable = item.IsStackable,
                    StackSize = item.StackSize,
                    Properties = new Dictionary<string, object>()
                };

                // 收集所有自定义属性
                foreach (var key in GetAllPropertyKeys(item))
                {
                    var value = item.GetProperty(key);
                    if (value != null)
                    {
                        itemInfo.Properties[key] = value;
                    }
                }

                var options = new JsonSerializerOptions { WriteIndented = true };
                return JsonSerializer.Serialize(itemInfo, options);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "保存物品数据异常");
                return null;
            }
        }

        /// <summary>
        /// 获取物品位置
        /// </summary>
        /// <param name="itemId">物品ID</param>
        /// <returns>物品位置</returns>
        public ItemLocation GetItemLocation(Guid itemId)
        {
            return _itemRegistry.GetItemLocation(itemId);
        }

        /// <summary>
        /// 更新物品位置
        /// </summary>
        /// <param name="itemId">物品ID</param>
        /// <param name="location">新位置</param>
        /// <returns>是否更新成功</returns>
        public bool UpdateItemLocation(Guid itemId, ItemLocation location)
        {
            return _itemRegistry.UpdateItemLocation(itemId, location);
        }

        /// <summary>
        /// 删除物品
        /// </summary>
        /// <param name="itemId">物品ID</param>
        /// <returns>是否删除成功</returns>
        public bool DeleteItem(Guid itemId)
        {
            return _itemRegistry.RemoveItem(itemId);
        }

        /// <summary>
        /// 获取物品属性
        /// </summary>
        /// <param name="itemId">物品ID</param>
        /// <param name="key">属性键</param>
        /// <returns>属性值</returns>
        public object GetItemAttribute(Guid itemId, string key)
        {
            var item = _itemRegistry.GetItem(itemId);
            if (item == null)
            {
                _logger.LogWarning($"获取物品属性失败：找不到物品 {itemId}");
                return null;
            }

            return item.GetProperty(key);
        }

        /// <summary>
        /// 设置物品属性
        /// </summary>
        /// <param name="itemId">物品ID</param>
        /// <param name="key">属性键</param>
        /// <param name="value">属性值</param>
        /// <returns>是否设置成功</returns>
        public bool SetItemAttribute(Guid itemId, string key, object value)
        {
            var item = _itemRegistry.GetItem(itemId);
            if (item == null)
            {
                _logger.LogWarning($"设置物品属性失败：找不到物品 {itemId}");
                return false;
            }

            item.SetProperty(key, value);
            return true;
        }

        /// <summary>
        /// 设置物品堆叠数量
        /// </summary>
        /// <param name="itemId">物品ID</param>
        /// <param name="stackSize">堆叠数量</param>
        /// <returns>是否设置成功</returns>
        public bool SetItemStackSize(Guid itemId, int stackSize)
        {
            var item = _itemRegistry.GetItem(itemId);
            if (item == null)
            {
                _logger.LogWarning($"设置物品堆叠数量失败：找不到物品 {itemId}");
                return false;
            }

            if (!item.IsStackable && stackSize > 1)
            {
                _logger.LogWarning($"设置物品堆叠数量失败：物品不可堆叠 {itemId}");
                return false;
            }

            if (stackSize < 0)
            {
                _logger.LogWarning($"设置物品堆叠数量失败：数量不能为负数 {stackSize}");
                return false;
            }

            if (stackSize > item.MaxStackSize)
            {
                _logger.LogWarning($"设置物品堆叠数量失败：数量超过最大堆叠数量 {stackSize} > {item.MaxStackSize}");
                return false;
            }

            if (stackSize == 0)
            {
                // 数量为0，直接删除物品
                return DeleteItem(itemId);
            }

            item.StackSize = stackSize;
            _logger.LogDebug($"设置物品堆叠数量成功：{itemId}，{stackSize}");
            return true;
        }

        /// <summary>
        /// 拆分物品堆叠
        /// </summary>
        /// <param name="itemId">物品ID</param>
        /// <param name="amount">拆分数量</param>
        /// <returns>拆分后的新物品ID</returns>
        public Guid SplitItemStack(Guid itemId, int amount)
        {
            var item = _itemRegistry.GetItem(itemId);
            if (item == null)
            {
                _logger.LogWarning($"拆分物品堆叠失败：找不到物品 {itemId}");
                return Guid.Empty;
            }

            if (!item.IsStackable)
            {
                _logger.LogWarning($"拆分物品堆叠失败：物品不可堆叠 {itemId}");
                return Guid.Empty;
            }

            if (amount <= 0 || amount >= item.StackSize)
            {
                _logger.LogWarning($"拆分物品堆叠失败：拆分数量无效 {amount}，当前数量 {item.StackSize}");
                return Guid.Empty;
            }

            var newItem = item.SplitStack(amount);
            if (newItem == null)
            {
                _logger.LogWarning($"拆分物品堆叠失败：拆分操作返回空值 {itemId}");
                return Guid.Empty;
            }

            // 注册新物品并继承位置
            var newItemId = _itemRegistry.RegisterItem(newItem);
            var location = _itemRegistry.GetItemLocation(itemId);
            _itemRegistry.UpdateItemLocation(newItemId, location);

            _logger.LogDebug($"拆分物品堆叠成功：原物品 {itemId}，新物品 {newItemId}，拆分数量 {amount}");
            return newItemId;
        }

        /// <summary>
        /// 合并物品堆叠
        /// </summary>
        /// <param name="sourceItemId">源物品ID</param>
        /// <param name="targetItemId">目标物品ID</param>
        /// <returns>是否合并成功</returns>
        public bool MergeItemStacks(Guid sourceItemId, Guid targetItemId)
        {
            var sourceItem = _itemRegistry.GetItem(sourceItemId);
            var targetItem = _itemRegistry.GetItem(targetItemId);

            if (sourceItem == null || targetItem == null)
            {
                _logger.LogWarning($"合并物品堆叠失败：找不到物品 源：{sourceItemId}，目标：{targetItemId}");
                return false;
            }

            if (!sourceItem.IsStackable || !targetItem.IsStackable)
            {
                _logger.LogWarning($"合并物品堆叠失败：物品不可堆叠 源：{sourceItemId}，目标：{targetItemId}");
                return false;
            }

            if (sourceItem.TemplateId != targetItem.TemplateId)
            {
                _logger.LogWarning($"合并物品堆叠失败：物品类型不同 源：{sourceItem.TemplateId}，目标：{targetItem.TemplateId}");
                return false;
            }

            var remainingItem = targetItem.MergeStack(sourceItem);
            if (remainingItem == null)
            {
                // 完全合并，删除源物品
                _itemRegistry.RemoveItem(sourceItemId);
                _logger.LogDebug($"合并物品堆叠成功：源物品 {sourceItemId} 完全合并到 {targetItemId}");
                return true;
            }
            else
            {
                // 部分合并，更新源物品
                _logger.LogDebug($"合并物品堆叠部分成功：源物品 {sourceItemId} 部分合并到 {targetItemId}，剩余 {remainingItem.StackSize}");
                return true;
            }
        }

        /// <summary>
        /// 清除所有物品模板
        /// </summary>
        public void ClearItemTemplates()
        {
            _itemTemplates.Clear();
            _logger.LogInformation("清除所有物品模板");
        }

        /// <summary>
        /// 获取物品的所有属性键
        /// </summary>
        /// <param name="item">物品实例</param>
        /// <returns>属性键集合</returns>
        private IEnumerable<string> GetAllPropertyKeys(IItem item)
        {
            // 这个是一个辅助方法，获取物品的所有自定义属性键
            // 实际实现可能需要依赖于Item类提供的方法
            // 这里仅作为示例，假设物品类有一个获取所有属性键的方法
            var keys = new List<string>
            {
                "Durability",
                "MaxDurability",
                "Level",
                "LevelRequirement",
                "IsUsable",
                "IsEquippable",
                "IsSellable",
                "IsDroppable",
                "IsTradable",
                "IsBound",
                "BindType",
                "Cooldown",
                "CooldownGroup",
                "UseEffect",
                "EquipSlot"
            };

            // 添加其他可能的属性键
            return keys;
        }
    }

    /// <summary>
    /// 物品序列化信息
    /// </summary>
    internal class ItemSerializationInfo
    {
        /// <summary>
        /// 物品ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 模板ID
        /// </summary>
        public Guid TemplateId { get; set; }

        /// <summary>
        /// 物品名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 物品描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 物品类型
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 物品稀有度
        /// </summary>
        public int Rarity { get; set; }

        /// <summary>
        /// 物品重量
        /// </summary>
        public float Weight { get; set; }

        /// <summary>
        /// 物品价值
        /// </summary>
        public float Value { get; set; }

        /// <summary>
        /// 是否可堆叠
        /// </summary>
        public bool IsStackable { get; set; }

        /// <summary>
        /// 堆叠数量
        /// </summary>
        public int StackSize { get; set; }

        /// <summary>
        /// 自定义属性
        /// </summary>
        public Dictionary<string, object> Properties { get; set; }
    }
}