namespace TacticalRPG.Core.Modules.AI
{
    /// <summary>
    /// AI行动结果类，表示AI执行行动的结果
    /// </summary>
    public class AIActionResult
    {
        /// <summary>
        /// 是否成功执行
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 执行的行动
        /// </summary>
        public AIAction Action { get; set; }

        /// <summary>
        /// 结果消息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 创建成功结果
        /// </summary>
        /// <param name="action">执行的行动</param>
        /// <param name="message">结果消息</param>
        /// <returns>AI行动结果</returns>
        public static AIActionResult CreateSuccess(AIAction action, string message = "")
        {
            return new AIActionResult
            {
                Success = true,
                Action = action,
                Message = message
            };
        }

        /// <summary>
        /// 创建失败结果
        /// </summary>
        /// <param name="action">尝试执行的行动</param>
        /// <param name="message">失败原因</param>
        /// <returns>AI行动结果</returns>
        public static AIActionResult CreateFailure(AIAction action, string message)
        {
            return new AIActionResult
            {
                Success = false,
                Action = action,
                Message = message
            };
        }
    }
}