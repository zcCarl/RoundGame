using System;

namespace TacticalRPG.Core.Modules.Inventory
{
    /// <summary>
    /// 定义游戏中不同类型的物品
    /// </summary>
    [Flags]
    public enum ItemType
    {
        /// <summary>
        /// 未定义的物品类型
        /// </summary>
        None = 0,

        /// <summary>
        /// 装备类物品
        /// </summary>
        Equipment = 1 << 0,

        /// <summary>
        /// 消耗品
        /// </summary>
        Consumable = 1 << 1,

        /// <summary>
        /// 材料
        /// </summary>
        Material = 1 << 2,

        /// <summary>
        /// 任务物品
        /// </summary>
        Quest = 1 << 3,

        /// <summary>
        /// 任务物品别名，与Quest相同
        /// </summary>
        QuestItem = Quest,

        /// <summary>
        /// 宝石
        /// </summary>
        Gem = 1 << 4,

        /// <summary>
        /// 货币
        /// </summary>
        Currency = 1 << 5,

        /// <summary>
        /// 图纸/配方
        /// </summary>
        Blueprint = 1 << 6,

        /// <summary>
        /// 书籍
        /// </summary>
        Book = 1 << 7,

        /// <summary>
        /// 技能书，书籍子类
        /// </summary>
        SkillBook = Book | (1 << 17),

        /// <summary>
        /// 箱子/容器
        /// </summary>
        Container = 1 << 8,

        /// <summary>
        /// 钥匙
        /// </summary>
        Key = 1 << 9,

        /// <summary>
        /// 药剂（消耗品子类）
        /// </summary>
        Potion = Consumable | (1 << 10),

        /// <summary>
        /// 食物（消耗品子类）
        /// </summary>
        Food = Consumable | (1 << 11),

        /// <summary>
        /// 卷轴（消耗品子类）
        /// </summary>
        Scroll = Consumable | (1 << 12),

        /// <summary>
        /// 珍贵物品/收藏品
        /// </summary>
        Valuable = 1 << 13,

        /// <summary>
        /// 装饰品
        /// </summary>
        Decoration = 1 << 14,

        /// <summary>
        /// 弹药
        /// </summary>
        Ammunition = 1 << 15,

        /// <summary>
        /// 工具
        /// </summary>
        Tool = 1 << 16,

        /// <summary>
        /// 所有物品类型
        /// </summary>
        All = (1 << 18) - 1  // 更新以包含新添加的类型
    }
}