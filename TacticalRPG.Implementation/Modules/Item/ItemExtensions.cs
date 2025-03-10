using System;
using TacticalRPG.Core.Modules.Item;

namespace TacticalRPG.Implementation.Modules.Item
{
    /// <summary>
    /// 物品扩展方法
    /// </summary>
    public static class ItemExtensions
    {
        /// <summary>
        /// 设置物品堆叠数量
        /// </summary>
        /// <param name="item">物品</param>
        /// <param name="newStackSize">新数量</param>
        public static void SetStackSize(this IItem item, int newStackSize)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            if (item is Item concreteItem)
            {
                concreteItem.StackSize = Math.Max(0, Math.Min(concreteItem.MaxStackSize, newStackSize));
            }
            else
            {
                throw new InvalidOperationException($"物品类型{item.GetType().Name}不支持设置堆叠数量");
            }
        }

        /// <summary>
        /// 设置物品属性
        /// </summary>
        /// <param name="item">物品</param>
        /// <param name="key">属性键</param>
        /// <param name="value">属性值</param>
        public static void SetAttribute(this IItem item, string key, object value)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("属性键不能为空", nameof(key));

            if (item is Item concreteItem)
            {
                concreteItem.SetProperty(key, value);
            }
            else
            {
                throw new InvalidOperationException($"物品类型{item.GetType().Name}不支持设置属性");
            }
        }

        /// <summary>
        /// 获取物品属性
        /// </summary>
        /// <param name="item">物品</param>
        /// <param name="key">属性键</param>
        /// <returns>属性值</returns>
        public static object GetAttribute(this IItem item, string key)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("属性键不能为空", nameof(key));

            if (item is Item concreteItem)
            {
                return concreteItem.GetProperty(key);
            }

            throw new InvalidOperationException($"物品类型{item.GetType().Name}不支持获取属性");
        }
    }
}