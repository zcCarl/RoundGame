using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using TacticalRPG.Core.Modules.Inventory;

namespace TacticalRPG.Implementation.Modules.Inventory
{
    /// <summary>
    /// 物品管理器实现类
    /// </summary>
    public class ItemManager : IItemManager
    {
        private readonly Dictionary<string, IItem> _itemTemplates = new Dictionary<string, IItem>();
        private readonly Dictionary<Guid, IInventory> _inventories = new Dictionary<Guid, IInventory>();
        private readonly ILogger<ItemManager> _logger;
        private readonly IItemFactory _itemFactory;

        /// <summary>
        /// 物品工厂实例
        /// </summary>
        public IItemFactory ItemFactory => _itemFactory;

        /// <summary>
        /// 注册的物品模板
        /// </summary>
        public Dictionary<string, IItem> ItemTemplates => _itemTemplates;

        /// <summary>
        /// 注册的库存列表
        /// </summary>
        public Dictionary<Guid, IInventory> Inventories => _inventories;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="itemFactory">物品工厂</param>
        /// <param name="logger">日志记录器</param>
        public ItemManager(IItemFactory itemFactory, ILogger<ItemManager> logger)
        {
            _itemFactory = itemFactory ?? throw new ArgumentNullException(nameof(itemFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// 注册物品模板
        /// </summary>
        /// <param name="templateId">模板ID</param>
        /// <param name="itemTemplate">物品模板</param>
        public void RegisterItemTemplate(string templateId, IItem itemTemplate)
        {
            if (string.IsNullOrEmpty(templateId))
            {
                throw new ArgumentException("物品模板ID不能为空", nameof(templateId));
            }

            if (itemTemplate == null)
            {
                throw new ArgumentNullException(nameof(itemTemplate));
            }

            if (_itemTemplates.ContainsKey(templateId))
            {
                _logger.LogWarning($"物品模板 {templateId} 已存在，将被覆盖");
            }

            _itemTemplates[templateId] = itemTemplate;
            _logger.LogInformation($"注册物品模板 {templateId}: {itemTemplate.Name}");
        }

        /// <summary>
        /// 获取物品模板
        /// </summary>
        /// <param name="templateId">模板ID</param>
        /// <returns>物品模板</returns>
        public IItem GetItemTemplate(string templateId)
        {
            if (string.IsNullOrEmpty(templateId))
            {
                throw new ArgumentException("物品模板ID不能为空", nameof(templateId));
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
        public IItem CreateItem(string templateId, int stackSize = 1)
        {
            var template = GetItemTemplate(templateId);
            if (template == null)
            {
                return null;
            }

            var item = _itemFactory.CreateItem(template, stackSize);
            _logger.LogDebug($"创建物品 {item.Name} (ID: {item.Id})，堆叠数量: {stackSize}");
            return item;
        }

        /// <summary>
        /// 注册库存
        /// </summary>
        /// <param name="inventory">库存</param>
        public void RegisterInventory(IInventory inventory)
        {
            if (inventory == null)
            {
                throw new ArgumentNullException(nameof(inventory));
            }

            if (_inventories.ContainsKey(inventory.Id))
            {
                _logger.LogWarning($"库存 {inventory.Id} 已存在，将被覆盖");
            }

            _inventories[inventory.Id] = inventory;
            _logger.LogInformation($"注册库存 {inventory.Id}: {inventory.Name}，拥有者: {inventory.OwnerId}");
        }

        /// <summary>
        /// 注销库存
        /// </summary>
        /// <param name="inventoryId">库存ID</param>
        public void UnregisterInventory(Guid inventoryId)
        {
            if (_inventories.ContainsKey(inventoryId))
            {
                _inventories.Remove(inventoryId);
                _logger.LogInformation($"注销库存 {inventoryId}");
            }
            else
            {
                _logger.LogWarning($"尝试注销不存在的库存 {inventoryId}");
            }
        }

        /// <summary>
        /// 获取库存
        /// </summary>
        /// <param name="inventoryId">库存ID</param>
        /// <returns>库存</returns>
        public IInventory GetInventory(Guid inventoryId)
        {
            if (_inventories.TryGetValue(inventoryId, out var inventory))
            {
                return inventory;
            }

            _logger.LogWarning($"库存 {inventoryId} 不存在");
            return null;
        }

        /// <summary>
        /// 获取角色的所有库存
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <returns>库存列表</returns>
        public List<IInventory> GetCharacterInventories(Guid characterId)
        {
            var result = new List<IInventory>();
            foreach (var inventory in _inventories.Values)
            {
                if (inventory.OwnerId == characterId)
                {
                    result.Add(inventory);
                }
            }

            return result;
        }

        /// <summary>
        /// 创建新库存
        /// </summary>
        /// <param name="ownerId">所有者ID</param>
        /// <param name="name">库存名称</param>
        /// <param name="capacity">容量</param>
        /// <param name="inventoryType">库存类型</param>
        /// <param name="maxWeight">最大重量</param>
        /// <returns>创建的库存实例</returns>
        public IInventory CreateInventory(
            Guid ownerId,
            string name,
            int capacity,
            InventoryType inventoryType = InventoryType.Normal,
            float maxWeight = 0)
        {
            var inventoryId = Guid.NewGuid();
            var inventory = new Inventory(inventoryId, ownerId, inventoryType, capacity, maxWeight);

            if (!string.IsNullOrEmpty(name))
            {
                inventory.SetName(name);
            }

            RegisterInventory(inventory);
            _logger.LogInformation($"创建库存 {inventoryId}: {inventory.Name}，拥有者: {ownerId}");

            return inventory;
        }

        /// <summary>
        /// 在两个库存之间移动物品
        /// </summary>
        /// <param name="sourceInventoryId">源库存ID</param>
        /// <param name="sourceSlotIndex">源槽索引</param>
        /// <param name="targetInventoryId">目标库存ID</param>
        /// <param name="targetSlotIndex">目标槽索引</param>
        /// <param name="amount">移动数量</param>
        /// <returns>是否移动成功</returns>
        public bool MoveItemBetweenInventories(
            Guid sourceInventoryId,
            int sourceSlotIndex,
            Guid targetInventoryId,
            int targetSlotIndex,
            int amount = 0)
        {
            if (!_inventories.TryGetValue(sourceInventoryId, out var sourceInventory))
            {
                _logger.LogWarning($"源库存 {sourceInventoryId} 不存在");
                return false;
            }

            if (!_inventories.TryGetValue(targetInventoryId, out var targetInventory))
            {
                _logger.LogWarning($"目标库存 {targetInventoryId} 不存在");
                return false;
            }

            var sourceSlot = sourceInventory.GetSlot(sourceSlotIndex);
            if (sourceSlot == null || sourceSlot.IsEmpty || sourceSlot.IsLocked)
            {
                _logger.LogWarning($"源槽位 {sourceSlotIndex} 为空或已锁定");
                return false;
            }

            var targetSlot = targetInventory.GetSlot(targetSlotIndex);
            if (targetSlot == null || targetSlot.IsLocked)
            {
                _logger.LogWarning($"目标槽位 {targetSlotIndex} 不存在或已锁定");
                return false;
            }

            if (amount <= 0 || amount >= sourceSlot.Item.StackSize)
            {
                // 移动全部物品
                IItem item = sourceInventory.RemoveItem(sourceSlotIndex);
                if (item == null)
                {
                    _logger.LogWarning($"从源库存移除物品失败");
                    return false;
                }

                int addedIndex = targetInventory.AddItem(item, targetSlotIndex);
                if (addedIndex == -1)
                {
                    // 添加失败，归还物品
                    sourceInventory.AddItem(item, sourceSlotIndex);
                    _logger.LogWarning($"移动物品失败：无法添加到目标槽位");
                    return false;
                }

                _logger.LogInformation($"移动物品 {item.Name} 从库存 {sourceInventoryId} 槽位 {sourceSlotIndex} 到库存 {targetInventoryId} 槽位 {targetSlotIndex}");
                return true;
            }
            else
            {
                // 移动部分物品
                IItem item = sourceInventory.RemoveItem(sourceSlotIndex, amount);
                if (item == null)
                {
                    _logger.LogWarning($"从源库存移除部分物品失败");
                    return false;
                }

                int addedAmount = targetInventory.AddItem(item, amount, targetSlotIndex);
                if (addedAmount < amount)
                {
                    // 添加不完全，归还物品
                    int notAddedAmount = amount - addedAmount;
                    sourceInventory.AddItem(item.Clone(notAddedAmount), sourceSlotIndex);
                    _logger.LogWarning($"移动物品部分失败：仅移动了 {addedAmount}/{amount} 个物品");
                    return addedAmount > 0;
                }

                _logger.LogInformation($"移动 {amount} 个 {item.Name} 从库存 {sourceInventoryId} 槽位 {sourceSlotIndex} 到库存 {targetInventoryId} 槽位 {targetSlotIndex}");
                return true;
            }
        }

        /// <summary>
        /// 交换两个库存中的物品
        /// </summary>
        /// <param name="firstInventoryId">第一个库存ID</param>
        /// <param name="firstSlotIndex">第一个槽索引</param>
        /// <param name="secondInventoryId">第二个库存ID</param>
        /// <param name="secondSlotIndex">第二个槽索引</param>
        /// <returns>是否交换成功</returns>
        public bool SwapItemsBetweenInventories(
            Guid firstInventoryId,
            int firstSlotIndex,
            Guid secondInventoryId,
            int secondSlotIndex)
        {
            if (!_inventories.TryGetValue(firstInventoryId, out var firstInventory))
            {
                _logger.LogWarning($"第一个库存 {firstInventoryId} 不存在");
                return false;
            }

            if (!_inventories.TryGetValue(secondInventoryId, out var secondInventory))
            {
                _logger.LogWarning($"第二个库存 {secondInventoryId} 不存在");
                return false;
            }

            var firstSlot = firstInventory.GetSlot(firstSlotIndex);
            var secondSlot = secondInventory.GetSlot(secondSlotIndex);

            if (firstSlot == null || secondSlot == null)
            {
                _logger.LogWarning("无效的槽位索引");
                return false;
            }

            if (firstSlot.IsLocked || secondSlot.IsLocked)
            {
                _logger.LogWarning("槽位已锁定，无法交换物品");
                return false;
            }

            bool firstEmpty = firstSlot.IsEmpty;
            bool secondEmpty = secondSlot.IsEmpty;

            // 处理空槽位情况
            if (firstEmpty && secondEmpty)
            {
                // 两个槽位都为空，不需要交换
                return true;
            }
            else if (firstEmpty)
            {
                // 第一个槽位为空，直接移动第二个槽位的物品到第一个槽位
                return MoveItemBetweenInventories(secondInventoryId, secondSlotIndex, firstInventoryId, firstSlotIndex);
            }
            else if (secondEmpty)
            {
                // 第二个槽位为空，直接移动第一个槽位的物品到第二个槽位
                return MoveItemBetweenInventories(firstInventoryId, firstSlotIndex, secondInventoryId, secondSlotIndex);
            }

            // 两个槽位都不为空，需要交换物品
            IItem firstItem = firstInventory.RemoveItem(firstSlotIndex);
            IItem secondItem = secondInventory.RemoveItem(secondSlotIndex);

            if (firstItem == null || secondItem == null)
            {
                // 恢复原状
                if (firstItem != null)
                {
                    firstInventory.AddItem(firstItem, firstSlotIndex);
                }
                if (secondItem != null)
                {
                    secondInventory.AddItem(secondItem, secondSlotIndex);
                }
                _logger.LogWarning("从槽位移除物品失败");
                return false;
            }

            // 检查是否可以放入对应槽位
            int secondToFirstResult = firstInventory.AddItem(secondItem, firstSlotIndex);
            if (secondToFirstResult == -1)
            {
                // 恢复原状
                firstInventory.AddItem(firstItem, firstSlotIndex);
                secondInventory.AddItem(secondItem, secondSlotIndex);
                _logger.LogWarning("第二个物品无法放入第一个槽位");
                return false;
            }

            int firstToSecondResult = secondInventory.AddItem(firstItem, secondSlotIndex);
            if (firstToSecondResult == -1)
            {
                // 恢复原状
                firstInventory.RemoveItem(firstSlotIndex);
                firstInventory.AddItem(firstItem, firstSlotIndex);
                secondInventory.AddItem(secondItem, secondSlotIndex);
                _logger.LogWarning("第一个物品无法放入第二个槽位");
                return false;
            }

            _logger.LogInformation($"交换物品成功：{firstItem.Name} 与 {secondItem.Name}");
            return true;
        }

        /// <summary>
        /// 使用物品
        /// </summary>
        /// <param name="inventoryId">库存ID</param>
        /// <param name="slotIndex">槽索引</param>
        /// <param name="targetId">目标ID</param>
        /// <returns>是否使用成功</returns>
        public bool UseItem(Guid inventoryId, int slotIndex, Guid targetId)
        {
            if (!_inventories.TryGetValue(inventoryId, out var inventory))
            {
                _logger.LogWarning($"库存 {inventoryId} 不存在");
                return false;
            }

            var (success, message) = inventory.UseItem(slotIndex, targetId);
            if (success)
            {
                _logger.LogInformation($"使用物品成功：{message}");
            }
            else
            {
                _logger.LogWarning($"使用物品失败：{message}");
            }

            return success;
        }

        /// <summary>
        /// 丢弃物品
        /// </summary>
        /// <param name="inventoryId">库存ID</param>
        /// <param name="slotIndex">槽索引</param>
        /// <param name="amount">丢弃数量</param>
        /// <returns>丢弃的物品</returns>
        public IItem DropItem(Guid inventoryId, int slotIndex, int amount = 0)
        {
            if (!_inventories.TryGetValue(inventoryId, out var inventory))
            {
                _logger.LogWarning($"库存 {inventoryId} 不存在");
                return null;
            }

            IItem item;
            if (amount <= 0)
            {
                item = inventory.RemoveItem(slotIndex);
            }
            else
            {
                item = inventory.RemoveItem(slotIndex, amount);
            }

            if (item != null)
            {
                _logger.LogInformation($"丢弃物品：{item.Name} x {item.StackSize}");
            }
            else
            {
                _logger.LogWarning($"丢弃物品失败：槽位 {slotIndex} 为空或已锁定");
            }

            return item;
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
                _logger.LogWarning("物品数据为空");
                return null;
            }

            try
            {
                var itemInfo = JsonSerializer.Deserialize<ItemSerializationInfo>(itemData);
                if (itemInfo == null)
                {
                    _logger.LogWarning("物品数据反序列化失败");
                    return null;
                }

                IItem item = null;

                // 根据物品类型创建不同的物品
                switch (itemInfo.Type)
                {
                    case ItemType.Equipment:
                        item = _itemFactory.CreateEquipment(
                            itemInfo.Name,
                            itemInfo.Description,
                            (EquipmentType)Enum.Parse(typeof(EquipmentType), itemInfo.SubType),
                            (ItemRarity)itemInfo.Rarity,
                            itemInfo.IsStackable,
                            itemInfo.Weight,
                            itemInfo.Value,
                            itemInfo.Properties.ContainsKey("ArmorValue") ? Convert.ToSingle(itemInfo.Properties["ArmorValue"]) : 0,
                            itemInfo.Properties.ContainsKey("DamageValue") ? Convert.ToSingle(itemInfo.Properties["DamageValue"]) : 0,
                            itemInfo.StackSize);
                        break;

                    case ItemType.Consumable:
                        item = _itemFactory.CreateConsumable(
                            itemInfo.Name,
                            itemInfo.Description,
                            (ItemRarity)itemInfo.Rarity,
                            itemInfo.IsStackable,
                            itemInfo.Weight,
                            itemInfo.Value,
                            itemInfo.Properties.ContainsKey("EffectType") ? itemInfo.Properties["EffectType"].ToString() : string.Empty,
                            itemInfo.Properties.ContainsKey("EffectValue") ? Convert.ToSingle(itemInfo.Properties["EffectValue"]) : 0,
                            itemInfo.StackSize);
                        break;

                    case ItemType.Material:
                        item = _itemFactory.CreateMaterial(
                            itemInfo.Name,
                            itemInfo.Description,
                            (ItemRarity)itemInfo.Rarity,
                            itemInfo.IsStackable,
                            itemInfo.Weight,
                            itemInfo.Value,
                            itemInfo.Properties.ContainsKey("MaterialType") ? itemInfo.Properties["MaterialType"].ToString() : string.Empty,
                            itemInfo.StackSize);
                        break;

                    case ItemType.QuestItem:
                        item = _itemFactory.CreateQuestItem(
                            itemInfo.Name,
                            itemInfo.Description,
                            (ItemRarity)itemInfo.Rarity,
                            itemInfo.Weight,
                            itemInfo.Value,
                            itemInfo.Properties.ContainsKey("QuestId") ? itemInfo.Properties["QuestId"].ToString() : string.Empty,
                            itemInfo.Properties.ContainsKey("IsLocked") && Convert.ToBoolean(itemInfo.Properties["IsLocked"]));
                        break;

                    default:
                        item = _itemFactory.CreateItem(
                            itemInfo.Name,
                            itemInfo.Description,
                            (ItemType)itemInfo.Type,
                            (ItemRarity)itemInfo.Rarity,
                            itemInfo.IsStackable,
                            itemInfo.Weight,
                            itemInfo.Value,
                            itemInfo.StackSize);
                        break;
                }

                if (item != null)
                {
                    // 设置自定义属性
                    foreach (var property in itemInfo.Properties)
                    {
                        if (!string.IsNullOrEmpty(property.Key))
                        {
                            item.SetProperty(property.Key, property.Value);
                        }
                    }

                    _logger.LogInformation($"成功加载物品：{item.Name}");
                }

                return item;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "加载物品数据时发生错误");
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
                _logger.LogWarning("物品为空，无法保存");
                return null;
            }

            try
            {
                var itemInfo = new ItemSerializationInfo
                {
                    Id = item.Id.ToString(),
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

                // 保存所有自定义属性
                var properties = item.GetAllProperties();
                if (properties != null)
                {
                    foreach (var property in properties)
                    {
                        itemInfo.Properties[property.Key] = property.Value;
                    }
                }

                string jsonData = JsonSerializer.Serialize(itemInfo);
                _logger.LogInformation($"成功保存物品数据：{item.Name}");
                return jsonData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "保存物品数据时发生错误");
                return null;
            }
        }

        /// <summary>
        /// 加载库存数据
        /// </summary>
        /// <param name="inventoryData">库存数据</param>
        /// <returns>加载的库存</returns>
        public IInventory LoadInventoryData(string inventoryData)
        {
            if (string.IsNullOrEmpty(inventoryData))
            {
                _logger.LogWarning("库存数据为空");
                return null;
            }

            try
            {
                var inventoryInfo = JsonSerializer.Deserialize<InventorySerializationInfo>(inventoryData);
                if (inventoryInfo == null)
                {
                    _logger.LogWarning("库存数据反序列化失败");
                    return null;
                }

                var inventoryId = Guid.Parse(inventoryInfo.Id);
                var ownerId = Guid.Parse(inventoryInfo.OwnerId);
                var inventory = new Inventory(
                    inventoryId,
                    ownerId,
                    (InventoryType)Enum.Parse(typeof(InventoryType), inventoryInfo.Type),
                    inventoryInfo.Capacity,
                    inventoryInfo.MaxWeight);

                inventory.SetName(inventoryInfo.Name);

                // 加载物品
                foreach (var slotInfo in inventoryInfo.Slots)
                {
                    if (slotInfo.Value?.ItemData != null)
                    {
                        var item = LoadItemData(slotInfo.Value.ItemData);
                        if (item != null)
                        {
                            inventory.AddItem(item, slotInfo.Key);

                            // 设置槽位属性
                            if (slotInfo.Value.IsLocked)
                            {
                                inventory.SetSlotLocked(slotInfo.Key, true);
                            }

                            if (!string.IsNullOrEmpty(slotInfo.Value.Label))
                            {
                                inventory.SetSlotLabel(slotInfo.Key, slotInfo.Value.Label);
                            }

                            if (slotInfo.Value.AcceptedItemType.HasValue)
                            {
                                inventory.SetSlotAcceptedItemType(slotInfo.Key, (ItemType)slotInfo.Value.AcceptedItemType.Value);
                            }
                        }
                    }
                }

                // 设置自定义属性
                foreach (var property in inventoryInfo.Properties)
                {
                    if (!string.IsNullOrEmpty(property.Key))
                    {
                        inventory.SetProperty(property.Key, property.Value);
                    }
                }

                RegisterInventory(inventory);
                _logger.LogInformation($"成功加载库存：{inventory.Name}");
                return inventory;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "加载库存数据时发生错误");
                return null;
            }
        }

        /// <summary>
        /// 保存库存数据
        /// </summary>
        /// <param name="inventory">库存实例</param>
        /// <returns>保存的库存数据</returns>
        public string SaveInventoryData(IInventory inventory)
        {
            if (inventory == null)
            {
                _logger.LogWarning("库存为空，无法保存");
                return null;
            }

            try
            {
                var inventoryInfo = new InventorySerializationInfo
                {
                    Id = inventory.Id.ToString(),
                    OwnerId = inventory.OwnerId.ToString(),
                    Name = inventory.Name,
                    Type = inventory.InventoryType,
                    Capacity = inventory.Capacity,
                    MaxWeight = inventory.MaxWeight ?? 0,
                    Slots = new Dictionary<int, SlotSerializationInfo>(),
                    Properties = new Dictionary<string, object>()
                };

                // 保存槽位信息
                foreach (var slot in inventory.Slots)
                {
                    var slotInfo = new SlotSerializationInfo
                    {
                        Index = slot.Index,
                        IsLocked = slot.IsLocked,
                        Label = slot.Label,
                        AcceptedItemType = slot.AcceptedItemType.HasValue ? (int)slot.AcceptedItemType.Value : null,
                        ItemData = slot.IsEmpty ? null : SaveItemData(slot.Item)
                    };

                    inventoryInfo.Slots[slot.Index] = slotInfo;
                }

                // 保存自定义属性
                var properties = inventory.GetAllProperties();
                if (properties != null)
                {
                    foreach (var property in properties)
                    {
                        inventoryInfo.Properties[property.Key] = property.Value;
                    }
                }

                string jsonData = JsonSerializer.Serialize(inventoryInfo);
                _logger.LogInformation($"成功保存库存数据：{inventory.Name}");
                return jsonData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "保存库存数据时发生错误");
                return null;
            }
        }

        /// <summary>
        /// 获取物品的全局唯一ID
        /// </summary>
        /// <param name="item">物品实例</param>
        /// <returns>物品的全局唯一ID</returns>
        public string GetItemGlobalId(IItem item)
        {
            if (item == null)
            {
                _logger.LogWarning("物品为空，无法获取全局ID");
                return null;
            }

            foreach (var inventory in _inventories.Values)
            {
                var slots = inventory.FindItemSlots(item.Id);
                if (slots != null && slots.Count > 0)
                {
                    // 全局ID格式：库存ID:槽位索引:物品ID
                    return $"{inventory.Id}:{slots[0]}:{item.Id}";
                }
            }

            // 未找到，仅返回物品ID
            return item.Id.ToString();
        }

        /// <summary>
        /// 根据全局唯一ID查找物品
        /// </summary>
        /// <param name="globalId">全局唯一ID</param>
        /// <returns>物品实例和所在库存信息的元组</returns>
        public (IInventory Inventory, int SlotIndex, IItem Item) FindItemByGlobalId(string globalId)
        {
            if (string.IsNullOrEmpty(globalId))
            {
                _logger.LogWarning("全局ID为空");
                return (null, -1, null);
            }

            try
            {
                // 尝试解析全局ID
                var parts = globalId.Split(':');
                if (parts.Length >= 3)
                {
                    // 解析库存ID
                    if (Guid.TryParse(parts[0], out Guid inventoryId))
                    {
                        var inventory = GetInventory(inventoryId);
                        if (inventory != null)
                        {
                            // 解析槽位索引
                            if (int.TryParse(parts[1], out int slotIndex))
                            {
                                var slot = inventory.GetSlot(slotIndex);
                                if (slot != null && !slot.IsEmpty)
                                {
                                    // 检查物品ID是否匹配
                                    if (Guid.TryParse(parts[2], out Guid itemId) && slot.Item.Id == itemId)
                                    {
                                        return (inventory, slotIndex, slot.Item);
                                    }
                                }
                            }
                        }
                    }
                }
                else if (parts.Length == 1 && Guid.TryParse(parts[0], out Guid itemId))
                {
                    // 仅包含物品ID，尝试在所有库存中查找
                    foreach (var inventory in _inventories.Values)
                    {
                        var slots = inventory.FindItemSlots(itemId);
                        if (slots != null && slots.Count > 0)
                        {
                            int slotIndex = slots[0];
                            var slot = inventory.GetSlot(slotIndex);
                            if (slot != null && !slot.IsEmpty && slot.Item.Id == itemId)
                            {
                                return (inventory, slotIndex, slot.Item);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"查找全局ID {globalId} 时发生错误");
            }

            _logger.LogWarning($"未找到全局ID为 {globalId} 的物品");
            return (null, -1, null);
        }

        /// <summary>
        /// 移除库存
        /// </summary>
        /// <param name="inventoryId">库存ID</param>
        /// <returns>是否成功移除</returns>
        public bool RemoveInventory(Guid inventoryId)
        {
            if (_inventories.TryGetValue(inventoryId, out var inventory))
            {
                _inventories.Remove(inventoryId);
                _logger.LogInformation($"移除库存 {inventoryId}: {inventory.Name}");
                return true;
            }

            _logger.LogWarning($"尝试移除不存在的库存 {inventoryId}");
            return false;
        }

        /// <summary>
        /// 清空所有库存
        /// </summary>
        public void ClearInventories()
        {
            _inventories.Clear();
            _logger.LogInformation("清空所有库存");
        }

        /// <summary>
        /// 清空所有物品模板
        /// </summary>
        public void ClearItemTemplates()
        {
            _itemTemplates.Clear();
            _logger.LogInformation("清空所有物品模板");
        }
    }

    /// <summary>
    /// 物品序列化信息
    /// </summary>
    internal class ItemSerializationInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Type { get; set; }
        public string SubType { get; set; }
        public int Rarity { get; set; }
        public float Weight { get; set; }
        public float Value { get; set; }
        public bool IsStackable { get; set; }
        public int StackSize { get; set; }
        public Dictionary<string, object> Properties { get; set; }
    }

    /// <summary>
    /// 槽位序列化信息
    /// </summary>
    internal class SlotSerializationInfo
    {
        public int Index { get; set; }
        public bool IsLocked { get; set; }
        public string Label { get; set; }
        public int? AcceptedItemType { get; set; }
        public string ItemData { get; set; }
    }

    /// <summary>
    /// 库存序列化信息
    /// </summary>
    internal class InventorySerializationInfo
    {
        public string Id { get; set; }
        public string OwnerId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int Capacity { get; set; }
        public float MaxWeight { get; set; }
        public Dictionary<int, SlotSerializationInfo> Slots { get; set; }
        public Dictionary<string, object> Properties { get; set; }
    }
}