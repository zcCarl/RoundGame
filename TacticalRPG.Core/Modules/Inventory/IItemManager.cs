using System;
using System.Collections.Generic;

namespace TacticalRPG.Core.Modules.Inventory
{
    /// <summary>
    /// 物品管理器接口，定义物品管理的全局功能
    /// </summary>
    public interface IItemManager
    {
        /// <summary>
        /// 物品工厂实例
        /// </summary>
        IItemFactory ItemFactory { get; }

        /// <summary>
        /// 注册的物品模板
        /// </summary>
        Dictionary<string, IItem> ItemTemplates { get; }

        /// <summary>
        /// 注册的库存列表
        /// </summary>
        Dictionary<Guid, IInventory> Inventories { get; }

        /// <summary>
        /// 注册物品模板
        /// </summary>
        /// <param name="templateId">模板ID</param>
        /// <param name="itemTemplate">物品模板</param>
        void RegisterItemTemplate(string templateId, IItem itemTemplate);

        /// <summary>
        /// 获取物品模板
        /// </summary>
        /// <param name="templateId">模板ID</param>
        /// <returns>物品模板</returns>
        IItem GetItemTemplate(string templateId);

        /// <summary>
        /// 创建物品实例
        /// </summary>
        /// <param name="templateId">模板ID</param>
        /// <param name="stackSize">堆叠数量</param>
        /// <returns>创建的物品实例</returns>
        IItem CreateItem(string templateId, int stackSize = 1);

        /// <summary>
        /// 注册库存
        /// </summary>
        /// <param name="inventory">库存实例</param>
        void RegisterInventory(IInventory inventory);

        /// <summary>
        /// 注销库存
        /// </summary>
        /// <param name="inventoryId">库存ID</param>
        void UnregisterInventory(Guid inventoryId);

        /// <summary>
        /// 获取库存
        /// </summary>
        /// <param name="inventoryId">库存ID</param>
        /// <returns>库存实例</returns>
        IInventory GetInventory(Guid inventoryId);

        /// <summary>
        /// 获取角色的所有库存
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <returns>库存列表</returns>
        List<IInventory> GetCharacterInventories(Guid characterId);

        /// <summary>
        /// 创建新库存
        /// </summary>
        /// <param name="ownerId">所有者ID</param>
        /// <param name="name">库存名称</param>
        /// <param name="capacity">容量</param>
        /// <param name="inventoryType">库存类型</param>
        /// <param name="maxWeight">最大重量</param>
        /// <returns>创建的库存实例</returns>
        IInventory CreateInventory(
            Guid ownerId,
            string name,
            int capacity,
            InventoryType inventoryType = InventoryType.Normal,
            float maxWeight = 0);

        /// <summary>
        /// 在两个库存之间移动物品
        /// </summary>
        /// <param name="sourceInventoryId">源库存ID</param>
        /// <param name="sourceSlotIndex">源槽索引</param>
        /// <param name="targetInventoryId">目标库存ID</param>
        /// <param name="targetSlotIndex">目标槽索引</param>
        /// <param name="amount">移动数量</param>
        /// <returns>是否移动成功</returns>
        bool MoveItemBetweenInventories(
            Guid sourceInventoryId,
            int sourceSlotIndex,
            Guid targetInventoryId,
            int targetSlotIndex,
            int amount = 0);

        /// <summary>
        /// 交换两个库存中的物品
        /// </summary>
        /// <param name="firstInventoryId">第一个库存ID</param>
        /// <param name="firstSlotIndex">第一个槽索引</param>
        /// <param name="secondInventoryId">第二个库存ID</param>
        /// <param name="secondSlotIndex">第二个槽索引</param>
        /// <returns>是否交换成功</returns>
        bool SwapItemsBetweenInventories(
            Guid firstInventoryId,
            int firstSlotIndex,
            Guid secondInventoryId,
            int secondSlotIndex);

        /// <summary>
        /// 使用物品
        /// </summary>
        /// <param name="inventoryId">库存ID</param>
        /// <param name="slotIndex">槽索引</param>
        /// <param name="targetId">目标ID</param>
        /// <returns>是否使用成功</returns>
        bool UseItem(Guid inventoryId, int slotIndex, Guid targetId);

        /// <summary>
        /// 丢弃物品
        /// </summary>
        /// <param name="inventoryId">库存ID</param>
        /// <param name="slotIndex">槽索引</param>
        /// <param name="amount">丢弃数量</param>
        /// <returns>丢弃的物品</returns>
        IItem DropItem(Guid inventoryId, int slotIndex, int amount = 0);

        /// <summary>
        /// 加载物品数据
        /// </summary>
        /// <param name="itemData">物品数据</param>
        /// <returns>加载的物品</returns>
        IItem LoadItemData(string itemData);

        /// <summary>
        /// 保存物品数据
        /// </summary>
        /// <param name="item">物品实例</param>
        /// <returns>保存的物品数据</returns>
        string SaveItemData(IItem item);

        /// <summary>
        /// 加载库存数据
        /// </summary>
        /// <param name="inventoryData">库存数据</param>
        /// <returns>加载的库存</returns>
        IInventory LoadInventoryData(string inventoryData);

        /// <summary>
        /// 保存库存数据
        /// </summary>
        /// <param name="inventory">库存实例</param>
        /// <returns>保存的库存数据</returns>
        string SaveInventoryData(IInventory inventory);

        /// <summary>
        /// 获取物品的全局唯一ID
        /// </summary>
        /// <param name="item">物品实例</param>
        /// <returns>物品的全局唯一ID</returns>
        string GetItemGlobalId(IItem item);

        /// <summary>
        /// 根据全局唯一ID查找物品
        /// </summary>
        /// <param name="globalId">全局唯一ID</param>
        /// <returns>物品实例和所在库存信息的元组</returns>
        (IInventory Inventory, int SlotIndex, IItem Item) FindItemByGlobalId(string globalId);
    }
}