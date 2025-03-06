namespace TacticalRPG.Core.Modules.AI
{
    /// <summary>
    /// AI行动类型枚举
    /// </summary>
    public enum AIActionType
    {
        /// <summary>
        /// 无行动
        /// </summary>
        None = 0,

        /// <summary>
        /// 移动
        /// </summary>
        Move = 1,

        /// <summary>
        /// 攻击
        /// </summary>
        Attack = 2,

        /// <summary>
        /// 使用技能
        /// </summary>
        UseSkill = 3,

        /// <summary>
        /// 使用物品
        /// </summary>
        UseItem = 4,

        /// <summary>
        /// 等待
        /// </summary>
        Wait = 5,

        /// <summary>
        /// 结束回合
        /// </summary>
        EndTurn = 6
    }
}