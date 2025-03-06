using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using TacticalRPG.Core.Modules.Character;
using TacticalRPG.Core.Modules.Equipment;
using TacticalRPG.Core.Modules.Inventory;
using TacticalRPG.Core.Modules.Skill;
using TacticalRPG.Core.Modules.Config;

namespace TacticalRPG.Implementation.Modules.Inventory
{
    /// <summary>
    /// 物品工厂实现类
    /// </summary>
    public class ItemFactory : IItemFactory
    {
        private readonly ILogger<ItemFactory> _logger;
        private readonly Random _random = new Random();
        private readonly Dictionary<string, Func<IItem, IItem>> _itemFactories = new Dictionary<string, Func<IItem, IItem>>();
        private readonly IConfigManager _configManager;
        private readonly ItemConfigManager _itemConfigManager;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="logger">日志记录器</param>
        /// <param name="configManager">配置管理器</param>
        /// <param name="itemConfigManager">物品配置管理器</param>
        public ItemFactory(ILogger<ItemFactory> logger, IConfigManager configManager, ItemConfigManager itemConfigManager)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configManager = configManager ?? throw new ArgumentNullException(nameof(configManager));
            _itemConfigManager = itemConfigManager ?? throw new ArgumentNullException(nameof(itemConfigManager));
        }

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
        public IItem CreateItem(
            string name,
            string description,
            ItemType type,
            ItemRarity rarity,
            bool isStackable = false,
            int maxStackSize = 1,
            float weight = 0.0f,
            int value = 0,
            int stackSize = 1)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("物品名称不能为空", nameof(name));

            if (string.IsNullOrEmpty(description))
                throw new ArgumentException("物品描述不能为空", nameof(description));

            if (maxStackSize < 1)
                maxStackSize = 1;

            if (stackSize < 1)
                stackSize = 1;

            if (stackSize > maxStackSize)
                stackSize = maxStackSize;

            // 创建唯一的物品ID
            string itemId = $"{type.ToString().ToLower()}_{Guid.NewGuid():N}";

            // 创建物品配置
            var config = _itemConfigManager.CreateItemConfig(itemId, name);
            config.SetName(name);
            config.SetDescription(description);
            config.SetType(type);
            config.SetRarity(rarity);
            config.SetIconPath($"Icons/{type}/{rarity}/{name.Replace(" ", "")}.png");
            config.SetIsStackable(isStackable);
            config.SetMaxStackSize(maxStackSize);
            config.SetWeight(weight);
            config.SetValue(value);
            config.SetIsUsable(type == ItemType.Consumable);
            config.SetIsEquippable(type == ItemType.Equipment);
            config.SetIsSellable(true);
            config.SetIsDroppable(true);
            config.SetIsTradable(true);
            config.SetIsBound(false);
            config.SetBindType(BindType.None);

            // 使用配置创建物品
            var item = new Item(config, stackSize);

            _logger.LogDebug($"创建物品: {name} (ID: {item.Id})");
            return item;
        }

        /// <summary>
        /// 从装备创建物品
        /// </summary>
        /// <param name="equipment">装备实例</param>
        /// <returns>创建的物品实例</returns>
        public IItem CreateFromEquipment(IEquipment equipment)
        {
            if (equipment == null)
                throw new ArgumentNullException(nameof(equipment));

            // 创建唯一的物品ID
            string itemId = $"equipment_{Guid.NewGuid():N}";

            // 创建物品配置
            var config = _itemConfigManager.CreateItemConfig(itemId, equipment.Name);
            config.SetName(equipment.Name);
            config.SetDescription(equipment.Description);
            config.SetType(ItemType.Equipment);
            config.SetRarity((ItemRarity)equipment.Rarity);
            config.SetIsStackable(false);
            config.SetMaxStackSize(1);
            config.SetWeight(equipment.Weight);
            config.SetValue(equipment.Value);
            config.SetIsUsable(false);
            config.SetIsEquippable(true);
            config.SetIsSellable(true);
            config.SetIsDroppable(true);
            config.SetIsTradable(true);
            config.SetIsBound(false);
            config.SetBindType(BindType.None);
            config.SetLevel(equipment.Level);
            config.SetLevelRequirement(equipment.Level - 2);
            config.SetDurability((int)equipment.Durability);
            config.SetMaxDurability((int)equipment.MaxDurability);
            config.SetEquipSlot(EquipmentSlot.MainHand);

            // 使用配置创建物品
            var item = new Item(config);
            item.SetEquipmentInstance(equipment);

            _logger.LogDebug($"从装备创建物品: {item.Name} (ID: {item.Id})");
            return item;
        }

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
        public IItem CreateConsumable(
            string name,
            string description,
            ItemRarity rarity,
            EquipmentStatType effectType,
            float effectValue,
            int maxStackSize = 99,
            float weight = 0.1f,
            int value = 10,
            int stackSize = 1)
        {
            var item = CreateItem(
                name,
                description,
                ItemType.Consumable,
                rarity,
                true,
                maxStackSize,
                weight,
                value,
                stackSize);

            // 设置消耗品特有属性
            ((Item)item).SetUseEffect($"{effectType}:{effectValue}");
            ((Item)item).SetUsable(true);
            ((Item)item).SetAttribute("EffectType", effectType);
            ((Item)item).SetAttribute("EffectValue", effectValue);

            _logger.LogDebug($"创建消耗品: {item.Name} (ID: {item.Id})，效果: {effectType} {effectValue}");
            return item;
        }

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
        public IItem CreateMaterial(
            string name,
            string description,
            ItemRarity rarity,
            int maxStackSize = 999,
            float weight = 0.05f,
            int value = 5,
            int stackSize = 1,
            string materialType = "")
        {
            var item = CreateItem(
                name,
                description,
                ItemType.Material,
                rarity,
                true,
                maxStackSize,
                weight,
                value,
                stackSize);

            // 设置材料特有属性
            if (!string.IsNullOrEmpty(materialType))
                ((Item)item).SetAttribute("MaterialType", materialType);

            _logger.LogDebug($"创建材料: {item.Name} (ID: {item.Id})，类型: {materialType}");
            return item;
        }

        /// <summary>
        /// 创建任务物品
        /// </summary>
        /// <param name="name">物品名称</param>
        /// <param name="description">物品描述</param>
        /// <param name="questId">任务ID</param>
        /// <param name="isLocked">是否锁定</param>
        /// <returns>创建的任务物品实例</returns>
        public IItem CreateQuestItem(
            string name,
            string description,
            Guid questId,
            bool isLocked = true)
        {
            var item = CreateItem(
                name,
                description,
                ItemType.QuestItem,
                ItemRarity.Quest,
                false,
                1,
                0.1f,
                0,
                1);

            // 设置任务物品特有属性
            ((Item)item).SetAttribute("QuestId", questId);
            ((Item)item).SetDroppable(false);
            ((Item)item).SetSellable(false);
            ((Item)item).SetLocked(isLocked);

            _logger.LogDebug($"创建任务物品: {item.Name} (ID: {item.Id})，任务ID: {questId}");
            return item;
        }

        /// <summary>
        /// 创建货币物品
        /// </summary>
        /// <param name="name">物品名称</param>
        /// <param name="description">物品描述</param>
        /// <param name="value">货币价值</param>
        /// <param name="amount">数量</param>
        /// <returns>创建的货币物品实例</returns>
        public IItem CreateCurrency(
            string name,
            string description,
            int value = 1,
            int amount = 1)
        {
            var item = CreateItem(
                name,
                description,
                ItemType.Currency,
                ItemRarity.Common,
                true,
                9999,
                0.01f,
                value,
                amount);

            _logger.LogDebug($"创建货币: {item.Name} (ID: {item.Id})，价值: {value}，数量: {amount}");
            return item;
        }

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
        public IItem CreateContainer(
            string name,
            string description,
            ItemRarity rarity,
            int capacity = 10,
            float weight = 1.0f,
            int value = 50)
        {
            var item = CreateItem(
                name,
                description,
                ItemType.Container,
                rarity,
                false,
                1,
                weight,
                value,
                1);

            // 设置容器特有属性
            ((Item)item).SetAttribute("Capacity", capacity);
            ((Item)item).SetUsable(true);

            _logger.LogDebug($"创建容器: {item.Name} (ID: {item.Id})，容量: {capacity}");
            return item;
        }

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
        public IItem CreateSkillBook(
            string name,
            string description,
            ItemRarity rarity,
            Guid skillId,
            float weight = 0.5f,
            int value = 100)
        {
            var item = CreateItem(
                name,
                description,
                ItemType.SkillBook,
                rarity,
                false,
                1,
                weight,
                value,
                1);

            // 设置技能书特有属性
            ((Item)item).SetAttribute("SkillId", skillId);
            ((Item)item).SetUsable(true);

            _logger.LogDebug($"创建技能书: {item.Name} (ID: {item.Id})，技能ID: {skillId}");
            return item;
        }

        /// <summary>
        /// 根据模板创建物品
        /// </summary>
        /// <param name="templateId">模板ID</param>
        /// <param name="stackSize">堆叠数量</param>
        /// <returns>创建的物品实例</returns>
        public IItem CreateFromTemplate(string templateId, int stackSize = 1)
        {
            if (string.IsNullOrEmpty(templateId))
                throw new ArgumentException("模板ID不能为空", nameof(templateId));

            var config = _itemConfigManager.GetItemConfig(templateId);
            if (config == null)
                throw new KeyNotFoundException($"找不到物品配置: {templateId}");

            var item = new Item(config, stackSize);

            _logger.LogDebug($"从模板创建物品: {item.Name} (ID: {item.Id})，模板ID: {templateId}，堆叠数量: {item.StackSize}");
            return item;
        }

        /// <summary>
        /// 创建随机物品
        /// </summary>
        /// <param name="type">物品类型</param>
        /// <param name="minRarity">最低稀有度</param>
        /// <param name="maxRarity">最高稀有度</param>
        /// <param name="level">物品等级（如适用）</param>
        /// <returns>创建的随机物品实例</returns>
        public IItem CreateRandomItem(
            ItemType type,
            ItemRarity minRarity = ItemRarity.Common,
            ItemRarity maxRarity = ItemRarity.Rare,
            int level = 1)
        {
            // 随机选择稀有度
            int minRarityValue = (int)minRarity;
            int maxRarityValue = (int)maxRarity;
            int rarityValue = _random.Next(minRarityValue, maxRarityValue + 1);
            var rarity = (ItemRarity)rarityValue;

            // 根据物品类型创建随机物品
            IItem item;
            switch (type)
            {
                case ItemType.Equipment:
                    string[] equipmentNames = { "剑", "盔甲", "盾牌", "头盔", "护手", "靴子", "项链", "戒指" };
                    string equipName = $"随机{equipmentNames[_random.Next(equipmentNames.Length)]}";
                    string equipDesc = $"一件随机生成的{equipName}，等级 {level}";

                    item = CreateItem(
                        equipName,
                        equipDesc,
                        ItemType.Equipment,
                        rarity,
                        false,
                        1,
                        0.5f + (float)_random.NextDouble(),
                        50 * level * (rarityValue + 1),
                        1);

                    ((Item)item).SetLevel(level);
                    ((Item)item).SetLevelRequirement(Math.Max(1, level - 2));
                    ((Item)item).SetDurability(100);
                    ((Item)item).SetMaxDurability(100);
                    break;

                case ItemType.Consumable:
                    string[] consumableNames = { "生命药水", "魔法药水", "治疗药剂", "抗毒剂", "耐力药剂", "防御药剂" };
                    string consName = consumableNames[_random.Next(consumableNames.Length)];
                    string consDesc = $"一瓶随机生成的{consName}，效果随稀有度增强";

                    item = CreateItem(
                        consName,
                        consDesc,
                        ItemType.Consumable,
                        rarity,
                        true,
                        99,
                        0.1f,
                        10 * (rarityValue + 1) * level,
                        _random.Next(1, 5));

                    ((Item)item).SetUsable(true);
                    ((Item)item).SetUseEffect($"HP:{20 * (rarityValue + 1)}");
                    break;

                case ItemType.Material:
                    string[] materialNames = { "铁矿石", "银矿石", "金矿石", "魔法结晶", "草药", "木材", "皮革", "布料" };
                    string matName = materialNames[_random.Next(materialNames.Length)];
                    string matDesc = $"一份随机生成的{matName}，用于制作物品";

                    item = CreateItem(
                        matName,
                        matDesc,
                        ItemType.Material,
                        rarity,
                        true,
                        999,
                        0.05f,
                        5 * (rarityValue + 1),
                        _random.Next(1, 10));
                    break;

                default:
                    // 默认创建一个通用物品
                    item = CreateItem(
                        "随机物品",
                        "一件随机生成的物品",
                        type,
                        rarity,
                        false,
                        1,
                        0.5f,
                        50 * (rarityValue + 1),
                        1);
                    break;
            }

            _logger.LogDebug($"创建随机物品: {item.Name} (ID: {item.Id})，类型: {type}，稀有度: {rarity}");
            return item;
        }

        /// <summary>
        /// 根据配置创建物品
        /// </summary>
        /// <param name="itemId">物品ID</param>
        /// <param name="stackSize">堆叠数量</param>
        /// <returns>创建的物品实例</returns>
        public IItem CreateFromConfig(string itemId, int stackSize = 1)
        {
            if (string.IsNullOrEmpty(itemId))
                throw new ArgumentException("物品ID不能为空", nameof(itemId));

            var config = _itemConfigManager.GetItemConfig(itemId);
            if (config == null)
                throw new KeyNotFoundException($"找不到物品配置: {itemId}");

            var item = new Item(config, stackSize);

            _logger.LogDebug($"从配置创建物品: {item.Name} (ID: {item.Id})，物品ID: {itemId}，堆叠数量: {item.StackSize}");
            return item;
        }

        /// <summary>
        /// 创建物品
        /// </summary>
        /// <param name="itemId">物品ID</param>
        /// <param name="amount">数量</param>
        /// <returns>创建的物品</returns>
        public IItem CreateItem(string itemId, int amount = 1)
        {
            if (string.IsNullOrEmpty(itemId) || amount <= 0)
                return null;

            // 从配置中获取物品配置
            var config = _itemConfigManager.GetItemConfig(itemId);
            if (config == null)
                return null;

            // 从配置中获取最大堆叠大小
            var inventoryConfig = _configManager.GetConfig<InventoryConfig>(InventoryConfig.MODULE_ID);
            int configMaxStackSize = inventoryConfig?.MaxStackSize ?? 99;

            // 使用配置创建物品
            var item = new Item(config, Math.Min(amount, config.IsStackable ? (config.MaxStackSize > 0 ? config.MaxStackSize : configMaxStackSize) : 1));

            _logger.LogDebug($"创建物品: {item.Name} (ID: {item.Id}, 物品ID: {itemId})");
            return item;
        }
    }
}