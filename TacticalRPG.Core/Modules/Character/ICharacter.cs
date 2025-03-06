using System;
using System.Collections.Generic;

namespace TacticalRPG.Core.Modules.Character
{
    /// <summary>
    /// 角色接口
    /// </summary>
    public interface ICharacter
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// 角色名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 角色职业
        /// </summary>
        CharacterClass Class { get; }

        /// <summary>
        /// 角色等级
        /// </summary>
        int Level { get; }

        /// <summary>
        /// 当前经验值
        /// </summary>
        int Experience { get; }

        /// <summary>
        /// 升级所需经验值
        /// </summary>
        int ExperienceToNextLevel { get; }

        /// <summary>
        /// 最大生命值
        /// </summary>
        int MaxHP { get; }

        /// <summary>
        /// 最大魔法值
        /// </summary>
        int MaxMP { get; }

        /// <summary>
        /// 力量
        /// </summary>
        int Strength { get; }

        /// <summary>
        /// 敏捷
        /// </summary>
        int Agility { get; }

        /// <summary>
        /// 智力
        /// </summary>
        int Intelligence { get; }

        /// <summary>
        /// 体质
        /// </summary>
        int Constitution { get; }

        /// <summary>
        /// 幸运
        /// </summary>
        int Luck { get; }

        /// <summary>
        /// 移动力
        /// </summary>
        int Movement { get; }

        /// <summary>
        /// 技能列表
        /// </summary>
        IReadOnlyList<Guid> SkillIds { get; }

        /// <summary>
        /// 装备
        /// </summary>
        IReadOnlyDictionary<EquipmentSlot, Guid> Equipment { get; }

        /// <summary>
        /// 设置名称
        /// </summary>
        /// <param name="name">名称</param>
        void SetName(string name);

        /// <summary>
        /// 设置职业
        /// </summary>
        /// <param name="characterClass">职业</param>
        void SetClass(CharacterClass characterClass);

        /// <summary>
        /// 增加经验值
        /// </summary>
        /// <param name="amount">经验值</param>
        /// <returns>是否升级</returns>
        bool AddExperience(int amount);

        /// <summary>
        /// 升级
        /// </summary>
        /// <returns>是否成功升级</returns>
        bool LevelUp();

        /// <summary>
        /// 学习技能
        /// </summary>
        /// <param name="skillId">技能ID</param>
        /// <returns>是否成功学习</returns>
        bool LearnSkill(Guid skillId);

        /// <summary>
        /// 遗忘技能
        /// </summary>
        /// <param name="skillId">技能ID</param>
        /// <returns>是否成功遗忘</returns>
        bool ForgetSkill(Guid skillId);

        /// <summary>
        /// 装备物品
        /// </summary>
        /// <param name="itemId">物品ID</param>
        /// <param name="slot">装备槽位</param>
        /// <returns>是否成功装备</returns>
        bool EquipItem(Guid itemId, EquipmentSlot slot);

        /// <summary>
        /// 卸下装备
        /// </summary>
        /// <param name="slot">装备槽位</param>
        /// <returns>卸下的物品ID，如果没有装备则返回Guid.Empty</returns>
        Guid UnequipItem(EquipmentSlot slot);

        /// <summary>
        /// 计算战斗属性
        /// </summary>
        /// <returns>战斗属性</returns>
        CharacterCombatStats CalculateCombatStats();
    }
}