using System;
using TacticalRPG.Core.Modules.Character;

namespace TacticalRPG.Core.Modules.Equipment
{
    /// <summary>
    /// 装备变更事件参数
    /// </summary>
    public class EquipmentChangedEventArgs : EventArgs
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        public Guid CharacterId { get; }

        /// <summary>
        /// 装备槽
        /// </summary>
        public EquipmentSlot Slot { get; }

        /// <summary>
        /// 新装备ID（如果是卸下装备则为null）
        /// </summary>
        public Guid? NewEquipmentId { get; }

        /// <summary>
        /// 旧装备ID（如果是首次装备则为null）
        /// </summary>
        public Guid? OldEquipmentId { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <param name="slot">装备槽</param>
        /// <param name="newEquipmentId">新装备ID</param>
        /// <param name="oldEquipmentId">旧装备ID</param>
        public EquipmentChangedEventArgs(Guid characterId, EquipmentSlot slot, Guid? newEquipmentId, Guid? oldEquipmentId)
        {
            CharacterId = characterId;
            Slot = slot;
            NewEquipmentId = newEquipmentId;
            OldEquipmentId = oldEquipmentId;
        }
    }

    /// <summary>
    /// 装备创建事件参数
    /// </summary>
    public class EquipmentCreatedEventArgs : EventArgs
    {
        /// <summary>
        /// 装备ID
        /// </summary>
        public Guid EquipmentId { get; }

        /// <summary>
        /// 装备类型
        /// </summary>
        public EquipmentType Type { get; }

        /// <summary>
        /// 装备名称
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 装备稀有度
        /// </summary>
        public EquipmentRarity Rarity { get; }

        /// <summary>
        /// 创建者ID（如果是系统创建则为null）
        /// </summary>
        public Guid? CreatorId { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="equipmentId">装备ID</param>
        /// <param name="type">装备类型</param>
        /// <param name="name">装备名称</param>
        /// <param name="rarity">装备稀有度</param>
        /// <param name="creatorId">创建者ID</param>
        public EquipmentCreatedEventArgs(Guid equipmentId, EquipmentType type, string name, EquipmentRarity rarity, Guid? creatorId = null)
        {
            EquipmentId = equipmentId;
            Type = type;
            Name = name;
            Rarity = rarity;
            CreatorId = creatorId;
        }
    }

    /// <summary>
    /// 装备销毁事件参数
    /// </summary>
    public class EquipmentDestroyedEventArgs : EventArgs
    {
        /// <summary>
        /// 装备ID
        /// </summary>
        public Guid EquipmentId { get; }

        /// <summary>
        /// 销毁原因
        /// </summary>
        public string Reason { get; }

        /// <summary>
        /// 销毁者ID（如果是系统销毁则为null）
        /// </summary>
        public Guid? DestroyerId { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="equipmentId">装备ID</param>
        /// <param name="reason">销毁原因</param>
        /// <param name="destroyerId">销毁者ID</param>
        public EquipmentDestroyedEventArgs(Guid equipmentId, string reason = "", Guid? destroyerId = null)
        {
            EquipmentId = equipmentId;
            Reason = reason;
            DestroyerId = destroyerId;
        }
    }

    /// <summary>
    /// 装备属性变更事件参数
    /// </summary>
    public class EquipmentStatChangedEventArgs : EventArgs
    {
        /// <summary>
        /// 装备ID
        /// </summary>
        public Guid EquipmentId { get; }

        /// <summary>
        /// 变更的属性类型
        /// </summary>
        public EquipmentStatType StatType { get; }

        /// <summary>
        /// 新属性值
        /// </summary>
        public float NewValue { get; }

        /// <summary>
        /// 旧属性值
        /// </summary>
        public float OldValue { get; }

        /// <summary>
        /// 变更原因
        /// </summary>
        public string Reason { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="equipmentId">装备ID</param>
        /// <param name="statType">属性类型</param>
        /// <param name="newValue">新属性值</param>
        /// <param name="oldValue">旧属性值</param>
        /// <param name="reason">变更原因</param>
        public EquipmentStatChangedEventArgs(Guid equipmentId, EquipmentStatType statType, float newValue, float oldValue, string reason = "")
        {
            EquipmentId = equipmentId;
            StatType = statType;
            NewValue = newValue;
            OldValue = oldValue;
            Reason = reason;
        }
    }

    /// <summary>
    /// 装备升级事件参数
    /// </summary>
    public class EquipmentUpgradedEventArgs : EventArgs
    {
        /// <summary>
        /// 装备ID
        /// </summary>
        public Guid EquipmentId { get; }

        /// <summary>
        /// 新的等级/强化等级
        /// </summary>
        public int NewLevel { get; }

        /// <summary>
        /// 旧的等级/强化等级
        /// </summary>
        public int OldLevel { get; }

        /// <summary>
        /// 升级方式
        /// </summary>
        public string UpgradeMethod { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="equipmentId">装备ID</param>
        /// <param name="newLevel">新等级</param>
        /// <param name="oldLevel">旧等级</param>
        /// <param name="upgradeMethod">升级方式</param>
        public EquipmentUpgradedEventArgs(Guid equipmentId, int newLevel, int oldLevel, string upgradeMethod = "")
        {
            EquipmentId = equipmentId;
            NewLevel = newLevel;
            OldLevel = oldLevel;
            UpgradeMethod = upgradeMethod;
        }
    }

    /// <summary>
    /// 装备耐久度变更事件参数
    /// </summary>
    public class EquipmentDurabilityChangedEventArgs : EventArgs
    {
        /// <summary>
        /// 装备ID
        /// </summary>
        public Guid EquipmentId { get; }

        /// <summary>
        /// 新的耐久度
        /// </summary>
        public float NewDurability { get; }

        /// <summary>
        /// 旧的耐久度
        /// </summary>
        public float OldDurability { get; }

        /// <summary>
        /// 最大耐久度
        /// </summary>
        public float MaxDurability { get; }

        /// <summary>
        /// 变更原因
        /// </summary>
        public string Reason { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="equipmentId">装备ID</param>
        /// <param name="newDurability">新耐久度</param>
        /// <param name="oldDurability">旧耐久度</param>
        /// <param name="maxDurability">最大耐久度</param>
        /// <param name="reason">变更原因</param>
        public EquipmentDurabilityChangedEventArgs(Guid equipmentId, float newDurability, float oldDurability, float maxDurability, string reason = "")
        {
            EquipmentId = equipmentId;
            NewDurability = newDurability;
            OldDurability = oldDurability;
            MaxDurability = maxDurability;
            Reason = reason;
        }
    }
}