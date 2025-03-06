namespace TacticalRPG.Core.Modules.Equipment
{
    /// <summary>
    /// 定义装备的稀有度/品质等级
    /// </summary>
    public enum EquipmentRarity
    {
        /// <summary>
        /// 普通品质（灰色）
        /// </summary>
        Common = 0,

        /// <summary>
        /// 优秀品质（绿色）
        /// </summary>
        Uncommon = 1,

        /// <summary>
        /// 精良品质（蓝色）
        /// </summary>
        Rare = 2,

        /// <summary>
        /// 史诗品质（紫色）
        /// </summary>
        Epic = 3,

        /// <summary>
        /// 传说品质（橙色）
        /// </summary>
        Legendary = 4,

        /// <summary>
        /// 神话品质（红色）
        /// </summary>
        Mythic = 5,

        /// <summary>
        /// 独特品质（黄色）- 特殊任务或成就奖励
        /// </summary>
        Unique = 6,

        /// <summary>
        /// 工艺品质（青色）- 玩家制作
        /// </summary>
        Crafted = 7,

        /// <summary>
        /// 遗物品质（金色）- 古代物品
        /// </summary>
        Relic = 8,

        /// <summary>
        /// 神器品质（彩虹色）- 游戏中最高级别物品
        /// </summary>
        Artifact = 9
    }
}