using System;
using System.Threading.Tasks;
using TacticalRPG.Core.Modules.Character;

namespace TacticalRPG.Implementation.Modules.Character
{
    /// <summary>
    /// 战斗角色类，继承基础角色并实现ICombatCharacter接口
    /// </summary>
    public class CombatCharacter : Character, ICombatCharacter
    {
        /// <summary>
        /// 当前生命值
        /// </summary>
        public int CurrentHP { get; private set; }

        /// <summary>
        /// 当前魔法值
        /// </summary>
        public int CurrentMP { get; private set; }

        /// <summary>
        /// 是否可以行动
        /// </summary>
        public bool CanAct { get; private set; } = true;

        /// <summary>
        /// 是否可以移动
        /// </summary>
        public bool CanMove { get; private set; } = true;

        /// <summary>
        /// 是否可以使用技能
        /// </summary>
        public bool CanUseSkills { get; private set; } = true;

        /// <summary>
        /// 是否存活
        /// </summary>
        public bool IsAlive => CurrentHP > 0;

        /// <summary>
        /// 战斗开始事件
        /// </summary>
        public event EventHandler<EventArgs> OnBattleStart;

        /// <summary>
        /// 战斗结束事件
        /// </summary>
        public event EventHandler<EventArgs> OnBattleEnd;

        /// <summary>
        /// 受到伤害事件
        /// </summary>
        public event EventHandler<DamageEventArgs> OnDamageTaken;

        /// <summary>
        /// 治疗事件
        /// </summary>
        public event EventHandler<HealEventArgs> OnHealed;

        /// <summary>
        /// 死亡事件
        /// </summary>
        public event EventHandler<EventArgs> OnDeath;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="id">角色ID</param>
        /// <param name="name">角色名称</param>
        /// <param name="characterClass">角色职业</param>
        public CombatCharacter(Guid id, string name, CharacterClass characterClass)
            : base(id, name, characterClass)
        {
            CurrentHP = MaxHP;
            CurrentMP = MaxMP;
        }

        /// <summary>
        /// 设置是否可以行动
        /// </summary>
        /// <param name="canAct">是否可以行动</param>
        public void SetCanAct(bool canAct)
        {
            CanAct = canAct;
        }

        /// <summary>
        /// 设置是否可以移动
        /// </summary>
        /// <param name="canMove">是否可以移动</param>
        public void SetCanMove(bool canMove)
        {
            CanMove = canMove;
        }

        /// <summary>
        /// 设置是否可以使用技能
        /// </summary>
        /// <param name="canUseSkills">是否可以使用技能</param>
        public void SetCanUseSkills(bool canUseSkills)
        {
            CanUseSkills = canUseSkills;
        }

        /// <summary>
        /// 受到伤害
        /// </summary>
        /// <param name="damage">伤害值</param>
        /// <param name="source">伤害来源</param>
        /// <returns>实际伤害值</returns>
        public async Task<int> TakeDamageAsync(int damage, ICharacter source = null)
        {
            if (damage <= 0 || !IsAlive)
                return 0;

            int previousHP = CurrentHP;
            CurrentHP = Math.Max(0, CurrentHP - damage);
            int actualDamage = previousHP - CurrentHP;

            // 触发受伤事件
            OnDamageTaken?.Invoke(this, new DamageEventArgs(actualDamage, source));

            // 检查是否死亡
            if (CurrentHP <= 0)
            {
                OnDeath?.Invoke(this, EventArgs.Empty);
            }

            return actualDamage;
        }

        /// <summary>
        /// 治疗生命值
        /// </summary>
        /// <param name="amount">治疗量</param>
        /// <param name="source">治疗来源</param>
        /// <returns>实际恢复值</returns>
        public async Task<int> HealAsync(int amount, ICharacter source = null)
        {
            if (amount <= 0 || !IsAlive)
                return 0;

            int previousHP = CurrentHP;
            CurrentHP = Math.Min(MaxHP, CurrentHP + amount);
            int actualHeal = CurrentHP - previousHP;

            // 触发治疗事件
            OnHealed?.Invoke(this, new HealEventArgs(actualHeal, source));

            return actualHeal;
        }

        /// <summary>
        /// 恢复魔法值
        /// </summary>
        /// <param name="amount">恢复量</param>
        /// <returns>实际恢复值</returns>
        public int RestoreMP(int amount)
        {
            if (amount <= 0 || !IsAlive)
                return 0;

            int previousMP = CurrentMP;
            CurrentMP = Math.Min(MaxMP, CurrentMP + amount);
            return CurrentMP - previousMP;
        }

        /// <summary>
        /// 消耗魔法值
        /// </summary>
        /// <param name="amount">消耗量</param>
        /// <returns>是否成功消耗</returns>
        public bool ConsumeMP(int amount)
        {
            if (amount <= 0)
                return true;

            if (CurrentMP < amount || !IsAlive)
                return false;

            CurrentMP -= amount;
            return true;
        }

        /// <summary>
        /// 重置战斗状态
        /// </summary>
        public void ResetCombatState()
        {
            CanAct = true;
            CanMove = true;
            CanUseSkills = true;
        }

        /// <summary>
        /// 启动战斗模式
        /// </summary>
        public void StartBattle()
        {
            OnBattleStart?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 结束战斗模式
        /// </summary>
        public void EndBattle()
        {
            ResetCombatState();
            OnBattleEnd?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// 伤害事件参数
    /// </summary>
    public class DamageEventArgs : EventArgs
    {
        /// <summary>
        /// 伤害值
        /// </summary>
        public int Damage { get; }

        /// <summary>
        /// 伤害来源
        /// </summary>
        public ICharacter Source { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="damage">伤害值</param>
        /// <param name="source">伤害来源</param>
        public DamageEventArgs(int damage, ICharacter source)
        {
            Damage = damage;
            Source = source;
        }
    }

    /// <summary>
    /// 治疗事件参数
    /// </summary>
    public class HealEventArgs : EventArgs
    {
        /// <summary>
        /// 治疗量
        /// </summary>
        public int Amount { get; }

        /// <summary>
        /// 治疗来源
        /// </summary>
        public ICharacter Source { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="amount">治疗量</param>
        /// <param name="source">治疗来源</param>
        public HealEventArgs(int amount, ICharacter source)
        {
            Amount = amount;
            Source = source;
        }
    }
}