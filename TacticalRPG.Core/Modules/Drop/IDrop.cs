using System;
using System.Collections.Generic;

namespace TacticalRPG.Core.Modules.Drop
{
    /// <summary>
    /// 掉落物接口
    /// </summary>
    public interface IDrop
    {
        /// <summary>
        /// 获取掉落物的唯一标识符
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// 获取掉落物的X坐标
        /// </summary>
        int X { get; }

        /// <summary>
        /// 获取掉落物的Y坐标
        /// </summary>
        int Y { get; }

        /// <summary>
        /// 获取掉落物的创建时间
        /// </summary>
        DateTime CreationTime { get; }

        /// <summary>
        /// 获取掉落物的存在时间（秒）
        /// </summary>
        int ExistDuration { get; }

        /// <summary>
        /// 获取掉落物包含的物品ID列表
        /// </summary>
        IReadOnlyList<Guid> ItemIds { get; }

        /// <summary>
        /// 获取掉落物是否已被拾取
        /// </summary>
        bool IsPickedUp { get; }

        /// <summary>
        /// 获取拾取掉落物的角色ID
        /// </summary>
        Guid? PickedUpByCharacterId { get; }

        /// <summary>
        /// 获取掉落物拾取时间
        /// </summary>
        DateTime? PickupTime { get; }

        /// <summary>
        /// 检查掉落物是否过期
        /// </summary>
        /// <returns>是否已过期</returns>
        bool IsExpired();

        /// <summary>
        /// 检查掉落物是否可以被指定角色拾取
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <returns>检查结果和原因</returns>
        (bool canPickup, string reason) CanBePickedUpBy(Guid characterId);

        /// <summary>
        /// 标记掉落物已被拾取
        /// </summary>
        /// <param name="characterId">拾取角色ID</param>
        void MarkAsPickedUp(Guid characterId);
    }
}