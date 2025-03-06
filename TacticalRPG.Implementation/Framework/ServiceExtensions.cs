using Microsoft.Extensions.DependencyInjection;
using TacticalRPG.Core.Framework;
using TacticalRPG.Core.Framework.Events;
using TacticalRPG.Core.Modules.AI;
using TacticalRPG.Core.Modules.Battle;
using TacticalRPG.Core.Modules.Character;
using TacticalRPG.Core.Modules.Config;
using TacticalRPG.Core.Modules.Inventory;
using TacticalRPG.Core.Modules.Map;
using TacticalRPG.Implementation.Modules.AI;
using TacticalRPG.Implementation.Modules.Battle;
using TacticalRPG.Implementation.Modules.Character;
using TacticalRPG.Implementation.Modules.Config;
using TacticalRPG.Implementation.Modules.Inventory;
using TacticalRPG.Implementation.Modules.Map;

namespace TacticalRPG.Implementation.Framework
{
    /// <summary>
    /// 服务扩展类，用于注册游戏服务
    /// </summary>
    public static class ServiceExtensions
    {
        /// <summary>
        /// 添加游戏框架服务
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection AddGameFramework(this IServiceCollection services)
        {
            // 注册核心服务
            services.AddSingleton<IGameSystem, GameSystem>();
            services.AddSingleton<IGameModuleManager, GameModuleManager>();
            services.AddSingleton<IEventManager, EventManager>();
            services.AddSingleton<IConfigManager, ConfigManager>();
            // 注册游戏模块
            services.AddSingleton<IMapModule, MapModule>();
            services.AddSingleton<ICharacterModule, CharacterModule>();
            services.AddSingleton<IBattleModule, BattleModule>();
            services.AddSingleton<IAIModule, AIModule>();
            services.AddSingleton<IInventoryModule, InventoryModule>();

            return services;
        }

        /// <summary>
        /// 添加战术RPG实现服务
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection AddTacticalRPGImplementation(this IServiceCollection services)
        {
            // 注册AI相关服务
            services.AddSingleton<IAIControllerFactory, AIControllerFactory>();
            services.AddSingleton<IAIBattleEvaluator, AIBattleEvaluator>();
            services.AddSingleton<IAIStrategySelector, AIStrategySelector>();

            // 注册AI控制器
            services.AddTransient<AggressiveAIController>();
            services.AddTransient<DefensiveAIController>();
            services.AddTransient<BalancedAIController>();
            services.AddTransient<BerserkerAIController>();
            services.AddTransient<HealerAIController>();
            services.AddTransient<PassiveAIController>();
            services.AddTransient<SupportAIController>();
            services.AddTransient<TacticalAIController>();

            // 注册物品管理
            services.AddSingleton<IItemManager, ItemManager>();
            services.AddSingleton<IItemFactory, ItemFactory>();

            // 注册掉落管理
            services.AddSingleton<IDropManager, DropManager>();

            return services;
        }
    }
}
