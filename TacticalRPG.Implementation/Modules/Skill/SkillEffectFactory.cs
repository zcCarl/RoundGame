using System;
using System.Collections.Generic;
using TacticalRPG.Core.Modules.Skill;

namespace TacticalRPG.Implementation.Modules.Skill
{
    /// <summary>
    /// 技能效果工厂类，用于创建不同类型的技能效果
    /// </summary>
    public class SkillEffectFactory : ISkillEffectFactory
    {
        /// <summary>
        /// 创建技能效果实例
        /// </summary>
        /// <param name="effectType">效果类型</param>
        /// <param name="effectTarget">效果目标</param>
        /// <returns>技能效果实例</returns>
        public ISkillEffect CreateEffect(SkillEffectType effectType, SkillEffectTarget effectTarget)
        {
            var id = Guid.NewGuid();
            string name = $"{effectType} Effect";
            string description = $"Default {effectType} effect";
            var skillId = Guid.Empty;
            int duration = 0;
            int power = 0;
            bool isAreaEffect = false;

            return new SkillEffect(
                id,
                name,
                description,
                skillId,
                effectType,
                effectTarget,
                duration,
                power,
                isAreaEffect);
        }

        /// <summary>
        /// 创建直接伤害效果
        /// </summary>
        /// <param name="name">效果名称</param>
        /// <param name="description">效果描述</param>
        /// <param name="skillId">所属技能ID</param>
        /// <param name="power">伤害值</param>
        /// <param name="isAreaEffect">是否为区域效果</param>
        /// <returns>直接伤害效果</returns>
        public ISkillEffect CreateDirectDamageEffect(string name, string description, Guid skillId, int power, bool isAreaEffect)
        {
            var id = Guid.NewGuid();
            return new SkillEffect(
                id,
                name,
                description,
                skillId,
                SkillEffectType.DirectDamage,
                SkillEffectTarget.HP,
                0, // 直接伤害是即时效果，持续时间为0
                power,
                isAreaEffect);
        }

        /// <summary>
        /// 创建持续伤害效果
        /// </summary>
        /// <param name="name">效果名称</param>
        /// <param name="description">效果描述</param>
        /// <param name="skillId">所属技能ID</param>
        /// <param name="power">每回合伤害值</param>
        /// <param name="duration">持续回合数</param>
        /// <param name="isAreaEffect">是否为区域效果</param>
        /// <returns>持续伤害效果</returns>
        public ISkillEffect CreateDamageOverTimeEffect(string name, string description, Guid skillId, int power, int duration, bool isAreaEffect)
        {
            var id = Guid.NewGuid();
            return new SkillEffect(
                id,
                name,
                description,
                skillId,
                SkillEffectType.DamageOverTime,
                SkillEffectTarget.HP,
                duration,
                power,
                isAreaEffect);
        }

        /// <summary>
        /// 创建直接治疗效果
        /// </summary>
        /// <param name="name">效果名称</param>
        /// <param name="description">效果描述</param>
        /// <param name="skillId">所属技能ID</param>
        /// <param name="power">治疗值</param>
        /// <param name="isAreaEffect">是否为区域效果</param>
        /// <returns>直接治疗效果</returns>
        public ISkillEffect CreateDirectHealEffect(string name, string description, Guid skillId, int power, bool isAreaEffect)
        {
            var id = Guid.NewGuid();
            return new SkillEffect(
                id,
                name,
                description,
                skillId,
                SkillEffectType.DirectHeal,
                SkillEffectTarget.HP,
                0, // 直接治疗是即时效果，持续时间为0
                power,
                isAreaEffect);
        }

        /// <summary>
        /// 创建持续治疗效果
        /// </summary>
        /// <param name="name">效果名称</param>
        /// <param name="description">效果描述</param>
        /// <param name="skillId">所属技能ID</param>
        /// <param name="power">每回合治疗值</param>
        /// <param name="duration">持续回合数</param>
        /// <param name="isAreaEffect">是否为区域效果</param>
        /// <returns>持续治疗效果</returns>
        public ISkillEffect CreateHealOverTimeEffect(string name, string description, Guid skillId, int power, int duration, bool isAreaEffect)
        {
            var id = Guid.NewGuid();
            return new SkillEffect(
                id,
                name,
                description,
                skillId,
                SkillEffectType.HealOverTime,
                SkillEffectTarget.HP,
                duration,
                power,
                isAreaEffect);
        }

        /// <summary>
        /// 创建属性增强效果
        /// </summary>
        /// <param name="name">效果名称</param>
        /// <param name="description">效果描述</param>
        /// <param name="skillId">所属技能ID</param>
        /// <param name="effectTarget">效果目标属性</param>
        /// <param name="power">增强幅度</param>
        /// <param name="duration">持续回合数</param>
        /// <param name="isAreaEffect">是否为区域效果</param>
        /// <returns>属性增强效果</returns>
        public ISkillEffect CreateStatBoostEffect(string name, string description, Guid skillId, SkillEffectTarget effectTarget, int power, int duration, bool isAreaEffect)
        {
            var id = Guid.NewGuid();
            return new SkillEffect(
                id,
                name,
                description,
                skillId,
                SkillEffectType.StatBoost,
                effectTarget,
                duration,
                power,
                isAreaEffect);
        }

        /// <summary>
        /// 创建属性减弱效果
        /// </summary>
        /// <param name="name">效果名称</param>
        /// <param name="description">效果描述</param>
        /// <param name="skillId">所属技能ID</param>
        /// <param name="effectTarget">效果目标属性</param>
        /// <param name="power">减弱幅度</param>
        /// <param name="duration">持续回合数</param>
        /// <param name="isAreaEffect">是否为区域效果</param>
        /// <returns>属性减弱效果</returns>
        public ISkillEffect CreateStatReductionEffect(string name, string description, Guid skillId, SkillEffectTarget effectTarget, int power, int duration, bool isAreaEffect)
        {
            var id = Guid.NewGuid();
            return new SkillEffect(
                id,
                name,
                description,
                skillId,
                SkillEffectType.StatReduction,
                effectTarget,
                duration,
                power,
                isAreaEffect);
        }

        /// <summary>
        /// 创建状态改变效果
        /// </summary>
        /// <param name="name">效果名称</param>
        /// <param name="description">效果描述</param>
        /// <param name="skillId">所属技能ID</param>
        /// <param name="duration">持续回合数</param>
        /// <param name="isAreaEffect">是否为区域效果</param>
        /// <returns>状态改变效果</returns>
        public ISkillEffect CreateStatusChangeEffect(string name, string description, Guid skillId, int duration, bool isAreaEffect)
        {
            var id = Guid.NewGuid();
            return new SkillEffect(
                id,
                name,
                description,
                skillId,
                SkillEffectType.StatusChange,
                SkillEffectTarget.Status,
                duration,
                0, // 状态改变通常不需要power值
                isAreaEffect);
        }
    }
}