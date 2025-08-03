using Huan.Framework.Core.Input;
using Huan.Framework.Core.Scene;
using Huan.Framework.Core.UI;
using Huan.Framework.Runtime.Core;
using UnityEngine;

namespace Huan.Framework.Core.FrameworkComponents
{
    public class SceneFrameworkInitializer : FrameworkInitializer
    {
        
        public override void Init()
        {
            var sceneManager = FrameworkManager.Instance.CreateManager<SceneManager>();
            if (sceneManager == null)
            {
                Debug.LogError($"[Framework Initializer] Failed to create {nameof(SceneManager)}");
                return;
            }
            
            SceneManager.Instance = (SceneManager)sceneManager;
            Debug.Log($"[{nameof(SceneManager)}] created over.");
        }
        
    }
}