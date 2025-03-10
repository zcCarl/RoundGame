using System;
using System.Collections.Generic;
using TacticalRPG.Core.Modules.Character;
using TacticalRPG.Core.Modules.Config;
using TacticalRPG.Core.Modules.Equipment;
using TacticalRPG.Core.Modules.Inventory;
using TacticalRPG.Core.Modules.Item;
using TacticalRPG.Implementation.Modules.Config;

namespace TacticalRPG.Implementation.Modules.Item
{
    /// <summary>
    /// 物品配置类，用于存储物品的基本配置信息
    /// </summary>
    public class ItemConfig : ConfigBase
    {
        // 配置键名
        public const string KEY_ID = "Id";
        private const string KEY_NAME = "Name";
        private const string KEY_DESCRIPTION = "Description";
        private const string KEY_TYPE = "Type";
        private const string KEY_RARITY = "Rarity";
        private const string KEY_ICON_PATH = "IconPath";
        private const string KEY_IS_STACKABLE = "IsStackable";
        private const string KEY_MAX_STACK_SIZE = "MaxStackSize";
        private const string KEY_WEIGHT = "Weight";
        private const string KEY_VALUE = "Value";
        private const string KEY_LEVEL = "Level";
        private const string KEY_LEVEL_REQUIREMENT = "LevelRequirement";
        private const string KEY_DURABILITY = "Durability";
        private const string KEY_MAX_DURABILITY = "MaxDurability";
        private const string KEY_IS_USABLE = "IsUsable";
        private const string KEY_IS_EQUIPPABLE = "IsEquippable";
        private const string KEY_IS_SELLABLE = "IsSellable";
        private const string KEY_IS_DROPPABLE = "IsDroppable";
        private const string KEY_IS_TRADABLE = "IsTradable";
        private const string KEY_IS_BOUND = "IsBound";
        private const string KEY_BIND_TYPE = "BindType";
        private const string KEY_COOLDOWN = "Cooldown";
        private const string KEY_COOLDOWN_GROUP = "CooldownGroup";
        private const string KEY_USE_EFFECT = "UseEffect";
        private const string KEY_EQUIP_SLOT = "EquipSlot";
        private const string KEY_EQUIPMENT_STATS = "EquipmentStats";
        private const string KEY_USE_REQUIREMENTS = "UseRequirements";
        private const string KEY_ITEM_EFFECTS = "ItemEffects";

        /// <summary>
        /// 物品配置模块ID前缀
        /// </summary>
        public const string MODULE_ID_PREFIX = "item";

        /// <summary>
        /// 获取物品配置ID
        /// </summary>
        /// <param name="itemId">物品ID</param>
        /// <returns>配置模块ID</returns>
        public static string GetModuleId(string itemId) => $"{MODULE_ID_PREFIX}.{itemId}";

        /// <summary>
        /// 获取物品模板ID
        /// </summary>
        /// <returns>模板ID</returns>
        public string TemplateId => GetValue<string>(KEY_ID, "");

        /// <summary>
        /// 获取物品名称
        /// </summary>
        public string Name => GetValue<string>(KEY_NAME, "未命名物品");
        public void SetTemplateId(string value) => SetValue(KEY_ID, value);

        /// <summary>
        /// 设置物品名称
        /// </summary>
        /// <param name="value">名称</param>
        public void SetName(string value) => SetValue(KEY_NAME, value);

        /// <summary>
        /// 获取物品描述
        /// </summary>
        public string Description => GetValue<string>(KEY_DESCRIPTION, "");

        /// <summary>
        /// 设置物品描述
        /// </summary>
        /// <param name="value">描述</param>
        public void SetDescription(string value) => SetValue(KEY_DESCRIPTION, value);

        /// <summary>
        /// 获取物品类型
        /// </summary>
        public ItemType Type => GetValue<ItemType>(KEY_TYPE, ItemType.None);

        /// <summary>
        /// 设置物品类型
        /// </summary>
        /// <param name="value">类型</param>
        public void SetType(ItemType value) => SetValue(KEY_TYPE, value);

        /// <summary>
        /// 获取物品稀有度
        /// </summary>
        public ItemRarity Rarity => GetValue<ItemRarity>(KEY_RARITY, ItemRarity.Common);

        /// <summary>
        /// 设置物品稀有度
        /// </summary>
        /// <param name="value">稀有度</param>
        public void SetRarity(ItemRarity value) => SetValue(KEY_RARITY, value);

        /// <summary>
        /// 获取物品图标路径
        /// </summary>
        public string IconPath => GetValue<string>(KEY_ICON_PATH, "");

        /// <summary>
        /// 设置物品图标路径
        /// </summary>
        /// <param name="value">图标路径</param>
        public void SetIconPath(string value) => SetValue(KEY_ICON_PATH, value);

        /// <summary>
        /// 获取物品是否可堆叠
        /// </summary>
        public bool IsStackable => GetValue<bool>(KEY_IS_STACKABLE, false);

        /// <summary>
        /// 设置物品是否可堆叠
        /// </summary>
        /// <param name="value">是否可堆叠</param>
        public void SetIsStackable(bool value) => SetValue(KEY_IS_STACKABLE, value);

        /// <summary>
        /// 获取物品最大堆叠数量
        /// </summary>
        public int MaxStackSize => GetValue<int>(KEY_MAX_STACK_SIZE, 1);

        /// <summary>
        /// 设置物品最大堆叠数量
        /// </summary>
        /// <param name="value">最大堆叠数量</param>
        public void SetMaxStackSize(int value) => SetValue(KEY_MAX_STACK_SIZE, Math.Max(1, value));

        /// <summary>
        /// 获取物品重量
        /// </summary>
        public float Weight => GetValue<float>(KEY_WEIGHT, 0.1f);

        /// <summary>
        /// 设置物品重量
        /// </summary>
        /// <param name="value">重量</param>
        public void SetWeight(float value) => SetValue(KEY_WEIGHT, Math.Max(0, value));

        /// <summary>
        /// 获取物品价值
        /// </summary>
        public int Value => GetValue<int>(KEY_VALUE, 0);

        /// <summary>
        /// 设置物品价值
        /// </summary>
        /// <param name="value">价值</param>
        public void SetValue(int value) => SetValue(KEY_VALUE, Math.Max(0, value));

        /// <summary>
        /// 获取物品等级
        /// </summary>
        public int Level => GetValue<int>(KEY_LEVEL, 1);

        /// <summary>
        /// 设置物品等级
        /// </summary>
        /// <param name="value">等级</param>
        public void SetLevel(int value) => SetValue(KEY_LEVEL, Math.Max(1, value));

        /// <summary>
        /// 获取物品使用等级要求
        /// </summary>
        public int LevelRequirement => GetValue<int>(KEY_LEVEL_REQUIREMENT, 1);

        /// <summary>
        /// 设置物品使用等级要求
        /// </summary>
        /// <param name="value">等级要求</param>
        public void SetLevelRequirement(int value) => SetValue(KEY_LEVEL_REQUIREMENT, Math.Max(1, value));

        /// <summary>
        /// 获取物品耐久度
        /// </summary>
        public int Durability => GetValue<int>(KEY_DURABILITY, 100);

        /// <summary>
        /// 设置物品耐久度
        /// </summary>
        /// <param name="value">耐久度</param>
        public void SetDurability(int value) => SetValue(KEY_DURABILITY, Math.Max(0, value));

        /// <summary>
        /// 获取物品最大耐久度
        /// </summary>
        public int MaxDurability => GetValue<int>(KEY_MAX_DURABILITY, 100);

        /// <summary>
        /// 设置物品最大耐久度
        /// </summary>
        /// <param name="value">最大耐久度</param>
        public void SetMaxDurability(int value) => SetValue(KEY_MAX_DURABILITY, Math.Max(1, value));

        /// <summary>
        /// 获取物品是否可用
        /// </summary>
        public bool IsUsable => GetValue<bool>(KEY_IS_USABLE, false);

        /// <summary>
        /// 设置物品是否可用
        /// </summary>
        /// <param name="value">是否可用</param>
        public void SetIsUsable(bool value) => SetValue(KEY_IS_USABLE, value);

        /// <summary>
        /// 获取物品是否可装备
        /// </summary>
        public bool IsEquippable => GetValue<bool>(KEY_IS_EQUIPPABLE, false);

        /// <summary>
        /// 设置物品是否可装备
        /// </summary>
        /// <param name="value">是否可装备</param>
        public void SetIsEquippable(bool value) => SetValue(KEY_IS_EQUIPPABLE, value);

        /// <summary>
        /// 获取物品是否可出售
        /// </summary>
        public bool IsSellable => GetValue<bool>(KEY_IS_SELLABLE, true);

        /// <summary>
        /// 设置物品是否可出售
        /// </summary>
        /// <param name="value">是否可出售</param>
        public void SetIsSellable(bool value) => SetValue(KEY_IS_SELLABLE, value);

        /// <summary>
        /// 获取物品是否可丢弃
        /// </summary>
        public bool IsDroppable => GetValue<bool>(KEY_IS_DROPPABLE, true);

        /// <summary>
        /// 设置物品是否可丢弃
        /// </summary>
        /// <param name="value">是否可丢弃</param>
        public void SetIsDroppable(bool value) => SetValue(KEY_IS_DROPPABLE, value);

        /// <summary>
        /// 获取物品是否可交易
        /// </summary>
        public bool IsTradable => GetValue<bool>(KEY_IS_TRADABLE, true);

        /// <summary>
        /// 设置物品是否可交易
        /// </summary>
        /// <param name="value">是否可交易</param>
        public void SetIsTradable(bool value) => SetValue(KEY_IS_TRADABLE, value);

        /// <summary>
        /// 获取物品是否绑定
        /// </summary>
        public bool IsBound => GetValue<bool>(KEY_IS_BOUND, false);

        /// <summary>
        /// 设置物品是否绑定
        /// </summary>
        /// <param name="value">是否绑定</param>
        public void SetIsBound(bool value) => SetValue(KEY_IS_BOUND, value);

        /// <summary>
        /// 获取物品绑定类型
        /// </summary>
        public BindType BindType => GetValue<BindType>(KEY_BIND_TYPE, BindType.None);

        /// <summary>
        /// 设置物品绑定类型
        /// </summary>
        /// <param name="value">绑定类型</param>
        public void SetBindType(BindType value) => SetValue(KEY_BIND_TYPE, value);

        /// <summary>
        /// 获取物品冷却时间
        /// </summary>
        public float Cooldown => GetValue<float>(KEY_COOLDOWN, 0f);

        /// <summary>
        /// 设置物品冷却时间
        /// </summary>
        /// <param name="value">冷却时间</param>
        public void SetCooldown(float value) => SetValue(KEY_COOLDOWN, Math.Max(0, value));

        /// <summary>
        /// 获取物品冷却组
        /// </summary>
        public string CooldownGroup => GetValue<string>(KEY_COOLDOWN_GROUP, "");

        /// <summary>
        /// 设置物品冷却组
        /// </summary>
        /// <param name="value">冷却组</param>
        public void SetCooldownGroup(string value) => SetValue(KEY_COOLDOWN_GROUP, value);

        /// <summary>
        /// 获取物品使用效果
        /// </summary>
        public string UseEffect => GetValue<string>(KEY_USE_EFFECT, "");

        /// <summary>
        /// 设置物品使用效果
        /// </summary>
        /// <param name="value">使用效果</param>
        public void SetUseEffect(string value) => SetValue(KEY_USE_EFFECT, value);

        /// <summary>
        /// 获取物品装备槽位
        /// </summary>
        public EquipmentSlot EquipSlot => GetValue<EquipmentSlot>(KEY_EQUIP_SLOT, EquipmentSlot.None);

        /// <summary>
        /// 设置物品装备槽位
        /// </summary>
        /// <param name="value">装备槽位</param>
        public void SetEquipSlot(EquipmentSlot value) => SetValue(KEY_EQUIP_SLOT, value);

        /// <summary>
        /// 获取装备属性
        /// </summary>
        public Dictionary<string, float> EquipmentStats => GetValue<Dictionary<string, float>>(KEY_EQUIPMENT_STATS, new Dictionary<string, float>());

        /// <summary>
        /// 设置装备属性
        /// </summary>
        /// <param name="value">装备属性</param>
        public void SetEquipmentStats(Dictionary<string, float> value) => SetValue(KEY_EQUIPMENT_STATS, value);

        /// <summary>
        /// 获取使用要求
        /// </summary>
        public Dictionary<string, object> UseRequirements => GetValue<Dictionary<string, object>>(KEY_USE_REQUIREMENTS, new Dictionary<string, object>());

        /// <summary>
        /// 设置使用要求
        /// </summary>
        /// <param name="value">使用要求</param>
        public void SetUseRequirements(Dictionary<string, object> value) => SetValue(KEY_USE_REQUIREMENTS, value);

        /// <summary>
        /// 获取物品效果
        /// </summary>
        public List<Dictionary<string, object>> ItemEffects => GetValue<List<Dictionary<string, object>>>(KEY_ITEM_EFFECTS, new List<Dictionary<string, object>>());

        /// <summary>
        /// 设置物品效果
        /// </summary>
        /// <param name="value">物品效果</param>
        public void SetItemEffects(List<Dictionary<string, object>> value) => SetValue(KEY_ITEM_EFFECTS, value);

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="itemId">物品ID</param>
        /// <param name="name">物品名称</param>
        public ItemConfig(string itemId, string name = "")
            : base(GetModuleId(itemId), name ?? "未命名物品", new Version(1, 0, 0))
        {
            if (string.IsNullOrEmpty(itemId))
                throw new ArgumentException("物品ID不能为空", nameof(itemId));

            InitDefaultValues();
            if (!string.IsNullOrEmpty(name))
                SetName(name);
        }

        /// <summary>
        /// 初始化默认值
        /// </summary>
        protected override void InitDefaultValues()
        {
            SetValue(KEY_ID, Guid.NewGuid().ToString());
            SetValue(KEY_NAME, "未命名物品");
            SetValue(KEY_DESCRIPTION, "");
            SetValue(KEY_TYPE, ItemType.None);
            SetValue(KEY_RARITY, ItemRarity.Common);
            SetValue(KEY_ICON_PATH, "");
            SetValue(KEY_IS_STACKABLE, false);
            SetValue(KEY_MAX_STACK_SIZE, 1);
            SetValue(KEY_WEIGHT, 0.1f);
            SetValue(KEY_VALUE, 0);
            SetValue(KEY_LEVEL, 1);
            SetValue(KEY_LEVEL_REQUIREMENT, 1);
            SetValue(KEY_DURABILITY, 100);
            SetValue(KEY_MAX_DURABILITY, 100);
            SetValue(KEY_IS_USABLE, false);
            SetValue(KEY_IS_EQUIPPABLE, false);
            SetValue(KEY_IS_SELLABLE, true);
            SetValue(KEY_IS_DROPPABLE, true);
            SetValue(KEY_IS_TRADABLE, true);
            SetValue(KEY_IS_BOUND, false);
            SetValue(KEY_BIND_TYPE, BindType.None);
            SetValue(KEY_COOLDOWN, 0f);
            SetValue(KEY_COOLDOWN_GROUP, "");
            SetValue(KEY_USE_EFFECT, "");
            SetValue(KEY_EQUIP_SLOT, EquipmentSlot.None);
            SetValue(KEY_EQUIPMENT_STATS, new Dictionary<string, float>());
            SetValue(KEY_USE_REQUIREMENTS, new Dictionary<string, object>());
            SetValue(KEY_ITEM_EFFECTS, new List<Dictionary<string, object>>());
        }

        /// <summary>
        /// 验证配置是否有效
        /// </summary>
        /// <returns>验证结果</returns>
        public override (bool IsValid, string ErrorMessage) Validate()
        {
            if (string.IsNullOrEmpty(Name))
                return (false, "物品名称不能为空");

            if (MaxStackSize < 1)
                return (false, "物品最大堆叠数量必须大于0");

            if (Weight < 0)
                return (false, "物品重量不能为负");

            if (IsEquippable && EquipSlot == EquipmentSlot.None)
                return (false, "可装备物品必须指定装备槽位");

            return base.Validate();
        }

        public T GetCustomProperty<T>(string key)
        {
            return GetValue<T>(key);
        }

        public object GetCustomProperty(string key)
        {
            return GetValue<object>(key);
        }

        public void SetCustomProperty<T>(string key, T value)
        {
            SetValue(key, value);
        }

        public void SetCustomProperty(string key, object value)
        {
            SetValue(key, value);
        }

        public IEnumerable<string> GetCustomPropertyKeys()
        {
            return ConfigItems.Keys;
        }
    }
}