using System;

namespace TacticalRPG.Core.Modules.Projectile
{
    /// <summary>
    /// 定义游戏中不同类型的投射物
    /// </summary>
    [Flags]
    public enum ProjectileType
    {
        /// <summary>
        /// 未定义的投射物类型
        /// </summary>
        None = 0,

        /// <summary>
        /// 直线移动的投射物
        /// </summary>
        Linear = 1 << 0,

        /// <summary>
        /// 抛物线移动的投射物
        /// </summary>
        Parabolic = 1 << 1,

        /// <summary>
        /// 跟踪目标的投射物
        /// </summary>
        Homing = 1 << 2,

        /// <summary>
        /// 区域效果的投射物
        /// </summary>
        AreaEffect = 1 << 3,

        /// <summary>
        /// 瞬间到达目标的投射物
        /// </summary>
        Instant = 1 << 4,

        /// <summary>
        /// 可穿透的投射物
        /// </summary>
        Piercing = 1 << 5,

        /// <summary>
        /// 反弹的投射物
        /// </summary>
        Bouncing = 1 << 6,

        /// <summary>
        /// 持续型投射物（如火焰）
        /// </summary>
        Continuous = 1 << 7,

        /// <summary>
        /// 链式投射物（从一个目标跳到另一个目标）
        /// </summary>
        Chain = 1 << 8
    }
}