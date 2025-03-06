using System.Collections.Generic;
using System.Linq;
using TacticalRPG.Core.Modules.Inventory;

namespace TacticalRPG.Implementation.Modules.Inventory.SortStrategies
{
    /// <summary>
    /// 按物品稀有度排序策略
    /// </summary>
    public class RaritySortStrategy : IInventorySortStrategy
    {
        /// <summary>
        /// 排序名称
        /// </summary>
        public string Name => "按稀有度排序";

        /// <summary>
        /// 排序描述
        /// </summary>
        public string Description => "将物品按稀有度从高到低排序，空槽位排在最后";

        /// <summary>
        /// 对槽位进行排序
        /// </summary>
        /// <param name="slots">要排序的槽位集合</param>
        /// <returns>排序后的槽位集合</returns>
        public IEnumerable<IInventorySlot> Sort(IEnumerable<IInventorySlot> slots)
        {
            if (slots == null)
                return Enumerable.Empty<IInventorySlot>();

            // 首先按稀有度降序排序，然后按物品类型排序，最后按名称排序
            // 空槽位排在最后
            return slots.OrderBy(s => s.IsEmpty) // 非空槽位优先
                        .ThenByDescending(s => s.IsEmpty ? 0 : (int)s.Item.Rarity) // 按稀有度降序排序
                        .ThenBy(s => s.IsEmpty ? 0 : (int)s.Item.Type) // 按物品类型排序
                        .ThenBy(s => s.IsEmpty ? string.Empty : s.Item.Name); // 按名称排序
        }
    }
}