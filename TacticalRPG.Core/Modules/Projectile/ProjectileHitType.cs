using System;

namespace TacticalRPG.Core.Modules.Projectile
{
    /// <summary>
    /// 定义投射物命中后的效果类型
    /// </summary>
    [Flags]
    public enum ProjectileHitType
    {
        /// <summary>
        /// 无效果
        /// </summary>
        None = 0,

        /// <summary>
        /// 直接伤害
        /// </summary>
        DirectDamage = 1 << 0,

        /// <summary>
        /// 范围伤害（爆炸效果）
        /// </summary>
        AreaDamage = 1 << 1,

        /// <summary>
        /// 持续伤害（如毒、燃烧）
        /// </summary>
        DamageOverTime = 1 << 2,

        /// <summary>
        /// 击退效果
        /// </summary>
        Knockback = 1 << 3,

        /// <summary>
        /// 眩晕效果
        /// </summary>
        Stun = 1 << 4,

        /// <summary>
        /// 减速效果
        /// </summary>
        Slow = 1 << 5,

        /// <summary>
        /// 治疗效果
        /// </summary>
        Heal = 1 << 6,

        /// <summary>
        /// 增益效果
        /// </summary>
        Buff = 1 << 7,

        /// <summary>
        /// 减益效果
        /// </summary>
        Debuff = 1 << 8,

        /// <summary>
        /// 控制效果（如恐惧、沉默）
        /// </summary>
        ControlEffect = 1 << 9,

        /// <summary>
        /// 击中后生成新的投射物
        /// </summary>
        SpawnProjectile = 1 << 10,

        /// <summary>
        /// 传送效果
        /// </summary>
        Teleport = 1 << 11
    }
}