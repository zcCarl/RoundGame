using System;
using System.Threading.Tasks;
using TacticalRPG.Core.Framework;
using TacticalRPG.Core.Modules.Battle;
using TacticalRPG.Core.Modules.Character;

namespace TacticalRPG.Core.Modules.AI
{
    /// <summary>
    /// AI模块接口，负责管理游戏中的AI行为
    /// </summary>
    public interface IAIModule
    {
        /// <summary>
        /// 为角色分配AI控制器
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <param name="aiType">AI类型</param>
        /// <returns>操作结果</returns>
        Task<bool> AssignAIController(Guid characterId, AIType aiType);

        /// <summary>
        /// 移除角色的AI控制器
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <returns>操作结果</returns>
        Task<bool> RemoveAIController(Guid characterId);

        /// <summary>
        /// 获取角色的AI控制器
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <returns>AI控制器</returns>
        IAIController GetAIController(Guid characterId);

        /// <summary>
        /// 执行AI回合
        /// </summary>
        /// <param name="character">战斗角色</param>
        /// <param name="battle">当前战斗</param>
        /// <returns>AI执行结果</returns>
        Task<AIActionResult> ExecuteAITurn(IBattleCharacter character, IBattle battle);

        /// <summary>
        /// 注册自定义AI策略
        /// </summary>
        /// <param name="aiType">AI类型</param>
        /// <param name="factory">AI控制器工厂方法</param>
        /// <returns>操作结果</returns>
        bool RegisterAIStrategy(AIType aiType, Func<IAIController> factory);
    }
}