using System;

namespace TacticalRPG.Core.Modules.Inventory
{
    /// <summary>
    /// 定义背包槽的接口
    /// </summary>
    public interface IInventorySlot
    {
        /// <summary>
        /// 获取槽位索引
        /// </summary>
        int Index { get; }

        /// <summary>
        /// 获取或设置槽位中的物品
        /// </summary>
        IItem Item { get; set; }

        /// <summary>
        /// 获取槽位是否为空
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// 获取槽位是否已锁定
        /// </summary>
        bool IsLocked { get; }

        /// <summary>
        /// 获取槽位可接受的物品类型（如为null则接受所有类型）
        /// </summary>
        ItemType? AcceptedItemType { get; }

        /// <summary>
        /// 获取槽位的标签/分类
        /// </summary>
        string Label { get; }

        /// <summary>
        /// 添加物品到槽位
        /// </summary>
        /// <param name="item">要添加的物品</param>
        /// <returns>成功添加返回true，否则返回false</returns>
        bool AddItem(IItem item);

        /// <summary>
        /// 添加指定数量的物品到槽位
        /// </summary>
        /// <param name="item">要添加的物品</param>
        /// <param name="amount">添加数量</param>
        /// <returns>实际添加的数量</returns>
        int AddItem(IItem item, int amount);

        /// <summary>
        /// 从槽位移除物品
        /// </summary>
        /// <returns>移除的物品，如果槽位为空则返回null</returns>
        IItem RemoveItem();

        /// <summary>
        /// 从槽位移除指定数量的物品
        /// </summary>
        /// <param name="amount">移除数量</param>
        /// <returns>移除的物品，包含指定数量</returns>
        IItem RemoveItem(int amount);

        /// <summary>
        /// 锁定/解锁槽位
        /// </summary>
        /// <param name="locked">是否锁定</param>
        /// <returns>设置是否成功</returns>
        bool SetLocked(bool locked);

        /// <summary>
        /// 设置槽位接受的物品类型
        /// </summary>
        /// <param name="itemType">物品类型，null表示接受所有类型</param>
        /// <returns>设置是否成功</returns>
        bool SetAcceptedItemType(ItemType? itemType);

        /// <summary>
        /// 设置槽位标签
        /// </summary>
        /// <param name="label">标签文本</param>
        void SetLabel(string label);

        /// <summary>
        /// 检查槽位是否可以接受指定物品
        /// </summary>
        /// <param name="item">要检查的物品</param>
        /// <returns>是否可接受</returns>
        bool CanAcceptItem(IItem item);

        /// <summary>
        /// 检查槽位是否可以接受指定类型的物品
        /// </summary>
        /// <param name="itemType">物品类型</param>
        /// <returns>是否可接受</returns>
        bool CanAcceptItemType(ItemType itemType);

        /// <summary>
        /// 获取槽位的自定义属性
        /// </summary>
        /// <param name="key">属性键</param>
        /// <returns>属性值，如不存在则返回null</returns>
        object GetProperty(string key);

        /// <summary>
        /// 设置槽位的自定义属性
        /// </summary>
        /// <param name="key">属性键</param>
        /// <param name="value">属性值</param>
        void SetProperty(string key, object value);

        /// <summary>
        /// 尝试合并物品到槽位中的物品
        /// </summary>
        /// <param name="item">要合并的物品</param>
        /// <returns>合并后剩余的物品（如完全合并则返回null）</returns>
        IItem MergeItem(IItem item);

        /// <summary>
        /// 设置槽位中的物品
        /// </summary>
        /// <param name="item">物品实例</param>
        void SetItem(IItem item);
    }
}