using System.Collections.Generic;
using TacticalRPG.Core.Modules.Character;

namespace TacticalRPG.Core.Modules.Map
{
    /// <summary>
    /// 地图单元格接口
    /// </summary>
    public interface IMapCell
    {
        /// <summary>
        /// X坐标
        /// </summary>
        int X { get; }

        /// <summary>
        /// Y坐标
        /// </summary>
        int Y { get; }

        /// <summary>
        /// 地形
        /// </summary>
        ITerrain Terrain { get; }

        /// <summary>
        /// 当前位于此单元格的实体
        /// </summary>
        ICharacter OccupyingEntity { get; }

        /// <summary>
        /// 此单元格上的视觉效果列表
        /// </summary>
        IReadOnlyList<string> VisualEffects { get; }

        /// <summary>
        /// 单元格是否可通行
        /// </summary>
        bool IsPassable { get; }

        /// <summary>
        /// 单元格是否已被占据
        /// </summary>
        bool IsOccupied { get; }

        /// <summary>
        /// 单元格是否可见（未被战争迷雾覆盖）
        /// </summary>
        bool IsVisible { get; }

        /// <summary>
        /// 单元格是否已被探索过
        /// </summary>
        bool IsExplored { get; }

        /// <summary>
        /// 设置地形
        /// </summary>
        /// <param name="terrain">地形</param>
        void SetTerrain(ITerrain terrain);

        /// <summary>
        /// 设置占据此单元格的实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>是否成功设置</returns>
        bool SetOccupyingEntity(ICharacter entity);

        /// <summary>
        /// 清除占据此单元格的实体
        /// </summary>
        void ClearOccupyingEntity();

        /// <summary>
        /// 添加视觉效果
        /// </summary>
        /// <param name="effectId">效果ID</param>
        void AddVisualEffect(string effectId);

        /// <summary>
        /// 移除视觉效果
        /// </summary>
        /// <param name="effectId">效果ID</param>
        void RemoveVisualEffect(string effectId);

        /// <summary>
        /// 清除所有视觉效果
        /// </summary>
        void ClearVisualEffects();

        /// <summary>
        /// 设置单元格可见性
        /// </summary>
        /// <param name="isVisible">是否可见</param>
        void SetVisibility(bool isVisible);

        /// <summary>
        /// 标记单元格为已探索
        /// </summary>
        void MarkAsExplored();

        /// <summary>
        /// 获取移动到该单元格的消耗
        /// </summary>
        /// <returns>移动消耗</returns>
        int GetMovementCost();

        /// <summary>
        /// 检查单元格是否可被指定实体进入
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>是否可进入</returns>
        bool CanBeEnteredBy(ICharacter entity);
    }
}