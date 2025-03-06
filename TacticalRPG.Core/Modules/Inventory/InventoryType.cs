namespace TacticalRPG.Core.Modules.Inventory
{
    /// <summary>
    /// 库存类型枚举
    /// </summary>
    public enum InventoryType
    {
        /// <summary>
        /// 普通库存
        /// </summary>
        Normal,

        /// <summary>
        /// 装备库存
        /// </summary>
        Equipment,

        /// <summary>
        /// 任务物品库存
        /// </summary>
        Quest,

        /// <summary>
        /// 商店库存
        /// </summary>
        Shop,

        /// <summary>
        /// 银行库存
        /// </summary>
        Bank,

        /// <summary>
        /// 制作材料库存
        /// </summary>
        Crafting,

        /// <summary>
        /// 宝箱库存
        /// </summary>
        Chest,

        /// <summary>
        /// 掉落库存
        /// </summary>
        Loot
    }
}