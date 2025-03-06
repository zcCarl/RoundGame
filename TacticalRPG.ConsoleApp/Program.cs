using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TacticalRPG.Core.Framework;
using TacticalRPG.Implementation.Framework;

namespace TacticalRPG.ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("战旗回合制游戏引擎启动中...");

            // 创建依赖注入容器并配置服务
            var serviceProvider = ConfigureServices();

            // 获取游戏系统
            var gameSystem = serviceProvider.GetRequiredService<IGameSystem>();

            try
            {
                // 初始化游戏系统
                await gameSystem.Initialize();

                // 启动游戏系统
                await gameSystem.Start();

                Console.WriteLine("\n游戏系统已启动。按任意键暂停...");
                Console.ReadKey(true);

                // 暂停游戏系统
                await gameSystem.Pause();

                Console.WriteLine("\n游戏系统已暂停。按任意键恢复...");
                Console.ReadKey(true);

                // 恢复游戏系统
                await gameSystem.Resume();

                Console.WriteLine("\n游戏系统已恢复。按任意键停止...");
                Console.ReadKey(true);

                // 停止游戏系统
                await gameSystem.Stop();

                Console.WriteLine("\n游戏系统已停止。按任意键卸载...");
                Console.ReadKey(true);

                // 卸载游戏系统
                await gameSystem.Unload();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"错误: {ex.Message}");
            }

            Console.WriteLine("\n游戏系统已卸载。按任意键退出...");
            Console.ReadKey(true);
        }

        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            // 添加日志服务
            services.AddLogging(configure =>
            {
                configure.AddConsole();
                configure.SetMinimumLevel(LogLevel.Debug);
            });

            // 添加游戏框架核心服务
            services.AddGameFramework();

            return services.BuildServiceProvider();
        }
    }
}
