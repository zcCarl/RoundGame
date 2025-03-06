using System;
using System.Threading.Tasks;
using TacticalRPG.Core.Modules.Battle;
using TacticalRPG.Core.Modules.Character;

namespace TacticalRPG.Implementation.Modules.Character
{
    /// <summary>
    /// 战斗角色类，实现IBattleCharacter接口
    /// </summary>
    public class BattleCharacter : IBattleCharacter
    {
        /// <summary>
        /// 角色
        /// </summary>
        public ICombatCharacter Character { get; private set; }

        /// <summary>
        /// 队伍
        /// </summary>
        public BattleTeam Team { get; private set; }

        /// <summary>
        /// X坐标
        /// </summary>
        public int X { get; private set; }

        /// <summary>
        /// Y坐标
        /// </summary>
        public int Y { get; private set; }

        /// <summary>
        /// 是否已行动
        /// </summary>
        public bool HasActed { get; private set; }

        /// <summary>
        /// 是否已移动
        /// </summary>
        public bool HasMoved { get; private set; }

        /// <summary>
        /// 当前生命值
        /// </summary>
        public int CurrentHP { get; private set; }

        /// <summary>
        /// 当前魔法值
        /// </summary>
        public int CurrentMP { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="character">角色</param>
        /// <param name="team">队伍</param>
        /// <param name="x">初始X坐标</param>
        /// <param name="y">初始Y坐标</param>
        public BattleCharacter(ICombatCharacter character, BattleTeam team, int x, int y)
        {
            Character = character ?? throw new ArgumentNullException(nameof(character));
            Team = team;
            X = x;
            Y = y;
            HasActed = false;
            HasMoved = false;
            CurrentHP = character.MaxHP;
            CurrentMP = character.MaxMP;
        }

        /// <summary>
        /// 设置位置
        /// </summary>
        /// <param name="x">X坐标</param>
        /// <param name="y">Y坐标</param>
        public void SetPosition(int x, int y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// 设置行动状态
        /// </summary>
        /// <param name="hasActed">是否已行动</param>
        public void SetActed(bool hasActed)
        {
            HasActed = hasActed;

            // 同步更新角色的行动状态
            Character.SetCanAct(!hasActed);
        }

        /// <summary>
        /// 设置移动状态
        /// </summary>
        /// <param name="hasMoved">是否已移动</param>
        public void SetMoved(bool hasMoved)
        {
            HasMoved = hasMoved;

            // 同步更新角色的移动状态
            Character.SetCanMove(!hasMoved);
        }

        /// <summary>
        /// 设置当前生命值
        /// </summary>
        /// <param name="hp">生命值</param>
        public void SetCurrentHP(int hp)
        {
            // 通过伤害或治疗来设置生命值
            if (hp < CurrentHP)
            {
                int damage = CurrentHP - hp;
                _ = Character.TakeDamageAsync(damage);
            }
            else if (hp > CurrentHP)
            {
                int heal = hp - CurrentHP;
                _ = Character.HealAsync(heal);
            }
        }

        /// <summary>
        /// 设置当前魔法值
        /// </summary>
        /// <param name="mp">魔法值</param>
        public void SetCurrentMP(int mp)
        {
            if (mp < CurrentMP)
            {
                Character.ConsumeMP(CurrentMP - mp);
            }
            else if (mp > CurrentMP)
            {
                Character.RestoreMP(mp - CurrentMP);
            }
        }

        /// <summary>
        /// 受到伤害
        /// </summary>
        /// <param name="damage">伤害值</param>
        /// <returns>实际伤害值</returns>
        public int TakeDamage(int damage)
        {
            // 使用异步方法并等待结果
            return Character.TakeDamageAsync(damage).GetAwaiter().GetResult();
        }

        /// <summary>
        /// 恢复生命值
        /// </summary>
        /// <param name="amount">恢复量</param>
        /// <returns>实际恢复量</returns>
        public int HealHP(int amount)
        {
            // 使用异步方法并等待结果
            return Character.HealAsync(amount).GetAwaiter().GetResult();
        }

        /// <summary>
        /// 消耗魔法值
        /// </summary>
        /// <param name="amount">消耗量</param>
        /// <returns>是否成功消耗</returns>
        public bool ConsumeMP(int amount)
        {
            return Character.ConsumeMP(amount);
        }

        /// <summary>
        /// 恢复魔法值
        /// </summary>
        /// <param name="amount">恢复量</param>
        /// <returns>实际恢复量</returns>
        public int RestoreMP(int amount)
        {
            return Character.RestoreMP(amount);
        }

        /// <summary>
        /// 重置回合状态
        /// </summary>
        public void ResetTurnState()
        {
            HasActed = false;
            HasMoved = false;
            Character.SetCanAct(true);
            Character.SetCanMove(true);
            Character.SetCanUseSkills(true);
        }

        /// <summary>
        /// 是否存活
        /// </summary>
        /// <returns>是否存活</returns>
        public bool IsAlive()
        {
            return Character.IsAlive;
        }

        /// <summary>
        /// 获取角色ID
        /// </summary>
        public Guid GetCharacterId()
        {
            return Character.Id;
        }

        /// <summary>
        /// 距离另一个战斗角色的曼哈顿距离
        /// </summary>
        /// <param name="other">另一个战斗角色</param>
        /// <returns>曼哈顿距离</returns>
        public int DistanceTo(IBattleCharacter other)
        {
            return Math.Abs(X - other.X) + Math.Abs(Y - other.Y);
        }

        /// <summary>
        /// 距离指定坐标的曼哈顿距离
        /// </summary>
        /// <param name="x">目标X坐标</param>
        /// <param name="y">目标Y坐标</param>
        /// <returns>曼哈顿距离</returns>
        public int DistanceTo(int x, int y)
        {
            return Math.Abs(X - x) + Math.Abs(Y - y);
        }
    }
}