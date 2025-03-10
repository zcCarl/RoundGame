using System;
using System.Collections.Generic;
using TacticalRPG.Core.Modules.Inventory;
using TacticalRPG.Core.Modules.Item;

namespace TacticalRPG.Implementation.Modules.Item
{
    /// <summary>
    /// 物品模板类，用于存储物品模板数据
    /// </summary>
    public class ItemTemplate : IItemTemplate
    {
        private readonly Dictionary<string, object> _attributes = new Dictionary<string, object>();

        /// <summary>
        /// 模板ID
        /// </summary>
        public string TemplateId { get; }

        /// <summary>
        /// 物品名称
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 物品描述
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// 物品类型
        /// </summary>
        public ItemType Type { get; private set; }

        /// <summary>
        /// 物品稀有度
        /// </summary>
        public ItemRarity Rarity { get; private set; }

        /// <summary>
        /// 物品图标路径
        /// </summary>
        public string IconPath { get; private set; }

        /// <summary>
        /// 是否可堆叠
        /// </summary>
        public bool IsStackable { get; private set; }

        /// <summary>
        /// 最大堆叠数量
        /// </summary>
        public int MaxStackSize { get; private set; }

        /// <summary>
        /// 物品重量
        /// </summary>
        public float Weight { get; private set; }

        /// <summary>
        /// 物品价值
        /// </summary>
        public int Value { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="templateId">模板ID</param>
        /// <param name="name">物品名称</param>
        public ItemTemplate(string templateId, string name)
        {
            if (string.IsNullOrEmpty(templateId))
                throw new ArgumentException("模板ID不能为空", nameof(templateId));

            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("物品名称不能为空", nameof(name));

            TemplateId = templateId;
            Name = name;
            Description = string.Empty;
            Type = ItemType.Material; // 默认类型
            Rarity = ItemRarity.Common; // 默认稀有度
            IconPath = string.Empty;
            IsStackable = false;
            MaxStackSize = 1;
            Weight = 0;
            Value = 0;
        }

        /// <summary>
        /// 设置属性值
        /// </summary>
        /// <param name="key">属性键</param>
        /// <param name="value">属性值</param>
        public void SetAttribute(string key, object value)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("属性键不能为空", nameof(key));

            // 处理预定义属性
            switch (key.ToLower())
            {
                case "name":
                    Name = value?.ToString() ?? string.Empty;
                    break;
                case "description":
                    Description = value?.ToString() ?? string.Empty;
                    break;
                case "type":
                    if (value is string typeStr && Enum.TryParse<ItemType>(typeStr, true, out var itemType))
                        Type = itemType;
                    else if (value is ItemType itemType2)
                        Type = itemType2;
                    break;
                case "rarity":
                    if (value is string rarityStr && Enum.TryParse<ItemRarity>(rarityStr, true, out var itemRarity))
                        Rarity = itemRarity;
                    else if (value is ItemRarity itemRarity2)
                        Rarity = itemRarity2;
                    else if (value is int rarityInt && Enum.IsDefined(typeof(ItemRarity), rarityInt))
                        Rarity = (ItemRarity)rarityInt;
                    break;
                case "iconpath":
                    IconPath = value?.ToString() ?? string.Empty;
                    break;
                case "isstackable":
                    if (value is bool isStackable)
                        IsStackable = isStackable;
                    else if (value is string stackableStr && bool.TryParse(stackableStr, out var stackableBool))
                        IsStackable = stackableBool;
                    break;
                case "maxstacksize":
                    if (value is int maxStackSize)
                        MaxStackSize = maxStackSize;
                    else if (value is string maxStackStr && int.TryParse(maxStackStr, out var maxStackInt))
                        MaxStackSize = maxStackInt;
                    break;
                case "weight":
                    if (value is float weight)
                        Weight = weight;
                    else if (value is double weightDouble)
                        Weight = (float)weightDouble;
                    else if (value is string weightStr && float.TryParse(weightStr, out var weightFloat))
                        Weight = weightFloat;
                    break;
                case "value":
                    if (value is int itemValue)
                        Value = itemValue;
                    else if (value is string valueStr && int.TryParse(valueStr, out var valueInt))
                        Value = valueInt;
                    break;
                default:
                    // 存储为自定义属性
                    _attributes[key] = value;
                    break;
            }
        }

        /// <summary>
        /// 获取属性值
        /// </summary>
        /// <param name="key">属性键</param>
        /// <returns>属性值</returns>
        public object GetAttribute(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("属性键不能为空", nameof(key));

            // 处理预定义属性
            switch (key.ToLower())
            {
                case "name":
                    return Name;
                case "description":
                    return Description;
                case "type":
                    return Type;
                case "rarity":
                    return Rarity;
                case "iconpath":
                    return IconPath;
                case "isstackable":
                    return IsStackable;
                case "maxstacksize":
                    return MaxStackSize;
                case "weight":
                    return Weight;
                case "value":
                    return Value;
                default:
                    // 尝试获取自定义属性
                    return _attributes.TryGetValue(key, out var value) ? value : null;
            }
        }

        /// <summary>
        /// 获取所有属性键
        /// </summary>
        /// <returns>属性键集合</returns>
        public IEnumerable<string> GetAttributeKeys()
        {
            // 返回预定义属性和自定义属性的键
            var keys = new List<string>
            {
                "Name",
                "Description",
                "Type",
                "Rarity",
                "IconPath",
                "IsStackable",
                "MaxStackSize",
                "Weight",
                "Value"
            };

            keys.AddRange(_attributes.Keys);
            return keys;
        }
    }
}