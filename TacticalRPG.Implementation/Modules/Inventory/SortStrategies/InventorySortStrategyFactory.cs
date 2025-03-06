using System;
using System.Collections.Generic;
using System.Linq;
using TacticalRPG.Core.Modules.Inventory;

namespace TacticalRPG.Implementation.Modules.Inventory.SortStrategies
{
    /// <summary>
    /// 背包排序策略工厂
    /// </summary>
    public class InventorySortStrategyFactory
    {
        private static readonly Dictionary<string, IInventorySortStrategy> _strategies = new Dictionary<string, IInventorySortStrategy>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// 静态构造函数，注册默认策略
        /// </summary>
        static InventorySortStrategyFactory()
        {
            RegisterStrategy("type", new ItemTypeSortStrategy());
            RegisterStrategy("rarity", new RaritySortStrategy());
            RegisterStrategy("name", new NameSortStrategy());
        }

        /// <summary>
        /// 注册排序策略
        /// </summary>
        /// <param name="name">策略名称</param>
        /// <param name="strategy">策略实例</param>
        public static void RegisterStrategy(string name, IInventorySortStrategy strategy)
        {
            if (string.IsNullOrEmpty(name) || strategy == null)
                return;

            _strategies[name] = strategy;
        }

        /// <summary>
        /// 获取排序策略
        /// </summary>
        /// <param name="name">策略名称</param>
        /// <returns>策略实例，如不存在则返回默认策略（按类型排序）</returns>
        public static IInventorySortStrategy GetStrategy(string name)
        {
            if (string.IsNullOrEmpty(name) || !_strategies.ContainsKey(name))
                return _strategies["type"]; // 默认按类型排序

            return _strategies[name];
        }

        /// <summary>
        /// 获取所有可用的排序策略
        /// </summary>
        /// <returns>策略列表</returns>
        public static IReadOnlyList<IInventorySortStrategy> GetAllStrategies()
        {
            return _strategies.Values.ToList();
        }

        /// <summary>
        /// 获取所有可用的排序策略名称
        /// </summary>
        /// <returns>策略名称列表</returns>
        public static IReadOnlyList<string> GetAllStrategyNames()
        {
            return _strategies.Keys.ToList();
        }
    }
}