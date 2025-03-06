using System;
using System.Threading.Tasks;
using TacticalRPG.Core.Modules.Character;

namespace TacticalRPG.Core.Modules.Buff
{
    /// <summary>
    /// Buff接口
    /// </summary>
    public interface IBuff
    {
        /// <summary>
        /// Buff ID
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Buff名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Buff描述
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Buff类型
        /// </summary>
        BuffType Type { get; }

        /// <summary>
        /// 状态效果类型
        /// </summary>
        StatusEffectType StatusEffectType { get; }

        /// <summary>
        /// 持续回合数（-1表示永久）
        /// </summary>
        int Duration { get; }

        /// <summary>
        /// 当前剩余回合数
        /// </summary>
        int RemainingDuration { get; }

        /// <summary>
        /// 效果强度
        /// </summary>
        int Power { get; }

        /// <summary>
        /// 是否可叠加
        /// </summary>
        bool IsStackable { get; }

        /// <summary>
        /// 当前叠加层数
        /// </summary>
        int StackCount { get; }

        /// <summary>
        /// 最大叠加层数
        /// </summary>
        int MaxStackCount { get; }

        /// <summary>
        /// 施法者
        /// </summary>
        ICombatCharacter Caster { get; }

        /// <summary>
        /// 目标
        /// </summary>
        ICombatCharacter Target { get; }

        /// <summary>
        /// 是否为隐藏效果
        /// </summary>
        bool IsHidden { get; }

        /// <summary>
        /// 图标路径
        /// </summary>
        string IconPath { get; }

        /// <summary>
        /// 视觉效果ID
        /// </summary>
        string VisualEffectId { get; }

        /// <summary>
        /// 设置名称
        /// </summary>
        /// <param name="name">名称</param>
        void SetName(string name);

        /// <summary>
        /// 设置描述
        /// </summary>
        /// <param name="description">描述</param>
        void SetDescription(string description);

        /// <summary>
        /// 设置持续回合数
        /// </summary>
        /// <param name="duration">持续回合数</param>
        void SetDuration(int duration);

        /// <summary>
        /// 设置效果强度
        /// </summary>
        /// <param name="power">效果强度</param>
        void SetPower(int power);

        /// <summary>
        /// 设置施法者
        /// </summary>
        /// <param name="caster">施法者</param>
        void SetCaster(ICombatCharacter caster);

        /// <summary>
        /// 设置目标
        /// </summary>
        /// <param name="target">目标</param>
        void SetTarget(ICombatCharacter target);

        /// <summary>
        /// 减少剩余回合数
        /// </summary>
        /// <param name="amount">减少量</param>
        /// <returns>是否已过期</returns>
        bool ReduceDuration(int amount = 1);

        /// <summary>
        /// 增加叠加层数
        /// </summary>
        /// <param name="amount">增加量</param>
        /// <returns>当前叠加层数</returns>
        int AddStack(int amount = 1);

        /// <summary>
        /// 减少叠加层数
        /// </summary>
        /// <param name="amount">减少量</param>
        /// <returns>当前叠加层数</returns>
        int RemoveStack(int amount = 1);

        /// <summary>
        /// 应用Buff效果
        /// </summary>
        /// <returns>操作结果</returns>
        Task<bool> ApplyAsync();

        /// <summary>
        /// 移除Buff效果
        /// </summary>
        /// <returns>操作结果</returns>
        Task<bool> RemoveAsync();

        /// <summary>
        /// 回合开始时触发
        /// </summary>
        /// <returns>操作结果</returns>
        Task<bool> OnTurnStartAsync();

        /// <summary>
        /// 回合结束时触发
        /// </summary>
        /// <returns>操作结果</returns>
        Task<bool> OnTurnEndAsync();

        /// <summary>
        /// 受到伤害时触发
        /// </summary>
        /// <param name="damage">伤害量</param>
        /// <param name="attacker">攻击者</param>
        /// <returns>操作结果</returns>
        Task<bool> OnDamageTakenAsync(int damage, ICombatCharacter attacker);

        /// <summary>
        /// 造成伤害时触发
        /// </summary>
        /// <param name="damage">伤害量</param>
        /// <param name="victim">受害者</param>
        /// <returns>操作结果</returns>
        Task<bool> OnDamageDealtAsync(int damage, ICombatCharacter victim);

        /// <summary>
        /// 移动前触发
        /// </summary>
        /// <param name="fromX">起始X坐标</param>
        /// <param name="fromY">起始Y坐标</param>
        /// <param name="toX">目标X坐标</param>
        /// <param name="toY">目标Y坐标</param>
        /// <returns>是否允许移动</returns>
        Task<bool> OnBeforeMoveAsync(int fromX, int fromY, int toX, int toY);

        /// <summary>
        /// 移动后触发
        /// </summary>
        /// <param name="fromX">起始X坐标</param>
        /// <param name="fromY">起始Y坐标</param>
        /// <param name="toX">目标X坐标</param>
        /// <param name="toY">目标Y坐标</param>
        /// <returns>操作结果</returns>
        Task<bool> OnAfterMoveAsync(int fromX, int fromY, int toX, int toY);

        /// <summary>
        /// 创建Buff的复制品
        /// </summary>
        /// <returns>Buff复制品</returns>
        IBuff Clone();
    }
}