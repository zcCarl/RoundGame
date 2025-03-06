namespace TacticalRPG.Core.Modules.Buff
{
    /// <summary>
    /// 状态效果类型枚举
    /// </summary>
    public enum StatusEffectType
    {
        /// <summary>
        /// 无效果
        /// </summary>
        None = 0,

        /// <summary>
        /// 眩晕：无法行动
        /// </summary>
        Stun = 1,

        /// <summary>
        /// 沉默：无法使用技能
        /// </summary>
        Silence = 2,

        /// <summary>
        /// 禁锢：无法移动
        /// </summary>
        Root = 3,

        /// <summary>
        /// 睡眠：无法行动，受伤后解除
        /// </summary>
        Sleep = 4,

        /// <summary>
        /// 混乱：随机行动
        /// </summary>
        Confusion = 5,

        /// <summary>
        /// 魅惑：为敌方行动
        /// </summary>
        Charm = 6,

        /// <summary>
        /// 恐惧：无法接近施法者
        /// </summary>
        Fear = 7,

        /// <summary>
        /// 减速：移动力降低
        /// </summary>
        Slow = 8,

        /// <summary>
        /// 加速：移动力提高
        /// </summary>
        Haste = 9,

        /// <summary>
        /// 中毒：每回合受到伤害
        /// </summary>
        Poison = 10,

        /// <summary>
        /// 燃烧：每回合受到伤害
        /// </summary>
        Burn = 11,

        /// <summary>
        /// 冻结：无法行动，受物理攻击有概率解除
        /// </summary>
        Freeze = 12,

        /// <summary>
        /// 麻痹：有概率无法行动
        /// </summary>
        Paralyze = 13,

        /// <summary>
        /// 虚弱：攻击力降低
        /// </summary>
        Weaken = 14,

        /// <summary>
        /// 破甲：防御力降低
        /// </summary>
        ArmorBreak = 15,

        /// <summary>
        /// 致盲：命中率降低
        /// </summary>
        Blind = 16,

        /// <summary>
        /// 隐身：敌方无法选择为目标
        /// </summary>
        Invisible = 17,

        /// <summary>
        /// 嘲讽：强制攻击施法者
        /// </summary>
        Taunt = 18,

        /// <summary>
        /// 狂暴：攻击力提高，防御力降低
        /// </summary>
        Berserk = 19,

        /// <summary>
        /// 护盾：吸收伤害
        /// </summary>
        Shield = 20,

        /// <summary>
        /// 无敌：免疫所有伤害
        /// </summary>
        Invulnerable = 21,

        /// <summary>
        /// 净化：免疫所有负面效果
        /// </summary>
        Purify = 22,

        /// <summary>
        /// 反射：反射技能效果
        /// </summary>
        Reflect = 23,

        /// <summary>
        /// 吸血：攻击回复生命
        /// </summary>
        LifeSteal = 24,

        /// <summary>
        /// 再生：每回合回复生命
        /// </summary>
        Regeneration = 25
    }
}