namespace TacticalRPG.Core.Modules.Buff
{
    /// <summary>
    /// Buff类型枚举
    /// </summary>
    public enum BuffType
    {
        /// <summary>
        /// 增益效果
        /// </summary>
        Positive,

        /// <summary>
        /// 减益效果
        /// </summary>
        Negative,

        /// <summary>
        /// 中性效果
        /// </summary>
        Neutral,

        /// <summary>
        /// 控制效果
        /// </summary>
        Control,

        /// <summary>
        /// 持续伤害效果
        /// </summary>
        DamageOverTime,

        /// <summary>
        /// 持续治疗效果
        /// </summary>
        HealOverTime,

        /// <summary>
        /// 免疫效果
        /// </summary>
        Immunity,

        /// <summary>
        /// 反射效果
        /// </summary>
        Reflect,

        /// <summary>
        /// 吸收效果
        /// </summary>
        Absorb,

        /// <summary>
        /// 特殊效果
        /// </summary>
        Special
    }
}