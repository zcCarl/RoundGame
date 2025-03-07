using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TacticalRPG.Core.Framework;
using TacticalRPG.Core.Modules.Character;

namespace TacticalRPG.Core.Modules.Buff
{
    /// <summary>
    /// Buff模块实现类
    /// </summary>
    public class BuffModule : BaseGameModule, IBuffModule
    {
        private readonly Dictionary<Guid, IBuff> _buffs;
        private readonly Dictionary<Guid, List<Guid>> _characterBuffs;
        private readonly IBuffFactory _buffFactory;

        /// <summary>
        /// 获取Buff工厂
        /// </summary>
        public IBuffFactory BuffFactory => _buffFactory;

        /// <summary>
        /// 模块名称
        /// </summary>
        public override string ModuleName => "Buff";

        /// <summary>
        /// 模块优先级
        /// </summary>
        public override int Priority => 40;

        /// <summary>
        /// 构造函数
        /// </summary>
        public BuffModule(IGameSystem gameSystem, ILogger<BuffModule> logger)
            : base(gameSystem, logger)
        {
            _buffs = new Dictionary<Guid, IBuff>();
            _characterBuffs = new Dictionary<Guid, List<Guid>>();
            _buffFactory = new BuffFactory();
        }

        /// <summary>
        /// 初始化模块
        /// </summary>
        /// <returns>操作结果</returns>
        public Task<bool> InitializeAsync()
        {
            return Task.FromResult(true);
        }

        /// <summary>
        /// 创建Buff
        /// </summary>
        /// <param name="name">Buff名称</param>
        /// <param name="description">Buff描述</param>
        /// <param name="buffType">Buff类型</param>
        /// <param name="statusEffectType">状态效果类型</param>
        /// <param name="duration">持续回合数</param>
        /// <param name="power">效果强度</param>
        /// <param name="isStackable">是否可叠加</param>
        /// <param name="maxStackCount">最大叠加层数</param>
        /// <returns>新Buff的ID</returns>
        public async Task<Guid> CreateBuffAsync(string name, string description, BuffType buffType, StatusEffectType statusEffectType,
            int duration, int power, bool isStackable = false, int maxStackCount = 1)
        {
            var buff = new BaseBuff(name, description, buffType, statusEffectType, duration, power, isStackable, maxStackCount);

            await SaveBuffAsync(buff);

            return buff.Id;
        }

        /// <summary>
        /// 获取Buff
        /// </summary>
        /// <param name="buffId">Buff ID</param>
        /// <returns>Buff对象</returns>
        public IBuff GetBuff(Guid buffId)
        {
            if (_buffs.TryGetValue(buffId, out var buff))
            {
                return buff;
            }

            return null;
        }

        /// <summary>
        /// 获取所有Buff
        /// </summary>
        /// <returns>所有Buff的列表</returns>
        public IReadOnlyList<IBuff> GetAllBuffs()
        {
            return _buffs.Values.ToList();
        }

        /// <summary>
        /// 获取指定类型的所有Buff
        /// </summary>
        /// <param name="buffType">Buff类型</param>
        /// <returns>指定类型的所有Buff</returns>
        public IReadOnlyList<IBuff> GetBuffsByType(BuffType buffType)
        {
            return _buffs.Values.Where(b => b.Type == buffType).ToList();
        }

        /// <summary>
        /// 获取指定状态效果类型的所有Buff
        /// </summary>
        /// <param name="statusEffectType">状态效果类型</param>
        /// <returns>指定状态效果类型的所有Buff</returns>
        public IReadOnlyList<IBuff> GetBuffsByStatusEffectType(StatusEffectType statusEffectType)
        {
            return _buffs.Values.Where(b => b.StatusEffectType == statusEffectType).ToList();
        }

        /// <summary>
        /// 保存Buff
        /// </summary>
        /// <param name="buff">Buff对象</param>
        /// <returns>操作结果</returns>
        public Task<bool> SaveBuffAsync(IBuff buff)
        {
            if (buff == null)
                return Task.FromResult(false);

            _buffs[buff.Id] = buff;
            return Task.FromResult(true);
        }

        /// <summary>
        /// 删除Buff
        /// </summary>
        /// <param name="buffId">Buff ID</param>
        /// <returns>操作结果</returns>
        public Task<bool> DeleteBuffAsync(Guid buffId)
        {
            if (!_buffs.ContainsKey(buffId))
                return Task.FromResult(false);

            // 从所有角色中移除该Buff
            foreach (var characterId in _characterBuffs.Keys)
            {
                if (_characterBuffs[characterId].Contains(buffId))
                {
                    _characterBuffs[characterId].Remove(buffId);
                }
            }

            _buffs.Remove(buffId);
            return Task.FromResult(true);
        }

        /// <summary>
        /// 给角色应用Buff
        /// </summary>
        /// <param name="buffId">Buff ID</param>
        /// <param name="caster">施法者</param>
        /// <param name="target">目标</param>
        /// <returns>操作结果</returns>
        public async Task<bool> ApplyBuffToCharacterAsync(Guid buffId, ICombatCharacter caster, ICombatCharacter target)
        {
            if (target == null || !_buffs.TryGetValue(buffId, out var buff))
                return false;

            // 设置施法者和目标
            buff.SetCaster(caster);
            buff.SetTarget(target);

            // 检查目标是否已有相同类型的Buff
            if (_characterBuffs.TryGetValue(target.Id, out var buffs))
            {
                var existingBuff = buffs
                    .Select(id => GetBuff(id))
                    .FirstOrDefault(b => b != null &&
                                        b.Type == buff.Type &&
                                        b.StatusEffectType == buff.StatusEffectType);

                if (existingBuff != null)
                {
                    // 如果已有相同类型的Buff且可叠加
                    if (existingBuff.IsStackable)
                    {
                        existingBuff.AddStack();
                        return true;
                    }
                    else
                    {
                        // 如果不可叠加，则移除旧的，应用新的
                        await RemoveBuffFromCharacterAsync(target.Id, existingBuff.Id);
                    }
                }
            }

            // 应用Buff效果
            bool result = await buff.ApplyAsync();

            if (result)
            {
                // 将Buff添加到角色的Buff列表中
                if (!_characterBuffs.ContainsKey(target.Id))
                {
                    _characterBuffs[target.Id] = new List<Guid>();
                }

                _characterBuffs[target.Id].Add(buffId);
            }

            return result;
        }

        /// <summary>
        /// 从角色移除Buff
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <param name="buffId">Buff ID</param>
        /// <returns>操作结果</returns>
        public async Task<bool> RemoveBuffFromCharacterAsync(Guid characterId, Guid buffId)
        {
            if (!_characterBuffs.TryGetValue(characterId, out var buffs) || !buffs.Contains(buffId))
                return false;

            if (!_buffs.TryGetValue(buffId, out var buff))
                return false;

            // 移除Buff效果
            bool result = await buff.RemoveAsync();

            if (result)
            {
                // 从角色的Buff列表中移除
                buffs.Remove(buffId);
            }

            return result;
        }

        /// <summary>
        /// 从角色移除所有Buff
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <returns>操作结果</returns>
        public async Task<bool> RemoveAllBuffsFromCharacterAsync(Guid characterId)
        {
            if (!_characterBuffs.TryGetValue(characterId, out var buffs))
                return true; // 没有Buff，视为成功

            bool result = true;

            // 复制列表，因为在移除过程中会修改原列表
            var buffIds = buffs.ToList();

            foreach (var buffId in buffIds)
            {
                bool removeResult = await RemoveBuffFromCharacterAsync(characterId, buffId);
                result = result && removeResult;
            }

            return result;
        }

        /// <summary>
        /// 从角色移除指定类型的所有Buff
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <param name="buffType">Buff类型</param>
        /// <returns>操作结果</returns>
        public async Task<bool> RemoveBuffsByTypeFromCharacterAsync(Guid characterId, BuffType buffType)
        {
            if (!_characterBuffs.TryGetValue(characterId, out var buffs))
                return true; // 没有Buff，视为成功

            bool result = true;

            // 获取指定类型的Buff ID列表
            var buffIds = buffs
                .Select(id => GetBuff(id))
                .Where(b => b != null && b.Type == buffType)
                .Select(b => b.Id)
                .ToList();

            foreach (var buffId in buffIds)
            {
                bool removeResult = await RemoveBuffFromCharacterAsync(characterId, buffId);
                result = result && removeResult;
            }

            return result;
        }

        /// <summary>
        /// 从角色移除指定状态效果类型的所有Buff
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <param name="statusEffectType">状态效果类型</param>
        /// <returns>操作结果</returns>
        public async Task<bool> RemoveBuffsByStatusEffectTypeFromCharacterAsync(Guid characterId, StatusEffectType statusEffectType)
        {
            if (!_characterBuffs.TryGetValue(characterId, out var buffs))
                return true; // 没有Buff，视为成功

            bool result = true;

            // 获取指定状态效果类型的Buff ID列表
            var buffIds = buffs
                .Select(id => GetBuff(id))
                .Where(b => b != null && b.StatusEffectType == statusEffectType)
                .Select(b => b.Id)
                .ToList();

            foreach (var buffId in buffIds)
            {
                bool removeResult = await RemoveBuffFromCharacterAsync(characterId, buffId);
                result = result && removeResult;
            }

            return result;
        }

        /// <summary>
        /// 获取角色身上的所有Buff
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <returns>角色身上的所有Buff</returns>
        public IReadOnlyList<IBuff> GetBuffsOnCharacter(Guid characterId)
        {
            if (!_characterBuffs.TryGetValue(characterId, out var buffs))
                return new List<IBuff>();

            return buffs
                .Select(id => GetBuff(id))
                .Where(b => b != null)
                .ToList();
        }

        /// <summary>
        /// 获取角色身上指定类型的所有Buff
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <param name="buffType">Buff类型</param>
        /// <returns>角色身上指定类型的所有Buff</returns>
        public IReadOnlyList<IBuff> GetBuffsOnCharacterByType(Guid characterId, BuffType buffType)
        {
            if (!_characterBuffs.TryGetValue(characterId, out var buffs))
                return new List<IBuff>();

            return buffs
                .Select(id => GetBuff(id))
                .Where(b => b != null && b.Type == buffType)
                .ToList();
        }

        /// <summary>
        /// 获取角色身上指定状态效果类型的所有Buff
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <param name="statusEffectType">状态效果类型</param>
        /// <returns>角色身上指定状态效果类型的所有Buff</returns>
        public IReadOnlyList<IBuff> GetBuffsOnCharacterByStatusEffectType(Guid characterId, StatusEffectType statusEffectType)
        {
            if (!_characterBuffs.TryGetValue(characterId, out var buffs))
                return new List<IBuff>();

            return buffs
                .Select(id => GetBuff(id))
                .Where(b => b != null && b.StatusEffectType == statusEffectType)
                .ToList();
        }

        /// <summary>
        /// 检查角色是否拥有指定Buff
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <param name="buffId">Buff ID</param>
        /// <returns>是否拥有指定Buff</returns>
        public bool HasBuff(Guid characterId, Guid buffId)
        {
            if (!_characterBuffs.TryGetValue(characterId, out var buffs))
                return false;

            return buffs.Contains(buffId);
        }

        /// <summary>
        /// 检查角色是否拥有指定类型的Buff
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <param name="buffType">Buff类型</param>
        /// <returns>是否拥有指定类型的Buff</returns>
        public bool HasBuffOfType(Guid characterId, BuffType buffType)
        {
            if (!_characterBuffs.TryGetValue(characterId, out var buffs))
                return false;

            return buffs
                .Select(id => GetBuff(id))
                .Any(b => b != null && b.Type == buffType);
        }

        /// <summary>
        /// 检查角色是否拥有指定状态效果类型的Buff
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <param name="statusEffectType">状态效果类型</param>
        /// <returns>是否拥有指定状态效果类型的Buff</returns>
        public bool HasBuffOfStatusEffectType(Guid characterId, StatusEffectType statusEffectType)
        {
            if (!_characterBuffs.TryGetValue(characterId, out var buffs))
                return false;

            return buffs
                .Select(id => GetBuff(id))
                .Any(b => b != null && b.StatusEffectType == statusEffectType);
        }

        /// <summary>
        /// 处理角色回合开始时的Buff效果
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <returns>操作结果</returns>
        public async Task<bool> ProcessTurnStartBuffsAsync(Guid characterId)
        {
            if (!_characterBuffs.TryGetValue(characterId, out var buffs))
                return true; // 没有Buff，视为成功

            bool result = true;

            foreach (var buffId in buffs.ToList()) // 使用ToList()创建副本，避免在迭代过程中修改集合
            {
                if (_buffs.TryGetValue(buffId, out var buff))
                {
                    bool processResult = await buff.OnTurnStartAsync();
                    result = result && processResult;
                }
            }

            return result;
        }

        /// <summary>
        /// 处理角色回合结束时的Buff效果
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <returns>操作结果</returns>
        public async Task<bool> ProcessTurnEndBuffsAsync(Guid characterId)
        {
            if (!_characterBuffs.TryGetValue(characterId, out var buffs))
                return true; // 没有Buff，视为成功

            bool result = true;

            foreach (var buffId in buffs.ToList()) // 使用ToList()创建副本，避免在迭代过程中修改集合
            {
                if (_buffs.TryGetValue(buffId, out var buff))
                {
                    bool processResult = await buff.OnTurnEndAsync();

                    // 如果Buff已过期，移除它
                    if (!processResult)
                    {
                        await RemoveBuffFromCharacterAsync(characterId, buffId);
                    }

                    result = result && processResult;
                }
            }

            return result;
        }

        /// <summary>
        /// 处理角色受到伤害时的Buff效果
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <param name="damage">伤害值</param>
        /// <param name="attacker">攻击者</param>
        /// <returns>实际伤害值</returns>
        public async Task<int> ProcessDamageTakenBuffsAsync(Guid characterId, int damage, ICombatCharacter attacker)
        {
            if (damage <= 0 || !_characterBuffs.TryGetValue(characterId, out var buffs))
                return damage;

            int actualDamage = damage;

            // 检查是否有无敌Buff
            bool hasInvulnerable = buffs
                .Select(id => GetBuff(id))
                .Any(b => b != null && b.StatusEffectType == StatusEffectType.Invulnerable);

            if (hasInvulnerable)
                return 0; // 无敌状态，不受伤害

            // 处理护盾Buff
            var shieldBuffs = buffs
                .Select(id => GetBuff(id))
                .Where(b => b != null && b.StatusEffectType == StatusEffectType.Shield)
                .OrderByDescending(b => b.Power * b.StackCount) // 优先使用最强的护盾
                .ToList();

            foreach (var shieldBuff in shieldBuffs)
            {
                if (actualDamage <= 0)
                    break;

                int shieldValue = shieldBuff.Power * shieldBuff.StackCount;

                if (actualDamage <= shieldValue)
                {
                    // 护盾完全吸收伤害
                    shieldBuff.SetPower(shieldBuff.Power - actualDamage / shieldBuff.StackCount);

                    if (shieldBuff.Power <= 0)
                    {
                        // 护盾破碎
                        await RemoveBuffFromCharacterAsync(characterId, shieldBuff.Id);
                    }

                    actualDamage = 0;
                    break;
                }
                else
                {
                    // 护盾部分吸收伤害，然后破碎
                    actualDamage -= shieldValue;
                    await RemoveBuffFromCharacterAsync(characterId, shieldBuff.Id);
                }
            }

            // 处理其他Buff的受伤效果
            foreach (var buffId in buffs.ToList())
            {
                if (_buffs.TryGetValue(buffId, out var buff))
                {
                    await buff.OnDamageTakenAsync(actualDamage, attacker);
                }
            }

            return actualDamage;
        }

        /// <summary>
        /// 处理角色造成伤害时的Buff效果
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <param name="damage">伤害值</param>
        /// <param name="victim">受害者</param>
        /// <returns>实际伤害值</returns>
        public async Task<int> ProcessDamageDealtBuffsAsync(Guid characterId, int damage, ICombatCharacter victim)
        {
            if (damage <= 0 || !_characterBuffs.TryGetValue(characterId, out var buffs))
                return damage;

            int actualDamage = damage;

            // 处理Buff的造成伤害效果
            foreach (var buffId in buffs.ToList())
            {
                if (_buffs.TryGetValue(buffId, out var buff))
                {
                    await buff.OnDamageDealtAsync(actualDamage, victim);

                    // 根据Buff类型调整伤害
                    if (buff.Type == BuffType.Positive)
                    {
                        // 增益Buff可能增加伤害
                        actualDamage += (int)(damage * (buff.Power * 0.1)); // 每点强度增加10%伤害
                    }
                    else if (buff.Type == BuffType.Negative)
                    {
                        // 减益Buff可能减少伤害
                        actualDamage -= (int)(damage * (buff.Power * 0.1)); // 每点强度减少10%伤害
                    }
                }
            }

            return Math.Max(0, actualDamage);
        }

        /// <summary>
        /// 处理角色移动前的Buff效果
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <param name="fromX">起始X坐标</param>
        /// <param name="fromY">起始Y坐标</param>
        /// <param name="toX">目标X坐标</param>
        /// <param name="toY">目标Y坐标</param>
        /// <returns>是否允许移动</returns>
        public async Task<bool> ProcessBeforeMoveBuffsAsync(Guid characterId, int fromX, int fromY, int toX, int toY)
        {
            if (!_characterBuffs.TryGetValue(characterId, out var buffs))
                return true; // 没有Buff，允许移动

            // 检查是否有禁止移动的Buff
            bool hasImmobilizingBuff = buffs
                .Select(id => GetBuff(id))
                .Any(b => b != null && (
                    b.StatusEffectType == StatusEffectType.Root ||
                    b.StatusEffectType == StatusEffectType.Stun ||
                    b.StatusEffectType == StatusEffectType.Sleep ||
                    b.StatusEffectType == StatusEffectType.Freeze
                ));

            if (hasImmobilizingBuff)
                return false; // 有禁止移动的Buff，不允许移动

            // 处理其他Buff的移动前效果
            foreach (var buffId in buffs.ToList())
            {
                if (_buffs.TryGetValue(buffId, out var buff))
                {
                    bool canMove = await buff.OnBeforeMoveAsync(fromX, fromY, toX, toY);

                    if (!canMove)
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 处理角色移动后的Buff效果
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <param name="fromX">起始X坐标</param>
        /// <param name="fromY">起始Y坐标</param>
        /// <param name="toX">目标X坐标</param>
        /// <param name="toY">目标Y坐标</param>
        /// <returns>操作结果</returns>
        public async Task<bool> ProcessAfterMoveBuffsAsync(Guid characterId, int fromX, int fromY, int toX, int toY)
        {
            if (!_characterBuffs.TryGetValue(characterId, out var buffs))
                return true; // 没有Buff，视为成功

            bool result = true;

            // 处理Buff的移动后效果
            foreach (var buffId in buffs.ToList())
            {
                if (_buffs.TryGetValue(buffId, out var buff))
                {
                    bool processResult = await buff.OnAfterMoveAsync(fromX, fromY, toX, toY);
                    result = result && processResult;
                }
            }

            return result;
        }

        /// <summary>
        /// 清理过期的Buff
        /// </summary>
        /// <returns>操作结果</returns>
        public async Task<bool> CleanupExpiredBuffsAsync()
        {
            bool result = true;

            // 获取所有角色的ID
            var characterIds = _characterBuffs.Keys.ToList();

            foreach (var characterId in characterIds)
            {
                if (_characterBuffs.TryGetValue(characterId, out var buffs))
                {
                    foreach (var buffId in buffs.ToList())
                    {
                        if (_buffs.TryGetValue(buffId, out var buff))
                        {
                            if (buff.RemainingDuration <= 0 && buff.Duration != -1) // 非永久Buff且已过期
                            {
                                bool removeResult = await RemoveBuffFromCharacterAsync(characterId, buffId);
                                result = result && removeResult;
                            }
                        }
                    }
                }
            }

            return result;
        }
    }
}
