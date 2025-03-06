using System;

namespace TacticalRPG.Core.Modules.AI
{
    /// <summary>
    /// 物品信息类，用于AI战场评估
    /// </summary>
    public class ItemInfo
    {
        /// <summary>
        /// 物品ID
        /// </summary>
        public Guid ItemId { get; set; }

        /// <summary>
        /// 物品名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 物品类型
        /// </summary>
        public string ItemType { get; set; }

        /// <summary>
        /// 物品效果类型
        /// </summary>
        public string EffectType { get; set; }

        /// <summary>
        /// 物品作用范围
        /// </summary>
        public int Range { get; set; }

        /// <summary>
        /// 物品影响区域
        /// </summary>
        public int AreaOfEffect { get; set; }

        /// <summary>
        /// 物品数量
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// 物品威力
        /// </summary>
        public int Power { get; set; }

        /// <summary>
        /// 物品持续时间（回合）
        /// </summary>
        public int Duration { get; set; }

        /// <summary>
        /// 物品最佳使用目标ID
        /// </summary>
        public Guid BestTargetId { get; set; }

        /// <summary>
        /// 预期物品效果值
        /// </summary>
        public int ExpectedEffect { get; set; }

        /// <summary>
        /// 物品的战略价值
        /// </summary>
        public int StrategicValue { get; set; }

        /// <summary>
        /// 物品是否为一次性消耗品
        /// </summary>
        public bool IsConsumable { get; set; }

        /// <summary>
        /// 物品优先级
        /// </summary>
        public int Priority { get; set; }
    }
}