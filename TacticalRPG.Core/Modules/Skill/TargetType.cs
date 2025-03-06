namespace TacticalRPG.Core.Modules.Skill
{
    /// <summary>
    /// 技能目标类型枚举
    /// </summary>
    public enum TargetType
    {
        /// <summary>
        /// 单体敌人
        /// </summary>
        SingleEnemy,

        /// <summary>
        /// 单体友方
        /// </summary>
        SingleAlly,

        /// <summary>
        /// 自身
        /// </summary>
        Self,

        /// <summary>
        /// 全体敌人
        /// </summary>
        AllEnemies,

        /// <summary>
        /// 全体友方
        /// </summary>
        AllAllies,

        /// <summary>
        /// 区域敌人
        /// </summary>
        AreaEnemy,

        /// <summary>
        /// 区域友方
        /// </summary>
        AreaAlly,

        /// <summary>
        /// 区域混合
        /// </summary>
        AreaMixed,

        /// <summary>
        /// 直线
        /// </summary>
        Line,

        /// <summary>
        /// 空地
        /// </summary>
        EmptyTile
    }
}