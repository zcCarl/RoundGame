namespace TacticalRPG.Core.Modules.Skill
{
    /// <summary>
    /// 技能效果类型枚举
    /// </summary>
    public enum SkillEffectType
    {
        /// <summary>
        /// 无效果
        /// </summary>
        None,

        /// <summary>
        /// 直接伤害
        /// </summary>
        DirectDamage,

        /// <summary>
        /// 持续伤害
        /// </summary>
        DamageOverTime,

        /// <summary>
        /// 直接治疗
        /// </summary>
        DirectHeal,

        /// <summary>
        /// 持续治疗
        /// </summary>
        HealOverTime,

        /// <summary>
        /// 属性增强
        /// </summary>
        StatBoost,

        /// <summary>
        /// 属性减弱
        /// </summary>
        StatReduction,

        /// <summary>
        /// 状态改变（如眩晕、睡眠等）
        /// </summary>
        StatusChange,

        /// <summary>
        /// 召唤实体
        /// </summary>
        Summon,

        /// <summary>
        /// 位移效果
        /// </summary>
        Movement,

        /// <summary>
        /// 地形改变
        /// </summary>
        TerrainChange,

        /// <summary>
        /// 反射伤害
        /// </summary>
        DamageReflection,

        /// <summary>
        /// 伤害吸收
        /// </summary>
        DamageAbsorption,

        /// <summary>
        /// 驱散效果
        /// </summary>
        Dispel,

        /// <summary>
        /// 特殊效果
        /// </summary>
        Special
    }
}