//------------------------------------------------------------
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
        
        private void Start()
        {
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
            YooAssetShim.ReleaseGameObject(go);
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
