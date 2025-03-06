using System;
using System.Collections.Generic;
using TacticalRPG.Core.Modules.AI;

namespace TacticalRPG.Implementation.Modules.AI
{
    /// <summary>
    /// AI控制器工厂，用于创建不同类型的AI控制器
    /// </summary>
    public class AIControllerFactory : IAIControllerFactory
    {
        private readonly Dictionary<AIType, Func<IAIController>> _controllerFactories;
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="serviceProvider">服务提供者</param>
        public AIControllerFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _controllerFactories = new Dictionary<AIType, Func<IAIController>>
            {
                { AIType.Aggressive, () => new AggressiveAIController() },
                { AIType.Defensive, () => new DefensiveAIController() },
                { AIType.Balanced, () => new BalancedAIController() },
                { AIType.Berserker, () => new BerserkerAIController() },
                { AIType.Healer, () => new HealerAIController() },
                { AIType.Support, () => new SupportAIController() },
                { AIType.Tactical, () => new TacticalAIController() },
                { AIType.Passive, () => new PassiveAIController() }
            };
        }

        /// <summary>
        /// 创建AI控制器
        /// </summary>
        /// <param name="type">AI类型</param>
        /// <returns>AI控制器实例</returns>
        public IAIController CreateController(AIType type)
        {
            if (_controllerFactories.TryGetValue(type, out var factory))
            {
                return factory();
            }

            throw new ArgumentException($"不支持的AI类型: {type}", nameof(type));
        }

        /// <summary>
        /// 注册自定义AI控制器工厂
        /// </summary>
        /// <param name="type">AI类型</param>
        /// <param name="factory">工厂方法</param>
        public void RegisterControllerFactory(AIType type, Func<IAIController> factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            _controllerFactories[type] = factory;
        }

        /// <summary>
        /// 获取所有支持的AI类型
        /// </summary>
        /// <returns>AI类型列表</returns>
        public IEnumerable<AIType> GetSupportedTypes()
        {
            return _controllerFactories.Keys;
        }
    }
}