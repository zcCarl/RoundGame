using System;
using TacticalRPG.Core.Modules.Character;

namespace TacticalRPG.Core.Modules.Battle
{
    /// <summary>
    /// 战斗角色接口
    /// </summary>
    public interface IBattleCharacter
    {
        /// <summary>
        /// 角色
        /// </summary>
        ICombatCharacter Character { get; }

        /// <summary>
        /// 队伍
        /// </summary>
        BattleTeam Team { get; }

        /// <summary>
        /// X坐标
        /// </summary>
        int X { get; }

        /// <summary>
        /// Y坐标
        /// </summary>
        int Y { get; }

        /// <summary>
        /// 是否已行动
        /// </summary>
        bool HasActed { get; }

        /// <summary>
        /// 是否已移动
        /// </summary>
        bool HasMoved { get; }

        /// <summary>
        /// 当前生命值
        /// </summary>
        int CurrentHP { get; }

        /// <summary>
        /// 当前魔法值
        /// </summary>
        int CurrentMP { get; }

        /// <summary>
        /// 设置位置
        /// </summary>
        /// <param name="x">X坐标</param>
        /// <param name="y">Y坐标</param>
        void SetPosition(int x, int y);

        /// <summary>
        /// 设置行动状态
        /// </summary>
        /// <param name="hasActed">是否已行动</param>
        void SetActed(bool hasActed);

        /// <summary>
        /// 设置移动状态
        /// </summary>
        /// <param name="hasMoved">是否已移动</param>
        void SetMoved(bool hasMoved);

        /// <summary>
        /// 设置当前生命值
        /// </summary>
        /// <param name="hp">生命值</param>
        void SetCurrentHP(int hp);

        /// <summary>
        /// 设置当前魔法值
        /// </summary>
        /// <param name="mp">魔法值</param>
        void SetCurrentMP(int mp);

        /// <summary>
        /// 受到伤害
        /// </summary>
        /// <param name="damage">伤害值</param>
        /// <returns>实际伤害值</returns>
        int TakeDamage(int damage);

        /// <summary>
        /// 恢复生命值
        /// </summary>
        /// <param name="amount">恢复量</param>
        /// <returns>实际恢复量</returns>
        int HealHP(int amount);

        /// <summary>
        /// 消耗魔法值
        /// </summary>
        /// <param name="amount">消耗量</param>
        /// <returns>是否成功消耗</returns>
        bool ConsumeMP(int amount);

        /// <summary>
        /// 恢复魔法值
        /// </summary>
        /// <param name="amount">恢复量</param>
        /// <returns>实际恢复量</returns>
        int RestoreMP(int amount);

        /// <summary>
        /// 重置回合状态
        /// </summary>
        void ResetTurnState();

        /// <summary>
        /// 是否存活
        /// </summary>
        /// <returns>是否存活</returns>
        bool IsAlive();
    }
}