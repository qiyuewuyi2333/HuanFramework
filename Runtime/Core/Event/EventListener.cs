using System;
using System.Collections.Generic;
using UnityEngine;

namespace Huan.Framework.Core.Event
{
    public class EventListener
    {
        // 一个Action可以存放多个回调的哦
        private readonly Dictionary<EventConstType, Action<object[]>> _eventListeners;

        public EventListener()
        {
            _eventListeners = new Dictionary<EventConstType, Action<object[]>>();
        }

        public void Invoke(EventConstType eventType, params object[] args)
        {
            if (_eventListeners.TryGetValue(eventType, out var action))
            {
                action?.Invoke(args ?? Array.Empty<object>());
            }
            else
            {
                Debug.LogError(
                    "[EventListener] Failed to find listener for this event. If you want to listen this event, you should register it first. ");
            }
        }

        private void RegisterEvent(EventConstType eventType, Action<object[]> action)
        {
            if (!_eventListeners.ContainsKey(eventType))
                _eventListeners.Add(eventType, action);
            else
                _eventListeners[eventType] += action;

            // 告诉事件管理器：“我要监听这个事件，你拿到这个事件的时候，记得分派给我哦”
            EventManager.AddListener(eventType, this);
        }

        public void UnRegisterEvent(EventConstType eventType)
        {
            // 此监听器不再监听该事件
            _eventListeners.Remove(eventType);
            // 告诉事件管理器：“我**已经**不再监听这个事件了，你不要分派给我了”
            EventManager.TryRemoveListener(eventType, this);
        }

        public void UnRegisterAllEvents()
        {
            // 如果字典为空，直接返回
            if (_eventListeners.Count == 0)
                return;

            // 批量通知事件管理器
            foreach (var eventType in _eventListeners.Keys)
            {
                EventManager.TryRemoveListener(eventType, this);
            }

            // 一次性清空
            _eventListeners.Clear();
        }

        // 无参数版本
        public void RegisterEvent(EventConstType eventType, Action action)
        {
            RegisterEvent(eventType, (parameters) => action.Invoke());
        }

        // 单参数泛型版本
        public void RegisterEvent<T>(EventConstType eventType, Action<T> action)
        {
            RegisterEvent(eventType, (parameters) =>
            {
                if (parameters != null && parameters.Length > 0 && parameters[0] is T value)
                {
                    action.Invoke(value);
                }
                else
                {
                    Debug.LogError($"事件 {eventType} 参数类型不匹配，期望 {typeof(T)}, 实际收到: {parameters?[0]?.GetType()}");
                }
            });
        }

        // 双参数版本
        public void RegisterEvent<T1, T2>(EventConstType eventType, Action<T1, T2> action)
        {
            RegisterEvent(eventType, (parameters) =>
            {
                if (parameters != null && parameters.Length >= 2 &&
                    parameters[0] is T1 param1 && parameters[1] is T2 param2)
                {
                    action.Invoke(param1, param2);
                }
                else
                {
                    Debug.LogError($"事件 {eventType} 参数数量或类型不匹配");
                }
            });
        }

        // 三参数版本
        public void RegisterEvent<T1, T2, T3>(EventConstType eventType, Action<T1, T2, T3> action)
        {
            RegisterEvent(eventType, (parameters) =>
            {
                if (parameters != null && parameters.Length >= 3 &&
                    parameters[0] is T1 param1 && parameters[1] is T2 param2 && parameters[2] is T3 param3)
                {
                    action.Invoke(param1, param2, param3);
                }
                else
                {
                    Debug.LogError($"事件 {eventType} 参数数量或类型不匹配");
                }
            });
        }
    }
}