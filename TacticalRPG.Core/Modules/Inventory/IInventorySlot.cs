using System;
using TacticalRPG.Core.Modules.Item;

namespace TacticalRPG.Core.Modules.Inventory
{
    /// <summary>
    /// 背包槽位接口
    /// </summary>
    public interface IInventorySlot
    {
        /// <summary>
        /// 获取槽位索引
        /// </summary>
        int SlotIndex { get; }

        /// <summary>
        /// 获取槽位是否为空
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// 获取槽位是否被锁定
        /// </summary>
        bool IsLocked { get; }

        /// <summary>
        /// 获取槽位限制的物品类型
        /// </summary>
        ItemType? AcceptedItemType { get; }

        /// <summary>
        /// 获取槽位标签
        /// </summary>
        string Label { get; }


        /// <summary>
        /// 获取槽位中的物品
        /// </summary>
        IItem Item { get; }

        /// <summary>
        /// 获取物品数量
        /// </summary>
        int Count { get; }

        /// <summary>
        /// 设置物品ID和数量
        /// </summary>
        /// <param name="item">物品</param>
        /// <param name="count">数量</param>
        /// <returns>是否设置成功</returns>
        bool SetItem(IItem item, int count);

        /// <summary>
        /// 清空槽位
        /// </summary>
        void Clear();

        /// <summary>
        /// 锁定或解锁槽位
        /// </summary>
        /// <param name="locked">是否锁定</param>
        void SetLocked(bool locked);

        /// <summary>
        /// 设置槽位接受的物品类型
        /// </summary>
        /// <param name="itemType">物品类型</param>
        bool SetAcceptedItemType(ItemType? itemType);

        /// <summary>
        /// 设置槽位标签
        /// </summary>
        /// <param name="label">标签文本</param>
        void SetLabel(string label);

        /// <summary>
        /// 增加物品数量
        /// </summary>
        /// <param name="amount">增加数量</param>
        /// <returns>实际增加的数量</returns>
        int AddCount(int amount);

        /// <summary>
        /// 减少物品数量
        /// </summary>
        /// <param name="amount">减少数量</param>
        /// <returns>实际减少的数量</returns>
        IItem RemoveItem(int amount);
        /// <summary>
        /// 移除物品引用
        /// </summary>
        /// <returns>物品ID</returns>
        IItem RemoveItem();
        /// <summary>
        /// 设置物品引用
        /// </summary>
        /// <param name="item">物品</param>
        void SetItem(IItem item);
        /// <summary>
        /// 检查槽位是否可以接受指定物品
        /// </summary>
        /// <param name="item">物品</param>
        /// <returns>是否可以接受</returns>
        bool CanAcceptItem(IItem item);
        /// <summary>
        /// 合并物品
        /// </summary>
        /// <param name="item">物品</param>
        /// <returns>合并后的物品</returns>
        IItem MergeItem(IItem item);
    }
}