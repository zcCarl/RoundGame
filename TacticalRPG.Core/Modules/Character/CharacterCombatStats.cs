namespace TacticalRPG.Core.Modules.Character
{
    /// <summary>
    /// 角色战斗属性类
    /// </summary>
    public class CharacterCombatStats
    {
        /// <summary>
        /// 物理攻击力
        /// </summary>
        public int PhysicalAttack { get; }

        /// <summary>
        /// 魔法攻击力
        /// </summary>
        public int MagicAttack { get; }

        /// <summary>
        /// 物理防御力
        /// </summary>
        public int PhysicalDefense { get; }

        /// <summary>
        /// 魔法防御力
        /// </summary>
        public int MagicDefense { get; }

        /// <summary>
        /// 命中率
        /// </summary>
        public int HitRate { get; }

        /// <summary>
        /// 闪避率
        /// </summary>
        public int EvasionRate { get; }

        /// <summary>
        /// 暴击率
        /// </summary>
        public int CriticalRate { get; }

        /// <summary>
        /// 暴击伤害倍率
        /// </summary>
        public double CriticalDamage { get; }

        /// <summary>
        /// 移动力
        /// </summary>
        public int Movement { get; }

        /// <summary>
        /// 攻击范围
        /// </summary>
        public int AttackRange { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="physicalAttack">物理攻击力</param>
        /// <param name="magicAttack">魔法攻击力</param>
        /// <param name="physicalDefense">物理防御力</param>
        /// <param name="magicDefense">魔法防御力</param>
        /// <param name="hitRate">命中率</param>
        /// <param name="evasionRate">闪避率</param>
        /// <param name="criticalRate">暴击率</param>
        /// <param name="criticalDamage">暴击伤害倍率</param>
        /// <param name="movement">移动力</param>
        /// <param name="attackRange">攻击范围</param>
        public CharacterCombatStats(
            int physicalAttack,
            int magicAttack,
            int physicalDefense,
            int magicDefense,
            int hitRate,
            int evasionRate,
            int criticalRate,
            double criticalDamage,
            int movement,
            int attackRange)
        {
            PhysicalAttack = physicalAttack;
            MagicAttack = magicAttack;
            PhysicalDefense = physicalDefense;
            MagicDefense = magicDefense;
            HitRate = hitRate;
            EvasionRate = evasionRate;
            CriticalRate = criticalRate;
            CriticalDamage = criticalDamage;
            Movement = movement;
            AttackRange = attackRange;
        }
    }
}