using System.Collections.Generic;

namespace TacticalRPG.Core.Modules.Inventory
{
    /// <summary>
    /// 背包排序策略接口
    /// </summary>
    public interface IInventorySortStrategy
    {
        /// <summary>
        /// 排序名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 排序描述
        /// </summary>
        string Description { get; }

        /// <summary>
        /// 对槽位进行排序
        /// </summary>
        /// <param name="slots">要排序的槽位集合</param>
        /// <returns>排序后的槽位集合</returns>
        IEnumerable<IInventorySlot> Sort(IEnumerable<IInventorySlot> slots);
    }
}