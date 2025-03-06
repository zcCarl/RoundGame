using System;

namespace TacticalRPG.Core.Modules.Projectile
{
    /// <summary>
    /// 定义投射物的移动方式
    /// </summary>
    public enum ProjectileMovementType
    {
        /// <summary>
        /// 无移动
        /// </summary>
        None = 0,

        /// <summary>
        /// 直线移动
        /// </summary>
        Linear = 1,

        /// <summary>
        /// 抛物线轨迹
        /// </summary>
        Parabolic = 2,

        /// <summary>
        /// 锁定目标跟踪
        /// </summary>
        Homing = 3,

        /// <summary>
        /// 随机路径
        /// </summary>
        Random = 4,

        /// <summary>
        /// 瞬间移动到目标位置
        /// </summary>
        Teleport = 5,

        /// <summary>
        /// 曲线路径
        /// </summary>
        Curved = 6,

        /// <summary>
        /// 螺旋路径
        /// </summary>
        Spiral = 7,

        /// <summary>
        /// 弹跳路径
        /// </summary>
        Bounce = 8,

        /// <summary>
        /// 自定义路径（由具体实现决定）
        /// </summary>
        Custom = 9
    }
}