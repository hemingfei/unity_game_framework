//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using System;
using Cysharp.Threading.Tasks;
using GameFramework;
using GameFramework.Resource;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityGameFramework.Runtime;
using Object = UnityEngine.Object;

namespace CustomGameFramework.Runtime
{
    /// <summary>
    ///     资源组件。
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("Game Framework/Resource")]
    [RequireComponent(typeof(FrameworkResourceComponent))]
    public sealed class ResourceComponent : GameFrameworkComponent
    {
        [SerializeField] private IResourceMode m_ResourceMode = IResourceMode.EditorSimulateMode;

        [SerializeField] private string m_ResourceHelperTypeName = "CustomGameFramework.Runtime.ResourceHelperBase";

        [SerializeField] private ResourceHelperBase m_CustomResourceHelper;

        private EventComponent Event;

        private IResourceHelper m_ResourceHelper;

        private void Start()
        {
            Event = GameEntry.GetComponent<EventComponent>();

            var resourceHelper = Helper.CreateHelper(m_ResourceHelperTypeName, m_CustomResourceHelper);
            if (resourceHelper == null)
            {
                Log.Error("Can not create resource helper.");
                return;
            }

            resourceHelper.name = "Resource Helper";
            var transform = resourceHelper.transform;
            transform.SetParent(this.transform);
            transform.localScale = Vector3.one;

            m_ResourceHelper = resourceHelper;
        }

        public void Init()
        {
            var mode = GetRuntimeResourceMode();
            m_ResourceHelper.Init(mode,
                () => Event.Fire(this, ResourceInitSuccessEventArgs.Create()),
                msg => Event.Fire(this, ResourceInitFailEventArgs.Create(msg)));
        }

        private IResourceMode GetRuntimeResourceMode()
        {
            var mode = IResourceMode.HostPlayMode;
            if (m_ResourceMode != IResourceMode.EditorSimulateMode)
                mode = m_ResourceMode;
            else
                mode = Application.platform is RuntimePlatform.OSXEditor or RuntimePlatform.LinuxEditor
                    or RuntimePlatform.WindowsEditor
                    ? m_ResourceMode
                    : IResourceMode.OfflinePlayMode;

            return mode;
        }

        public void UpdateVersionAndManifest()
        {
            m_ResourceHelper.UpdateVersionAndManifest(
                () => Event.Fire(this, ResourceUpdateManifestSuccessEventArgs.Create()),
                msg => Event.Fire(this, ResourceUpdateManifestFailEventArgs.Create(msg)));
        }

        public long GetDownloadSize()
        {
            return m_ResourceHelper.GetDownloadSize();
        }

        public void StartDownload()
        {
            var progressReporter = ProgressReporter.Create(progress =>
            {
                Event.Fire(this, ResourceDownloadProgressEventArgs.Create(progress));
            });
            m_ResourceHelper.StartDownload(
                () =>
                {
                    Event.Fire(this, ResourceDownloadSuccessEventArgs.Create());
                    ReferencePool.Release(progressReporter);
                },
                msg =>
                {
                    Event.Fire(this, ResourceDownloadFailEventArgs.Create(msg));
                    ReferencePool.Release(progressReporter);
                },
                progressReporter);
        }

        public void UnloadUnusedAssets()
        {
            m_ResourceHelper.UnloadUnusedAssets();
        }

        #region 加载资源对象

        public async UniTask<T> LoadAssetAsync<T>(string location) where T : Object
        {
            return await m_ResourceHelper.LoadAssetAsync<T>(location);
        }

        public void LoadAssetAsync<T>(string location, Action<T> callback) where T : Object
        {
            m_ResourceHelper.LoadAssetAsync(location, callback);
        }

        public T LoadAssetSync<T>(string location) where T : Object
        {
            return m_ResourceHelper.LoadAssetSync<T>(location);
        }

        public void ReleaseAsset(Object obj)
        {
            m_ResourceHelper.ReleaseAsset(obj);
        }

        #endregion

        #region 加载 gameobject

        public async UniTask<GameObject> LoadGameObjectAsync(string location, Transform parentTransform = null)
        {
            return await m_ResourceHelper.LoadGameObjectAsync(location, parentTransform);
        }
        
        public void LoadGameObjectAsync(string location, Action<GameObject> callback, Transform parentTransform = null)
        {
            m_ResourceHelper.LoadGameObjectAsync(location, callback, parentTransform);
        }

        public GameObject LoadGameObjectSync(string location, Transform parentTransform = null)
        {
            return m_ResourceHelper.LoadGameObjectSync(location, parentTransform);
        }

        public void ReleaseGameObject(GameObject go)
        {
            m_ResourceHelper.ReleaseGameObject(go);
        }

        #endregion

        #region 加载场景

        public async UniTask<int> LoadSceneAsync(string location, IProgress<float> progress,
            LoadSceneMode sceneMode = LoadSceneMode.Single, bool activateOnLoad = true)
        {
            return await m_ResourceHelper.LoadSceneAsync(location, progress, sceneMode, activateOnLoad);
        }

        public void LoadSceneAsync(string location, IProgress<float> progress, Action<int> callback,
            LoadSceneMode sceneMode = LoadSceneMode.Single, bool activateOnLoad = true)
        {
            m_ResourceHelper.LoadSceneAsync(location, progress, callback, sceneMode, activateOnLoad);
        }

        public void UnloadSceneAsync(int sceneId)
        {
            m_ResourceHelper.UnloadSceneAsync(sceneId);
        }

        #endregion

        #region 框架内部使用的加载卸载方法

        // gf框架内部使用的加载资源
        internal async void LoadAsset(string assetName, int priority, LoadAssetCallbacks loadAssetCallbacks,
            object userData)
        {
            try
            {
                var asset = await LoadAssetAsync<Object>(assetName);
                loadAssetCallbacks.LoadAssetSuccessCallback?.Invoke(assetName, asset, 0f, userData);
            }
            catch (Exception e)
            {
                Log.Error(e);
                loadAssetCallbacks.LoadAssetFailureCallback?.Invoke(assetName, LoadResourceStatus.AssetError, e.Message,
                    userData);
            }
        }

        // gf框架内部使用的卸载资源
        internal void UnloadAsset(object asset)
        {
            ReleaseAsset((Object)asset);
        }

        #endregion
    }
}