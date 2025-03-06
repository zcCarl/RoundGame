using System;
using System.Threading.Tasks;

namespace TacticalRPG.Core.Modules.Character
{
    /// <summary>
    /// 战斗角色接口，扩展基础角色接口以提供战斗相关功能
    /// </summary>
    public interface ICombatCharacter : ICharacter
    {
        /// <summary>
        /// 当前生命值
        /// </summary>
        int CurrentHP { get; }

        /// <summary>
        /// 当前魔法值
        /// </summary>
        int CurrentMP { get; }

        /// <summary>
        /// 是否可以行动
        /// </summary>
        bool CanAct { get; }

        /// <summary>
        /// 是否可以移动
        /// </summary>
        bool CanMove { get; }

        /// <summary>
        /// 是否可以使用技能
        /// </summary>
        bool CanUseSkills { get; }

        /// <summary>
        /// 是否存活
        /// </summary>
        bool IsAlive { get; }

        /// <summary>
        /// 设置是否可以行动
        /// </summary>
        /// <param name="canAct">是否可以行动</param>
        void SetCanAct(bool canAct);

        /// <summary>
        /// 设置是否可以移动
        /// </summary>
        /// <param name="canMove">是否可以移动</param>
        void SetCanMove(bool canMove);

        /// <summary>
        /// 设置是否可以使用技能
        /// </summary>
        /// <param name="canUseSkills">是否可以使用技能</param>
        void SetCanUseSkills(bool canUseSkills);

        /// <summary>
        /// 受到伤害
        /// </summary>
        /// <param name="damage">伤害值</param>
        /// <param name="source">伤害来源</param>
        /// <returns>实际伤害值</returns>
        Task<int> TakeDamageAsync(int damage, ICharacter source = null);

        /// <summary>
        /// 治疗生命值
        /// </summary>
        /// <param name="amount">治疗量</param>
        /// <param name="source">治疗来源</param>
        /// <returns>实际恢复值</returns>
        Task<int> HealAsync(int amount, ICharacter source = null);

        /// <summary>
        /// 恢复魔法值
        /// </summary>
        /// <param name="amount">恢复量</param>
        /// <returns>实际恢复值</returns>
        int RestoreMP(int amount);

        /// <summary>
        /// 消耗魔法值
        /// </summary>
        /// <param name="amount">消耗量</param>
        /// <returns>是否成功消耗</returns>
        bool ConsumeMP(int amount);
    }
}