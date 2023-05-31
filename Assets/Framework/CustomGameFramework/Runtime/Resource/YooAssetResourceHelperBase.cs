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
        private ResourcePackage _defaultPackage;
        public abstract string GetUrl();
        public abstract IQueryServices GetQueryServices();
        public abstract IDecryptionServices GetDecryptionServices();

        public virtual string GetDefaultPackageName()
        {
            return "DefaultPackage";
        }

        public override void UnloadUnusedAssets()
        {
            _defaultPackage?.UnloadUnusedAssets();
        }

        public override async void Init(IResourceMode mode, Action success, Action<string> fail)
        {
            YooAssetShim.InitializeYooAsset();
            _defaultPackage = YooAssetShim.GetPackage(GetDefaultPackageName(), true);
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
                var initializeOperation = await YooAssetShim.InitializePackageAsync(_defaultPackage, emode, GetUrl(), GetQueryServices(), GetDecryptionServices());
                if (initializeOperation.Status == EOperationStatus.Succeed)
                    success();
                else
                    fail("");
            }
            catch (Exception e)
            {
                fail(e.Message);
            }
        }

        public override async void UpdateVersionAndManifest(Action success, Action<string> fail)
        {
            try
            {
                var version = await YooAssetShim.UpdateStaticVersion(_defaultPackage);

                var isSuccess = await YooAssetShim.UpdateManifest(_defaultPackage, version);

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
            return YooAssetShim.GetDownloadSize(_defaultPackage);
        }

        public override async void StartDownload(Action success, Action<string> fail, IProgress<float> progress = null)
        {
            try
            {
                var isSuccess = await YooAssetShim.Download(_defaultPackage, progress);
                if (isSuccess)
                {
                    await YooAssetShim.ClearCache(_defaultPackage);
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
            return await YooAssetShim.LoadAssetAsync<T>(_defaultPackage, location);
        }

        public override void LoadAssetAsync<T>(string location, Action<T> callback)
        {
            YooAssetShim.LoadAssetAsync(_defaultPackage, location, callback);
        }

        public override T LoadAssetSync<T>(string location)
        {
            return YooAssetShim.LoadAssetSync<T>(_defaultPackage, location);
        }

        public override void ReleaseAsset(Object obj)
        {
            YooAssetShim.ReleaseAsset(obj);
        }

        public override async UniTask<GameObject> LoadGameObjectAsync(string location, Transform parentTransform = null)
        {
            return await YooAssetShim.LoadGameObjectAsync(_defaultPackage, location, parentTransform);
        }

        public override void LoadGameObjectAsync(string location, Action<GameObject> callback,
            Transform parentTransform = null)
        {
            YooAssetShim.LoadGameObjectAsync(_defaultPackage, location, callback, parentTransform);
        }

        public override GameObject LoadGameObjectSync(string location, Transform parentTransform = null)
        {
            return YooAssetShim.LoadGameObjectSync(_defaultPackage, location, parentTransform);
        }

        public override void ReleaseGameObject(GameObject go)
        {
            YooAssetShim.ReleaseGameObject(go);
        }

        public override async UniTask<int> LoadSceneAsync(string location, IProgress<float> progress,
            LoadSceneMode sceneMode = LoadSceneMode.Single, bool activateOnLoad = true)
        {
            return await YooAssetShim.LoadSceneAsync(_defaultPackage, location, progress, sceneMode, activateOnLoad);
        }

        public override void LoadSceneAsync(string location, IProgress<float> progress, Action<int> callback,
            LoadSceneMode sceneMode = LoadSceneMode.Single, bool activateOnLoad = true)
        {
            YooAssetShim.LoadSceneAsync(_defaultPackage, location, progress, callback, sceneMode, activateOnLoad);
        }

        public override void UnloadSceneAsync(int sceneId)
        {
            YooAssetShim.UnloadSceneAsync(sceneId);
        }
    }
}