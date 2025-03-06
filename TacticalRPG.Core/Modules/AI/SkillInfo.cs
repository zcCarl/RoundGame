using System;

namespace TacticalRPG.Core.Modules.AI
{
    /// <summary>
    /// 技能信息类，用于AI战场评估
    /// </summary>
    public class SkillInfo
    {
        /// <summary>
        /// 技能ID
        /// </summary>
        public Guid SkillId { get; set; }

        /// <summary>
        /// 技能名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 技能类型
        /// </summary>
        public string SkillType { get; set; }

        /// <summary>
        /// 技能效果类型
        /// </summary>
        public string EffectType { get; set; }

        /// <summary>
        /// 技能作用范围
        /// </summary>
        public int Range { get; set; }

        /// <summary>
        /// 技能影响区域
        /// </summary>
        public int AreaOfEffect { get; set; }

        /// <summary>
        /// 技能冷却时间
        /// </summary>
        public int Cooldown { get; set; }

        /// <summary>
        /// 当前冷却剩余回合
        /// </summary>
        public int CurrentCooldown { get; set; }

        /// <summary>
        /// 技能所需魔法值
        /// </summary>
        public int ManaCost { get; set; }

        /// <summary>
        /// 技能威力
        /// </summary>
        public int Power { get; set; }

        /// <summary>
        /// 技能命中率
        /// </summary>
        public int Accuracy { get; set; }

        /// <summary>
        /// 技能最适合的X坐标
        /// </summary>
        public int BestTargetX { get; set; }

        /// <summary>
        /// 技能最适合的Y坐标
        /// </summary>
        public int BestTargetY { get; set; }

        /// <summary>
        /// 预期技能效果值
        /// </summary>
        public int ExpectedEffect { get; set; }

        /// <summary>
        /// 影响的目标数量
        /// </summary>
        public int AffectedTargets { get; set; }

        /// <summary>
        /// 技能的战略价值
        /// </summary>
        public int StrategicValue { get; set; }
    }
}