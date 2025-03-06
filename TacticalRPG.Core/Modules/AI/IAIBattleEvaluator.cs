using TacticalRPG.Core.Modules.Battle;

namespace TacticalRPG.Core.Modules.AI
{
    /// <summary>
    /// 战场评估器接口
    /// </summary>
    public interface IAIBattleEvaluator
    {
        /// <summary>
        /// 评估战场状况
        /// </summary>
        /// <param name="character">当前角色</param>
        /// <param name="battle">战斗实例</param>
        /// <returns>战场评估结果</returns>
        BattleEvaluation EvaluateBattle(IBattleCharacter character, IBattle battle);
    }
}