using System;
using TacticalRPG.Core.Modules.Battle;
using TacticalRPG.Core.Modules.Map;

namespace TacticalRPG.Core.Modules.Projectile
{
    /// <summary>
    /// 定义投射物工厂的接口，用于创建各种类型的投射物
    /// </summary>
    public interface IProjectileFactory
    {
        /// <summary>
        /// 创建基础投射物
        /// </summary>
        /// <param name="name">投射物名称</param>
        /// <param name="type">投射物类型</param>
        /// <param name="movementType">移动类型</param>
        /// <param name="creatorId">创建者ID</param>
        /// <param name="startPosition">起始位置</param>
        /// <param name="targetPosition">目标位置（可选）</param>
        /// <param name="speed">速度</param>
        /// <param name="damage">伤害值</param>
        /// <returns>创建的投射物实例</returns>
        IProjectile CreateProjectile(
            string name,
            ProjectileType type,
            ProjectileMovementType movementType,
            Guid creatorId,
            (int x, int y) startPosition,
            (int x, int y)? targetPosition = null,
            float speed = 1.0f,
            int damage = 0);

        /// <summary>
        /// 创建直线投射物
        /// </summary>
        /// <param name="name">投射物名称</param>
        /// <param name="creatorId">创建者ID</param>
        /// <param name="startPosition">起始位置</param>
        /// <param name="direction">方向（使用地图方向枚举）</param>
        /// <param name="speed">速度</param>
        /// <param name="damage">伤害值</param>
        /// <param name="maxRange">最大射程</param>
        /// <param name="isPiercing">是否穿透</param>
        /// <param name="pierceCount">可穿透目标数量</param>
        /// <returns>创建的直线投射物</returns>
        IProjectile CreateLinearProjectile(
            string name,
            Guid creatorId,
            (int x, int y) startPosition,
            MapDirection direction,
            float speed = 1.0f,
            int damage = 0,
            int maxRange = 0,
            bool isPiercing = false,
            int pierceCount = 0);

        /// <summary>
        /// 创建跟踪投射物
        /// </summary>
        /// <param name="name">投射物名称</param>
        /// <param name="creatorId">创建者ID</param>
        /// <param name="startPosition">起始位置</param>
        /// <param name="targetId">目标ID</param>
        /// <param name="targetPosition">目标初始位置</param>
        /// <param name="speed">速度</param>
        /// <param name="damage">伤害值</param>
        /// <param name="maxRange">最大射程</param>
        /// <returns>创建的跟踪投射物</returns>
        IProjectile CreateHomingProjectile(
            string name,
            Guid creatorId,
            (int x, int y) startPosition,
            Guid targetId,
            (int x, int y) targetPosition,
            float speed = 1.0f,
            int damage = 0,
            int maxRange = 0);

        /// <summary>
        /// 创建区域效果投射物
        /// </summary>
        /// <param name="name">投射物名称</param>
        /// <param name="creatorId">创建者ID</param>
        /// <param name="position">位置</param>
        /// <param name="hitRadius">效果半径</param>
        /// <param name="damage">伤害值</param>
        /// <param name="effectDuration">效果持续时间</param>
        /// <param name="hitType">命中类型</param>
        /// <returns>创建的区域效果投射物</returns>
        IProjectile CreateAreaEffectProjectile(
            string name,
            Guid creatorId,
            (int x, int y) position,
            int hitRadius,
            int damage = 0,
            int effectDuration = 0,
            ProjectileHitType hitType = ProjectileHitType.AreaDamage);

        /// <summary>
        /// 创建抛物线投射物
        /// </summary>
        /// <param name="name">投射物名称</param>
        /// <param name="creatorId">创建者ID</param>
        /// <param name="startPosition">起始位置</param>
        /// <param name="targetPosition">目标位置</param>
        /// <param name="arcHeight">弧线高度</param>
        /// <param name="speed">速度</param>
        /// <param name="damage">伤害值</param>
        /// <param name="hitRadius">命中半径</param>
        /// <returns>创建的抛物线投射物</returns>
        IProjectile CreateParabolicProjectile(
            string name,
            Guid creatorId,
            (int x, int y) startPosition,
            (int x, int y) targetPosition,
            float arcHeight,
            float speed = 1.0f,
            int damage = 0,
            int hitRadius = 0);

        /// <summary>
        /// 创建弹跳投射物
        /// </summary>
        /// <param name="name">投射物名称</param>
        /// <param name="creatorId">创建者ID</param>
        /// <param name="startPosition">起始位置</param>
        /// <param name="direction">初始方向</param>
        /// <param name="bounceCount">弹跳次数</param>
        /// <param name="speed">速度</param>
        /// <param name="damage">伤害值</param>
        /// <param name="damageDecayFactor">每次弹跳后的伤害衰减因子</param>
        /// <returns>创建的弹跳投射物</returns>
        IProjectile CreateBouncingProjectile(
            string name,
            Guid creatorId,
            (int x, int y) startPosition,
            MapDirection direction,
            int bounceCount,
            float speed = 1.0f,
            int damage = 0,
            float damageDecayFactor = 1.0f);

        /// <summary>
        /// 创建链式投射物
        /// </summary>
        /// <param name="name">投射物名称</param>
        /// <param name="creatorId">创建者ID</param>
        /// <param name="startPosition">起始位置</param>
        /// <param name="initialTargetId">初始目标ID</param>
        /// <param name="chainCount">链式跳跃次数</param>
        /// <param name="chainRadius">链式跳跃半径</param>
        /// <param name="speed">速度</param>
        /// <param name="damage">伤害值</param>
        /// <returns>创建的链式投射物</returns>
        IProjectile CreateChainProjectile(
            string name,
            Guid creatorId,
            (int x, int y) startPosition,
            Guid initialTargetId,
            int chainCount,
            int chainRadius,
            float speed = 1.0f,
            int damage = 0);
    }
}