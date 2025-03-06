using System;
using TacticalRPG.Core.Modules.Skill;

namespace TacticalRPG.Core.Modules.Equipment
{
    /// <summary>
    /// 定义武器的接口，继承自装备接口
    /// </summary>
    public interface IWeapon : IEquipment
    {
        /// <summary>
        /// 获取武器的基础伤害
        /// </summary>
        int BaseDamage { get; }

        /// <summary>
        /// 获取武器的攻击范围（格子数）
        /// </summary>
        int AttackRange { get; }

        /// <summary>
        /// 获取武器的攻击速度
        /// </summary>
        float AttackSpeed { get; }

        /// <summary>
        /// 获取武器的暴击率
        /// </summary>
        float CriticalRate { get; }

        /// <summary>
        /// 获取武器的暴击伤害倍率
        /// </summary>
        float CriticalDamage { get; }

        /// <summary>
        /// 获取武器的命中率
        /// </summary>
        float AccuracyRate { get; }

        /// <summary>
        /// 获取武器的穿透率
        /// </summary>
        float PenetrationRate { get; }

        /// <summary>
        /// 获取武器的元素伤害类型（如有）
        /// </summary>
        SkillEffectType ElementalDamageType { get; }

        /// <summary>
        /// 获取武器的元素伤害值
        /// </summary>
        int ElementalDamage { get; }

        /// <summary>
        /// 获取武器攻击是否为范围攻击
        /// </summary>
        bool IsAreaAttack { get; }

        /// <summary>
        /// 获取武器范围攻击的半径（如适用）
        /// </summary>
        int AreaAttackRadius { get; }

        /// <summary>
        /// 获取武器是否为双手武器
        /// </summary>
        bool IsTwoHanded { get; }

        /// <summary>
        /// 获取武器的攻击动画名称
        /// </summary>
        string AttackAnimationName { get; }

        /// <summary>
        /// 获取武器的特殊效果描述
        /// </summary>
        string SpecialEffectDescription { get; }

        /// <summary>
        /// 计算武器对目标的伤害
        /// </summary>
        /// <param name="targetDefense">目标的防御值</param>
        /// <param name="isCritical">是否为暴击</param>
        /// <returns>计算出的伤害值</returns>
        int CalculateDamage(int targetDefense, bool isCritical = false);

        /// <summary>
        /// 尝试触发武器的特殊效果
        /// </summary>
        /// <param name="targetId">目标ID</param>
        /// <param name="chance">触发几率（0-1）</param>
        /// <returns>是否成功触发特殊效果</returns>
        bool TryTriggerSpecialEffect(Guid targetId, float chance = 1.0f);

        /// <summary>
        /// 设置武器的基础伤害
        /// </summary>
        /// <param name="damage">新的基础伤害值</param>
        /// <param name="reason">更改原因</param>
        /// <returns>是否成功设置</returns>
        bool SetBaseDamage(int damage, string reason = "");

        /// <summary>
        /// 设置武器的攻击范围
        /// </summary>
        /// <param name="range">新的攻击范围</param>
        /// <param name="reason">更改原因</param>
        /// <returns>是否成功设置</returns>
        bool SetAttackRange(int range, string reason = "");

        /// <summary>
        /// 设置武器的元素伤害
        /// </summary>
        /// <param name="type">元素类型</param>
        /// <param name="damage">元素伤害值</param>
        /// <param name="reason">更改原因</param>
        /// <returns>是否成功设置</returns>
        bool SetElementalDamage(SkillEffectType type, int damage, string reason = "");
    }
}