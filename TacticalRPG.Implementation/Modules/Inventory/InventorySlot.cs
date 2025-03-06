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
        private string _label = string.Empty;
        private ItemType? _acceptedItemType = null;

        /// <summary>
        /// 获取槽位索引
        /// </summary>
        public int Index => SlotIndex;

        /// <summary>
        /// 获取槽位索引
        /// </summary>
        public int SlotIndex { get; }

        /// <summary>
        /// 获取或设置槽位中的物品
        /// </summary>
        public IItem Item { get; set; }

        /// <summary>
        /// 获取槽位是否为空
        /// </summary>
        public bool IsEmpty => Item == null;

        /// <summary>
        /// 获取槽位是否已锁定
        /// </summary>
        public bool IsLocked { get; private set; }

        /// <summary>
        /// 获取槽位可接受的物品类型（如为null则接受所有类型）
        /// </summary>
        public ItemType? AcceptedItemType => _acceptedItemType;

        /// <summary>
        /// 获取槽位的标签/分类
        /// </summary>
        public string Label => _label;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="slotIndex">槽位索引</param>
        public InventorySlot(int slotIndex)
        {
            SlotIndex = slotIndex;
            Item = null;
            IsLocked = false;
        }

        /// <summary>
        /// 添加物品到槽位
        /// </summary>
        /// <param name="item">要添加的物品</param>
        /// <returns>成功添加返回true，否则返回false</returns>
        public bool AddItem(IItem item)
        {
            if (IsLocked || !CanAcceptItem(item) || !IsEmpty)
            {
                return false;
            }

            Item = item;
            return true;
        }

        /// <summary>
        /// 添加指定数量的物品到槽位
        /// </summary>
        /// <param name="item">要添加的物品</param>
        /// <param name="amount">添加数量</param>
        /// <returns>实际添加的数量</returns>
        public int AddItem(IItem item, int amount)
        {
            if (IsLocked || !CanAcceptItem(item) || !IsEmpty)
            {
                return 0;
            }

            if (amount <= 0)
            {
                return 0;
            }

            Item = item;
            Item.AddToStack(amount - 1); // -1 因为物品本身已经算一个
            return amount;
        }

        /// <summary>
        /// 从槽位移除物品
        /// </summary>
        /// <returns>移除的物品，如果槽位为空则返回null</returns>
        public IItem RemoveItem()
        {
            if (IsLocked || IsEmpty)
            {
                return null;
            }

            var item = Item;
            Item = null;
            return item;
        }

        /// <summary>
        /// 从槽位移除指定数量的物品
        /// </summary>
        /// <param name="amount">移除数量</param>
        /// <returns>移除的物品，包含指定数量</returns>
        public IItem RemoveItem(int amount)
        {
            if (IsLocked || IsEmpty || amount <= 0)
            {
                return null;
            }

            if (Item.StackSize <= amount)
            {
                return RemoveItem();
            }

            var removedItem = Item.SplitStack(amount);
            return removedItem;
        }

        /// <summary>
        /// 锁定/解锁槽位
        /// </summary>
        /// <param name="locked">是否锁定</param>
        /// <returns>设置是否成功</returns>
        public bool SetLocked(bool locked)
        {
            IsLocked = locked;
            return true;
        }

        /// <summary>
        /// 设置槽位接受的物品类型
        /// </summary>
        /// <param name="itemType">物品类型，null表示接受所有类型</param>
        /// <returns>设置是否成功</returns>
        public bool SetAcceptedItemType(ItemType? itemType)
        {
            _acceptedItemType = itemType;
            return true;
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