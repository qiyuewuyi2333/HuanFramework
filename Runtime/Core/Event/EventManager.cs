using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Huan.Framework.Core.Event
{
    /// <summary>
    /// 事件管理器，负责分派事件给该事件的监听者，该事件的监听者可能有很多，并且监听者也可能不止监听这一类事件
    /// </summary>
    public class EventManager : BaseManager<EventManager>
    {
        #region API

        public static void AddListener(EventConstType eventType, EventListener listener)
        {
            _instance.AddListenerInternal(eventType, listener);
        }

        public static void FireEvent(EventConstType eventType, params object[] args)
        {
            _instance.FireEventInternal(eventType, args);
        }

        public static void TryRemoveListener(EventConstType eventType, EventListener eventListener)
        {
            _instance.TryRemoveListenerInternal(eventType, eventListener);
        }

        #endregion

        private Dictionary<EventConstType, HashSet<EventListener>> _listeners;

        private void AddListenerInternal(EventConstType eventType, EventListener listener)
        {
            if (!_listeners.ContainsKey(eventType))
            {
                _listeners.Add(eventType, new HashSet<EventListener>());
            }

            _listeners[eventType].Add(listener);
        }

        private void FireEventInternal(EventConstType eventType, object[] args)
        {
            var listeners = GetListener(eventType);
            if (listeners == null)
            {
                Debug.LogWarning(
                    "[EventManager] Failed to find listener for this event. If you want to listen this event, you should register it first. ");
                return;
            }

            // 调用listener的回调函数
            foreach (var listener in listeners)
            {
                listener.Invoke(eventType, args);
            }
        }


        private void TryRemoveListenerInternal(EventConstType eventType, EventListener eventListener)
        {
            if (!_listeners.ContainsKey(eventType))
            {
                Debug.LogError(
                    "[EventManager] Failed to find listener for this event. ");
                return;
            }

            var listeners = _listeners[eventType];
            if (listeners.Remove(eventListener))
            {
                if (listeners.Count == 0)
                {
                    _listeners.Remove(eventType);
                }
            }
        }


        private HashSet<EventListener> GetListener(EventConstType eventType)
        {
            return _listeners.GetValueOrDefault(eventType, null);
        }

        protected override Task InitInternal()
        {
            _listeners = new Dictionary<EventConstType, HashSet<EventListener>>();

            return Task.CompletedTask;
        }

        protected override void CleanupInternal()
        {
        }
    }
}