using System;
using System.Threading.Tasks;
using TacticalRPG.Core.Modules.Character;

namespace TacticalRPG.Core.Modules.Buff
{
    /// <summary>
    /// 状态效果Buff实现类
    /// </summary>
    public class StatusBuff : BaseBuff
    {
        /// <summary>
        /// 恢复状态效果时发生的事件
        /// </summary>
        public event EventHandler<StatusEffectEventArgs> OnStatusEffectRecovered;

        /// <summary>
        /// 状态效果改变前发生的事件
        /// </summary>
        public event EventHandler<StatusEffectChangeEventArgs> OnBeforeStatusEffectChange;

        /// <summary>
        /// 状态效果改变后发生的事件
        /// </summary>
        public event EventHandler<StatusEffectChangeEventArgs> OnAfterStatusEffectChange;


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">Buff名称</param>
        /// <param name="description">Buff描述</param>
        /// <param name="statusEffectType">状态效果类型</param>
        /// <param name="duration">持续回合数</param>
        /// <param name="power">效果强度</param>
        /// <param name="isStackable">是否可叠加</param>
        /// <param name="maxStackCount">最大叠加层数</param>
        public StatusBuff(
            string name,
            string description,
            StatusEffectType statusEffectType,
            int duration,
            int power,
            bool isStackable = false,
            int maxStackCount = 1)
            : base(name, description, DetermineBuffType(statusEffectType), statusEffectType, duration, power, isStackable, maxStackCount)
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">Buff名称</param>
        /// <param name="description">Buff描述</param>
        /// <param name="statusEffectType">状态效果类型</param>
        /// <param name="duration">持续回合数</param>
        /// <param name="power">效果强度</param>
        /// <param name="caster">施法者</param>
        /// <param name="target">目标</param>
        /// <param name="isStackable">是否可叠加</param>
        /// <param name="maxStackCount">最大叠加层数</param>
        public StatusBuff(
            string name,
            string description,
            StatusEffectType statusEffectType,
            int duration,
            int power,
            ICombatCharacter caster,
            ICombatCharacter target,
            bool isStackable = false,
            int maxStackCount = 1)
            : base(name, description, DetermineBuffType(statusEffectType), statusEffectType, duration, power, isStackable, maxStackCount)
        {
            Caster = caster;
            Target = target;
        }

        /// <summary>
        /// 根据状态效果类型确定对应的Buff类型
        /// </summary>
        /// <param name="statusEffectType">状态效果类型</param>
        /// <returns>对应的Buff类型</returns>
        private static BuffType DetermineBuffType(StatusEffectType statusEffectType)
        {
            switch (statusEffectType)
            {
                case StatusEffectType.Stun:
                case StatusEffectType.Silence:
                case StatusEffectType.Root:
                case StatusEffectType.Sleep:
                case StatusEffectType.Confusion:
                case StatusEffectType.Charm:
                case StatusEffectType.Fear:
                    return BuffType.Control;

                case StatusEffectType.Poison:
                case StatusEffectType.Burn:
                    return BuffType.DamageOverTime;

                case StatusEffectType.Slow:
                case StatusEffectType.Freeze:
                case StatusEffectType.Paralyze:
                case StatusEffectType.Weaken:
                case StatusEffectType.ArmorBreak:
                case StatusEffectType.Blind:
                    return BuffType.Negative;

                case StatusEffectType.Haste:
                case StatusEffectType.Shield:
                case StatusEffectType.Invulnerable:
                case StatusEffectType.Purify:
                case StatusEffectType.Invisible:
                case StatusEffectType.Regeneration:
                    return BuffType.Positive;

                case StatusEffectType.LifeSteal:
                case StatusEffectType.Reflect:
                    return BuffType.Special;

                case StatusEffectType.Berserk:
                    return BuffType.Neutral;

                case StatusEffectType.Taunt:
                    return BuffType.Control;

                default:
                    return BuffType.Neutral;
            }
        }

        /// <summary>
        /// 应用状态效果
        /// </summary>
        /// <returns>操作结果</returns>
        public override async Task<bool> ApplyAsync()
        {
            if (Target == null)
                return false;

            // 触发状态效果改变前事件
            var beforeArgs = new StatusEffectChangeEventArgs(Target, StatusEffectType.None, StatusEffectType);
            OnBeforeStatusEffectChange?.Invoke(this, beforeArgs);

            if (beforeArgs.IsCancelled)
                return false;

            // 基础应用逻辑
            bool result = await base.ApplyAsync();
            if (!result)
                return false;

            // 应用特定状态效果
            var combatCharacter = Target as ICombatCharacter;
            if (combatCharacter != null)
            {
                switch (StatusEffectType)
                {
                    case StatusEffectType.Stun:
                        combatCharacter.SetCanAct(false);
                        combatCharacter.SetCanMove(false);
                        break;
                    case StatusEffectType.Silence:
                        combatCharacter.SetCanUseSkills(false);
                        break;
                    case StatusEffectType.Root:
                        combatCharacter.SetCanMove(false);
                        break;
                    case StatusEffectType.Sleep:
                        combatCharacter.SetCanAct(false);
                        combatCharacter.SetCanMove(false);
                        break;
                        // 其他状态效果...
                }
            }

            // 触发状态效果改变后事件
            var afterArgs = new StatusEffectChangeEventArgs(Target, StatusEffectType.None, StatusEffectType);
            OnAfterStatusEffectChange?.Invoke(this, afterArgs);

            return true;
        }

        /// <summary>
        /// 移除状态效果
        /// </summary>
        /// <returns>操作结果</returns>
        public override async Task<bool> RemoveAsync()
        {
            if (Target == null)
                return false;

            // 触发状态效果改变前事件
            var beforeArgs = new StatusEffectChangeEventArgs(Target, StatusEffectType, StatusEffectType.None);
            OnBeforeStatusEffectChange?.Invoke(this, beforeArgs);

            if (beforeArgs.IsCancelled)
                return false;

            // 恢复特定状态效果
            var combatCharacter = Target as ICombatCharacter;
            if (combatCharacter != null)
            {
                switch (StatusEffectType)
                {
                    case StatusEffectType.Stun:
                        combatCharacter.SetCanAct(true);
                        combatCharacter.SetCanMove(true);
                        break;
                    case StatusEffectType.Silence:
                        combatCharacter.SetCanUseSkills(true);
                        break;
                    case StatusEffectType.Root:
                        combatCharacter.SetCanMove(true);
                        break;
                    case StatusEffectType.Sleep:
                        combatCharacter.SetCanAct(true);
                        combatCharacter.SetCanMove(true);
                        break;
                        // 其他状态效果...
                }
            }

            // 基础移除逻辑
            bool result = await base.RemoveAsync();
            if (!result)
                return false;

            // 触发状态效果恢复事件
            OnStatusEffectRecovered?.Invoke(this, new StatusEffectEventArgs(Target, StatusEffectType));

            // 触发状态效果改变后事件
            var afterArgs = new StatusEffectChangeEventArgs(Target, StatusEffectType, StatusEffectType.None);
            OnAfterStatusEffectChange?.Invoke(this, afterArgs);

            return true;
        }

        /// <summary>
        /// 受到伤害时触发
        /// </summary>
        /// <param name="damage">伤害量</param>
        /// <param name="attacker">攻击者</param>
        /// <returns>操作结果</returns>
        public override async Task<bool> OnDamageTakenAsync(int damage, ICombatCharacter attacker)
        {
            // 处理特殊状态效果的伤害响应
            if (StatusEffectType == StatusEffectType.Sleep && damage > 0)
            {
                // 睡眠状态下受到伤害会解除睡眠
                await RemoveAsync();
                return true;
            }

            return await base.OnDamageTakenAsync(damage, attacker);
        }

        /// <summary>
        /// 回合结束时触发
        /// </summary>
        /// <returns>操作结果</returns>
        public override async Task<bool> OnTurnEndAsync()
        {
            // 处理持续伤害类型的效果
            if (StatusEffectType == StatusEffectType.Poison || StatusEffectType == StatusEffectType.Burn)
            {
                if (Target != null)
                {
                    // 计算实际伤害（可以基于层数、强度等）
                    int actualDamage = Power * StackCount;
                    await Target.TakeDamageAsync(actualDamage, Caster);
                }
            }
            else if (StatusEffectType == StatusEffectType.Regeneration)
            {
                if (Target != null)
                {
                    // 计算实际治疗量
                    int healAmount = Power * StackCount;
                    await Target.HealAsync(healAmount);
                }
            }

            return await base.OnTurnEndAsync();
        }

        /// <summary>
        /// 创建状态Buff的复制品
        /// </summary>
        /// <returns>Buff复制品</returns>
        public override IBuff Clone()
        {
            StatusBuff clone = new StatusBuff(
                Name,
                Description,
                StatusEffectType,
                Duration,
                Power,
                IsStackable,
                MaxStackCount
            );

            clone.RemainingDuration = RemainingDuration;
            clone.StackCount = StackCount;
            clone.Caster = Caster;
            clone.Target = Target;
            clone.IsHidden = IsHidden;
            clone.IconPath = IconPath;
            clone.VisualEffectId = VisualEffectId;

            return clone;
        }
    }

    /// <summary>
    /// 状态效果事件参数
    /// </summary>
    public class StatusEffectEventArgs : EventArgs
    {
        /// <summary>
        /// 目标
        /// </summary>
        public ICombatCharacter Target { get; }

        /// <summary>
        /// 状态效果类型
        /// </summary>
        public StatusEffectType StatusEffectType { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="target">目标</param>
        /// <param name="statusEffectType">状态效果类型</param>
        public StatusEffectEventArgs(ICombatCharacter target, StatusEffectType statusEffectType)
        {
            Target = target;
            StatusEffectType = statusEffectType;
        }
    }

    /// <summary>
    /// 状态效果改变事件参数
    /// </summary>
    public class StatusEffectChangeEventArgs : StatusEffectEventArgs
    {
        /// <summary>
        /// 原状态效果类型
        /// </summary>
        public StatusEffectType OldStatusEffectType { get; }

        /// <summary>
        /// 新状态效果类型
        /// </summary>
        public StatusEffectType NewStatusEffectType { get; }

        /// <summary>
        /// 是否取消状态效果改变
        /// </summary>
        public bool IsCancelled { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="target">目标角色</param>
        /// <param name="oldStatusEffectType">原状态效果类型</param>
        /// <param name="newStatusEffectType">新状态效果类型</param>
        public StatusEffectChangeEventArgs(ICombatCharacter target, StatusEffectType oldStatusEffectType, StatusEffectType newStatusEffectType)
            : base(target, newStatusEffectType)
        {
            OldStatusEffectType = oldStatusEffectType;
            NewStatusEffectType = newStatusEffectType;
            IsCancelled = false;
        }
    }
}