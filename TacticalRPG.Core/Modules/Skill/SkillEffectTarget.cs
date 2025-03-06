namespace TacticalRPG.Core.Modules.Skill
{
    /// <summary>
    /// 技能效果目标枚举
    /// </summary>
    public enum SkillEffectTarget
    {
        /// <summary>
        /// 生命值
        /// </summary>
        HP,

        /// <summary>
        /// 魔法值
        /// </summary>
        MP,

        /// <summary>
        /// 物理攻击
        /// </summary>
        PhysicalAttack,

        /// <summary>
        /// 魔法攻击
        /// </summary>
        MagicalAttack,

        /// <summary>
        /// 物理防御
        /// </summary>
        PhysicalDefense,

        /// <summary>
        /// 魔法防御
        /// </summary>
        MagicalDefense,

        /// <summary>
        /// 速度
        /// </summary>
        Speed,

        /// <summary>
        /// 命中率
        /// </summary>
        Accuracy,

        /// <summary>
        /// 闪避率
        /// </summary>
        Evasion,

        /// <summary>
        /// 暴击率
        /// </summary>
        CriticalRate,

        /// <summary>
        /// 暴击伤害
        /// </summary>
        CriticalDamage,

        /// <summary>
        /// 移动力
        /// </summary>
        Movement,

        /// <summary>
        /// 行动点数
        /// </summary>
        ActionPoints,

        /// <summary>
        /// 状态（如眩晕、睡眠等）
        /// </summary>
        Status,

        /// <summary>
        /// 地形
        /// </summary>
        Terrain,

        /// <summary>
        /// 多个目标（组合效果）
        /// </summary>
        Multiple
    }
}
