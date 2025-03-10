using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TacticalRPG.Core.Framework.Events;

namespace TacticalRPG.Implementation.Framework
{
    /// <summary>
    /// 事件管理器实现类
    /// </summary>
    public class EventManager : IEventManager
    {
        private readonly ILogger<EventManager> _logger;
        private readonly ConcurrentDictionary<Type, List<Delegate>> _handlers = new ConcurrentDictionary<Type, List<Delegate>>();
        private readonly ConcurrentDictionary<Type, List<Delegate>> _asyncHandlers = new ConcurrentDictionary<Type, List<Delegate>>();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="logger">日志记录器</param>
        public EventManager(ILogger<EventManager> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// 发布事件
        /// </summary>
        /// <param name="gameEvent">要发布的游戏事件</param>
        public void Publish(GameEvent gameEvent)
        {
            if (gameEvent == null)
                throw new ArgumentNullException(nameof(gameEvent));

            var eventType = gameEvent.GetType();
            _logger.LogDebug($"正在发布事件: {eventType.Name}, ID: {gameEvent.Id}");

            if (_handlers.TryGetValue(eventType, out var handlers))
            {
                foreach (var handler in handlers)
                {
                    try
                    {
                        handler.DynamicInvoke(gameEvent);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"处理事件 {eventType.Name} 时发生错误");
                    }
                }
            }
        }

        /// <summary>
        /// 异步发布事件
        /// </summary>
        /// <param name="gameEvent">要发布的游戏事件</param>
        public async Task PublishAsync(GameEvent gameEvent)
        {
            if (gameEvent == null)
                throw new ArgumentNullException(nameof(gameEvent));

            var eventType = gameEvent.GetType();
            _logger.LogDebug($"正在异步发布事件: {eventType.Name}, ID: {gameEvent.Id}");

            // 处理同步处理器
            if (_handlers.TryGetValue(eventType, out var handlers))
            {
                foreach (var handler in handlers)
                {
                    try
                    {
                        handler.DynamicInvoke(gameEvent);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"处理事件 {eventType.Name} 时发生错误");
                    }
                }
            }

            // 处理异步处理器
            if (_asyncHandlers.TryGetValue(eventType, out var asyncHandlers))
            {
                var tasks = new List<Task>();

                foreach (var handler in asyncHandlers)
                {
                    try
                    {
                        var task = handler.DynamicInvoke(gameEvent);

                        tasks.Add(task as Task);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"异步处理事件 {eventType.Name} 时发生错误");
                    }
                }

                if (tasks.Count > 0)
                {
                    await Task.WhenAll(tasks);
                }
            }
        }

        /// <summary>
        /// 订阅特定类型的事件
        /// </summary>
        /// <typeparam name="T">事件类型</typeparam>
        /// <param name="handler">事件处理器</param>
        public void Subscribe<T>(Action<T> handler) where T : GameEvent
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            var eventType = typeof(T);
            _handlers.TryAdd(eventType, new List<Delegate>());

            if (_handlers.TryGetValue(eventType, out var handlers))
            {
                lock (handlers)
                {
                    handlers.Add(handler);
                }
                _logger.LogDebug($"已订阅事件: {eventType.Name}");
            }
        }

        /// <summary>
        /// 订阅特定类型的事件（异步处理）
        /// </summary>
        /// <typeparam name="T">事件类型</typeparam>
        /// <param name="handler">事件处理器</param>
        public void SubscribeAsync<T>(Func<T, Task> handler) where T : GameEvent
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            var eventType = typeof(T);
            _asyncHandlers.TryAdd(eventType, new List<Delegate>());

            if (_asyncHandlers.TryGetValue(eventType, out var handlers))
            {
                lock (handlers)
                {
                    handlers.Add(handler);
                }
                _logger.LogDebug($"已异步订阅事件: {eventType.Name}");
            }
        }

        /// <summary>
        /// 取消订阅特定类型的事件
        /// </summary>
        /// <typeparam name="T">事件类型</typeparam>
        /// <param name="handler">要取消的事件处理器</param>
        public void Unsubscribe<T>(Action<T> handler) where T : GameEvent
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            var eventType = typeof(T);

            if (_handlers.TryGetValue(eventType, out var handlers))
            {
                lock (handlers)
                {
                    handlers.Remove(handler);
                }
                _logger.LogDebug($"已取消订阅事件: {eventType.Name}");
            }
        }

        /// <summary>
        /// 取消订阅特定类型的事件（异步处理）
        /// </summary>
        /// <typeparam name="T">事件类型</typeparam>
        /// <param name="handler">要取消的事件处理器</param>
        public void UnsubscribeAsync<T>(Func<T, Task> handler) where T : GameEvent
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            var eventType = typeof(T);

            if (_asyncHandlers.TryGetValue(eventType, out var handlers))
            {
                lock (handlers)
                {
                    handlers.Remove(handler);
                }
                _logger.LogDebug($"已取消异步订阅事件: {eventType.Name}");
            }
        }
    }
}