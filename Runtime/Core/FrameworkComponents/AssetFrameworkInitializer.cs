using Huan.Framework.Runtime.Core;
using Huan.Framework.Runtime.Core.Asset;
using UnityEngine;

namespace Huan.Framework.Core.FrameworkComponents
{
    public class AssetFrameworkInitializer : FrameworkInitializer
    {
        public override void Init()
        {
            var assetManager = FrameworkManager.Instance.CreateManager<AssetManager>();
            if (assetManager == null)
            {
                Debug.LogError($"[Framework Initializer] Failed to create {nameof(AssetManager)}");
                return;
            }
            
            AssetManager.Instance = (AssetManager)assetManager;

            Debug.Log($"[{nameof(AssetManager)}] created over.");
        }
    }
}