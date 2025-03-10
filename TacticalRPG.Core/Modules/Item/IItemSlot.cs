using System;

namespace TacticalRPG.Core.Modules.Item
{
    /// <summary>
    /// 物品槽接口，定义物品槽的基本属性和方法
    /// </summary>
    public interface IItemSlot
    {
        /// <summary>
        /// 物品槽索引
        /// </summary>
        int Index { get; }

        /// <summary>
        /// 物品槽中的物品
        /// </summary>
        IItem Item { get; }

        /// <summary>
        /// 物品槽是否为空
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// 物品槽是否被锁定
        /// </summary>
        bool IsLocked { get; }

        /// <summary>
        /// 物品槽类型（可用于限制特定类型物品）
        /// </summary>
        string SlotType { get; }

        /// <summary>
        /// 物品槽所属的库存ID
        /// </summary>
        Guid InventoryId { get; }

        /// <summary>
        /// 设置物品槽中的物品
        /// </summary>
        /// <param name="item">要设置的物品</param>
        /// <returns>是否设置成功</returns>
        bool SetItem(IItem item);

        /// <summary>
        /// 移除物品槽中的物品
        /// </summary>
        /// <returns>移除的物品</returns>
        IItem RemoveItem();

        /// <summary>
        /// 清空物品槽
        /// </summary>
        void Clear();

        /// <summary>
        /// 锁定物品槽
        /// </summary>
        /// <param name="locked">是否锁定</param>
        void SetLocked(bool locked);

        /// <summary>
        /// 设置物品槽类型
        /// </summary>
        /// <param name="slotType">槽类型</param>
        void SetSlotType(string slotType);

        /// <summary>
        /// 检查物品是否可以放入此槽
        /// </summary>
        /// <param name="item">要检查的物品</param>
        /// <returns>是否可以放入</returns>
        bool CanAcceptItem(IItem item);

        /// <summary>
        /// 尝试添加物品到槽中
        /// </summary>
        /// <param name="item">要添加的物品</param>
        /// <returns>添加后剩余的物品（如果完全添加则为null）</returns>
        IItem TryAddItem(IItem item);

        /// <summary>
        /// 尝试从槽中移除指定数量的物品
        /// </summary>
        /// <param name="amount">要移除的数量</param>
        /// <returns>移除的物品</returns>
        IItem TryRemoveItem(int amount);

        /// <summary>
        /// 交换物品槽中的物品
        /// </summary>
        /// <param name="otherSlot">另一个物品槽</param>
        /// <returns>是否交换成功</returns>
        bool SwapWith(IItemSlot otherSlot);

        /// <summary>
        /// 获取物品槽的描述信息
        /// </summary>
        /// <returns>描述信息</returns>
        string GetDescription();
    }
}