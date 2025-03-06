using System;
using System.Collections.Generic;

namespace TacticalRPG.Core.Modules.Inventory
{
    /// <summary>
    /// 物品添加事件参数
    /// </summary>
    public class ItemAddedEventArgs : EventArgs
    {
        /// <summary>
        /// 背包拥有者ID
        /// </summary>
        public Guid OwnerId { get; }

        /// <summary>
        /// 物品ID
        /// </summary>
        public Guid ItemId { get; }

        /// <summary>
        /// 物品类型
        /// </summary>
        public ItemType ItemType { get; }

        /// <summary>
        /// 数量
        /// </summary>
        public int Quantity { get; }

        /// <summary>
        /// 背包位置（如适用）
        /// </summary>
        public int? SlotIndex { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ownerId">背包拥有者ID</param>
        /// <param name="itemId">物品ID</param>
        /// <param name="itemType">物品类型</param>
        /// <param name="quantity">数量</param>
        /// <param name="slotIndex">背包位置</param>
        public ItemAddedEventArgs(Guid ownerId, Guid itemId, ItemType itemType, int quantity, int? slotIndex = null)
        {
            OwnerId = ownerId;
            ItemId = itemId;
            ItemType = itemType;
            Quantity = quantity;
            SlotIndex = slotIndex;
        }
    }

    /// <summary>
    /// 物品移除事件参数
    /// </summary>
    public class ItemRemovedEventArgs : EventArgs
    {
        /// <summary>
        /// 背包拥有者ID
        /// </summary>
        public Guid OwnerId { get; }

        /// <summary>
        /// 物品ID
        /// </summary>
        public Guid ItemId { get; }

        /// <summary>
        /// 物品类型
        /// </summary>
        public ItemType ItemType { get; }

        /// <summary>
        /// 数量
        /// </summary>
        public int Quantity { get; }

        /// <summary>
        /// 背包位置
        /// </summary>
        public int SlotIndex { get; }

        /// <summary>
        /// 移除原因
        /// </summary>
        public string Reason { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ownerId">背包拥有者ID</param>
        /// <param name="itemId">物品ID</param>
        /// <param name="itemType">物品类型</param>
        /// <param name="quantity">数量</param>
        /// <param name="slotIndex">背包位置</param>
        /// <param name="reason">移除原因</param>
        public ItemRemovedEventArgs(Guid ownerId, Guid itemId, ItemType itemType, int quantity, int slotIndex, string reason = "")
        {
            OwnerId = ownerId;
            ItemId = itemId;
            ItemType = itemType;
            Quantity = quantity;
            SlotIndex = slotIndex;
            Reason = reason;
        }
    }

    /// <summary>
    /// 物品使用事件参数
    /// </summary>
    public class ItemUsedEventArgs : EventArgs
    {
        /// <summary>
        /// 背包拥有者ID
        /// </summary>
        public Guid OwnerId { get; }

        /// <summary>
        /// 物品ID
        /// </summary>
        public Guid ItemId { get; }

        /// <summary>
        /// 物品类型
        /// </summary>
        public ItemType ItemType { get; }

        /// <summary>
        /// 背包位置
        /// </summary>
        public int SlotIndex { get; }

        /// <summary>
        /// 目标ID（如适用）
        /// </summary>
        public Guid? TargetId { get; }

        /// <summary>
        /// 使用结果
        /// </summary>
        public bool Success { get; }

        /// <summary>
        /// 结果消息
        /// </summary>
        public string ResultMessage { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ownerId">背包拥有者ID</param>
        /// <param name="itemId">物品ID</param>
        /// <param name="itemType">物品类型</param>
        /// <param name="slotIndex">背包位置</param>
        /// <param name="success">使用结果</param>
        /// <param name="targetId">目标ID</param>
        /// <param name="resultMessage">结果消息</param>
        public ItemUsedEventArgs(Guid ownerId, Guid itemId, ItemType itemType, int slotIndex, bool success, Guid? targetId = null, string resultMessage = "")
        {
            OwnerId = ownerId;
            ItemId = itemId;
            ItemType = itemType;
            SlotIndex = slotIndex;
            Success = success;
            TargetId = targetId;
            ResultMessage = resultMessage;
        }
    }

    /// <summary>
    /// 物品移动事件参数
    /// </summary>
    public class ItemMovedEventArgs : EventArgs
    {
        /// <summary>
        /// 背包拥有者ID
        /// </summary>
        public Guid OwnerId { get; }

        /// <summary>
        /// 物品ID
        /// </summary>
        public Guid ItemId { get; }

        /// <summary>
        /// 物品类型
        /// </summary>
        public ItemType ItemType { get; }

        /// <summary>
        /// 源背包位置
        /// </summary>
        public int SourceSlotIndex { get; }

        /// <summary>
        /// 目标背包位置
        /// </summary>
        public int TargetSlotIndex { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ownerId">背包拥有者ID</param>
        /// <param name="itemId">物品ID</param>
        /// <param name="itemType">物品类型</param>
        /// <param name="sourceSlotIndex">源背包位置</param>
        /// <param name="targetSlotIndex">目标背包位置</param>
        public ItemMovedEventArgs(Guid ownerId, Guid itemId, ItemType itemType, int sourceSlotIndex, int targetSlotIndex)
        {
            OwnerId = ownerId;
            ItemId = itemId;
            ItemType = itemType;
            SourceSlotIndex = sourceSlotIndex;
            TargetSlotIndex = targetSlotIndex;
        }
    }

    /// <summary>
    /// 背包容量变更事件参数
    /// </summary>
    public class InventoryCapacityChangedEventArgs : EventArgs
    {
        /// <summary>
        /// 背包拥有者ID
        /// </summary>
        public Guid OwnerId { get; }

        /// <summary>
        /// 新容量
        /// </summary>
        public int NewCapacity { get; }

        /// <summary>
        /// 旧容量
        /// </summary>
        public int OldCapacity { get; }

        /// <summary>
        /// 变更原因
        /// </summary>
        public string Reason { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ownerId">背包拥有者ID</param>
        /// <param name="newCapacity">新容量</param>
        /// <param name="oldCapacity">旧容量</param>
        /// <param name="reason">变更原因</param>
        public InventoryCapacityChangedEventArgs(Guid ownerId, int newCapacity, int oldCapacity, string reason = "")
        {
            OwnerId = ownerId;
            NewCapacity = newCapacity;
            OldCapacity = oldCapacity;
            Reason = reason;
        }
    }

    /// <summary>
    /// 物品转移事件参数
    /// </summary>
    public class ItemTransferredEventArgs : EventArgs
    {
        /// <summary>
        /// 源背包拥有者ID
        /// </summary>
        public Guid SourceOwnerId { get; }

        /// <summary>
        /// 目标背包拥有者ID
        /// </summary>
        public Guid TargetOwnerId { get; }

        /// <summary>
        /// 物品ID
        /// </summary>
        public Guid ItemId { get; }

        /// <summary>
        /// 物品类型
        /// </summary>
        public ItemType ItemType { get; }

        /// <summary>
        /// 数量
        /// </summary>
        public int Quantity { get; }

        /// <summary>
        /// 源背包位置
        /// </summary>
        public int SourceSlotIndex { get; }

        /// <summary>
        /// 目标背包位置
        /// </summary>
        public int? TargetSlotIndex { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="sourceOwnerId">源背包拥有者ID</param>
        /// <param name="targetOwnerId">目标背包拥有者ID</param>
        /// <param name="itemId">物品ID</param>
        /// <param name="itemType">物品类型</param>
        /// <param name="quantity">数量</param>
        /// <param name="sourceSlotIndex">源背包位置</param>
        /// <param name="targetSlotIndex">目标背包位置</param>
        public ItemTransferredEventArgs(Guid sourceOwnerId, Guid targetOwnerId, Guid itemId, ItemType itemType, int quantity, int sourceSlotIndex, int? targetSlotIndex = null)
        {
            SourceOwnerId = sourceOwnerId;
            TargetOwnerId = targetOwnerId;
            ItemId = itemId;
            ItemType = itemType;
            Quantity = quantity;
            SourceSlotIndex = sourceSlotIndex;
            TargetSlotIndex = targetSlotIndex;
        }
    }
}