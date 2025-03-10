using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TacticalRPG.Core.Framework;
using TacticalRPG.Core.Modules.Character;
using TacticalRPG.Core.Modules.Equipment;
using TacticalRPG.Core.Modules.Item;
using TacticalRPG.Core.Modules.Inventory;

namespace TacticalRPG.Implementation.Modules.Equipment
{
    /// <summary>
    /// 装备模块实现类
    /// </summary>
    public class EquipmentModule : BaseGameModule, IEquipmentModule
    {
        private readonly ILogger<EquipmentModule> _logger;
        private readonly IEquipmentFactory _equipmentFactory;
        private readonly IItemModule _itemModule;
        private readonly Dictionary<Guid, Dictionary<EquipmentSlot, Guid>> _characterEquipment = new Dictionary<Guid, Dictionary<EquipmentSlot, Guid>>();

        /// <summary>
        /// 获取装备工厂实例
        /// </summary>
        public IEquipmentFactory EquipmentFactory => _equipmentFactory;

        /// <summary>
        /// 获取模块名称
        /// </summary>
        public override string ModuleName => "装备管理模块";

        /// <summary>
        /// 获取模块优先级
        /// </summary>
        public override int Priority => 45; // 装备模块优先级低于物品模块

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="gameSystem">游戏系统</param>
        /// <param name="logger">日志记录器</param>
        /// <param name="equipmentFactory">装备工厂</param>
        /// <param name="itemModule">物品模块</param>
        public EquipmentModule(
            IGameSystem gameSystem,
            ILogger<EquipmentModule> logger,
            IEquipmentFactory equipmentFactory,
            IItemModule itemModule) : base(gameSystem, logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _equipmentFactory = equipmentFactory ?? throw new ArgumentNullException(nameof(equipmentFactory));
            _itemModule = itemModule ?? throw new ArgumentNullException(nameof(itemModule));
        }

        /// <summary>
        /// 初始化模块
        /// </summary>
        public override async Task Initialize()
        {
            _logger.LogInformation("正在初始化{ModuleName}...", ModuleName);
            await base.Initialize();
            _logger.LogInformation("{ModuleName}初始化完成", ModuleName);
        }

        /// <summary>
        /// 根据ID获取装备
        /// </summary>
        /// <param name="equipmentId">装备ID</param>
        /// <returns>找到的装备实例，未找到则返回null</returns>
        public IEquipment GetEquipment(Guid equipmentId)
        {
            try
            {
                var itemTask = _itemModule.GetItemAsync(equipmentId);
                itemTask.Wait();
                var item = itemTask.Result;

                if (item == null || !item.IsEquippable)
                {
                    _logger.LogWarning("尝试获取不存在或非装备的物品: {EquipmentId}", equipmentId);
                    return null;
                }

                // 这里假设装备信息已存储在物品的属性中
                // 实际实现中可能需要更复杂的逻辑将物品转换为装备
                return ConvertItemToEquipment(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取装备时发生错误: {EquipmentId}", equipmentId);
                return null;
            }
        }

        /// <summary>
        /// 获取武器类装备
        /// </summary>
        /// <param name="equipmentId">装备ID</param>
        /// <returns>找到的武器实例，未找到或不是武器则返回null</returns>
        public IWeapon GetWeapon(Guid equipmentId)
        {
            var equipment = GetEquipment(equipmentId);
            if (equipment == null || equipment.Type != EquipmentType.Weapon)
            {
                return null;
            }

            return equipment as IWeapon;
        }

        /// <summary>
        /// 注册装备到系统
        /// </summary>
        /// <param name="equipment">装备实例</param>
        /// <returns>注册后的装备ID</returns>
        public Guid RegisterEquipment(IEquipment equipment)
        {
            if (equipment == null)
            {
                throw new ArgumentNullException(nameof(equipment));
            }

            try
            {
                // 为装备创建对应的物品
                var createTask = _itemModule.CreateItemAsync($"equip_{equipment.Type}_{equipment.Rarity}", 1);
                createTask.Wait();
                var itemId = createTask.Result;

                // 设置物品属性以匹配装备属性
                SetItemPropertiesFromEquipment(itemId, equipment);

                _logger.LogInformation("装备已注册: {EquipmentName} (ID: {ItemId})", equipment.Name, itemId);
                return itemId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "注册装备时发生错误: {EquipmentName}", equipment.Name);
                throw;
            }
        }

        /// <summary>
        /// 创建并注册基础装备
        /// </summary>
        public Guid CreateEquipment(
            string name,
            string description,
            EquipmentType type,
            EquipmentRarity rarity,
            Dictionary<EquipmentStatType, float> stats,
            int level = 1,
            IEnumerable<CharacterClass>? allowedClasses = null)
        {
            try
            {
                // 使用装备工厂创建装备
                IEquipment equipment;

                if (type == EquipmentType.Weapon)
                {
                    // 对于武器，创建IWeapon实例
                    var weaponStats = stats ?? new Dictionary<EquipmentStatType, float>();
                    float baseDamage = weaponStats.ContainsKey(EquipmentStatType.PhysicalDamage) ?
                        weaponStats[EquipmentStatType.PhysicalDamage] : 10;
                    float attackSpeed = weaponStats.ContainsKey(EquipmentStatType.AttackSpeed) ?
                        weaponStats[EquipmentStatType.AttackSpeed] : 1.0f;

                    equipment = _equipmentFactory.CreateWeapon(
                        name,
                        description,
                        WeaponType.Sword, // 默认值，实际应从参数获取
                        rarity,
                        (int)baseDamage,
                        1, // 默认攻击范围
                        attackSpeed,
                        stats,
                        level,
                        false); // 是否双手武器
                }
                else
                {
                    // 创建普通装备
                    equipment = _equipmentFactory.CreateEquipment(
                        name,
                        description,
                        type,
                        rarity,
                        level,
                        stats);

                    // 设置允许的职业
                    if (allowedClasses != null)
                    {
                        foreach (var charClass in allowedClasses)
                        {
                            equipment.AddAllowedClass(charClass);
                        }
                    }
                }

                // 将装备注册到系统
                Guid equipmentId = RegisterEquipment(equipment);

                // 创建装备对应的物品模板
                string templateId = CreateItemTemplateFromEquipment(equipmentId);

                // 创建物品实例
                var createItemTask = _itemModule.CreateItemAsync(templateId, 1);
                createItemTask.Wait();
                Guid itemId = createItemTask.Result;

                // 更新物品位置信息为未分配状态
                var updateLocationTask = _itemModule.UpdateItemLocationAsync(
                    itemId,
                    ItemLocation.Unassigned);
                updateLocationTask.Wait();

                _logger.LogInformation("创建装备: {EquipmentName} (ID: {EquipmentId}, ItemID: {ItemId})",
                    name, equipmentId, itemId);

                // 返回物品ID，而不是装备ID
                return itemId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "创建装备时发生错误: {EquipmentName}", name);
                throw;
            }
        }

        /// <summary>
        /// 装备物品到角色
        /// </summary>
        public bool EquipItem(Guid characterId, Guid equipmentId, EquipmentSlot slot)
        {
            try
            {
                // 检查物品是否存在且是装备
                var getItemTask = _itemModule.GetItemAsync(equipmentId);
                getItemTask.Wait();
                var item = getItemTask.Result;

                if (item == null || !item.IsEquippable)
                {
                    _logger.LogWarning("尝试装备不存在或非装备物品: {EquipmentId}", equipmentId);
                    return false;
                }

                // 获取当前角色的装备槽
                if (!_characterEquipment.TryGetValue(characterId, out var equipmentSlots))
                {
                    equipmentSlots = new Dictionary<EquipmentSlot, Guid>();
                    _characterEquipment[characterId] = equipmentSlots;
                }

                // 如果槽位已有物品，先卸下
                if (equipmentSlots.TryGetValue(slot, out var currentEquipId) && currentEquipId != Guid.Empty)
                {
                    UnequipItem(characterId, slot);
                }

                // 更新物品位置信息
                var location = ItemLocation.Create(
                    ItemContainerType.EquipmentSlot,
                    characterId,
                    (int)slot,
                    characterId);

                var updateLocationTask = _itemModule.UpdateItemLocationAsync(equipmentId, location);
                updateLocationTask.Wait();

                if (!updateLocationTask.Result)
                {
                    _logger.LogWarning("更新物品位置失败: {EquipmentId}", equipmentId);
                    return false;
                }

                // 更新角色装备信息
                equipmentSlots[slot] = equipmentId;

                _logger.LogInformation("角色{CharacterId}装备物品: {EquipmentId} 到槽位: {Slot}",
                    characterId, equipmentId, slot);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "装备物品时发生错误: 角色{CharacterId}, 物品{EquipmentId}, 槽位{Slot}",
                    characterId, equipmentId, slot);
                return false;
            }
        }

        /// <summary>
        /// 从角色卸下装备
        /// </summary>
        public Guid? UnequipItem(Guid characterId, EquipmentSlot slot)
        {
            try
            {
                // 获取当前角色的装备槽
                if (!_characterEquipment.TryGetValue(characterId, out var equipmentSlots) ||
                    !equipmentSlots.TryGetValue(slot, out var equipmentId) ||
                    equipmentId == Guid.Empty)
                {
                    _logger.LogWarning("角色{CharacterId}的槽位{Slot}没有装备", characterId, slot);
                    return null;
                }

                // 更新物品位置为未分配
                var updateLocationTask = _itemModule.UpdateItemLocationAsync(equipmentId, ItemLocation.Unassigned);
                updateLocationTask.Wait();

                if (!updateLocationTask.Result)
                {
                    _logger.LogWarning("更新物品位置失败: {EquipmentId}", equipmentId);
                    return null;
                }

                // 更新角色装备信息
                equipmentSlots.Remove(slot);

                _logger.LogInformation("角色{CharacterId}卸下装备: {EquipmentId} 从槽位: {Slot}",
                    characterId, equipmentId, slot);

                return equipmentId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "卸下装备时发生错误: 角色{CharacterId}, 槽位{Slot}", characterId, slot);
                return null;
            }
        }

        /// <summary>
        /// 卸下角色的所有装备
        /// </summary>
        public IReadOnlyList<Guid> UnequipAllItems(Guid characterId)
        {
            try
            {
                // 获取当前角色的装备槽
                if (!_characterEquipment.TryGetValue(characterId, out var equipmentSlots) ||
                    equipmentSlots.Count == 0)
                {
                    _logger.LogWarning("角色{CharacterId}没有装备", characterId);
                    return new List<Guid>();
                }

                var unequippedItems = new List<Guid>();

                // 复制装备槽字典的键，以避免在迭代过程中修改集合
                var slots = equipmentSlots.Keys.ToList();

                foreach (var slot in slots)
                {
                    var equipmentId = UnequipItem(characterId, slot);
                    if (equipmentId.HasValue && equipmentId.Value != Guid.Empty)
                    {
                        unequippedItems.Add(equipmentId.Value);
                    }
                }

                _logger.LogInformation("角色{CharacterId}卸下所有装备: {Count}件", characterId, unequippedItems.Count);
                return unequippedItems;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "卸下所有装备时发生错误: 角色{CharacterId}", characterId);
                return new List<Guid>();
            }
        }

        /// <summary>
        /// 获取角色的装备列表
        /// </summary>
        public IReadOnlyDictionary<EquipmentSlot, Guid> GetCharacterEquipment(Guid characterId)
        {
            if (!_characterEquipment.TryGetValue(characterId, out var equipmentSlots))
            {
                return new Dictionary<EquipmentSlot, Guid>();
            }

            return equipmentSlots;
        }

        /// <summary>
        /// 获取角色特定槽位的装备
        /// </summary>
        public Guid? GetEquippedItem(Guid characterId, EquipmentSlot slot)
        {
            if (!_characterEquipment.TryGetValue(characterId, out var equipmentSlots) ||
                !equipmentSlots.TryGetValue(slot, out var equipmentId))
            {
                return null;
            }

            return equipmentId;
        }

        /// <summary>
        /// 创建装备对应的物品模板
        /// </summary>
        /// <param name="equipmentId">装备ID</param>
        /// <returns>创建的物品模板ID</returns>
        public string CreateItemTemplateFromEquipment(Guid equipmentId)
        {
            if (equipmentId == Guid.Empty)
                throw new ArgumentException("装备ID不能为空", nameof(equipmentId));

            // 获取装备实例
            var equipment = GetEquipment(equipmentId);
            if (equipment == null)
                throw new ArgumentException($"找不到ID为{equipmentId}的装备", nameof(equipmentId));

            // 为这个装备创建唯一的模板ID
            string templateId = $"equipment_{equipment.Type.ToString().ToLower()}_{Guid.NewGuid():N}";

            // 通过物品模块创建物品模板
            var createTemplateTask = _itemModule.CreateItemTemplateAsync(templateId, equipment.Name);
            createTemplateTask.Wait();

            // 设置模板基本属性
            var tasks = new List<Task<bool>>();
            tasks.Add(_itemModule.SetTemplateAttributeAsync(templateId, "Name", equipment.Name));
            tasks.Add(_itemModule.SetTemplateAttributeAsync(templateId, "Description", equipment.Description));
            tasks.Add(_itemModule.SetTemplateAttributeAsync(templateId, "Type", "Equipment"));
            tasks.Add(_itemModule.SetTemplateAttributeAsync(templateId, "Rarity", equipment.Rarity));
            tasks.Add(_itemModule.SetTemplateAttributeAsync(templateId, "IsStackable", false));
            tasks.Add(_itemModule.SetTemplateAttributeAsync(templateId, "MaxStackSize", 1));
            tasks.Add(_itemModule.SetTemplateAttributeAsync(templateId, "Weight", equipment.Weight));
            tasks.Add(_itemModule.SetTemplateAttributeAsync(templateId, "Value", equipment.Value));
            tasks.Add(_itemModule.SetTemplateAttributeAsync(templateId, "IsUsable", false));
            tasks.Add(_itemModule.SetTemplateAttributeAsync(templateId, "IsEquippable", true));
            tasks.Add(_itemModule.SetTemplateAttributeAsync(templateId, "Level", equipment.Level));
            tasks.Add(_itemModule.SetTemplateAttributeAsync(templateId, "Durability", equipment.Durability));
            tasks.Add(_itemModule.SetTemplateAttributeAsync(templateId, "MaxDurability", equipment.MaxDurability));

            // 根据装备类型设置装备槽
            EquipmentSlot equipSlot = DetermineEquipmentSlot(equipment);
            tasks.Add(_itemModule.SetTemplateAttributeAsync(templateId, "EquipSlot", equipSlot));

            // 添加装备属性
            foreach (var stat in equipment.Stats)
            {
                tasks.Add(_itemModule.SetTemplateAttributeAsync(templateId, $"Stat_{stat.Key}", stat.Value));
            }

            // 添加武器特有属性
            if (equipment is IWeapon weapon)
            {
                tasks.Add(_itemModule.SetTemplateAttributeAsync(templateId, "WeaponType", weapon.WeaponType));
                tasks.Add(_itemModule.SetTemplateAttributeAsync(templateId, "BaseDamage", weapon.BaseDamage));
                tasks.Add(_itemModule.SetTemplateAttributeAsync(templateId, "AttackRange", weapon.AttackRange));
                tasks.Add(_itemModule.SetTemplateAttributeAsync(templateId, "AttackSpeed", weapon.AttackSpeed));
                tasks.Add(_itemModule.SetTemplateAttributeAsync(templateId, "IsTwoHanded", weapon.IsTwoHanded));
            }

            // 记录装备与模板的关联
            tasks.Add(_itemModule.SetTemplateAttributeAsync(templateId, "EquipmentId", equipmentId));

            // 等待所有任务完成
            Task.WaitAll(tasks.ToArray());

            _logger.LogInformation("从装备{EquipmentName} (ID: {EquipmentId})创建物品模板: {TemplateId}",
                equipment.Name, equipmentId, templateId);

            return templateId;
        }

        /// <summary>
        /// 从物品ID获取装备数据
        /// </summary>
        /// <param name="itemId">物品ID</param>
        /// <returns>装备数据</returns>
        public IEquipment GetEquipmentFromItem(Guid itemId)
        {
            try
            {
                // 获取物品
                var getItemTask = _itemModule.GetItemAsync(itemId);
                getItemTask.Wait();
                var item = getItemTask.Result;

                if (item == null)
                {
                    _logger.LogWarning("尝试获取不存在的物品: {ItemId}", itemId);
                    return null;
                }

                // 根据物品属性创建装备
                return ConvertItemToEquipment(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "从物品获取装备时发生错误: {ItemId}", itemId);
                return null;
            }
        }

        /// <summary>
        /// 将物品转换为装备实例
        /// </summary>
        private IEquipment ConvertItemToEquipment(IItem item)
        {
            // 实际实现中需要根据物品的属性创建装备实例
            // 这里只是示例，实际实现可能更复杂
            var getAttributeTask = _itemModule.GetItemAttributeAsync(item.Id, "EquipmentType");
            getAttributeTask.Wait();
            var equipTypeStr = getAttributeTask.Result?.ToString();

            if (string.IsNullOrEmpty(equipTypeStr) || !Enum.TryParse<EquipmentType>(equipTypeStr, out var equipType))
            {
                return null;
            }

            // 实际实现中，应该根据存储在物品属性中的装备数据创建装备实例
            // 这里假设有一个从物品创建装备的工厂方法
            return _equipmentFactory.CreateFromItemAttributes(item.Id);
        }

        /// <summary>
        /// 将装备属性设置到物品中
        /// </summary>
        private void SetItemPropertiesFromEquipment(Guid itemId, IEquipment equipment)
        {
            var tasks = new List<Task<bool>>();

            // 设置基本属性
            tasks.Add(_itemModule.SetItemAttributeAsync(itemId, "Name", equipment.Name));
            tasks.Add(_itemModule.SetItemAttributeAsync(itemId, "Description", equipment.Description));
            tasks.Add(_itemModule.SetItemAttributeAsync(itemId, "EquipmentType", equipment.Type));
            tasks.Add(_itemModule.SetItemAttributeAsync(itemId, "EquipmentRarity", equipment.Rarity));
            tasks.Add(_itemModule.SetItemAttributeAsync(itemId, "Level", equipment.Level));
            tasks.Add(_itemModule.SetItemAttributeAsync(itemId, "Durability", equipment.Durability));
            tasks.Add(_itemModule.SetItemAttributeAsync(itemId, "MaxDurability", equipment.MaxDurability));
            tasks.Add(_itemModule.SetItemAttributeAsync(itemId, "Value", equipment.Value));
            tasks.Add(_itemModule.SetItemAttributeAsync(itemId, "Weight", equipment.Weight));

            // 设置装备的属性
            foreach (var stat in equipment.Stats)
            {
                tasks.Add(_itemModule.SetItemAttributeAsync(itemId, $"Stat_{stat.Key}", stat.Value));
            }

            // 设置其他属性
            if (equipment.GetType() == typeof(Weapon))
            {
                var weapon = (IWeapon)equipment;
                tasks.Add(_itemModule.SetItemAttributeAsync(itemId, "WeaponType", weapon.WeaponType));
                tasks.Add(_itemModule.SetItemAttributeAsync(itemId, "BaseDamage", weapon.BaseDamage));
                tasks.Add(_itemModule.SetItemAttributeAsync(itemId, "AttackRange", weapon.AttackRange));
                tasks.Add(_itemModule.SetItemAttributeAsync(itemId, "AttackSpeed", weapon.AttackSpeed));
                tasks.Add(_itemModule.SetItemAttributeAsync(itemId, "IsTwoHanded", weapon.IsTwoHanded));
            }

            // 等待所有任务完成
            Task.WaitAll(tasks.ToArray());
        }

        /// <summary>
        /// 根据装备类型确定装备槽位
        /// </summary>
        private EquipmentSlot DetermineEquipmentSlot(IEquipment equipment)
        {
            switch (equipment.Type)
            {
                case EquipmentType.Weapon:
                    var weaponItem = equipment as IWeapon;
                    return weaponItem != null && weaponItem.IsTwoHanded ?
                        EquipmentSlot.MainHand : EquipmentSlot.MainHand;
                case EquipmentType.Shield:
                    return EquipmentSlot.OffHand;
                case EquipmentType.Head:
                    return EquipmentSlot.Head;
                case EquipmentType.Body:
                    return EquipmentSlot.Body;
                case EquipmentType.Hands:
                    return EquipmentSlot.Hands;
                case EquipmentType.Feet:
                    return EquipmentSlot.Feet;
                default:
                    return EquipmentSlot.None;
            }
        }
    }
}