namespace TacticalRPG.Core.Modules.AI
{
    /// <summary>
    /// AI类型枚举
    /// </summary>
    public enum AIType
    {
        /// <summary>
        /// 无AI
        /// </summary>
        None = 0,

        /// <summary>
        /// 进攻型AI
        /// </summary>
        Aggressive = 1,

        /// <summary>
        /// 防守型AI
        /// </summary>
        Defensive = 2,

        /// <summary>
        /// 平衡型AI
        /// </summary>
        Balanced = 3,

        /// <summary>
        /// 狂战士型AI
        /// </summary>
        Berserker = 4,

        /// <summary>
        /// 治疗型AI
        /// </summary>
        Healer = 5,

        /// <summary>
        /// 支援型AI
        /// </summary>
        Support = 6,

        /// <summary>
        /// 战术型AI
        /// </summary>
        Tactical = 7,

        /// <summary>
        /// 被动型AI
        /// </summary>
        Passive = 8,

        /// <summary>
        /// 自定义AI
        /// </summary>
        Custom = 99
    }
}