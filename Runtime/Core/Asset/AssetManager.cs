using System.Collections;
using System.Threading.Tasks;
using Huan.Framework.Core;
using Huan.Framework.Core.Const;
using UnityEngine;
using UnityEngine.SceneManagement;
using YooAsset;

namespace Huan.Framework.Runtime.Core.Asset
{
    public class AssetManager : BaseManager<AssetManager>
    {
        private ResourcePackage _package;

        #region API

        public static T LoadAsset<T>(string assetPath) where T : UnityEngine.Object
        {
            return _instance.LoadAssetInternal<T>(assetPath);
        }

        public static AssetHandle LoadAssetAsync<T>(string assetPath) where T : UnityEngine.Object
        {
            return _instance.LoadAssetAsyncInternal<T>(assetPath);
        }

        public static SceneHandle LoadSceneAsync(string assetPath, LoadSceneMode mode = LoadSceneMode.Single)
        {
            return _instance.LoadSceneAsyncInternal(assetPath, mode);
        }

        #endregion

        protected override Task InitInternal()
        {
            YooAssets.Initialize();

            _package = YooAssets.CreatePackage(FrameworkConst.DefaultPackageName);

            YooAssets.SetDefaultPackage(_package);

            // 在管线中进行初始化
            return InitPackage();
        }

        protected override void CleanupInternal()
        {
            var _ = DestroyPackage();
        }

        private Task InitPackage()
        {
#if UNITY_EDITOR
            return InitPackageEditorSimulateMode();
#else
            return InitPackageOfflineMode();
#endif
        }

        private async Task InitPackageOfflineMode()
        {
            // 创建离线模式参数
            var buildinFileSysParams = FileSystemParameters.CreateDefaultBuildinFileSystemParameters();
            var initParams = new OfflinePlayModeParameters
            {
                BuildinFileSystemParameters = buildinFileSysParams
            };

            // 初始化资源包
            var initOperation = _package.InitializeAsync(initParams);
            await initOperation.Task;

            if (initOperation.Status == EOperationStatus.Succeed)
            {
                Debug.Log("Resource package in OfflineMode init succeed! ");
            }
            else
            {
                Debug.LogError($"Resource package in OfflineMode init failed: {initOperation.Error}");
                return; // 初始化失败直接返回
            }

            // 获取包版本信息
            var versionOperation = _package.RequestPackageVersionAsync();
            await versionOperation.Task;

            if (versionOperation.Status == EOperationStatus.Succeed)
            {
                Debug.Log("Package version: " + versionOperation.PackageVersion);
            }
            else
            {
                Debug.LogError($"Package version request failed: {versionOperation.Error}");
                return;
            }

            // 更新包清单
            var manifestOperation = _package.UpdatePackageManifestAsync(versionOperation.PackageVersion);
            await manifestOperation.Task;

            if (manifestOperation.Status == EOperationStatus.Succeed)
            {
                Debug.Log("Package manifest update succeed! ");
            }
            else
            {
                Debug.LogError($"Package manifest update failed: {manifestOperation.Error}");
            }
        }


        private async Task InitPackageEditorSimulateMode()
        {
            var buildRes = EditorSimulateModeHelper.SimulateBuild(FrameworkConst.DefaultPackageName);
            var packageRoot = buildRes.PackageRootDirectory;
            var editorFileSystemParams = FileSystemParameters.CreateDefaultEditorFileSystemParameters(packageRoot);
            var initParams = new EditorSimulateModeParameters
            {
                EditorFileSystemParameters = editorFileSystemParams
            };
            var initOperation = _package.InitializeAsync(initParams);
            await initOperation.Task;

            if (initOperation.Status == EOperationStatus.Succeed)
            {
                Debug.Log("Resource package in EditorSimulateMode init succeed! ");
            }
            else
            {
                Debug.LogError($"Resource package in EditorSimulateMode init failed: {initOperation.Error}");
            }

            var operation = _package.RequestPackageVersionAsync();
            await operation.Task;
            if (operation.Status == EOperationStatus.Succeed)
            {
                Debug.Log("Package version: " + operation.PackageVersion);
            }
            else
            {
                Debug.LogError($"Package version: {operation.Error}");
            }

            var operation2 = _package.UpdatePackageManifestAsync(operation.PackageVersion);
            await operation2.Task;
            if (operation2.Status == EOperationStatus.Succeed)
            {
                Debug.Log("Package manifest update succeed! ");
            }
            else
            {
                Debug.LogError($"Package manifest update failed: {operation2.Error}");
            }
        }

        private IEnumerator DestroyPackage()
        {
            var package = YooAssets.GetPackage(FrameworkConst.DefaultPackageName);
            DestroyOperation operation = package.DestroyAsync();
            yield return operation;

            string packageName = package.PackageName;
            if (YooAssets.RemovePackage(package))
            {
                Debug.Log($"Resource package {packageName} has been removed！ ");
            }
        }

        #region 同步方法

        /// <summary>
        /// 同步加载
        /// </summary>
        /// <param name="assetPath"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private T LoadAssetInternal<T>(string assetPath) where T : Object
        {
            var handle = _package.LoadAssetSync<T>(assetPath);

            return handle.AssetObject as T;
        }

        #endregion

        #region 异步方法

        private AssetHandle LoadAssetAsyncInternal<T>(string assetPath) where T : Object
        {
            var handle = _package.LoadAssetAsync<T>(assetPath);
            return handle;
        }

        private SceneHandle LoadSceneAsyncInternal(string scenePath, LoadSceneMode loadSceneMode)
        {
            var handle = _package.LoadSceneAsync(scenePath, loadSceneMode);
            return handle;
        }

        #endregion
    }
}