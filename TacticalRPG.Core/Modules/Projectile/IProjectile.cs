using System;
using System.Collections.Generic;
using TacticalRPG.Core.Modules.Battle;
using TacticalRPG.Core.Modules.Map;

namespace TacticalRPG.Core.Modules.Projectile
{
    /// <summary>
    /// 定义投射物的接口
    /// </summary>
    public interface IProjectile
    {
        /// <summary>
        /// 获取投射物的唯一标识符
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// 获取投射物的名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 获取投射物的类型
        /// </summary>
        ProjectileType Type { get; }

        /// <summary>
        /// 获取投射物的移动类型
        /// </summary>
        ProjectileMovementType MovementType { get; }

        /// <summary>
        /// 获取投射物的当前位置
        /// </summary>
        (int x, int y) Position { get; }

        /// <summary>
        /// 获取投射物的起始位置
        /// </summary>
        (int x, int y) StartPosition { get; }

        /// <summary>
        /// 获取投射物的目标位置（如果有）
        /// </summary>
        (int x, int y)? TargetPosition { get; }

        /// <summary>
        /// 获取投射物的创建者ID
        /// </summary>
        Guid CreatorId { get; }

        /// <summary>
        /// 获取投射物的速度（每回合移动的格子数）
        /// </summary>
        float Speed { get; }

        /// <summary>
        /// 获取投射物的最大飞行距离（0表示无限制）
        /// </summary>
        int MaxRange { get; }

        /// <summary>
        /// 获取投射物已飞行的距离
        /// </summary>
        float TraveledDistance { get; }

        /// <summary>
        /// 获取投射物的命中类型
        /// </summary>
        ProjectileHitType HitType { get; }

        /// <summary>
        /// 获取投射物的命中半径（用于区域效果）
        /// </summary>
        int HitRadius { get; }

        /// <summary>
        /// 获取投射物的伤害值
        /// </summary>
        int Damage { get; }

        /// <summary>
        /// 获取投射物的命中效果持续时间（如适用）
        /// </summary>
        int EffectDuration { get; }

        /// <summary>
        /// 获取投射物是否穿透目标
        /// </summary>
        bool IsPiercing { get; }

        /// <summary>
        /// 获取投射物可穿透的目标数量（0表示无限制）
        /// </summary>
        int PierceCount { get; }

        /// <summary>
        /// 获取投射物已命中的目标ID列表
        /// </summary>
        IReadOnlyList<Guid> HitTargets { get; }

        /// <summary>
        /// 获取投射物是否已销毁
        /// </summary>
        bool IsDestroyed { get; }

        /// <summary>
        /// 更新投射物状态
        /// </summary>
        /// <param name="deltaTime">时间增量</param>
        /// <returns>更新后的位置</returns>
        (int x, int y) Update(float deltaTime);

        /// <summary>
        /// 检查投射物是否命中指定位置
        /// </summary>
        /// <param name="position">要检查的位置</param>
        /// <returns>是否命中</returns>
        bool CheckHit((int x, int y) position);

        /// <summary>
        /// 处理与目标的碰撞
        /// </summary>
        /// <param name="targetId">目标ID</param>
        /// <param name="position">碰撞位置</param>
        /// <returns>碰撞处理结果</returns>
        ProjectileHitType HandleCollision(Guid targetId, (int x, int y) position);

        /// <summary>
        /// 处理与地形的碰撞
        /// </summary>
        /// <param name="terrain">地形</param>
        /// <param name="position">碰撞位置</param>
        /// <returns>碰撞处理结果</returns>
        ProjectileHitType HandleTerrainCollision(ITerrain terrain, (int x, int y) position);

        /// <summary>
        /// 设置投射物的目标
        /// </summary>
        /// <param name="targetId">目标ID</param>
        /// <param name="position">目标位置</param>
        void SetTarget(Guid targetId, (int x, int y) position);

        /// <summary>
        /// 销毁投射物
        /// </summary>
        /// <param name="reason">销毁原因</param>
        void Destroy(string reason = "");

        /// <summary>
        /// 获取投射物的自定义属性
        /// </summary>
        /// <param name="key">属性键</param>
        /// <returns>属性值，如不存在则返回null</returns>
        object GetProperty(string key);

        /// <summary>
        /// 设置投射物的自定义属性
        /// </summary>
        /// <param name="key">属性键</param>
        /// <param name="value">属性值</param>
        void SetProperty(string key, object value);
    }
}