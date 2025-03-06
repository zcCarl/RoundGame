using System;

namespace TacticalRPG.Core.Modules.AI
{
    /// <summary>
    /// 目标信息类，用于AI战场评估
    /// </summary>
    public class TargetInfo
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
        /// 目标当前生命值百分比
        /// </summary>
        public int HealthPercentage { get; set; }

        /// <summary>
        /// 目标当前魔法值百分比
        /// </summary>
        public int ManaPercentage { get; set; }

        /// <summary>
        /// 目标构成的威胁等级
        /// </summary>
        public int ThreatLevel { get; set; }

        /// <summary>
        /// 目标的优先级
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// 预期对目标造成的伤害
        /// </summary>
        public int ExpectedDamage { get; set; }

        /// <summary>
        /// 预期对目标提供的治疗
        /// </summary>
        public int ExpectedHealing { get; set; }

        /// <summary>
        /// 到达目标所需的移动距离
        /// </summary>
        public int DistanceToTarget { get; set; }

        /// <summary>
        /// 目标当前的状态效果
        /// </summary>
        public string[] StatusEffects { get; set; }

        /// <summary>
        /// 目标职业
        /// </summary>
        public string CharacterClass { get; set; }
    }
}