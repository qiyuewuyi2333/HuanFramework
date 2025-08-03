using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

namespace Huan.Framework.Core.Timer
{
    public class TimerManager : BaseManager<TimerManager>, IUpdater
    {
        #region API

        public static Timer CreateTimer(float totalTime, float timeInterval,
            bool updateExecuteOnStart = false,
            Action<float, float> onUpdate = null, Action onStart = null, Action onEnd = null)
        {
            return Timer.Create(totalTime, timeInterval, updateExecuteOnStart, onUpdate, onStart, onEnd);
        }

        public static void AddTimer(Timer timer)
        {
            _instance.AddTimerInternal(timer);
        }

        #endregion

        private Dictionary<ulong, Timer> _timers;

        private List<ulong> _removeTimers;
        protected override Task InitInternal()
        {
            _timers = new Dictionary<ulong, Timer>();
            _removeTimers = new List<ulong>();

            return Task.CompletedTask;
        }

        protected override void CleanupInternal()
        {
        }

        public void Update(float deltaTime, float unscaledDeltaTime)
        {
            if (!IsInitialized) return;
            
            foreach (var timer in _timers.Values)
            {
                if (!timer.IsValid)
                {
                    _removeTimers.Add(timer.Id);
                    continue;
                }
                timer.Update(deltaTime, unscaledDeltaTime);
            }

            foreach (var removeId in _removeTimers)
            {
                _timers.Remove(removeId);
            }
            _removeTimers.Clear();
        }

        private void AddTimerInternal(Timer timer)
        {
            _timers.Add(timer.Id, timer);
        }
        
        private void RemoveTimer(Timer timer)
        {
            if(!_timers.TryGetValue(timer.Id, out var targetTimer))
            {
                Debug.LogError("[TimerManager] Can not find timer id, Remove Timer failed");
            }

            if (targetTimer != null)
            {
                targetTimer.IsValid = false;
            }
        }
    }
}