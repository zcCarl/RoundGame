using System;

namespace TacticalRPG.Core.Modules.Map
{
    /// <summary>
    /// 地图方向枚举
    /// </summary>
    public enum MapDirection
    {
        /// <summary>
        /// 无方向
        /// </summary>
        None = 0,

        /// <summary>
        /// 上方
        /// </summary>
        Up = 1,

        /// <summary>
        /// 右方
        /// </summary>
        Right = 2,

        /// <summary>
        /// 下方
        /// </summary>
        Down = 3,

        /// <summary>
        /// 左方
        /// </summary>
        Left = 4,

        /// <summary>
        /// 右上
        /// </summary>
        UpRight = 5,

        /// <summary>
        /// 右下
        /// </summary>
        DownRight = 6,

        /// <summary>
        /// 左下
        /// </summary>
        DownLeft = 7,

        /// <summary>
        /// 左上
        /// </summary>
        UpLeft = 8
    }

    /// <summary>
    /// 方向辅助类
    /// </summary>
    public static class DirectionHelper
    {
        /// <summary>
        /// 获取相反方向
        /// </summary>
        /// <param name="direction">方向</param>
        /// <returns>相反方向</returns>
        public static MapDirection GetOppositeDirection(this MapDirection direction)
        {
            return direction switch
            {
                MapDirection.Up => MapDirection.Down,
                MapDirection.Right => MapDirection.Left,
                MapDirection.Down => MapDirection.Up,
                MapDirection.Left => MapDirection.Right,
                MapDirection.UpRight => MapDirection.DownLeft,
                MapDirection.DownRight => MapDirection.UpLeft,
                MapDirection.DownLeft => MapDirection.UpRight,
                MapDirection.UpLeft => MapDirection.DownRight,
                _ => MapDirection.None
            };
        }

        /// <summary>
        /// 将方向转换为坐标偏移
        /// </summary>
        /// <param name="direction">方向</param>
        /// <returns>坐标偏移 (dx, dy)</returns>
        public static (int dx, int dy) ToOffset(this MapDirection direction)
        {
            return direction switch
            {
                MapDirection.Up => (0, -1),
                MapDirection.Right => (1, 0),
                MapDirection.Down => (0, 1),
                MapDirection.Left => (-1, 0),
                MapDirection.UpRight => (1, -1),
                MapDirection.DownRight => (1, 1),
                MapDirection.DownLeft => (-1, 1),
                MapDirection.UpLeft => (-1, -1),
                _ => (0, 0)
            };
        }

        /// <summary>
        /// 从一个位置到另一个位置的方向
        /// </summary>
        /// <param name="fromX">起始X坐标</param>
        /// <param name="fromY">起始Y坐标</param>
        /// <param name="toX">目标X坐标</param>
        /// <param name="toY">目标Y坐标</param>
        /// <returns>方向</returns>
        public static MapDirection GetDirection(int fromX, int fromY, int toX, int toY)
        {
            int dx = Math.Sign(toX - fromX);
            int dy = Math.Sign(toY - fromY);

            return (dx, dy) switch
            {
                (0, -1) => MapDirection.Up,
                (1, 0) => MapDirection.Right,
                (0, 1) => MapDirection.Down,
                (-1, 0) => MapDirection.Left,
                (1, -1) => MapDirection.UpRight,
                (1, 1) => MapDirection.DownRight,
                (-1, 1) => MapDirection.DownLeft,
                (-1, -1) => MapDirection.UpLeft,
                _ => MapDirection.None
            };
        }
    }
}
