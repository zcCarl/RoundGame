using System;
using System.Collections.Generic;

namespace TacticalRPG.Core.Modules.AI
{
    /// <summary>
    /// 战场评估结果
    /// </summary>
    public class BattleEvaluation
    {
        /// <summary>
        /// 我方队伍健康度（0-100）
        /// </summary>
        public int TeamHealth { get; set; }

        /// <summary>
        /// 敌方队伍健康度（0-100）
        /// </summary>
        public int EnemyHealth { get; set; }

        /// <summary>
        /// 我方队伍数量优势（-100到100，正数表示我方数量优势）
        /// </summary>
        public int TeamNumberAdvantage { get; set; }

        /// <summary>
        /// 我方队伍位置优势（-100到100，正数表示我方位置优势）
        /// </summary>
        public int TeamPositionAdvantage { get; set; }

        /// <summary>
        /// 当前角色健康状态（0-100）
        /// </summary>
        public int CurrentCharacterHealth { get; set; }

        /// <summary>
        /// 当前角色是否处于危险位置
        /// </summary>
        public bool IsInDangerousPosition { get; set; }

        /// <summary>
        /// 安全位置列表
        /// </summary>
        public List<TacticalPosition> SafePositions { get; set; } = new List<TacticalPosition>();

        /// <summary>
        /// 战术位置列表
        /// </summary>
        public List<TacticalPosition> TacticalPositions { get; set; } = new List<TacticalPosition>();

        /// <summary>
        /// 可攻击的敌人列表
        /// </summary>
        public List<TargetInfo> AttackableEnemies { get; set; } = new List<TargetInfo>();

        /// <summary>
        /// 需要治疗的队友列表
        /// </summary>
        public List<TargetInfo> HealableAllies { get; set; } = new List<TargetInfo>();

        /// <summary>
        /// 可用技能列表
        /// </summary>
        public List<SkillInfo> AvailableSkills { get; set; } = new List<SkillInfo>();

        /// <summary>
        /// 可用物品列表
        /// </summary>
        public List<ItemInfo> AvailableItems { get; set; } = new List<ItemInfo>();

        /// <summary>
        /// 整体战场态势（-100到100，正数表示我方优势）
        /// </summary>
        public int OverallSituation { get; set; }
    }

    /// <summary>
    /// 目标评估类，用于评估攻击或治疗目标
    /// </summary>
    public class TargetEvaluation
    {
        /// <summary>
        /// 目标角色ID
        /// </summary>
        public Guid CharacterId { get; set; }

        /// <summary>
        /// 目标X坐标
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// 目标Y坐标
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// 目标当前生命值百分比（0-100）
        /// </summary>
        public int HealthPercentage { get; set; }

        /// <summary>
        /// 目标威胁等级（0-100）
        /// </summary>
        public int ThreatLevel { get; set; }

        /// <summary>
        /// 目标优先级（数值越高优先级越高）
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// 攻击该目标的预期伤害
        /// </summary>
        public int ExpectedDamage { get; set; }

        /// <summary>
        /// 治疗该目标的预期恢复量
        /// </summary>
        public int ExpectedHealing { get; set; }

        /// <summary>
        /// 到达该目标所需的移动步数
        /// </summary>
        public int MovementRequired { get; set; }
    }

    /// <summary>
    /// 技能评估类，用于评估技能使用价值
    /// </summary>
    public class SkillEvaluation
    {
        /// <summary>
        /// 技能ID
        /// </summary>
        public Guid SkillId { get; set; }

        /// <summary>
        /// 技能名称
        /// </summary>
        public string SkillName { get; set; }

        /// <summary>
        /// 技能类型
        /// </summary>
        public string SkillType { get; set; }

        /// <summary>
        /// 技能使用价值（0-100）
        /// </summary>
        public int Value { get; set; }

        /// <summary>
        /// 最佳目标X坐标
        /// </summary>
        public int BestTargetX { get; set; }

        /// <summary>
        /// 最佳目标Y坐标
        /// </summary>
        public int BestTargetY { get; set; }

        /// <summary>
        /// 预期效果（伤害或治疗量）
        /// </summary>
        public int ExpectedEffect { get; set; }

        /// <summary>
        /// 影响的目标数量
        /// </summary>
        public int AffectedTargets { get; set; }
    }

    /// <summary>
    /// 物品评估类，用于评估物品使用价值
    /// </summary>
    public class ItemEvaluation
    {
        /// <summary>
        /// 物品ID
        /// </summary>
        public Guid ItemId { get; set; }

        /// <summary>
        /// 物品名称
        /// </summary>
        public string ItemName { get; set; }

        /// <summary>
        /// 物品类型
        /// </summary>
        public string ItemType { get; set; }

        /// <summary>
        /// 物品使用价值（0-100）
        /// </summary>
        public int Value { get; set; }

        /// <summary>
        /// 最佳目标角色ID
        /// </summary>
        public Guid BestTargetId { get; set; }

        /// <summary>
        /// 预期效果
        /// </summary>
        public int ExpectedEffect { get; set; }
    }

    /// <summary>
    /// 位置评估类，用于评估移动位置
    /// </summary>
    public class PositionEvaluation
    {
        /// <summary>
        /// X坐标
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// Y坐标
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// 位置价值（0-100）
        /// </summary>
        public int Value { get; set; }

        /// <summary>
        /// 到达该位置所需的移动步数
        /// </summary>
        public int MovementRequired { get; set; }

        /// <summary>
        /// 该位置的防御加成
        /// </summary>
        public int DefenseBonus { get; set; }

        /// <summary>
        /// 从该位置可攻击的敌人数量
        /// </summary>
        public int AttackableEnemiesCount { get; set; }

        /// <summary>
        /// 该位置的危险等级（0-100）
        /// </summary>
        public int DangerLevel { get; set; }
    }
}