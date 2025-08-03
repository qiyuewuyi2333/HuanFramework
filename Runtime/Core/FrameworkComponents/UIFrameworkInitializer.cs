using Huan.Framework.Core.Input;
using Huan.Framework.Core.UI;
using Huan.Framework.Runtime.Core;
using UnityEngine;

namespace Huan.Framework.Core.FrameworkComponents
{
    public class UIFrameworkInitializer : FrameworkInitializer
    {
        [SerializeField] private Transform uiRoot;
        
        public override void Init()
        {
            var uiManager = FrameworkManager.Instance.CreateManager<UIManager>();
            if (uiManager == null)
            {
                Debug.LogError($"[Framework Initializer] Failed to create {nameof(UIManager)}");
                return;
            }
            
            UIManager.Instance = (UIManager)uiManager;
            
            Debug.Log($"[{nameof(UIManager)}] created over.");
        }
    }
}