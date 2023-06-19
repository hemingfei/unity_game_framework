/****************************************************
*	文件：YooAssetResourceHelper.cs
*	作者：hemingfei
*	邮箱：hemingfei@outlook.com
*	日期：2023/05/10 10:24:03
*	功能：暂无
*****************************************************/

using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using YooAsset;
using Object = UnityEngine.Object;

namespace CustomGameFramework.Runtime
{
    public abstract class YooAssetResourceHelperBase : ResourceHelperBase
    {
        private ResourcePackage _currentPackage;
        private bool _isInitPackage = false;
        public abstract string GetUrl();
        public abstract IQueryServices GetQueryServices();
        public abstract IDecryptionServices GetDecryptionServices();

        public virtual string GetDefaultPackageName()
        {
            return "DefaultPackage";
        }

        public string GetCurrentPackageName()
        {
            return _currentPackage?.PackageName;
        }

        public ResourcePackage GetCurrentPackage()
        {
            return _currentPackage;
        }

        public override void UnloadUnusedAssets()
        {
            _currentPackage?.UnloadUnusedAssets();
        }

        public override void InitResourceSystem()
        {
            YooAssetShim.InitializeYooAsset();
        }

        public override void DestroyResourceSystem()
        {
            YooAssetShim.DestroyYooAsset();
        }

        public override async void InitPackage(IResourceMode mode, Action success, Action<string> fail)
        {
            await InitializeHelperPackage(GetDefaultPackageName(), mode, success, fail, true);
        }

        public override void DestroyPackage()
        {
            DestroyHelperPackage();
        }

        /// <summary>
        /// 初始化特定的资源包，一个helper只能操控一个package
        /// </summary>
        public async UniTask<bool> InitializeHelperPackage(string packageName, IResourceMode mode, Action success,
            Action<string> fail, bool isSetDefaultPackage = false)
        {
            _currentPackage = YooAssetShim.GetPackage(packageName, isSetDefaultPackage);
            var emode = EPlayMode.EditorSimulateMode;
            switch (mode)
            {
                case IResourceMode.HostPlayMode:
                    emode = EPlayMode.HostPlayMode;
                    break;
                case IResourceMode.OfflinePlayMode:
                    emode = EPlayMode.OfflinePlayMode;
                    break;
                default:
                    emode = EPlayMode.EditorSimulateMode;
                    break;
            }

            try
            {
                var initializeOperation = await YooAssetShim.InitializePackageAsync(_currentPackage, emode, GetUrl(),
                    GetQueryServices(), GetDecryptionServices());
                if (initializeOperation.Status == EOperationStatus.Succeed)
                {
                    success();
                    _isInitPackage = true;
                    return true;
                }
                else
                {
                    fail("");
                    _isInitPackage = false;
                    return false;
                }
            }
            catch (Exception e)
            {
                fail(e.Message);
                return false;
            }
        }

        public void DestroyHelperPackage()
        {
            if (_currentPackage != null)
            {
                string packageName = _currentPackage.PackageName;
                YooAssetShim.DestroyPackage(packageName);
            }

            _currentPackage = null;
            _isInitPackage = false;
        }

        public override async void UpdateVersionAndManifest(Action success, Action<string> fail)
        {
            if (!_isInitPackage)
            {
                fail?.Invoke("package not initialized");
                return;
            }
            try
            {
                var version = await YooAssetShim.UpdateStaticVersion(_currentPackage);

                var isSuccess = await YooAssetShim.UpdateManifest(_currentPackage, version);

                if (isSuccess)
                    success();
                else
                    fail("");
            }
            catch (Exception e)
            {
                fail(e.Message);
            }
        }

        public override long GetDownloadSize()
        {
            if (!_isInitPackage)
            {
                return 0;
            }
            return YooAssetShim.GetDownloadSize(_currentPackage);
        }

        public override async void StartDownload(Action success, Action<string> fail, IProgress<float> progress = null)
        {
            if (!_isInitPackage)
            {
                fail?.Invoke("package not initialized");
                return;
            }
            try
            {
                var isSuccess = await YooAssetShim.Download(_currentPackage, progress);
                if (isSuccess)
                {
                    await YooAssetShim.ClearCache(_currentPackage);
                    success();
                }
                else
                {
                    fail("");
                }
            }
            catch (Exception e)
            {
                fail(e.Message);
            }
        }

        public override async UniTask<T> LoadAssetAsync<T>(string location)
        {
            return await YooAssetShim.LoadAssetAsync<T>(_currentPackage, location);
        }

        public override void LoadAssetAsync<T>(string location, Action<T> callback)
        {
            YooAssetShim.LoadAssetAsync(_currentPackage, location, callback);
        }

        public override T LoadAssetSync<T>(string location)
        {
            return YooAssetShim.LoadAssetSync<T>(_currentPackage, location);
        }

        public override void ReleaseAsset(Object obj)
        {
            YooAssetShim.ReleaseAsset(obj);
        }

        public override async UniTask<GameObject> LoadGameObjectAsync(string location, Transform parentTransform = null)
        {
            return await YooAssetShim.LoadGameObjectAsync(_currentPackage, location, parentTransform);
        }

        public override void LoadGameObjectAsync(string location, Action<GameObject> callback,
            Transform parentTransform = null)
        {
            YooAssetShim.LoadGameObjectAsync(_currentPackage, location, callback, parentTransform);
        }

        public override GameObject LoadGameObjectSync(string location, Transform parentTransform = null)
        {
            return YooAssetShim.LoadGameObjectSync(_currentPackage, location, parentTransform);
        }

        public override void ReleaseGameObject(GameObject go)
        {
            YooAssetShim.ReleaseGameObject(go);
        }

        public override async UniTask<int> LoadSceneAsync(string location, IProgress<float> progress,
            LoadSceneMode sceneMode = LoadSceneMode.Single, bool activateOnLoad = true)
        {
            return await YooAssetShim.LoadSceneAsync(_currentPackage, location, progress, sceneMode, activateOnLoad);
        }

        public override void LoadSceneAsync(string location, IProgress<float> progress, Action<int> callback,
            LoadSceneMode sceneMode = LoadSceneMode.Single, bool activateOnLoad = true)
        {
            YooAssetShim.LoadSceneAsync(_currentPackage, location, progress, callback, sceneMode, activateOnLoad);
        }

        public override void UnloadSceneAsync(int sceneId)
        {
            YooAssetShim.UnloadSceneAsync(sceneId);
        }
    }
}