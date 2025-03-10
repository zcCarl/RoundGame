using System;
using System.Collections.Generic;
using TacticalRPG.Core.Modules.Item;

namespace TacticalRPG.Implementation.Modules.Item
{
    /// <summary>
    /// 物品实现类
    /// </summary>
    public class Item : IItem
    {
        private readonly Dictionary<string, object> _attributes = new Dictionary<string, object>();
        private bool _isLocked = false;
        private Guid? _equipmentId = null;
        private readonly ItemConfig _config;

        /// <summary>
        /// 获取物品的唯一标识符
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// 获取物品的名称
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 获取物品的描述
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// 获取物品的类型
        /// </summary>
        public ItemType Type { get; private set; }

        /// <summary>
        /// 获取物品的稀有度
        /// </summary>
        public ItemRarity Rarity { get; private set; }

        /// <summary>
        /// 获取物品的图标路径
        /// </summary>
        public string IconPath { get; private set; }

        /// <summary>
        /// 获取物品是否可堆叠
        /// </summary>
        public bool IsStackable { get; private set; }

        /// <summary>
        /// 获取物品最大堆叠数量
        /// </summary>
        public int MaxStackSize { get; private set; }

        /// <summary>
        /// 获取物品当前堆叠数量
        /// </summary>
        public int StackSize { get; set; }

        /// <summary>
        /// 获取物品的重量
        /// </summary>
        public float Weight { get; private set; }

        /// <summary>
        /// 获取物品的价值
        /// </summary>
        public int Value { get; private set; }

        /// <summary>
        /// 获取物品的等级
        /// </summary>
        public int Level { get; private set; }

        /// <summary>
        /// 获取物品的使用等级要求
        /// </summary>
        public int LevelRequirement { get; private set; }

        /// <summary>
        /// 获取物品的耐久度
        /// </summary>
        public int Durability { get; private set; }

        /// <summary>
        /// 获取物品的最大耐久度
        /// </summary>
        public int MaxDurability { get; private set; }

        /// <summary>
        /// 获取物品是否可用
        /// </summary>
        public bool IsUsable { get; private set; }

        /// <summary>
        /// 获取物品是否可装备
        /// </summary>
        public bool IsEquippable { get; private set; }

        /// <summary>
        /// 获取物品是否可出售
        /// </summary>
        public bool IsSellable { get; private set; }

        /// <summary>
        /// 获取物品是否可丢弃
        /// </summary>
        public bool IsDroppable { get; private set; }

        /// <summary>
        /// 获取物品是否可交易
        /// </summary>
        public bool IsTradable { get; private set; }

        /// <summary>
        /// 获取物品是否绑定
        /// </summary>
        public bool IsBound { get; private set; }

        /// <summary>
        /// 获取物品的绑定类型
        /// </summary>
        public BindType BindType { get; private set; }

        /// <summary>
        /// 获取物品的冷却时间（秒）
        /// </summary>
        public float Cooldown { get; private set; }

        /// <summary>
        /// 获取物品的冷却组
        /// </summary>
        public string CooldownGroup { get; private set; }

        /// <summary>
        /// 获取物品的使用效果
        /// </summary>
        public string UseEffect { get; private set; }

        /// <summary>
        /// 获取物品的装备槽位
        /// </summary>
        public EquipmentSlot EquipSlot { get; private set; }

        /// <summary>
        /// 获取物品的模板ID
        /// </summary>
        public string TemplateId => _config?.TemplateId;

        /// <summary>
        /// 获取物品是否为装备
        /// </summary>
        public bool IsEquipment => Type == ItemType.Equipment && _equipmentId.HasValue;

        /// <summary>
        /// 获取物品是否为消耗品
        /// </summary>
        public bool IsConsumable => Type == ItemType.Consumable;

        /// <summary>
        /// 获取物品是否为任务物品
        /// </summary>
        public bool IsQuestItem => Type == ItemType.QuestItem;

        /// <summary>
        /// 获取装备ID（如果物品是装备）
        /// </summary>
        public Guid? EquipmentId => _equipmentId;

        /// <summary>
        /// 获取物品是否被锁定（不可交易/丢弃/销毁）
        /// </summary>
        public bool IsLocked => _isLocked;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="config">物品配置</param>
        /// <param name="stackSize">堆叠数量</param>
        public Item(ItemConfig config, int stackSize = 1)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            _config = config;
            Id = Guid.NewGuid();
            Name = config.Name;
            Description = config.Description;
            Type = config.Type;
            Rarity = config.Rarity;
            IconPath = config.IconPath;
            IsStackable = config.IsStackable;
            MaxStackSize = config.MaxStackSize;
            StackSize = Math.Min(stackSize, MaxStackSize);
            Weight = config.Weight;
            Value = config.Value;
            Level = config.Level;
            LevelRequirement = config.LevelRequirement;
            Durability = config.Durability;
            MaxDurability = config.MaxDurability;
            IsUsable = config.IsUsable;
            IsEquippable = config.IsEquippable;
            IsSellable = config.IsSellable;
            IsDroppable = config.IsDroppable;
            IsTradable = config.IsTradable;
            IsBound = config.IsBound;
            BindType = config.BindType;
            Cooldown = config.Cooldown;
            CooldownGroup = config.CooldownGroup;
            UseEffect = config.UseEffect;
            EquipSlot = config.EquipSlot;

            // 复制属性
            foreach (var key in config.GetCustomPropertyKeys())
            {
                _attributes[key] = config.GetCustomProperty(key);
            }
        }

        /// <summary>
        /// 复制构造函数，用于创建物品的副本
        /// </summary>
        /// <param name="source">源物品</param>
        /// <param name="stackSize">堆叠数量，默认使用源物品的堆叠数量</param>
        private Item(Item source, int stackSize = 0)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            _config = source._config;
            Id = Guid.NewGuid();
            Name = source.Name;
            Description = source.Description;
            Type = source.Type;
            Rarity = source.Rarity;
            IconPath = source.IconPath;
            IsStackable = source.IsStackable;
            MaxStackSize = source.MaxStackSize;
            StackSize = stackSize > 0 ? Math.Min(stackSize, MaxStackSize) : source.StackSize;
            Weight = source.Weight;
            Value = source.Value;
            Level = source.Level;
            LevelRequirement = source.LevelRequirement;
            Durability = source.Durability;
            MaxDurability = source.MaxDurability;
            IsUsable = source.IsUsable;
            IsEquippable = source.IsEquippable;
            IsSellable = source.IsSellable;
            IsDroppable = source.IsDroppable;
            IsTradable = source.IsTradable;
            IsBound = source.IsBound;
            BindType = source.BindType;
            Cooldown = source.Cooldown;
            CooldownGroup = source.CooldownGroup;
            UseEffect = source.UseEffect;
            EquipSlot = source.EquipSlot;
            _equipmentId = source._equipmentId;

            // 复制属性
            foreach (var key in source._attributes.Keys)
            {
                _attributes[key] = source._attributes[key];
            }
        }

        /// <summary>
        /// 设置物品名称
        /// </summary>
        /// <param name="name">名称</param>
        public void SetName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("物品名称不能为空", nameof(name));
            }

            Name = name;
        }

        /// <summary>
        /// 设置物品描述
        /// </summary>
        /// <param name="description">描述</param>
        public void SetDescription(string description)
        {
            Description = description ?? string.Empty;
        }

        /// <summary>
        /// 设置物品类型
        /// </summary>
        /// <param name="type">类型</param>
        public void SetType(ItemType type)
        {
            Type = type;
        }

        /// <summary>
        /// 设置物品稀有度
        /// </summary>
        /// <param name="rarity">稀有度</param>
        public void SetRarity(ItemRarity rarity)
        {
            Rarity = rarity;
        }

        /// <summary>
        /// 设置物品图标路径
        /// </summary>
        /// <param name="iconPath">图标路径</param>
        public void SetIconPath(string iconPath)
        {
            IconPath = iconPath ?? string.Empty;
        }

        /// <summary>
        /// 设置物品是否可堆叠
        /// </summary>
        /// <param name="isStackable">是否可堆叠</param>
        public void SetStackable(bool isStackable)
        {
            IsStackable = isStackable;
        }

        /// <summary>
        /// 设置物品最大堆叠数量
        /// </summary>
        /// <param name="maxStackSize">最大堆叠数量</param>
        public void SetMaxStackSize(int maxStackSize)
        {
            if (maxStackSize < 1)
            {
                throw new ArgumentException("最大堆叠数量必须大于0", nameof(maxStackSize));
            }

            MaxStackSize = maxStackSize;
        }

        /// <summary>
        /// 设置物品当前堆叠数量
        /// </summary>
        /// <param name="stackSize">堆叠数量</param>
        public void SetStackSize(int stackSize)
        {
            if (stackSize < 0)
            {
                throw new ArgumentException("堆叠数量不能为负数", nameof(stackSize));
            }

            if (stackSize > MaxStackSize)
            {
                throw new ArgumentException($"堆叠数量不能超过最大堆叠数量({MaxStackSize})", nameof(stackSize));
            }

            StackSize = stackSize;
        }

        /// <summary>
        /// 设置物品重量
        /// </summary>
        /// <param name="weight">重量</param>
        public void SetWeight(float weight)
        {
            if (weight < 0)
            {
                throw new ArgumentException("重量不能为负数", nameof(weight));
            }

            Weight = weight;
        }

        /// <summary>
        /// 设置物品价值
        /// </summary>
        /// <param name="value">价值</param>
        public void SetValue(int value)
        {
            if (value < 0)
            {
                throw new ArgumentException("价值不能为负数", nameof(value));
            }

            Value = value;
        }

        /// <summary>
        /// 设置物品等级
        /// </summary>
        /// <param name="level">等级</param>
        public void SetLevel(int level)
        {
            if (level < 0)
            {
                throw new ArgumentException("等级不能为负数", nameof(level));
            }

            Level = level;
        }

        /// <summary>
        /// 设置物品使用等级要求
        /// </summary>
        /// <param name="levelRequirement">等级要求</param>
        public void SetLevelRequirement(int levelRequirement)
        {
            if (levelRequirement < 0)
            {
                throw new ArgumentException("等级要求不能为负数", nameof(levelRequirement));
            }

            LevelRequirement = levelRequirement;
        }

        /// <summary>
        /// 设置物品耐久度
        /// </summary>
        /// <param name="durability">耐久度</param>
        public void SetDurability(int durability)
        {
            if (durability < 0)
            {
                throw new ArgumentException("耐久度不能为负数", nameof(durability));
            }

            if (durability > MaxDurability)
            {
                throw new ArgumentException($"耐久度不能超过最大耐久度({MaxDurability})", nameof(durability));
            }

            Durability = durability;
        }

        /// <summary>
        /// 设置物品最大耐久度
        /// </summary>
        /// <param name="maxDurability">最大耐久度</param>
        public void SetMaxDurability(int maxDurability)
        {
            if (maxDurability < 0)
            {
                throw new ArgumentException("最大耐久度不能为负数", nameof(maxDurability));
            }

            var oldMaxDurability = MaxDurability;
            MaxDurability = maxDurability;

            // 如果当前耐久度超过新的最大耐久度，则调整当前耐久度
            if (Durability > MaxDurability)
            {
                Durability = MaxDurability;
            }
        }

        /// <summary>
        /// 设置物品是否可用
        /// </summary>
        /// <param name="isUsable">是否可用</param>
        public void SetUsable(bool isUsable)
        {
            IsUsable = isUsable;
        }

        /// <summary>
        /// 设置物品是否可装备
        /// </summary>
        /// <param name="isEquippable">是否可装备</param>
        public void SetEquippable(bool isEquippable)
        {
            IsEquippable = isEquippable;
        }

        /// <summary>
        /// 设置物品是否可出售
        /// </summary>
        /// <param name="isSellable">是否可出售</param>
        public void SetSellable(bool isSellable)
        {
            IsSellable = isSellable;
        }

        /// <summary>
        /// 设置物品是否可丢弃
        /// </summary>
        /// <param name="isDroppable">是否可丢弃</param>
        public void SetDroppable(bool isDroppable)
        {
            IsDroppable = isDroppable;
        }

        /// <summary>
        /// 设置物品是否可交易
        /// </summary>
        /// <param name="isTradable">是否可交易</param>
        public void SetTradable(bool isTradable)
        {
            IsTradable = isTradable;
        }

        /// <summary>
        /// 设置物品是否绑定
        /// </summary>
        /// <param name="isBound">是否绑定</param>
        public void SetBound(bool isBound)
        {
            IsBound = isBound;
        }

        /// <summary>
        /// 设置物品绑定类型
        /// </summary>
        /// <param name="bindType">绑定类型</param>
        public void SetBindType(BindType bindType)
        {
            BindType = bindType;
        }

        /// <summary>
        /// 设置物品冷却时间
        /// </summary>
        /// <param name="cooldown">冷却时间（秒）</param>
        public void SetCooldown(float cooldown)
        {
            if (cooldown < 0)
            {
                throw new ArgumentException("冷却时间不能为负数", nameof(cooldown));
            }

            Cooldown = cooldown;
        }

        /// <summary>
        /// 设置物品冷却组
        /// </summary>
        /// <param name="cooldownGroup">冷却组</param>
        public void SetCooldownGroup(string cooldownGroup)
        {
            CooldownGroup = cooldownGroup ?? string.Empty;
        }

        /// <summary>
        /// 设置物品使用效果
        /// </summary>
        /// <param name="useEffect">使用效果</param>
        public void SetUseEffect(string useEffect)
        {
            UseEffect = useEffect ?? string.Empty;
        }

        /// <summary>
        /// 设置物品装备槽位
        /// </summary>
        /// <param name="equipSlot">装备槽位</param>
        public void SetEquipSlot(EquipmentSlot equipSlot)
        {
            EquipSlot = equipSlot;
        }

        /// <summary>
        /// 设置物品属性
        /// </summary>
        /// <param name="key">属性键</param>
        /// <param name="value">属性值</param>
        public void SetAttribute(string key, object value)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("属性键不能为空", nameof(key));
            }

            _attributes[key] = value;
        }

        /// <summary>
        /// 获取物品属性
        /// </summary>
        /// <param name="key">属性键</param>
        /// <returns>属性值</returns>
        public object GetAttribute(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("属性键不能为空", nameof(key));
            }

            if (!_attributes.ContainsKey(key))
            {
                return null;
            }

            return _attributes[key];
        }

        /// <summary>
        /// 获取物品属性（泛型版本）
        /// </summary>
        /// <typeparam name="T">属性类型</typeparam>
        /// <param name="key">属性键</param>
        /// <returns>属性值</returns>
        public T GetAttribute<T>(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("属性键不能为空", nameof(key));
            }

            if (!_attributes.ContainsKey(key))
            {
                return default;
            }

            var value = _attributes[key];
            if (value is T typedValue)
            {
                return typedValue;
            }

            try
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch
            {
                return default;
            }
        }

        /// <summary>
        /// 获取所有属性键
        /// </summary>
        /// <returns>属性键集合</returns>
        public IEnumerable<string> GetAttributeKeys()
        {
            return _attributes.Keys;
        }

        /// <summary>
        /// 移除物品属性
        /// </summary>
        /// <param name="key">属性键</param>
        /// <returns>是否成功移除</returns>
        public bool RemoveAttribute(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("属性键不能为空", nameof(key));
            }

            return _attributes.Remove(key);
        }

        /// <summary>
        /// 清空所有属性
        /// </summary>
        public void ClearAttributes()
        {
            _attributes.Clear();
        }

        /// <summary>
        /// 使用物品
        /// </summary>
        /// <param name="characterId">使用者ID</param>
        /// <param name="targetId">目标ID（可选）</param>
        /// <returns>使用结果</returns>
        public (bool success, string message) Use(Guid characterId, Guid? targetId = null)
        {
            if (!IsUsable)
            {
                return (false, "物品不可使用");
            }

            if (StackSize <= 0)
            {
                return (false, "物品数量不足");
            }

            // 物品模块不应该知道如何使用物品，只返回使用请求成功，具体效果由其他模块实现
            // 这里仅做基本验证，减少堆叠数量
            RemoveFromStack(1);
            return (true, $"使用了{Name}");
        }

        /// <summary>
        /// 检查物品是否可以被特定角色使用
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <param name="targetId">目标ID（可选）</param>
        /// <returns>检查结果</returns>
        public (bool canUse, string reason) CanBeUsedBy(Guid characterId, Guid? targetId = null)
        {
            if (!IsUsable)
            {
                return (false, "物品不可使用");
            }

            if (StackSize <= 0)
            {
                return (false, "物品数量不足");
            }

            // 物品模块不应该知道具体的使用条件，只检查基本条件
            return (true, "可以使用");
        }

        /// <summary>
        /// 添加物品到堆叠
        /// </summary>
        /// <param name="amount">添加数量</param>
        /// <returns>实际添加的数量</returns>
        public int AddToStack(int amount)
        {
            if (!IsStackable || amount <= 0)
            {
                return 0;
            }

            int canAdd = MaxStackSize - StackSize;
            if (canAdd <= 0)
            {
                return 0;
            }

            int actualAdd = Math.Min(amount, canAdd);
            StackSize += actualAdd;
            return actualAdd;
        }

        /// <summary>
        /// 从堆叠中移除物品
        /// </summary>
        /// <param name="amount">移除数量</param>
        /// <returns>实际移除的数量</returns>
        public int RemoveFromStack(int amount)
        {
            if (amount <= 0)
            {
                return 0;
            }

            int actualRemove = Math.Min(amount, StackSize);
            StackSize -= actualRemove;
            return actualRemove;
        }

        /// <summary>
        /// 拆分堆叠
        /// </summary>
        /// <param name="amount">拆分数量</param>
        /// <returns>拆分出的新物品实例</returns>
        public IItem SplitStack(int amount)
        {
            if (!IsStackable || amount <= 0 || amount >= StackSize)
            {
                return null;
            }

            var newItem = Clone(amount);
            StackSize -= amount;
            return newItem;
        }

        /// <summary>
        /// 合并堆叠
        /// </summary>
        /// <param name="item">要合并的物品</param>
        /// <returns>合并后的剩余物品（如完全合并则为null）</returns>
        public IItem MergeStack(IItem item)
        {
            if (item == null || !IsStackable || !item.IsStackable || item.Id == Id || item.Type != Type)
            {
                return item;
            }

            int canAdd = MaxStackSize - StackSize;
            if (canAdd <= 0)
            {
                return item;
            }

            int actualAdd = Math.Min(item.StackSize, canAdd);
            StackSize += actualAdd;

            if (actualAdd >= item.StackSize)
            {
                return null; // 完全合并
            }
            else
            {
                item.StackSize -= actualAdd;
                return item; // 部分合并
            }
        }

        /// <summary>
        /// 锁定/解锁物品
        /// </summary>
        /// <param name="locked">是否锁定</param>
        public void SetLocked(bool locked)
        {
            _isLocked = locked;
        }

        /// <summary>
        /// 获取物品的自定义属性
        /// </summary>
        /// <param name="key">属性键</param>
        /// <returns>属性值，如不存在则返回null</returns>
        public object GetProperty(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return null;
            }

            if (_attributes.TryGetValue(key, out var value))
            {
                return value;
            }

            return null;
        }

        /// <summary>
        /// 设置物品的自定义属性
        /// </summary>
        /// <param name="key">属性键</param>
        /// <param name="value">属性值</param>
        public void SetProperty(string key, object value)
        {
            if (string.IsNullOrEmpty(key))
            {
                return;
            }

            _attributes[key] = value;
        }

        /// <summary>
        /// 创建物品的副本
        /// </summary>
        /// <param name="amount">数量（默认全部）</param>
        /// <returns>物品的副本</returns>
        public IItem Clone(int amount = 0)
        {
            return new Item(this, amount);
        }

        /// <summary>
        /// 设置装备ID
        /// </summary>
        /// <param name="equipmentId">装备ID</param>
        public void SetEquipmentId(Guid? equipmentId)
        {
            _equipmentId = equipmentId;
        }
    }

    /// <summary>
    /// 装备槽位枚举
    /// </summary>
    public enum EquipmentSlot
    {
        /// <summary>
        /// 无
        /// </summary>
        None = 0,

        /// <summary>
        /// 主手武器
        /// </summary>
        MainHand = 1,

        /// <summary>
        /// 副手
        /// </summary>
        OffHand = 2,

        /// <summary>
        /// 头部
        /// </summary>
        Head = 3,

        /// <summary>
        /// 身体
        /// </summary>
        Body = 4,

        /// <summary>
        /// 手部
        /// </summary>
        Hands = 5,

        /// <summary>
        /// 腿部
        /// </summary>
        Legs = 6,

        /// <summary>
        /// 足部
        /// </summary>
        Feet = 7,

        /// <summary>
        /// 颈部
        /// </summary>
        Neck = 8,

        /// <summary>
        /// 戒指1
        /// </summary>
        Ring1 = 9,

        /// <summary>
        /// 戒指2
        /// </summary>
        Ring2 = 10
    }

    /// <summary>
    /// 物品绑定类型
    /// </summary>
    public enum BindType
    {
        /// <summary>
        /// 不绑定
        /// </summary>
        None,

        /// <summary>
        /// 拾取绑定
        /// </summary>
        BindOnPickup,

        /// <summary>
        /// 装备绑定
        /// </summary>
        BindOnEquip,

        /// <summary>
        /// 使用绑定
        /// </summary>
        BindOnUse
    }
}