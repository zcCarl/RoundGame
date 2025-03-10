namespace TacticalRPG.Core.Modules.Item
{
    /// <summary>
    /// 物品容器类型
    /// </summary>
    public enum ItemContainerType
    {
        /// <summary>
        /// 无容器/未分配
        /// </summary>
        None = 0,

        /// <summary>
        /// 角色背包
        /// </summary>
        Inventory = 1,

        /// <summary>
        /// 角色装备栏
        /// </summary>
        Equipment = 2,

        /// <summary>
        /// 角色快捷栏
        /// </summary>
        Shortcut = 3,

        /// <summary>
        /// 商店
        /// </summary>
        Shop = 4,

        /// <summary>
        /// 宝箱/容器
        /// </summary>
        Container = 5,

        /// <summary>
        /// 地面
        /// </summary>
        Ground = 6,

        /// <summary>
        /// 邮件附件
        /// </summary>
        Mail = 7,

        /// <summary>
        /// 银行
        /// </summary>
        Bank = 8,

        /// <summary>
        /// 公会仓库
        /// </summary>
        GuildStorage = 9,

        /// <summary>
        /// 交易界面
        /// </summary>
        Trade = 10,

        /// <summary>
        /// 制作界面
        /// </summary>
        Crafting = 11,

        /// <summary>
        /// 拍卖行
        /// </summary>
        Auction = 12
    }
}