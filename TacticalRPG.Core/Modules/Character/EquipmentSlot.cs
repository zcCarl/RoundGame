namespace TacticalRPG.Core.Modules.Character
{
    /// <summary>
    /// 装备槽位枚举
    /// </summary>
    public enum EquipmentSlot
    {
        None = -1,
        /// <summary>
        /// 主手武器
        /// </summary>
        MainHand = 0,

        /// <summary>
        /// 副手武器/盾牌
        /// </summary>
        OffHand = 1,

        /// <summary>
        /// 头部
        /// </summary>
        Head = 2,

        /// <summary>
        /// 身体
        /// </summary>
        Body = 3,

        /// <summary>
        /// 手部
        /// </summary>
        Hands = 4,

        /// <summary>
        /// 脚部
        /// </summary>
        Feet = 5,

        /// <summary>
        /// 饰品1
        /// </summary>
        Accessory1 = 6,

        /// <summary>
        /// 饰品2
        /// </summary>
        Accessory2 = 7
    }
}