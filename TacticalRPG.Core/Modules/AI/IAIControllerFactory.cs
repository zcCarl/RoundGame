using System;
using System.Collections.Generic;

namespace TacticalRPG.Core.Modules.AI
{
    /// <summary>
    /// AI控制器工厂接口
    /// </summary>
    public interface IAIControllerFactory
    {
        /// <summary>
        /// 创建AI控制器
        /// </summary>
        /// <param name="type">AI类型</param>
        /// <returns>AI控制器实例</returns>
        IAIController CreateController(AIType type);

        /// <summary>
        /// 注册自定义AI控制器工厂
        /// </summary>
        /// <param name="type">AI类型</param>
        /// <param name="factory">工厂方法</param>
        void RegisterControllerFactory(AIType type, Func<IAIController> factory);

        /// <summary>
        /// 获取所有支持的AI类型
        /// </summary>
        /// <returns>AI类型列表</returns>
        IEnumerable<AIType> GetSupportedTypes();
    }
}