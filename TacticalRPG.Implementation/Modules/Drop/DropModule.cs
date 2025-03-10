using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TacticalRPG.Core.Framework;
using TacticalRPG.Core.Framework.Events;
using TacticalRPG.Core.Modules.Drop;
using TacticalRPG.Core.Modules.Item;

namespace TacticalRPG.Implementation.Modules.Drop
{
    /// <summary>
    /// 掉落模块实现类
    /// </summary>
    public class DropModule : BaseGameModule, IDropModule
    {
        private readonly IDropManager _dropManager;
        private readonly IItemModule _itemModule;
        private readonly ILogger<DropModule> _logger;
        private readonly IEventManager _eventManager;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="gameSystem">游戏系统</param>
        /// <param name="logger">日志记录器</param>
        /// <param name="dropManager">掉落管理器</param>
        /// <param name="itemModule">物品模块</param>
        public DropModule(
            IGameSystem gameSystem,
            ILogger<DropModule> logger,
            IDropManager dropManager,
            IItemModule itemModule)
            : base(gameSystem, logger)
        {
            _dropManager = dropManager ?? throw new ArgumentNullException(nameof(dropManager));
            _itemModule = itemModule ?? throw new ArgumentNullException(nameof(itemModule));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _eventManager = gameSystem.EventManager;
        }

        /// <summary>
        /// 模块名称
        /// </summary>
        public override string ModuleName => "掉落模块";

        /// <summary>
        /// 模块优先级
        /// </summary>
        public override int Priority => 55; // 物品模块之后初始化

        /// <summary>
        /// 初始化模块
        /// </summary>
        public override async Task Initialize()
        {
            _logger.LogInformation("开始初始化掉落模块");
            await base.Initialize();
            _logger.LogInformation("掉落模块初始化完成");
        }

        /// <summary>
        /// 启动模块
        /// </summary>
        public override async Task Start()
        {
            _logger.LogInformation("开始启动掉落模块");
            // 启动定期清理任务
            await base.Start();
            _logger.LogInformation("掉落模块启动完成");
        }

        /// <summary>
        /// 停止模块
        /// </summary>
        public override async Task Stop()
        {
            _logger.LogInformation("开始停止掉落模块");
            // 清理所有掉落物
            await CleanupDropsAsync();
            await base.Stop();
            _logger.LogInformation("掉落模块停止完成");
        }

        /// <summary>
        /// 创建掉落物
        /// </summary>
        /// <param name="position">位置坐标</param>
        /// <param name="itemIds">物品ID列表</param>
        /// <returns>掉落物ID</returns>
        public async Task<Guid> CreateDropAsync((int x, int y) position, IReadOnlyList<Guid> itemIds)
        {
            if (itemIds == null || itemIds.Count == 0)
            {
                _logger.LogWarning($"尝试创建空掉落物，位置: ({position.x}, {position.y})");
                return Guid.Empty;
            }

            // 验证物品ID是否有效
            foreach (var itemId in itemIds)
            {
                var item = await _itemModule.GetItemAsync(itemId);
                if (item == null)
                {
                    _logger.LogWarning($"物品ID: {itemId} 不存在，无法创建掉落物");
                    return Guid.Empty;
                }
            }

            var drop = _dropManager.CreateDrop(position.x, position.y, itemIds);
            if (drop != null)
            {
                // 发布掉落物创建事件
                _eventManager.Publish(new DropCreatedEvent(drop.Id, position.x, position.y, itemIds));
                _logger.LogInformation($"创建掉落物 ID: {drop.Id}，位置: ({position.x}, {position.y})，包含 {itemIds.Count} 个物品");
                return drop.Id;
            }

            return Guid.Empty;
        }

        /// <summary>
        /// 通过掉落表创建随机掉落物
        /// </summary>
        /// <param name="position">位置坐标</param>
        /// <param name="lootTableId">掉落表ID</param>
        /// <param name="existDuration">存在时间（秒），0表示永久存在</param>
        /// <returns>掉落物ID</returns>
        public async Task<Guid> CreateRandomDropAsync((int x, int y) position, string lootTableId, int existDuration = 60)
        {
            var drop = _dropManager.CreateRandomDrop(position.x, position.y, lootTableId, existDuration);
            if (drop != null)
            {
                // 发布掉落物创建事件
                _eventManager.Publish(new DropCreatedEvent(drop.Id, position.x, position.y, drop.ItemIds));
                _logger.LogInformation($"通过掉落表 {lootTableId} 创建随机掉落物 ID: {drop.Id}，位置: ({position.x}, {position.y})");
                return drop.Id;
            }

            _logger.LogWarning($"通过掉落表 {lootTableId} 创建随机掉落物失败");
            return Guid.Empty;
        }

        /// <summary>
        /// 拾取掉落物
        /// </summary>
        /// <param name="dropId">掉落物ID</param>
        /// <param name="characterId">拾取角色ID</param>
        /// <returns>拾取结果</returns>
        public async Task<(bool success, string message, IReadOnlyList<Guid> itemIds)> PickupDropAsync(Guid dropId, Guid characterId)
        {
            var (success, message, itemIds) = _dropManager.PickupDrop(dropId, characterId);
            if (success)
            {
                // 发布掉落物拾取事件
                _eventManager.Publish(new DropPickedUpEvent(dropId, characterId, itemIds));
                _logger.LogInformation($"角色 {characterId} 拾取掉落物 ID: {dropId}，获得 {itemIds.Count} 个物品");
            }
            else
            {
                _logger.LogWarning($"角色 {characterId} 拾取掉落物 ID: {dropId} 失败: {message}");
            }

            return (success, message, itemIds);
        }

        /// <summary>
        /// 获取掉落物信息
        /// </summary>
        /// <param name="dropId">掉落物ID</param>
        /// <returns>掉落物信息</returns>
        public Task<IDrop> GetDropAsync(Guid dropId)
        {
            var drop = _dropManager.GetDrop(dropId);
            return Task.FromResult(drop);
        }

        /// <summary>
        /// 获取指定位置的掉落物
        /// </summary>
        /// <param name="position">位置坐标</param>
        /// <returns>掉落物ID列表</returns>
        public Task<IReadOnlyList<Guid>> GetDropsAtPositionAsync((int x, int y) position)
        {
            var drops = _dropManager.GetDropsAtPosition(position.x, position.y);
            return Task.FromResult<IReadOnlyList<Guid>>(drops.Select(d => d.Id).ToList());
        }

        /// <summary>
        /// 获取指定区域内的掉落物
        /// </summary>
        /// <param name="position">中心位置坐标</param>
        /// <param name="radius">区域半径</param>
        /// <returns>掉落物ID列表</returns>
        public Task<IReadOnlyList<Guid>> GetDropsInRadiusAsync((int x, int y) position, int radius)
        {
            var drops = _dropManager.GetDropsInRadius(position.x, position.y, radius);
            return Task.FromResult<IReadOnlyList<Guid>>(drops.Select(d => d.Id).ToList());
        }

        /// <summary>
        /// 移除掉落物
        /// </summary>
        /// <param name="dropId">掉落物ID</param>
        /// <returns>是否成功移除</returns>
        public Task<bool> RemoveDropAsync(Guid dropId)
        {
            var result = _dropManager.RemoveDrop(dropId);
            if (result)
            {
                // 发布掉落物移除事件
                _eventManager.Publish(new DropRemovedEvent(dropId, "手动移除"));
                _logger.LogInformation($"掉落物 ID: {dropId} 已被手动移除");
            }
            return Task.FromResult(result);
        }

        /// <summary>
        /// 清理已拾取或过期的掉落物
        /// </summary>
        /// <returns>清理的掉落物数量</returns>
        public Task<int> CleanupDropsAsync()
        {
            var pickedCount = _dropManager.CleanupPickedDrops();
            var expiredCount = _dropManager.CleanupExpiredDrops();
            var totalCount = pickedCount + expiredCount;

            if (totalCount > 0)
            {
                _logger.LogInformation($"清理了 {totalCount} 个掉落物，其中已拾取: {pickedCount}，已过期: {expiredCount}");
            }

            return Task.FromResult(totalCount);
        }

        /// <summary>
        /// 注册掉落表
        /// </summary>
        /// <param name="lootTableId">掉落表ID</param>
        /// <param name="entries">掉落表条目（物品模板ID及其权重）</param>
        public Task<bool> RegisterLootTableAsync(string lootTableId, IDictionary<string, int> entries)
        {
            if (string.IsNullOrEmpty(lootTableId))
            {
                _logger.LogWarning("尝试注册无效的掉落表ID");
                return Task.FromResult(false);
            }

            if (entries == null || entries.Count == 0)
            {
                _logger.LogWarning($"尝试为掉落表 {lootTableId} 注册空条目");
                return Task.FromResult(false);
            }

            var entriesDict = new Dictionary<string, int>(entries);
            _dropManager.RegisterLootTable(lootTableId, entriesDict);

            // 发布掉落表注册事件
            _eventManager.Publish(new LootTableRegisteredEvent(lootTableId, entries.Count));
            _logger.LogInformation($"注册掉落表 {lootTableId}，包含 {entries.Count} 个物品条目");

            return Task.FromResult(true);
        }

        /// <summary>
        /// 获取掉落表
        /// </summary>
        /// <param name="lootTableId">掉落表ID</param>
        /// <returns>掉落表条目</returns>
        public Task<IDictionary<string, int>> GetLootTableAsync(string lootTableId)
        {
            var lootTable = _dropManager.GetLootTable(lootTableId);
            return Task.FromResult<IDictionary<string, int>>(lootTable ?? new Dictionary<string, int>());
        }

        /// <summary>
        /// 加载掉落数据
        /// </summary>
        /// <param name="data">掉落数据</param>
        /// <returns>加载结果</returns>
        public async Task<bool> LoadDropDataAsync(string data)
        {
            return await _dropManager.LoadDropDataAsync(data);
        }

        /// <summary>
        /// 保存掉落数据
        /// </summary>
        /// <returns>保存的数据</returns>
        public async Task<string> SaveDropDataAsync()
        {
            return await _dropManager.SaveDropDataAsync();
        }
    }
}