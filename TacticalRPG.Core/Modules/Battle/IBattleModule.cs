using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TacticalRPG.Core.Framework;
using TacticalRPG.Core.Modules.Character;
using TacticalRPG.Core.Modules.Map;

namespace TacticalRPG.Core.Modules.Battle
{
    /// <summary>
    /// 战斗模块接口
    /// </summary>
    public interface IBattleModule
    {
        /// <summary>
        /// 当前战斗
        /// </summary>
        IBattle CurrentBattle { get; }

        /// <summary>
        /// 创建新战斗
        /// </summary>
        /// <param name="mapId">地图ID</param>
        /// <param name="name">战斗名称</param>
        /// <returns>新战斗的ID</returns>
        Task<Guid> CreateBattleAsync(Guid mapId, string name);

        /// <summary>
        /// 加载战斗
        /// </summary>
        /// <param name="battleId">战斗ID</param>
        /// <returns>操作结果</returns>
        Task<bool> LoadBattleAsync(Guid battleId);

        /// <summary>
        /// 结束当前战斗
        /// </summary>
        /// <returns>操作结果</returns>
        Task<bool> EndCurrentBattleAsync();

        /// <summary>
        /// 添加角色到战斗
        /// </summary>
        /// <param name="character">角色</param>
        /// <param name="team">队伍</param>
        /// <param name="x">X坐标</param>
        /// <param name="y">Y坐标</param>
        /// <returns>操作结果</returns>
        bool AddCharacterToBattle(ICharacter character, BattleTeam team, int x, int y);

        /// <summary>
        /// 从战斗中移除角色
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <returns>操作结果</returns>
        bool RemoveCharacterFromBattle(Guid characterId);

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
        /// 开始回合
        /// </summary>
        /// <returns>操作结果</returns>
        Task<bool> StartTurnAsync();

        /// <summary>
        /// 结束回合
        /// </summary>
        /// <returns>操作结果</returns>
        Task<bool> EndTurnAsync();

        /// <summary>
        /// 移动角色
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <param name="targetX">目标X坐标</param>
        /// <param name="targetY">目标Y坐标</param>
        /// <returns>操作结果</returns>
        Task<bool> MoveCharacterAsync(Guid characterId, int targetX, int targetY);

        /// <summary>
        /// 执行攻击
        /// </summary>
        /// <param name="attackerId">攻击者ID</param>
        /// <param name="targetId">目标ID</param>
        /// <returns>攻击结果</returns>
        Task<BattleActionResult> ExecuteAttackAsync(Guid attackerId, Guid targetId);

        /// <summary>
        /// 使用技能
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <param name="skillId">技能ID</param>
        /// <param name="targetX">目标X坐标</param>
        /// <param name="targetY">目标Y坐标</param>
        /// <returns>技能使用结果</returns>
        Task<BattleActionResult> UseSkillAsync(Guid characterId, Guid skillId, int targetX, int targetY);

        /// <summary>
        /// 使用物品
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <param name="itemId">物品ID</param>
        /// <param name="targetId">目标ID</param>
        /// <returns>物品使用结果</returns>
        Task<BattleActionResult> UseItemAsync(Guid characterId, Guid itemId, Guid targetId);

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