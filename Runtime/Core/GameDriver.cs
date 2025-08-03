using System;
using System.Collections.Generic;
using Huan.Framework.Base;
using Huan.Framework.Core.FrameworkComponents;
using UnityEngine;
using UnityEngine.Serialization;

namespace Huan.Framework.Runtime.Core
{
    /// <summary>
    /// 可谓是游戏本体
    /// </summary>
    public class GameDriver : SingletonMono<GameDriver>
    {
        [SerializeField] private List<FrameworkInitializer> frameworkComponents = new();
        private List<IGameBootstrap> _gameBootstraps = new();
        public bool IsInitialized { get; private set; } = false;

        private async void Awake()
        {
            Debug.Log("[GameEntry] Enter game.");

            // 解锁帧率，设置为不限制
            Application.targetFrameRate = 144; // -1 表示不限制帧率

            DontDestroyOnLoad(this);

            // 创建管理器实例
            foreach (var component in frameworkComponents)
            {
                component.Init();
            }

            // 初始化管理器
            await FrameworkManager.Instance.Init();
            Debug.Log($"[{nameof(FrameworkManager)}] Initialized.");

            IsInitialized = true;
            
            OnFrameworkInitialized?.Invoke();

            InitGameBootstraps();
        }

        private void InitGameBootstraps()
        {
            foreach (var bootstrap in _gameBootstraps)
            {
                try
                {
                    bootstrap.Init();
                    Debug.Log($"Game bootstrap {bootstrap.GetType().Name} initialized.");
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Failed to initialize bootstrap {bootstrap.GetType().Name}: {e}");
                }
            }
        }

        public void RegisterBootstrap(IGameBootstrap bootstrap)
        {
            if (!_gameBootstraps.Contains(bootstrap))
            {
                _gameBootstraps.Add(bootstrap);

                // 如果框架已经初始化，立即初始化新注册的启动器
                if (IsInitialized)
                {
                    bootstrap.Init();
                }
            }
        }

        public void UnregisterBootstrap(IGameBootstrap bootstrap)
        {
            if (_gameBootstraps.Contains(bootstrap))
            {
                bootstrap.Cleanup();
                _gameBootstraps.Remove(bootstrap);
            }
        }


        public event Action OnFrameworkInitialized;
        private event Action<float, float> OnUpdate;

        public void AddUpdate(Action<float, float> action)
        {
            OnUpdate += action;
        }

        private void Start()
        {
        }

        private void Update()
        {
            FrameworkManager.Instance.Update(Time.deltaTime, Time.unscaledDeltaTime);
            OnUpdate?.Invoke(Time.deltaTime, Time.unscaledDeltaTime);
        }
    }
}