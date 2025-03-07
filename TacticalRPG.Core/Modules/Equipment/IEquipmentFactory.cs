using System;
using System.Collections.Generic;
using TacticalRPG.Core.Modules.Character;
using TacticalRPG.Core.Modules.Skill;

namespace TacticalRPG.Core.Modules.Equipment
{
    /// <summary>
    /// 定义装备工厂的接口，用于创建各种类型的装备
    /// </summary>
    public interface IEquipmentFactory
    {
        /// <summary>
        /// 创建基础装备
        /// </summary>
        /// <param name="name">装备名称</param>
        /// <param name="description">装备描述</param>
        /// <param name="type">装备类型</param>
        /// <param name="rarity">装备稀有度</param>
        /// <param name="stats">装备属性</param>
        /// <param name="level">装备等级</param>
        /// <param name="allowedClasses">允许装备的职业</param>
        /// <returns>创建的装备实例</returns>
        IEquipment CreateEquipment(
            string name,
            string description,
            EquipmentType type,
            EquipmentRarity rarity,
            Dictionary<EquipmentStatType, float> stats,
            int level = 1,
            IEnumerable<CharacterClass>? allowedClasses = null);

        /// <summary>
        /// 创建武器
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
        /// <param name="elementalDamageType">元素伤害类型</param>
        /// <param name="elementalDamage">元素伤害值</param>
        /// <param name="allowedClasses">允许装备的职业</param>
        /// <returns>创建的武器实例</returns>
        IWeapon CreateWeapon(
            string name,
            string description,
            WeaponType weaponType,
            EquipmentRarity rarity,
            int baseDamage,
            int attackRange,
            float attackSpeed,
            Dictionary<EquipmentStatType, float>? stats = null,
            int level = 1,
            bool isTwoHanded = false,
            SkillEffectType elementalDamageType = SkillEffectType.None,
            int elementalDamage = 0,
            IEnumerable<CharacterClass>? allowedClasses = null);

        /// <summary>
        /// 创建护甲
        /// </summary>
        /// <param name="name">护甲名称</param>
        /// <param name="description">护甲描述</param>
        /// <param name="type">护甲类型</param>
        /// <param name="rarity">护甲稀有度</param>
        /// <param name="physicalDefense">物理防御</param>
        /// <param name="magicalDefense">魔法防御</param>
        /// <param name="stats">护甲属性</param>
        /// <param name="level">护甲等级</param>
        /// <param name="allowedClasses">允许装备的职业</param>
        /// <returns>创建的护甲实例</returns>
        IEquipment CreateArmor(
            string name,
            string description,
            EquipmentType type,
            EquipmentRarity rarity,
            int physicalDefense,
            int magicalDefense,
            Dictionary<EquipmentStatType, float>? stats = null,
            int level = 1,
            IEnumerable<CharacterClass>? allowedClasses = null);

        /// <summary>
        /// 创建饰品
        /// </summary>
        /// <param name="name">饰品名称</param>
        /// <param name="description">饰品描述</param>
        /// <param name="type">饰品类型</param>
        /// <param name="rarity">饰品稀有度</param>
        /// <param name="stats">饰品属性</param>
        /// <param name="level">饰品等级</param>
        /// <param name="specialEffect">特殊效果描述</param>
        /// <param name="allowedClasses">允许装备的职业</param>
        /// <returns>创建的饰品实例</returns>
        IEquipment CreateAccessory(
            string name,
            string description,
            EquipmentType type,
            EquipmentRarity rarity,
            Dictionary<EquipmentStatType, float> stats,
            int level = 1,
            string specialEffect = "",
            IEnumerable<CharacterClass>? allowedClasses = null);

        /// <summary>
        /// 创建消耗品
        /// </summary>
        /// <param name="name">消耗品名称</param>
        /// <param name="description">消耗品描述</param>
        /// <param name="rarity">消耗品稀有度</param>
        /// <param name="effectType">效果类型</param>
        /// <param name="effectValue">效果值</param>
        /// <param name="durability">使用次数</param>
        /// <param name="stats">临时属性加成</param>
        /// <param name="effectDuration">效果持续时间</param>
        /// <returns>创建的消耗品实例</returns>
        IEquipment CreateConsumable(
            string name,
            string description,
            EquipmentRarity rarity,
            EquipmentStatType effectType,
            float effectValue,
            int durability = 1,
            Dictionary<EquipmentStatType, float>? stats = null,
            int effectDuration = 0);

        /// <summary>
        /// 根据模板创建装备
        /// </summary>
        /// <param name="templateId">模板ID</param>
        /// <param name="level">装备等级</param>
        /// <returns>创建的装备实例</returns>
        IEquipment CreateFromTemplate(string templateId, int level = 1);

        /// <summary>
        /// 创建随机装备
        /// </summary>
        /// <param name="type">装备类型</param>
        /// <param name="minRarity">最低稀有度</param>
        /// <param name="maxRarity">最高稀有度</param>
        /// <param name="level">装备等级</param>
        /// <param name="allowedClasses">允许装备的职业</param>
        /// <returns>创建的随机装备实例</returns>
        IEquipment CreateRandomEquipment(
            EquipmentType type,
            EquipmentRarity minRarity = EquipmentRarity.Common,
            EquipmentRarity maxRarity = EquipmentRarity.Legendary,
            int level = 1,
            IEnumerable<CharacterClass>? allowedClasses = null);

        /// <summary>
        /// 创建套装装备
        /// </summary>
        /// <param name="setId">套装ID</param>
        /// <param name="itemName">装备名称</param>
        /// <param name="itemType">装备类型</param>
        /// <param name="level">装备等级</param>
        /// <returns>创建的套装装备实例</returns>
        IEquipment CreateSetItem(Guid setId, string itemName, EquipmentType itemType, int level = 1);
    }
}