using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TacticalRPG.Core.Modules.Item
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
        Dictionary<string, IItemTemplate> ItemTemplates { get; }

        /// <summary>
        /// 注册物品模板
        /// </summary>
        /// <param name="templateId">模板ID</param>
        /// <param name="itemTemplate">物品模板</param>
        void RegisterItemTemplate(string templateId, IItemTemplate itemTemplate);

        /// <summary>
        /// 获取物品模板
        /// </summary>
        /// <param name="templateId">模板ID</param>
        /// <returns>物品模板</returns>
        IItemTemplate GetItemTemplate(string templateId);

        /// <summary>
        /// 创建物品实例
        /// </summary>
        /// <param name="templateId">模板ID</param>
        /// <param name="stackSize">堆叠数量</param>
        /// <returns>创建的物品实例</returns>
        IItem CreateItem(string templateId, int stackSize = 1);

        /// <summary>
        /// 查找物品
        /// </summary>
        /// <param name="itemId">物品ID</param>
        /// <returns>物品实例</returns>
        IItem FindItem(Guid itemId);

        /// <summary>
        /// 根据模板ID查找物品
        /// </summary>
        /// <param name="templateId">模板ID</param>
        /// <returns>物品实例列表</returns>
        List<IItem> FindItemsByTemplate(string templateId);

        /// <summary>
        /// 根据所有者ID查找物品
        /// </summary>
        /// <param name="ownerId">所有者ID</param>
        /// <returns>物品ID列表</returns>
        List<Guid> GetItemsByOwner(Guid ownerId);

        /// <summary>
        /// 使用物品
        /// </summary>
        /// <param name="itemId">物品ID</param>
        /// <param name="characterId">角色ID</param>
        /// <param name="targetId">目标ID</param>
        /// <returns>是否使用成功</returns>
        bool UseItem(Guid itemId, Guid characterId, Guid? targetId = null);

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
        /// 获取物品位置
        /// </summary>
        /// <param name="itemId">物品ID</param>
        /// <returns>物品位置</returns>
        ItemLocation GetItemLocation(Guid itemId);

        /// <summary>
        /// 更新物品位置
        /// </summary>
        /// <param name="itemId">物品ID</param>
        /// <param name="location">新位置</param>
        /// <returns>是否更新成功</returns>
        bool UpdateItemLocation(Guid itemId, ItemLocation location);

        /// <summary>
        /// 删除物品
        /// </summary>
        /// <param name="itemId">物品ID</param>
        /// <returns>是否删除成功</returns>
        bool DeleteItem(Guid itemId);

        /// <summary>
        /// 获取物品属性
        /// </summary>
        /// <param name="itemId">物品ID</param>
        /// <param name="key">属性键</param>
        /// <returns>属性值</returns>
        object GetItemAttribute(Guid itemId, string key);

        /// <summary>
        /// 设置物品属性
        /// </summary>
        /// <param name="itemId">物品ID</param>
        /// <param name="key">属性键</param>
        /// <param name="value">属性值</param>
        /// <returns>是否设置成功</returns>
        bool SetItemAttribute(Guid itemId, string key, object value);

        /// <summary>
        /// 设置物品堆叠数量
        /// </summary>
        /// <param name="itemId">物品ID</param>
        /// <param name="stackSize">堆叠数量</param>
        /// <returns>是否设置成功</returns>
        bool SetItemStackSize(Guid itemId, int stackSize);

        /// <summary>
        /// 拆分物品堆叠
        /// </summary>
        /// <param name="itemId">物品ID</param>
        /// <param name="amount">拆分数量</param>
        /// <returns>拆分后的新物品ID</returns>
        Guid SplitItemStack(Guid itemId, int amount);

        /// <summary>
        /// 合并物品堆叠
        /// </summary>
        /// <param name="sourceItemId">源物品ID</param>
        /// <param name="targetItemId">目标物品ID</param>
        /// <returns>是否合并成功</returns>
        bool MergeItemStacks(Guid sourceItemId, Guid targetItemId);

        /// <summary>
        /// 清除所有物品模板
        /// </summary>
        void ClearItemTemplates();
    }
}