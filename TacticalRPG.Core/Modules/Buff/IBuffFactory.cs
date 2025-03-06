using System;
using TacticalRPG.Core.Modules.Character;

namespace TacticalRPG.Core.Modules.Buff
{
    /// <summary>
    /// Buff工厂接口
    /// </summary>
    public interface IBuffFactory
    {
        /// <summary>
        /// 创建Buff实例
        /// </summary>
        /// <param name="buffType">Buff类型</param>
        /// <param name="statusEffectType">状态效果类型</param>
        /// <returns>Buff实例</returns>
        IBuff CreateBuff(BuffType buffType, StatusEffectType statusEffectType);

        /// <summary>
        /// 创建状态效果Buff
        /// </summary>
        /// <param name="name">Buff名称</param>
        /// <param name="description">Buff描述</param>
        /// <param name="statusEffectType">状态效果类型</param>
        /// <param name="duration">持续回合数</param>
        /// <param name="caster">施法者</param>
        /// <param name="target">目标</param>
        /// <returns>状态效果Buff</returns>
        IBuff CreateStatusBuff(string name, string description, StatusEffectType statusEffectType,
            int duration, ICombatCharacter caster, ICombatCharacter target);

        /// <summary>
        /// 创建属性增强Buff
        /// </summary>
        /// <param name="name">Buff名称</param>
        /// <param name="description">Buff描述</param>
        /// <param name="duration">持续回合数</param>
        /// <param name="power">增强幅度</param>
        /// <param name="caster">施法者</param>
        /// <param name="target">目标</param>
        /// <returns>属性增强Buff</returns>
        IBuff CreateStatBoostBuff(string name, string description, int duration,
            int power, ICombatCharacter caster, ICombatCharacter target);

        /// <summary>
        /// 创建属性减弱Buff
        /// </summary>
        /// <param name="name">Buff名称</param>
        /// <param name="description">Buff描述</param>
        /// <param name="duration">持续回合数</param>
        /// <param name="power">减弱幅度</param>
        /// <param name="caster">施法者</param>
        /// <param name="target">目标</param>
        /// <returns>属性减弱Buff</returns>
        IBuff CreateStatReductionBuff(string name, string description, int duration,
            int power, ICombatCharacter caster, ICombatCharacter target);

        /// <summary>
        /// 创建持续伤害Buff
        /// </summary>
        /// <param name="name">Buff名称</param>
        /// <param name="description">Buff描述</param>
        /// <param name="duration">持续回合数</param>
        /// <param name="damagePerTurn">每回合伤害</param>
        /// <param name="caster">施法者</param>
        /// <param name="target">目标</param>
        /// <returns>持续伤害Buff</returns>
        IBuff CreateDamageOverTimeBuff(string name, string description, int duration,
            int damagePerTurn, ICombatCharacter caster, ICombatCharacter target);

        /// <summary>
        /// 创建持续治疗Buff
        /// </summary>
        /// <param name="name">Buff名称</param>
        /// <param name="description">Buff描述</param>
        /// <param name="duration">持续回合数</param>
        /// <param name="healPerTurn">每回合治疗量</param>
        /// <param name="caster">施法者</param>
        /// <param name="target">目标</param>
        /// <returns>持续治疗Buff</returns>
        IBuff CreateHealOverTimeBuff(string name, string description, int duration,
            int healPerTurn, ICombatCharacter caster, ICombatCharacter target);

        /// <summary>
        /// 创建护盾Buff
        /// </summary>
        /// <param name="name">Buff名称</param>
        /// <param name="description">Buff描述</param>
        /// <param name="duration">持续回合数</param>
        /// <param name="shieldAmount">护盾值</param>
        /// <param name="caster">施法者</param>
        /// <param name="target">目标</param>
        /// <returns>护盾Buff</returns>
        IBuff CreateShieldBuff(string name, string description, int duration,
            int shieldAmount, ICombatCharacter caster, ICombatCharacter target);

        /// <summary>
        /// 创建控制Buff
        /// </summary>
        /// <param name="name">Buff名称</param>
        /// <param name="description">Buff描述</param>
        /// <param name="statusEffectType">状态效果类型</param>
        /// <param name="duration">持续回合数</param>
        /// <param name="caster">施法者</param>
        /// <param name="target">目标</param>
        /// <returns>控制Buff</returns>
        IBuff CreateControlBuff(string name, string description, StatusEffectType statusEffectType,
            int duration, ICombatCharacter caster, ICombatCharacter target);

        /// <summary>
        /// 创建反射Buff
        /// </summary>
        /// <param name="name">Buff名称</param>
        /// <param name="description">Buff描述</param>
        /// <param name="duration">持续回合数</param>
        /// <param name="reflectChance">反射概率 (0-100)</param>
        /// <param name="caster">施法者</param>
        /// <param name="target">目标</param>
        /// <returns>反射Buff</returns>
        IBuff CreateReflectBuff(string name, string description, int duration,
            int reflectChance, ICombatCharacter caster, ICombatCharacter target);

        /// <summary>
        /// 创建免疫Buff
        /// </summary>
        /// <param name="name">Buff名称</param>
        /// <param name="description">Buff描述</param>
        /// <param name="duration">持续回合数</param>
        /// <param name="immuneToStatusEffects">是否免疫状态效果</param>
        /// <param name="caster">施法者</param>
        /// <param name="target">目标</param>
        /// <returns>免疫Buff</returns>
        IBuff CreateImmunityBuff(string name, string description, int duration,
            bool immuneToStatusEffects, ICombatCharacter caster, ICombatCharacter target);
    }
}
