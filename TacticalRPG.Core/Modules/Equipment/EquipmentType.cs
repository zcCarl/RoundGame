using System;

namespace TacticalRPG.Core.Modules.Equipment
{
    /// <summary>
    /// 定义游戏中不同类型的装备
    /// </summary>
    [Flags]
    public enum EquipmentType
    {
        /// <summary>
        /// 未定义的装备类型
        /// </summary>
        None = 0,

        /// <summary>
        /// 武器装备
        /// </summary>
        Weapon = 1 << 0,

        /// <summary>
        /// 头部装备
        /// </summary>
        Headgear = 1 << 1,

        /// <summary>
        /// 身体装备
        /// </summary>
        BodyArmor = 1 << 2,

        /// <summary>
        /// 手部装备
        /// </summary>
        Gloves = 1 << 3,

        /// <summary>
        /// 腿部装备
        /// </summary>
        Leggings = 1 << 4,

        /// <summary>
        /// 足部装备
        /// </summary>
        Footwear = 1 << 5,

        /// <summary>
        /// 饰品装备
        /// </summary>
        Accessory = 1 << 6,

        /// <summary>
        /// 盾牌装备
        /// </summary>
        Shield = 1 << 7,

        /// <summary>
        /// 背包装备
        /// </summary>
        Backpack = 1 << 8,

        /// <summary>
        /// 消耗品
        /// </summary>
        Consumable = 1 << 9,

        /// <summary>
        /// 副手装备
        /// </summary>
        OffHand = 1 << 10,

        /// <summary>
        /// 主手武器装备（细分武器类型）
        /// </summary>
        MainHandWeapon = Weapon | (1 << 11),

        /// <summary>
        /// 双手武器装备（细分武器类型）
        /// </summary>
        TwoHandedWeapon = Weapon | (1 << 12),

        /// <summary>
        /// 远程武器装备（细分武器类型）
        /// </summary>
        RangedWeapon = Weapon | (1 << 13),

        /// <summary>
        /// 魔法装备
        /// </summary>
        MagicalItem = 1 << 14,

        /// <summary>
        /// 护身符装备
        /// </summary>
        Talisman = Accessory | (1 << 15),

        /// <summary>
        /// 戒指装备
        /// </summary>
        Ring = Accessory | (1 << 16),

        /// <summary>
        /// 项链装备
        /// </summary>
        Necklace = Accessory | (1 << 17),

        /// <summary>
        /// 所有装备类型
        /// </summary>
        All = (1 << 18) - 1
    }
}