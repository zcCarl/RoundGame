using System;
using System.Threading.Tasks;
using TacticalRPG.Core.Modules.Battle;
using TacticalRPG.Core.Modules.Character;

namespace TacticalRPG.Core.Modules.AI
{
    /// <summary>
    /// AI控制器接口，负责控制单个角色的AI行为
    /// </summary>
    public interface IAIController
    {
        /// <summary>
        /// 获取AI类型
        /// </summary>
        AIType Type { get; }

        /// <summary>
        /// 获取被控制的角色ID
        /// </summary>
        Guid CharacterId { get; }

        /// <summary>
        /// 设置被控制的角色ID
        /// </summary>
        /// <param name="characterId">角色ID</param>
        void SetCharacterId(Guid characterId);

        /// <summary>
        /// 决策下一步行动
        /// </summary>
        /// <param name="character">战斗角色</param>
        /// <param name="battle">当前战斗</param>
        /// <returns>AI行动决策结果</returns>
        Task<AIAction> DecideNextAction(IBattleCharacter character, IBattle battle);

        /// <summary>
        /// 评估当前战场状态
        /// </summary>
        /// <param name="character">战斗角色</param>
        /// <param name="battle">当前战斗</param>
        /// <returns>战场评估结果</returns>
        BattleEvaluation EvaluateBattlefield(IBattleCharacter character, IBattle battle);
    }
}