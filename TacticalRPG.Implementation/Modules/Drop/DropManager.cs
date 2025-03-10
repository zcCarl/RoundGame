using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TacticalRPG.Core.Modules.Drop;
using TacticalRPG.Core.Modules.Item;

namespace TacticalRPG.Implementation.Modules.Drop
{
    /// <summary>
    /// 掉落物管理器实现类
    /// </summary>
    public class DropManager : IDropManager
    {
        private readonly Dictionary<Guid, IDrop> _drops = new Dictionary<Guid, IDrop>();
        private readonly Dictionary<string, Dictionary<string, int>> _lootTables = new Dictionary<string, Dictionary<string, int>>();
        private readonly IItemModule _itemModule;
        private readonly ILogger<DropManager> _logger;
        private readonly Random _random = new Random();

        /// <summary>
        /// 当前游戏世界中的所有掉落物
        /// </summary>
        public IReadOnlyDictionary<Guid, IDrop> ActiveDrops => _drops;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="itemModule">物品模块</param>
        /// <param name="logger">日志记录器</param>
        public DropManager(IItemModule itemModule, ILogger<DropManager> logger)
        {
            _itemModule = itemModule ?? throw new ArgumentNullException(nameof(itemModule));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// 创建掉落物
        /// </summary>
        /// <param name="x">X坐标</param>
        /// <param name="y">Y坐标</param>
        /// <param name="itemIds">物品ID列表</param>
        /// <param name="existDuration">存在时间（秒），0表示永久存在</param>
        /// <returns>创建的掉落物</returns>
        public IDrop CreateDrop(int x, int y, IReadOnlyList<Guid> itemIds, int existDuration = 60)
        {
            var drop = new Drop(x, y, itemIds, existDuration);
            _drops[drop.Id] = drop;
            _logger.LogInformation($"创建掉落物 ID: {drop.Id} 在位置 ({x}, {y})，包含 {itemIds.Count} 个物品");
            return drop;
        }

        /// <summary>
        /// 创建掉落物
        /// </summary>
        /// <param name="x">X坐标</param>
        /// <param name="y">Y坐标</param>
        /// <param name="itemId">物品ID</param>
        /// <param name="existDuration">存在时间（秒），0表示永久存在</param>
        /// <returns>创建的掉落物</returns>
        public IDrop CreateDrop(int x, int y, Guid itemId, int existDuration = 60)
        {
            return CreateDrop(x, y, new List<Guid> { itemId }, existDuration);
        }

        /// <summary>
        /// 创建随机掉落物
        /// </summary>
        /// <param name="x">X坐标</param>
        /// <param name="y">Y坐标</param>
        /// <param name="lootTableId">掉落表ID</param>
        /// <param name="existDuration">存在时间（秒），0表示永久存在</param>
        /// <returns>创建的掉落物</returns>
        public IDrop CreateRandomDrop(int x, int y, string lootTableId, int existDuration = 60)
        {
            if (!_lootTables.TryGetValue(lootTableId, out var table))
            {
                _logger.LogWarning($"掉落表 {lootTableId} 不存在");
                return null;
            }

            var itemIds = new List<Guid>();
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
                        // 创建物品并获取ID (异步方法的同步调用，仅在此管理器内部使用)
                        var itemId = _itemModule.CreateItemAsync(entry.Key, 1).GetAwaiter().GetResult();
                        if (itemId != Guid.Empty)
                        {
                            itemIds.Add(itemId);
                        }
                        break;
                    }
                }
            }

            if (itemIds.Count == 0)
            {
                _logger.LogWarning($"从掉落表 {lootTableId} 生成的掉落物没有任何物品");
                return null;
            }

            return CreateDrop(x, y, itemIds, existDuration);
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
        public (bool success, string message, IReadOnlyList<Guid> itemIds) PickupDrop(Guid dropId, Guid characterId)
        {
            if (!_drops.TryGetValue(dropId, out var drop))
            {
                return (false, "掉落物不存在", new List<Guid>());
            }

            var (canPickup, reason) = drop.CanBePickedUpBy(characterId);
            if (!canPickup)
            {
                return (false, reason, new List<Guid>());
            }

            var itemIds = drop.ItemIds.ToList();
            drop.MarkAsPickedUp(characterId);

            _logger.LogInformation($"角色 {characterId} 拾取掉落物 ID: {dropId}，获得 {itemIds.Count} 个物品");
            return (true, $"成功拾取 {itemIds.Count} 个物品", itemIds);
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

            if (pickedDrops.Count > 0)
            {
                _logger.LogInformation($"清理了 {pickedDrops.Count} 个已拾取的掉落物");
            }

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

            if (expiredDrops.Count > 0)
            {
                _logger.LogInformation($"清理了 {expiredDrops.Count} 个过期的掉落物");
            }

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
            _logger.LogInformation($"注册掉落表 {lootTableId}，包含 {items.Count} 个物品条目");
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

        /// <summary>
        /// 加载掉落数据
        /// </summary>
        /// <param name="data">掉落数据</param>
        /// <returns>加载结果</returns>
        public Task<bool> LoadDropDataAsync(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                _logger.LogWarning("尝试加载空的掉落数据");
                return Task.FromResult(false);
            }

            try
            {
                var dropData = JsonSerializer.Deserialize<DropData>(data);
                if (dropData == null)
                {
                    _logger.LogWarning("反序列化掉落数据失败");
                    return Task.FromResult(false);
                }

                // 清空当前掉落物和掉落表
                _drops.Clear();
                _lootTables.Clear();

                // 加载掉落表
                foreach (var lootTable in dropData.LootTables)
                {
                    _lootTables[lootTable.Key] = lootTable.Value;
                }

                // 加载掉落物
                foreach (var dropInfo in dropData.Drops)
                {
                    var drop = new Drop(
                        dropInfo.X,
                        dropInfo.Y,
                        dropInfo.ItemIds,
                        dropInfo.ExistDuration);

                    _drops[drop.Id] = drop;

                    // 如果已被拾取，恢复拾取状态
                    if (dropInfo.IsPickedUp && dropInfo.PickedUpByCharacterId.HasValue)
                    {
                        drop.MarkAsPickedUp(dropInfo.PickedUpByCharacterId.Value);
                    }
                }

                _logger.LogInformation($"成功加载掉落数据: {_drops.Count} 个掉落物, {_lootTables.Count} 个掉落表");
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "加载掉落数据时发生错误");
                return Task.FromResult(false);
            }
        }

        /// <summary>
        /// 保存掉落数据
        /// </summary>
        /// <returns>保存的数据</returns>
        public Task<string> SaveDropDataAsync()
        {
            try
            {
                var dropData = new DropData
                {
                    LootTables = new Dictionary<string, Dictionary<string, int>>(_lootTables),
                    Drops = _drops.Values.Select(d => new DropInfo
                    {
                        Id = d.Id,
                        X = d.X,
                        Y = d.Y,
                        ItemIds = d.ItemIds.ToList(),
                        CreationTime = d.CreationTime,
                        ExistDuration = d.ExistDuration,
                        IsPickedUp = d.IsPickedUp,
                        PickedUpByCharacterId = d.PickedUpByCharacterId,
                        PickupTime = d.PickupTime
                    }).ToList()
                };

                var options = new JsonSerializerOptions { WriteIndented = true };
                string jsonData = JsonSerializer.Serialize(dropData, options);

                _logger.LogInformation($"成功保存掉落数据: {_drops.Count} 个掉落物, {_lootTables.Count} 个掉落表");
                return Task.FromResult(jsonData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "保存掉落数据时发生错误");
                return Task.FromResult(string.Empty);
            }
        }

        #region 数据模型

        /// <summary>
        /// 掉落数据
        /// </summary>
        private class DropData
        {
            /// <summary>
            /// 掉落表
            /// </summary>
            public Dictionary<string, Dictionary<string, int>> LootTables { get; set; } = new Dictionary<string, Dictionary<string, int>>();

            /// <summary>
            /// 掉落物信息
            /// </summary>
            public List<DropInfo> Drops { get; set; } = new List<DropInfo>();
        }

        /// <summary>
        /// 掉落物信息
        /// </summary>
        private class DropInfo
        {
            /// <summary>
            /// 掉落物ID
            /// </summary>
            public Guid Id { get; set; }

            /// <summary>
            /// X坐标
            /// </summary>
            public int X { get; set; }

            /// <summary>
            /// Y坐标
            /// </summary>
            public int Y { get; set; }

            /// <summary>
            /// 物品ID列表
            /// </summary>
            public List<Guid> ItemIds { get; set; } = new List<Guid>();

            /// <summary>
            /// 创建时间
            /// </summary>
            public DateTime CreationTime { get; set; }

            /// <summary>
            /// 存在时间（秒）
            /// </summary>
            public int ExistDuration { get; set; }

            /// <summary>
            /// 是否已被拾取
            /// </summary>
            public bool IsPickedUp { get; set; }

            /// <summary>
            /// 拾取角色ID
            /// </summary>
            public Guid? PickedUpByCharacterId { get; set; }

            /// <summary>
            /// 拾取时间
            /// </summary>
            public DateTime? PickupTime { get; set; }
        }

        #endregion
    }
}