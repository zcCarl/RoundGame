using System;
using TacticalRPG.Core.Modules.Character;

namespace TacticalRPG.Core.Modules.Buff
{
    /// <summary>
    /// Buff工厂实现类
    /// </summary>
    public class BuffFactory : IBuffFactory
    {
        /// <summary>
        /// 创建Buff实例
        /// </summary>
        /// <param name="buffType">Buff类型</param>
        /// <param name="statusEffectType">状态效果类型</param>
        /// <returns>Buff实例</returns>
        public IBuff CreateBuff(BuffType buffType, StatusEffectType statusEffectType)
        {
            // 创建一个基础Buff
            return new BaseBuff("Buff", "基础Buff", buffType, statusEffectType, 1, 1);
        }

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
        public IBuff CreateStatusBuff(string name, string description, StatusEffectType statusEffectType,
            int duration, ICombatCharacter caster, ICombatCharacter target)
        {
            // 创建一个状态效果Buff
            return new StatusBuff(name,
                                 description,
                                 statusEffectType,
                                 duration,
                                 1, // 默认强度为1 
                                 caster,
                                 target);
        }

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
        public IBuff CreateStatBoostBuff(string name, string description, int duration,
            int power, ICombatCharacter caster, ICombatCharacter target)
        {
            // 创建一个属性增强Buff
            var buff = new BaseBuff(name,
                               description,
                               BuffType.Positive,
                               StatusEffectType.None,
                               duration,
                               power);

            buff.SetCaster(caster);
            buff.SetTarget(target);

            return buff;
        }

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
        public IBuff CreateStatReductionBuff(string name, string description, int duration,
            int power, ICombatCharacter caster, ICombatCharacter target)
        {
            // 创建一个属性减弱Buff
            var buff = new BaseBuff(name,
                               description,
                               BuffType.Negative,
                               StatusEffectType.None,
                               duration,
                               power);

            buff.SetCaster(caster);
            buff.SetTarget(target);

            return buff;
        }

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
        public IBuff CreateDamageOverTimeBuff(string name, string description, int duration,
            int damagePerTurn, ICombatCharacter caster, ICombatCharacter target)
        {
            // 创建一个持续伤害Buff
            return new StatusBuff(name,
                                 description,
                                 StatusEffectType.Poison, // 使用毒性作为默认持续伤害类型
                                 duration,
                                 damagePerTurn,
                                 caster,
                                 target);
        }

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
        public IBuff CreateHealOverTimeBuff(string name, string description, int duration,
            int healPerTurn, ICombatCharacter caster, ICombatCharacter target)
        {
            // 创建一个持续治疗Buff
            return new StatusBuff(name,
                                 description,
                                 StatusEffectType.Regeneration,
                                 duration,
                                 healPerTurn,
                                 caster,
                                 target);
        }

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
        public IBuff CreateShieldBuff(string name, string description, int duration,
            int shieldAmount, ICombatCharacter caster, ICombatCharacter target)
        {
            // 创建一个护盾Buff
            return new StatusBuff(name,
                                 description,
                                 StatusEffectType.Shield,
                                 duration,
                                 shieldAmount,
                                 caster,
                                 target);
        }

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
        public IBuff CreateControlBuff(string name, string description, StatusEffectType statusEffectType,
            int duration, ICombatCharacter caster, ICombatCharacter target)
        {
            // 验证状态效果类型是否为控制类型
            if (statusEffectType != StatusEffectType.Stun &&
                statusEffectType != StatusEffectType.Silence &&
                statusEffectType != StatusEffectType.Root &&
                statusEffectType != StatusEffectType.Sleep &&
                statusEffectType != StatusEffectType.Confusion &&
                statusEffectType != StatusEffectType.Charm &&
                statusEffectType != StatusEffectType.Fear &&
                statusEffectType != StatusEffectType.Freeze &&
                statusEffectType != StatusEffectType.Paralyze &&
                statusEffectType != StatusEffectType.Taunt)
            {
                throw new ArgumentException("Invalid control status effect type", nameof(statusEffectType));
            }

            // 创建一个控制Buff
            return new StatusBuff(name,
                                 description,
                                 statusEffectType,
                                 duration,
                                 1, // 控制效果通常强度为1
                                 caster,
                                 target);
        }

        /// <summary>
        /// 创建反射Buff
        /// </summary>
        /// <param name="name">Buff名称</param>
        /// <param name="description">Buff描述</param>
        /// <param name="duration">持续回合数</param>
        /// <param name="reflectChance">反射几率</param>
        /// <param name="caster">施法者</param>
        /// <param name="target">目标</param>
        /// <returns>反射Buff</returns>
        public IBuff CreateReflectBuff(string name, string description, int duration,
            int reflectChance, ICombatCharacter caster, ICombatCharacter target)
        {
            // 创建一个反射Buff
            return new StatusBuff(name,
                                 description,
                                 StatusEffectType.Reflect,
                                 duration,
                                 reflectChance,
                                 caster,
                                 target);
        }

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
        public IBuff CreateImmunityBuff(string name, string description, int duration,
            bool immuneToStatusEffects, ICombatCharacter caster, ICombatCharacter target)
        {
            // 创建一个免疫Buff
            var buff = new StatusBuff(name,
                                     description,
                                     StatusEffectType.Invulnerable,
                                     duration,
                                     immuneToStatusEffects ? 1 : 0,
                                     caster,
                                     target);

            return buff;
        }
    }
}