using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TacticalRPG.Core.Framework;
using TacticalRPG.Core.Modules.Character;

namespace TacticalRPG.Core.Modules.Skill
{
    /// <summary>
    /// 技能模块接口
    /// </summary>
    public interface ISkillModule
    {
        /// <summary>
        /// 创建技能
        /// </summary>
        /// <param name="name">技能名称</param>
        /// <param name="description">技能描述</param>
        /// <param name="skillType">技能类型</param>
        /// <param name="targetType">目标类型</param>
        /// <param name="range">技能范围</param>
        /// <param name="area">影响区域</param>
        /// <param name="mpCost">魔法消耗</param>
        /// <param name="cooldown">冷却回合</param>
        /// <returns>新技能的ID</returns>
        Task<Guid> CreateSkillAsync(string name, string description, SkillType skillType, TargetType targetType, int range, int area, int mpCost, int cooldown);

        /// <summary>
        /// 获取技能
        /// </summary>
        /// <param name="skillId">技能ID</param>
        /// <returns>技能对象</returns>
        ISkill GetSkill(Guid skillId);

        /// <summary>
        /// 获取所有技能
        /// </summary>
        /// <returns>所有技能的列表</returns>
        IReadOnlyList<ISkill> GetAllSkills();

        /// <summary>
        /// 获取指定类型的所有技能
        /// </summary>
        /// <param name="skillType">技能类型</param>
        /// <returns>指定类型的所有技能</returns>
        IReadOnlyList<ISkill> GetSkillsByType(SkillType skillType);

        /// <summary>
        /// 获取指定职业可学习的所有技能
        /// </summary>
        /// <param name="characterClass">角色职业</param>
        /// <returns>指定职业可学习的所有技能</returns>
        IReadOnlyList<ISkill> GetSkillsByClass(CharacterClass characterClass);

        /// <summary>
        /// 保存技能
        /// </summary>
        /// <param name="skill">技能对象</param>
        /// <returns>操作结果</returns>
        Task<bool> SaveSkillAsync(ISkill skill);

        /// <summary>
        /// 删除技能
        /// </summary>
        /// <param name="skillId">技能ID</param>
        /// <returns>操作结果</returns>
        Task<bool> DeleteSkillAsync(Guid skillId);

        /// <summary>
        /// 计算技能范围
        /// </summary>
        /// <param name="skillId">技能ID</param>
        /// <param name="x">中心X坐标</param>
        /// <param name="y">中心Y坐标</param>
        /// <returns>技能可影响的坐标列表</returns>
        IReadOnlyList<(int X, int Y)> CalculateSkillRange(Guid skillId, int x, int y);

        /// <summary>
        /// 计算技能影响区域
        /// </summary>
        /// <param name="skillId">技能ID</param>
        /// <param name="targetX">目标X坐标</param>
        /// <param name="targetY">目标Y坐标</param>
        /// <returns>技能影响的坐标列表</returns>
        IReadOnlyList<(int X, int Y)> CalculateSkillArea(Guid skillId, int targetX, int targetY);

        /// <summary>
        /// 检查角色是否可以学习技能
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <param name="skillId">技能ID</param>
        /// <returns>是否可以学习</returns>
        bool CanLearnSkill(Guid characterId, Guid skillId);

        /// <summary>
        /// 检查角色是否可以使用技能
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <param name="skillId">技能ID</param>
        /// <returns>是否可以使用</returns>
        bool CanUseSkill(Guid characterId, Guid skillId);
    }
}
