using System.Collections.Generic;
using System.Linq;
using TacticalRPG.Core.Modules.Inventory;

namespace TacticalRPG.Implementation.Modules.Inventory.SortStrategies
{
    /// <summary>
    /// 按物品类型排序策略
    /// </summary>
    public class ItemTypeSortStrategy : IInventorySortStrategy
    {
        /// <summary>
        /// 排序名称
        /// </summary>
        public string Name => "按类型排序";

        /// <summary>
        /// 排序描述
        /// </summary>
        public string Description => "将物品按类型分组排序，空槽位排在最后";

        /// <summary>
        /// 对槽位进行排序
        /// </summary>
        /// <param name="slots">要排序的槽位集合</param>
        /// <returns>排序后的槽位集合</returns>
        public IEnumerable<IInventorySlot> Sort(IEnumerable<IInventorySlot> slots)
        {
            if (slots == null)
                return Enumerable.Empty<IInventorySlot>();

            // 首先按物品类型排序，然后按稀有度排序，最后按名称排序
            // 空槽位排在最后
            return slots.OrderBy(s => s.IsEmpty) // 非空槽位优先
                        .ThenBy(s => s.IsEmpty ? 0 : (int)s.Item.Type) // 按物品类型排序
                        .ThenByDescending(s => s.IsEmpty ? 0 : (int)s.Item.Rarity) // 按稀有度降序排序
                        .ThenBy(s => s.IsEmpty ? string.Empty : s.Item.Name); // 按名称排序
        }
    }
}