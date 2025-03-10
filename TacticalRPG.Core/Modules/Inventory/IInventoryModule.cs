using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TacticalRPG.Core.Framework;
using TacticalRPG.Core.Modules.Item;

namespace TacticalRPG.Core.Modules.Inventory
{
    /// <summary>
    /// 物品背包模块接口，负责管理游戏中的所有背包
    /// </summary>
    public interface IInventoryModule : IGameModule
    {
        /// <summary>
        /// 获取物品模块引用
        /// </summary>
        IItemModule ItemModule { get; }

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
        /// <returns>添加结果，成功返回true，失败返回false</returns>
        Task<bool> AddItemToCharacterAsync(Guid characterId, string templateId, int amount = 1, InventoryType inventoryType = InventoryType.Normal);

        /// <summary>
        /// 给角色添加物品
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <param name="itemId">物品ID</param>
        /// <param name="inventoryType">目标背包类型</param>
        /// <returns>添加结果，成功返回true，失败返回false</returns>
        Task<bool> AddItemToCharacterAsync(Guid characterId, Guid itemId, InventoryType inventoryType = InventoryType.Normal);

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
        /// 检查角色是否拥有足够数量的物品(按物品实例ID)
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <param name="itemId">物品实例ID</param>
        /// <param name="count">数量</param>
        /// <returns>是否拥有足够数量</returns>
        Task<bool> HasEnoughItemsByIdAsync(Guid characterId, Guid itemId, int count);

        /// <summary>
        /// 检查角色是否拥有足够数量的物品(按物品模板ID)
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <param name="templateId">物品模板ID</param>
        /// <param name="count">数量</param>
        /// <returns>是否拥有足够数量</returns>
        Task<bool> HasEnoughItemsByTemplateIdAsync(Guid characterId, string templateId, int count);

        /// <summary>
        /// 获取物品实例ID
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <param name="itemId">物品实例ID</param>
        /// <returns>物品实例ID</returns>
        Task<Guid> GetItemAsync(Guid characterId, Guid itemId);

        /// <summary>
        /// 根据模板ID获取物品实例ID
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <param name="templateId">物品模板ID</param>
        /// <returns>物品实例ID</returns>
        Task<Guid> GetItemByTemplateIdAsync(Guid characterId, string templateId);

        /// <summary>
        /// 背包管理相关API
        /// </summary>
        Task<Guid> CreateInventoryAsync(Guid ownerId, InventoryType type, int capacity);
        Task<bool> AddItemAsync(Guid inventoryId, Guid itemId, int? slotIndex = null);
        Task<bool> RemoveItemAsync(Guid inventoryId, Guid itemId, int amount = 1);
        Task<IReadOnlyList<Guid>> GetInventoryItemsAsync(Guid inventoryId);
        Task<bool> HasSpaceForItemAsync(Guid inventoryId, Guid itemId);
        Task<bool> TransferItemAsync(Guid sourceInventoryId, Guid targetInventoryId, Guid itemId);
        Task<bool> MoveItemAsync(Guid inventoryId, int fromSlotIndex, int toSlotIndex);
        Task<bool> SwapItemsAsync(Guid inventoryId, int slotIndex1, int slotIndex2);
        Task<bool> SortInventoryAsync(Guid inventoryId);

        /// <summary>
        /// 加载背包数据
        /// </summary>
        /// <param name="data">背包数据</param>
        /// <returns>加载结果</returns>
        Task<bool> LoadInventoryDataAsync(string data);

        /// <summary>
        /// 保存背包数据
        /// </summary>
        /// <returns>保存的数据</returns>
        Task<string> SaveInventoryDataAsync();
    }
}