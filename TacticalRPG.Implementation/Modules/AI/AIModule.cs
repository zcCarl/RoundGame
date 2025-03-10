using Microsoft.Extensions.Logging;
using TacticalRPG.Core.Framework;
using TacticalRPG.Core.Modules.AI;
using TacticalRPG.Core.Modules.Battle;

namespace TacticalRPG.Implementation.Modules.AI
{
    /// <summary>
    /// AI模块实现
    /// </summary>
    public class AIModule : BaseGameModule, IAIModule
    {
        private readonly IAIControllerFactory _controllerFactory;
        private readonly IAIStrategySelector _strategySelector;
        private readonly Dictionary<Guid, IAIController> _characterControllers = new Dictionary<Guid, IAIController>();
        private readonly Dictionary<AIType, Func<IAIController>> _registeredStrategies = new Dictionary<AIType, Func<IAIController>>();

        /// <summary>
        /// 模块名称
        /// </summary>
        public override string ModuleName => "AI";

        /// <summary>
        /// 模块优先级
        /// </summary>
        public override int Priority => 50;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="logger">日志记录器</param>
        /// <param name="controllerFactory">AI控制器工厂</param>
        /// <param name="strategySelector">AI策略选择器</param>
        public AIModule(
            IGameSystem gameSystem,
            ILogger<AIModule> logger,
            IAIControllerFactory controllerFactory,
            IAIStrategySelector strategySelector) : base(gameSystem, logger)
        {
            _controllerFactory = controllerFactory ?? throw new ArgumentNullException(nameof(controllerFactory));
            _strategySelector = strategySelector ?? throw new ArgumentNullException(nameof(strategySelector));
        }

        /// <summary>
        /// 为角色分配AI控制器
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <param name="aiType">AI类型</param>
        /// <returns>操作结果</returns>
        public Task<bool> AssignAIController(Guid characterId, AIType aiType)
        {
            try
            {
                if (_characterControllers.ContainsKey(characterId))
                {
                    Logger.LogWarning($"角色 {characterId} 已经有AI控制器，将被替换");
                    _characterControllers.Remove(characterId);
                }

                var controller = _controllerFactory.CreateController(aiType);
                if (controller == null)
                {
                    Logger.LogError($"无法为类型 {aiType} 创建AI控制器");
                    return Task.FromResult(false);
                }

                _characterControllers[characterId] = controller;
                Logger.LogInformation($"已为角色 {characterId} 分配类型为 {aiType} 的AI控制器");
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"为角色 {characterId} 分配AI控制器时发生错误");
                return Task.FromResult(false);
            }
        }

        /// <summary>
        /// 移除角色的AI控制器
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <returns>操作结果</returns>
        public Task<bool> RemoveAIController(Guid characterId)
        {
            if (!_characterControllers.ContainsKey(characterId))
            {
                Logger.LogWarning($"角色 {characterId} 没有AI控制器，无法移除");
                return Task.FromResult(false);
            }

            _characterControllers.Remove(characterId);
            Logger.LogInformation($"已移除角色 {characterId} 的AI控制器");
            return Task.FromResult(true);
        }

        /// <summary>
        /// 获取角色的AI控制器
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <returns>AI控制器</returns>
        public IAIController GetAIController(Guid characterId)
        {
            if (!_characterControllers.TryGetValue(characterId, out var controller))
            {
                Logger.LogWarning($"角色 {characterId} 没有AI控制器");
                return null;
            }

            return controller;
        }

        /// <summary>
        /// 执行AI回合
        /// </summary>
        /// <param name="character">战斗角色</param>
        /// <param name="battle">当前战斗</param>
        /// <returns>AI执行结果</returns>
        public async Task<AIActionResult> ExecuteAITurn(IBattleCharacter character, IBattle battle)
        {
            if (character == null)
            {
                Logger.LogError("无法执行AI回合：角色为null");
                return AIActionResult.CreateFailure(AIAction.CreateEndTurnAction(), "角色为null");
            }

            if (battle == null)
            {
                Logger.LogError("无法执行AI回合：战斗为null");
                return AIActionResult.CreateFailure(AIAction.CreateEndTurnAction(), "战斗为null");
            }

            if (!_characterControllers.TryGetValue(character.Character.Id, out var controller))
            {
                // 如果角色没有指定控制器，使用默认控制器
                controller = _controllerFactory.CreateController(AIType.Basic);
                _characterControllers[character.Character.Id] = controller;
                Logger.LogWarning($"角色 {character.Character.Id} 没有AI控制器，使用默认控制器");
            }

            try
            {
                var action = await controller.DecideNextAction(character, battle);
                return AIActionResult.CreateSuccess(action);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"执行角色 {character.Character.Id} 的AI回合时发生错误");
                return AIActionResult.CreateFailure(AIAction.CreateEndTurnAction(), $"AI执行错误: {ex.Message}");
            }
        }

        /// <summary>
        /// 注册自定义AI策略
        /// </summary>
        /// <param name="aiType">AI类型</param>
        /// <param name="factory">AI控制器工厂方法</param>
        /// <returns>操作结果</returns>
        public bool RegisterAIStrategy(AIType aiType, Func<IAIController> factory)
        {
            if (_registeredStrategies.ContainsKey(aiType))
            {
                Logger.LogWarning($"AI类型 {aiType} 已经注册，将被替换");
                _registeredStrategies.Remove(aiType);
            }

            _registeredStrategies[aiType] = factory;
            Logger.LogInformation($"已注册类型为 {aiType} 的AI策略");
            return true;
        }
    }
}