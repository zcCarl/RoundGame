using System;
using System.Collections.Generic;

namespace TacticalRPG.Core.Modules.Inventory
{
    /// <summary>
    /// 掉落物管理器接口
    /// </summary>
    public interface IDropManager
    {
        /// <summary>
        /// 当前游戏世界中的所有掉落物
        /// </summary>
        IReadOnlyDictionary<Guid, IDrop> ActiveDrops { get; }

        /// <summary>
        /// 创建掉落物
        /// </summary>
        /// <param name="x">X坐标</param>
        /// <param name="y">Y坐标</param>
        /// <param name="items">物品列表</param>
        /// <param name="existDuration">存在时间（秒），0表示永久存在</param>
        /// <returns>创建的掉落物</returns>
        IDrop CreateDrop(int x, int y, IReadOnlyList<IItem> items, int existDuration = 60);

        /// <summary>
        /// 创建掉落物
        /// </summary>
        /// <param name="x">X坐标</param>
        /// <param name="y">Y坐标</param>
        /// <param name="item">物品</param>
        /// <param name="existDuration">存在时间（秒），0表示永久存在</param>
        /// <returns>创建的掉落物</returns>
        IDrop CreateDrop(int x, int y, IItem item, int existDuration = 60);

        /// <summary>
        /// 创建随机掉落物
        /// </summary>
        /// <param name="x">X坐标</param>
        /// <param name="y">Y坐标</param>
        /// <param name="lootTable">掉落表ID</param>
        /// <param name="existDuration">存在时间（秒），0表示永久存在</param>
        /// <returns>创建的掉落物</returns>
        IDrop CreateRandomDrop(int x, int y, string lootTable, int existDuration = 60);

        /// <summary>
        /// 移除掉落物
        /// </summary>
        /// <param name="dropId">掉落物ID</param>
        /// <returns>是否成功移除</returns>
        bool RemoveDrop(Guid dropId);

        /// <summary>
        /// 拾取掉落物
        /// </summary>
        /// <param name="dropId">掉落物ID</param>
        /// <param name="characterId">角色ID</param>
        /// <returns>拾取结果</returns>
        (bool success, string message, IReadOnlyList<IItem> items) PickupDrop(Guid dropId, Guid characterId);

        /// <summary>
        /// 获取掉落物
        /// </summary>
        /// <param name="dropId">掉落物ID</param>
        /// <returns>掉落物</returns>
        IDrop GetDrop(Guid dropId);

        /// <summary>
        /// 获取指定位置的掉落物
        /// </summary>
        /// <param name="x">X坐标</param>
        /// <param name="y">Y坐标</param>
        /// <returns>掉落物列表</returns>
        IReadOnlyList<IDrop> GetDropsAtPosition(int x, int y);

        /// <summary>
        /// 获取指定区域的掉落物
        /// </summary>
        /// <param name="x">中心X坐标</param>
        /// <param name="y">中心Y坐标</param>
        /// <param name="radius">半径</param>
        /// <returns>掉落物列表</returns>
        IReadOnlyList<IDrop> GetDropsInRadius(int x, int y, int radius);

        /// <summary>
        /// 清理已拾取的掉落物
        /// </summary>
        /// <returns>清理的掉落物数量</returns>
        int CleanupPickedDrops();

        /// <summary>
        /// 清理过期的掉落物
        /// </summary>
        /// <returns>清理的掉落物数量</returns>
        int CleanupExpiredDrops();

        /// <summary>
        /// 注册掉落表
        /// </summary>
        /// <param name="lootTableId">掉落表ID</param>
        /// <param name="items">物品列表及其权重</param>
        void RegisterLootTable(string lootTableId, Dictionary<string, int> items);

        /// <summary>
        /// 获取掉落表
        /// </summary>
        /// <param name="lootTableId">掉落表ID</param>
        /// <returns>掉落表</returns>
        Dictionary<string, int> GetLootTable(string lootTableId);
    }
}