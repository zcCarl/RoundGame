using System;
using System.Collections.Generic;
using TacticalRPG.Core.Modules.Character;

namespace TacticalRPG.Core.Modules.Skill
{
    /// <summary>
    /// 技能接口
    /// </summary>
    public interface ISkill
    {
        /// <summary>
        /// 技能ID
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// 技能名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 技能描述
        /// </summary>
        string Description { get; }

        /// <summary>
        /// 技能类型
        /// </summary>
        SkillType Type { get; }

        /// <summary>
        /// 目标类型
        /// </summary>
        TargetType TargetType { get; }

        /// <summary>
        /// 技能范围
        /// </summary>
        int Range { get; }

        /// <summary>
        /// 影响区域
        /// </summary>
        int Area { get; }

        /// <summary>
        /// 魔法消耗
        /// </summary>
        int MPCost { get; }

        /// <summary>
        /// 冷却回合
        /// </summary>
        int Cooldown { get; }

        /// <summary>
        /// 当前冷却
        /// </summary>
        int CurrentCooldown { get; }

        /// <summary>
        /// 基础伤害/治疗值
        /// </summary>
        int BasePower { get; }

        /// <summary>
        /// 可学习的职业列表
        /// </summary>
        IReadOnlyList<CharacterClass> LearnableClasses { get; }

        /// <summary>
        /// 设置名称
        /// </summary>
        /// <param name="name">名称</param>
        void SetName(string name);

        /// <summary>
        /// 设置描述
        /// </summary>
        /// <param name="description">描述</param>
        void SetDescription(string description);

        /// <summary>
        /// 设置技能类型
        /// </summary>
        /// <param name="type">技能类型</param>
        void SetType(SkillType type);

        /// <summary>
        /// 设置目标类型
        /// </summary>
        /// <param name="targetType">目标类型</param>
        void SetTargetType(TargetType targetType);

        /// <summary>
        /// 设置技能范围
        /// </summary>
        /// <param name="range">范围</param>
        void SetRange(int range);

        /// <summary>
        /// 设置影响区域
        /// </summary>
        /// <param name="area">区域</param>
        void SetArea(int area);

        /// <summary>
        /// 设置魔法消耗
        /// </summary>
        /// <param name="mpCost">魔法消耗</param>
        void SetMPCost(int mpCost);

        /// <summary>
        /// 设置冷却回合
        /// </summary>
        /// <param name="cooldown">冷却回合</param>
        void SetCooldown(int cooldown);

        /// <summary>
        /// 设置当前冷却
        /// </summary>
        /// <param name="currentCooldown">当前冷却</param>
        void SetCurrentCooldown(int currentCooldown);

        /// <summary>
        /// 设置基础伤害/治疗值
        /// </summary>
        /// <param name="basePower">基础伤害/治疗值</param>
        void SetBasePower(int basePower);

        /// <summary>
        /// 添加可学习职业
        /// </summary>
        /// <param name="characterClass">职业</param>
        void AddLearnableClass(CharacterClass characterClass);

        /// <summary>
        /// 移除可学习职业
        /// </summary>
        /// <param name="characterClass">职业</param>
        void RemoveLearnableClass(CharacterClass characterClass);

        /// <summary>
        /// 重置冷却
        /// </summary>
        void ResetCooldown();

        /// <summary>
        /// 减少冷却
        /// </summary>
        /// <param name="amount">减少量</param>
        void ReduceCooldown(int amount);

        /// <summary>
        /// 检查是否可用
        /// </summary>
        /// <returns>是否可用</returns>
        bool IsReady();

        /// <summary>
        /// 检查角色是否可以学习此技能
        /// </summary>
        /// <param name="character">角色</param>
        /// <returns>是否可以学习</returns>
        bool CanLearn(ICharacter character);
    }
}
