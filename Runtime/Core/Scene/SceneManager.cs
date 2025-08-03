using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using cfg.Scene;
using Huan.Framework.Core.Data;
using Huan.Framework.Core.Event;
using Huan.Framework.Runtime.Core.Asset;
using UnityEngine;
using UnityEngine.SceneManagement;
using YooAsset;

namespace Huan.Framework.Core.Scene
{
    public class SceneManager : BaseManager<SceneManager>
    {
        #region API

        /// <summary>
        /// 注册场景控制器
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="sceneController"></param>
        public static void RegisterSceneController(string sceneName, ISceneController sceneController)
        {
            _instance.RegisterSceneControllerInternal(sceneName, sceneController);
        }

        /// <summary>
        /// 加载场景
        /// </summary>
        public static void LoadScene(string sceneName, LoadSceneMode loadMode = LoadSceneMode.Single)
        {
            _instance.LoadSceneInternal(sceneName, loadMode);
        }
        

        #endregion

        private TbSceneConfig _sceneConfig;
        private SceneHandle _currentSceneHandle;
        private string _currentSceneName;
        private bool _isLoading = false;
        private MonoBehaviour _coroutineRunner;

        private Dictionary<string, ISceneController> _sceneDic = new();

        public event Action<string> OnSceneLoadStart;
        public event Action<string, float> OnSceneLoadProgress;
        public event Action<string> OnSceneLoadComplete;
        public event Action<string> OnSceneUnloadComplete;

        public string CurrentSceneName => _currentSceneName;
        public bool IsLoading => _isLoading;

        private EventListener _eventListener = new EventListener();

        protected override async Task InitInternal()
        {
            // 创建协程运行器
            CreateCoroutineRunner();

            // 加载场景配置
            await LoadSceneConfig();

            Debug.Log("SceneManager初始化完成");
        }

        protected override void CleanupInternal()
        {
            // 清理当前场景
            if (_currentSceneHandle != null && _currentSceneHandle.IsValid)
            {
                _currentSceneHandle.UnloadAsync();
                _currentSceneHandle = null;
            }

            // 清理协程运行器
            if (_coroutineRunner != null)
            {
                UnityEngine.Object.DestroyImmediate(_coroutineRunner.gameObject);
                _coroutineRunner = null;
            }

            // 清理事件
            OnSceneLoadStart = null;
            OnSceneLoadProgress = null;
            OnSceneLoadComplete = null;
            OnSceneUnloadComplete = null;

            _currentSceneName = null;
            _isLoading = false;
        }

        /// <summary>
        /// 创建协程运行器
        /// </summary>
        private void CreateCoroutineRunner()
        {
            if (_coroutineRunner == null)
            {
                var runnerGO = new GameObject("[SceneManager_CoroutineRunner]");
                UnityEngine.Object.DontDestroyOnLoad(runnerGO);
                _coroutineRunner = runnerGO.AddComponent<CoroutineRunner>();
            }
        }

        /// <summary>
        /// 加载场景配置
        /// </summary>
        private async Task LoadSceneConfig()
        {
            // 这里可以通过YooAsset或Resources加载配置
            _sceneConfig = DataManager.Tables.TbSceneConfig;
            if (_sceneConfig == null)
            {
                Debug.LogError("找不到SceneConfig配置文件");
                return;
            }

            await Task.CompletedTask;
        }

        /// <summary>
        /// 加载场景
        /// </summary>
        private void LoadSceneInternal(string sceneName, LoadSceneMode loadMode = LoadSceneMode.Single)
        {
            if (!IsInitialized)
            {
                Debug.LogError("SceneManager未初始化，无法加载场景");
                return;
            }

            if (_isLoading)
            {
                Debug.LogWarning($"正在加载场景中，无法加载新场景: {sceneName}");
                return;
            }

            _coroutineRunner.StartCoroutine(LoadSceneAsync(sceneName, loadMode));
        }

        private void RegisterSceneControllerInternal(string sceneName, ISceneController sceneController)
        {
            if (_sceneDic.ContainsKey(sceneName))
            {
                Debug.LogError($"场景控制器已存在: {sceneName}");
                return;
            }

            _sceneDic.Add(sceneName, sceneController);
        }

        /// <summary>
        /// 异步加载场景
        /// </summary>
        private IEnumerator LoadSceneAsync(string sceneName, LoadSceneMode loadMode)
        {
            _isLoading = true;
            OnSceneLoadStart?.Invoke(sceneName);

            // 先卸载当前场景（如果是单场景模式）
            if (loadMode == LoadSceneMode.Single && _currentSceneHandle != null)
            {
                yield return _coroutineRunner.StartCoroutine(UnloadCurrentScene());
            }

            // 加载新场景
            var sceneInfo = _sceneConfig.Get(sceneName);
            if (sceneInfo == null)
            {
                Debug.LogError($"找不到场景配置: {sceneName}");
                _isLoading = false;
                yield break;
            }

            Debug.Log($"开始加载场景: {sceneInfo.SceneName}");

            // 使用YooAsset加载场景
            _currentSceneHandle = AssetManager.LoadSceneAsync(sceneInfo.AssetPath, loadMode);

            // 等待场景加载完成，同时报告进度
            while (!_currentSceneHandle.IsDone)
            {
                float progress = _currentSceneHandle.Progress;
                OnSceneLoadProgress?.Invoke(sceneName, progress);
                yield return null;
            }

            if (_currentSceneHandle.Status == EOperationStatus.Succeed)
            {
                _currentSceneName = sceneName;
                Debug.Log($"场景加载成功: {sceneName}");

                // 执行场景加载完成后的初始化
                yield return _coroutineRunner.StartCoroutine(OnSceneLoaded(sceneInfo));

                OnSceneLoadComplete?.Invoke(sceneName);
            }
            else
            {
                Debug.LogError($"场景加载失败: {sceneName}, 错误: {_currentSceneHandle.LastError}");
                _currentSceneHandle = null;
            }

            _isLoading = false;
        }

        /// <summary>
        /// 卸载当前场景
        /// </summary>
        private IEnumerator UnloadCurrentScene()
        {
            if (_currentSceneHandle != null && _currentSceneHandle.IsValid)
            {
                Debug.Log($"卸载场景: {_currentSceneName}");

                var unloadHandle = _currentSceneHandle.UnloadAsync();
                yield return unloadHandle;

                OnSceneUnloadComplete?.Invoke(_currentSceneName);

                _currentSceneHandle = null;
                _currentSceneName = null;
            }
        }

        /// <summary>
        /// 场景加载完成后的处理
        /// </summary>
        private IEnumerator OnSceneLoaded(SceneConfig sceneInfo)
        {
            // 等待一帧确保场景完全加载
            yield return null;

            // 执行场景特定的初始化逻辑
            yield return InitializeScene(sceneInfo.SceneName);

            // 强制垃圾回收
            System.GC.Collect();
            yield return Resources.UnloadUnusedAssets();
        }

        private IEnumerator InitializeScene(string sceneName)
        {
            var sceneController = _sceneDic[sceneName];
            sceneController.Init();
            yield return null;
        }
    }


    /// <summary>
    /// 协程运行器组件
    /// </summary>
    public class CoroutineRunner : MonoBehaviour
    {
        // 空的MonoBehaviour，仅用于运行协程
    }
}