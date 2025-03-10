using System;
using System.Collections.Generic;
using System.Linq;
using TacticalRPG.Core.Modules.Drop;

namespace TacticalRPG.Implementation.Modules.Drop
{
    /// <summary>
    /// 掉落物实现类
    /// </summary>
    public class Drop : IDrop
    {
        private readonly List<Guid> _itemIds = new List<Guid>();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="x">X坐标</param>
        /// <param name="y">Y坐标</param>
        /// <param name="itemIds">物品ID列表</param>
        /// <param name="existDuration">存在时间（秒），0表示永久存在</param>
        public Drop(int x, int y, IEnumerable<Guid> itemIds, int existDuration = 60)
        {
            Id = Guid.NewGuid();
            X = x;
            Y = y;
            CreationTime = DateTime.UtcNow;
            ExistDuration = existDuration;
            IsPickedUp = false;
            PickupTime = null;

            if (itemIds != null)
            {
                _itemIds.AddRange(itemIds);
            }
        }

        /// <summary>
        /// 获取掉落物的唯一标识符
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// 获取掉落物的X坐标
        /// </summary>
        public int X { get; }

        /// <summary>
        /// 获取掉落物的Y坐标
        /// </summary>
        public int Y { get; }

        /// <summary>
        /// 获取掉落物的创建时间
        /// </summary>
        public DateTime CreationTime { get; }

        /// <summary>
        /// 获取掉落物的存在时间（秒）
        /// </summary>
        public int ExistDuration { get; }

        /// <summary>
        /// 获取掉落物包含的物品ID列表
        /// </summary>
        public IReadOnlyList<Guid> ItemIds => _itemIds.AsReadOnly();

        /// <summary>
        /// 获取掉落物是否已被拾取
        /// </summary>
        public bool IsPickedUp { get; private set; }

        /// <summary>
        /// 获取拾取掉落物的角色ID
        /// </summary>
        public Guid? PickedUpByCharacterId { get; private set; }

        /// <summary>
        /// 获取掉落物拾取时间
        /// </summary>
        public DateTime? PickupTime { get; private set; }

        /// <summary>
        /// 检查掉落物是否过期
        /// </summary>
        /// <returns>是否已过期</returns>
        public bool IsExpired()
        {
            // 如果存在时间为0，表示永久存在
            if (ExistDuration <= 0)
            {
                return false;
            }

            return DateTime.UtcNow > CreationTime.AddSeconds(ExistDuration);
        }

        /// <summary>
        /// 检查掉落物是否可以被指定角色拾取
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <returns>检查结果和原因</returns>
        public (bool canPickup, string reason) CanBePickedUpBy(Guid characterId)
        {
            if (IsPickedUp)
            {
                return (false, "掉落物已被拾取");
            }

            if (IsExpired())
            {
                return (false, "掉落物已过期");
            }

            if (_itemIds.Count == 0)
            {
                return (false, "掉落物中没有物品");
            }

            // 在此可以添加更多的拾取条件检查
            // 例如：角色等级限制、阵营限制等

            return (true, "可以拾取");
        }

        /// <summary>
        /// 标记掉落物已被拾取
        /// </summary>
        /// <param name="characterId">拾取角色ID</param>
        public void MarkAsPickedUp(Guid characterId)
        {
            IsPickedUp = true;
            PickedUpByCharacterId = characterId;
            PickupTime = DateTime.UtcNow;
        }
    }
}