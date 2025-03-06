using System;
using System.Collections.Generic;
using System.Linq;
using TacticalRPG.Core.Modules.Inventory;

namespace TacticalRPG.Implementation.Modules.Inventory
{
    /// <summary>
    /// 掉落物实现类
    /// </summary>
    public class Drop : IDrop
    {
        private readonly List<IItem> _items = new List<IItem>();
        private readonly int _defaultExistDuration = 60; // 默认存在60秒

        /// <summary>
        /// 掉落物唯一ID
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// 掉落物在地图上的位置坐标X
        /// </summary>
        public int X { get; private set; }

        /// <summary>
        /// 掉落物在地图上的位置坐标Y
        /// </summary>
        public int Y { get; private set; }

        /// <summary>
        /// 掉落时间
        /// </summary>
        public DateTime DropTime { get; }

        /// <summary>
        /// 存在时间（秒），0表示永久存在
        /// </summary>
        public int ExistDuration { get; private set; }

        /// <summary>
        /// 是否已被拾取
        /// </summary>
        public bool IsPickedUp { get; private set; }

        /// <summary>
        /// 拾取者ID（如果已被拾取）
        /// </summary>
        public Guid? PickerId { get; private set; }

        /// <summary>
        /// 拾取时间（如果已被拾取）
        /// </summary>
        public DateTime? PickUpTime { get; private set; }

        /// <summary>
        /// 掉落物包含的物品列表
        /// </summary>
        public IReadOnlyList<IItem> Items => _items.AsReadOnly();

        /// <summary>
        /// 掉落物的图标
        /// </summary>
        public string IconPath { get; private set; }

        /// <summary>
        /// 掉落物的描述
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// 掉落物的名称
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="x">X坐标</param>
        /// <param name="y">Y坐标</param>
        /// <param name="existDuration">存在时间（秒）</param>
        public Drop(int x, int y, int existDuration = 60)
        {
            Id = Guid.NewGuid();
            X = x;
            Y = y;
            DropTime = DateTime.UtcNow;
            ExistDuration = existDuration;
            IsPickedUp = false;
            PickerId = null;
            PickUpTime = null;
            IconPath = "Assets/Icons/Drops/default_drop.png";
            Name = "掉落物";
            Description = "一堆掉落的物品";
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="x">X坐标</param>
        /// <param name="y">Y坐标</param>
        /// <param name="items">物品列表</param>
        /// <param name="existDuration">存在时间（秒）</param>
        public Drop(int x, int y, IEnumerable<IItem> items, int existDuration = 60)
            : this(x, y, existDuration)
        {
            if (items != null)
            {
                foreach (var item in items)
                {
                    AddItem(item);
                }
            }

            UpdateNameAndDescription();
        }

        /// <summary>
        /// 设置掉落物位置
        /// </summary>
        /// <param name="x">X坐标</param>
        /// <param name="y">Y坐标</param>
        public void SetPosition(int x, int y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// 设置存在时间
        /// </summary>
        /// <param name="duration">存在时间（秒）</param>
        public void SetExistDuration(int duration)
        {
            ExistDuration = duration;
        }

        /// <summary>
        /// 添加物品到掉落物
        /// </summary>
        /// <param name="item">物品</param>
        public void AddItem(IItem item)
        {
            if (item != null)
            {
                _items.Add(item);
                UpdateNameAndDescription();
            }
        }

        /// <summary>
        /// 移除物品
        /// </summary>
        /// <param name="itemIndex">物品索引</param>
        /// <returns>移除的物品</returns>
        public IItem RemoveItem(int itemIndex)
        {
            if (itemIndex < 0 || itemIndex >= _items.Count)
            {
                return null;
            }

            var item = _items[itemIndex];
            _items.RemoveAt(itemIndex);
            UpdateNameAndDescription();
            return item;
        }

        /// <summary>
        /// 标记为已拾取
        /// </summary>
        /// <param name="pickerId">拾取者ID</param>
        public void MarkAsPickedUp(Guid pickerId)
        {
            IsPickedUp = true;
            PickerId = pickerId;
            PickUpTime = DateTime.UtcNow;
        }

        /// <summary>
        /// 是否可以被拾取
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <returns>检查结果</returns>
        public (bool canPickup, string reason) CanBePickedUpBy(Guid characterId)
        {
            if (IsPickedUp)
            {
                return (false, "该掉落物已被拾取");
            }

            if (IsExpired())
            {
                return (false, "该掉落物已过期");
            }

            if (IsEmpty())
            {
                return (false, "该掉落物没有任何物品");
            }

            return (true, string.Empty);
        }

        /// <summary>
        /// 是否已过期
        /// </summary>
        public bool IsExpired()
        {
            if (ExistDuration <= 0)
            {
                return false; // 永久存在
            }

            return (DateTime.UtcNow - DropTime).TotalSeconds > ExistDuration;
        }

        /// <summary>
        /// 是否为空（没有物品）
        /// </summary>
        public bool IsEmpty()
        {
            return _items.Count == 0;
        }

        /// <summary>
        /// 更新掉落物的名称和描述
        /// </summary>
        private void UpdateNameAndDescription()
        {
            if (_items.Count == 0)
            {
                Name = "空掉落物";
                Description = "这里没有任何物品";
                IconPath = "Assets/Icons/Drops/empty_drop.png";
                return;
            }

            if (_items.Count == 1)
            {
                var item = _items[0];
                Name = item.Name;
                Description = item.Description;
                IconPath = item.IconPath;
                return;
            }

            // 多个物品时，按稀有度排序，取最稀有的物品图标
            var mostRareItem = _items.OrderByDescending(i => (int)i.Rarity).First();
            IconPath = mostRareItem.IconPath;

            // 名称显示最稀有物品和总数
            Name = $"{mostRareItem.Name}等{_items.Count}件物品";

            // 描述显示前三个物品
            var topItems = _items.Take(3).Select(i => i.Name);
            Description = string.Join("、", topItems);

            if (_items.Count > 3)
            {
                Description += $"等{_items.Count}件物品";
            }
        }
    }
}