using System;
using System.Collections.Generic;
using System.Linq;
using TacticalRPG.Core.Modules.Inventory;
using TacticalRPG.Implementation.Modules.Inventory.SortStrategies;
using TacticalRPG.Core.Modules.Config;
using TacticalRPG.Core.Modules.Item;

namespace TacticalRPG.Implementation.Modules.Inventory
{
    /// <summary>
    /// 库存排序类型枚举
    /// </summary>
    public enum InventorySortType
    {
        /// <summary>
        /// 按名称排序
        /// </summary>
        ByName,

        /// <summary>
        /// 按类型排序
        /// </summary>
        ByType,

        /// <summary>
        /// 按稀有度排序
        /// </summary>
        ByRarity,

        /// <summary>
        /// 按价值排序
        /// </summary>
        ByValue,

        /// <summary>
        /// 按重量排序
        /// </summary>
        ByWeight
    }

    /// <summary>
    /// 库存实现类
    /// </summary>
    public class Inventory : IInventory
    {
        private readonly Dictionary<int, IInventorySlot> _slots = new Dictionary<int, IInventorySlot>();
        private Dictionary<string, object> _properties = new Dictionary<string, object>();
        private float? _maxWeight;
        private List<IItem> _tempItems = new List<IItem>();
        private readonly IConfigManager _configManager;

        /// <summary>
        /// 获取库存的唯一标识符
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// 获取库存拥有者的唯一标识符
        /// </summary>
        public Guid OwnerId { get; }

        /// <summary>
        /// 获取或设置库存的名称
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 获取库存的容量
        /// </summary>
        public int Capacity { get; private set; }

        /// <summary>
        /// 获取已使用的槽位数量
        /// </summary>
        public int UsedSlots => _slots.Count(s => !s.Value.IsEmpty);

        /// <summary>
        /// 获取剩余的槽位数量
        /// </summary>
        public int RemainingSlots => Capacity - UsedSlots;

        /// <summary>
        /// 获取库存是否已满
        /// </summary>
        public bool IsFull => UsedSlots >= Capacity;

        /// <summary>
        /// 获取库存是否为空
        /// </summary>
        public bool IsEmpty => UsedSlots == 0;

        /// <summary>
        /// 获取库存的槽位集合
        /// </summary>
        public IReadOnlyList<IInventorySlot> Slots => _slots.Values.ToList();

        /// <summary>
        /// 获取库存的最大重量
        /// </summary>
        public float? MaxWeight => _maxWeight;

        /// <summary>
        /// 获取库存的当前重量
        /// </summary>
        public float CurrentWeight => _slots.Values.Where(s => !s.IsEmpty).Sum(s => s.Item.Weight * s.Item.StackSize);

        /// <summary>
        /// 获取库存类型
        /// </summary>
        public InventoryType InventoryType { get; private set; }

        /// <summary>
        /// 构造一个新的背包实例
        /// </summary>
        /// <param name="id">背包ID</param>
        /// <param name="ownerId">拥有者ID</param>
        /// <param name="inventoryType">背包类型</param>
        /// <param name="configManager">配置管理器</param>
        /// <param name="capacity">容量（可选）</param>
        /// <param name="maxWeight">最大重量（可选）</param>
        public Inventory(Guid id, Guid ownerId, InventoryType inventoryType, IConfigManager configManager, int? capacity = null, float? maxWeight = null)
        {
            Id = id;
            OwnerId = ownerId;
            InventoryType = inventoryType;
            _configManager = configManager ?? throw new ArgumentNullException(nameof(configManager));

            // 获取背包配置
            var config = _configManager.GetConfig<InventoryConfig>(InventoryConfig.MODULE_ID);

            // 使用配置或指定参数初始化
            Capacity = capacity ?? config?.DefaultCapacity ?? 20;
            _maxWeight = maxWeight ?? (config?.EnableWeightLimit == true ? config.MaxWeight : null);

            // 初始化槽位
            for (int i = 0; i < Capacity; i++)
            {
                _slots[i] = new InventorySlot(i);
            }

            Name = $"{inventoryType} Inventory";
        }

        /// <summary>
        /// 设置库存名称
        /// </summary>
        /// <param name="name">名称</param>
        public void SetName(string name)
        {
            Name = !string.IsNullOrEmpty(name) ? name : InventoryType.ToString();
        }

        /// <summary>
        /// 调整库存容量
        /// </summary>
        /// <param name="newCapacity">新容量</param>
        /// <returns>是否调整成功</returns>
        public bool SetCapacity(int newCapacity, string reason = "")
        {
            if (newCapacity < UsedSlots)
                return false;

            // 添加新槽位
            for (int i = Capacity; i < newCapacity; i++)
            {
                _slots[i] = new InventorySlot(i);
            }

            // 移除超过新容量的槽位
            for (int i = newCapacity; i < Capacity; i++)
            {
                if (_slots.ContainsKey(i) && !_slots[i].IsEmpty)
                    return false; // 不能移除有物品的槽位

                _slots.Remove(i);
            }

            Capacity = newCapacity;
            return true;
        }

        /// <summary>
        /// 设置最大重量
        /// </summary>
        /// <param name="maxWeight">最大重量</param>
        /// <returns>是否设置成功</returns>
        public bool SetMaxWeight(float maxWeight)
        {
            if (maxWeight < CurrentWeight)
                return false;

            _maxWeight = maxWeight;
            return true;
        }

        /// <summary>
        /// 获取指定索引的槽位
        /// </summary>
        /// <param name="slotIndex">槽位索引</param>
        /// <returns>槽位</returns>
        public IInventorySlot GetSlot(int slotIndex)
        {
            return _slots.TryGetValue(slotIndex, out var slot) ? slot : null;
        }

        /// <summary>
        /// 添加物品到背包
        /// </summary>
        /// <param name="item">要添加的物品</param>
        /// <param name="slotIndex">指定槽位索引（可选）</param>
        /// <returns>添加结果，如果成功则返回槽位索引，否则返回-1</returns>
        public int AddItem(IItem item, int? slotIndex = null)
        {
            if (item == null)
                return -1;

            // 从配置中获取物品堆叠相关设置
            var config = _configManager.GetConfig<InventoryConfig>(InventoryConfig.MODULE_ID);
            int maxStackSize = config?.MaxStackSize ?? 99;
            bool autoMergeStacks = config?.AutoMergeStacks ?? true;

            // 检查是否超过重量限制
            if (MaxWeight.HasValue && CurrentWeight + (item.Weight * item.StackSize) > MaxWeight.Value)
                return -1;

            // 如果指定了槽位，优先尝试在该槽位添加
            if (slotIndex.HasValue)
            {
                if (!_slots.TryGetValue(slotIndex.Value, out var slot))
                    return -1;

                // 尝试使用MergeItem方法合并物品
                var remaining = slot.MergeItem(item);
                if (remaining == null) // 完全合并
                    return slotIndex.Value;

                // 如果有剩余物品，继续尝试添加
                item = remaining;
            }

            // 如果启用了自动合并堆叠，先尝试合并已有的相同物品
            if (autoMergeStacks && item.IsStackable)
            {
                // 查找所有可以合并的槽位（包含相同类型、相同稀有度的物品）
                foreach (var kvp in _slots)
                {
                    var slot = kvp.Value;
                    if (!slot.IsEmpty && !slot.IsLocked &&
                        slot.Item.IsStackable && slot.Item.StackSize < (slot.Item.MaxStackSize > 0 ? slot.Item.MaxStackSize : maxStackSize) &&
                        AreItemsSameType(slot.Item, item))
                    {
                        // 尝试合并
                        var remaining = slot.MergeItem(item);
                        if (remaining == null) // 完全合并
                            return kvp.Key;

                        // 部分合并，更新剩余物品
                        item = remaining;
                    }
                }
            }

            // 寻找空槽位
            foreach (var kvp in _slots)
            {
                var slot = kvp.Value;
                if (slot.IsEmpty && !slot.IsLocked && slot.CanAcceptItem(item))
                {
                    slot.SetItem(item);
                    return kvp.Key;
                }
            }

            return -1; // 没有可用槽位
        }

        /// <summary>
        /// 添加指定数量的物品到背包
        /// </summary>
        /// <param name="item">要添加的物品</param>
        /// <param name="count">数量</param>
        /// <param name="slotIndex">指定槽位索引（可选）</param>
        /// <returns>实际添加的数量</returns>
        public int AddItem(IItem item, int count, int? slotIndex = null)
        {
            if (item == null || count <= 0)
                return 0;

            // 从配置中获取物品堆叠相关设置
            var config = _configManager.GetConfig<InventoryConfig>(InventoryConfig.MODULE_ID);
            int maxStackSize = config?.MaxStackSize ?? 99;
            bool autoMergeStacks = config?.AutoMergeStacks ?? true;

            int amountAdded = 0;

            // 如果指定了槽位，优先尝试在该槽位添加
            if (slotIndex.HasValue)
            {
                if (!_slots.TryGetValue(slotIndex.Value, out var slot))
                    return 0;
                if (slot.IsEmpty)
                {
                    if (slot.SetItem(item))
                    {
                        return slotIndex.Value;
                    }
                    return 0;
                }
                if (slot.CanAcceptItem(item))
                {
                    slot.AddCount(count);
                    if (slot.SetItem(item))
                    {
                        return slotIndex.Value;
                    }
                }
                // 直接使用槽位的AddItem方法
                return slot.AddItem(item, count);
            }

            // 创建临时物品进行添加
            IItem tempItem = item.Clone(count);
            if (tempItem == null)
                return 0;

            // 如果启用了自动合并堆叠，先尝试合并已有的相同物品
            if (autoMergeStacks)
            {
                MergeStacks();
            }

            // 尝试先将物品堆叠到已有的相同类型的物品上
            if (tempItem.IsStackable)
            {
                // 获取所有包含相同类型的槽位，并按剩余空间排序（优先填满快满的槽位）
                var suitableSlots = _slots.Values
                    .Where(s => !s.IsEmpty && !s.IsLocked &&
                               s.Item.IsStackable &&
                               AreItemsSameType(s.Item, tempItem) &&
                               s.Item.StackSize < (s.Item.MaxStackSize > 0 ? s.Item.MaxStackSize : maxStackSize))
                    .OrderByDescending(s => s.Item.StackSize)
                    .ToList();

                foreach (var slot in suitableSlots)
                {
                    // 检查重量限制
                    int maxAddable = Math.Min(tempItem.StackSize,
                        (slot.Item.MaxStackSize > 0 ? slot.Item.MaxStackSize : maxStackSize) - slot.Item.StackSize);

                    if (MaxWeight.HasValue)
                    {
                        float addedWeight = tempItem.Weight * maxAddable;
                        if (CurrentWeight + addedWeight > MaxWeight.Value)
                        {
                            int weightAllowed = (int)Math.Floor((MaxWeight.Value - CurrentWeight) / tempItem.Weight);
                            maxAddable = Math.Min(maxAddable, Math.Max(0, weightAllowed));
                        }
                    }

                    if (maxAddable <= 0)
                        continue;

                    // 克隆需要合并的数量
                    IItem partialItem = tempItem.SplitStack(maxAddable);
                    if (partialItem == null)
                        continue;

                    // 尝试合并
                    var remaining = slot.MergeItem(partialItem);
                    if (remaining == null)
                    {
                        amountAdded += maxAddable;
                    }
                    else
                    {
                        // 如果有剩余物品，重新合并回临时物品
                        tempItem.AddToStack(remaining.StackSize);
                        amountAdded += maxAddable - remaining.StackSize;
                    }

                    if (tempItem.StackSize <= 0)
                        return amountAdded;
                }
            }

            // 找空槽位添加剩余物品
            while (tempItem != null && tempItem.StackSize > 0)
            {
                var emptySlot = _slots.Values
                    .FirstOrDefault(s => s.IsEmpty && !s.IsLocked && s.CanAcceptItem(tempItem));

                if (emptySlot == null)
                    break;

                // 检查重量限制
                int maxAddable = tempItem.StackSize;
                if (MaxWeight.HasValue)
                {
                    float addedWeight = tempItem.Weight * maxAddable;
                    if (CurrentWeight + addedWeight > MaxWeight.Value)
                    {
                        int weightAllowed = (int)Math.Floor((MaxWeight.Value - CurrentWeight) / tempItem.Weight);
                        maxAddable = Math.Min(maxAddable, Math.Max(0, weightAllowed));
                    }
                }

                if (maxAddable <= 0)
                    break;

                // 如果需要拆分堆叠
                if (maxAddable < tempItem.StackSize)
                {
                    IItem splitItem = tempItem.SplitStack(maxAddable);
                    if (splitItem != null)
                    {
                        emptySlot.SetItem(splitItem);
                        amountAdded += maxAddable;
                    }
                }
                else
                {
                    emptySlot.SetItem(tempItem);
                    amountAdded += tempItem.StackSize;
                    tempItem = null;
                }
            }

            return amountAdded;
        }

        /// <summary>
        /// 从库存移除物品
        /// </summary>
        /// <param name="slotIndex">槽位索引</param>
        /// <returns>移除的物品</returns>
        public IItem RemoveItem(int slotIndex)
        {
            if (!_slots.TryGetValue(slotIndex, out var slot) || slot.IsEmpty || slot.IsLocked)
                return null;

            return slot.RemoveItem();
        }

        /// <summary>
        /// 从库存移除指定数量的物品
        /// </summary>
        /// <param name="slotIndex">槽位索引</param>
        /// <param name="count">数量</param>
        /// <returns>移除的物品</returns>
        public IItem RemoveItem(int slotIndex, int count)
        {
            if (!_slots.TryGetValue(slotIndex, out var slot) || slot.IsEmpty || slot.IsLocked || count <= 0)
                return null;

            return slot.RemoveItem(count);
        }

        /// <summary>
        /// 从库存移除指定ID的物品
        /// </summary>
        /// <param name="itemId">物品ID</param>
        /// <returns>是否移除成功</returns>
        public bool RemoveItem(Guid? itemId)
        {
            if (itemId == null)
                return false;

            var slot = _slots.Values.FirstOrDefault(s => !s.IsEmpty && !s.IsLocked && s.Item.Id == itemId);
            if (slot == null)
                return false;

            slot.RemoveItem();
            return true;
        }

        /// <summary>
        /// 从库存移除指定ID的物品的指定数量
        /// </summary>
        /// <param name="templateId">物品模板ID</param>
        /// <param name="count">数量</param>
        /// <returns>实际移除的数量</returns>
        public int RemoveItem(Guid? templateId, int count)
        {
            if (templateId == null || count <= 0)
                return 0;

            int amountRemoved = 0;

            foreach (var slot in _slots.Values.Where(s => !s.IsEmpty && !s.IsLocked && s.Item.TemplateId == templateId))
            {
                int toRemove = Math.Min(count - amountRemoved, slot.Item.StackSize);
                IItem removed = slot.RemoveItem(toRemove);

                if (removed != null)
                    amountRemoved += removed.StackSize;

                if (amountRemoved >= count)
                    break;
            }

            return amountRemoved;
        }

        /// <summary>
        /// 在两个槽位间移动物品
        /// </summary>
        /// <param name="fromSlotIndex">源槽位索引</param>
        /// <param name="toSlotIndex">目标槽位索引</param>
        /// <returns>是否移动成功</returns>
        public bool MoveItem(int fromSlotIndex, int toSlotIndex)
        {
            // 检查索引的有效性
            if (!_slots.TryGetValue(fromSlotIndex, out var fromSlot) ||
                !_slots.TryGetValue(toSlotIndex, out var toSlot))
                return false;

            // 如果源槽位为空或锁定
            if (fromSlot.IsEmpty || fromSlot.IsLocked)
                return false;

            // 如果目标槽位锁定
            if (toSlot.IsLocked)
                return false;

            // 如果目标槽位不为空
            if (!toSlot.IsEmpty)
            {
                // 如果物品相同且可堆叠
                if (fromSlot.Item.Id == toSlot.Item.Id && fromSlot.Item.IsStackable && toSlot.Item.IsStackable)
                {
                    int canAdd = Math.Min(fromSlot.Item.StackSize, toSlot.Item.MaxStackSize - toSlot.Item.StackSize);
                    if (canAdd <= 0)
                        return false;

                    toSlot.Item.AddToStack(canAdd);

                    if (canAdd >= fromSlot.Item.StackSize)
                        fromSlot.RemoveItem();
                    else
                        fromSlot.RemoveItem(canAdd);

                    return true;
                }
                else
                {
                    // 交换物品
                    IItem fromItem = fromSlot.RemoveItem();
                    IItem toItem = toSlot.RemoveItem();

                    if (fromItem != null && toItem != null)
                    {
                        toSlot.SetItem(fromItem);
                        fromSlot.SetItem(toItem);
                        return true;
                    }
                    else
                    {
                        // 恢复原状
                        if (fromItem != null)
                            fromSlot.SetItem(fromItem);
                        if (toItem != null)
                            toSlot.SetItem(toItem);
                        return false;
                    }
                }
            }
            // 如果目标槽位为空
            else
            {
                IItem item = fromSlot.RemoveItem();
                return toSlot.SetItem(item);
            }
        }

        /// <summary>
        /// 移动指定数量的物品
        /// </summary>
        /// <param name="fromSlotIndex">源槽位索引</param>
        /// <param name="toSlotIndex">目标槽位索引</param>
        /// <param name="amount">数量</param>
        /// <returns>实际移动的数量</returns>
        public int MoveItem(int fromSlotIndex, int toSlotIndex, int amount)
        {
            if (amount <= 0)
                return 0;

            // 检查索引的有效性
            if (!_slots.TryGetValue(fromSlotIndex, out var fromSlot) ||
                !_slots.TryGetValue(toSlotIndex, out var toSlot))
                return 0;

            // 如果源槽位为空或锁定
            if (fromSlot.IsEmpty || fromSlot.IsLocked)
                return 0;

            // 如果目标槽位锁定
            if (toSlot.IsLocked)
                return 0;

            // 如果要移动的数量大于源槽位的物品数量
            if (amount > fromSlot.Item.StackSize)
                amount = fromSlot.Item.StackSize;

            // 如果目标槽位为空
            if (toSlot.IsEmpty)
            {
                IItem item = fromSlot.RemoveItem(amount);
                if (item != null && toSlot.SetItem(item))
                    return amount;
                return 0;
            }
            // 如果物品相同且可堆叠
            else if (fromSlot.Item.Id == toSlot.Item.Id && fromSlot.Item.IsStackable && toSlot.Item.IsStackable)
            {
                int canAdd = Math.Min(amount, toSlot.Item.MaxStackSize - toSlot.Item.StackSize);

                if (canAdd <= 0)
                    return 0;

                toSlot.Item.AddToStack(canAdd);

                if (canAdd >= fromSlot.Item.StackSize)
                    fromSlot.RemoveItem();
                else
                    fromSlot.RemoveItem(canAdd);

                return canAdd;
            }

            return 0;
        }

        /// <summary>
        /// 交换两个槽位的物品
        /// </summary>
        /// <param name="slotIndex1">槽位1索引</param>
        /// <param name="slotIndex2">槽位2索引</param>
        /// <returns>是否交换成功</returns>
        public bool SwapItems(int slotIndex1, int slotIndex2)
        {
            if (!_slots.TryGetValue(slotIndex1, out var slot1) || !_slots.TryGetValue(slotIndex2, out var slot2))
                return false;

            if (slot1.IsLocked || slot2.IsLocked)
                return false;

            if (slot1.IsEmpty && slot2.IsEmpty)
                return true; // 都为空，无需交换

            if (slot1.IsEmpty)
            {
                IItem item = slot2.RemoveItem();
                return slot1.SetItem(item);
            }
            else if (slot2.IsEmpty)
            {
                IItem item = slot1.RemoveItem();
                return slot2.SetItem(item);
            }
            else
            {
                // 检查是否能接受对方的物品
                if (!slot1.CanAcceptItem(slot2.Item) || !slot2.CanAcceptItem(slot1.Item))
                    return false;

                IItem item1 = slot1.RemoveItem();
                IItem item2 = slot2.RemoveItem();

                bool success = true;

                if (!slot2.SetItem(item1))
                {
                    slot1.SetItem(item1);
                    success = false;
                }

                if (!slot1.SetItem(item2))
                {
                    slot2.SetItem(item2);
                    success = false;
                }

                return success;
            }
        }

        /// <summary>
        /// 使用指定槽位的物品
        /// </summary>
        /// <param name="slotIndex">槽位索引</param>
        /// <param name="targetId">目标ID（可选）</param>
        /// <returns>使用结果</returns>
        public (bool success, string message) UseItem(int slotIndex, Guid? targetId = null)
        {
            if (!_slots.TryGetValue(slotIndex, out var slot) || slot.IsEmpty || slot.IsLocked)
                return (false, "物品不存在或槽位已锁定");

            if (!slot.Item.IsUsable)
                return (false, "物品无法使用");

            (bool canUse, string reason) = slot.Item.CanBeUsedBy(OwnerId, targetId);
            if (!canUse)
                return (false, reason);

            (bool success, string message) = slot.Item.Use(OwnerId, targetId);

            // 如果使用成功并且是消耗品且数量用完
            if (success && slot.Item.IsConsumable)
            {
                if (slot.Item.StackSize <= 1)
                    slot.RemoveItem();
                else
                    slot.Item.RemoveFromStack(1);
            }

            return (success, message);
        }

        /// <summary>
        /// 清空背包
        /// </summary>
        /// <param name="reason">清空原因</param>
        /// <returns>移除的物品列表</returns>
        public IReadOnlyList<IItem> Clear(string reason = "")
        {
            List<IItem> removedItems = new List<IItem>();

            foreach (var slot in _slots.Values)
            {
                if (!slot.IsLocked && !slot.IsEmpty)
                {
                    IItem item = slot.RemoveItem();
                    if (item != null)
                    {
                        removedItems.Add(item);
                    }
                }
            }

            return removedItems;
        }

        public IInventorySlot? FindInventorySlotById(Guid? itemId)
        {
            if (itemId == null)
                return null;

            return _slots.Values.FirstOrDefault(s => !s.IsEmpty && !s.IsLocked && s.Item.Id == itemId);
        }

        public IReadOnlyList<IInventorySlot> FindInventoriesSlotByTemplateId(Guid? templateId)
        {
            if (templateId == null)
                return new List<IInventorySlot>();

            return _slots.Values.Where(s => !s.IsEmpty && !s.IsLocked && s.Item.TemplateId == templateId).ToList();
        }
        /// <summary>
        /// 根据物品ID查找槽位索引
        /// </summary>
        /// <param name="itemId">物品ID</param>
        /// <returns>槽位索引</returns>
        public int FindInventorySlotIndexById(Guid? itemId)
        {
            if (itemId == null)
                return -1;
            var findResult = _slots.FirstOrDefault(s => !s.Value.IsEmpty && !s.Value.IsLocked && s.Value.Item.Id == itemId);
            return findResult.Equals(default(KeyValuePair<int, IInventorySlot>)) ? -1 : findResult.Key;
        }
        public IReadOnlyList<int> FindItemSlotIndexByType(ItemType itemType)
        {
            return _slots.Where(s => !s.Value.IsEmpty && !s.Value.IsLocked && s.Value.Item.Type == itemType)
                         .Select(s => s.Key)
                         .ToList();
        }

        /// <summary>
        /// 整理背包
        /// </summary>
        /// <param name="sortBy">排序方式，如：类型、名称、价值等</param>
        /// <returns>是否成功整理</returns>
        public bool Sort(string? sortBy = null)
        {
            // 从配置中获取默认排序策略
            var config = _configManager.GetConfig<InventoryConfig>(InventoryConfig.MODULE_ID);
            string defaultStrategy = config?.DefaultSortStrategy ?? "type";

            // 使用提供的排序方式或默认排序方式
            sortBy = sortBy ?? defaultStrategy;

            IInventorySortStrategy strategy;
            switch (sortBy.ToLower())
            {
                case "name":
                    strategy = new NameSortStrategy();
                    break;
                case "type":
                    strategy = new ItemTypeSortStrategy();
                    break;
                case "rarity":
                    strategy = new RaritySortStrategy();
                    break;
                // case "value":
                //     strategy = new ValueSortStrategy();
                //     break;
                // case "weight":
                //     strategy = new WeightSortStrategy();
                //     break;
                default:
                    strategy = new ItemTypeSortStrategy();
                    break;
            }

            return Sort(strategy);
        }

        /// <summary>
        /// 使用指定的排序策略整理背包
        /// </summary>
        /// <param name="sortStrategy">排序策略</param>
        /// <returns>是否成功整理</returns>
        public bool Sort(IInventorySortStrategy sortStrategy)
        {
            if (sortStrategy == null)
                return false;

            try
            {
                // 获取所有槽位
                var allSlots = _slots.Values.ToList();

                // 使用策略对槽位进行排序
                var sortedSlots = sortStrategy.Sort(allSlots).ToList();

                // 提取所有非空且未锁定的槽位中的物品
                var itemsToReorganize = new List<IItem>();
                foreach (var slot in sortedSlots.Where(s => !s.IsEmpty && !s.IsLocked))
                {
                    var item = slot.RemoveItem();
                    if (item != null)
                    {
                        itemsToReorganize.Add(item);
                    }
                }

                // 按照排序后的顺序将物品放回未锁定的槽位
                int itemIndex = 0;
                foreach (var slot in sortedSlots.Where(s => !s.IsLocked))
                {
                    if (itemIndex < itemsToReorganize.Count)
                    {
                        slot.SetItem(itemsToReorganize[itemIndex]);
                        itemIndex++;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                // 记录错误
                Console.WriteLine($"使用策略整理背包时发生错误: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 检查背包是否包含特定物品
        /// </summary>
        /// <param name="itemId">物品ID</param>
        /// <returns>是否包含</returns>
        public bool ContainsItem(Guid? templateId)
        {
            if (templateId == null)
                return false;
            return _slots.Values.Any(slot => !slot.IsEmpty &&
                                           slot.Item.TemplateId == templateId);
        }

        /// <summary>
        /// 检查背包是否包含指定数量的特定物品
        /// </summary>
        /// <param name="itemId">物品ID</param>
        /// <param name="amount">数量</param>
        /// <returns>是否包含足够数量</returns>
        public bool ContainsItem(Guid? templateId, int amount)
        {
            if (templateId == null || amount <= 0)
                return false;

            int totalAmount = CountItem(templateId);
            return totalAmount >= amount;
        }

        /// <summary>
        /// 计算背包中指定物品的总数量
        /// </summary>
        /// <param name="templateId">物品模板ID</param>
        /// <returns>总数量</returns>
        public int CountItem(Guid? templateId)
        {
            if (templateId == null)
                return 0;

            return _slots.Values
                .Where(slot => !slot.IsEmpty && slot.Item.TemplateId == templateId)
                .Sum(slot => slot.Item.StackSize);
        }

        /// <summary>
        /// 计算背包中指定类型物品的总数量
        /// </summary>
        /// <param name="itemType">物品类型</param>
        /// <returns>总数量</returns>
        public int CountItemsByType(ItemType itemType)
        {
            return _slots.Values
                .Where(slot => !slot.IsEmpty && slot.Item.Type == itemType)
                .Sum(slot => slot.Item.StackSize);
        }


        /// <summary>
        /// 锁定指定槽位
        /// </summary>
        /// <param name="slotIndex">槽位索引</param>
        /// <param name="locked">是否锁定</param>
        /// <returns>是否操作成功</returns>
        public bool SetSlotLocked(int slotIndex, bool locked)
        {
            if (!_slots.TryGetValue(slotIndex, out var slot))
                return false;

            return slot.SetLocked(locked);
        }

        /// <summary>
        /// 锁定所有槽位
        /// </summary>
        /// <param name="locked">是否锁定</param>
        public void LockAllSlots(bool locked)
        {
            foreach (var slot in _slots.Values)
            {
                slot.SetLocked(locked);
            }
        }

        /// <summary>
        /// 设置槽位标签
        /// </summary>
        /// <param name="slotIndex">槽位索引</param>
        /// <param name="label">标签</param>
        /// <returns>是否设置成功</returns>
        public bool SetSlotLabel(int slotIndex, string label)
        {
            if (!_slots.TryGetValue(slotIndex, out var slot))
                return false;

            slot.SetLabel(label);
            return true;
        }

        /// <summary>
        /// 合并堆叠
        /// </summary>
        /// <returns>合并的堆叠数量</returns>
        public int MergeStacks()
        {
            int mergeCount = 0;
            var config = _configManager.GetConfig<InventoryConfig>(InventoryConfig.MODULE_ID);
            int maxStackSize = config?.MaxStackSize ?? 99;

            // 按类型和稀有度分组
            var itemGroups = _slots.Values
                .Where(slot => !slot.IsEmpty && !slot.IsLocked && slot.Item.IsStackable)
                .GroupBy(slot => GetItemGroupKey(slot.Item))
                .ToList();

            foreach (var group in itemGroups)
            {
                var slots = group.OrderByDescending(slot => slot.Item.StackSize).ToList();

                // 如果该组只有一个槽位，跳过
                if (slots.Count <= 1)
                    continue;

                for (int i = 0; i < slots.Count - 1; i++)
                {
                    var sourceSlot = slots[i];
                    // 如果已经满堆叠，或被锁定，跳过
                    if (sourceSlot.Item.StackSize >= (sourceSlot.Item.MaxStackSize > 0 ? sourceSlot.Item.MaxStackSize : maxStackSize)
                        || sourceSlot.IsLocked)
                        continue;

                    for (int j = i + 1; j < slots.Count; j++)
                    {
                        var targetSlot = slots[j];
                        if (targetSlot.IsLocked)
                            continue;

                        // 计算源槽位可以接收的数量
                        int canReceive = (sourceSlot.Item.MaxStackSize > 0 ? sourceSlot.Item.MaxStackSize : maxStackSize) - sourceSlot.Item.StackSize;

                        if (canReceive <= 0)
                            break;  // 源槽位已满

                        // 计算目标槽位可以提供的数量
                        int canProvide = Math.Min(canReceive, targetSlot.Item.StackSize);

                        if (canProvide <= 0)
                            continue;  // 目标槽位没有物品

                        // 从目标槽位移除物品
                        var tempItem = targetSlot.RemoveItem(canProvide);

                        if (tempItem != null)
                        {
                            // 合并到源槽位
                            var remaining = sourceSlot.MergeItem(tempItem);

                            // 如果有剩余，放回目标槽位
                            if (remaining != null)
                            {
                                targetSlot.SetItem(remaining);
                            }
                            else
                            {
                                mergeCount++;
                            }

                            // 如果源槽位已满，跳出内层循环
                            if (sourceSlot.Item.StackSize >= (sourceSlot.Item.MaxStackSize > 0 ? sourceSlot.Item.MaxStackSize : maxStackSize))
                                break;
                        }
                    }
                }
            }

            return mergeCount;
        }
        /// <summary>
        /// 设置槽位的可接受物品类型
        /// </summary>
        /// <param name="slotIndex">槽位索引</param>
        /// <param name="itemType">物品类型</param>
        /// <returns>是否设置成功</returns>
        public bool SetSlotAcceptedItemType(int slotIndex, ItemType? itemType)
        {
            if (!_slots.TryGetValue(slotIndex, out var slot))
                return false;

            return slot.SetAcceptedItemType(itemType);
        }




        /// <summary>
        /// 获取物品的分组键（用于堆叠分组）
        /// </summary>
        /// <param name="item">物品实例</param>
        /// <returns>分组键</returns>
        private string GetItemGroupKey(IItem item)
        {
            return $"{item.TemplateId}_{item.Type}_{item.Rarity}";
        }

        /// <summary>
        /// 检查两个物品是否可以堆叠在一起
        /// </summary>
        /// <param name="item1">物品1</param>
        /// <param name="item2">物品2</param>
        /// <returns>是否可以堆叠</returns>
        private bool AreItemsSameType(IItem item1, IItem item2)
        {
            return item1.TemplateId == item2.TemplateId &&
                   item1.Type == item2.Type &&
                   item1.Rarity == item2.Rarity;
        }

        /// <summary>
        /// 获取自定义属性
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>值</returns>
        public object GetProperty(string key)
        {
            if (string.IsNullOrEmpty(key) || !_properties.ContainsKey(key))
                return null;

            return _properties[key];
        }

        /// <summary>
        /// 设置自定义属性
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public void SetProperty(string key, object value)
        {
            if (string.IsNullOrEmpty(key))
                return;

            _properties[key] = value;
        }

        /// <summary>
        /// 获取所有物品的ID列表
        /// </summary>
        /// <returns>物品ID列表</returns>
        public IReadOnlyList<Guid> GetAllItemIds()
        {
            return _slots.Values.Where(slot => !slot.IsEmpty && !slot.IsLocked)
                                .Select(slot => slot.Item.Id)
                                .ToList();
        }
    }
}