using System;
using Unity.Burst;
using UnityEngine;

namespace Huan.Framework.Core.Timer
{
    public class Timer : IUpdater
    {
        private static ulong sId = 0;

        private ulong _id;
        public ulong Id => _id;
        private float _totalTime; // second
        private float _timeInterval;

        private bool _updateExecuteOnStart;

        // 私有字段用于跟踪状态
        private float _elapsedTotalTime;
        private float _elapsedIntervalTime;
        private bool _hasStarted;
        public bool IsValid { get; set; }

        private event Action _onStart;
        private event Action _onEnd;
        private event Action<float, float> _onUpdate;

        public void Update(float deltaTime, float unscaledDeltaTime)
        {
            if (!IsValid)
                return;
            if (_elapsedTotalTime >= _totalTime)
            {
                _onEnd?.Invoke();
                IsValid = false;
                return;
            }

            // 第一次Update时调用OnStart
            if (!_hasStarted)
            {
                _hasStarted = true;
                _onStart?.Invoke();

                // 如果设置了开始时执行Update，则立即调用一次
                if (_updateExecuteOnStart)
                {
                    float progress = _elapsedTotalTime / _totalTime;
                    _onUpdate?.Invoke(_elapsedTotalTime, progress);
                }
            }

            // 累加时间
            _elapsedTotalTime += deltaTime;
            _elapsedIntervalTime += deltaTime;

            // 检查是否到达间隔时间
            if (_elapsedIntervalTime >= _timeInterval)
            {
                // 计算进度
                float progress = Mathf.Clamp01(_elapsedTotalTime / _totalTime);

                // 调用更新回调
                _onUpdate?.Invoke(_elapsedTotalTime, progress);

                // 重置间隔时间（保持精度）
                _elapsedIntervalTime -= _timeInterval;
            }

            // 再次检查是否完成（因为可能在本帧就完成了）
            if (_elapsedTotalTime >= _totalTime)
            {
                // 确保在结束前调用最后一次Update（如果还没调用的话）
                if (_elapsedIntervalTime > 0)
                {
                    float progress = 1.0f;
                    _onUpdate?.Invoke(_totalTime, progress);
                }

                _onEnd?.Invoke();
                IsValid = false;
            }
        }

        public static Timer Create(float totalTime, float timeInterval, bool updateExecuteOnStart = false,
            Action<float, float> onUpdate = null, Action onStart = null, Action onEnd = null)
        {
            var timer = new Timer();
            timer._id = Timer.sId++;
            timer._totalTime = totalTime;
            timer._timeInterval = timeInterval;
            timer._updateExecuteOnStart = updateExecuteOnStart;
            timer.IsValid = true;
            timer._elapsedIntervalTime = 0;
            timer._elapsedTotalTime = 0;
            timer._hasStarted = false;
            timer._onUpdate += onUpdate;
            timer._onStart += onStart;
            timer._onEnd += onEnd;

            return timer;
        }
    }
}