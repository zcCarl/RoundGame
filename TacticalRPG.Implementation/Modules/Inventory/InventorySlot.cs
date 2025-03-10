using System;
using System.Collections.Generic;
using TacticalRPG.Core.Modules.Inventory;

namespace TacticalRPG.Implementation.Modules.Inventory
{
    /// <summary>
    /// 背包槽位实现类
    /// </summary>
    public class InventorySlot : IInventorySlot
    {
        private Dictionary<string, object> _properties = new Dictionary<string, object>();
        private bool _isLocked = false;
        private ItemType? _acceptedItemType = null;
        private string _label = string.Empty;
        private Guid _itemId = Guid.Empty;
        private int _count = 0;

        /// <summary>
        /// 获取槽位索引
        /// </summary>
        public int SlotIndex { get; }

        /// <summary>
        /// 获取槽位是否为空
        /// </summary>
        public bool IsEmpty => _itemId == Guid.Empty || _count <= 0;

        /// <summary>
        /// 获取槽位是否被锁定
        /// </summary>
        public bool IsLocked => _isLocked;

        /// <summary>
        /// 获取槽位限制的物品类型
        /// </summary>
        public ItemType? AcceptedItemType => _acceptedItemType;

        /// <summary>
        /// 获取槽位标签
        /// </summary>
        public string Label => _label;

        /// <summary>
        /// 获取槽位中的物品ID
        /// </summary>
        public Guid ItemId => _itemId;

        /// <summary>
        /// 获取物品数量
        /// </summary>
        public int Count => _count;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="slotIndex">槽位索引</param>
        public InventorySlot(int slotIndex)
        {
            SlotIndex = slotIndex;
        }

        /// <summary>
        /// 设置物品ID和数量
        /// </summary>
        /// <param name="itemId">物品ID</param>
        /// <param name="count">数量</param>
        /// <returns>是否设置成功</returns>
        public bool SetItem(Guid itemId, int count)
        {
            if (_isLocked)
            {
                return false; // 槽位被锁定，无法设置物品
            }

            if (itemId == Guid.Empty || count <= 0)
            {
                Clear(); // 清空槽位
                return true;
            }

            _itemId = itemId;
            _count = count;
            return true;
        }

        /// <summary>
        /// 清空槽位
        /// </summary>
        public void Clear()
        {
            if (_isLocked)
            {
                return; // 槽位被锁定，无法清空
            }

            _itemId = Guid.Empty;
            _count = 0;
        }

        /// <summary>
        /// 锁定或解锁槽位
        /// </summary>
        /// <param name="locked">是否锁定</param>
        public void SetLocked(bool locked)
        {
            _isLocked = locked;
        }

        /// <summary>
        /// 设置槽位接受的物品类型
        /// </summary>
        /// <param name="itemType">物品类型</param>
        public void SetAcceptedItemType(ItemType? itemType)
        {
            _acceptedItemType = itemType;
        }

        /// <summary>
        /// 设置槽位标签
        /// </summary>
        /// <param name="label">标签文本</param>
        public void SetLabel(string label)
        {
            _label = label ?? string.Empty;
        }

        /// <summary>
        /// 增加物品数量
        /// </summary>
        /// <param name="amount">增加数量</param>
        /// <returns>实际增加的数量</returns>
        public int AddCount(int amount)
        {
            if (_isLocked || IsEmpty || amount <= 0)
            {
                return 0;
            }

            _count += amount;
            return amount;
        }

        /// <summary>
        /// 减少物品数量
        /// </summary>
        /// <param name="amount">减少数量</param>
        /// <returns>实际减少的数量</returns>
        public int RemoveCount(int amount)
        {
            if (_isLocked || IsEmpty || amount <= 0)
            {
                return 0;
            }

            int actualAmount = Math.Min(_count, amount);
            _count -= actualAmount;

            if (_count <= 0)
            {
                Clear(); // 数量为零，清空槽位
            }

            return actualAmount;
        }

        /// <summary>
        /// 获取槽位的自定义属性
        /// </summary>
        /// <param name="key">属性键</param>
        /// <returns>属性值，如不存在则返回null</returns>
        public object GetProperty(string key)
        {
            if (string.IsNullOrEmpty(key) || !_properties.ContainsKey(key))
            {
                return null;
            }

            return _properties[key];
        }

        /// <summary>
        /// 设置槽位的自定义属性
        /// </summary>
        /// <param name="key">属性键</param>
        /// <param name="value">属性值</param>
        public void SetProperty(string key, object value)
        {
            if (string.IsNullOrEmpty(key))
            {
                return;
            }

            if (value == null && _properties.ContainsKey(key))
            {
                _properties.Remove(key);
                return;
            }

            _properties[key] = value;
        }

        /// <summary>
        /// 检查槽位是否可以接受指定物品
        /// </summary>
        /// <param name="item">要检查的物品</param>
        /// <returns>是否可接受</returns>
        public bool CanAcceptItem(IItem item)
        {
            if (item == null)
            {
                return false;
            }

            // 基本类型检查
            if (_acceptedItemType != null && _acceptedItemType != item.Type)
            {
                return false;
            }

            // 如果槽位已有物品
            if (!IsEmpty)
            {
                // 如果双方都可堆叠且是相同模板的物品
                if (item.IsStackable && Item.IsStackable &&
                    item.TemplateId == Item.TemplateId &&
                    item.Type == Item.Type &&
                    item.Rarity == Item.Rarity)
                {
                    // 检查是否有堆叠空间
                    return Item.StackSize < Item.MaxStackSize;
                }

                // 如果槽位已有物品且不能堆叠，则拒绝
                return false;
            }

            // 空槽位，按类型接受
            return true;
        }

        /// <summary>
        /// 检查槽位是否可以接受指定类型的物品
        /// </summary>
        /// <param name="itemType">物品类型</param>
        /// <returns>是否可接受</returns>
        public bool CanAcceptItemType(ItemType itemType)
        {
            return _acceptedItemType == null || _acceptedItemType == itemType;
        }

        /// <summary>
        /// 尝试合并物品到槽位中的物品
        /// </summary>
        /// <param name="item">要合并的物品</param>
        /// <returns>合并后剩余的物品（如完全合并则返回null）</returns>
        public IItem MergeItem(IItem item)
        {
            if (IsLocked || item == null)
            {
                return item;
            }

            // 如果槽位为空，直接添加
            if (IsEmpty)
            {
                if (CanAcceptItem(item))
                {
                    Item = item;
                    return null;
                }
                return item;
            }

            // 如果两个物品可以堆叠
            if (Item.IsStackable && item.IsStackable &&
                Item.TemplateId == item.TemplateId &&
                Item.Type == item.Type &&
                Item.Rarity == item.Rarity)
            {
                return Item.MergeStack(item);
            }

            return item;
        }

        /// <summary>
        /// 设置槽位中的物品
        /// </summary>
        /// <param name="item">物品实例</param>
        public void SetItem(IItem item)
        {
            if (!IsLocked)
            {
                Item = item;
            }
        }
    }
}