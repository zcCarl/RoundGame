using TacticalRPG.Core.Modules.Character;

namespace TacticalRPG.Core.Modules.AI
{
    /// <summary>
    /// AI策略选择器接口
    /// </summary>
    public interface IAIStrategySelector
    {
        /// <summary>
        /// 为角色选择合适的AI策略
        /// </summary>
        /// <param name="character">角色</param>
        /// <returns>AI控制器</returns>
        IAIController SelectStrategy(ICharacter character);
    }
}