using System;
using System.Collections.Generic;
using TacticalRPG.Core.Modules.Character;
using TacticalRPG.Core.Modules.Map;

namespace TacticalRPG.Core.Modules.Battle
{
    /// <summary>
    /// 战斗接口
    /// </summary>
    public interface IBattle
    {
        /// <summary>
        /// 战斗ID
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// 战斗名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 战斗地图
        /// </summary>
        IMap Map { get; }

        /// <summary>
        /// 当前回合数
        /// </summary>
        int CurrentTurn { get; }

        /// <summary>
        /// 当前行动的角色
        /// </summary>
        IBattleCharacter CurrentActor { get; }

        /// <summary>
        /// 战斗状态
        /// </summary>
        BattleState State { get; }

        /// <summary>
        /// 获取战斗中的所有角色
        /// </summary>
        /// <returns>战斗中的所有角色</returns>
        IReadOnlyList<IBattleCharacter> GetAllBattleCharacters();

        /// <summary>
        /// 获取指定队伍的所有角色
        /// </summary>
        /// <param name="team">队伍</param>
        /// <returns>指定队伍的所有角色</returns>
        IReadOnlyList<IBattleCharacter> GetTeamCharacters(BattleTeam team);

        /// <summary>
        /// 根据ID获取战斗角色
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <returns>战斗角色</returns>
        IBattleCharacter GetBattleCharacter(Guid characterId);

        /// <summary>
        /// 根据坐标获取战斗角色
        /// </summary>
        /// <param name="x">X坐标</param>
        /// <param name="y">Y坐标</param>
        /// <returns>战斗角色，如果该位置没有角色则返回null</returns>
        IBattleCharacter GetBattleCharacterAtPosition(int x, int y);

        /// <summary>
        /// 添加角色到战斗
        /// </summary>
        /// <param name="character">角色</param>
        /// <param name="team">队伍</param>
        /// <param name="x">X坐标</param>
        /// <param name="y">Y坐标</param>
        /// <returns>操作结果</returns>
        bool AddCharacter(ICharacter character, BattleTeam team, int x, int y);

        /// <summary>
        /// 从战斗中移除角色
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <returns>操作结果</returns>
        bool RemoveCharacter(Guid characterId);

        /// <summary>
        /// 移动角色
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <param name="targetX">目标X坐标</param>
        /// <param name="targetY">目标Y坐标</param>
        /// <returns>操作结果</returns>
        bool MoveCharacter(Guid characterId, int targetX, int targetY);

        /// <summary>
        /// 计算可移动范围
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <returns>可移动的坐标列表</returns>
        IReadOnlyList<(int X, int Y)> CalculateMoveRange(Guid characterId);

        /// <summary>
        /// 计算攻击范围
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <returns>可攻击的坐标列表</returns>
        IReadOnlyList<(int X, int Y)> CalculateAttackRange(Guid characterId);

        /// <summary>
        /// 计算技能范围
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <param name="skillId">技能ID</param>
        /// <returns>技能可影响的坐标列表</returns>
        IReadOnlyList<(int X, int Y)> CalculateSkillRange(Guid characterId, Guid skillId);
    }
}