using System;
using System.Collections.Generic;

namespace TacticalRPG.Core.Modules.Inventory
{
    /// <summary>
    /// 定义背包的接口
    /// </summary>
    public interface IInventory
    {
        /// <summary>
        /// 获取背包的唯一标识符
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// 获取背包拥有者ID
        /// </summary>
        Guid OwnerId { get; }

        /// <summary>
        /// 获取背包名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 获取背包最大容量
        /// </summary>
        int Capacity { get; }

        /// <summary>
        /// 获取背包已使用的槽位数
        /// </summary>
        int UsedSlots { get; }

        /// <summary>
        /// 获取背包剩余槽位数
        /// </summary>
        int RemainingSlots { get; }

        /// <summary>
        /// 获取背包是否已满
        /// </summary>
        bool IsFull { get; }

        /// <summary>
        /// 获取背包所有槽位
        /// </summary>
        IReadOnlyList<IInventorySlot> Slots { get; }

        /// <summary>
        /// 获取背包最大重量限制（如果有）
        /// </summary>
        float? MaxWeight { get; }

        /// <summary>
        /// 获取背包当前总重量
        /// </summary>
        float CurrentWeight { get; }

        /// <summary>
        /// 获取背包类型标识
        /// </summary>
        string InventoryType { get; }

        /// <summary>
        /// 获取指定索引的槽位
        /// </summary>
        /// <param name="slotIndex">槽位索引</param>
        /// <returns>槽位实例</returns>
        IInventorySlot GetSlot(int slotIndex);

        /// <summary>
        /// 添加物品到背包
        /// </summary>
        /// <param name="item">要添加的物品</param>
        /// <param name="slotIndex">指定槽位索引（可选）</param>
        /// <returns>添加结果，如果成功则返回槽位索引，否则返回-1</returns>
        int AddItem(IItem item, int? slotIndex = null);

        /// <summary>
        /// 添加指定数量的物品到背包
        /// </summary>
        /// <param name="item">要添加的物品</param>
        /// <param name="amount">数量</param>
        /// <param name="slotIndex">指定槽位索引（可选）</param>
        /// <returns>实际添加的数量</returns>
        int AddItem(IItem item, int amount, int? slotIndex = null);

        /// <summary>
        /// 从背包移除物品
        /// </summary>
        /// <param name="slotIndex">槽位索引</param>
        /// <returns>移除的物品，如果槽位为空则返回null</returns>
        IItem RemoveItem(int slotIndex);

        /// <summary>
        /// 从背包移除指定数量的物品
        /// </summary>
        /// <param name="slotIndex">槽位索引</param>
        /// <param name="amount">数量</param>
        /// <returns>移除的物品，包含指定数量</returns>
        IItem RemoveItem(int slotIndex, int amount);

        /// <summary>
        /// 移动背包中的物品
        /// </summary>
        /// <param name="fromSlotIndex">源槽位索引</param>
        /// <param name="toSlotIndex">目标槽位索引</param>
        /// <returns>是否成功移动</returns>
        bool MoveItem(int fromSlotIndex, int toSlotIndex);

        /// <summary>
        /// 移动指定数量的物品
        /// </summary>
        /// <param name="fromSlotIndex">源槽位索引</param>
        /// <param name="toSlotIndex">目标槽位索引</param>
        /// <param name="amount">数量</param>
        /// <returns>实际移动的数量</returns>
        int MoveItem(int fromSlotIndex, int toSlotIndex, int amount);

        /// <summary>
        /// 交换两个槽位的物品
        /// </summary>
        /// <param name="slotIndex1">槽位1索引</param>
        /// <param name="slotIndex2">槽位2索引</param>
        /// <returns>是否成功交换</returns>
        bool SwapItems(int slotIndex1, int slotIndex2);

        /// <summary>
        /// 使用背包中的物品
        /// </summary>
        /// <param name="slotIndex">槽位索引</param>
        /// <param name="targetId">目标ID（可选）</param>
        /// <returns>使用结果</returns>
        (bool success, string message) UseItem(int slotIndex, Guid? targetId = null);

        /// <summary>
        /// 查找物品在背包中的位置
        /// </summary>
        /// <param name="itemTemplateId">物品模版ID</param>
        /// <returns>找到的槽位索引列表，未找到则返回空列表</returns>
        IReadOnlyList<int> FindItemSlots(string itemTemplateId);

        /// <summary>
        /// 查找特定类型的物品在背包中的位置
        /// </summary>
        /// <param name="itemType">物品类型</param>
        /// <returns>找到的槽位索引列表，未找到则返回空列表</returns>
        IReadOnlyList<int> FindItemSlotsByType(ItemType itemType);

        /// <summary>
        /// 检查背包是否包含特定物品
        /// </summary>
        /// <param name="itemTemplateId">物品模版ID</param>
        /// <returns>是否包含</returns>
        bool ContainsItem(string itemTemplateId);

        /// <summary>
        /// 检查背包是否包含指定数量的特定物品
        /// </summary>
        /// <param name="itemTemplateId">物品模版ID</param>
        /// <param name="amount">数量</param>
        /// <returns>是否包含足够数量</returns>
        bool ContainsItem(string itemTemplateId, int amount);

        /// <summary>
        /// 计算背包中指定物品的总数量
        /// </summary>
        /// <param name="itemTemplateId">物品模版ID</param>
        /// <returns>总数量</returns>
        int CountItem(string itemTemplateId);

        /// <summary>
        /// 计算背包中指定类型物品的总数量
        /// </summary>
        /// <param name="itemType">物品类型</param>
        /// <returns>总数量</returns>
        int CountItemsByType(ItemType itemType);

        /// <summary>
        /// 清空背包
        /// </summary>
        /// <param name="reason">清空原因</param>
        /// <returns>移除的物品列表</returns>
        IReadOnlyList<IItem> Clear(string reason = "");

        /// <summary>
        /// 设置背包容量
        /// </summary>
        /// <param name="capacity">新容量</param>
        /// <param name="reason">变更原因</param>
        /// <returns>是否成功设置</returns>
        bool SetCapacity(int capacity, string reason = "");

        /// <summary>
        /// 锁定/解锁槽位
        /// </summary>
        /// <param name="slotIndex">槽位索引</param>
        /// <param name="locked">是否锁定</param>
        /// <returns>是否成功设置</returns>
        bool SetSlotLocked(int slotIndex, bool locked);

        /// <summary>
        /// 设置槽位接受的物品类型
        /// </summary>
        /// <param name="slotIndex">槽位索引</param>
        /// <param name="itemType">物品类型，null表示接受所有类型</param>
        /// <returns>是否成功设置</returns>
        bool SetSlotAcceptedItemType(int slotIndex, ItemType? itemType);

        /// <summary>
        /// 整理背包
        /// </summary>
        /// <param name="sortBy">排序方式，如：类型、名称、价值等</param>
        /// <returns>是否成功整理</returns>
        bool Sort(string sortBy = "type");

        /// <summary>
        /// 使用指定的排序策略整理背包
        /// </summary>
        /// <param name="sortStrategy">排序策略</param>
        /// <returns>是否成功整理</returns>
        bool Sort(IInventorySortStrategy sortStrategy);

        /// <summary>
        /// 合并相同物品的堆叠
        /// </summary>
        /// <returns>合并的堆叠数量</returns>
        int MergeStacks();

        /// <summary>
        /// 获取背包的自定义属性
        /// </summary>
        /// <param name="key">属性键</param>
        /// <returns>属性值，如不存在则返回null</returns>
        object GetProperty(string key);

        /// <summary>
        /// 设置背包的自定义属性
        /// </summary>
        /// <param name="key">属性键</param>
        /// <param name="value">属性值</param>
        void SetProperty(string key, object value);
    }
}