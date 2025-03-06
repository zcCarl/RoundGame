namespace TacticalRPG.Core.Modules.Battle
{
    /// <summary>
    /// 战斗状态枚举
    /// </summary>
    public enum BattleState
    {
        /// <summary>
        /// 未开始
        /// </summary>
        NotStarted = 0,

        /// <summary>
        /// 准备中
        /// </summary>
        Preparing = 1,

        /// <summary>
        /// 进行中
        /// </summary>
        InProgress = 2,

        /// <summary>
        /// 暂停
        /// </summary>
        Paused = 3,

        /// <summary>
        /// 玩家胜利
        /// </summary>
        PlayerVictory = 4,

        /// <summary>
        /// 玩家失败
        /// </summary>
        PlayerDefeat = 5,

        /// <summary>
        /// 平局
        /// </summary>
        Draw = 6,

        /// <summary>
        /// 已结束
        /// </summary>
        Ended = 7
    }
}