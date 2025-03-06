using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TacticalRPG.Core.Framework;
using TacticalRPG.Core.Framework.Events;
using TacticalRPG.Core.Modules.Character;
using TacticalRPG.Core.Modules.Equipment;
using TacticalRPG.Core.Modules.Inventory;
using TacticalRPG.Core.Modules.Skill;

namespace TacticalRPG.Implementation.Modules.Character
{
    /// <summary>
    /// 角色模块实现
    /// </summary>
    public class CharacterModule : ICharacterModule
    {
        private readonly IEventManager _eventManager;
        private readonly ISkillModule _skillModule;
        private readonly IInventoryModule _inventoryModule;
        private readonly IEquipmentModule _equipmentModule;

        private readonly Dictionary<Guid, ICharacter> _characters = new Dictionary<Guid, ICharacter>();
        private readonly Dictionary<Guid, List<Guid>> _characterSkills = new Dictionary<Guid, List<Guid>>();

        /// <summary>
        /// 模块名称
        /// </summary>
        public string ModuleName => "Character";

        /// <summary>
        /// 模块优先级
        /// </summary>
        public int Priority => 20;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="eventManager">事件管理器</param>
        /// <param name="skillModule">技能模块</param>
        /// <param name="inventoryModule">背包模块</param>
        /// <param name="equipmentModule">装备模块</param>
        public CharacterModule(
            IEventManager eventManager,
            ISkillModule skillModule,
            IInventoryModule inventoryModule,
            IEquipmentModule equipmentModule)
        {
            _eventManager = eventManager ?? throw new ArgumentNullException(nameof(eventManager));
            _skillModule = skillModule ?? throw new ArgumentNullException(nameof(skillModule));
            _inventoryModule = inventoryModule ?? throw new ArgumentNullException(nameof(inventoryModule));
            _equipmentModule = equipmentModule ?? throw new ArgumentNullException(nameof(equipmentModule));
        }

        /// <summary>
        /// 初始化模块
        /// </summary>
        public Task Initialize()
        {
            Console.WriteLine("角色模块初始化");
            return Task.CompletedTask;
        }

        /// <summary>
        /// 启动模块
        /// </summary>
        public Task Start()
        {
            Console.WriteLine("角色模块启动");
            return Task.CompletedTask;
        }

        /// <summary>
        /// 暂停模块
        /// </summary>
        public Task Pause()
        {
            Console.WriteLine("角色模块暂停");
            return Task.CompletedTask;
        }

        /// <summary>
        /// 恢复模块
        /// </summary>
        public Task Resume()
        {
            Console.WriteLine("角色模块恢复");
            return Task.CompletedTask;
        }

        /// <summary>
        /// 停止模块
        /// </summary>
        public Task Stop()
        {
            Console.WriteLine("角色模块停止");
            return Task.CompletedTask;
        }

        /// <summary>
        /// 卸载模块
        /// </summary>
        public Task Unload()
        {
            Console.WriteLine("角色模块卸载");
            return Task.CompletedTask;
        }

        /// <summary>
        /// 初始化模块（旧方法，保持与自定义接口兼容）
        /// </summary>
        public Task InitializeAsync()
        {
            Console.WriteLine("角色模块异步初始化");

            // 注册事件处理程序
            _eventManager.RegisterEvent<CharacterCreatedEventArgs>(OnCharacterCreated);
            _eventManager.RegisterEvent<CharacterDeletedEventArgs>(OnCharacterDeleted);
            _eventManager.RegisterEvent<CharacterLevelUpEventArgs>(OnCharacterLevelUp);
            _eventManager.RegisterEvent<CharacterSkillLearnedEventArgs>(OnCharacterSkillLearned);
            _eventManager.RegisterEvent<CharacterSkillForgottenEventArgs>(OnCharacterSkillForgotten);

            return Task.CompletedTask;
        }

        /// <summary>
        /// 关闭模块
        /// </summary>
        public Task ShutdownAsync()
        {
            Console.WriteLine("角色模块关闭");

            // 移除事件处理程序
            _eventManager.UnregisterEvent<CharacterCreatedEventArgs>(OnCharacterCreated);
            _eventManager.UnregisterEvent<CharacterDeletedEventArgs>(OnCharacterDeleted);
            _eventManager.UnregisterEvent<CharacterLevelUpEventArgs>(OnCharacterLevelUp);
            _eventManager.UnregisterEvent<CharacterSkillLearnedEventArgs>(OnCharacterSkillLearned);
            _eventManager.UnregisterEvent<CharacterSkillForgottenEventArgs>(OnCharacterSkillForgotten);

            return Task.CompletedTask;
        }

        /// <summary>
        /// 创建角色
        /// </summary>
        /// <param name="name">角色名称</param>
        /// <param name="characterClass">角色职业</param>
        /// <returns>新角色的ID</returns>
        public async Task<Guid> CreateCharacterAsync(string name, CharacterClass characterClass)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("角色名称不能为空", nameof(name));

            Guid id = Guid.NewGuid();
            Character character = new Character(id, name, characterClass);
            _characters[id] = character;
            _characterSkills[id] = new List<Guid>();

            // 创建初始装备和物品（可以根据职业不同设置不同的初始装备）
            await CreateInitialEquipmentAsync(character);

            // 触发角色创建事件
            await _eventManager.PublishAsync(new CharacterCreatedEventArgs(character));

            return id;
        }

        /// <summary>
        /// 获取角色
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <returns>角色对象</returns>
        public ICharacter GetCharacter(Guid characterId)
        {
            if (_characters.TryGetValue(characterId, out ICharacter character))
            {
                return character;
            }
            return null;
        }

        /// <summary>
        /// 获取所有角色
        /// </summary>
        /// <returns>所有角色的列表</returns>
        public IReadOnlyList<ICharacter> GetAllCharacters()
        {
            return _characters.Values.ToList().AsReadOnly();
        }

        /// <summary>
        /// 保存角色
        /// </summary>
        /// <param name="character">角色对象</param>
        /// <returns>操作结果</returns>
        public Task<bool> SaveCharacterAsync(ICharacter character)
        {
            if (character == null)
                return Task.FromResult(false);

            _characters[character.Id] = character;
            return Task.FromResult(true);
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <returns>操作结果</returns>
        public async Task<bool> DeleteCharacterAsync(Guid characterId)
        {
            if (!_characters.ContainsKey(characterId))
                return false;

            ICharacter character = _characters[characterId];
            _characters.Remove(characterId);
            _characterSkills.Remove(characterId);

            // 触发角色删除事件
            await _eventManager.RaiseEventAsync(new CharacterDeletedEventArgs(character));

            return true;
        }

        /// <summary>
        /// 为角色学习技能
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <param name="skillId">技能ID</param>
        /// <returns>操作结果</returns>
        public async Task<bool> LearnSkillAsync(Guid characterId, Guid skillId)
        {
            if (!_characters.TryGetValue(characterId, out ICharacter character))
                return false;

            ISkill skill = _skillModule.GetSkill(skillId);
            if (skill == null)
                return false;

            // 检查是否可以学习
            if (!skill.CanLearn(character))
                return false;

            if (character.LearnSkill(skillId))
            {
                if (!_characterSkills.ContainsKey(characterId))
                {
                    _characterSkills[characterId] = new List<Guid>();
                }

                if (!_characterSkills[characterId].Contains(skillId))
                {
                    _characterSkills[characterId].Add(skillId);
                }

                // 触发技能学习事件
                await _eventManager.RaiseEventAsync(new CharacterSkillLearnedEventArgs(character, skill));
                return true;
            }

            return false;
        }

        /// <summary>
        /// 为角色遗忘技能
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <param name="skillId">技能ID</param>
        /// <returns>操作结果</returns>
        public async Task<bool> ForgetSkillAsync(Guid characterId, Guid skillId)
        {
            if (!_characters.TryGetValue(characterId, out ICharacter character))
                return false;

            ISkill skill = _skillModule.GetSkill(skillId);
            if (skill == null)
                return false;

            if (character.ForgetSkill(skillId))
            {
                if (_characterSkills.ContainsKey(characterId))
                {
                    _characterSkills[characterId].Remove(skillId);
                }

                // 触发技能遗忘事件
                await _eventManager.RaiseEventAsync(new CharacterSkillForgottenEventArgs(character, skill));
                return true;
            }

            return false;
        }

        /// <summary>
        /// 获取角色的所有技能
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <returns>角色的所有技能</returns>
        public IReadOnlyList<ISkill> GetCharacterSkills(Guid characterId)
        {
            if (!_characterSkills.TryGetValue(characterId, out List<Guid> skillIds))
                return new List<ISkill>().AsReadOnly();

            List<ISkill> skills = new List<ISkill>();
            foreach (Guid skillId in skillIds)
            {
                ISkill skill = _skillModule.GetSkill(skillId);
                if (skill != null)
                {
                    skills.Add(skill);
                }
            }

            return skills.AsReadOnly();
        }

        /// <summary>
        /// 为角色装备物品
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <param name="itemId">物品ID</param>
        /// <param name="slot">装备槽位</param>
        /// <returns>操作结果</returns>
        public async Task<bool> EquipItemAsync(Guid characterId, Guid itemId, EquipmentSlot slot)
        {
            if (!_characters.TryGetValue(characterId, out ICharacter character))
                return false;

            // 检查物品是否存在及是否是装备
            IItem item = _inventoryModule.GetItem(itemId);
            if (item == null || item.Type != ItemType.Equipment)
                return false;

            // 检查是否存在于玩家的库存中
            if (!_inventoryModule.HasItem(characterId, itemId))
                return false;

            // 如果原来有装备，需要先卸下
            Guid oldItemId = character.UnequipItem(slot);
            if (oldItemId != Guid.Empty)
            {
                // 将原来的装备归还到库存
                await _inventoryModule.AddItemToInventoryAsync(characterId, oldItemId, 1);
            }

            // 从库存中移除要装备的物品
            await _inventoryModule.RemoveItemFromInventoryAsync(characterId, itemId, 1);

            // 在角色上装备
            if (character.EquipItem(itemId, slot))
            {
                return true;
            }
            else
            {
                // 如果装备失败，将物品归还到库存
                await _inventoryModule.AddItemToInventoryAsync(characterId, itemId, 1);
                return false;
            }
        }

        /// <summary>
        /// 为角色卸下装备
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <param name="slot">装备槽位</param>
        /// <returns>操作结果</returns>
        public async Task<bool> UnequipItemAsync(Guid characterId, EquipmentSlot slot)
        {
            if (!_characters.TryGetValue(characterId, out ICharacter character))
                return false;

            Guid itemId = character.UnequipItem(slot);
            if (itemId != Guid.Empty)
            {
                // 将卸下的装备添加到库存
                await _inventoryModule.AddItemToInventoryAsync(characterId, itemId, 1);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 获取角色的所有装备
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <returns>角色的所有装备</returns>
        public IReadOnlyDictionary<EquipmentSlot, Guid> GetCharacterEquipment(Guid characterId)
        {
            if (!_characters.TryGetValue(characterId, out ICharacter character))
                return new Dictionary<EquipmentSlot, Guid>().AsReadOnly();

            return character.Equipment;
        }

        /// <summary>
        /// 角色升级
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <returns>操作结果</returns>
        public async Task<bool> LevelUpCharacterAsync(Guid characterId)
        {
            if (!_characters.TryGetValue(characterId, out ICharacter character))
                return false;

            int oldLevel = character.Level;

            if (character.LevelUp())
            {
                // 触发角色升级事件
                await _eventManager.RaiseEventAsync(new CharacterLevelUpEventArgs(character, oldLevel, character.Level));
                return true;
            }

            return false;
        }

        /// <summary>
        /// 计算角色的战斗属性
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <returns>角色的战斗属性</returns>
        public CharacterCombatStats CalculateCombatStats(Guid characterId)
        {
            if (!_characters.TryGetValue(characterId, out ICharacter character))
                return new CharacterCombatStats();

            // 获取基础战斗属性
            CharacterCombatStats baseStats = character.CalculateCombatStats();

            // 获取装备提供的额外属性
            var equipmentStats = CalculateEquipmentStats(character);

            // 合并基础属性和装备属性
            CharacterCombatStats finalStats = new CharacterCombatStats(
                physicalAttack: baseStats.PhysicalAttack + equipmentStats.PhysicalAttack,
                magicAttack: baseStats.MagicAttack + equipmentStats.MagicAttack,
                physicalDefense: baseStats.PhysicalDefense + equipmentStats.PhysicalDefense,
                magicDefense: baseStats.MagicDefense + equipmentStats.MagicDefense,
                hitRate: baseStats.HitRate + equipmentStats.HitRate,
                evasionRate: baseStats.EvasionRate + equipmentStats.EvasionRate,
                criticalRate: baseStats.CriticalRate + equipmentStats.CriticalRate,
                criticalDamage: baseStats.CriticalDamage + equipmentStats.CriticalDamage,
                movement: baseStats.Movement + equipmentStats.Movement,
                attackRange: baseStats.AttackRange + equipmentStats.AttackRange
            );

            return finalStats;
        }

        #region 私有方法

        /// <summary>
        /// 计算装备提供的属性
        /// </summary>
        /// <param name="character">角色</param>
        /// <returns>装备属性</returns>
        private CharacterCombatStats CalculateEquipmentStats(ICharacter character)
        {
            if (_equipmentModule == null)
                return new CharacterCombatStats();

            return _equipmentModule.GetTotalEquipmentStats(character.Id);
        }

        /// <summary>
        /// 创建角色初始装备
        /// </summary>
        /// <param name="character">角色</param>
        private async Task CreateInitialEquipmentAsync(ICharacter character)
        {
            // 为不同职业创建初始装备
            switch (character.Class)
            {
                case CharacterClass.Warrior:
                    await CreateWarriorInitialEquipmentAsync(character);
                    break;
                case CharacterClass.Mage:
                    await CreateMageInitialEquipmentAsync(character);
                    break;
                case CharacterClass.Archer:
                    await CreateArcherInitialEquipmentAsync(character);
                    break;
                case CharacterClass.Priest:
                    await CreatePriestInitialEquipmentAsync(character);
                    break;
                case CharacterClass.Knight:
                    await CreateKnightInitialEquipmentAsync(character);
                    break;
                // 其他职业初始装备
                default:
                    await CreateDefaultInitialEquipmentAsync(character);
                    break;
            }
        }

        private async Task CreateWarriorInitialEquipmentAsync(ICharacter character)
        {
            // 创建战士的初始武器
            Guid swordId = await _equipmentModule.CreateWeaponAsync("初级战剑", "战士的初始武器", 10, 1, 1.0f, false);
            await _inventoryModule.AddItemToInventoryAsync(character.Id, swordId, 1);

            // 创建战士的初始防具
            Guid armorId = await _equipmentModule.CreateArmorAsync("皮甲", "战士的初始防具", 5, 2);
            await _inventoryModule.AddItemToInventoryAsync(character.Id, armorId, 1);

            // 给予一些恢复药水
            Guid potionId = await _inventoryModule.CreateItemAsync("初级生命药水", "恢复少量生命值", ItemType.Consumable);
            await _inventoryModule.AddItemToInventoryAsync(character.Id, potionId, 3);
        }

        private async Task CreateMageInitialEquipmentAsync(ICharacter character)
        {
            // 创建法师的初始武器
            Guid staffId = await _equipmentModule.CreateWeaponAsync("初级法杖", "法师的初始武器", 5, 2, 0.8f, false);
            await _inventoryModule.AddItemToInventoryAsync(character.Id, staffId, 1);

            // 创建法师的初始防具
            Guid robeId = await _equipmentModule.CreateArmorAsync("学徒法袍", "法师的初始防具", 2, 5);
            await _inventoryModule.AddItemToInventoryAsync(character.Id, robeId, 1);

            // 给予一些魔法药水
            Guid manaId = await _inventoryModule.CreateItemAsync("初级魔法药水", "恢复少量魔法值", ItemType.Consumable);
            await _inventoryModule.AddItemToInventoryAsync(character.Id, manaId, 3);
        }

        private async Task CreateArcherInitialEquipmentAsync(ICharacter character)
        {
            // 创建弓箭手的初始武器
            Guid bowId = await _equipmentModule.CreateWeaponAsync("初级短弓", "弓箭手的初始武器", 8, 3, 1.2f, false);
            await _inventoryModule.AddItemToInventoryAsync(character.Id, bowId, 1);

            // 创建弓箭手的初始防具
            Guid leatherId = await _equipmentModule.CreateArmorAsync("轻皮甲", "弓箭手的初始防具", 3, 3);
            await _inventoryModule.AddItemToInventoryAsync(character.Id, leatherId, 1);

            // 给予一些箭矢
            Guid arrowId = await _inventoryModule.CreateItemAsync("普通箭矢", "基础箭矢", ItemType.Ammunition);
            await _inventoryModule.AddItemToInventoryAsync(character.Id, arrowId, 20);
        }

        private async Task CreatePriestInitialEquipmentAsync(ICharacter character)
        {
            // 创建牧师的初始武器
            Guid maceId = await _equipmentModule.CreateWeaponAsync("初级权杖", "牧师的初始武器", 6, 1, 0.9f, false);
            await _inventoryModule.AddItemToInventoryAsync(character.Id, maceId, 1);

            // 创建牧师的初始防具
            Guid clothId = await _equipmentModule.CreateArmorAsync("祭司长袍", "牧师的初始防具", 2, 4);
            await _inventoryModule.AddItemToInventoryAsync(character.Id, clothId, 1);

            // 给予一些治疗药水
            Guid healId = await _inventoryModule.CreateItemAsync("初级治疗药水", "治疗少量生命值", ItemType.Consumable);
            await _inventoryModule.AddItemToInventoryAsync(character.Id, healId, 5);
        }

        private async Task CreateKnightInitialEquipmentAsync(ICharacter character)
        {
            // 创建骑士的初始武器
            Guid swordId = await _equipmentModule.CreateWeaponAsync("初级长剑", "骑士的初始武器", 12, 1, 0.8f, false);
            await _inventoryModule.AddItemToInventoryAsync(character.Id, swordId, 1);

            // 创建骑士的初始防具
            Guid plateId = await _equipmentModule.CreateArmorAsync("初级板甲", "骑士的初始防具", 8, 1);
            await _inventoryModule.AddItemToInventoryAsync(character.Id, plateId, 1);

            // 创建骑士的初始盾牌
            Guid shieldId = await _equipmentModule.CreateEquipmentAsync("初级盾牌", "骑士的初始盾牌");
            _equipmentModule.AddStatToEquipment(shieldId, EquipmentStatType.PhysicalDefense, 5);
            await _inventoryModule.AddItemToInventoryAsync(character.Id, shieldId, 1);
        }

        private async Task CreateDefaultInitialEquipmentAsync(ICharacter character)
        {
            // 创建通用初始武器
            Guid daggerId = await _equipmentModule.CreateWeaponAsync("初级匕首", "通用初始武器", 5, 1, 1.0f, false);
            await _inventoryModule.AddItemToInventoryAsync(character.Id, daggerId, 1);

            // 创建通用初始防具
            Guid clothId = await _equipmentModule.CreateArmorAsync("初级布衣", "通用初始防具", 3, 3);
            await _inventoryModule.AddItemToInventoryAsync(character.Id, clothId, 1);

            // 给予一些通用物品
            Guid foodId = await _inventoryModule.CreateItemAsync("干粮", "恢复少量生命值", ItemType.Consumable);
            await _inventoryModule.AddItemToInventoryAsync(character.Id, foodId, 3);
        }

        #endregion

        #region 事件处理

        private void OnCharacterCreated(CharacterCreatedEventArgs args)
        {
            Console.WriteLine($"角色 {args.Character.Name} 被创建了！");
        }

        private void OnCharacterDeleted(CharacterDeletedEventArgs args)
        {
            Console.WriteLine($"角色 {args.Character.Name} 被删除了！");
        }

        private void OnCharacterLevelUp(CharacterLevelUpEventArgs args)
        {
            Console.WriteLine($"角色 {args.Character.Name} 从 {args.OldLevel} 级升到了 {args.NewLevel} 级！");
        }

        private void OnCharacterSkillLearned(CharacterSkillLearnedEventArgs args)
        {
            Console.WriteLine($"角色 {args.Character.Name} 学会了技能 {args.Skill.Name}！");
        }

        private void OnCharacterSkillForgotten(CharacterSkillForgottenEventArgs args)
        {
            Console.WriteLine($"角色 {args.Character.Name} 遗忘了技能 {args.Skill.Name}！");
        }

        #endregion
    }

    #region 事件参数类

    /// <summary>
    /// 角色创建事件参数
    /// </summary>
    public class CharacterCreatedEventArgs : EventArgs
    {
        public ICharacter Character { get; }

        public CharacterCreatedEventArgs(ICharacter character)
        {
            Character = character;
        }
    }

    /// <summary>
    /// 角色删除事件参数
    /// </summary>
    public class CharacterDeletedEventArgs : EventArgs
    {
        public ICharacter Character { get; }

        public CharacterDeletedEventArgs(ICharacter character)
        {
            Character = character;
        }
    }

    /// <summary>
    /// 角色升级事件参数
    /// </summary>
    public class CharacterLevelUpEventArgs : EventArgs
    {
        public ICharacter Character { get; }
        public int OldLevel { get; }
        public int NewLevel { get; }

        public CharacterLevelUpEventArgs(ICharacter character, int oldLevel, int newLevel)
        {
            Character = character;
            OldLevel = oldLevel;
            NewLevel = newLevel;
        }
    }

    /// <summary>
    /// 角色学习技能事件参数
    /// </summary>
    public class CharacterSkillLearnedEventArgs : EventArgs
    {
        public ICharacter Character { get; }
        public ISkill Skill { get; }

        public CharacterSkillLearnedEventArgs(ICharacter character, ISkill skill)
        {
            Character = character;
            Skill = skill;
        }
    }

    /// <summary>
    /// 角色遗忘技能事件参数
    /// </summary>
    public class CharacterSkillForgottenEventArgs : EventArgs
    {
        public ICharacter Character { get; }
        public ISkill Skill { get; }

        public CharacterSkillForgottenEventArgs(ICharacter character, ISkill skill)
        {
            Character = character;
            Skill = skill;
        }
    }

    #endregion
}