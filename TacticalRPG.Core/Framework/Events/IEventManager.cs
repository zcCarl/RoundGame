using System;
using System.Threading.Tasks;

namespace TacticalRPG.Core.Framework.Events
{
    /// <summary>
    /// 事件管理器接口，用于模块之间的事件通信
    /// </summary>
    public interface IEventManager
    {
        /// <summary>
        /// 发布事件
        /// </summary>
        /// <param name="gameEvent">要发布的游戏事件</param>
        Task PublishAsync(GameEvent gameEvent);

        /// <summary>
        /// 同步发布事件
        /// </summary>
        /// <param name="gameEvent">要发布的游戏事件</param>
        void Publish(GameEvent gameEvent);

        /// <summary>
        /// 订阅特定类型的事件
        /// </summary>
        /// <typeparam name="T">事件类型</typeparam>
        /// <param name="handler">事件处理器</param>
        void Subscribe<T>(Action<T> handler) where T : GameEvent;

        /// <summary>
        /// 订阅特定类型的事件（异步处理）
        /// </summary>
        /// <typeparam name="T">事件类型</typeparam>
        /// <param name="handler">事件处理器</param>
        void SubscribeAsync<T>(Func<T, Task> handler) where T : GameEvent;

        /// <summary>
        /// 取消订阅特定类型的事件
        /// </summary>
        /// <typeparam name="T">事件类型</typeparam>
        /// <param name="handler">要取消的事件处理器</param>
        void Unsubscribe<T>(Action<T> handler) where T : GameEvent;

        /// <summary>
        /// 取消订阅特定类型的事件（异步处理）
        /// </summary>
        /// <typeparam name="T">事件类型</typeparam>
        /// <param name="handler">要取消的事件处理器</param>
        void UnsubscribeAsync<T>(Func<T, Task> handler) where T : GameEvent;
    }
}