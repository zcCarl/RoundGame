using System;
using System.Collections.Generic;
using TacticalRPG.Core.Modules.Character;
using TacticalRPG.Core.Modules.Equipment;

namespace TacticalRPG.Core.Modules.Inventory
{
    /// <summary>
    /// 定义游戏物品的接口
    /// </summary>
    public interface IItem
    {
        /// <summary>
        /// 获取物品的唯一标识符
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// 获取物品的模板标识符
        /// </summary>
        string TemplateId { get; }

        /// <summary>
        /// 获取物品的名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 获取物品的描述
        /// </summary>
        string Description { get; }

        /// <summary>
        /// 获取物品的类型
        /// </summary>
        ItemType Type { get; }

        /// <summary>
        /// 获取物品的稀有度
        /// </summary>
        ItemRarity Rarity { get; }

        /// <summary>
        /// 获取物品的图标路径
        /// </summary>
        string IconPath { get; }

        /// <summary>
        /// 获取物品是否可堆叠
        /// </summary>
        bool IsStackable { get; }

        /// <summary>
        /// 获取物品最大堆叠数量
        /// </summary>
        int MaxStackSize { get; }

        /// <summary>
        /// 获取物品当前堆叠数量
        /// </summary>
        int StackSize { get; set; }

        /// <summary>
        /// 获取物品的重量（每单位）
        /// </summary>
        float Weight { get; }

        /// <summary>
        /// 获取物品的价值（游戏内货币，每单位）
        /// </summary>
        int Value { get; }

        /// <summary>
        /// 获取物品是否可使用
        /// </summary>
        bool IsUsable { get; }

        /// <summary>
        /// 获取物品是否为装备
        /// </summary>
        bool IsEquipment { get; }

        /// <summary>
        /// 获取物品是否为消耗品
        /// </summary>
        bool IsConsumable { get; }

        /// <summary>
        /// 获取物品是否为任务物品
        /// </summary>
        bool IsQuestItem { get; }

        /// <summary>
        /// 获取装备实例（如果物品是装备）
        /// </summary>
        IEquipment EquipmentInstance { get; }

        /// <summary>
        /// 获取物品是否被锁定（不可交易/丢弃/销毁）
        /// </summary>
        bool IsLocked { get; }

        /// <summary>
        /// 使用物品
        /// </summary>
        /// <param name="characterId">使用者ID</param>
        /// <param name="targetId">目标ID（可选）</param>
        /// <returns>使用结果</returns>
        (bool success, string message) Use(Guid characterId, Guid? targetId = null);

        /// <summary>
        /// 检查物品是否可以被特定角色使用
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <param name="targetId">目标ID（可选）</param>
        /// <returns>检查结果</returns>
        (bool canUse, string reason) CanBeUsedBy(Guid characterId, Guid? targetId = null);

        /// <summary>
        /// 添加物品到堆叠
        /// </summary>
        /// <param name="amount">添加数量</param>
        /// <returns>实际添加的数量</returns>
        int AddToStack(int amount);

        /// <summary>
        /// 从堆叠中移除物品
        /// </summary>
        /// <param name="amount">移除数量</param>
        /// <returns>实际移除的数量</returns>
        int RemoveFromStack(int amount);

        /// <summary>
        /// 拆分堆叠
        /// </summary>
        /// <param name="amount">拆分数量</param>
        /// <returns>拆分出的新物品实例</returns>
        IItem SplitStack(int amount);

        /// <summary>
        /// 合并堆叠
        /// </summary>
        /// <param name="item">要合并的物品</param>
        /// <returns>合并后的剩余物品（如完全合并则为null）</returns>
        IItem MergeStack(IItem item);

        /// <summary>
        /// 锁定/解锁物品
        /// </summary>
        /// <param name="locked">是否锁定</param>
        void SetLocked(bool locked);

        /// <summary>
        /// 获取物品的自定义属性
        /// </summary>
        /// <param name="key">属性键</param>
        /// <returns>属性值，如不存在则返回null</returns>
        object GetProperty(string key);

        /// <summary>
        /// 设置物品的自定义属性
        /// </summary>
        /// <param name="key">属性键</param>
        /// <param name="value">属性值</param>
        void SetProperty(string key, object value);

        /// <summary>
        /// 创建物品的副本
        /// </summary>
        /// <param name="amount">数量（默认全部）</param>
        /// <returns>物品的副本</returns>
        IItem Clone(int amount = 0);
    }
}