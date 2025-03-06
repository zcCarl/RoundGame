using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using TacticalRPG.Core.Modules.Inventory;

namespace TacticalRPG.Implementation.Modules.Inventory
{
    /// <summary>
    /// 掉落物管理器实现类
    /// </summary>
    public class DropManager : IDropManager
    {
        private readonly Dictionary<Guid, IDrop> _drops = new Dictionary<Guid, IDrop>();
        private readonly Dictionary<string, Dictionary<string, int>> _lootTables = new Dictionary<string, Dictionary<string, int>>();
        private readonly IItemManager _itemManager;
        private readonly ILogger<DropManager> _logger;
        private readonly Random _random = new Random();

        /// <summary>
        /// 当前游戏世界中的所有掉落物
        /// </summary>
        public IReadOnlyDictionary<Guid, IDrop> ActiveDrops => _drops;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="itemManager">物品管理器</param>
        /// <param name="logger">日志记录器</param>
        public DropManager(IItemManager itemManager, ILogger<DropManager> logger)
        {
            _itemManager = itemManager ?? throw new ArgumentNullException(nameof(itemManager));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// 创建掉落物
        /// </summary>
        /// <param name="x">X坐标</param>
        /// <param name="y">Y坐标</param>
        /// <param name="items">物品列表</param>
        /// <param name="existDuration">存在时间（秒），0表示永久存在</param>
        /// <returns>创建的掉落物</returns>
        public IDrop CreateDrop(int x, int y, IReadOnlyList<IItem> items, int existDuration = 60)
        {
            var drop = new Drop(x, y, items, existDuration);
            _drops[drop.Id] = drop;
            _logger.LogInformation($"创建掉落物 ID: {drop.Id} 在位置 ({x}, {y})，包含 {items.Count} 个物品");
            return drop;
        }

        /// <summary>
        /// 创建掉落物
        /// </summary>
        /// <param name="x">X坐标</param>
        /// <param name="y">Y坐标</param>
        /// <param name="item">物品</param>
        /// <param name="existDuration">存在时间（秒），0表示永久存在</param>
        /// <returns>创建的掉落物</returns>
        public IDrop CreateDrop(int x, int y, IItem item, int existDuration = 60)
        {
            return CreateDrop(x, y, new List<IItem> { item }, existDuration);
        }

        /// <summary>
        /// 创建随机掉落物
        /// </summary>
        /// <param name="x">X坐标</param>
        /// <param name="y">Y坐标</param>
        /// <param name="lootTable">掉落表ID</param>
        /// <param name="existDuration">存在时间（秒），0表示永久存在</param>
        /// <returns>创建的掉落物</returns>
        public IDrop CreateRandomDrop(int x, int y, string lootTable, int existDuration = 60)
        {
            if (!_lootTables.TryGetValue(lootTable, out var table))
            {
                _logger.LogWarning($"掉落表 {lootTable} 不存在");
                return null;
            }

            var items = new List<IItem>();
            var totalWeight = table.Values.Sum();

            // 随机决定掉落数量（1-3个物品）
            var dropCount = _random.Next(1, 4);

            for (int i = 0; i < dropCount; i++)
            {
                var roll = _random.Next(1, totalWeight + 1);
                var currentWeight = 0;

                foreach (var entry in table)
                {
                    currentWeight += entry.Value;
                    if (roll <= currentWeight)
                    {
                        var item = _itemManager.CreateItem(entry.Key);
                        if (item != null)
                        {
                            items.Add(item);
                        }
                        break;
                    }
                }
            }

            if (items.Count == 0)
            {
                _logger.LogWarning($"从掉落表 {lootTable} 生成的掉落物没有任何物品");
                return null;
            }

            return CreateDrop(x, y, items, existDuration);
        }

        /// <summary>
        /// 移除掉落物
        /// </summary>
        /// <param name="dropId">掉落物ID</param>
        /// <returns>是否成功移除</returns>
        public bool RemoveDrop(Guid dropId)
        {
            if (_drops.TryGetValue(dropId, out var drop))
            {
                _drops.Remove(dropId);
                _logger.LogInformation($"移除掉落物 ID: {dropId}");
                return true;
            }

            _logger.LogWarning($"尝试移除不存在的掉落物 ID: {dropId}");
            return false;
        }

        /// <summary>
        /// 拾取掉落物
        /// </summary>
        /// <param name="dropId">掉落物ID</param>
        /// <param name="characterId">角色ID</param>
        /// <returns>拾取结果</returns>
        public (bool success, string message, IReadOnlyList<IItem> items) PickupDrop(Guid dropId, Guid characterId)
        {
            if (!_drops.TryGetValue(dropId, out var drop))
            {
                return (false, "掉落物不存在", new List<IItem>());
            }

            var (canPickup, reason) = drop.CanBePickedUpBy(characterId);
            if (!canPickup)
            {
                return (false, reason, new List<IItem>());
            }

            var items = drop.Items.ToList();
            drop.MarkAsPickedUp(characterId);

            _logger.LogInformation($"角色 {characterId} 拾取掉落物 ID: {dropId}，获得 {items.Count} 个物品");
            return (true, $"成功拾取 {items.Count} 个物品", items);
        }

        /// <summary>
        /// 获取掉落物
        /// </summary>
        /// <param name="dropId">掉落物ID</param>
        /// <returns>掉落物</returns>
        public IDrop GetDrop(Guid dropId)
        {
            if (_drops.TryGetValue(dropId, out var drop))
            {
                return drop;
            }

            return null;
        }

        /// <summary>
        /// 获取指定位置的掉落物
        /// </summary>
        /// <param name="x">X坐标</param>
        /// <param name="y">Y坐标</param>
        /// <returns>掉落物列表</returns>
        public IReadOnlyList<IDrop> GetDropsAtPosition(int x, int y)
        {
            return _drops.Values
                .Where(d => !d.IsPickedUp && !d.IsExpired() && d.X == x && d.Y == y)
                .ToList();
        }

        /// <summary>
        /// 获取指定区域的掉落物
        /// </summary>
        /// <param name="x">中心X坐标</param>
        /// <param name="y">中心Y坐标</param>
        /// <param name="radius">半径</param>
        /// <returns>掉落物列表</returns>
        public IReadOnlyList<IDrop> GetDropsInRadius(int x, int y, int radius)
        {
            return _drops.Values
                .Where(d => !d.IsPickedUp && !d.IsExpired() &&
                           Math.Sqrt(Math.Pow(d.X - x, 2) + Math.Pow(d.Y - y, 2)) <= radius)
                .ToList();
        }

        /// <summary>
        /// 清理已拾取的掉落物
        /// </summary>
        /// <returns>清理的掉落物数量</returns>
        public int CleanupPickedDrops()
        {
            var pickedDrops = _drops.Values.Where(d => d.IsPickedUp).ToList();
            foreach (var drop in pickedDrops)
            {
                _drops.Remove(drop.Id);
            }

            _logger.LogInformation($"清理了 {pickedDrops.Count} 个已拾取的掉落物");
            return pickedDrops.Count;
        }

        /// <summary>
        /// 清理过期的掉落物
        /// </summary>
        /// <returns>清理的掉落物数量</returns>
        public int CleanupExpiredDrops()
        {
            var expiredDrops = _drops.Values.Where(d => !d.IsPickedUp && d.IsExpired()).ToList();
            foreach (var drop in expiredDrops)
            {
                _drops.Remove(drop.Id);
            }

            _logger.LogInformation($"清理了 {expiredDrops.Count} 个过期的掉落物");
            return expiredDrops.Count;
        }

        /// <summary>
        /// 注册掉落表
        /// </summary>
        /// <param name="lootTableId">掉落表ID</param>
        /// <param name="items">物品列表及其权重</param>
        public void RegisterLootTable(string lootTableId, Dictionary<string, int> items)
        {
            if (string.IsNullOrEmpty(lootTableId))
            {
                throw new ArgumentException("掉落表ID不能为空", nameof(lootTableId));
            }

            if (items == null || items.Count == 0)
            {
                throw new ArgumentException("掉落表物品不能为空", nameof(items));
            }

            _lootTables[lootTableId] = new Dictionary<string, int>(items);
            _logger.LogInformation($"注册掉落表 {lootTableId}，包含 {items.Count} 个物品");
        }

        /// <summary>
        /// 获取掉落表
        /// </summary>
        /// <param name="lootTableId">掉落表ID</param>
        /// <returns>掉落表</returns>
        public Dictionary<string, int> GetLootTable(string lootTableId)
        {
            if (_lootTables.TryGetValue(lootTableId, out var table))
            {
                return new Dictionary<string, int>(table);
            }

            return null;
        }
    }
}