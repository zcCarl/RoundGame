using System;

namespace TacticalRPG.Core.Modules.Item
{
    /// <summary>
    /// 定义物品的稀有度级别
    /// </summary>
    public enum ItemRarity
    {
        /// <summary>
        /// 普通物品
        /// </summary>
        Common = 0,

        /// <summary>
        /// 非普通物品
        /// </summary>
        Uncommon = 1,

        /// <summary>
        /// 稀有物品
        /// </summary>
        Rare = 2,

        /// <summary>
        /// 史诗物品
        /// </summary>
        Epic = 3,

        /// <summary>
        /// 传说物品
        /// </summary>
        Legendary = 4,

        /// <summary>
        /// 神话物品
        /// </summary>
        Mythic = 5,

        /// <summary>
        /// 独特物品
        /// </summary>
        Unique = 6,

        /// <summary>
        /// 制作物品
        /// </summary>
        Crafted = 7,

        /// <summary>
        /// 遗物物品
        /// </summary>
        Relic = 8,

        /// <summary>
        /// 神器
        /// </summary>
        Artifact = 9,

        /// <summary>
        /// 任务物品
        /// </summary>
        Quest = 10
    }
}