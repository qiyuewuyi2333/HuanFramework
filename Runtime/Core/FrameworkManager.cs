using System;
using System.Linq;
using System.Threading.Tasks;
using Huan.Framework.Core;
using UnityEngine;

namespace Huan.Framework.Runtime.Core
{
    public class FrameworkManager : IManager, IUpdater
    {
        public event Func<Task> OnInitialized;
        public event Action<float, float> OnUpdate;
        public event Action OnCleanup;

        public async Task Init()
        {
            var handles = OnInitialized?.GetInvocationList().Cast<Func<Task>>();
            if (handles == null)
            {
                Debug.LogError("[Framework Manager] Failed to initialize Framework Manager.");
                return;
            }

            // TODO: 当前的系统初始化是并行改串行的，后续考虑使用 “管线模式”改造优化初始化流程
            foreach (var handle in handles)
            {
                await handle();
            }
        }

        public void Cleanup()
        {
            OnCleanup?.Invoke();

            OnInitialized = null;
            OnUpdate = null;
            OnCleanup = null;
        }

        public void Update(float deltaTime, float unscaledDeltaTime)
        {
            OnUpdate?.Invoke(deltaTime, unscaledDeltaTime);
        }

        internal IManager CreateManager<T>() where T : new()
        {
            Type type = typeof(T);

            IManager manager = (IManager)Activator.CreateInstance(type);

            OnInitialized += manager.Init;

            if (manager is IUpdater updater)
            {
                OnUpdate += updater.Update;
            }

            OnCleanup += manager.Cleanup;

            return manager;
        }


        #region 单例

        private static FrameworkManager _instance;

        public static FrameworkManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new();
                }

                return _instance;
            }
        }

        #endregion
    }
}