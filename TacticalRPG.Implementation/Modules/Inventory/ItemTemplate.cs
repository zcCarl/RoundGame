using System;
using System.Collections.Generic;
using TacticalRPG.Core.Modules.Character;
using TacticalRPG.Core.Modules.Equipment;
using TacticalRPG.Core.Modules.Inventory;

namespace TacticalRPG.Implementation.Modules.Inventory
{
    /// <summary>
    /// 物品模板类，用于创建物品实例
    /// </summary>
    public class ItemTemplate : IItem
    {
        private readonly Dictionary<string, object> _attributes = new Dictionary<string, object>();
        private IEquipment _equipmentInstance;

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ItemType Type { get; set; }
        public ItemRarity Rarity { get; set; }
        public string IconPath { get; set; }
        public bool IsStackable { get; set; }
        public int MaxStackSize { get; set; }
        public int StackSize { get; set; }
        public float Weight { get; set; }
        public int Value { get; set; }
        public int Level { get; set; }
        public int LevelRequirement { get; set; }
        public int Durability { get; set; }
        public int MaxDurability { get; set; }
        public bool IsUsable { get; set; }
        public bool IsEquippable { get; set; }
        public bool IsSellable { get; set; }
        public bool IsDroppable { get; set; }
        public bool IsTradable { get; set; }
        public bool IsBound { get; set; }
        public BindType BindType { get; set; }
        public int Cooldown { get; set; }
        public string CooldownGroup { get; set; }
        public string UseEffect { get; set; }
        public EquipmentSlot EquipSlot { get; set; }
        public string TemplateId { get; set; }

        public bool IsEquipment => Type == ItemType.Equipment && _equipmentInstance != null;
        public bool IsConsumable => Type == ItemType.Consumable;
        public bool IsQuestItem => Type == ItemType.QuestItem;
        public IEquipment EquipmentInstance => _equipmentInstance;
        public bool IsLocked { get; set; }

        public ItemTemplate()
        {
            Id = Guid.NewGuid();
        }

        public void SetEquipmentInstance(IEquipment equipment)
        {
            _equipmentInstance = equipment;
        }

        public bool CanUse(ICharacter character)
        {
            return IsUsable && !IsLocked;
        }

        public bool Use(ICharacter character)
        {
            return IsUsable && !IsLocked;
        }

        public bool CanAddToStack(IItem item)
        {
            return IsStackable && item.Type == Type && item.Rarity == Rarity && StackSize < MaxStackSize;
        }

        public int AddToStack(int amount)
        {
            if (!IsStackable || amount <= 0)
                return 0;

            int canAdd = MaxStackSize - StackSize;
            int actualAdd = Math.Min(canAdd, amount);
            StackSize += actualAdd;
            return actualAdd;
        }

        public int RemoveFromStack(int amount)
        {
            if (amount <= 0)
                return 0;

            int actualRemove = Math.Min(StackSize, amount);
            StackSize -= actualRemove;
            return actualRemove;
        }

        public IItem SplitStack(int amount)
        {
            return this;
        }

        public bool CanMergeWith(IItem item)
        {
            return IsStackable && item.IsStackable && item.Type == Type && item.Rarity == Rarity;
        }

        public void MergeWith(IItem item)
        {
            if (CanMergeWith(item))
            {
                AddToStack(item.StackSize);
            }
        }

        public void SetStackSize(int size)
        {
            StackSize = Math.Max(1, Math.Min(MaxStackSize, size));
        }

        public void Lock()
        {
            IsLocked = true;
        }

        public void Unlock()
        {
            IsLocked = false;
        }

        public void SetAttribute(string key, object value)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("属性键不能为空", nameof(key));

            _attributes[key] = value;
        }

        public T GetAttribute<T>(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("属性键不能为空", nameof(key));

            if (_attributes.TryGetValue(key, out var value) && value is T typedValue)
                return typedValue;

            return default;
        }

        public bool HasAttribute(string key)
        {
            return !string.IsNullOrEmpty(key) && _attributes.ContainsKey(key);
        }

        public void RemoveAttribute(string key)
        {
            if (!string.IsNullOrEmpty(key) && _attributes.ContainsKey(key))
                _attributes.Remove(key);
        }

        public IEnumerable<string> GetAttributeKeys()
        {
            return _attributes.Keys;
        }

        public object GetAttribute(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("属性键不能为空", nameof(key));

            return _attributes.TryGetValue(key, out var value) ? value : null;
        }
    }
}