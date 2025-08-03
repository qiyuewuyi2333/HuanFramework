using Huan.Framework.Core.Input;
using Huan.Framework.Runtime.Core;
using UnityEngine;

namespace Huan.Framework.Core.FrameworkComponents
{
    public class InputFrameworkInitializer : FrameworkInitializer
    {
        public override void Init()
        {
            var inputManager = FrameworkManager.Instance.CreateManager<InputManager>();
            if (inputManager == null)
            {
                Debug.LogError($"[Framework Initializer] Failed to create {nameof(InputManager)}");
                return;
            }

            InputManager.Instance = (InputManager)inputManager;

            Debug.Log($"[{nameof(InputManager)}] created over.");
        }
    }
}