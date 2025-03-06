using System;
using System.Collections.Generic;
using TacticalRPG.Core.Framework;
using TacticalRPG.Core.Modules.Battle;
using TacticalRPG.Core.Modules.Map;

namespace TacticalRPG.Core.Modules.Projectile
{
    /// <summary>
    /// 定义投射物模块的接口，负责管理游戏中的所有投射物
    /// </summary>
    public interface IProjectileModule : IGameModule
    {
        /// <summary>
        /// 获取投射物工厂实例
        /// </summary>
        IProjectileFactory ProjectileFactory { get; }

        /// <summary>
        /// 根据ID获取投射物
        /// </summary>
        /// <param name="projectileId">投射物ID</param>
        /// <returns>找到的投射物实例，未找到则返回null</returns>
        IProjectile GetProjectile(Guid projectileId);

        /// <summary>
        /// 获取所有活动的投射物
        /// </summary>
        /// <returns>活动投射物列表</returns>
        IReadOnlyList<IProjectile> GetAllProjectiles();

        /// <summary>
        /// 获取指定位置的投射物
        /// </summary>
        /// <param name="position">位置坐标</param>
        /// <returns>在该位置的投射物列表</returns>
        IReadOnlyList<IProjectile> GetProjectilesAtPosition((int x, int y) position);

        /// <summary>
        /// 获取指定区域内的投射物
        /// </summary>
        /// <param name="position">中心位置</param>
        /// <param name="radius">搜索半径</param>
        /// <returns>在该区域内的投射物列表</returns>
        IReadOnlyList<IProjectile> GetProjectilesInArea((int x, int y) position, int radius);

        /// <summary>
        /// 获取由指定创建者创建的所有投射物
        /// </summary>
        /// <param name="creatorId">创建者ID</param>
        /// <returns>该创建者的投射物列表</returns>
        IReadOnlyList<IProjectile> GetProjectilesByCreator(Guid creatorId);

        /// <summary>
        /// 创建并注册新的投射物
        /// </summary>
        /// <param name="projectile">要注册的投射物实例</param>
        /// <returns>注册后的投射物ID</returns>
        Guid RegisterProjectile(IProjectile projectile);

        /// <summary>
        /// 创建并发射线性投射物
        /// </summary>
        /// <param name="name">投射物名称</param>
        /// <param name="creatorId">创建者ID</param>
        /// <param name="startPosition">起始位置</param>
        /// <param name="direction">方向</param>
        /// <param name="speed">速度</param>
        /// <param name="damage">伤害值</param>
        /// <param name="maxRange">最大射程</param>
        /// <param name="isPiercing">是否穿透</param>
        /// <param name="pierceCount">可穿透目标数量</param>
        /// <returns>创建的投射物ID</returns>
        Guid FireLinearProjectile(
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
        /// 创建并发射跟踪投射物
        /// </summary>
        /// <param name="name">投射物名称</param>
        /// <param name="creatorId">创建者ID</param>
        /// <param name="startPosition">起始位置</param>
        /// <param name="targetId">目标ID</param>
        /// <param name="targetPosition">目标初始位置</param>
        /// <param name="speed">速度</param>
        /// <param name="damage">伤害值</param>
        /// <param name="maxRange">最大射程</param>
        /// <returns>创建的投射物ID</returns>
        Guid FireHomingProjectile(
            string name,
            Guid creatorId,
            (int x, int y) startPosition,
            Guid targetId,
            (int x, int y) targetPosition,
            float speed = 1.0f,
            int damage = 0,
            int maxRange = 0);

        /// <summary>
        /// 创建并发射区域效果投射物
        /// </summary>
        /// <param name="name">投射物名称</param>
        /// <param name="creatorId">创建者ID</param>
        /// <param name="position">位置</param>
        /// <param name="hitRadius">效果半径</param>
        /// <param name="damage">伤害值</param>
        /// <param name="effectDuration">效果持续时间</param>
        /// <param name="hitType">命中类型</param>
        /// <returns>创建的投射物ID</returns>
        Guid CreateAreaEffect(
            string name,
            Guid creatorId,
            (int x, int y) position,
            int hitRadius,
            int damage = 0,
            int effectDuration = 0,
            ProjectileHitType hitType = ProjectileHitType.AreaDamage);

        /// <summary>
        /// 更新所有投射物状态
        /// </summary>
        /// <param name="deltaTime">时间增量</param>
        void UpdateProjectiles(float deltaTime);

        /// <summary>
        /// 处理投射物与地图的碰撞检测
        /// </summary>
        /// <param name="map">地图实例</param>
        void HandleMapCollisions(IMap map);

        /// <summary>
        /// 处理投射物与战斗单位的碰撞检测
        /// </summary>
        /// <param name="battle">战斗实例</param>
        void HandleBattleUnitCollisions(IBattle battle);

        /// <summary>
        /// 销毁指定的投射物
        /// </summary>
        /// <param name="projectileId">投射物ID</param>
        /// <param name="reason">销毁原因</param>
        /// <returns>是否成功销毁</returns>
        bool DestroyProjectile(Guid projectileId, string reason = "");

        /// <summary>
        /// 销毁所有投射物
        /// </summary>
        /// <param name="reason">销毁原因</param>
        void DestroyAllProjectiles(string reason = "");

        /// <summary>
        /// 销毁指定创建者的所有投射物
        /// </summary>
        /// <param name="creatorId">创建者ID</param>
        /// <param name="reason">销毁原因</param>
        /// <returns>销毁的投射物数量</returns>
        int DestroyProjectilesByCreator(Guid creatorId, string reason = "");

        /// <summary>
        /// 获取指定投射物的可视化信息
        /// </summary>
        /// <param name="projectileId">投射物ID</param>
        /// <returns>可视化信息字典</returns>
        Dictionary<string, object> GetProjectileVisualizationData(Guid projectileId);

        /// <summary>
        /// 设置投射物的自定义属性
        /// </summary>
        /// <param name="projectileId">投射物ID</param>
        /// <param name="key">属性键</param>
        /// <param name="value">属性值</param>
        /// <returns>是否成功设置</returns>
        bool SetProjectileProperty(Guid projectileId, string key, object value);

        /// <summary>
        /// 获取投射物的自定义属性
        /// </summary>
        /// <param name="projectileId">投射物ID</param>
        /// <param name="key">属性键</param>
        /// <returns>属性值，如不存在则返回null</returns>
        object GetProjectileProperty(Guid projectileId, string key);
    }
}