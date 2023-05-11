/****************************************************
*	文件：YooAssetShim.cs
*	作者：hemingfei
*	邮箱：hemingfei@outlook.com
*	日期：2023/05/10 09:59:52
*	功能：暂无
*****************************************************/

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;
using YooAsset;
using Object = UnityEngine.Object;

namespace CustomGameFramework.Runtime
{
    public static class YooAssetShim
    {
        private static readonly Dictionary<Object, AssetOperationHandle> _OBJ_2_HANDLES = new();

        private static readonly Dictionary<GameObject, Object> _GO_2_OBJ = new();
        
        private static readonly Dictionary<int, SceneOperationHandle> _SCENEID_2_HANDLES = new();

        private static int _SCENE_ID = 0;

        private static ResourceDownloaderOperation _DOWNLOADER;

        #region 初始化、更新等
        
        /// <summary>
        /// 初始化 yooasset 并设置默认 package
        /// </summary>
        public static ResourcePackage InitializeYooAsset(string defaultPackageName = "DefaultPackage")
        {
            // 初始化资源系统
            YooAssets.Initialize();
            YooAssets.SetOperationSystemMaxTimeSlice(30);
            // 创建默认的资源包
            var defaultPackage = YooAssets.TryGetPackage(defaultPackageName);
            if (defaultPackage == null)
            {
                defaultPackage = YooAssets.CreatePackage(defaultPackageName);
                // 设置该资源包为默认的资源包，可以使用YooAssets相关加载接口加载该资源包内容。
                YooAssets.SetDefaultPackage(defaultPackage);
            }

            return defaultPackage;
        }

        /// <summary>
        /// 初始化 package
        /// </summary>
        public static UniTask InitializePackageAsync(this ResourcePackage package, YooAsset.EPlayMode playMode,
            string cdnURL,
            IQueryServices queryServices,
            IDecryptionServices decryptionServices = null)
        {
            YooAsset.InitializeParameters parameters = playMode switch
            {
                YooAsset.EPlayMode.EditorSimulateMode => new YooAsset.EditorSimulateModeParameters()
                {
                    SimulateManifestFilePath = EditorSimulateModeHelper.SimulateBuild(package.PackageName),
                },
                YooAsset.EPlayMode.OfflinePlayMode => new YooAsset.OfflinePlayModeParameters()
                {
                    DecryptionServices = decryptionServices,
                },
                YooAsset.EPlayMode.HostPlayMode => new YooAsset.HostPlayModeParameters
                {
                    QueryServices = queryServices,
                    DecryptionServices = decryptionServices,
                    DefaultHostServer = cdnURL,
                    FallbackHostServer = cdnURL
                },
                _ => throw new ArgumentOutOfRangeException(nameof(playMode), playMode, null)
            };
            return package.InitializeAsync(parameters).ToUniTask();
        }

        /// <summary>
        /// 更新版本号
        /// </summary>
        public static async UniTask<string> UpdateStaticVersion(this ResourcePackage package, int time_out = 60)
        {
            var operation = package.UpdatePackageVersionAsync(true, time_out);

            await operation.ToUniTask();

            if (operation.Status != EOperationStatus.Succeed)
            {
                return string.Empty;
            }

            return operation.PackageVersion;
        }

        /// <summary>
        /// 更新 manifest
        /// </summary>
        public static async UniTask<bool> UpdateManifest(this ResourcePackage package, string packageVersion,
            int timeOut = 30)
        {
            var operation = package.UpdatePackageManifestAsync(packageVersion, true, timeOut);

            await operation.ToUniTask();

            return operation.Status == EOperationStatus.Succeed;
        }

        /// <summary>
        /// 获取更新下载内容的大小
        /// </summary>
        public static long GetDownloadSize(int downloadingMaxNum = 10, int failedTryAgain = 3)
        {
            _DOWNLOADER = YooAssets.CreateResourceDownloader(downloadingMaxNum, failedTryAgain);

            return _DOWNLOADER.TotalDownloadCount == 0 ? 0 : _DOWNLOADER.TotalDownloadBytes;
        }

        /// <summary>
        /// 开始下载
        /// </summary>
        public static async UniTask<bool> Download(IProgress<float> progress = null)
        {
            if (_DOWNLOADER is null)
            {
                return false;
            }

            _DOWNLOADER.BeginDownload();

            await _DOWNLOADER.ToUniTask(progress);

            return _DOWNLOADER.Status == EOperationStatus.Succeed;
        }
        
        #endregion

        public static async UniTask<T> LoadAssetAsync<T>(string location)
            where T : Object
        {
            var handle = YooAssets.LoadAssetAsync<T>(location);

            await handle.ToUniTask();

            if (!handle.IsValid)
            {
                throw new Exception($"[YooAssetShim] Failed to load asset: {location}");
            }

            _OBJ_2_HANDLES.TryAdd(handle.AssetObject, handle);

            return handle.AssetObject as T;
        }
        
        public static void LoadAssetAsync<T>(string location, Action<T> callback)
            where T : Object
        {
            AssetOperationHandle handle = YooAssets.LoadAssetAsync<T>(location);

            handle.Completed += operation =>
            {
                if (!operation.IsValid)
                {
                    throw new Exception($"[YooAssetShim] Failed to load asset: {location}");
                }
                _OBJ_2_HANDLES.TryAdd(operation.AssetObject, operation);

                callback?.Invoke(operation.AssetObject as T);
            };
        }
        
        public static T LoadAssetSync<T>(string location)
            where T : Object
        {
            var handle = YooAssets.LoadAssetSync<T>(location);
            
            if (!handle.IsValid)
            {
                throw new Exception($"[YooAssetShim] Failed to load asset: {location}");
            }

            _OBJ_2_HANDLES.TryAdd(handle.AssetObject, handle);

            return handle.AssetObject as T;
        }

        public static void ReleaseAsset(Object obj)
        {
            if (obj is null)
            {
                return;
            }

            if (_OBJ_2_HANDLES.ContainsKey(obj))
            {
                _OBJ_2_HANDLES.Remove(obj, out AssetOperationHandle handle);

                handle?.Release();
            }
        }

        public static async UniTask<GameObject> LoadGameObjectAsync(string location,
            Transform parentTransform = null)
        {
            var handle = YooAssets.LoadAssetAsync<GameObject>(location);

            await handle.ToUniTask();

            if (!handle.IsValid)
            {
                throw new Exception($"[YooAssetShim] Failed to load asset: {location}");
            }

            _OBJ_2_HANDLES.TryAdd(handle.AssetObject, handle);

            if (Object.Instantiate(handle.AssetObject, parentTransform) is not GameObject go)
            {
                ReleaseAsset(handle.AssetObject);
                throw new Exception($"[YooAssetShim] Failed to instantiate asset: {location}");
            }

            _GO_2_OBJ.Add(go, handle.AssetObject);

            go.transform.localPosition = Vector3.zero;
            go.transform.localScale    = Vector3.one;
            return go;
        }
        
        public static void LoadGameObjectAsync(string location, Action<GameObject> callback,
            Transform parentTransform = null)
        {
            AssetOperationHandle handle = YooAssets.LoadAssetAsync<GameObject>(location);

            handle.Completed += operation =>
            {
                if (!operation.IsValid)
                {
                    throw new Exception($"[YooAssetShim] Failed to load asset: {location}");
                }

                _OBJ_2_HANDLES.TryAdd(operation.AssetObject, operation);

                if (Object.Instantiate(operation.AssetObject, parentTransform) is not GameObject go)
                {
                    ReleaseAsset(operation.AssetObject);
                    throw new Exception($"[YooAssetShim] Failed to instantiate asset: {location}");
                }

                _GO_2_OBJ.Add(go, operation.AssetObject);

                go.transform.localPosition = Vector3.zero;
                go.transform.localScale    = Vector3.one;
                callback?.Invoke(go);
            };
        }
        
        public static GameObject LoadGameObjectSync(string location, Transform parentTransform = null)
        {
            var handle = YooAssets.LoadAssetSync<GameObject>(location);

            if (!handle.IsValid)
            {
                throw new Exception($"[YooAssetShim] Failed to load asset: {location}");
            }

            _OBJ_2_HANDLES.TryAdd(handle.AssetObject, handle);

            if (Object.Instantiate(handle.AssetObject, parentTransform) is not GameObject go)
            {
                ReleaseAsset(handle.AssetObject);
                throw new Exception($"[YooAssetShim] Failed to instantiate asset: {location}");
            }

            _GO_2_OBJ.Add(go, handle.AssetObject);

            go.transform.localPosition = Vector3.zero;
            go.transform.localScale    = Vector3.one;
            return go;
        }

        public static void ReleaseGameObject(GameObject go)
        {
            if (go is null)
            {
                return;
            }

            Object.Destroy(go);

            _GO_2_OBJ.Remove(go, out Object obj);

            ReleaseAsset(obj);
        }

        public static async UniTask<int> LoadSceneAsync(string location, IProgress<float> progress, LoadSceneMode sceneMode = LoadSceneMode.Single, bool activateOnLoad = true)
        {
            var handle = YooAssets.LoadSceneAsync(location, sceneMode, activateOnLoad);

            await handle.ToUniTask(progress);
            
            if (!handle.IsValid)
            {
                throw new Exception($"[YooAssetShim] Failed to load scene: {location}");
            }

            int sceneId = _SCENE_ID++;
            
            _SCENEID_2_HANDLES.TryAdd(sceneId, handle);

            return sceneId;
        }

        public static void UnloadSceneAsync(int sceneId)
        {
            if (_SCENEID_2_HANDLES.ContainsKey(sceneId))
            {
                _SCENEID_2_HANDLES.Remove(sceneId, out var handle);
                handle?.UnloadAsync();
            }
        }
    }
}
