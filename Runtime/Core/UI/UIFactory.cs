using System.Threading.Tasks;
using Huan.Framework.Core.Data;
using Huan.Framework.Core.UI;
using Huan.Framework.Core.UI.View;
using Huan.Framework.Runtime.Core.Asset;
using Unity.VisualScripting;
using UnityEngine;
using YooAsset;

namespace Huan.Framework.Runtime.Core.UI
{
    public class UIFactory
    {
        private Canvas _uiRoot;
        
        public UIFactory(Canvas uiRoot)
        {
            _uiRoot = uiRoot;
        }
        public async Task<IView> CreateUI(UIName name)
        {
            var uiConfig = DataManager.Tables.TbUIConfig.DataMap[name.ToString()];
            AssetHandle uiAssetHandle = AssetManager.LoadAssetAsync<GameObject>(uiConfig.PrefabPath);
            await uiAssetHandle.Task;
            if (uiAssetHandle.AssetObject is not GameObject uiPrefab)
                Debug.LogError("Load");
            return Object.Instantiate(uiAssetHandle.AssetObject, _uiRoot.transform).GetComponent<IView>();
        }
    }
}