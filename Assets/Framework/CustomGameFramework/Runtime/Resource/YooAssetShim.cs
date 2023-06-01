/****************************************************
*	文件：YooAssetShim.cs
*	作者：hemingfei
*	邮箱：hemingfei@outlook.com
*	日期：2023/05/10 09:59:52
*	功能：暂无
*****************************************************/

using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using YooAsset;
using Object = UnityEngine.Object;

namespace CustomGameFramework.Runtime
{
    public static class YooAssetShim
    {
        private static readonly Dictionary<Object, AssetOperationHandle> _OBJ_2_HANDLES = new();

        private static readonly Dictionary<GameObject, Object> _GO_2_OBJ = new();

        private static readonly Dictionary<int, SceneOperationHandle> _SCENEID_2_HANDLES = new();

        private static int _SCENE_ID;

        private static Dictionary<string, ResourceDownloaderOperation> _PACKAGE_DOWNLOADER = new();

        public static async UniTask<T> LoadAssetAsync<T>(ResourcePackage package, string location)
            where T : Object
        {
            var handle = package.LoadAssetAsync<T>(location);

            await handle.ToUniTask();

            if (!handle.IsValid) throw new Exception($"[YooAssetShim] Failed to load asset: {location}");

            _OBJ_2_HANDLES.TryAdd(handle.AssetObject, handle);

            return handle.AssetObject as T;
        }

        public static void LoadAssetAsync<T>(ResourcePackage package, string location, Action<T> callback)
            where T : Object
        {
            var handle = package.LoadAssetAsync<T>(location);

            handle.Completed += operation =>
            {
                if (!operation.IsValid) throw new Exception($"[YooAssetShim] Failed to load asset: {location}");
                _OBJ_2_HANDLES.TryAdd(operation.AssetObject, operation);

                callback?.Invoke(operation.AssetObject as T);
            };
        }

        public static T LoadAssetSync<T>(ResourcePackage package, string location)
            where T : Object
        {
            var handle = package.LoadAssetSync<T>(location);

            if (!handle.IsValid) throw new Exception($"[YooAssetShim] Failed to load asset: {location}");

            _OBJ_2_HANDLES.TryAdd(handle.AssetObject, handle);

            return handle.AssetObject as T;
        }

        public static void ReleaseAsset(Object obj)
        {
            if (obj is null) return;

            if (_OBJ_2_HANDLES.ContainsKey(obj))
            {
                _OBJ_2_HANDLES.Remove(obj, out var handle);

                handle?.Release();
            }
        }

        public static async UniTask<GameObject> LoadGameObjectAsync(ResourcePackage package, string location,
            Transform parentTransform = null)
        {
            var handle = package.LoadAssetAsync<GameObject>(location);

            await handle.ToUniTask();

            if (!handle.IsValid) throw new Exception($"[YooAssetShim] Failed to load asset: {location}");

            _OBJ_2_HANDLES.TryAdd(handle.AssetObject, handle);

            if (Object.Instantiate(handle.AssetObject, parentTransform) is not GameObject go)
            {
                ReleaseAsset(handle.AssetObject);
                throw new Exception($"[YooAssetShim] Failed to instantiate asset: {location}");
            }

            _GO_2_OBJ.Add(go, handle.AssetObject);

            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;
            return go;
        }

        public static void LoadGameObjectAsync(ResourcePackage package, string location, Action<GameObject> callback,
            Transform parentTransform = null)
        {
            var handle = package.LoadAssetAsync<GameObject>(location);

            handle.Completed += operation =>
            {
                if (!operation.IsValid) throw new Exception($"[YooAssetShim] Failed to load asset: {location}");

                _OBJ_2_HANDLES.TryAdd(operation.AssetObject, operation);

                if (Object.Instantiate(operation.AssetObject, parentTransform) is not GameObject go)
                {
                    ReleaseAsset(operation.AssetObject);
                    throw new Exception($"[YooAssetShim] Failed to instantiate asset: {location}");
                }

                _GO_2_OBJ.Add(go, operation.AssetObject);

                go.transform.localPosition = Vector3.zero;
                go.transform.localScale = Vector3.one;
                callback?.Invoke(go);
            };
        }

        public static GameObject LoadGameObjectSync(ResourcePackage package, string location, Transform parentTransform = null)
        {
            var handle = package.LoadAssetSync<GameObject>(location);

            if (!handle.IsValid) throw new Exception($"[YooAssetShim] Failed to load asset: {location}");

            _OBJ_2_HANDLES.TryAdd(handle.AssetObject, handle);

            if (Object.Instantiate(handle.AssetObject, parentTransform) is not GameObject go)
            {
                ReleaseAsset(handle.AssetObject);
                throw new Exception($"[YooAssetShim] Failed to instantiate asset: {location}");
            }

            _GO_2_OBJ.Add(go, handle.AssetObject);

            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;
            return go;
        }

        public static void ReleaseGameObject(GameObject go)
        {
            if (go is null) return;

            Object.Destroy(go);

            _GO_2_OBJ.Remove(go, out var obj);

            ReleaseAsset(obj);
        }

        public static async UniTask<int> LoadSceneAsync(ResourcePackage package, string location, IProgress<float> progress,
            LoadSceneMode sceneMode = LoadSceneMode.Single, bool activateOnLoad = true)
        {
            var handle = package.LoadSceneAsync(location, sceneMode, activateOnLoad);

            await handle.ToUniTask(progress);

            if (!handle.IsValid) throw new Exception($"[YooAssetShim] Failed to load scene: {location}");

            var sceneId = _SCENE_ID++;

            _SCENEID_2_HANDLES.TryAdd(sceneId, handle);

            return sceneId;
        }

        public static async void LoadSceneAsync(ResourcePackage package, string location, IProgress<float> progress, Action<int> callback,
            LoadSceneMode sceneMode = LoadSceneMode.Single, bool activateOnLoad = true)
        {
            var handle = package.LoadSceneAsync(location, sceneMode, activateOnLoad);

            await handle.ToUniTask(progress);

            if (!handle.IsValid) throw new Exception($"[YooAssetShim] Failed to load scene: {location}");

            var sceneId = _SCENE_ID++;

            _SCENEID_2_HANDLES.TryAdd(sceneId, handle);

            callback?.Invoke(sceneId);
        }

        public static void UnloadSceneAsync(int sceneId)
        {
            if (_SCENEID_2_HANDLES.ContainsKey(sceneId))
            {
                _SCENEID_2_HANDLES.Remove(sceneId, out var handle);
                handle?.UnloadAsync();
            }
        }

        #region 初始化、更新等

        /// <summary>
        ///     初始化 yooasset 并设置默认 package
        /// </summary>
        public static void InitializeYooAsset()
        {
            // 初始化资源系统
            YooAssets.Initialize();
            YooAssets.SetOperationSystemMaxTimeSlice(30);
        }
        
        /// <summary>
        ///     获取 package
        /// </summary>
        public static ResourcePackage GetPackage(string packageName, bool isSetDefaultPackageWhenCreate = false)
        {
            // 创建默认的资源包
            var defaultPackage = YooAssets.TryGetPackage(packageName);
            if (defaultPackage == null)
            {
                defaultPackage = YooAssets.CreatePackage(packageName);
                if (isSetDefaultPackageWhenCreate)
                {
                    // 设置该资源包为默认的资源包，可以使用YooAssets相关加载接口加载该资源包内容。
                    YooAssets.SetDefaultPackage(defaultPackage);
                }
            }

            return defaultPackage;
        }

        /// <summary>
        ///     初始化 package
        /// </summary>
        public static async UniTask<InitializationOperation> InitializePackageAsync(ResourcePackage package, EPlayMode playMode,
            string cdnURL,
            IQueryServices queryServices,
            IDecryptionServices decryptionServices = null)
        {
            InitializeParameters parameters = playMode switch
            {
                EPlayMode.EditorSimulateMode => new EditorSimulateModeParameters
                {
                    SimulateManifestFilePath = EditorSimulateModeHelper.SimulateBuild(package.PackageName)
                },
                EPlayMode.OfflinePlayMode => new OfflinePlayModeParameters
                {
                    DecryptionServices = decryptionServices
                },
                EPlayMode.HostPlayMode => new HostPlayModeParameters
                {
                    QueryServices = queryServices,
                    DecryptionServices = decryptionServices,
                    DefaultHostServer = cdnURL,
                    FallbackHostServer = cdnURL
                },
                _ => throw new ArgumentOutOfRangeException(nameof(playMode), playMode, null)
            };
            var initializeOperation = package.InitializeAsync(parameters);
            await initializeOperation.ToUniTask();
            return initializeOperation;
        }

        /// <summary>
        ///     更新版本号
        /// </summary>
        public static async UniTask<string> UpdateStaticVersion(ResourcePackage package, bool urlAppendTimeTicks = false, int time_out = 60)
        {
            var operation = package.UpdatePackageVersionAsync(urlAppendTimeTicks, time_out);

            await operation.ToUniTask();

            if (operation.Status != EOperationStatus.Succeed) return string.Empty;

            return operation.PackageVersion;
        }

        /// <summary>
        ///     更新 manifest
        /// </summary>
        public static async UniTask<bool> UpdateManifest(ResourcePackage package, string packageVersion,
            int timeOut = 30)
        {
            var operation = package.UpdatePackageManifestAsync(packageVersion, true, timeOut);

            await operation.ToUniTask();

            return operation.Status == EOperationStatus.Succeed;
        }

        /// <summary>
        ///     获取更新下载内容的大小
        /// </summary>
        public static long GetDownloadSize(ResourcePackage package, int downloadingMaxNum = 10, int failedTryAgain = 3)
        {
            var downloader = package.CreateResourceDownloader(downloadingMaxNum, failedTryAgain);
            string packageName = package.PackageName;
            if (!_PACKAGE_DOWNLOADER.ContainsKey(packageName))
            {
                _PACKAGE_DOWNLOADER.Add(packageName, downloader);
            }
            else
            {
                _PACKAGE_DOWNLOADER[packageName] = downloader;
            }
            return _PACKAGE_DOWNLOADER[packageName].TotalDownloadCount == 0 ? 0 : _PACKAGE_DOWNLOADER[packageName].TotalDownloadBytes;
        }

        /// <summary>
        ///     开始下载
        /// </summary>
        public static async UniTask<bool> Download(ResourcePackage package, IProgress<float> progress = null)
        {
            string packageName = package.PackageName;
            if (!_PACKAGE_DOWNLOADER.ContainsKey(packageName) || _PACKAGE_DOWNLOADER[packageName] is null)
            {
                return false;
            }

            _PACKAGE_DOWNLOADER[packageName].BeginDownload();

            await _PACKAGE_DOWNLOADER[packageName].ToUniTask(progress);

            return _PACKAGE_DOWNLOADER[packageName].Status == EOperationStatus.Succeed;
        }

        public static async UniTask<bool> ClearCache(ResourcePackage package)
        {
            var operation = package.ClearUnusedCacheFilesAsync();
            await operation.ToUniTask();
            return operation.Status == EOperationStatus.Succeed;
        }

        #endregion
    }
}