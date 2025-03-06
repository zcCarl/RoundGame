using System;
using TacticalRPG.Core.Modules.Character;

namespace TacticalRPG.Core.Modules.Map
{
    /// <summary>
    /// 地图加载事件参数
    /// </summary>
    public class MapLoadEventArgs : EventArgs
    {
        /// <summary>
        /// 地图
        /// </summary>
        public IMap Map { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="map">地图</param>
        public MapLoadEventArgs(IMap map)
        {
            Map = map;
        }
    }

    /// <summary>
    /// 地图卸载事件参数
    /// </summary>
    public class MapUnloadEventArgs : EventArgs
    {
        /// <summary>
        /// 地图
        /// </summary>
        public IMap Map { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="map">地图</param>
        public MapUnloadEventArgs(IMap map)
        {
            Map = map;
        }
    }

    /// <summary>
    /// 地形变化事件参数
    /// </summary>
    public class TerrainChangeEventArgs : EventArgs
    {
        /// <summary>
        /// X坐标
        /// </summary>
        public int X { get; }

        /// <summary>
        /// Y坐标
        /// </summary>
        public int Y { get; }

        /// <summary>
        /// 旧地形
        /// </summary>
        public ITerrain OldTerrain { get; }

        /// <summary>
        /// 新地形
        /// </summary>
        public ITerrain NewTerrain { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="x">X坐标</param>
        /// <param name="y">Y坐标</param>
        /// <param name="oldTerrain">旧地形</param>
        /// <param name="newTerrain">新地形</param>
        public TerrainChangeEventArgs(int x, int y, ITerrain oldTerrain, ITerrain newTerrain)
        {
            X = x;
            Y = y;
            OldTerrain = oldTerrain;
            NewTerrain = newTerrain;
        }
    }

    /// <summary>
    /// 实体移动事件参数
    /// </summary>
    public class EntityMoveEventArgs : EventArgs
    {
        /// <summary>
        /// 实体
        /// </summary>
        public ICharacter Entity { get; }

        /// <summary>
        /// 起始X坐标
        /// </summary>
        public int FromX { get; }

        /// <summary>
        /// 起始Y坐标
        /// </summary>
        public int FromY { get; }

        /// <summary>
        /// 目标X坐标
        /// </summary>
        public int ToX { get; }

        /// <summary>
        /// 目标Y坐标
        /// </summary>
        public int ToY { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="fromX">起始X坐标</param>
        /// <param name="fromY">起始Y坐标</param>
        /// <param name="toX">目标X坐标</param>
        /// <param name="toY">目标Y坐标</param>
        public EntityMoveEventArgs(ICharacter entity, int fromX, int fromY, int toX, int toY)
        {
            Entity = entity;
            FromX = fromX;
            FromY = fromY;
            ToX = toX;
            ToY = toY;
        }
    }
}