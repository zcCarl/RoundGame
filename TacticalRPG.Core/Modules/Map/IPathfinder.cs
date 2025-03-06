using System.Collections.Generic;
using TacticalRPG.Core.Modules.Character;

namespace TacticalRPG.Core.Modules.Map
{
    /// <summary>
    /// 路径查找器接口
    /// </summary>
    public interface IPathfinder
    {
        /// <summary>
        /// 查找两点之间的最短路径
        /// </summary>
        /// <param name="map">地图</param>
        /// <param name="startX">起始X坐标</param>
        /// <param name="startY">起始Y坐标</param>
        /// <param name="endX">目标X坐标</param>
        /// <param name="endY">目标Y坐标</param>
        /// <param name="ignoreEntityOccupation">是否忽略实体占据</param>
        /// <returns>路径坐标列表，如果无法到达则返回空列表</returns>
        IReadOnlyList<(int X, int Y)> FindPath(IMap map, int startX, int startY, int endX, int endY, bool ignoreEntityOccupation = false);

        /// <summary>
        /// 查找角色可以到达的所有位置
        /// </summary>
        /// <param name="map">地图</param>
        /// <param name="character">角色</param>
        /// <param name="startX">起始X坐标</param>
        /// <param name="startY">起始Y坐标</param>
        /// <param name="movementPoints">移动点数</param>
        /// <returns>可到达位置的坐标和消耗的字典</returns>
        IDictionary<(int X, int Y), int> FindReachablePositions(IMap map, ICharacter character, int startX, int startY, int movementPoints);

        /// <summary>
        /// 计算两点间的曼哈顿距离
        /// </summary>
        /// <param name="startX">起始X坐标</param>
        /// <param name="startY">起始Y坐标</param>
        /// <param name="endX">目标X坐标</param>
        /// <param name="endY">目标Y坐标</param>
        /// <returns>曼哈顿距离</returns>
        int CalculateManhattanDistance(int startX, int startY, int endX, int endY);

        /// <summary>
        /// 计算两点间的欧几里得距离
        /// </summary>
        /// <param name="startX">起始X坐标</param>
        /// <param name="startY">起始Y坐标</param>
        /// <param name="endX">目标X坐标</param>
        /// <param name="endY">目标Y坐标</param>
        /// <returns>欧几里得距离</returns>
        double CalculateEuclideanDistance(int startX, int startY, int endX, int endY);

        /// <summary>
        /// 判断两点之间是否有直线路径（没有障碍物）
        /// </summary>
        /// <param name="map">地图</param>
        /// <param name="startX">起始X坐标</param>
        /// <param name="startY">起始Y坐标</param>
        /// <param name="endX">目标X坐标</param>
        /// <param name="endY">目标Y坐标</param>
        /// <returns>是否有直线路径</returns>
        bool HasLineOfSight(IMap map, int startX, int startY, int endX, int endY);

        /// <summary>
        /// 获取直线路径上的所有坐标
        /// </summary>
        /// <param name="startX">起始X坐标</param>
        /// <param name="startY">起始Y坐标</param>
        /// <param name="endX">目标X坐标</param>
        /// <param name="endY">目标Y坐标</param>
        /// <returns>直线路径上的所有坐标</returns>
        IEnumerable<(int X, int Y)> GetLinePoints(int startX, int startY, int endX, int endY);
    }
}