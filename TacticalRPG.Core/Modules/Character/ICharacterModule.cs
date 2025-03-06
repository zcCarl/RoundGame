using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TacticalRPG.Core.Framework;
using TacticalRPG.Core.Modules.Skill;

namespace TacticalRPG.Core.Modules.Character
{
    /// <summary>
    /// 角色模块接口
    /// </summary>
    public interface ICharacterModule
    {
        /// <summary>
        /// 创建角色
        /// </summary>
        /// <param name="name">角色名称</param>
        /// <param name="characterClass">角色职业</param>
        /// <returns>新角色的ID</returns>
        Task<Guid> CreateCharacterAsync(string name, CharacterClass characterClass);

        /// <summary>
        /// 获取角色
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <returns>角色对象</returns>
        ICharacter GetCharacter(Guid characterId);

        /// <summary>
        /// 获取所有角色
        /// </summary>
        /// <returns>所有角色的列表</returns>
        IReadOnlyList<ICharacter> GetAllCharacters();

        /// <summary>
        /// 保存角色
        /// </summary>
        /// <param name="character">角色对象</param>
        /// <returns>操作结果</returns>
        Task<bool> SaveCharacterAsync(ICharacter character);

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <returns>操作结果</returns>
        Task<bool> DeleteCharacterAsync(Guid characterId);

        /// <summary>
        /// 为角色学习技能
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <param name="skillId">技能ID</param>
        /// <returns>操作结果</returns>
        Task<bool> LearnSkillAsync(Guid characterId, Guid skillId);

        /// <summary>
        /// 为角色遗忘技能
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <param name="skillId">技能ID</param>
        /// <returns>操作结果</returns>
        Task<bool> ForgetSkillAsync(Guid characterId, Guid skillId);

        /// <summary>
        /// 获取角色的所有技能
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <returns>角色的所有技能</returns>
        IReadOnlyList<ISkill> GetCharacterSkills(Guid characterId);

        /// <summary>
        /// 为角色装备物品
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <param name="itemId">物品ID</param>
        /// <param name="slot">装备槽位</param>
        /// <returns>操作结果</returns>
        Task<bool> EquipItemAsync(Guid characterId, Guid itemId, EquipmentSlot slot);

        /// <summary>
        /// 为角色卸下装备
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <param name="slot">装备槽位</param>
        /// <returns>操作结果</returns>
        Task<bool> UnequipItemAsync(Guid characterId, EquipmentSlot slot);

        /// <summary>
        /// 获取角色的所有装备
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <returns>角色的所有装备</returns>
        IReadOnlyDictionary<EquipmentSlot, Guid> GetCharacterEquipment(Guid characterId);

        /// <summary>
        /// 角色升级
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <returns>操作结果</returns>
        Task<bool> LevelUpCharacterAsync(Guid characterId);

        /// <summary>
        /// 计算角色的战斗属性
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <returns>角色的战斗属性</returns>
        CharacterCombatStats CalculateCombatStats(Guid characterId);
    }
}