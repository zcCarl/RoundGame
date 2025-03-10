using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TacticalRPG.Core.Framework;
using TacticalRPG.Core.Modules.Character;
using TacticalRPG.Core.Modules.Skill;

namespace TacticalRPG.Implementation.Modules.Skill
{
    /// <summary>
    /// 技能效果管理器，负责管理技能效果的创建、应用和移除
    /// </summary>
    public class SkillEffectManager : ISkillEffectManager
    {
        private readonly ILogger<SkillEffectManager> _logger;
        private readonly ISkillEffectFactory _effectFactory;
        private readonly Dictionary<Guid, ISkillEffect> _effects = new Dictionary<Guid, ISkillEffect>();
        private readonly Dictionary<Guid, List<(Guid EffectId, int RemainingTurns)>> _characterActiveEffects = new Dictionary<Guid, List<(Guid EffectId, int RemainingTurns)>>();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="logger">日志记录器</param>
        /// <param name="effectFactory">效果工厂</param>
        public SkillEffectManager(ILogger<SkillEffectManager> logger, ISkillEffectFactory effectFactory)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _effectFactory = effectFactory ?? throw new ArgumentNullException(nameof(effectFactory));
        }

        /// <summary>
        /// 创建技能效果
        /// </summary>
        /// <param name="name">效果名称</param>
        /// <param name="description">效果描述</param>
        /// <param name="skillId">所属技能ID</param>
        /// <param name="effectType">效果类型</param>
        /// <param name="effectTarget">效果目标</param>
        /// <param name="duration">持续回合数</param>
        /// <param name="power">效果力量值</param>
        /// <param name="isAreaEffect">是否为区域效果</param>
        /// <returns>新效果的ID</returns>
        public Task<Guid> CreateEffectAsync(
            string name,
            string description,
            Guid skillId,
            SkillEffectType effectType,
            SkillEffectTarget effectTarget,
            int duration,
            int power,
            bool isAreaEffect)
        {
            try
            {
                var id = Guid.NewGuid();
                ISkillEffect effect;

                // 根据效果类型调用对应的特定工厂方法
                switch (effectType)
                {
                    case SkillEffectType.DirectDamage:
                        effect = _effectFactory.CreateDirectDamageEffect(name, description, skillId, power, isAreaEffect);
                        break;
                    case SkillEffectType.DamageOverTime:
                        effect = _effectFactory.CreateDamageOverTimeEffect(name, description, skillId, power, duration, isAreaEffect);
                        break;
                    case SkillEffectType.DirectHeal:
                        effect = _effectFactory.CreateDirectHealEffect(name, description, skillId, power, isAreaEffect);
                        break;
                    case SkillEffectType.HealOverTime:
                        effect = _effectFactory.CreateHealOverTimeEffect(name, description, skillId, power, duration, isAreaEffect);
                        break;
                    case SkillEffectType.StatBoost:
                        effect = _effectFactory.CreateStatBoostEffect(name, description, skillId, effectTarget, power, duration, isAreaEffect);
                        break;
                    case SkillEffectType.StatReduction:
                        effect = _effectFactory.CreateStatReductionEffect(name, description, skillId, effectTarget, power, duration, isAreaEffect);
                        break;
                    case SkillEffectType.StatusChange:
                        effect = _effectFactory.CreateStatusChangeEffect(name, description, skillId, duration, isAreaEffect);
                        break;
                    default:
                        // 对于其他类型，使用基本的CreateEffect方法
                        effect = _effectFactory.CreateEffect(effectType, effectTarget);
                        // 手动设置其他属性
                        if (effect != null)
                        {
                            effect.SetName(name);
                            effect.SetDescription(description);
                            effect.SetSkillId(skillId);
                            effect.SetDuration(duration);
                            effect.SetPower(power);
                            effect.SetIsAreaEffect(isAreaEffect);
                        }
                        break;
                }

                if (effect != null)
                {
                    _effects[effect.Id] = effect;
                    _logger.LogInformation($"创建技能效果：{name}，ID：{effect.Id}");
                    return Task.FromResult(effect.Id);
                }

                _logger.LogError($"创建技能效果失败：{name}");
                return Task.FromResult(Guid.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"创建技能效果失败：{name}");
                throw;
            }
        }

        /// <summary>
        /// 获取技能效果
        /// </summary>
        /// <param name="effectId">效果ID</param>
        /// <returns>效果对象</returns>
        public ISkillEffect GetEffect(Guid effectId)
        {
            if (_effects.TryGetValue(effectId, out var effect))
            {
                return effect;
            }

            _logger.LogWarning($"未找到ID为 {effectId} 的技能效果");
            return null;
        }

        /// <summary>
        /// 获取技能的所有效果
        /// </summary>
        /// <param name="skillId">技能ID</param>
        /// <returns>技能的所有效果</returns>
        public IReadOnlyList<ISkillEffect> GetEffectsBySkill(Guid skillId)
        {
            return _effects.Values
                .Where(e => e.SkillId == skillId)
                .ToList();
        }

        /// <summary>
        /// 获取特定类型的所有效果
        /// </summary>
        /// <param name="effectType">效果类型</param>
        /// <returns>特定类型的所有效果</returns>
        public IReadOnlyList<ISkillEffect> GetEffectsByType(SkillEffectType effectType)
        {
            return _effects.Values
                .Where(e => e.EffectType == effectType)
                .ToList();
        }

        /// <summary>
        /// 获取特定目标的所有效果
        /// </summary>
        /// <param name="effectTarget">效果目标</param>
        /// <returns>特定目标的所有效果</returns>
        public IReadOnlyList<ISkillEffect> GetEffectsByTarget(SkillEffectTarget effectTarget)
        {
            return _effects.Values
                .Where(e => e.EffectTarget == effectTarget)
                .ToList();
        }

        /// <summary>
        /// 保存技能效果
        /// </summary>
        /// <param name="effect">效果对象</param>
        /// <returns>操作结果</returns>
        public Task<bool> SaveEffectAsync(ISkillEffect effect)
        {
            if (effect == null)
            {
                throw new ArgumentNullException(nameof(effect));
            }

            try
            {
                _effects[effect.Id] = effect;
                _logger.LogInformation($"保存技能效果：{effect.Name}，ID：{effect.Id}");
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"保存技能效果失败：{effect.Name}，ID：{effect.Id}");
                return Task.FromResult(false);
            }
        }

        /// <summary>
        /// 删除技能效果
        /// </summary>
        /// <param name="effectId">效果ID</param>
        /// <returns>操作结果</returns>
        public Task<bool> DeleteEffectAsync(Guid effectId)
        {
            try
            {
                if (_effects.TryGetValue(effectId, out var effect))
                {
                    _effects.Remove(effectId);
                    _logger.LogInformation($"删除技能效果：{effect.Name}，ID：{effectId}");
                    return Task.FromResult(true);
                }

                _logger.LogWarning($"删除技能效果失败：未找到ID为 {effectId} 的效果");
                return Task.FromResult(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"删除技能效果失败：ID：{effectId}");
                return Task.FromResult(false);
            }
        }

        /// <summary>
        /// 应用技能效果
        /// </summary>
        /// <param name="effectId">效果ID</param>
        /// <param name="caster">施法者</param>
        /// <param name="targets">目标列表</param>
        /// <returns>操作结果</returns>
        public async Task<bool> ApplyEffectAsync(Guid effectId, ICharacter caster, IReadOnlyList<ICharacter> targets)
        {
            try
            {
                if (!_effects.TryGetValue(effectId, out var effect))
                {
                    _logger.LogWarning($"应用技能效果失败：未找到ID为 {effectId} 的效果");
                    return false;
                }

                if (targets == null || !targets.Any())
                {
                    _logger.LogWarning($"应用技能效果失败：目标列表为空");
                    return false;
                }

                // 复制一个效果实例，避免多个目标共享同一个效果实例
                var effectInstance = effect.Clone();

                // 应用即时效果
                if (effectInstance.Duration == 0)
                {
                    return await effectInstance.ApplyImmediateEffectAsync(caster, targets);
                }

                // 应用持续效果
                var result = await effectInstance.ApplyDurationEffectAsync(caster, targets);
                if (result)
                {
                    // 将效果添加到目标的活动效果列表中
                    foreach (var target in targets)
                    {
                        if (!_characterActiveEffects.TryGetValue(target.Id, out var activeEffects))
                        {
                            activeEffects = new List<(Guid EffectId, int RemainingTurns)>();
                            _characterActiveEffects[target.Id] = activeEffects;
                        }

                        // 检查是否已有相同类型的效果，如果有则移除
                        var existingEffectIndex = activeEffects.FindIndex(e =>
                            GetEffect(e.EffectId)?.EffectType == effectInstance.EffectType &&
                            GetEffect(e.EffectId)?.EffectTarget == effectInstance.EffectTarget);

                        if (existingEffectIndex >= 0)
                        {
                            var existingEffect = GetEffect(activeEffects[existingEffectIndex].EffectId);
                            if (existingEffect != null)
                            {
                                await existingEffect.RemoveEffectAsync(target);
                            }
                            activeEffects.RemoveAt(existingEffectIndex);
                        }

                        // 添加新效果
                        activeEffects.Add((effectInstance.Id, effectInstance.Duration));
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"应用技能效果失败：ID：{effectId}");
                return false;
            }
        }

        /// <summary>
        /// 移除角色身上的技能效果
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <param name="effectId">效果ID</param>
        /// <returns>操作结果</returns>
        public async Task<bool> RemoveEffectFromCharacterAsync(Guid characterId, Guid effectId)
        {
            try
            {
                if (!_characterActiveEffects.TryGetValue(characterId, out var activeEffects))
                {
                    _logger.LogWarning($"移除技能效果失败：角色 {characterId} 没有活动效果");
                    return false;
                }

                var effectIndex = activeEffects.FindIndex(e => e.EffectId == effectId);
                if (effectIndex < 0)
                {
                    _logger.LogWarning($"移除技能效果失败：角色 {characterId} 没有ID为 {effectId} 的效果");
                    return false;
                }

                var effect = GetEffect(effectId);
                if (effect == null)
                {
                    _logger.LogWarning($"移除技能效果失败：未找到ID为 {effectId} 的效果");
                    return false;
                }

                // TODO: 获取角色实例并移除效果
                // var character = _characterModule.GetCharacter(characterId);
                // if (character == null)
                // {
                //     _logger.LogWarning($"移除技能效果失败：未找到ID为 {characterId} 的角色");
                //     return false;
                // }

                // var result = await effect.RemoveEffectAsync(character);
                var result = true;  // 临时代码，等待角色模块集成

                if (result)
                {
                    activeEffects.RemoveAt(effectIndex);
                    _logger.LogInformation($"从角色 {characterId} 移除技能效果：{effect.Name}，ID：{effectId}");
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"从角色 {characterId} 移除技能效果失败：ID：{effectId}");
                return false;
            }
        }

        /// <summary>
        /// 获取角色身上的所有效果
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <returns>角色身上的所有效果</returns>
        public IReadOnlyList<ISkillEffect> GetActiveEffectsOnCharacter(Guid characterId)
        {
            if (!_characterActiveEffects.TryGetValue(characterId, out var activeEffects))
            {
                return new List<ISkillEffect>();
            }

            return activeEffects
                .Select(e => GetEffect(e.EffectId))
                .Where(e => e != null)
                .ToList();
        }

        /// <summary>
        /// 处理回合开始时的效果
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <returns>操作结果</returns>
        public async Task<bool> ProcessTurnStartEffectsAsync(Guid characterId)
        {
            try
            {
                if (!_characterActiveEffects.TryGetValue(characterId, out var activeEffects))
                {
                    return true; // 没有活动效果，返回成功
                }

                // TODO: 获取角色实例
                // var character = _characterModule.GetCharacter(characterId);
                // if (character == null)
                // {
                //     _logger.LogWarning($"处理回合开始效果失败：未找到ID为 {characterId} 的角色");
                //     return false;
                // }

                // 遍历所有活动效果，触发回合开始效果
                foreach (var (effectId, _) in activeEffects.ToList())
                {
                    var effect = GetEffect(effectId);
                    if (effect != null)
                    {
                        // await effect.OnTurnStartAsync(character);
                        // 临时代码，等待角色模块集成
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"处理角色 {characterId} 的回合开始效果失败");
                return false;
            }
        }

        /// <summary>
        /// 处理回合结束时的效果
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <returns>操作结果</returns>
        public async Task<bool> ProcessTurnEndEffectsAsync(Guid characterId)
        {
            try
            {
                if (!_characterActiveEffects.TryGetValue(characterId, out var activeEffects))
                {
                    return true; // 没有活动效果，返回成功
                }

                // TODO: 获取角色实例
                // var character = _characterModule.GetCharacter(characterId);
                // if (character == null)
                // {
                //     _logger.LogWarning($"处理回合结束效果失败：未找到ID为 {characterId} 的角色");
                //     return false;
                // }

                // 遍历所有活动效果，触发回合结束效果并减少持续时间
                for (int i = activeEffects.Count - 1; i >= 0; i--)
                {
                    var (effectId, remainingTurns) = activeEffects[i];
                    var effect = GetEffect(effectId);
                    if (effect != null)
                    {
                        // await effect.OnTurnEndAsync(character);
                        // 临时代码，等待角色模块集成

                        // 减少持续时间
                        remainingTurns--;
                        if (remainingTurns <= 0)
                        {
                            // 效果已过期，移除
                            // await effect.RemoveEffectAsync(character);
                            activeEffects.RemoveAt(i);
                            _logger.LogInformation($"角色 {characterId} 的效果 {effect.Name}（ID：{effectId}）已过期");
                        }
                        else
                        {
                            // 更新剩余持续时间
                            activeEffects[i] = (effectId, remainingTurns);
                        }
                    }
                    else
                    {
                        // 效果不存在，直接移除
                        activeEffects.RemoveAt(i);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"处理角色 {characterId} 的回合结束效果失败");
                return false;
            }
        }

        /// <summary>
        /// 清除过期的效果
        /// </summary>
        /// <returns>操作结果</returns>
        public async Task<bool> CleanupExpiredEffectsAsync()
        {
            try
            {
                foreach (var characterId in _characterActiveEffects.Keys.ToList())
                {
                    var activeEffects = _characterActiveEffects[characterId];
                    for (int i = activeEffects.Count - 1; i >= 0; i--)
                    {
                        var (effectId, remainingTurns) = activeEffects[i];
                        if (remainingTurns <= 0 || !_effects.ContainsKey(effectId))
                        {
                            // 效果已过期或不存在，移除
                            activeEffects.RemoveAt(i);
                            _logger.LogInformation($"清理过期效果：角色 {characterId} 的效果 ID：{effectId}");
                        }
                    }

                    // 如果角色没有活动效果了，从字典中移除
                    if (activeEffects.Count == 0)
                    {
                        _characterActiveEffects.Remove(characterId);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "清理过期效果失败");
                return false;
            }
        }
    }
}