using UnityEngine;

namespace Huan.Framework.Base
{
    public abstract class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        private static readonly object _lock = new object();
        private static bool _applicationIsQuitting = false;

        public static T Instance
        {
            get
            {
                if (_applicationIsQuitting)
                {
                    Debug.LogWarning($"[Singleton] Instance '{typeof(T)}' already destroyed on application quit.");
                    return null;
                }

                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = FindObjectOfType<T>();

                            if (_instance == null)
                            {
                                GameObject singleton = new GameObject($"[Singleton] {typeof(T).Name}");
                                _instance = singleton.AddComponent<T>();

                                // 可选：设置为DontDestroyOnLoad
                                if (Application.isPlaying)
                                {
                                    DontDestroyOnLoad(singleton);
                                }
                            }
                        }
                    }
                }

                return _instance;
            }
        }

        public static bool HasInstance => _instance != null;

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;

                if (Application.isPlaying)
                {
                    DontDestroyOnLoad(gameObject);
                }

                OnSingletonAwake();
            }
            else if (_instance != this)
            {
                Debug.LogWarning($"[Singleton] Duplicate instance of {typeof(T).Name} found. Destroying.");
                Destroy(gameObject);
            }
        }

        protected virtual void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
                OnSingletonDestroy();
            }
        }

        protected virtual void OnApplicationQuit()
        {
            _applicationIsQuitting = true;
        }

        // 子类可重写的方法
        protected virtual void OnSingletonAwake()
        {
        }

        protected virtual void OnSingletonDestroy()
        {
        }
    }
}