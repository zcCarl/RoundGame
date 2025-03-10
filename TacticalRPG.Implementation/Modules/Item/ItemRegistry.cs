using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using TacticalRPG.Core.Modules.Item;

namespace TacticalRPG.Implementation.Modules.Item
{
    /// <summary>
    /// 物品注册表，负责物品实例和位置信息的集中管理
    /// </summary>
    internal class ItemRegistry
    {
        private readonly Dictionary<Guid, IItem> _items = new Dictionary<Guid, IItem>();
        private readonly Dictionary<Guid, ItemLocation> _itemLocations = new Dictionary<Guid, ItemLocation>();
        private readonly ILogger _logger;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="logger">日志记录器</param>
        public ItemRegistry(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// 注册物品
        /// </summary>
        /// <param name="item">物品实例</param>
        /// <returns>物品ID</returns>
        public Guid RegisterItem(IItem item)
        {
            if (item == null)
            {
                _logger.LogWarning("注册物品失败：物品为空");
                return Guid.Empty;
            }

            var itemId = item.Id;
            if (itemId == Guid.Empty)
            {
                _logger.LogWarning("注册物品失败：物品ID为空");
                return Guid.Empty;
            }

            if (_items.ContainsKey(itemId))
            {
                _logger.LogWarning($"注册物品失败：物品ID {itemId} 已存在");
                return Guid.Empty;
            }

            _items[itemId] = item;
            _itemLocations[itemId] = ItemLocation.Unassigned;

            _logger.LogInformation($"注册物品成功：{item.Name}，ID：{itemId}");
            return itemId;
        }

        /// <summary>
        /// 获取物品实例
        /// </summary>
        /// <param name="itemId">物品ID</param>
        /// <returns>物品实例</returns>
        public IItem GetItem(Guid itemId)
        {
            if (_items.TryGetValue(itemId, out var item))
            {
                return item;
            }

            _logger.LogWarning($"获取物品失败：找不到ID为 {itemId} 的物品");
            return null;
        }

        /// <summary>
        /// 获取物品位置信息
        /// </summary>
        /// <param name="itemId">物品ID</param>
        /// <returns>物品位置信息</returns>
        public ItemLocation GetItemLocation(Guid itemId)
        {
            if (_itemLocations.TryGetValue(itemId, out var location))
            {
                return location;
            }

            _logger.LogWarning($"获取物品位置失败：找不到ID为 {itemId} 的物品位置信息");
            return ItemLocation.Unassigned;
        }

        /// <summary>
        /// 更新物品位置信息
        /// </summary>
        /// <param name="itemId">物品ID</param>
        /// <param name="location">新的位置信息</param>
        /// <returns>是否成功更新</returns>
        public bool UpdateItemLocation(Guid itemId, ItemLocation location)
        {
            if (!_items.ContainsKey(itemId))
            {
                _logger.LogWarning($"更新物品位置失败：找不到ID为 {itemId} 的物品");
                return false;
            }

            _itemLocations[itemId] = location;
            _logger.LogInformation($"更新物品位置成功：物品ID：{itemId}，容器类型：{location.ContainerType}，容器ID：{location.ContainerId}，槽位：{location.SlotIndex}");
            return true;
        }

        /// <summary>
        /// 移除物品
        /// </summary>
        /// <param name="itemId">物品ID</param>
        /// <returns>是否成功移除</returns>
        public bool RemoveItem(Guid itemId)
        {
            if (!_items.ContainsKey(itemId))
            {
                _logger.LogWarning($"移除物品失败：找不到ID为 {itemId} 的物品");
                return false;
            }

            var item = _items[itemId];
            _items.Remove(itemId);
            _itemLocations.Remove(itemId);

            _logger.LogInformation($"移除物品成功：{item.Name}，ID：{itemId}");
            return true;
        }

        /// <summary>
        /// 根据所有者ID获取物品列表
        /// </summary>
        /// <param name="ownerId">所有者ID</param>
        /// <returns>物品ID列表</returns>
        public IEnumerable<Guid> GetItemsByOwner(Guid ownerId)
        {
            return _itemLocations
                .Where(kv => kv.Value.OwnerId == ownerId)
                .Select(kv => kv.Key);
        }

        /// <summary>
        /// 根据容器类型和容器ID获取物品列表
        /// </summary>
        /// <param name="containerType">容器类型</param>
        /// <param name="containerId">容器ID</param>
        /// <returns>物品ID列表</returns>
        public IEnumerable<Guid> GetItemsByContainer(ItemContainerType containerType, Guid containerId)
        {
            return _itemLocations
                .Where(kv => kv.Value.ContainerType == containerType && kv.Value.ContainerId == containerId)
                .Select(kv => kv.Key);
        }

        /// <summary>
        /// 获取所有物品
        /// </summary>
        /// <returns>物品字典</returns>
        public IReadOnlyDictionary<Guid, IItem> GetItems()
        {
            return _items;
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <returns>序列化的数据</returns>
        public string SaveData()
        {
            var data = new ItemRegistryData
            {
                Items = _items.ToDictionary(kv => kv.Key, kv => new SerializableItem
                {
                    // 这里需要根据实际情况序列化物品数据
                    // 例如：物品ID、模板ID、堆叠数量等基本信息
                    Id = kv.Key,
                    TemplateId = kv.Value.TemplateId.ToString(),
                    StackSize = kv.Value.StackSize,
                    // 其他需要保存的物品属性...
                }),
                ItemLocations = _itemLocations
            };

            var options = new JsonSerializerOptions { WriteIndented = true };
            return JsonSerializer.Serialize(data, options);
        }

        /// <summary>
        /// 加载数据
        /// </summary>
        /// <param name="serializedData">序列化的数据</param>
        /// <returns>是否成功加载</returns>
        public bool LoadData(string serializedData)
        {
            try
            {
                var data = JsonSerializer.Deserialize<ItemRegistryData>(serializedData);
                if (data == null)
                {
                    _logger.LogWarning("加载物品数据失败：反序列化结果为空");
                    return false;
                }

                // 这里需要根据实际情况反序列化并创建物品实例
                // 例如：先清空当前数据，然后重新创建物品实例和位置信息

                _items.Clear();
                _itemLocations.Clear();

                foreach (var item in data.ItemLocations)
                {
                    _itemLocations[item.Key] = item.Value;
                }

                // 此处实际应用中，需要通过物品工厂或模板来重新创建物品实例
                // ...

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "加载物品数据异常");
                return false;
            }
        }
    }

    /// <summary>
    /// 物品注册表数据
    /// </summary>
    internal class ItemRegistryData
    {
        /// <summary>
        /// 物品数据
        /// </summary>
        public Dictionary<Guid, SerializableItem> Items { get; set; }

        /// <summary>
        /// 物品位置信息
        /// </summary>
        public Dictionary<Guid, ItemLocation> ItemLocations { get; set; }
    }

    /// <summary>
    /// 可序列化的物品数据
    /// </summary>
    internal class SerializableItem
    {
        /// <summary>
        /// 物品ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 物品模板ID
        /// </summary>
        public string TemplateId { get; set; }

        /// <summary>
        /// 堆叠数量
        /// </summary>
        public int StackSize { get; set; }

        // 其他需要序列化的物品属性...
    }
}