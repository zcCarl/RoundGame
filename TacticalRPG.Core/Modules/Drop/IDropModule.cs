using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TacticalRPG.Core.Framework;

namespace TacticalRPG.Core.Modules.Drop
{
    /// <summary>
    /// 掉落模块接口，负责管理游戏中的所有掉落物
    /// </summary>
    public interface IDropModule : IGameModule
    {
        /// <summary>
        /// 创建掉落物
        /// </summary>
        /// <param name="position">位置坐标</param>
        /// <param name="itemIds">物品ID列表</param>
        /// <returns>掉落物ID</returns>
        Task<Guid> CreateDropAsync((int x, int y) position, IReadOnlyList<Guid> itemIds);

        /// <summary>
        /// 通过掉落表创建随机掉落物
        /// </summary>
        /// <param name="position">位置坐标</param>
        /// <param name="lootTableId">掉落表ID</param>
        /// <param name="existDuration">存在时间（秒），0表示永久存在</param>
        /// <returns>掉落物ID</returns>
        Task<Guid> CreateRandomDropAsync((int x, int y) position, string lootTableId, int existDuration = 60);

        /// <summary>
        /// 拾取掉落物
        /// </summary>
        /// <param name="dropId">掉落物ID</param>
        /// <param name="characterId">拾取角色ID</param>
        /// <returns>拾取结果</returns>
        Task<(bool success, string message, IReadOnlyList<Guid> itemIds)> PickupDropAsync(Guid dropId, Guid characterId);

        /// <summary>
        /// 获取掉落物信息
        /// </summary>
        /// <param name="dropId">掉落物ID</param>
        /// <returns>掉落物信息</returns>
        Task<IDrop> GetDropAsync(Guid dropId);

        /// <summary>
        /// 获取指定位置的掉落物
        /// </summary>
        /// <param name="position">位置坐标</param>
        /// <returns>掉落物ID列表</returns>
        Task<IReadOnlyList<Guid>> GetDropsAtPositionAsync((int x, int y) position);

        /// <summary>
        /// 获取指定区域内的掉落物
        /// </summary>
        /// <param name="position">中心位置坐标</param>
        /// <param name="radius">区域半径</param>
        /// <returns>掉落物ID列表</returns>
        Task<IReadOnlyList<Guid>> GetDropsInRadiusAsync((int x, int y) position, int radius);

        /// <summary>
        /// 移除掉落物
        /// </summary>
        /// <param name="dropId">掉落物ID</param>
        /// <returns>是否成功移除</returns>
        Task<bool> RemoveDropAsync(Guid dropId);

        /// <summary>
        /// 清理已拾取或过期的掉落物
        /// </summary>
        /// <returns>清理的掉落物数量</returns>
        Task<int> CleanupDropsAsync();

        /// <summary>
        /// 注册掉落表
        /// </summary>
        /// <param name="lootTableId">掉落表ID</param>
        /// <param name="entries">掉落表条目（物品模板ID及其权重）</param>
        Task<bool> RegisterLootTableAsync(string lootTableId, IDictionary<string, int> entries);

        /// <summary>
        /// 获取掉落表
        /// </summary>
        /// <param name="lootTableId">掉落表ID</param>
        /// <returns>掉落表条目</returns>
        Task<IDictionary<string, int>> GetLootTableAsync(string lootTableId);

        /// <summary>
        /// 加载掉落数据
        /// </summary>
        /// <param name="data">掉落数据</param>
        /// <returns>加载结果</returns>
        Task<bool> LoadDropDataAsync(string data);

        /// <summary>
        /// 保存掉落数据
        /// </summary>
        /// <returns>保存的数据</returns>
        Task<string> SaveDropDataAsync();
    }
}