using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TacticalRPG.Core.Modules.Character;

namespace TacticalRPG.Core.Modules.Skill
{
    /// <summary>
    /// 技能效果管理器接口
    /// </summary>
    public interface ISkillEffectManager
    {
        /// <summary>
        /// 创建技能效果
        /// </summary>
        /// <param name="name">效果名称</param>
        /// <param name="description">效果描述</param>
        /// <param name="skillId">所属技能ID</param>
        /// <param name="effectType">效果类型</param>
        /// <param name="effectTarget">效果目标</param>
        /// <param name="duration">持续回合数</param>
        /// <param name="power">效果力量值</param>
        /// <param name="isAreaEffect">是否为区域效果</param>
        /// <returns>新效果的ID</returns>
        Task<Guid> CreateEffectAsync(string name, string description, Guid skillId, SkillEffectType effectType, SkillEffectTarget effectTarget, int duration, int power, bool isAreaEffect);

        /// <summary>
        /// 获取技能效果
        /// </summary>
        /// <param name="effectId">效果ID</param>
        /// <returns>效果对象</returns>
        ISkillEffect GetEffect(Guid effectId);

        /// <summary>
        /// 获取技能的所有效果
        /// </summary>
        /// <param name="skillId">技能ID</param>
        /// <returns>技能的所有效果</returns>
        IReadOnlyList<ISkillEffect> GetEffectsBySkill(Guid skillId);

        /// <summary>
        /// 获取特定类型的所有效果
        /// </summary>
        /// <param name="effectType">效果类型</param>
        /// <returns>特定类型的所有效果</returns>
        IReadOnlyList<ISkillEffect> GetEffectsByType(SkillEffectType effectType);

        /// <summary>
        /// 获取特定目标的所有效果
        /// </summary>
        /// <param name="effectTarget">效果目标</param>
        /// <returns>特定目标的所有效果</returns>
        IReadOnlyList<ISkillEffect> GetEffectsByTarget(SkillEffectTarget effectTarget);

        /// <summary>
        /// 保存技能效果
        /// </summary>
        /// <param name="effect">效果对象</param>
        /// <returns>操作结果</returns>
        Task<bool> SaveEffectAsync(ISkillEffect effect);

        /// <summary>
        /// 删除技能效果
        /// </summary>
        /// <param name="effectId">效果ID</param>
        /// <returns>操作结果</returns>
        Task<bool> DeleteEffectAsync(Guid effectId);

        /// <summary>
        /// 应用技能效果
        /// </summary>
        /// <param name="effectId">效果ID</param>
        /// <param name="caster">施法者</param>
        /// <param name="targets">目标列表</param>
        /// <returns>操作结果</returns>
        Task<bool> ApplyEffectAsync(Guid effectId, ICharacter caster, IReadOnlyList<ICharacter> targets);

        /// <summary>
        /// 移除角色身上的技能效果
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <param name="effectId">效果ID</param>
        /// <returns>操作结果</returns>
        Task<bool> RemoveEffectFromCharacterAsync(Guid characterId, Guid effectId);

        /// <summary>
        /// 获取角色身上的所有效果
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <returns>角色身上的所有效果</returns>
        IReadOnlyList<ISkillEffect> GetActiveEffectsOnCharacter(Guid characterId);

        /// <summary>
        /// 处理回合开始时的效果
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <returns>操作结果</returns>
        Task<bool> ProcessTurnStartEffectsAsync(Guid characterId);

        /// <summary>
        /// 处理回合结束时的效果
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <returns>操作结果</returns>
        Task<bool> ProcessTurnEndEffectsAsync(Guid characterId);

        /// <summary>
        /// 清除过期的效果
        /// </summary>
        /// <returns>操作结果</returns>
        Task<bool> CleanupExpiredEffectsAsync();
    }
}