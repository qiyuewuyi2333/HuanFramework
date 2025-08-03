using Huan.Framework.Core.Event;
using Huan.Framework.Runtime.Core;
using UnityEngine;

namespace Huan.Framework.Core.FrameworkComponents
{
    public class EventFrameworkInitializer : FrameworkInitializer
    {
        public override void Init()
        {
            var assetManager = FrameworkManager.Instance.CreateManager<EventManager>();
            if (assetManager == null)
            {
                Debug.LogError($"[Framework Initializer] Failed to create {nameof(EventManager)}");
                return;
            }

            EventManager.Instance = (EventManager)assetManager;

            Debug.Log($"[{nameof(EventManager)}] created over.");
        }
    }
}