﻿//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GameFramework;
using GameFramework.Download;
using GameFramework.FileSystem;
using GameFramework.ObjectPool;
using GameFramework.Resource;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityGameFramework.Runtime;
using Object = UnityEngine.Object;

namespace CustomGameFramework.Runtime
{
    /// <summary>
    /// 资源组件。
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("Game Framework/Resource")]
    [RequireComponent(typeof(FrameworkResourceComponent))]
    public sealed class ResourceComponent : GameFrameworkComponent
    {
        [SerializeField]
        private string m_ResourceHelperTypeName = "CustomGameFramework.Runtime.ResourceHelperBase";

        [SerializeField]
        private ResourceHelperBase m_CustomResourceHelper = null;

        private IResourceHelper m_ResourceHelper = null;

        private EventComponent Event;

        private void Start()
        {
            Event = GameEntry.GetComponent<EventComponent>();
            
            ResourceHelperBase resourceHelper = Helper.CreateHelper(m_ResourceHelperTypeName, m_CustomResourceHelper);
            if (resourceHelper == null)
            {
                Log.Error("Can not create resource helper.");
                return;
            }

            resourceHelper.name = "Resource Helper";
            Transform transform = resourceHelper.transform;
            transform.SetParent(this.transform);
            transform.localScale = Vector3.one;

            m_ResourceHelper = resourceHelper;
        }

        public void Init(IResourceMode mode)
        {
            m_ResourceHelper.Init(mode,
                () => Event.Fire(this, ResourceInitSuccessEventArgs.Create()),
                (msg) => Event.Fire(this, ResourceInitFailEventArgs.Create(msg)));
        }

        public void UpdateVersionAndManifest()
        {
            m_ResourceHelper.UpdateVersionAndManifest(
                () => Event.Fire(this, ResourceUpdateVersionAndManifestSuccessEventArgs.Create()),
                (msg) => Event.Fire(this, ResourceUpdateVersionAndManifestFailEventArgs.Create(msg)));
        }

        public long GetDownloadSize()
        {
            return m_ResourceHelper.GetDownloadSize();
        }

        public void StartDownload()
        {
            var progressReporter = ProgressReporter.Create((progress) =>
            {
                Event.Fire(this, ResourceUpdateVersionAndManifestProgressEventArgs.Create(progress));
            });
            m_ResourceHelper.StartDownload(
                () =>
                {
                    Event.Fire(this, ResourceDownloadSuccessEventArgs.Create());
                    ReferencePool.Release(progressReporter);
                },
                (msg) =>
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
            m_ResourceHelper.LoadAssetAsync<T>(location, callback);
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

        public void UnloadSceneAsync(int sceneId)
        {
            m_ResourceHelper.UnloadSceneAsync(sceneId);
        }
        
        #endregion
        
        #region 框架内部使用的加载卸载方法
        
        // gf框架内部使用的加载资源
        internal async void LoadAsset(string assetName, int priority, LoadAssetCallbacks loadAssetCallbacks, object userData)
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
