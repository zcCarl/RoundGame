using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TacticalRPG.Core.Modules.Character;

namespace TacticalRPG.Core.Modules.Skill
{
    /// <summary>
    /// 技能效果接口
    /// </summary>
    public interface ISkillEffect
    {
        /// <summary>
        /// 效果ID
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// 效果名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 效果描述
        /// </summary>
        string Description { get; }

        /// <summary>
        /// 所属技能ID
        /// </summary>
        Guid SkillId { get; }

        /// <summary>
        /// 效果类型
        /// </summary>
        SkillEffectType EffectType { get; }

        /// <summary>
        /// 效果目标
        /// </summary>
        SkillEffectTarget EffectTarget { get; }

        /// <summary>
        /// 效果持续回合数（0表示立即效果）
        /// </summary>
        int Duration { get; }

        /// <summary>
        /// 效果力量值（基础值）
        /// </summary>
        int Power { get; }

        /// <summary>
        /// 是否为区域效果
        /// </summary>
        bool IsAreaEffect { get; }

        /// <summary>
        /// 设置效果名称
        /// </summary>
        /// <param name="name">名称</param>
        void SetName(string name);

        /// <summary>
        /// 设置效果描述
        /// </summary>
        /// <param name="description">描述</param>
        void SetDescription(string description);

        /// <summary>
        /// 设置所属技能ID
        /// </summary>
        /// <param name="skillId">技能ID</param>
        void SetSkillId(Guid skillId);

        /// <summary>
        /// 设置效果类型
        /// </summary>
        /// <param name="effectType">效果类型</param>
        void SetEffectType(SkillEffectType effectType);

        /// <summary>
        /// 设置效果目标
        /// </summary>
        /// <param name="effectTarget">效果目标</param>
        void SetEffectTarget(SkillEffectTarget effectTarget);

        /// <summary>
        /// 设置持续回合数
        /// </summary>
        /// <param name="duration">持续回合数</param>
        void SetDuration(int duration);

        /// <summary>
        /// 设置效果力量值
        /// </summary>
        /// <param name="power">力量值</param>
        void SetPower(int power);

        /// <summary>
        /// 设置是否为区域效果
        /// </summary>
        /// <param name="isAreaEffect">是否为区域效果</param>
        void SetIsAreaEffect(bool isAreaEffect);

        /// <summary>
        /// 应用即时效果
        /// </summary>
        /// <param name="caster">施法者</param>
        /// <param name="targets">目标列表</param>
        /// <returns>操作结果</returns>
        Task<bool> ApplyImmediateEffectAsync(ICharacter caster, IReadOnlyList<ICharacter> targets);

        /// <summary>
        /// 应用持续效果
        /// </summary>
        /// <param name="caster">施法者</param>
        /// <param name="targets">目标列表</param>
        /// <returns>操作结果</returns>
        Task<bool> ApplyDurationEffectAsync(ICharacter caster, IReadOnlyList<ICharacter> targets);

        /// <summary>
        /// 移除效果
        /// </summary>
        /// <param name="target">目标</param>
        /// <returns>操作结果</returns>
        Task<bool> RemoveEffectAsync(ICharacter target);

        /// <summary>
        /// 效果回合开始时
        /// </summary>
        /// <param name="target">目标</param>
        /// <returns>操作结果</returns>
        Task<bool> OnTurnStartAsync(ICharacter target);

        /// <summary>
        /// 效果回合结束时
        /// </summary>
        /// <param name="target">目标</param>
        /// <returns>操作结果</returns>
        Task<bool> OnTurnEndAsync(ICharacter target);

        /// <summary>
        /// 计算实际效果值
        /// </summary>
        /// <param name="caster">施法者</param>
        /// <param name="target">目标</param>
        /// <returns>实际效果值</returns>
        int CalculateEffectValue(ICharacter caster, ICharacter target);

        /// <summary>
        /// 创建效果实例的深拷贝
        /// </summary>
        /// <returns>效果实例的深拷贝</returns>
        ISkillEffect Clone();
    }
}