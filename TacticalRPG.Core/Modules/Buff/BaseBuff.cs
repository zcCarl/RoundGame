using System;
using System.Threading.Tasks;
using TacticalRPG.Core.Modules.Character;

namespace TacticalRPG.Core.Modules.Buff
{
    /// <summary>
    /// Buff基类
    /// </summary>
    public class BaseBuff : IBuff
    {
        /// <summary>
        /// Buff ID
        /// </summary>
        public Guid Id { get; protected set; }

        /// <summary>
        /// Buff名称
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// Buff描述
        /// </summary>
        public string Description { get; protected set; }

        /// <summary>
        /// Buff类型
        /// </summary>
        public BuffType Type { get; protected set; }

        /// <summary>
        /// 状态效果类型
        /// </summary>
        public StatusEffectType StatusEffectType { get; protected set; }

        /// <summary>
        /// 持续回合数（-1表示永久）
        /// </summary>
        public int Duration { get; protected set; }

        /// <summary>
        /// 当前剩余回合数
        /// </summary>
        public int RemainingDuration { get; protected set; }

        /// <summary>
        /// 效果强度
        /// </summary>
        public int Power { get; protected set; }

        /// <summary>
        /// 是否可叠加
        /// </summary>
        public bool IsStackable { get; protected set; }

        /// <summary>
        /// 当前叠加层数
        /// </summary>
        public int StackCount { get; protected set; }

        /// <summary>
        /// 最大叠加层数
        /// </summary>
        public int MaxStackCount { get; protected set; }

        /// <summary>
        /// 施法者
        /// </summary>
        public ICombatCharacter Caster { get; protected set; }

        /// <summary>
        /// 目标
        /// </summary>
        public ICombatCharacter Target { get; protected set; }

        /// <summary>
        /// 是否为隐藏效果
        /// </summary>
        public bool IsHidden { get; protected set; }

        /// <summary>
        /// 图标路径
        /// </summary>
        public string IconPath { get; protected set; }

        /// <summary>
        /// 视觉效果ID
        /// </summary>
        public string VisualEffectId { get; protected set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        protected BaseBuff()
        {
            Id = Guid.NewGuid();
            StackCount = 1;
            MaxStackCount = 1;
            IsStackable = false;
            IsHidden = false;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">Buff名称</param>
        /// <param name="description">Buff描述</param>
        /// <param name="buffType">Buff类型</param>
        /// <param name="statusEffectType">状态效果类型</param>
        /// <param name="duration">持续回合数</param>
        /// <param name="power">效果强度</param>
        /// <param name="isStackable">是否可叠加</param>
        /// <param name="maxStackCount">最大叠加层数</param>
        public BaseBuff(string name, string description, BuffType buffType, StatusEffectType statusEffectType,
            int duration, int power, bool isStackable = false, int maxStackCount = 1)
        {
            Id = Guid.NewGuid();
            Name = name;
            Description = description;
            Type = buffType;
            StatusEffectType = statusEffectType;
            Duration = duration;
            RemainingDuration = duration;
            Power = power;
            IsStackable = isStackable;
            MaxStackCount = maxStackCount;
            StackCount = 1;
            IsHidden = false;
        }

        /// <summary>
        /// 设置名称
        /// </summary>
        /// <param name="name">名称</param>
        public void SetName(string name)
        {
            Name = name;
        }

        /// <summary>
        /// 设置描述
        /// </summary>
        /// <param name="description">描述</param>
        public void SetDescription(string description)
        {
            Description = description;
        }

        /// <summary>
        /// 设置持续回合数
        /// </summary>
        /// <param name="duration">持续回合数</param>
        public void SetDuration(int duration)
        {
            Duration = duration;
            RemainingDuration = duration;
        }

        /// <summary>
        /// 设置效果强度
        /// </summary>
        /// <param name="power">效果强度</param>
        public void SetPower(int power)
        {
            Power = power;
        }

        /// <summary>
        /// 设置施法者
        /// </summary>
        /// <param name="caster">施法者</param>
        public void SetCaster(ICombatCharacter caster)
        {
            Caster = caster;
        }

        /// <summary>
        /// 设置目标
        /// </summary>
        /// <param name="target">目标</param>
        public void SetTarget(ICombatCharacter target)
        {
            Target = target;
        }

        /// <summary>
        /// 减少剩余回合数
        /// </summary>
        /// <param name="amount">减少量</param>
        /// <returns>是否已过期</returns>
        public bool ReduceDuration(int amount = 1)
        {
            // 永久性buff不会过期
            if (Duration == -1)
                return false;

            RemainingDuration -= amount;
            return RemainingDuration <= 0;
        }

        /// <summary>
        /// 增加叠加层数
        /// </summary>
        /// <param name="amount">增加量</param>
        /// <returns>当前叠加层数</returns>
        public int AddStack(int amount = 1)
        {
            if (!IsStackable)
                return StackCount;

            StackCount = Math.Min(StackCount + amount, MaxStackCount);
            return StackCount;
        }

        /// <summary>
        /// 减少叠加层数
        /// </summary>
        /// <param name="amount">减少量</param>
        /// <returns>当前叠加层数</returns>
        public int RemoveStack(int amount = 1)
        {
            StackCount = Math.Max(StackCount - amount, 0);
            return StackCount;
        }

        /// <summary>
        /// 应用Buff效果
        /// </summary>
        /// <returns>操作结果</returns>
        public virtual Task<bool> ApplyAsync()
        {
            // 基类中的基本实现，派生类可以重写提供更丰富的功能
            return Task.FromResult(true);
        }

        /// <summary>
        /// 移除Buff效果
        /// </summary>
        /// <returns>操作结果</returns>
        public virtual Task<bool> RemoveAsync()
        {
            // 基类中的基本实现，派生类可以重写提供更丰富的功能
            return Task.FromResult(true);
        }

        /// <summary>
        /// 回合开始时触发
        /// </summary>
        /// <returns>操作结果</returns>
        public virtual Task<bool> OnTurnStartAsync()
        {
            // 基类中的基本实现，派生类可以重写提供更丰富的功能
            return Task.FromResult(true);
        }

        /// <summary>
        /// 回合结束时触发
        /// </summary>
        /// <returns>操作结果</returns>
        public virtual Task<bool> OnTurnEndAsync()
        {
            // 基类中的基本实现，派生类可以重写提供更丰富的功能
            bool isExpired = ReduceDuration();
            return Task.FromResult(!isExpired);
        }

        /// <summary>
        /// 受到伤害时触发
        /// </summary>
        /// <param name="damage">伤害量</param>
        /// <param name="attacker">攻击者</param>
        /// <returns>操作结果</returns>
        public virtual Task<bool> OnDamageTakenAsync(int damage, ICombatCharacter attacker)
        {
            // 基类中的基本实现，派生类可以重写提供更丰富的功能
            return Task.FromResult(true);
        }

        /// <summary>
        /// 造成伤害时触发
        /// </summary>
        /// <param name="damage">伤害量</param>
        /// <param name="victim">受害者</param>
        /// <returns>操作结果</returns>
        public virtual Task<bool> OnDamageDealtAsync(int damage, ICombatCharacter victim)
        {
            // 基类中的基本实现，派生类可以重写提供更丰富的功能
            return Task.FromResult(true);
        }

        /// <summary>
        /// 移动前触发
        /// </summary>
        /// <param name="fromX">起始X坐标</param>
        /// <param name="fromY">起始Y坐标</param>
        /// <param name="toX">目标X坐标</param>
        /// <param name="toY">目标Y坐标</param>
        /// <returns>是否允许移动</returns>
        public virtual Task<bool> OnBeforeMoveAsync(int fromX, int fromY, int toX, int toY)
        {
            // 基类中的基本实现，派生类可以重写提供更丰富的功能
            return Task.FromResult(true);
        }

        /// <summary>
        /// 移动后触发
        /// </summary>
        /// <param name="fromX">起始X坐标</param>
        /// <param name="fromY">起始Y坐标</param>
        /// <param name="toX">目标X坐标</param>
        /// <param name="toY">目标Y坐标</param>
        /// <returns>操作结果</returns>
        public virtual Task<bool> OnAfterMoveAsync(int fromX, int fromY, int toX, int toY)
        {
            // 基类中的基本实现，派生类可以重写提供更丰富的功能
            return Task.FromResult(true);
        }

        /// <summary>
        /// 创建Buff的复制品
        /// </summary>
        /// <returns>Buff复制品</returns>
        public virtual IBuff Clone()
        {
            return new BaseBuff(Name, Description, Type, StatusEffectType, Duration, Power, IsStackable, MaxStackCount)
            {
                RemainingDuration = RemainingDuration,
                StackCount = StackCount,
                Caster = Caster,
                Target = Target,
                IsHidden = IsHidden,
                IconPath = IconPath,
                VisualEffectId = VisualEffectId
            };
        }
    }
}