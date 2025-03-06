using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TacticalRPG.Core.Framework;
using TacticalRPG.Core.Modules.Character;

namespace TacticalRPG.Core.Modules.Buff
{
    /// <summary>
    /// Buff模块接口
    /// </summary>
    public interface IBuffModule
    {
        /// <summary>
        /// 获取Buff工厂
        /// </summary>
        IBuffFactory BuffFactory { get; }

        /// <summary>
        /// 创建Buff
        /// </summary>
        /// <param name="name">Buff名称</param>
        /// <param name="description">Buff描述</param>
        /// <param name="buffType">Buff类型</param>
        /// <param name="statusEffectType">状态效果类型</param>
        /// <param name="duration">持续回合数</param>
        /// <param name="power">效果强度</param>
        /// <param name="isStackable">是否可叠加</param>
        /// <param name="maxStackCount">最大叠加层数</param>
        /// <returns>新Buff的ID</returns>
        Task<Guid> CreateBuffAsync(string name, string description, BuffType buffType, StatusEffectType statusEffectType,
            int duration, int power, bool isStackable = false, int maxStackCount = 1);

        /// <summary>
        /// 获取Buff
        /// </summary>
        /// <param name="buffId">Buff ID</param>
        /// <returns>Buff对象</returns>
        IBuff GetBuff(Guid buffId);

        /// <summary>
        /// 获取所有Buff
        /// </summary>
        /// <returns>所有Buff的列表</returns>
        IReadOnlyList<IBuff> GetAllBuffs();

        /// <summary>
        /// 获取指定类型的所有Buff
        /// </summary>
        /// <param name="buffType">Buff类型</param>
        /// <returns>指定类型的所有Buff</returns>
        IReadOnlyList<IBuff> GetBuffsByType(BuffType buffType);

        /// <summary>
        /// 获取指定状态效果类型的所有Buff
        /// </summary>
        /// <param name="statusEffectType">状态效果类型</param>
        /// <returns>指定状态效果类型的所有Buff</returns>
        IReadOnlyList<IBuff> GetBuffsByStatusEffectType(StatusEffectType statusEffectType);

        /// <summary>
        /// 保存Buff
        /// </summary>
        /// <param name="buff">Buff对象</param>
        /// <returns>操作结果</returns>
        Task<bool> SaveBuffAsync(IBuff buff);

        /// <summary>
        /// 删除Buff
        /// </summary>
        /// <param name="buffId">Buff ID</param>
        /// <returns>操作结果</returns>
        Task<bool> DeleteBuffAsync(Guid buffId);

        /// <summary>
        /// 给角色应用Buff
        /// </summary>
        /// <param name="buffId">Buff ID</param>
        /// <param name="caster">施法者</param>
        /// <param name="target">目标</param>
        /// <returns>操作结果</returns>
        Task<bool> ApplyBuffToCharacterAsync(Guid buffId, ICombatCharacter caster, ICombatCharacter target);

        /// <summary>
        /// 从角色移除Buff
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <param name="buffId">Buff ID</param>
        /// <returns>操作结果</returns>
        Task<bool> RemoveBuffFromCharacterAsync(Guid characterId, Guid buffId);

        /// <summary>
        /// 从角色移除所有Buff
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <returns>操作结果</returns>
        Task<bool> RemoveAllBuffsFromCharacterAsync(Guid characterId);

        /// <summary>
        /// 从角色移除指定类型的所有Buff
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <param name="buffType">Buff类型</param>
        /// <returns>操作结果</returns>
        Task<bool> RemoveBuffsByTypeFromCharacterAsync(Guid characterId, BuffType buffType);

        /// <summary>
        /// 从角色移除指定状态效果类型的所有Buff
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <param name="statusEffectType">状态效果类型</param>
        /// <returns>操作结果</returns>
        Task<bool> RemoveBuffsByStatusEffectTypeFromCharacterAsync(Guid characterId, StatusEffectType statusEffectType);

        /// <summary>
        /// 获取角色身上的所有Buff
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <returns>角色身上的所有Buff</returns>
        IReadOnlyList<IBuff> GetBuffsOnCharacter(Guid characterId);

        /// <summary>
        /// 获取角色身上指定类型的所有Buff
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <param name="buffType">Buff类型</param>
        /// <returns>角色身上指定类型的所有Buff</returns>
        IReadOnlyList<IBuff> GetBuffsOnCharacterByType(Guid characterId, BuffType buffType);

        /// <summary>
        /// 获取角色身上指定状态效果类型的所有Buff
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <param name="statusEffectType">状态效果类型</param>
        /// <returns>角色身上指定状态效果类型的所有Buff</returns>
        IReadOnlyList<IBuff> GetBuffsOnCharacterByStatusEffectType(Guid characterId, StatusEffectType statusEffectType);

        /// <summary>
        /// 检查角色是否拥有指定Buff
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <param name="buffId">Buff ID</param>
        /// <returns>是否拥有指定Buff</returns>
        bool HasBuff(Guid characterId, Guid buffId);

        /// <summary>
        /// 检查角色是否拥有指定类型的Buff
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <param name="buffType">Buff类型</param>
        /// <returns>是否拥有指定类型的Buff</returns>
        bool HasBuffOfType(Guid characterId, BuffType buffType);

        /// <summary>
        /// 检查角色是否拥有指定状态效果类型的Buff
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <param name="statusEffectType">状态效果类型</param>
        /// <returns>是否拥有指定状态效果类型的Buff</returns>
        bool HasBuffOfStatusEffectType(Guid characterId, StatusEffectType statusEffectType);

        /// <summary>
        /// 处理角色回合开始时的Buff效果
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <returns>操作结果</returns>
        Task<bool> ProcessTurnStartBuffsAsync(Guid characterId);

        /// <summary>
        /// 处理角色回合结束时的Buff效果
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <returns>操作结果</returns>
        Task<bool> ProcessTurnEndBuffsAsync(Guid characterId);

        /// <summary>
        /// 处理角色受到伤害时的Buff效果
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <param name="damage">伤害值</param>
        /// <param name="attacker">攻击者</param>
        /// <returns>实际伤害值</returns>
        Task<int> ProcessDamageTakenBuffsAsync(Guid characterId, int damage, ICombatCharacter attacker);

        /// <summary>
        /// 处理角色造成伤害时的Buff效果
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <param name="damage">伤害值</param>
        /// <param name="victim">受害者</param>
        /// <returns>实际伤害值</returns>
        Task<int> ProcessDamageDealtBuffsAsync(Guid characterId, int damage, ICombatCharacter victim);

        /// <summary>
        /// El处理角色移动前的Buff效果
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <param name="fromX">起始X坐标</param>
        /// <param name="fromY">起始Y坐标</param>
        /// <param name="toX">目标X坐标</param>
        /// <param name="toY">目标Y坐标</param>
        /// <returns>是否允许移动</returns>
        Task<bool> ProcessBeforeMoveBuffsAsync(Guid characterId, int fromX, int fromY, int toX, int toY);

        /// <summary>
        /// 处理角色移动后的Buff效果
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <param name="fromX">起始X坐标</param>
        /// <param name="fromY">起始Y坐标</param>
        /// <param name="toX">目标X坐标</param>
        /// <param name="toY">目标Y坐标</param>
        /// <returns>操作结果</returns>
        Task<bool> ProcessAfterMoveBuffsAsync(Guid characterId, int fromX, int fromY, int toX, int toY);

        /// <summary>
        /// 清理过期的Buff
        /// </summary>
        /// <returns>操作结果</returns>
        Task<bool> CleanupExpiredBuffsAsync();
    }
}
