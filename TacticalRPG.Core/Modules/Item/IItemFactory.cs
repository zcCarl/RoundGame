

namespace TacticalRPG.Core.Modules.Item
{
    /// <summary>
    /// 定义物品工厂的接口，用于创建各种类型的物品
    /// </summary>
    public interface IItemFactory
    {
        /// <summary>
        /// 创建基础物品
        /// </summary>
        /// <param name="name">物品名称</param>
        /// <param name="description">物品描述</param>
        /// <param name="type">物品类型</param>
        /// <param name="rarity">物品稀有度</param>
        /// <param name="isStackable">是否可堆叠</param>
        /// <param name="maxStackSize">最大堆叠数量</param>
        /// <param name="weight">物品重量</param>
        /// <param name="value">物品价值</param>
        /// <param name="stackSize">初始堆叠数量</param>
        /// <returns>创建的物品实例</returns>
        IItem CreateItem(
            string name,
            string description,
            ItemType type,
            ItemRarity rarity,
            bool isStackable = false,
            int maxStackSize = 1,
            float weight = 0.0f,
            int value = 0,
            int stackSize = 1);

        /// <summary>
        /// 创建消耗品物品
        /// </summary>
        /// <param name="name">物品名称</param>
        /// <param name="description">物品描述</param>
        /// <param name="rarity">物品稀有度</param>
        /// <param name="effectType">效果类型</param>
        /// <param name="effectValue">效果值</param>
        /// <param name="maxStackSize">最大堆叠数量</param>
        /// <param name="weight">物品重量</param>
        /// <param name="value">物品价值</param>
        /// <param name="stackSize">初始堆叠数量</param>
        /// <returns>创建的消耗品物品实例</returns>
        IItem CreateConsumable(
            string name,
            string description,
            ItemRarity rarity,
            string effectType,
            float effectValue,
            int maxStackSize = 99,
            float weight = 0.1f,
            int value = 10,
            int stackSize = 1);

        /// <summary>
        /// 创建材料物品
        /// </summary>
        /// <param name="name">物品名称</param>
        /// <param name="description">物品描述</param>
        /// <param name="rarity">物品稀有度</param>
        /// <param name="maxStackSize">最大堆叠数量</param>
        /// <param name="weight">物品重量</param>
        /// <param name="value">物品价值</param>
        /// <param name="stackSize">初始堆叠数量</param>
        /// <param name="materialType">材料类型</param>
        /// <returns>创建的材料物品实例</returns>
        IItem CreateMaterial(
            string name,
            string description,
            ItemRarity rarity,
            int maxStackSize = 999,
            float weight = 0.05f,
            int value = 5,
            int stackSize = 1,
            string materialType = "");

        /// <summary>
        /// 创建任务物品
        /// </summary>
        /// <param name="name">物品名称</param>
        /// <param name="description">物品描述</param>
        /// <param name="questId">任务ID</param>
        /// <param name="isLocked">是否锁定</param>
        /// <returns>创建的任务物品实例</returns>
        IItem CreateQuestItem(
            string name,
            string description,
            Guid questId,
            bool isLocked = true);

        /// <summary>
        /// 创建货币物品
        /// </summary>
        /// <param name="name">物品名称</param>
        /// <param name="description">物品描述</param>
        /// <param name="value">货币价值</param>
        /// <param name="amount">数量</param>
        /// <returns>创建的货币物品实例</returns>
        IItem CreateCurrency(
            string name,
            string description,
            int value = 1,
            int amount = 1);

        /// <summary>
        /// 创建容器物品
        /// </summary>
        /// <param name="name">物品名称</param>
        /// <param name="description">物品描述</param>
        /// <param name="rarity">物品稀有度</param>
        /// <param name="capacity">容器容量</param>
        /// <param name="weight">物品重量</param>
        /// <param name="value">物品价值</param>
        /// <returns>创建的容器物品实例</returns>
        IItem CreateContainer(
            string name,
            string description,
            ItemRarity rarity,
            int capacity = 10,
            float weight = 1.0f,
            int value = 50);

        /// <summary>
        /// 创建技能书物品
        /// </summary>
        /// <param name="name">物品名称</param>
        /// <param name="description">物品描述</param>
        /// <param name="rarity">物品稀有度</param>
        /// <param name="skillId">技能ID</param>
        /// <param name="weight">物品重量</param>
        /// <param name="value">物品价值</param>
        /// <returns>创建的技能书物品实例</returns>
        IItem CreateSkillBook(
            string name,
            string description,
            ItemRarity rarity,
            Guid skillId,
            float weight = 0.5f,
            int value = 100);

        /// <summary>
        /// 根据模板创建物品
        /// </summary>
        /// <param name="templateId">模板ID</param>
        /// <param name="stackSize">堆叠数量</param>
        /// <returns>创建的物品实例</returns>
        IItem CreateFromTemplate(string templateId, int stackSize = 1);

        /// <summary>
        /// 创建随机物品
        /// </summary>
        /// <param name="type">物品类型</param>
        /// <param name="minRarity">最低稀有度</param>
        /// <param name="maxRarity">最高稀有度</param>
        /// <param name="level">物品等级（如适用）</param>
        /// <returns>创建的随机物品实例</returns>
        IItem CreateRandomItem(
            ItemType type,
            ItemRarity minRarity = ItemRarity.Common,
            ItemRarity maxRarity = ItemRarity.Rare,
            int level = 1);
    }
}