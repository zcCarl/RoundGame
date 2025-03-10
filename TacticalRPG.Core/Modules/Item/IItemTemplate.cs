using System;
using System.Collections.Generic;
using TacticalRPG.Core.Modules.Inventory;

namespace TacticalRPG.Core.Modules.Item
{
    /// <summary>
    /// 物品模板接口，定义创建物品实例的蓝图
    /// </summary>
    public interface IItemTemplate
    {
        /// <summary>
        /// 模板ID
        /// </summary>
        string TemplateId { get; }

        /// <summary>
        /// 物品名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 物品描述
        /// </summary>
        string Description { get; }

        /// <summary>
        /// 物品类型
        /// </summary>
        ItemType Type { get; }

        /// <summary>
        /// 物品稀有度
        /// </summary>
        ItemRarity Rarity { get; }

        /// <summary>
        /// 物品图标路径
        /// </summary>
        string IconPath { get; }

        /// <summary>
        /// 是否可堆叠
        /// </summary>
        bool IsStackable { get; }

        /// <summary>
        /// 最大堆叠数量
        /// </summary>
        int MaxStackSize { get; }

        /// <summary>
        /// 物品重量
        /// </summary>
        float Weight { get; }

        /// <summary>
        /// 物品价值
        /// </summary>
        int Value { get; }

        /// <summary>
        /// 设置属性值
        /// </summary>
        /// <param name="key">属性键</param>
        /// <param name="value">属性值</param>
        void SetAttribute(string key, object value);

        /// <summary>
        /// 获取属性值
        /// </summary>
        /// <param name="key">属性键</param>
        /// <returns>属性值</returns>
        object GetAttribute(string key);

        /// <summary>
        /// 获取所有属性键
        /// </summary>
        /// <returns>属性键集合</returns>
        IEnumerable<string> GetAttributeKeys();
    }
}