using UnityEngine;

namespace Huan.Framework.Runtime.Core
{
    public abstract class BaseGameBootstrap : MonoBehaviour, IGameBootstrap
    {
        private void Awake()
        {
            // 等待 GameDriver 初始化后再注册
            if (GameDriver.Instance != null)
            {
                RegisterToDriver();
            }
            else
            {
                // 如果 GameDriver 还没准备好，等待
                StartCoroutine(WaitForGameDriver());
            }
        }
        private void RegisterToDriver()
        {
            var driver = GameDriver.Instance;
            driver?.RegisterBootstrap(this);
        }

        private System.Collections.IEnumerator WaitForGameDriver()
        {
            while (GameDriver.Instance == null)
            {
                yield return null;
            }
            RegisterToDriver();
        }

        public abstract void Init();
        public abstract void UpdateGame(float deltaTime, float unscaledDeltaTime);

        public abstract void Cleanup();
    }
}