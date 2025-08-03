using Huan.Framework.Core.Data;
using Huan.Framework.Runtime.Core;
using UnityEngine;

namespace Huan.Framework.Core.FrameworkComponents
{
    public class DataFrameworkInitializer : FrameworkInitializer
    {
        public override void Init()
        {
            var dataManager = FrameworkManager.Instance.CreateManager<DataManager>();
            if (dataManager == null)
            {
                Debug.LogError($"[Framework Initializer] Failed to create {nameof(DataManager)}");
                return;
            }

            DataManager.Instance = (DataManager)dataManager;

            Debug.Log($"[{nameof(DataManager)}] created over.");
        }
    }
}