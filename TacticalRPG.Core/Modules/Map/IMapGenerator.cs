using System;
using System.Threading.Tasks;

namespace TacticalRPG.Core.Modules.Map
{
    /// <summary>
    /// 地图生成器类型
    /// </summary>
    public enum MapGeneratorType
    {
        /// <summary>
        /// 空白地图
        /// </summary>
        Empty,

        /// <summary>
        /// 随机地图
        /// </summary>
        Random,

        /// <summary>
        /// 房间型地图
        /// </summary>
        Rooms,

        /// <summary>
        /// 迷宫型地图
        /// </summary>
        Maze,

        /// <summary>
        /// 洞穴型地图
        /// </summary>
        Caves,

        /// <summary>
        /// 岛屿型地图
        /// </summary>
        Islands,

        /// <summary>
        /// 城镇型地图
        /// </summary>
        Town
    }

    /// <summary>
    /// 地图生成参数
    /// </summary>
    public class MapGeneratorParameters
    {
        /// <summary>
        /// 地图宽度
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// 地图高度
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// 地图名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 随机种子
        /// </summary>
        public int? Seed { get; set; }

        /// <summary>
        /// 水域百分比 (0-100)
        /// </summary>
        public int WaterPercentage { get; set; } = 20;

        /// <summary>
        /// 森林百分比 (0-100)
        /// </summary>
        public int ForestPercentage { get; set; } = 30;

        /// <summary>
        /// 山地百分比 (0-100)
        /// </summary>
        public int MountainPercentage { get; set; } = 15;

        /// <summary>
        /// 房间数量（用于房间型地图）
        /// </summary>
        public int RoomCount { get; set; } = 10;

        /// <summary>
        /// 最小房间尺寸（用于房间型地图）
        /// </summary>
        public int MinRoomSize { get; set; } = 4;

        /// <summary>
        /// 最大房间尺寸（用于房间型地图）
        /// </summary>
        public int MaxRoomSize { get; set; } = 10;

        /// <summary>
        /// 迷宫复杂度 (1-10)
        /// </summary>
        public int MazeComplexity { get; set; } = 5;

        /// <summary>
        /// 洞穴平滑度 (1-10)
        /// </summary>
        public int CaveSmoothness { get; set; } = 5;
    }

    /// <summary>
    /// 地图生成器接口
    /// </summary>
    public interface IMapGenerator
    {
        /// <summary>
        /// 生成器类型
        /// </summary>
        MapGeneratorType GeneratorType { get; }

        /// <summary>
        /// 生成地图
        /// </summary>
        /// <param name="parameters">生成参数</param>
        /// <returns>生成的地图</returns>
        Task<IMap> GenerateMapAsync(MapGeneratorParameters parameters);

        /// <summary>
        /// 在现有地图上生成内容
        /// </summary>
        /// <param name="map">现有地图</param>
        /// <param name="parameters">生成参数</param>
        /// <returns>操作结果</returns>
        Task<bool> GenerateOnExistingMapAsync(IMap map, MapGeneratorParameters parameters);

        /// <summary>
        /// 生成特定区域
        /// </summary>
        /// <param name="map">地图</param>
        /// <param name="startX">起始X坐标</param>
        /// <param name="startY">起始Y坐标</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <param name="parameters">生成参数</param>
        /// <returns>操作结果</returns>
        Task<bool> GenerateRegionAsync(IMap map, int startX, int startY, int width, int height, MapGeneratorParameters parameters);
    }
}