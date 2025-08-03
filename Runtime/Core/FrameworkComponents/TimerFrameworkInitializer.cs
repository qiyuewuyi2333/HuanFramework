using Huan.Framework.Core.Input;
using Huan.Framework.Core.Scene;
using Huan.Framework.Core.Timer;
using Huan.Framework.Core.UI;
using Huan.Framework.Runtime.Core;
using UnityEngine;

namespace Huan.Framework.Core.FrameworkComponents
{
    public class TimerFrameworkInitializer : FrameworkInitializer
    {
        
        public override void Init()
        {
            var timerManager = FrameworkManager.Instance.CreateManager<TimerManager>();
            if (timerManager == null)
            {
                Debug.LogError($"[Framework Initializer] Failed to create {nameof(TimerManager)}");
                return;
            }
            
            TimerManager.Instance = (TimerManager)timerManager;
            Debug.Log($"[{nameof(TimerManager)}] created over.");
        }
        
    }
}