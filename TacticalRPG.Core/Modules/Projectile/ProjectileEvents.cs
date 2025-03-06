using System;
using TacticalRPG.Core.Modules.Battle;
using TacticalRPG.Core.Modules.Map;

namespace TacticalRPG.Core.Modules.Projectile
{
    /// <summary>
    /// 投射物创建事件数据
    /// </summary>
    public class ProjectileCreatedEventArgs : EventArgs
    {
        /// <summary>
        /// 投射物ID
        /// </summary>
        public Guid ProjectileId { get; }

        /// <summary>
        /// 投射物类型
        /// </summary>
        public ProjectileType Type { get; }

        /// <summary>
        /// 创建者ID（如角色ID）
        /// </summary>
        public Guid CreatorId { get; }

        /// <summary>
        /// 起始位置
        /// </summary>
        public (int x, int y) StartPosition { get; }

        /// <summary>
        /// 目标位置（如适用）
        /// </summary>
        public (int x, int y)? TargetPosition { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="projectileId">投射物ID</param>
        /// <param name="type">投射物类型</param>
        /// <param name="creatorId">创建者ID</param>
        /// <param name="startPosition">起始位置</param>
        /// <param name="targetPosition">目标位置（可选）</param>
        public ProjectileCreatedEventArgs(Guid projectileId, ProjectileType type, Guid creatorId, (int x, int y) startPosition, (int x, int y)? targetPosition = null)
        {
            ProjectileId = projectileId;
            Type = type;
            CreatorId = creatorId;
            StartPosition = startPosition;
            TargetPosition = targetPosition;
        }
    }

    /// <summary>
    /// 投射物移动事件数据
    /// </summary>
    public class ProjectileMovedEventArgs : EventArgs
    {
        /// <summary>
        /// 投射物ID
        /// </summary>
        public Guid ProjectileId { get; }

        /// <summary>
        /// 上一个位置
        /// </summary>
        public (int x, int y) PreviousPosition { get; }

        /// <summary>
        /// 新位置
        /// </summary>
        public (int x, int y) NewPosition { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="projectileId">投射物ID</param>
        /// <param name="previousPosition">上一个位置</param>
        /// <param name="newPosition">新位置</param>
        public ProjectileMovedEventArgs(Guid projectileId, (int x, int y) previousPosition, (int x, int y) newPosition)
        {
            ProjectileId = projectileId;
            PreviousPosition = previousPosition;
            NewPosition = newPosition;
        }
    }

    /// <summary>
    /// 投射物命中事件数据
    /// </summary>
    public class ProjectileHitEventArgs : EventArgs
    {
        /// <summary>
        /// 投射物ID
        /// </summary>
        public Guid ProjectileId { get; }

        /// <summary>
        /// 命中位置
        /// </summary>
        public (int x, int y) HitPosition { get; }

        /// <summary>
        /// 被命中的目标ID（如果有）
        /// </summary>
        public Guid? TargetId { get; }

        /// <summary>
        /// 命中类型
        /// </summary>
        public ProjectileHitType HitType { get; }

        /// <summary>
        /// 命中效果数据
        /// </summary>
        public object HitData { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="projectileId">投射物ID</param>
        /// <param name="hitPosition">命中位置</param>
        /// <param name="hitType">命中类型</param>
        /// <param name="targetId">被命中的目标ID（可选）</param>
        /// <param name="hitData">命中效果数据（可选）</param>
        public ProjectileHitEventArgs(Guid projectileId, (int x, int y) hitPosition, ProjectileHitType hitType, Guid? targetId = null, object hitData = null)
        {
            ProjectileId = projectileId;
            HitPosition = hitPosition;
            HitType = hitType;
            TargetId = targetId;
            HitData = hitData;
        }
    }

    /// <summary>
    /// 投射物销毁事件数据
    /// </summary>
    public class ProjectileDestroyedEventArgs : EventArgs
    {
        /// <summary>
        /// 投射物ID
        /// </summary>
        public Guid ProjectileId { get; }

        /// <summary>
        /// 销毁原因
        /// </summary>
        public string Reason { get; }

        /// <summary>
        /// 最终位置
        /// </summary>
        public (int x, int y) FinalPosition { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="projectileId">投射物ID</param>
        /// <param name="finalPosition">最终位置</param>
        /// <param name="reason">销毁原因</param>
        public ProjectileDestroyedEventArgs(Guid projectileId, (int x, int y) finalPosition, string reason = "")
        {
            ProjectileId = projectileId;
            FinalPosition = finalPosition;
            Reason = reason;
        }
    }
}