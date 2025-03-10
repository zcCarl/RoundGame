using System;

namespace TacticalRPG.Core.Modules.Item
{
    /// <summary>
    /// 物品位置信息
    /// </summary>
    public struct ItemLocation : IEquatable<ItemLocation>
    {
        /// <summary>
        /// 容器类型
        /// </summary>
        public ItemContainerType ContainerType { get; set; }

        /// <summary>
        /// 容器ID
        /// </summary>
        public Guid ContainerId { get; set; }

        /// <summary>
        /// 所有者ID
        /// </summary>
        public Guid OwnerId { get; set; }

        /// <summary>
        /// 在容器中的槽位索引
        /// </summary>
        public int SlotIndex { get; set; }

        /// <summary>
        /// 未分配的位置
        /// </summary>
        public static ItemLocation Unassigned => new ItemLocation
        {
            ContainerType = ItemContainerType.None,
            ContainerId = Guid.Empty,
            OwnerId = Guid.Empty,
            SlotIndex = -1
        };

        /// <summary>
        /// 创建背包位置
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <param name="slotIndex">槽位索引</param>
        /// <returns>物品位置</returns>
        public static ItemLocation Inventory(Guid characterId, int slotIndex) => new ItemLocation
        {
            ContainerType = ItemContainerType.Inventory,
            ContainerId = characterId,
            OwnerId = characterId,
            SlotIndex = slotIndex
        };

        /// <summary>
        /// 创建装备位置
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <param name="slotIndex">装备槽位索引</param>
        /// <returns>物品位置</returns>
        public static ItemLocation Equipment(Guid characterId, int slotIndex) => new ItemLocation
        {
            ContainerType = ItemContainerType.Equipment,
            ContainerId = characterId,
            OwnerId = characterId,
            SlotIndex = slotIndex
        };

        /// <summary>
        /// 创建快捷栏位置
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <param name="slotIndex">快捷栏槽位索引</param>
        /// <returns>物品位置</returns>
        public static ItemLocation Shortcut(Guid characterId, int slotIndex) => new ItemLocation
        {
            ContainerType = ItemContainerType.Shortcut,
            ContainerId = characterId,
            OwnerId = characterId,
            SlotIndex = slotIndex
        };

        /// <summary>
        /// 创建商店位置
        /// </summary>
        /// <param name="shopId">商店ID</param>
        /// <param name="slotIndex">商店槽位索引</param>
        /// <returns>物品位置</returns>
        public static ItemLocation Shop(Guid shopId, int slotIndex) => new ItemLocation
        {
            ContainerType = ItemContainerType.Shop,
            ContainerId = shopId,
            OwnerId = Guid.Empty,
            SlotIndex = slotIndex
        };

        /// <summary>
        /// 创建容器位置
        /// </summary>
        /// <param name="containerId">容器ID</param>
        /// <param name="slotIndex">容器槽位索引</param>
        /// <returns>物品位置</returns>
        public static ItemLocation Container(Guid containerId, int slotIndex) => new ItemLocation
        {
            ContainerType = ItemContainerType.Container,
            ContainerId = containerId,
            OwnerId = Guid.Empty,
            SlotIndex = slotIndex
        };

        /// <summary>
        /// 创建地面位置
        /// </summary>
        /// <param name="mapId">地图ID</param>
        /// <returns>物品位置</returns>
        public static ItemLocation Ground(Guid mapId) => new ItemLocation
        {
            ContainerType = ItemContainerType.Ground,
            ContainerId = mapId,
            OwnerId = Guid.Empty,
            SlotIndex = -1
        };

        /// <summary>
        /// 比较两个物品位置是否相等
        /// </summary>
        /// <param name="other">另一个物品位置</param>
        /// <returns>是否相等</returns>
        public bool Equals(ItemLocation other)
        {
            return ContainerType == other.ContainerType &&
                   ContainerId == other.ContainerId &&
                   OwnerId == other.OwnerId &&
                   SlotIndex == other.SlotIndex;
        }

        /// <summary>
        /// 比较对象是否相等
        /// </summary>
        /// <param name="obj">另一个对象</param>
        /// <returns>是否相等</returns>
        public override bool Equals(object obj)
        {
            return obj is ItemLocation location && Equals(location);
        }

        /// <summary>
        /// 获取哈希码
        /// </summary>
        /// <returns>哈希码</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(ContainerType, ContainerId, OwnerId, SlotIndex);
        }

        /// <summary>
        /// 相等运算符
        /// </summary>
        /// <param name="left">左操作数</param>
        /// <param name="right">右操作数</param>
        /// <returns>是否相等</returns>
        public static bool operator ==(ItemLocation left, ItemLocation right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// 不等运算符
        /// </summary>
        /// <param name="left">左操作数</param>
        /// <param name="right">右操作数</param>
        /// <returns>是否不等</returns>
        public static bool operator !=(ItemLocation left, ItemLocation right)
        {
            return !(left == right);
        }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns>字符串表示</returns>
        public override string ToString()
        {
            return $"{{容器类型:{ContainerType}, 容器ID:{ContainerId}, 所有者ID:{OwnerId}, 槽位:{SlotIndex}}}";
        }
    }
}