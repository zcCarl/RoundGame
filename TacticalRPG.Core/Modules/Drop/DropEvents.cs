using System;
using System.Collections.Generic;
using TacticalRPG.Core.Framework;

namespace TacticalRPG.Core.Modules.Drop
{
    /// <summary>
    /// 掉落物创建事件
    /// </summary>
    public class DropCreatedEvent : GameEvent
    {
        /// <summary>
        /// 掉落物ID
        /// </summary>
        public Guid DropId { get; }

        /// <summary>
        /// 掉落物位置X坐标
        /// </summary>
        public int X { get; }

        /// <summary>
        /// 掉落物位置Y坐标
        /// </summary>
        public int Y { get; }

        /// <summary>
        /// 掉落物包含的物品ID列表
        /// </summary>
        public IReadOnlyList<Guid> ItemIds { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dropId">掉落物ID</param>
        /// <param name="x">X坐标</param>
        /// <param name="y">Y坐标</param>
        /// <param name="itemIds">物品ID列表</param>
        public DropCreatedEvent(Guid dropId, int x, int y, IReadOnlyList<Guid> itemIds)
            : base($"掉落物 {dropId} 被创建")
        {
            DropId = dropId;
            X = x;
            Y = y;
            ItemIds = itemIds;
        }
    }

    /// <summary>
    /// 掉落物被拾取事件
    /// </summary>
    public class DropPickedUpEvent : GameEvent
    {
        /// <summary>
        /// 掉落物ID
        /// </summary>
        public Guid DropId { get; }

        /// <summary>
        /// 拾取角色ID
        /// </summary>
        public Guid CharacterId { get; }

        /// <summary>
        /// 物品ID列表
        /// </summary>
        public IReadOnlyList<Guid> ItemIds { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dropId">掉落物ID</param>
        /// <param name="characterId">角色ID</param>
        /// <param name="itemIds">物品ID列表</param>
        public DropPickedUpEvent(Guid dropId, Guid characterId, IReadOnlyList<Guid> itemIds)
            : base($"掉落物 {dropId} 被角色 {characterId} 拾取")
        {
            DropId = dropId;
            CharacterId = characterId;
            ItemIds = itemIds;
        }
    }

    /// <summary>
    /// 掉落物移除事件
    /// </summary>
    public class DropRemovedEvent : GameEvent
    {
        /// <summary>
        /// 掉落物ID
        /// </summary>
        public Guid DropId { get; }

        /// <summary>
        /// 移除原因
        /// </summary>
        public string Reason { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dropId">掉落物ID</param>
        /// <param name="reason">移除原因</param>
        public DropRemovedEvent(Guid dropId, string reason)
            : base($"掉落物 {dropId} 被移除，原因: {reason}")
        {
            DropId = dropId;
            Reason = reason;
        }
    }

    /// <summary>
    /// 掉落表注册事件
    /// </summary>
    public class LootTableRegisteredEvent : GameEvent
    {
        /// <summary>
        /// 掉落表ID
        /// </summary>
        public string LootTableId { get; }

        /// <summary>
        /// 物品条目数量
        /// </summary>
        public int EntriesCount { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="lootTableId">掉落表ID</param>
        /// <param name="entriesCount">物品条目数量</param>
        public LootTableRegisteredEvent(string lootTableId, int entriesCount)
            : base($"掉落表 {lootTableId} 已注册，包含 {entriesCount} 个物品条目")
        {
            LootTableId = lootTableId;
            EntriesCount = entriesCount;
        }
    }
}