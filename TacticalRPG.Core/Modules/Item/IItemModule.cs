using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TacticalRPG.Core.Framework;

namespace TacticalRPG.Core.Modules.Item
{

    /// <summary>
    /// 物品模块接口
    /// </summary>
    public interface IItemModule : IGameModule
    {
        /// <summary>
        /// 创建物品
        /// </summary>
        /// <param name="templateId">物品模板ID</param>
        /// <param name="stackSize">堆叠数量</param>
        /// <returns>物品ID</returns>
        Task<Guid> CreateItemAsync(string templateId, int stackSize = 1);

        /// <summary>
        /// 获取物品实例
        /// </summary>
        /// <param name="itemId">物品ID</param>
        /// <returns>物品实例</returns>
        Task<IItem> GetItemAsync(Guid itemId);

        /// <summary>
        /// 获取物品位置信息
        /// </summary>
        /// <param name="itemId">物品ID</param>
        /// <returns>物品位置信息</returns>
        Task<ItemLocation> GetItemLocationAsync(Guid itemId);

        /// <summary>
        /// 更新物品位置信息
        /// </summary>
        /// <param name="itemId">物品ID</param>
        /// <param name="location">新的位置信息</param>
        /// <returns>是否成功更新</returns>
        Task<bool> UpdateItemLocationAsync(Guid itemId, ItemLocation location);

        /// <summary>
        /// 修改物品堆叠数量
        /// </summary>
        /// <param name="itemId">物品ID</param>
        /// <param name="newStackSize">新的堆叠数量</param>
        /// <returns>是否成功修改</returns>
        Task<bool> SetItemStackSizeAsync(Guid itemId, int newStackSize);

        /// <summary>
        /// 分割物品堆叠
        /// </summary>
        /// <param name="itemId">物品ID</param>
        /// <param name="amount">分割数量</param>
        /// <returns>新物品ID</returns>
        Task<Guid> SplitItemStackAsync(Guid itemId, int amount);

        /// <summary>
        /// 合并物品堆叠
        /// </summary>
        /// <param name="sourceItemId">源物品ID</param>
        /// <param name="targetItemId">目标物品ID</param>
        /// <returns>是否成功合并</returns>
        Task<bool> MergeItemStacksAsync(Guid sourceItemId, Guid targetItemId);

        /// <summary>
        /// 删除物品
        /// </summary>
        /// <param name="itemId">物品ID</param>
        /// <param name="reason">删除原因</param>
        /// <returns>是否成功删除</returns>
        Task<bool> DeleteItemAsync(Guid itemId, string reason = "");

        /// <summary>
        /// 使用物品
        /// </summary>
        /// <param name="itemId">物品ID</param>
        /// <param name="characterId">使用者ID</param>
        /// <param name="targetId">目标ID</param>
        /// <returns>使用结果</returns>
        Task<(bool success, string message)> UseItemAsync(Guid itemId, Guid characterId, Guid? targetId = null);

        /// <summary>
        /// 修改物品属性
        /// </summary>
        /// <param name="itemId">物品ID</param>
        /// <param name="key">属性键</param>
        /// <param name="value">属性值</param>
        /// <returns>是否成功修改</returns>
        Task<bool> SetItemAttributeAsync(Guid itemId, string key, object value);

        /// <summary>
        /// 获取物品属性
        /// </summary>
        /// <param name="itemId">物品ID</param>
        /// <param name="key">属性键</param>
        /// <returns>属性值</returns>
        Task<object> GetItemAttributeAsync(Guid itemId, string key);

        /// <summary>
        /// 获取角色所有物品
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <returns>物品ID列表</returns>
        Task<IReadOnlyList<Guid>> GetCharacterItemsAsync(Guid characterId);

        /// <summary>
        /// 保存物品数据
        /// </summary>
        /// <returns>保存的数据</returns>
        Task<string> SaveItemDataAsync();

        /// <summary>
        /// 加载物品数据
        /// </summary>
        /// <param name="data">物品数据</param>
        /// <returns>是否成功加载</returns>
        Task<bool> LoadItemDataAsync(string data);

        /// <summary>
        /// 获取指定模板ID的物品实例
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <param name="templateId">物品模板ID</param>
        /// <returns>物品ID列表</returns>
        Task<IReadOnlyList<Guid>> GetItemsByTemplateIdAsync(Guid characterId, string templateId);

        /// <summary>
        /// 注册物品模板
        /// </summary>
        /// <param name="templateId">模板ID</param>
        /// <param name="template">物品模板</param>
        void RegisterItemTemplate(string templateId, IItemTemplate template);

        /// <summary>
        /// 批量注册物品模板
        /// </summary>
        /// <param name="templates">模板字典</param>
        void RegisterItemTemplates(IDictionary<string, IItemTemplate> templates);

        /// <summary>
        /// 创建物品模板
        /// </summary>
        /// <param name="templateId">模板ID</param>
        /// <param name="name">模板名称</param>
        /// <returns>是否成功创建</returns>
        Task<bool> CreateItemTemplateAsync(string templateId, string name);

        /// <summary>
        /// 获取物品模板
        /// </summary>
        /// <param name="templateId">模板ID</param>
        /// <returns>物品模板</returns>
        Task<IItemTemplate> GetItemTemplateAsync(string templateId);

        /// <summary>
        /// 设置物品模板属性
        /// </summary>
        /// <param name="templateId">模板ID</param>
        /// <param name="key">属性键</param>
        /// <param name="value">属性值</param>
        /// <returns>是否成功设置</returns>
        Task<bool> SetTemplateAttributeAsync(string templateId, string key, object value);

        /// <summary>
        /// 获取物品模板属性
        /// </summary>
        /// <param name="templateId">模板ID</param>
        /// <param name="key">属性键</param>
        /// <returns>属性值</returns>
        Task<object> GetTemplateAttributeAsync(string templateId, string key);
    }
}