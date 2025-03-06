namespace TacticalRPG.Core.Modules.Battle
{
    /// <summary>
    /// 战斗行动类型枚举
    /// </summary>
    public enum BattleActionType
    {
        /// <summary>
        /// 移动
        /// </summary>
        Move = 0,

        /// <summary>
        /// 普通攻击
        /// </summary>
        Attack = 1,

        /// <summary>
        /// 使用技能
        /// </summary>
        Skill = 2,

        /// <summary>
        /// 使用物品
        /// </summary>
        Item = 3,

        /// <summary>
        /// 防御
        /// </summary>
        Defend = 4,

        /// <summary>
        /// 等待
        /// </summary>
        Wait = 5,

        /// <summary>
        /// 逃跑
        /// </summary>
        Escape = 6
    }
}