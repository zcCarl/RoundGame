using System;
using System.Collections.Generic;
using TacticalRPG.Core.Framework;
using TacticalRPG.Core.Modules.Character;

namespace TacticalRPG.Core.Modules.Equipment
{
    /// <summary>
    /// 定义装备模块的接口，负责管理游戏中的装备系统
    /// </summary>
    public interface IEquipmentModule
    {
        /// <summary>
        /// 获取装备工厂实例
        /// </summary>
        IEquipmentFactory EquipmentFactory { get; }

        /// <summary>
        /// 根据ID获取装备
        /// </summary>
        /// <param name="equipmentId">装备ID</param>
        /// <returns>找到的装备实例，未找到则返回null</returns>
        IEquipment GetEquipment(Guid equipmentId);

        /// <summary>
        /// 获取武器类装备
        /// </summary>
        /// <param name="equipmentId">装备ID</param>
        /// <returns>找到的武器实例，未找到或不是武器则返回null</returns>
        IWeapon GetWeapon(Guid equipmentId);

        /// <summary>
        /// 注册装备到系统
        /// </summary>
        /// <param name="equipment">装备实例</param>
        /// <returns>注册后的装备ID</returns>
        Guid RegisterEquipment(IEquipment equipment);

        /// <summary>
        /// 创建并注册基础装备
        /// </summary>
        /// <param name="name">装备名称</param>
        /// <param name="description">装备描述</param>
        /// <param name="type">装备类型</param>
        /// <param name="rarity">装备稀有度</param>
        /// <param name="stats">装备属性</param>
        /// <param name="level">装备等级</param>
        /// <param name="allowedClasses">允许装备的职业</param>
        /// <returns>创建的装备ID</returns>
        Guid CreateEquipment(
            string name,
            string description,
            EquipmentType type,
            EquipmentRarity rarity,
            Dictionary<EquipmentStatType, float> stats,
            int level = 1,
            IEnumerable<CharacterClass> allowedClasses = null);

        /// <summary>
        /// 创建并注册武器
        /// </summary>
        /// <param name="name">武器名称</param>
        /// <param name="description">武器描述</param>
        /// <param name="weaponType">武器类型</param>
        /// <param name="rarity">武器稀有度</param>
        /// <param name="baseDamage">基础伤害</param>
        /// <param name="attackRange">攻击范围</param>
        /// <param name="attackSpeed">攻击速度</param>
        /// <param name="stats">武器属性</param>
        /// <param name="level">武器等级</param>
        /// <param name="isTwoHanded">是否为双手武器</param>
        /// <returns>创建的武器ID</returns>
        Guid CreateWeapon(
            string name,
            string description,
            WeaponType weaponType,
            EquipmentRarity rarity,
            int baseDamage,
            int attackRange,
            float attackSpeed,
            Dictionary<EquipmentStatType, float> stats = null,
            int level = 1,
            bool isTwoHanded = false);

        /// <summary>
        /// 装备物品到角色
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <param name="equipmentId">装备ID</param>
        /// <param name="slot">装备槽</param>
        /// <returns>是否成功装备</returns>
        bool EquipItem(Guid characterId, Guid equipmentId, EquipmentSlot slot);

        /// <summary>
        /// 从角色卸下装备
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <param name="slot">装备槽</param>
        /// <returns>卸下的装备ID，如无装备则返回null</returns>
        Guid? UnequipItem(Guid characterId, EquipmentSlot slot);

        /// <summary>
        /// 卸下角色的所有装备
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <returns>卸下的装备ID列表</returns>
        IReadOnlyList<Guid> UnequipAllItems(Guid characterId);

        /// <summary>
        /// 获取角色的装备列表
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <returns>装备槽与装备ID的字典</returns>
        IReadOnlyDictionary<EquipmentSlot, Guid> GetCharacterEquipment(Guid characterId);

        /// <summary>
        /// 获取角色特定槽位的装备
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <param name="slot">装备槽</param>
        /// <returns>装备ID，如无装备则返回null</returns>
        Guid? GetEquippedItem(Guid characterId, EquipmentSlot slot);

        /// <summary>
        /// 修复装备
        /// </summary>
        /// <param name="equipmentId">装备ID</param>
        /// <param name="amount">修复量</param>
        /// <param name="reason">修复原因</param>
        /// <returns>修复后的耐久度</returns>
        float RepairEquipment(Guid equipmentId, float amount, string reason = "");

        /// <summary>
        /// 强化装备
        /// </summary>
        /// <param name="equipmentId">装备ID</param>
        /// <param name="levels">强化等级数</param>
        /// <param name="method">强化方式</param>
        /// <returns>是否成功强化</returns>
        bool UpgradeEquipment(Guid equipmentId, int levels = 1, string method = "");

        /// <summary>
        /// 销毁装备
        /// </summary>
        /// <param name="equipmentId">装备ID</param>
        /// <param name="reason">销毁原因</param>
        /// <param name="destroyerId">销毁者ID</param>
        /// <returns>是否成功销毁</returns>
        bool DestroyEquipment(Guid equipmentId, string reason = "", Guid? destroyerId = null);

        /// <summary>
        /// 比较两件装备
        /// </summary>
        /// <param name="equipmentId1">装备1 ID</param>
        /// <param name="equipmentId2">装备2 ID</param>
        /// <returns>比较结果（正数表示装备1更好，负数表示装备2更好，0表示相当）</returns>
        int CompareEquipment(Guid equipmentId1, Guid equipmentId2);

        /// <summary>
        /// 获取装备的总属性加成
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <returns>属性类型与加成值的字典</returns>
        Dictionary<EquipmentStatType, float> GetTotalEquipmentStats(Guid characterId);

        /// <summary>
        /// 计算套装效果
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <returns>套装ID与激活件数的字典</returns>
        Dictionary<Guid, int> CalculateSetBonuses(Guid characterId);

        /// <summary>
        /// 锁定/解锁装备
        /// </summary>
        /// <param name="equipmentId">装备ID</param>
        /// <param name="locked">是否锁定</param>
        /// <returns>是否成功设置</returns>
        bool SetEquipmentLocked(Guid equipmentId, bool locked);

        /// <summary>
        /// 创建装备的副本
        /// </summary>
        /// <param name="equipmentId">源装备ID</param>
        /// <returns>副本装备的ID</returns>
        Guid CloneEquipment(Guid equipmentId);

        /// <summary>
        /// 设置装备的自定义属性
        /// </summary>
        /// <param name="equipmentId">装备ID</param>
        /// <param name="key">属性键</param>
        /// <param name="value">属性值</param>
        /// <returns>是否成功设置</returns>
        bool SetEquipmentProperty(Guid equipmentId, string key, object value);

        /// <summary>
        /// 获取装备的自定义属性
        /// </summary>
        /// <param name="equipmentId">装备ID</param>
        /// <param name="key">属性键</param>
        /// <returns>属性值，如不存在则返回null</returns>
        object GetEquipmentProperty(Guid equipmentId, string key);
    }
}