using System;
using System.Collections.Generic;

namespace TacticalRPG.Core.Modules.Inventory
{
    /// <summary>
    /// 掉落物接口
    /// </summary>
    public interface IDrop
    {
        /// <summary>
        /// 掉落物唯一ID
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// 掉落物在地图上的位置坐标X
        /// </summary>
        int X { get; }

        /// <summary>
        /// 掉落物在地图上的位置坐标Y
        /// </summary>
        int Y { get; }

        /// <summary>
        /// 掉落时间
        /// </summary>
        DateTime DropTime { get; }

        /// <summary>
        /// 存在时间（秒），0表示永久存在
        /// </summary>
        int ExistDuration { get; }

        /// <summary>
        /// 是否已被拾取
        /// </summary>
        bool IsPickedUp { get; }

        /// <summary>
        /// 拾取者ID（如果已被拾取）
        /// </summary>
        Guid? PickerId { get; }

        /// <summary>
        /// 拾取时间（如果已被拾取）
        /// </summary>
        DateTime? PickUpTime { get; }

        /// <summary>
        /// 掉落物包含的物品列表
        /// </summary>
        IReadOnlyList<IItem> Items { get; }

        /// <summary>
        /// 掉落物的图标
        /// </summary>
        string IconPath { get; }

        /// <summary>
        /// 掉落物的描述
        /// </summary>
        string Description { get; }

        /// <summary>
        /// 掉落物的名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 设置掉落物位置
        /// </summary>
        /// <param name="x">X坐标</param>
        /// <param name="y">Y坐标</param>
        void SetPosition(int x, int y);

        /// <summary>
        /// 设置存在时间
        /// </summary>
        /// <param name="duration">存在时间（秒）</param>
        void SetExistDuration(int duration);

        /// <summary>
        /// 添加物品到掉落物
        /// </summary>
        /// <param name="item">物品</param>
        void AddItem(IItem item);

        /// <summary>
        /// 移除物品
        /// </summary>
        /// <param name="itemIndex">物品索引</param>
        /// <returns>移除的物品</returns>
        IItem RemoveItem(int itemIndex);

        /// <summary>
        /// 标记为已拾取
        /// </summary>
        /// <param name="pickerId">拾取者ID</param>
        void MarkAsPickedUp(Guid pickerId);

        /// <summary>
        /// 是否可以被拾取
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <returns>检查结果</returns>
        (bool canPickup, string reason) CanBePickedUpBy(Guid characterId);

        /// <summary>
        /// 是否已过期
        /// </summary>
        bool IsExpired();

        /// <summary>
        /// 是否为空（没有物品）
        /// </summary>
        bool IsEmpty();
    }
}