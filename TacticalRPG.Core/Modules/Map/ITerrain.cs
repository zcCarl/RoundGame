namespace TacticalRPG.Core.Modules.Map
{
    /// <summary>
    /// 地形接口
    /// </summary>
    public interface ITerrain
    {
        /// <summary>
        /// 地形类型
        /// </summary>
        TerrainType Type { get; }

        /// <summary>
        /// 移动消耗
        /// </summary>
        int MovementCost { get; }

        /// <summary>
        /// 是否可通行
        /// </summary>
        bool IsPassable { get; }

        /// <summary>
        /// 防御加成
        /// </summary>
        int DefenseBonus { get; }

        /// <summary>
        /// 闪避加成
        /// </summary>
        int EvasionBonus { get; }
    }
}