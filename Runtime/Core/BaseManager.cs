using System.Threading.Tasks;
using Huan.Framework.Base;
using UnityEngine;

namespace Huan.Framework.Core
{
    public abstract class BaseManager<T> : IManager where T : class, new()
    {
        protected static T _instance = null;

        public static T Instance
        {
            set => _instance = value;
        }

        public async Task Init()
        {
            if (IsInitialized)
            {
                Debug.LogError($"[{typeof(T).Name}] Manager has already been initialized.");
                return;
            }

            Debug.Log($"[{typeof(T).Name}] Manager Initialize start.");

            await InitInternal();

            IsInitialized = true;
            Debug.Log($"[{typeof(T).Name}] Manager Initialize over.");
        }

        /// <summary>
        /// 初始化的实际逻辑
        /// </summary>
        protected abstract Task InitInternal();

        public void Cleanup()
        {
            if (!IsInitialized)
            {
                Debug.LogError($"[{typeof(T)}] Manager has not been initialized.");
                return;
            }

            Debug.Log($"[{typeof(T)}] Manager cleanup start.");

            CleanupInternal();

            IsInitialized = false;
            Debug.Log($"[{typeof(T)}] Manager cleanup over.");
        }

        /// <summary>
        /// 清理的实际逻辑
        /// </summary>
        protected abstract void CleanupInternal();

        protected bool IsInitialized = false;
    }
}