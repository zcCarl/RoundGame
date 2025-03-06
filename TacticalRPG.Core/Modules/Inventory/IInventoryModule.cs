using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TacticalRPG.Core.Framework;

namespace TacticalRPG.Core.Modules.Inventory
{
    /// <summary>
    /// 物品背包模块接口，负责管理游戏中的所有物品和背包
    /// </summary>
    public interface IInventoryModule
    {
        /// <summary>
        /// 获取物品管理器
        /// </summary>
        IItemManager ItemManager { get; }

        /// <summary>
        /// 创建物品
        /// </summary>
        /// <param name="templateId">物品模板ID</param>
        /// <param name="stackSize">堆叠数量</param>
        /// <returns>创建的物品</returns>
        IItem CreateItem(string templateId, int stackSize = 1);

        /// <summary>
        /// 创建角色背包
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <param name="capacity">容量</param>
        /// <param name="inventoryType">背包类型</param>
        /// <returns>创建的背包</returns>
        Task<IInventory> CreateCharacterInventoryAsync(Guid characterId, int capacity, InventoryType inventoryType = InventoryType.Normal);

        /// <summary>
        /// 获取角色的主背包
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <returns>角色的主背包</returns>
        Task<IInventory> GetCharacterMainInventoryAsync(Guid characterId);

        /// <summary>
        /// 获取角色的所有背包
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <returns>角色的所有背包</returns>
        Task<IReadOnlyList<IInventory>> GetCharacterInventoriesAsync(Guid characterId);

        /// <summary>
        /// 给角色添加物品
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <param name="templateId">物品模板ID</param>
        /// <param name="amount">数量</param>
        /// <param name="inventoryType">目标背包类型</param>
        /// <returns>是否成功添加</returns>
        Task<bool> AddItemToCharacterAsync(Guid characterId, string templateId, int amount = 1, InventoryType inventoryType = InventoryType.Normal);

        /// <summary>
        /// 给角色添加物品
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <param name="item">物品实例</param>
        /// <param name="inventoryType">目标背包类型</param>
        /// <returns>是否成功添加</returns>
        Task<bool> AddItemToCharacterAsync(Guid characterId, IItem item, InventoryType inventoryType = InventoryType.Normal);

        /// <summary>
        /// 从角色背包移除物品
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <param name="itemId">物品ID</param>
        /// <param name="amount">数量</param>
        /// <returns>是否成功移除</returns>
        Task<bool> RemoveItemFromCharacterAsync(Guid characterId, Guid itemId, int amount = 1);

        /// <summary>
        /// 检查角色是否拥有物品
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <param name="templateId">物品模板ID</param>
        /// <param name="amount">数量</param>
        /// <returns>是否拥有</returns>
        Task<bool> CharacterHasItemAsync(Guid characterId, string templateId, int amount = 1);

        /// <summary>
        /// 角色使用物品
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <param name="itemId">物品ID</param>
        /// <param name="targetId">目标ID</param>
        /// <returns>使用结果</returns>
        Task<(bool success, string message)> UseItemAsync(Guid characterId, Guid itemId, Guid? targetId = null);

        /// <summary>
        /// 创建掉落物
        /// </summary>
        /// <param name="position">位置坐标</param>
        /// <param name="items">物品列表</param>
        /// <returns>掉落物ID</returns>
        Task<Guid> CreateDropAsync((int x, int y) position, IReadOnlyList<IItem> items);

        /// <summary>
        /// the loot drop
        /// </summary>
        /// <param name="dropId">掉落物ID</param>
        /// <param name="characterId">拾取角色ID</param>
        /// <returns>拾取结果</returns>
        Task<(bool success, string message)> PickupDropAsync(Guid dropId, Guid characterId);

        /// <summary>
        /// 注册物品模板
        /// </summary>
        /// <param name="templateId">模板ID</param>
        /// <param name="itemTemplate">物品模板</param>
        void RegisterItemTemplate(string templateId, IItem itemTemplate);

        /// <summary>
        /// 批量注册物品模板
        /// </summary>
        /// <param name="templates">模板字典</param>
        void RegisterItemTemplates(IDictionary<string, IItem> templates);

        /// <summary>
        /// 加载物品数据
        /// </summary>
        /// <param name="data">物品数据</param>
        /// <returns>加载结果</returns>
        Task<bool> LoadItemDataAsync(string data);

        /// <summary>
        /// 保存物品数据
        /// </summary>
        /// <returns>保存的数据</returns>
        Task<string> SaveItemDataAsync();
    }
}
