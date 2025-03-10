using System;

namespace TacticalRPG.Core.Framework.Events
{
    /// <summary>
    /// 游戏事件基类，所有事件都应该继承此类
    /// </summary>
    public abstract class GameEvent : EventArgs
    {
        /// <summary>
        /// 事件的唯一标识符
        /// </summary>
        public Guid Id { get; } = Guid.NewGuid();

        /// <summary>
        /// 事件发生的时间
        /// </summary>
        public DateTime Timestamp { get; } = DateTime.UtcNow;

    }
}