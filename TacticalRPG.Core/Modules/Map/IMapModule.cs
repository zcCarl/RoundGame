using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TacticalRPG.Core.Framework;

namespace TacticalRPG.Core.Modules.Map
{
    /// <summary>
    /// 地图模块接口
    /// </summary>
    public interface IMapModule
    {
        /// <summary>
        /// 创建新地图
        /// </summary>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <param name="name">地图名称</param>
        /// <returns>新地图的ID</returns>
        Task<Guid> CreateMapAsync(int width, int height, string name);

        /// <summary>
        /// 加载地图
        /// </summary>
        /// <param name="mapId">地图ID</param>
        /// <returns>操作结果</returns>
        Task<bool> LoadMapAsync(Guid mapId);

        /// <summary>
        /// 卸载当前地图
        /// </summary>
        /// <returns>操作结果</returns>
        Task<bool> UnloadCurrentMapAsync();

        /// <summary>
        /// 获取当前地图
        /// </summary>
        /// <returns>当前地图</returns>
        IMap GetCurrentMap();

        /// <summary>
        /// 获取所有可用地图
        /// </summary>
        /// <returns>所有地图</returns>
        IReadOnlyList<IMapInfo> GetAllMaps();

        /// <summary>
        /// 保存当前地图
        /// </summary>
        /// <returns>操作结果</returns>
        Task<bool> SaveCurrentMapAsync();

        /// <summary>
        /// 根据坐标获取地形
        /// </summary>
        /// <param name="x">X坐标</param>
        /// <param name="y">Y坐标</param>
        /// <returns>地形对象</returns>
        ITerrain GetTerrain(int x, int y);

        /// <summary>
        /// 设置地形
        /// </summary>
        /// <param name="x">X坐标</param>
        /// <param name="y">Y坐标</param>
        /// <param name="terrainType">地形类型</param>
        /// <returns>操作结果</returns>
        bool SetTerrain(int x, int y, TerrainType terrainType);

        /// <summary>
        /// 检查坐标是否在地图范围内
        /// </summary>
        /// <param name="x">X坐标</param>
        /// <param name="y">Y坐标</param>
        /// <returns>是否在地图内</returns>
        bool IsPositionInBounds(int x, int y);

        /// <summary>
        /// 获取指定范围内的所有坐标
        /// </summary>
        /// <param name="x">中心X坐标</param>
        /// <param name="y">中心Y坐标</param>
        /// <param name="range">范围</param>
        /// <returns>范围内的所有有效坐标</returns>
        IEnumerable<(int X, int Y)> GetPositionsInRange(int x, int y, int range);

        /// <summary>
        /// 计算两点间的路径
        /// </summary>
        /// <param name="startX">起点X坐标</param>
        /// <param name="startY">起点Y坐标</param>
        /// <param name="endX">终点X坐标</param>
        /// <param name="endY">终点Y坐标</param>
        /// <returns>路径坐标列表，如果无法到达则返回空列表</returns>
        IReadOnlyList<(int X, int Y)> FindPath(int startX, int startY, int endX, int endY);
    }
}