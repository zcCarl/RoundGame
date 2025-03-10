using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TacticalRPG.Core.Framework;
using TacticalRPG.Core.Modules.Character;
using TacticalRPG.Core.Modules.Config;
using TacticalRPG.Core.Modules.Inventory;
using TacticalRPG.Core.Modules.Item;

namespace TacticalRPG.Implementation.Modules.Inventory
{
    /// <summary>
    /// 物品背包模块实现类
    /// </summary>
    public class InventoryModule : BaseGameModule, IInventoryModule
    {
        private readonly IItemModule _itemModule;
        private readonly IConfigManager _configManager;
        private readonly Dictionary<Guid, Dictionary<InventoryType, IInventory>> _characterInventories = new Dictionary<Guid, Dictionary<InventoryType, IInventory>>();
        private readonly Dictionary<Guid, IInventory> _inventories = new Dictionary<Guid, IInventory>();

        /// <summary>
        /// 获取物品模块引用
        /// </summary>
        public IItemModule ItemModule => _itemModule;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="gameSystem">游戏系统</param>
        /// <param name="logger">日志记录器</param>
        /// <param name="itemModule">物品模块</param>
        /// <param name="configManager">配置管理器</param>
        public InventoryModule(
            IGameSystem gameSystem,
            ILogger<InventoryModule> logger,
            IItemModule itemModule,
            IConfigManager configManager)
            : base(gameSystem, logger)
        {
            _itemModule = itemModule ?? throw new ArgumentNullException(nameof(itemModule));
            _configManager = configManager ?? throw new ArgumentNullException(nameof(configManager));
        }

        /// <summary>
        /// 获取模块名称
        /// </summary>
        public override string ModuleName => "物品背包模块";

        /// <summary>
        /// 获取模块优先级
        /// </summary>
        public override int Priority => 50; // 中等优先级

        /// <summary>
        /// 模块初始化
        /// </summary>
        public override async Task Initialize()
        {
            await base.Initialize();
            Logger.LogInformation("物品背包模块初始化完成");
        }

        /// <summary>
        /// 创建物品
        /// </summary>
        /// <param name="templateId">物品模板ID</param>
        /// <param name="stackSize">堆叠数量</param>
        /// <returns>创建的物品</returns>
        public IItem CreateItem(string templateId, int stackSize = 1)
        {
            return _itemModule.CreateItem(templateId, stackSize);
        }

        /// <summary>
        /// 创建角色背包
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <param name="capacity">容量</param>
        /// <param name="inventoryType">背包类型</param>
        /// <returns>创建的背包</returns>
        public async Task<IInventory> CreateCharacterInventoryAsync(Guid characterId, int capacity, InventoryType inventoryType = InventoryType.Normal)
        {
            if (!_characterInventories.ContainsKey(characterId))
            {
                _characterInventories[characterId] = new Dictionary<InventoryType, IInventory>();
            }

            if (_characterInventories[characterId].ContainsKey(inventoryType))
            {
                Logger.LogWarning($"角色 {characterId} 已存在类型为 {inventoryType} 的背包");
                return _characterInventories[characterId][inventoryType];
            }

            var inventoryId = await CreateInventoryAsync(characterId, inventoryType, capacity);
            var inventory = _inventories[inventoryId];
            _characterInventories[characterId][inventoryType] = inventory;
            Logger.LogInformation($"为角色 {characterId} 创建类型为 {inventoryType} 的背包");
            return inventory;
        }

        /// <summary>
        /// 获取角色的主背包
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <returns>角色的主背包</returns>
        public async Task<IInventory> GetCharacterMainInventoryAsync(Guid characterId)
        {
            return await GetInventoryAsync(characterId, InventoryType.Normal);
        }

        /// <summary>
        /// 获取角色的所有背包
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <returns>角色的所有背包</returns>
        public async Task<IReadOnlyList<IInventory>> GetCharacterInventoriesAsync(Guid characterId)
        {
            if (!_characterInventories.ContainsKey(characterId))
            {
                return new List<IInventory>();
            }

            return _characterInventories[characterId].Values.ToList();
        }

        /// <summary>
        /// 给角色添加物品
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <param name="templateId">物品模板ID</param>
        /// <param name="amount">数量</param>
        /// <param name="inventoryType">目标背包类型</param>
        /// <returns>是否成功添加</returns>
        public async Task<bool> AddItemToCharacterAsync(Guid characterId, string templateId, int amount = 1, InventoryType inventoryType = InventoryType.Normal)
        {
            var inventory = await GetInventoryAsync(characterId, inventoryType);
            if (inventory == null)
            {
                Logger.LogWarning($"角色 {characterId} 没有类型为 {inventoryType} 的背包");
                return false;
            }

            var success = true;
            for (int i = 0; i < amount && success; i++)
            {
                var itemId = await _itemModule.CreateItemAsync(templateId);
                if (itemId == Guid.Empty)
                {
                    success = false;
                    break;
                }

                success = await AddItemAsync(inventory.Id, itemId);
            }

            Logger.LogInformation($"为角色 {characterId} 添加物品 {templateId} {amount} 个 {(success ? "成功" : "失败")}");
            return success;
        }

        /// <summary>
        /// 给角色添加物品
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <param name="itemId">物品实例ID</param>
        /// <param name="inventoryType">目标背包类型</param>
        /// <returns>是否成功添加</returns>
        public async Task<bool> AddItemToCharacterAsync(Guid characterId, Guid itemId, InventoryType inventoryType = InventoryType.Normal)
        {
            var inventory = await GetInventoryAsync(characterId, inventoryType);
            if (inventory == null)
            {
                Logger.LogWarning($"角色 {characterId} 没有类型为 {inventoryType} 的背包");
                return false;
            }

            var success = await AddItemAsync(inventory.Id, itemId);
            Logger.LogInformation($"为角色 {characterId} 添加物品 {itemId} {(success ? "成功" : "失败")}");
            return success;
        }

        /// <summary>
        /// 从角色背包移除物品
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <param name="itemId">物品ID</param>
        /// <param name="amount">数量</param>
        /// <returns>是否成功移除</returns>
        public async Task<bool> RemoveItemFromCharacterAsync(Guid characterId, Guid itemId, int amount = 1)
        {
            var inventories = await GetCharacterInventoriesAsync(characterId);
            foreach (var inventory in inventories)
            {
                if (await RemoveItemAsync(inventory.Id, itemId, amount))
                {
                    Logger.LogInformation($"从角色 {characterId} 的背包中移除物品 {itemId} {amount} 个成功");
                    return true;
                }
            }

            Logger.LogWarning($"从角色 {characterId} 的背包中移除物品 {itemId} {amount} 个失败");
            return false;
        }

        /// <summary>
        /// 检查角色是否拥有物品
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <param name="templateId">物品模板ID</param>
        /// <param name="amount">数量</param>
        /// <returns>是否拥有</returns>
        public async Task<bool> CharacterHasItemAsync(Guid characterId, string templateId, int amount = 1)
        {
            var inventories = await GetCharacterInventoriesAsync(characterId);
            var totalAmount = 0;

            foreach (var inventory in inventories)
            {
                var items = await GetInventoryItemsAsync(inventory.Id);
                foreach (var itemId in items)
                {
                    var item = await _itemModule.GetItemAsync(itemId);
                    if (item != null && item.TemplateId == templateId)
                    {
                        totalAmount++;
                        if (totalAmount >= amount)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 角色使用物品(通过物品实例ID)
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <param name="itemId">物品实例ID</param>
        /// <param name="targetId">目标ID</param>
        /// <returns>使用结果</returns>
        public async Task<(bool success, string message)> UseItemAsync(Guid characterId, Guid itemId, Guid? targetId = null)
        {
            var inventories = await GetCharacterInventoriesAsync(characterId);
            foreach (var inventory in inventories)
            {
                var slot = inventory.FindItemById(itemId);
                if (slot != null)
                {
                    var item = slot.Item;
                    if (item.Type.HasFlag(ItemType.Consumable))
                    {
                        // 使用消耗品
                        var result = await UseConsumableItemAsync(characterId, item, targetId);
                        if (result.success)
                        {
                            // 使用成功后减少物品数量
                            inventory.RemoveItem(slot.Index, 1);
                        }
                        return result;
                    }
                    else
                    {
                        return (false, $"物品 {item.Name} 不是可使用的物品");
                    }
                }
            }

            return (false, $"找不到物品 {itemId}");
        }

        /// <summary>
        /// 角色使用物品(通过物品模板ID)
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <param name="templateId">物品模板ID</param>
        /// <param name="targetId">目标ID</param>
        /// <returns>使用结果</returns>
        public async Task<(bool success, string message)> UseItemByTemplateIdAsync(Guid characterId, Guid templateId, Guid? targetId = null)
        {
            var inventories = await GetCharacterInventoriesAsync(characterId);
            foreach (var inventory in inventories)
            {
                var slot = inventory.FindItemByTemplateId(templateId);
                if (slot != null)
                {
                    var item = slot.Item;
                    if (item.Type.HasFlag(ItemType.Consumable))
                    {
                        // 使用消耗品
                        var result = await UseConsumableItemAsync(characterId, item, targetId);
                        if (result.success)
                        {
                            // 使用成功后减少物品数量
                            inventory.RemoveItem(slot.Index, 1);
                        }
                        return result;
                    }
                    else
                    {
                        return (false, $"物品 {item.Name} 不是可使用的物品");
                    }
                }
            }

            return (false, $"找不到模板ID为 {templateId} 的物品");
        }

        /// <summary>
        /// 创建掉落物
        /// </summary>
        /// <param name="position">位置坐标</param>
        /// <param name="items">物品列表</param>
        /// <returns>掉落物ID</returns>
        public Task<Guid> CreateDropAsync((int x, int y) position, IReadOnlyList<IItem> items)
        {
            var drop = _dropManager.CreateDrop(position.x, position.y, items);
            return Task.FromResult(drop.Id);
        }

        /// <summary>
        /// 拾取掉落物
        /// </summary>
        /// <param name="dropId">掉落物ID</param>
        /// <param name="characterId">拾取角色ID</param>
        /// <returns>拾取结果</returns>
        public async Task<(bool success, string message)> PickupDropAsync(Guid dropId, Guid characterId)
        {
            var (success, message, items) = _dropManager.PickupDrop(dropId, characterId);
            if (!success)
            {
                return (success, message);
            }

            // 尝试将物品添加到角色背包
            var inventory = await GetCharacterMainInventoryAsync(characterId);
            var addedItems = 0;
            var failedItems = new List<IItem>();

            foreach (var item in items)
            {
                if (inventory.AddItem(item) != -1)
                {
                    addedItems++;
                }
                else
                {
                    failedItems.Add(item);
                }
            }

            // 如果有物品无法添加到背包，则创建新的掉落物
            if (failedItems.Count > 0)
            {
                var drop = _dropManager.GetDrop(dropId);
                var newDrop = _dropManager.CreateDrop(drop.X, drop.Y, failedItems);
                return (true, $"成功拾取 {addedItems} 个物品，但有 {failedItems.Count} 个物品因背包已满而重新掉落");
            }

            return (true, $"成功拾取 {addedItems} 个物品");
        }

        /// <summary>
        /// 注册物品模板
        /// </summary>
        /// <param name="templateId">模板ID</param>
        /// <param name="itemTemplate">物品模板</param>
        public void RegisterItemTemplate(string templateId, IItem itemTemplate)
        {
            _itemModule.RegisterItemTemplate(templateId, itemTemplate);
        }

        /// <summary>
        /// 批量注册物品模板
        /// </summary>
        /// <param name="templates">模板字典</param>
        public void RegisterItemTemplates(IDictionary<string, IItem> templates)
        {
            foreach (var template in templates)
            {
                RegisterItemTemplate(template.Key, template.Value);
            }
        }

        /// <summary>
        /// 加载物品数据
        /// </summary>
        /// <param name="data">物品数据</param>
        /// <returns>加载结果</returns>
        public Task<bool> LoadItemDataAsync(string data)
        {
            try
            {
                if (string.IsNullOrEmpty(data))
                {
                    Logger.LogWarning("加载的物品数据为空");
                    return Task.FromResult(false);
                }

                var inventoryData = JsonSerializer.Deserialize<InventoryData>(data);
                if (inventoryData == null)
                {
                    Logger.LogWarning("无法解析物品数据");
                    return Task.FromResult(false);
                }

                // 清空现有数据
                _characterInventories.Clear();

                // 加载角色背包数据
                foreach (var characterData in inventoryData.CharacterInventories)
                {
                    var characterId = characterData.Key;
                    var inventories = new Dictionary<InventoryType, IInventory>();
                    _characterInventories[characterId] = inventories;

                    foreach (var inventoryDataPair in characterData.Value)
                    {
                        var inventoryType = inventoryDataPair.Key;
                        var inventory = new Inventory(Guid.NewGuid(), characterId, inventoryType, _configManager, inventoryDataPair.Value.Capacity);

                        // 加载物品
                        foreach (var slotData in inventoryDataPair.Value.Slots)
                        {
                            if (slotData.Value.Item != null)
                            {
                                var item = _itemModule.CreateItem(slotData.Value.Item.TemplateId, slotData.Value.Item.StackSize);
                                inventory.AddItem(item, 1, slotData.Key);
                            }
                        }

                        inventories[inventoryType] = inventory;
                        _itemModule.RegisterInventory(inventory);
                    }
                }

                Logger.LogInformation("物品数据加载成功");
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "加载物品数据时发生错误");
                return Task.FromResult(false);
            }
        }

        /// <summary>
        /// 保存物品数据
        /// </summary>
        /// <returns>保存的数据</returns>
        public Task<string> SaveItemDataAsync()
        {
            try
            {
                var inventoryData = new InventoryData
                {
                    CharacterInventories = new Dictionary<Guid, Dictionary<InventoryType, SerializableInventory>>()
                };

                foreach (var characterEntry in _characterInventories)
                {
                    var characterId = characterEntry.Key;
                    var inventories = characterEntry.Value;
                    var serializableInventories = new Dictionary<InventoryType, SerializableInventory>();
                    inventoryData.CharacterInventories[characterId] = serializableInventories;

                    foreach (var inventoryEntry in inventories)
                    {
                        var inventoryType = inventoryEntry.Key;
                        var inventory = inventoryEntry.Value;
                        var serializableInventory = new SerializableInventory
                        {
                            Capacity = inventory.Capacity,
                            Slots = new Dictionary<int, SerializableInventorySlot>()
                        };

                        for (int i = 0; i < inventory.Capacity; i++)
                        {
                            var slot = inventory.GetSlot(i);
                            if (slot != null && slot.Item != null)
                            {
                                serializableInventory.Slots[i] = new SerializableInventorySlot
                                {
                                    Item = new SerializableItem
                                    {
                                        TemplateId = slot.Item.TemplateId,
                                        StackSize = slot.Item.StackSize
                                    }
                                };
                            }
                        }

                        serializableInventories[inventoryType] = serializableInventory;
                    }
                }

                var json = JsonSerializer.Serialize(inventoryData, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                Logger.LogInformation("物品数据保存成功");
                return Task.FromResult(json);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "保存物品数据时发生错误");
                return Task.FromResult(string.Empty);
            }
        }

        /// <summary>
        /// 获取背包
        /// </summary>
        private async Task<IInventory> GetInventoryAsync(Guid characterId, InventoryType inventoryType)
        {
            if (!_characterInventories.ContainsKey(characterId))
            {
                return null;
            }

            if (!_characterInventories[characterId].ContainsKey(inventoryType))
            {
                // 如果是主背包，则自动创建
                if (inventoryType == InventoryType.Normal)
                {
                    return await CreateCharacterInventoryAsync(characterId, 20);
                }
                return null;
            }

            return _characterInventories[characterId][inventoryType];
        }

        /// <summary>
        /// 使用消耗品
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <param name="item">物品</param>
        /// <param name="targetId">目标ID</param>
        /// <returns>使用结果</returns>
        private Task<(bool success, string message)> UseConsumableItemAsync(Guid characterId, IItem item, Guid? targetId)
        {
            // 这里应该根据物品类型调用不同的效果处理逻辑
            // 例如，如果是治疗药水，则调用角色模块的治疗方法
            // 如果是技能书，则调用技能模块的学习方法
            // 等等

            // 简单实现，后续可以扩展
            Logger.LogInformation($"角色 {characterId} 使用了物品 {item.Name}");
            return Task.FromResult((true, $"成功使用了 {item.Name}"));
        }

        /// <summary>
        /// 检查角色是否拥有足够数量的物品
        /// </summary>
        public async Task<bool> HasEnoughItemsByIdAsync(Guid characterId, Guid itemId, int count)
        {
            var inventories = await GetCharacterInventoriesAsync(characterId);
            var totalCount = 0;

            foreach (var inventory in inventories)
            {
                var items = await GetInventoryItemsAsync(inventory.Id);
                totalCount += items.Count(id => id == itemId);
                if (totalCount >= count)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 检查角色是否拥有足够数量的物品
        /// </summary>
        public async Task<bool> HasEnoughItemsByTemplateIdAsync(Guid characterId, string templateId, int count)
        {
            var inventories = await GetCharacterInventoriesAsync(characterId);
            var totalCount = 0;

            foreach (var inventory in inventories)
            {
                var items = await GetInventoryItemsAsync(inventory.Id);
                foreach (var itemId in items)
                {
                    var item = await _itemModule.GetItemAsync(itemId);
                    if (item != null && item.TemplateId == templateId)
                    {
                        totalCount++;
                        if (totalCount >= count)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 获取物品实例
        /// </summary>
        public async Task<Guid> GetItemAsync(Guid characterId, Guid itemId)
        {
            var inventories = await GetCharacterInventoriesAsync(characterId);
            foreach (var inventory in inventories)
            {
                var items = await GetInventoryItemsAsync(inventory.Id);
                if (items.Contains(itemId))
                {
                    return itemId;
                }
            }

            return Guid.Empty;
        }

        /// <summary>
        /// 根据模板ID获取物品实例
        /// </summary>
        public async Task<Guid> GetItemByTemplateIdAsync(Guid characterId, string templateId)
        {
            var inventories = await GetCharacterInventoriesAsync(characterId);
            foreach (var inventory in inventories)
            {
                var items = await GetInventoryItemsAsync(inventory.Id);
                foreach (var itemId in items)
                {
                    var item = await _itemModule.GetItemAsync(itemId);
                    if (item != null && item.TemplateId == templateId)
                    {
                        return itemId;
                    }
                }
            }

            return Guid.Empty;
        }

        /// <summary>
        /// 创建背包
        /// </summary>
        public async Task<Guid> CreateInventoryAsync(Guid ownerId, InventoryType type, int capacity)
        {
            var inventoryId = Guid.NewGuid();
            var inventory = new SimpleInventory(inventoryId, ownerId, type, capacity);
            _inventories[inventoryId] = inventory;
            Logger.LogInformation($"创建背包 ID: {inventoryId}，类型: {type}，容量: {capacity}");
            return inventoryId;
        }

        /// <summary>
        /// 添加物品到背包
        /// </summary>
        public async Task<bool> AddItemAsync(Guid inventoryId, Guid itemId, int? slotIndex = null)
        {
            if (!_inventories.TryGetValue(inventoryId, out var inventory))
            {
                Logger.LogWarning($"背包 {inventoryId} 不存在");
                return false;
            }

            var success = slotIndex.HasValue ?
                inventory.AddItemReference(itemId, 1, slotIndex.Value) != -1 :
                inventory.AddItemReference(itemId, 1) != -1;

            Logger.LogInformation($"向背包 {inventoryId} {(slotIndex.HasValue ? $"的槽位 {slotIndex.Value} " : "")}添加物品 {itemId} {(success ? "成功" : "失败")}");
            return success;
        }

        /// <summary>
        /// 从背包移除物品
        /// </summary>
        public async Task<bool> RemoveItemAsync(Guid inventoryId, Guid itemId, int amount = 1)
        {
            if (!_inventories.TryGetValue(inventoryId, out var inventory))
            {
                Logger.LogWarning($"背包 {inventoryId} 不存在");
                return false;
            }

            var slot = inventory.FindItemById(itemId);
            if (slot == null)
            {
                Logger.LogWarning($"背包 {inventoryId} 中没有物品 {itemId}");
                return false;
            }

            var removedItemId = inventory.RemoveItemReference(slot.SlotIndex, amount);
            var success = removedItemId != Guid.Empty;
            Logger.LogInformation($"从背包 {inventoryId} 移除物品 {itemId} {amount} 个 {(success ? "成功" : "失败")}");
            return success;
        }

        /// <summary>
        /// 获取背包中的所有物品
        /// </summary>
        public async Task<IReadOnlyList<Guid>> GetInventoryItemsAsync(Guid inventoryId)
        {
            if (!_inventories.TryGetValue(inventoryId, out var inventory))
            {
                Logger.LogWarning($"背包 {inventoryId} 不存在");
                return new List<Guid>();
            }

            return inventory.GetAllItemIds();
        }

        /// <summary>
        /// 检查背包是否有空间放置物品
        /// </summary>
        public async Task<bool> HasSpaceForItemAsync(Guid inventoryId, Guid itemId)
        {
            if (!_inventories.TryGetValue(inventoryId, out var inventory))
            {
                Logger.LogWarning($"背包 {inventoryId} 不存在");
                return false;
            }

            return !inventory.IsFull;
        }

        /// <summary>
        /// 在背包间转移物品
        /// </summary>
        public async Task<bool> TransferItemAsync(Guid sourceInventoryId, Guid targetInventoryId, Guid itemId)
        {
            if (!_inventories.TryGetValue(sourceInventoryId, out var sourceInventory) ||
                !_inventories.TryGetValue(targetInventoryId, out var targetInventory))
            {
                Logger.LogWarning("源背包或目标背包不存在");
                return false;
            }

            var sourceSlot = sourceInventory.FindItemById(itemId);
            if (sourceSlot == null)
            {
                Logger.LogWarning($"源背包中没有物品 {itemId}");
                return false;
            }

            var count = sourceSlot.Count;
            var targetSlotIndex = targetInventory.AddItemReference(itemId, count);
            if (targetSlotIndex != -1)
            {
                sourceInventory.RemoveItemReference(sourceSlot.SlotIndex, count);
                Logger.LogInformation($"将物品 {itemId} 从背包 {sourceInventoryId} 转移到背包 {targetInventoryId} 成功");
                return true;
            }

            Logger.LogWarning($"将物品 {itemId} 从背包 {sourceInventoryId} 转移到背包 {targetInventoryId} 失败");
            return false;
        }

        /// <summary>
        /// 移动背包中的物品
        /// </summary>
        public async Task<bool> MoveItemAsync(Guid inventoryId, int fromSlotIndex, int toSlotIndex)
        {
            if (!_inventories.TryGetValue(inventoryId, out var inventory))
            {
                Logger.LogWarning($"背包 {inventoryId} 不存在");
                return false;
            }

            var success = inventory.MoveItem(fromSlotIndex, toSlotIndex);
            if (success)
            {
                Logger.LogInformation($"在背包 {inventoryId} 中将物品从槽位 {fromSlotIndex} 移动到槽位 {toSlotIndex} 成功");
            }
            else
            {
                Logger.LogWarning($"在背包 {inventoryId} 中将物品从槽位 {fromSlotIndex} 移动到槽位 {toSlotIndex} 失败");
            }
            return success;
        }

        /// <summary>
        /// 交换背包中的物品
        /// </summary>
        public async Task<bool> SwapItemsAsync(Guid inventoryId, int slotIndex1, int slotIndex2)
        {
            if (!_inventories.TryGetValue(inventoryId, out var inventory))
            {
                Logger.LogWarning($"背包 {inventoryId} 不存在");
                return false;
            }

            var success = inventory.SwapItems(slotIndex1, slotIndex2);
            if (success)
            {
                Logger.LogInformation($"在背包 {inventoryId} 中交换槽位 {slotIndex1} 和槽位 {slotIndex2} 的物品成功");
            }
            else
            {
                Logger.LogWarning($"在背包 {inventoryId} 中交换槽位 {slotIndex1} 和槽位 {slotIndex2} 的物品失败");
            }
            return success;
        }

        /// <summary>
        /// 对背包进行排序
        /// </summary>
        public async Task<bool> SortInventoryAsync(Guid inventoryId)
        {
            if (!_inventories.TryGetValue(inventoryId, out var inventory))
            {
                Logger.LogWarning($"背包 {inventoryId} 不存在");
                return false;
            }

            var success = inventory.Sort();
            if (success)
            {
                Logger.LogInformation($"背包 {inventoryId} 排序完成");
            }
            else
            {
                Logger.LogWarning($"背包 {inventoryId} 排序失败");
            }
            return success;
        }

        /// <summary>
        /// 加载背包数据
        /// </summary>
        public async Task<bool> LoadInventoryDataAsync(string data)
        {
            try
            {
                var inventoryData = System.Text.Json.JsonSerializer.Deserialize<InventoryData>(data);
                if (inventoryData == null)
                {
                    Logger.LogWarning("无效的背包数据");
                    return false;
                }

                _characterInventories.Clear();
                _inventories.Clear();

                foreach (var characterDataPair in inventoryData.CharacterInventories)
                {
                    var characterId = characterDataPair.Key;
                    _characterInventories[characterId] = new Dictionary<InventoryType, IInventory>();

                    foreach (var inventoryDataPair in characterDataPair.Value)
                    {
                        var inventoryType = inventoryDataPair.Key;
                        var inventoryId = await CreateInventoryAsync(characterId, inventoryType, inventoryDataPair.Value.Capacity);
                        var inventory = _inventories[inventoryId];

                        foreach (var slotData in inventoryDataPair.Value.Slots)
                        {
                            if (slotData.Value.Item != null)
                            {
                                var itemId = await _itemModule.CreateItemAsync(slotData.Value.Item.TemplateId, slotData.Value.Item.StackSize);
                                await AddItemAsync(inventoryId, itemId, slotData.Key);
                            }
                        }

                        _characterInventories[characterId][inventoryType] = inventory;
                    }
                }

                Logger.LogInformation("背包数据加载完成");
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "加载背包数据时发生错误");
                return false;
            }
        }

        /// <summary>
        /// 保存背包数据
        /// </summary>
        public async Task<string> SaveInventoryDataAsync()
        {
            try
            {
                var inventoryData = new InventoryData
                {
                    CharacterInventories = new Dictionary<Guid, Dictionary<InventoryType, SerializableInventory>>()
                };

                foreach (var characterPair in _characterInventories)
                {
                    var characterId = characterPair.Key;
                    var inventories = characterPair.Value;
                    var serializableInventories = new Dictionary<InventoryType, SerializableInventory>();
                    inventoryData.CharacterInventories[characterId] = serializableInventories;

                    foreach (var inventoryPair in inventories)
                    {
                        var inventoryType = inventoryPair.Key;
                        var inventory = inventoryPair.Value;

                        var serializableInventory = new SerializableInventory
                        {
                            Capacity = inventory.Capacity,
                            Slots = new Dictionary<int, SerializableInventorySlot>()
                        };

                        for (int i = 0; i < inventory.Capacity; i++)
                        {
                            var slot = inventory.GetSlot(i);
                            if (slot != null && !slot.IsEmpty)
                            {
                                serializableInventory.Slots[i] = new SerializableInventorySlot
                                {
                                    Item = new SerializableItem
                                    {
                                        TemplateId = slot.ItemId.ToString(),
                                        StackSize = slot.Count
                                    }
                                };
                            }
                        }

                        serializableInventories[inventoryType] = serializableInventory;
                    }
                }

                var options = new System.Text.Json.JsonSerializerOptions { WriteIndented = true };
                var jsonData = System.Text.Json.JsonSerializer.Serialize(inventoryData, options);

                Logger.LogInformation("背包数据保存完成");
                return jsonData;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "保存背包数据时发生错误");
                return string.Empty;
            }
        }

        /// <summary>
        /// 根据模板ID查找物品
        /// </summary>
        public async Task<IInventorySlot> FindItemByTemplateIdAsync(Guid inventoryId, string templateId)
        {
            if (!_inventories.TryGetValue(inventoryId, out var inventory))
            {
                return null;
            }

            return inventory.FindItemByTemplateId(new Guid(templateId), _itemModule);
        }

        #region 数据模型

        /// <summary>
        /// 背包数据
        /// </summary>
        private class InventoryData
        {
            /// <summary>
            /// 角色背包数据
            /// </summary>
            public Dictionary<Guid, Dictionary<InventoryType, SerializableInventory>> CharacterInventories { get; set; }
                = new Dictionary<Guid, Dictionary<InventoryType, SerializableInventory>>();
        }

        /// <summary>
        /// 可序列化的背包数据
        /// </summary>
        private class SerializableInventory
        {
            /// <summary>
            /// 背包容量
            /// </summary>
            public int Capacity { get; set; }

            /// <summary>
            /// 槽位数据
            /// </summary>
            public Dictionary<int, SerializableInventorySlot> Slots { get; set; }
                = new Dictionary<int, SerializableInventorySlot>();
        }

        /// <summary>
        /// 可序列化的槽位数据
        /// </summary>
        private class SerializableInventorySlot
        {
            /// <summary>
            /// 物品数据
            /// </summary>
            public SerializableItem Item { get; set; }
        }

        /// <summary>
        /// 可序列化的物品数据
        /// </summary>
        private class SerializableItem
        {
            /// <summary>
            /// 物品模板ID
            /// </summary>
            public string TemplateId { get; set; }

            /// <summary>
            /// 堆叠数量
            /// </summary>
            public int StackSize { get; set; }
        }

        #endregion
    }
}