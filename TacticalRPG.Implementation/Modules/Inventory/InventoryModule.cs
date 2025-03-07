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

namespace TacticalRPG.Implementation.Modules.Inventory
{
    /// <summary>
    /// 物品背包模块实现类
    /// </summary>
    public class InventoryModule : BaseGameModule, IInventoryModule
    {
        private readonly IItemManager _itemManager;
        private readonly IDropManager _dropManager;
        private readonly IConfigManager _configManager;

        public int AddItemToCharacter(Guid characterId, string itemId, int count, InventoryType inventoryType)
        {
            var item = _itemManager.CreateItem(itemId);
            if (item == null) return -1;
            return AddItemToCharacter(characterId, item, inventoryType);
        }

        public int AddItemToCharacter(Guid characterId, IItem item, InventoryType inventoryType)
        {
            if (!_characterInventories.ContainsKey(characterId)) return -1;
            if (!_characterInventories[characterId].ContainsKey(inventoryType)) return -1;

            var inventory = _characterInventories[characterId][inventoryType];
            return inventory.AddItem(item);
        }
        private readonly Dictionary<Guid, Dictionary<InventoryType, IInventory>> _characterInventories = new Dictionary<Guid, Dictionary<InventoryType, IInventory>>();
        private readonly Dictionary<Guid, IInventory> _inventories = new Dictionary<Guid, IInventory>();
        private readonly Dictionary<Guid, IItemManager> _characterItemManagers = new Dictionary<Guid, IItemManager>();

        /// <summary>
        /// 获取物品管理器
        /// </summary>
        public IItemManager ItemManager => _itemManager;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="gameSystem">游戏系统</param>
        /// <param name="logger">日志记录器</param>
        /// <param name="itemManager">物品管理器</param>
        /// <param name="dropManager">掉落物管理器</param>
        /// <param name="configManager">配置管理器</param>
        public InventoryModule(
            IGameSystem gameSystem,
            ILogger<InventoryModule> logger,
            IItemManager itemManager,
            IDropManager dropManager,
            IConfigManager configManager)
            : base(gameSystem, logger)
        {
            _itemManager = itemManager ?? throw new ArgumentNullException(nameof(itemManager));
            _dropManager = dropManager ?? throw new ArgumentNullException(nameof(dropManager));
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
            return _itemManager.CreateItem(templateId, stackSize);
        }

        /// <summary>
        /// 创建角色背包
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <param name="capacity">容量</param>
        /// <param name="inventoryType">背包类型</param>
        /// <returns>创建的背包</returns>
        public Task<IInventory> CreateCharacterInventoryAsync(Guid characterId, int capacity, InventoryType inventoryType = InventoryType.Normal)
        {
            // 从配置中获取默认值
            var config = _configManager.GetConfig<InventoryConfig>(InventoryConfig.MODULE_ID);
            int defaultCapacity = config?.DefaultCapacity ?? 20;
            float? defaultMaxWeight = config?.EnableWeightLimit == true ? config.MaxWeight : null;

            // 使用提供的值或默认值
            capacity = capacity > 0 ? capacity : defaultCapacity;

            if (!_characterInventories.TryGetValue(characterId, out var inventories))
            {
                inventories = new Dictionary<InventoryType, IInventory>();
                _characterInventories[characterId] = inventories;
            }

            if (inventories.ContainsKey(inventoryType))
            {
                Logger.LogWarning($"角色 {characterId} 已经拥有类型为 {inventoryType} 的背包");
                return Task.FromResult(inventories[inventoryType]);
            }

            var inventory = new Inventory(Guid.NewGuid(), characterId, inventoryType, _configManager, capacity, defaultMaxWeight);
            inventory.SetName($"{inventoryType} Inventory");

            inventories[inventoryType] = inventory;
            _itemManager.RegisterInventory(inventory);

            Logger.LogInformation($"为角色 {characterId} 创建了类型为 {inventoryType} 的背包，容量为 {capacity}，最大重量: {defaultMaxWeight}");
            return Task.FromResult<IInventory>(inventory);
        }

        /// <summary>
        /// 获取角色的主背包
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <returns>角色的主背包</returns>
        public async Task<IInventory> GetCharacterMainInventoryAsync(Guid characterId)
        {
            if (_characterInventories.TryGetValue(characterId, out var inventories) &&
                inventories.TryGetValue(InventoryType.Normal, out var mainInventory))
            {
                return mainInventory;
            }

            // 如果主背包不存在，则创建一个
            return await CreateCharacterInventoryAsync(characterId, 20);
        }

        /// <summary>
        /// 获取角色的所有背包
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <returns>角色的所有背包</returns>
        public Task<IReadOnlyList<IInventory>> GetCharacterInventoriesAsync(Guid characterId)
        {
            if (_characterInventories.TryGetValue(characterId, out var inventories))
            {
                return Task.FromResult<IReadOnlyList<IInventory>>(inventories.Values.ToList());
            }

            return Task.FromResult<IReadOnlyList<IInventory>>(new List<IInventory>());
        }

        /// <summary>
        /// 给角色添加物品
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <param name="templateId">物品模板ID</param>
        /// <param name="amount">数量</param>
        /// <param name="inventoryType">目标背包类型</param>
        /// <returns>是否成功添加</returns>
        public async Task<bool> AddItemToCharacterAsync(Guid characterId, string templateId, int amount = 1, string inventoryType = "Main")
        {
            if (amount <= 0)
            {
                Logger.LogWarning($"尝试添加无效数量的物品: {amount}");
                return false;
            }

            var inventory = await GetInventoryAsync(characterId, inventoryType);
            if (inventory == null)
            {
                Logger.LogWarning($"角色 {characterId} 没有类型为 {inventoryType} 的背包");
                return false;
            }

            var success = true;
            for (int i = 0; i < amount; i++)
            {
                var item = _itemManager.CreateItem(templateId);
                if (item == null)
                {
                    Logger.LogWarning($"无法创建物品模板 {templateId}");
                    return false;
                }

                if (inventory.AddItem(item) == -1)
                {
                    success = false;
                    break;
                }
            }

            Logger.LogInformation($"为角色 {characterId} 添加 {(success ? amount : 0)} 个物品 {templateId}");
            return success;
        }

        /// <summary>
        /// 给角色添加物品
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <param name="item">物品实例</param>
        /// <param name="inventoryType">目标背包类型</param>
        /// <returns>是否成功添加</returns>
        public async Task<bool> AddItemToCharacterAsync(Guid characterId, IItem item, string inventoryType = "Main")
        {
            if (item == null)
            {
                Logger.LogWarning("尝试添加空物品");
                return false;
            }

            var inventory = await GetInventoryAsync(characterId, inventoryType);
            if (inventory == null)
            {
                Logger.LogWarning($"角色 {characterId} 没有类型为 {inventoryType} 的背包");
                return false;
            }

            var success = inventory.AddItem(item) != -1;
            Logger.LogInformation($"为角色 {characterId} 添加物品 {item.Name} {(success ? "成功" : "失败")}");
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
                var slot = inventory.FindItemSlots(itemId);
                if (slot.Count > 0)
                {
                    var success = inventory.RemoveItem(slot.First(), amount) != null;
                    Logger.LogInformation($"从角色 {characterId} 的背包中移除物品 {itemId} {amount} 个 {(success ? "成功" : "失败")}");
                    return success;
                }
            }

            Logger.LogWarning($"角色 {characterId} 的背包中没有物品 {itemId}");
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
                totalAmount += inventory.CountItem(templateId);
                if (totalAmount >= amount)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 角色使用物品
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <param name="itemId">物品ID</param>
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
                            inventory.RemoveItem(slot.SlotIndex, 1);
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
                if (inventory.AddItem(item))
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
            _itemManager.RegisterItemTemplate(templateId, itemTemplate);
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
                    var inventories = new Dictionary<string, IInventory>();
                    _characterInventories[characterId] = inventories;

                    foreach (var inventoryData in characterData.Value)
                    {
                        var inventoryType = inventoryData.Key;
                        var inventory = new Inventory(characterId, inventoryType, inventoryData.Value.Capacity);

                        // 加载物品
                        foreach (var slotData in inventoryData.Value.Slots)
                        {
                            if (slotData.Value.Item != null)
                            {
                                var item = _itemManager.CreateItem(slotData.Value.Item.TemplateId, slotData.Value.Item.StackSize);
                                inventory.AddItemToSlot(slotData.Key, item);
                            }
                        }

                        inventories[inventoryType] = inventory;
                        _itemManager.RegisterInventory(inventory);
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
                    CharacterInventories = new Dictionary<Guid, Dictionary<string, SerializableInventory>>()
                };

                foreach (var characterEntry in _characterInventories)
                {
                    var characterId = characterEntry.Key;
                    var inventories = characterEntry.Value;
                    var serializableInventories = new Dictionary<string, SerializableInventory>();
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
        /// 获取角色的指定类型背包
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <param name="inventoryType">背包类型</param>
        /// <returns>背包</returns>
        private async Task<IInventory> GetInventoryAsync(Guid characterId, string inventoryType)
        {
            if (_characterInventories.TryGetValue(characterId, out var inventories) &&
                inventories.TryGetValue(inventoryType, out var inventory))
            {
                return inventory;
            }

            // 如果是主背包且不存在，则创建一个
            if (inventoryType == "Main")
            {
                return await CreateCharacterInventoryAsync(characterId, 20);
            }

            return null;
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
    }

    /// <summary>
    /// 用于序列化的物品数据类
    /// </summary>
    internal class InventoryData
    {
        public Dictionary<Guid, Dictionary<string, SerializableInventory>> CharacterInventories { get; set; }
    }

    /// <summary>
    /// 用于序列化的背包数据类
    /// </summary>
    internal class SerializableInventory
    {
        public int Capacity { get; set; }
        public Dictionary<int, SerializableInventorySlot> Slots { get; set; }
    }

    /// <summary>
    /// 用于序列化的背包槽位数据类
    /// </summary>
    internal class SerializableInventorySlot
    {
        public SerializableItem Item { get; set; }
    }

    /// <summary>
    /// 用于序列化的物品数据类
    /// </summary>
    internal class SerializableItem
    {
        public string TemplateId { get; set; }
        public int StackSize { get; set; }
    }
}