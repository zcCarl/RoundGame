using System;
using System.IO;
using TacticalRPG.Core.Modules.Character;
using TacticalRPG.Core.Modules.Config;
using TacticalRPG.Core.Modules.Inventory;
using TacticalRPG.Implementation.Modules.Battle;
using TacticalRPG.Implementation.Modules.Inventory;

namespace TacticalRPG.Implementation.Modules.Config
{
    /// <summary>
    /// 配置初始化器，用于注册和初始化各模块配置
    /// </summary>
    public static class ConfigInitializer
    {
        /// <summary>
        /// 初始化所有配置
        /// </summary>
        /// <param name="configManager">配置管理器</param>
        /// <param name="configFolderPath">配置文件夹路径</param>
        /// <returns>是否初始化成功</returns>
        public static bool InitializeAllConfigs(IConfigManager configManager, string configFolderPath)
        {
            if (configManager == null)
                throw new ArgumentNullException(nameof(configManager));

            try
            {
                // 注册各模块配置
                RegisterInventoryConfig(configManager);
                RegisterBattleConfig(configManager);
                RegisterItemConfigs(configManager);

                // 尝试从文件夹加载配置
                if (!string.IsNullOrEmpty(configFolderPath) && Directory.Exists(configFolderPath))
                {
                    configManager.LoadAllConfigs(configFolderPath);
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"初始化配置时发生错误: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 注册库存配置
        /// </summary>
        /// <param name="configManager">配置管理器</param>
        private static void RegisterInventoryConfig(IConfigManager configManager)
        {
            var inventoryConfig = new InventoryConfig();
            configManager.RegisterConfig(inventoryConfig);
        }

        /// <summary>
        /// 注册战斗配置
        /// </summary>
        /// <param name="configManager">配置管理器</param>
        private static void RegisterBattleConfig(IConfigManager configManager)
        {
            var battleConfig = new BattleConfig();
            configManager.RegisterConfig(battleConfig);
        }

        /// <summary>
        /// 注册物品配置
        /// </summary>
        /// <param name="configManager">配置管理器</param>
        private static void RegisterItemConfigs(IConfigManager configManager)
        {
            // 这里可以预先注册一些常用的物品配置
            // 例如：基础消耗品、常见材料等

            // 创建药水物品配置
            var healthPotionSmall = new ItemConfig("health_potion_small", "小型生命药水");
            healthPotionSmall.SetDescription("恢复少量生命值");
            healthPotionSmall.SetType(ItemType.Consumable);
            healthPotionSmall.SetRarity(ItemRarity.Common);
            healthPotionSmall.SetIsStackable(true);
            healthPotionSmall.SetMaxStackSize(99);
            healthPotionSmall.SetWeight(0.1f);
            healthPotionSmall.SetValue(10);
            healthPotionSmall.SetIsUsable(true);
            healthPotionSmall.SetUseEffect("Health:20");
            configManager.RegisterConfig(healthPotionSmall);

            var healthPotionMedium = new ItemConfig("health_potion_medium", "中型生命药水");
            healthPotionMedium.SetDescription("恢复中量生命值");
            healthPotionMedium.SetType(ItemType.Consumable);
            healthPotionMedium.SetRarity(ItemRarity.Uncommon);
            healthPotionMedium.SetIsStackable(true);
            healthPotionMedium.SetMaxStackSize(50);
            healthPotionMedium.SetWeight(0.2f);
            healthPotionMedium.SetValue(30);
            healthPotionMedium.SetIsUsable(true);
            healthPotionMedium.SetUseEffect("Health:50");
            configManager.RegisterConfig(healthPotionMedium);

            var healthPotionLarge = new ItemConfig("health_potion_large", "大型生命药水");
            healthPotionLarge.SetDescription("恢复大量生命值");
            healthPotionLarge.SetType(ItemType.Consumable);
            healthPotionLarge.SetRarity(ItemRarity.Rare);
            healthPotionLarge.SetIsStackable(true);
            healthPotionLarge.SetMaxStackSize(25);
            healthPotionLarge.SetWeight(0.3f);
            healthPotionLarge.SetValue(80);
            healthPotionLarge.SetIsUsable(true);
            healthPotionLarge.SetUseEffect("Health:120");
            configManager.RegisterConfig(healthPotionLarge);

            // 创建材料物品配置
            var ironOre = new ItemConfig("iron_ore", "铁矿石");
            ironOre.SetDescription("常见的金属矿石，可用于锻造");
            ironOre.SetType(ItemType.Material);
            ironOre.SetRarity(ItemRarity.Common);
            ironOre.SetIsStackable(true);
            ironOre.SetMaxStackSize(999);
            ironOre.SetWeight(0.3f);
            ironOre.SetValue(2);
            ironOre.SetCustomProperty("MaterialType", "Metal");
            configManager.RegisterConfig(ironOre);

            var silverOre = new ItemConfig("silver_ore", "银矿石");
            silverOre.SetDescription("较为稀有的金属矿石，可用于精细锻造");
            silverOre.SetType(ItemType.Material);
            silverOre.SetRarity(ItemRarity.Uncommon);
            silverOre.SetIsStackable(true);
            silverOre.SetMaxStackSize(999);
            silverOre.SetWeight(0.3f);
            silverOre.SetValue(10);
            silverOre.SetCustomProperty("MaterialType", "Metal");
            configManager.RegisterConfig(silverOre);

            var goldOre = new ItemConfig("gold_ore", "金矿石");
            goldOre.SetDescription("稀有的金属矿石，可用于高级锻造");
            goldOre.SetType(ItemType.Material);
            goldOre.SetRarity(ItemRarity.Rare);
            goldOre.SetIsStackable(true);
            goldOre.SetMaxStackSize(999);
            goldOre.SetWeight(0.5f);
            goldOre.SetValue(50);
            goldOre.SetCustomProperty("MaterialType", "Metal");
            configManager.RegisterConfig(goldOre);

            // 创建一些武器配置
            var woodenSword = new ItemConfig("wooden_sword", "木剑");
            woodenSword.SetDescription("简单的木制剑，攻击力很低");
            woodenSword.SetType(ItemType.Equipment);
            woodenSword.SetRarity(ItemRarity.Common);
            woodenSword.SetIsStackable(false);
            woodenSword.SetWeight(1.0f);
            woodenSword.SetValue(5);
            woodenSword.SetLevel(1);
            woodenSword.SetDurability(50);
            woodenSword.SetMaxDurability(50);
            woodenSword.SetIsEquippable(true);
            woodenSword.SetEquipSlot(EquipmentSlot.MainHand);
            woodenSword.SetCustomProperty("BaseDamage", "3");
            woodenSword.SetCustomProperty("WeaponType", "Sword");
            configManager.RegisterConfig(woodenSword);

            var ironSword = new ItemConfig("iron_sword", "铁剑");
            ironSword.SetDescription("普通的铁剑，适合初学者使用");
            ironSword.SetType(ItemType.Equipment);
            ironSword.SetRarity(ItemRarity.Common);
            ironSword.SetIsStackable(false);
            ironSword.SetWeight(2.0f);
            ironSword.SetValue(50);
            ironSword.SetLevel(5);
            ironSword.SetLevelRequirement(3);
            ironSword.SetDurability(100);
            ironSword.SetMaxDurability(100);
            ironSword.SetIsEquippable(true);
            ironSword.SetEquipSlot(EquipmentSlot.MainHand);
            ironSword.SetCustomProperty("BaseDamage", "8");
            ironSword.SetCustomProperty("WeaponType", "Sword");
            configManager.RegisterConfig(ironSword);
        }

        /// <summary>
        /// 保存所有配置
        /// </summary>
        /// <param name="configManager">配置管理器</param>
        /// <param name="configFolderPath">配置文件夹路径</param>
        /// <returns>是否保存成功</returns>
        public static bool SaveAllConfigs(IConfigManager configManager, string configFolderPath)
        {
            if (configManager == null)
                throw new ArgumentNullException(nameof(configManager));

            if (string.IsNullOrEmpty(configFolderPath))
                throw new ArgumentException("配置文件夹路径不能为空", nameof(configFolderPath));

            try
            {
                // 确保目录存在
                if (!Directory.Exists(configFolderPath))
                {
                    Directory.CreateDirectory(configFolderPath);
                }

                return configManager.SaveAllConfigs(configFolderPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"保存配置时发生错误: {ex.Message}");
                return false;
            }
        }
    }
}