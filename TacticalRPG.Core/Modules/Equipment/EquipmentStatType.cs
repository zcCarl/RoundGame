using System;

namespace TacticalRPG.Core.Modules.Equipment
{
    /// <summary>
    /// 定义装备可能具有的属性类型
    /// </summary>
    [Flags]
    public enum EquipmentStatType
    {
        /// <summary>
        /// 无属性
        /// </summary>
        None = 0,

        /// <summary>
        /// 物理攻击力
        /// </summary>
        PhysicalAttack = 1 << 0,

        /// <summary>
        /// 魔法攻击力
        /// </summary>
        MagicalAttack = 1 << 1,

        /// <summary>
        /// 物理防御力
        /// </summary>
        PhysicalDefense = 1 << 2,

        /// <summary>
        /// 魔法防御力
        /// </summary>
        MagicalDefense = 1 << 3,

        /// <summary>
        /// 最大生命值
        /// </summary>
        MaxHealth = 1 << 4,

        /// <summary>
        /// 最大魔法值/能量值
        /// </summary>
        MaxMana = 1 << 5,

        /// <summary>
        /// 命中率
        /// </summary>
        Accuracy = 1 << 6,

        /// <summary>
        /// 闪避率
        /// </summary>
        Evasion = 1 << 7,

        /// <summary>
        /// 暴击率
        /// </summary>
        CriticalRate = 1 << 8,

        /// <summary>
        /// 暴击伤害倍率
        /// </summary>
        CriticalDamage = 1 << 9,

        /// <summary>
        /// 攻击速度
        /// </summary>
        AttackSpeed = 1 << 10,

        /// <summary>
        /// 移动速度
        /// </summary>
        MovementSpeed = 1 << 11,

        /// <summary>
        /// 生命回复率
        /// </summary>
        HealthRegeneration = 1 << 12,

        /// <summary>
        /// 魔法回复率
        /// </summary>
        ManaRegeneration = 1 << 13,

        /// <summary>
        /// 物理穿透
        /// </summary>
        PhysicalPenetration = 1 << 14,

        /// <summary>
        /// 魔法穿透
        /// </summary>
        MagicalPenetration = 1 << 15,

        /// <summary>
        /// 减伤率
        /// </summary>
        DamageReduction = 1 << 16,

        /// <summary>
        /// 反伤率
        /// </summary>
        DamageReflection = 1 << 17,

        /// <summary>
        /// 吸血率
        /// </summary>
        LifeSteal = 1 << 18,

        /// <summary>
        /// 冷却时间减少
        /// </summary>
        CooldownReduction = 1 << 19,

        /// <summary>
        /// 经验获取加成
        /// </summary>
        ExperienceBonus = 1 << 20,

        /// <summary>
        /// 金币获取加成
        /// </summary>
        GoldBonus = 1 << 21,

        /// <summary>
        /// 元素伤害（火）
        /// </summary>
        FireDamage = 1 << 22,

        /// <summary>
        /// 元素伤害（冰）
        /// </summary>
        IceDamage = 1 << 23,

        /// <summary>
        /// 元素伤害（电）
        /// </summary>
        LightningDamage = 1 << 24,

        /// <summary>
        /// 元素伤害（毒）
        /// </summary>
        PoisonDamage = 1 << 25,

        /// <summary>
        /// 元素抗性（火）
        /// </summary>
        FireResistance = 1 << 26,

        /// <summary>
        /// 元素抗性（冰）
        /// </summary>
        IceResistance = 1 << 27,

        /// <summary>
        /// 元素抗性（电）
        /// </summary>
        LightningResistance = 1 << 28,

        /// <summary>
        /// 元素抗性（毒）
        /// </summary>
        PoisonResistance = 1 << 29
    }
}