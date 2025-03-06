using System;

namespace TacticalRPG.Core.Modules.Skill
{
    /// <summary>
    /// 技能效果工厂接口
    /// </summary>
    public interface ISkillEffectFactory
    {
        /// <summary>
        /// 创建技能效果实例
        /// </summary>
        /// <param name="effectType">效果类型</param>
        /// <param name="effectTarget">效果目标</param>
        /// <returns>技能效果实例</returns>
        ISkillEffect CreateEffect(SkillEffectType effectType, SkillEffectTarget effectTarget);

        /// <summary>
        /// 创建直接伤害效果
        /// </summary>
        /// <param name="name">效果名称</param>
        /// <param name="description">效果描述</param>
        /// <param name="skillId">所属技能ID</param>
        /// <param name="power">伤害值</param>
        /// <param name="isAreaEffect">是否为区域效果</param>
        /// <returns>直接伤害效果</returns>
        ISkillEffect CreateDirectDamageEffect(string name, string description, Guid skillId, int power, bool isAreaEffect);

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
        ISkillEffect CreateDamageOverTimeEffect(string name, string description, Guid skillId, int power, int duration, bool isAreaEffect);

        /// <summary>
        /// 创建直接治疗效果
        /// </summary>
        /// <param name="name">效果名称</param>
        /// <param name="description">效果描述</param>
        /// <param name="skillId">所属技能ID</param>
        /// <param name="power">治疗值</param>
        /// <param name="isAreaEffect">是否为区域效果</param>
        /// <returns>直接治疗效果</returns>
        ISkillEffect CreateDirectHealEffect(string name, string description, Guid skillId, int power, bool isAreaEffect);

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
        ISkillEffect CreateHealOverTimeEffect(string name, string description, Guid skillId, int power, int duration, bool isAreaEffect);

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
        ISkillEffect CreateStatBoostEffect(string name, string description, Guid skillId, SkillEffectTarget effectTarget, int power, int duration, bool isAreaEffect);

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
        ISkillEffect CreateStatReductionEffect(string name, string description, Guid skillId, SkillEffectTarget effectTarget, int power, int duration, bool isAreaEffect);

        /// <summary>
        /// 创建状态改变效果
        /// </summary>
        /// <param name="name">效果名称</param>
        /// <param name="description">效果描述</param>
        /// <param name="skillId">所属技能ID</param>
        /// <param name="duration">持续回合数</param>
        /// <param name="isAreaEffect">是否为区域效果</param>
        /// <returns>状态改变效果</returns>
        ISkillEffect CreateStatusChangeEffect(string name, string description, Guid skillId, int duration, bool isAreaEffect);
    }
}