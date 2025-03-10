using System;
using System.Collections.Generic;
using TacticalRPG.Core.Modules.Character;

namespace TacticalRPG.Implementation.Modules.Character
{
    /// <summary>
    /// 角色类，实现ICharacter接口
    /// </summary>
    public class Character : ICharacter
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 角色职业
        /// </summary>
        public CharacterClass Class { get; private set; }

        /// <summary>
        /// 角色等级
        /// </summary>
        public int Level { get; private set; }

        /// <summary>
        /// 当前经验值
        /// </summary>
        public int Experience { get; private set; }

        /// <summary>
        /// 升级所需经验值
        /// </summary>
        public int ExperienceToNextLevel => CalculateExperienceToNextLevel();

        /// <summary>
        /// 最大生命值
        /// </summary>
        public int MaxHP { get; private set; }

        /// <summary>
        /// 最大魔法值
        /// </summary>
        public int MaxMP { get; private set; }

        /// <summary>
        /// 力量
        /// </summary>
        public int Strength { get; private set; }

        /// <summary>
        /// 敏捷
        /// </summary>
        public int Agility { get; private set; }

        /// <summary>
        /// 智力
        /// </summary>
        public int Intelligence { get; private set; }

        /// <summary>
        /// 体质
        /// </summary>
        public int Constitution { get; private set; }

        /// <summary>
        /// 幸运
        /// </summary>
        public int Luck { get; private set; }

        /// <summary>
        /// 移动力
        /// </summary>
        public int Movement { get; private set; }

        /// <summary>
        /// 技能列表
        /// </summary>
        public IReadOnlyList<Guid> SkillIds => _skillIds.AsReadOnly();
        private List<Guid> _skillIds = new List<Guid>();

        /// <summary>
        /// 装备
        /// </summary>
        public IReadOnlyDictionary<EquipmentSlot, Guid> Equipment => _equipment;
        private Dictionary<EquipmentSlot, Guid> _equipment = new Dictionary<EquipmentSlot, Guid>();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="id">角色ID</param>
        /// <param name="name">角色名称</param>
        /// <param name="characterClass">角色职业</param>
        public Character(Guid id, string name, CharacterClass characterClass)
        {
            Id = id;
            Name = name;
            Class = characterClass;
            Level = 1;
            Experience = 0;

            // 初始化装备槽位
            foreach (EquipmentSlot slot in Enum.GetValues(typeof(EquipmentSlot)))
            {
                _equipment[slot] = Guid.Empty;
            }

            // 根据职业设置初始属性
            SetInitialStats();
        }

        /// <summary>
        /// 设置名称
        /// </summary>
        /// <param name="name">名称</param>
        public void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("角色名称不能为空", nameof(name));

            Name = name;
        }

        /// <summary>
        /// 设置职业
        /// </summary>
        /// <param name="characterClass">职业</param>
        public void SetClass(CharacterClass characterClass)
        {
            Class = characterClass;
            // 可能需要重新调整属性
            AdjustStatsForClass();
        }

        /// <summary>
        /// 增加经验值
        /// </summary>
        /// <param name="amount">经验值</param>
        /// <returns>是否升级</returns>
        public bool AddExperience(int amount)
        {
            if (amount <= 0)
                return false;

            Experience += amount;

            // 检查是否可以升级
            if (Experience >= ExperienceToNextLevel)
            {
                return LevelUp();
            }

            return false;
        }

        /// <summary>
        /// 升级
        /// </summary>
        /// <returns>是否成功升级</returns>
        public bool LevelUp()
        {
            if (Level >= 99) // 假设最大等级为99
                return false;

            // 减去升级所需经验值
            Experience -= ExperienceToNextLevel;
            Level++;

            // 提升属性
            IncreaseStatsOnLevelUp();

            return true;
        }

        /// <summary>
        /// 学习技能
        /// </summary>
        /// <param name="skillId">技能ID</param>
        /// <returns>是否成功学习</returns>
        public bool LearnSkill(Guid skillId)
        {
            if (skillId == Guid.Empty)
                return false;

            if (_skillIds.Contains(skillId))
                return false;

            _skillIds.Add(skillId);
            return true;
        }

        /// <summary>
        /// 遗忘技能
        /// </summary>
        /// <param name="skillId">技能ID</param>
        /// <returns>是否成功遗忘</returns>
        public bool ForgetSkill(Guid skillId)
        {
            if (skillId == Guid.Empty)
                return false;

            return _skillIds.Remove(skillId);
        }

        /// <summary>
        /// 装备物品
        /// </summary>
        /// <param name="itemId">物品ID</param>
        /// <param name="slot">装备槽位</param>
        /// <returns>是否成功装备</returns>
        public bool EquipItem(Guid itemId, EquipmentSlot slot)
        {
            if (itemId == Guid.Empty)
                return false;

            _equipment[slot] = itemId;
            return true;
        }

        /// <summary>
        /// 卸下装备
        /// </summary>
        /// <param name="slot">装备槽位</param>
        /// <returns>卸下的物品ID，如果没有装备则返回Guid.Empty</returns>
        public Guid UnequipItem(EquipmentSlot slot)
        {
            if (!_equipment.ContainsKey(slot))
                return Guid.Empty;

            Guid itemId = _equipment[slot];
            _equipment[slot] = Guid.Empty;
            return itemId;
        }

        /// <summary>
        /// 计算战斗属性
        /// </summary>
        /// <returns>战斗属性</returns>
        public CharacterCombatStats CalculateCombatStats()
        {
            // 基础战斗属性
            CharacterCombatStats stats = new CharacterCombatStats(
                physicalAttack: Strength * 2,
                magicAttack: Intelligence * 2,
                physicalDefense: Constitution * 1.5f,
                magicDefense: Intelligence * 0.5f + Constitution * 0.5f,
                hitRate: 80 + Agility * 0.5f,
                evasionRate: Agility * 0.7f,
                criticalRate: Luck * 0.5f,
                criticalDamage: 150 + Luck * 0.2f,
                movement: Movement,
                attackRange: 1 // 默认攻击范围为1
            );

            // 这里应该加上装备的属性加成，但需要依赖其他模块
            // 需要在CharacterModule中实现，这里只返回基础属性

            return stats;
        }

        #region 辅助方法

        /// <summary>
        /// 计算升级所需经验值
        /// </summary>
        private int CalculateExperienceToNextLevel()
        {
            // 简单的经验计算公式：100 * 当前等级 * 当前等级的平方根
            return (int)(100 * Level * Math.Sqrt(Level));
        }

        /// <summary>
        /// 根据职业设置初始属性
        /// </summary>
        private void SetInitialStats()
        {
            // 所有职业的基础属性
            MaxHP = 100;
            MaxMP = 50;
            Strength = 10;
            Agility = 10;
            Intelligence = 10;
            Constitution = 10;
            Luck = 5;
            Movement = 5;

            // 根据职业调整属性
            AdjustStatsForClass();
        }

        /// <summary>
        /// 根据职业调整属性
        /// </summary>
        private void AdjustStatsForClass()
        {
            switch (Class)
            {
                // 战士
                case CharacterClass.Warrior:
                    MaxHP += 50;
                    MaxMP -= 20;
                    Strength += 5;
                    Constitution += 3;
                    Intelligence -= 2;
                    break;
                // 法师
                case CharacterClass.Mage:
                    MaxHP -= 20;
                    MaxMP += 50;
                    Intelligence += 5;
                    Strength -= 2;
                    Constitution -= 1;
                    break;
                // 弓箭手
                case CharacterClass.Archer:
                    Agility += 5;
                    Luck += 2;
                    MaxHP -= 10;
                    break;
                // 牧师
                case CharacterClass.Cleric:
                    MaxMP += 30;
                    Intelligence += 3;
                    Constitution += 1;
                    MaxHP -= 10;
                    break;
                // 骑士
                case CharacterClass.Knight:
                    MaxHP += 30;
                    Constitution += 5;
                    Strength += 2;
                    Agility -= 1;
                    Movement -= 1;
                    break;
                // 其他职业可以继续添加
                default:
                    break;
            }
        }

        /// <summary>
        /// 升级时提升属性
        /// </summary>
        private void IncreaseStatsOnLevelUp()
        {
            // 通用属性提升
            MaxHP += 10 + (int)(Constitution * 0.5);
            MaxMP += 5 + (int)(Intelligence * 0.3);

            // 职业特定属性提升
            switch (Class)
            {
                case CharacterClass.Warrior:
                    Strength += 2;
                    Constitution += 1;
                    if (Level % 3 == 0) Agility += 1;
                    if (Level % 5 == 0) Intelligence += 1;
                    break;
                case CharacterClass.Mage:
                    Intelligence += 2;
                    MaxMP += 10;
                    if (Level % 3 == 0) Constitution += 1;
                    if (Level % 5 == 0) Strength += 1;
                    break;
                case CharacterClass.Archer:
                    Agility += 2;
                    Strength += 1;
                    if (Level % 3 == 0) Luck += 1;
                    if (Level % 5 == 0) Constitution += 1;
                    break;
                case CharacterClass.Cleric:
                    Intelligence += 2;
                    MaxMP += 5;
                    if (Level % 3 == 0) Constitution += 1;
                    if (Level % 5 == 0) Luck += 1;
                    break;
                case CharacterClass.Knight:
                    Constitution += 2;
                    Strength += 1;
                    if (Level % 3 == 0) Intelligence += 1;
                    if (Level % 4 == 0) MaxHP += 10;
                    break;
                // 其他职业可以继续添加
                default:
                    break;
            }

            // 每5级增加1点移动力，最大不超过10
            if (Level % 5 == 0 && Movement < 10)
            {
                Movement += 1;
            }

            // 每10级增加一点幸运
            if (Level % 10 == 0)
            {
                Luck += 1;
            }
        }

        #endregion
    }
}