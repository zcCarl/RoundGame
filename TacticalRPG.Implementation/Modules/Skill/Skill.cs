using System;
using System.Collections.Generic;
using TacticalRPG.Core.Modules.Character;
using TacticalRPG.Core.Modules.Skill;

namespace TacticalRPG.Implementation.Modules.Skill
{
    /// <summary>
    /// 技能实现类
    /// </summary>
    public class Skill : ISkill
    {
        private readonly List<CharacterClass> _learnableClasses = new List<CharacterClass>();

        /// <summary>
        /// 技能ID
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// 技能名称
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 技能描述
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// 技能类型
        /// </summary>
        public SkillType Type { get; private set; }

        /// <summary>
        /// 目标类型
        /// </summary>
        public TargetType TargetType { get; private set; }

        /// <summary>
        /// 技能范围
        /// </summary>
        public int Range { get; private set; }

        /// <summary>
        /// 影响区域
        /// </summary>
        public int Area { get; private set; }

        /// <summary>
        /// 魔法消耗
        /// </summary>
        public int MPCost { get; private set; }

        /// <summary>
        /// 冷却回合
        /// </summary>
        public int Cooldown { get; private set; }

        /// <summary>
        /// 当前冷却
        /// </summary>
        public int CurrentCooldown { get; private set; }

        /// <summary>
        /// 基础伤害/治疗值
        /// </summary>
        public int BasePower { get; private set; }

        /// <summary>
        /// 可学习的职业列表
        /// </summary>
        public IReadOnlyList<CharacterClass> LearnableClasses => _learnableClasses.AsReadOnly();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="id">技能ID</param>
        /// <param name="name">技能名称</param>
        /// <param name="description">技能描述</param>
        /// <param name="type">技能类型</param>
        /// <param name="targetType">目标类型</param>
        /// <param name="range">技能范围</param>
        /// <param name="area">影响区域</param>
        /// <param name="mpCost">魔法消耗</param>
        /// <param name="cooldown">冷却回合</param>
        public Skill(
            Guid id,
            string name,
            string description,
            SkillType type,
            TargetType targetType,
            int range,
            int area,
            int mpCost,
            int cooldown)
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id;
            Name = name;
            Description = description;
            Type = type;
            TargetType = targetType;
            Range = range;
            Area = area;
            MPCost = mpCost;
            Cooldown = cooldown;
            CurrentCooldown = 0;
            BasePower = 0;
        }

        /// <summary>
        /// 设置名称
        /// </summary>
        /// <param name="name">名称</param>
        public void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("技能名称不能为空", nameof(name));
            }

            Name = name;
        }

        /// <summary>
        /// 设置描述
        /// </summary>
        /// <param name="description">描述</param>
        public void SetDescription(string description)
        {
            Description = description ?? string.Empty;
        }

        /// <summary>
        /// 设置技能类型
        /// </summary>
        /// <param name="type">技能类型</param>
        public void SetType(SkillType type)
        {
            Type = type;
        }

        /// <summary>
        /// 设置目标类型
        /// </summary>
        /// <param name="targetType">目标类型</param>
        public void SetTargetType(TargetType targetType)
        {
            TargetType = targetType;
        }

        /// <summary>
        /// 设置技能范围
        /// </summary>
        /// <param name="range">范围</param>
        public void SetRange(int range)
        {
            if (range < 0)
            {
                throw new ArgumentException("技能范围不能为负数", nameof(range));
            }

            Range = range;
        }

        /// <summary>
        /// 设置影响区域
        /// </summary>
        /// <param name="area">区域</param>
        public void SetArea(int area)
        {
            if (area < 0)
            {
                throw new ArgumentException("影响区域不能为负数", nameof(area));
            }

            Area = area;
        }

        /// <summary>
        /// 设置魔法消耗
        /// </summary>
        /// <param name="mpCost">魔法消耗</param>
        public void SetMPCost(int mpCost)
        {
            if (mpCost < 0)
            {
                throw new ArgumentException("魔法消耗不能为负数", nameof(mpCost));
            }

            MPCost = mpCost;
        }

        /// <summary>
        /// 设置冷却回合
        /// </summary>
        /// <param name="cooldown">冷却回合</param>
        public void SetCooldown(int cooldown)
        {
            if (cooldown < 0)
            {
                throw new ArgumentException("冷却回合不能为负数", nameof(cooldown));
            }

            Cooldown = cooldown;
        }

        /// <summary>
        /// 设置当前冷却
        /// </summary>
        /// <param name="currentCooldown">当前冷却</param>
        public void SetCurrentCooldown(int currentCooldown)
        {
            if (currentCooldown < 0)
            {
                throw new ArgumentException("当前冷却不能为负数", nameof(currentCooldown));
            }

            CurrentCooldown = currentCooldown;
        }

        /// <summary>
        /// 设置基础伤害/治疗值
        /// </summary>
        /// <param name="basePower">基础伤害/治疗值</param>
        public void SetBasePower(int basePower)
        {
            BasePower = basePower;
        }

        /// <summary>
        /// 添加可学习职业
        /// </summary>
        /// <param name="characterClass">职业</param>
        public void AddLearnableClass(CharacterClass characterClass)
        {
            if (!_learnableClasses.Contains(characterClass))
            {
                _learnableClasses.Add(characterClass);
            }
        }

        /// <summary>
        /// 移除可学习职业
        /// </summary>
        /// <param name="characterClass">职业</param>
        public void RemoveLearnableClass(CharacterClass characterClass)
        {
            _learnableClasses.Remove(characterClass);
        }

        /// <summary>
        /// 重置冷却
        /// </summary>
        public void ResetCooldown()
        {
            CurrentCooldown = 0;
        }

        /// <summary>
        /// 减少冷却
        /// </summary>
        /// <param name="amount">减少量</param>
        public void ReduceCooldown(int amount)
        {
            if (amount < 0)
            {
                throw new ArgumentException("减少量不能为负数", nameof(amount));
            }

            CurrentCooldown = Math.Max(0, CurrentCooldown - amount);
        }

        /// <summary>
        /// 检查是否可用
        /// </summary>
        /// <returns>是否可用</returns>
        public bool IsReady()
        {
            return CurrentCooldown <= 0;
        }

        /// <summary>
        /// 检查角色是否可以学习此技能
        /// </summary>
        /// <param name="character">角色</param>
        /// <returns>是否可以学习</returns>
        public bool CanLearn(ICharacter character)
        {
            if (character == null)
            {
                throw new ArgumentNullException(nameof(character));
            }

            return _learnableClasses.Contains(character.Class);
        }
    }
}