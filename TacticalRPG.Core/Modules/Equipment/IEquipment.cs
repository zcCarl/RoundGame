using System;
using System.Collections.Generic;
using TacticalRPG.Core.Modules.Character;

namespace TacticalRPG.Core.Modules.Equipment
{
    /// <summary>
    /// 定义装备的接口
    /// </summary>
    public interface IEquipment
    {
        /// <summary>
        /// 获取装备的唯一标识符
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// 获取装备的名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 获取装备的描述
        /// </summary>
        string Description { get; }

        /// <summary>
        /// 获取装备的类型
        /// </summary>
        EquipmentType Type { get; }

        /// <summary>
        /// 获取武器类型（如果是武器）
        /// </summary>
        WeaponType WeaponType { get; }

        /// <summary>
        /// 获取装备的稀有度
        /// </summary>
        EquipmentRarity Rarity { get; }

        /// <summary>
        /// 获取装备的等级/强化等级
        /// </summary>
        int Level { get; }

        /// <summary>
        /// 获取装备的耐久度
        /// </summary>
        float Durability { get; }

        /// <summary>
        /// 获取装备的最大耐久度
        /// </summary>
        float MaxDurability { get; }

        /// <summary>
        /// 获取装备的价值（游戏内货币）
        /// </summary>
        int Value { get; }

        /// <summary>
        /// 获取装备的重量
        /// </summary>
        float Weight { get; }

        /// <summary>
        /// 获取装备的所有属性修饰值
        /// </summary>
        IReadOnlyDictionary<EquipmentStatType, float> Stats { get; }

        /// <summary>
        /// 获取装备可装备的角色职业
        /// </summary>
        IReadOnlyList<CharacterClass> AllowedClasses { get; }

        /// <summary>
        /// 获取装备的图标/资源路径
        /// </summary>
        string IconPath { get; }

        /// <summary>
        /// 获取装备的3D模型路径（如适用）
        /// </summary>
        string ModelPath { get; }

        /// <summary>
        /// 获取装备的当前装备者（如已被装备）
        /// </summary>
        Guid? EquippedBy { get; }

        /// <summary>
        /// 获取装备是否被锁定（不可交易/丢弃/销毁）
        /// </summary>
        bool IsLocked { get; }

        /// <summary>
        /// 获取装备所属的套装ID（如属于套装）
        /// </summary>
        Guid? SetId { get; }

        /// <summary>
        /// 获取特定属性的值
        /// </summary>
        /// <param name="statType">属性类型</param>
        /// <returns>属性值</returns>
        float GetStatValue(EquipmentStatType statType);

        /// <summary>
        /// 设置特定属性的值
        /// </summary>
        /// <param name="statType">属性类型</param>
        /// <param name="value">新的属性值</param>
        /// <param name="reason">更改原因</param>
        /// <returns>是否成功设置</returns>
        bool SetStatValue(EquipmentStatType statType, float value, string reason = "");

        /// <summary>
        /// 增加特定属性的值
        /// </summary>
        /// <param name="statType">属性类型</param>
        /// <param name="amount">增加的数值</param>
        /// <param name="reason">更改原因</param>
        /// <returns>增加后的属性值</returns>
        float AddStatValue(EquipmentStatType statType, float amount, string reason = "");

        /// <summary>
        /// 减少特定属性的值
        /// </summary>
        /// <param name="statType">属性类型</param>
        /// <param name="amount">减少的数值</param>
        /// <param name="reason">更改原因</param>
        /// <returns>减少后的属性值</returns>
        float ReduceStatValue(EquipmentStatType statType, float amount, string reason = "");

        /// <summary>
        /// 修复装备耐久度
        /// </summary>
        /// <param name="amount">修复的数值</param>
        /// <param name="reason">修复原因</param>
        /// <returns>修复后的耐久度</returns>
        float Repair(float amount, string reason = "");

        /// <summary>
        /// 损耗装备耐久度
        /// </summary>
        /// <param name="amount">损耗的数值</param>
        /// <param name="reason">损耗原因</param>
        /// <returns>损耗后的耐久度</returns>
        float Degrade(float amount, string reason = "");

        /// <summary>
        /// 升级装备
        /// </summary>
        /// <param name="levels">升级的等级数</param>
        /// <param name="method">升级方式</param>
        /// <returns>是否成功升级</returns>
        bool Upgrade(int levels = 1, string method = "");

        /// <summary>
        /// 检查装备是否适合指定职业
        /// </summary>
        /// <param name="characterClass">角色职业</param>
        /// <returns>是否适合</returns>
        bool IsAllowedForClass(CharacterClass characterClass);

        /// <summary>
        /// 锁定/解锁装备
        /// </summary>
        /// <param name="locked">是否锁定</param>
        void SetLocked(bool locked);

        /// <summary>
        /// 设置装备者ID
        /// </summary>
        /// <param name="characterId">角色ID，null表示卸下装备</param>
        void SetEquippedBy(Guid? characterId);

        /// <summary>
        /// 获取装备的自定义属性
        /// </summary>
        /// <param name="key">属性键</param>
        /// <returns>属性值，如不存在则返回null</returns>
        object GetProperty(string key);

        /// <summary>
        /// 设置装备的自定义属性
        /// </summary>
        /// <param name="key">属性键</param>
        /// <param name="value">属性值</param>
        void SetProperty(string key, object value);

        /// <summary>
        /// 创建装备的副本
        /// </summary>
        /// <returns>装备的副本</returns>
        IEquipment Clone();
    }
}