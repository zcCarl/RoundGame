using System;

namespace TacticalRPG.Core.Modules.AI
{
    /// <summary>
    /// 战术位置类，用于AI战场评估
    /// </summary>
    public class TacticalPosition
    {
        /// <summary>
        /// X坐标
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// Y坐标
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// 位置的战术价值（0-100）
        /// </summary>
        public int Value { get; set; }

        /// <summary>
        /// 位置的安全程度（0-100）
        /// </summary>
        public int Safety { get; set; }

        /// <summary>
        /// 到达该位置所需的移动距离
        /// </summary>
        public int MovementRequired { get; set; }

        /// <summary>
        /// 位置提供的防御加成
        /// </summary>
        public int DefenseBonus { get; set; }

        /// <summary>
        /// 从该位置可攻击的敌人数量
        /// </summary>
        public int AttackableEnemies { get; set; }

        /// <summary>
        /// 位置的危险等级
        /// </summary>
        public int DangerLevel { get; set; }

        /// <summary>
        /// 位置可能的战术用途
        /// </summary>
        public TacticalUseType TacticalUse { get; set; }
    }

    /// <summary>
    /// 战术位置用途类型
    /// </summary>
    public enum TacticalUseType
    {
        /// <summary>
        /// 攻击位置
        /// </summary>
        Attack,

        /// <summary>
        /// 防御位置
        /// </summary>
        Defense,

        /// <summary>
        /// 支援位置
        /// </summary>
        Support,

        /// <summary>
        /// 撤退位置
        /// </summary>
        Retreat,

        /// <summary>
        /// 控制位置
        /// </summary>
        Control,

        /// <summary>
        /// 包围位置
        /// </summary>
        Surround
    }
}