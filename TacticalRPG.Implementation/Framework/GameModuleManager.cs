using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TacticalRPG.Core.Framework;

namespace TacticalRPG.Implementation.Framework
{
    /// <summary>
    /// 游戏模块管理器实现类
    /// </summary>
    public class GameModuleManager : IGameModuleManager
    {
        private readonly Dictionary<string, IGameModule> _modules = new Dictionary<string, IGameModule>();
        private readonly ILogger<GameModuleManager> _logger;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="logger">日志记录器</param>
        public GameModuleManager(ILogger<GameModuleManager> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// 注册模块
        /// </summary>
        /// <param name="module">要注册的模块</param>
        public void RegisterModule(IGameModule module)
        {
            if (module == null)
                throw new ArgumentNullException(nameof(module));

            if (_modules.ContainsKey(module.ModuleName))
            {
                _logger.LogWarning($"模块 {module.ModuleName} 已经注册，将被覆盖");
                _modules[module.ModuleName] = module;
            }
            else
            {
                _modules.Add(module.ModuleName, module);
                _logger.LogInformation($"模块 {module.ModuleName} 已注册");
            }
        }

        /// <summary>
        /// 取消注册模块
        /// </summary>
        /// <param name="moduleName">要取消注册的模块名称</param>
        public void UnregisterModule(string moduleName)
        {
            if (string.IsNullOrEmpty(moduleName))
                throw new ArgumentNullException(nameof(moduleName));

            if (_modules.ContainsKey(moduleName))
            {
                _modules.Remove(moduleName);
                _logger.LogInformation($"模块 {moduleName} 已取消注册");
            }
            else
            {
                _logger.LogWarning($"尝试取消注册不存在的模块: {moduleName}");
            }
        }

        /// <summary>
        /// 获取模块
        /// </summary>
        /// <typeparam name="T">模块类型</typeparam>
        /// <returns>模块实例</returns>
        public T GetModule<T>() where T : IGameModule
        {
            foreach (var module in _modules.Values)
            {
                if (module is T typedModule)
                {
                    return typedModule;
                }
            }

            _logger.LogWarning($"未找到类型为 {typeof(T).Name} 的模块");
            return default;
        }

        /// <summary>
        /// 根据名称获取模块
        /// </summary>
        /// <param name="moduleName">模块名称</param>
        /// <returns>模块实例</returns>
        public IGameModule GetModule(string moduleName)
        {
            if (string.IsNullOrEmpty(moduleName))
                throw new ArgumentNullException(nameof(moduleName));

            if (_modules.TryGetValue(moduleName, out var module))
            {
                return module;
            }

            _logger.LogWarning($"未找到名称为 {moduleName} 的模块");
            return null;
        }

        /// <summary>
        /// 获取所有已注册的模块
        /// </summary>
        /// <returns>所有模块的列表</returns>
        public IReadOnlyList<IGameModule> GetAllModules()
        {
            return _modules.Values.ToList().AsReadOnly();
        }

        /// <summary>
        /// 初始化所有模块
        /// </summary>
        public async Task InitializeAllModules()
        {
            _logger.LogInformation("开始初始化所有模块...");

            // 按优先级排序模块并初始化
            var orderedModules = _modules.Values.OrderByDescending(m => m.Priority).ToList();

            foreach (var module in orderedModules)
            {
                try
                {
                    await module.Initialize();
                    _logger.LogInformation($"模块 {module.ModuleName} 初始化成功");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"初始化模块 {module.ModuleName} 时发生错误");
                }
            }

            _logger.LogInformation("所有模块初始化完成");
        }

        /// <summary>
        /// 启动所有模块
        /// </summary>
        public async Task StartAllModules()
        {
            _logger.LogInformation("开始启动所有模块...");

            // 按优先级排序模块并启动
            var orderedModules = _modules.Values.OrderByDescending(m => m.Priority).ToList();

            foreach (var module in orderedModules)
            {
                try
                {
                    await module.Start();
                    _logger.LogInformation($"模块 {module.ModuleName} 启动成功");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"启动模块 {module.ModuleName} 时发生错误");
                }
            }

            _logger.LogInformation("所有模块启动完成");
        }

        /// <summary>
        /// 暂停所有模块
        /// </summary>
        public async Task PauseAllModules()
        {
            _logger.LogInformation("开始暂停所有模块...");

            // 按优先级倒序暂停模块
            var orderedModules = _modules.Values.OrderBy(m => m.Priority).ToList();

            foreach (var module in orderedModules)
            {
                try
                {
                    await module.Pause();
                    _logger.LogInformation($"模块 {module.ModuleName} 暂停成功");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"暂停模块 {module.ModuleName} 时发生错误");
                }
            }

            _logger.LogInformation("所有模块暂停完成");
        }

        /// <summary>
        /// 恢复所有模块
        /// </summary>
        public async Task ResumeAllModules()
        {
            _logger.LogInformation("开始恢复所有模块...");

            // 按优先级排序恢复模块
            var orderedModules = _modules.Values.OrderByDescending(m => m.Priority).ToList();

            foreach (var module in orderedModules)
            {
                try
                {
                    await module.Resume();
                    _logger.LogInformation($"模块 {module.ModuleName} 恢复成功");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"恢复模块 {module.ModuleName} 时发生错误");
                }
            }

            _logger.LogInformation("所有模块恢复完成");
        }

        /// <summary>
        /// 停止所有模块
        /// </summary>
        public async Task StopAllModules()
        {
            _logger.LogInformation("开始停止所有模块...");

            // 按优先级倒序停止模块
            var orderedModules = _modules.Values.OrderBy(m => m.Priority).ToList();

            foreach (var module in orderedModules)
            {
                try
                {
                    await module.Stop();
                    _logger.LogInformation($"模块 {module.ModuleName} 停止成功");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"停止模块 {module.ModuleName} 时发生错误");
                }
            }

            _logger.LogInformation("所有模块停止完成");
        }

        /// <summary>
        /// 卸载所有模块
        /// </summary>
        public async Task UnloadAllModules()
        {
            _logger.LogInformation("开始卸载所有模块...");

            // 按优先级倒序卸载模块
            var orderedModules = _modules.Values.OrderBy(m => m.Priority).ToList();

            foreach (var module in orderedModules)
            {
                try
                {
                    await module.Unload();
                    _logger.LogInformation($"模块 {module.ModuleName} 卸载成功");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"卸载模块 {module.ModuleName} 时发生错误");
                }
            }

            _modules.Clear();
            _logger.LogInformation("所有模块卸载完成");
        }
    }
}