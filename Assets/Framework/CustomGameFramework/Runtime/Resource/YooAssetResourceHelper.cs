/****************************************************
*	文件：YooAssetResourceHelper.cs
*	作者：hemingfei
*	邮箱：hemingfei@outlook.com
*	日期：2023/05/10 10:24:03
*	功能：暂无
*****************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace CustomGameFramework.Runtime
{
    public class YooAssetResourceHelper : ResourceHelperBase
    {
        public override async UniTask<T> LoadAssetAsync<T>(string location)
        {
            return await YooAssetShim.LoadAssetAsync<T>(location);
        }

        public override void LoadAssetAsync<T>(string location, Action<T> callback)
        {
            YooAssetShim.LoadAssetAsync<T>(location, callback);
        }

        public override T LoadAssetSync<T>(string location)
        {
            return YooAssetShim.LoadAssetSync<T>(location);
        }

        public override void ReleaseAsset(Object obj)
        {
            YooAssetShim.ReleaseAsset(obj);
        }

        public override async UniTask<GameObject> LoadGameObjectAsync(string location, Transform parentTransform = null)
        {
            return await YooAssetShim.LoadGameObjectAsync(location, parentTransform);
        }

        public override void LoadGameObjectAsync(string location, Action<GameObject> callback, Transform parentTransform = null)
        {
            YooAssetShim.LoadGameObjectAsync(location, callback, parentTransform);
        }

        public override GameObject LoadGameObjectSync(string location, Transform parentTransform = null)
        {
            return YooAssetShim.LoadGameObjectSync(location, parentTransform);
        }

        public override void ReleaseGameObject(GameObject go)
        {
            YooAssetShim.ReleaseGameObject(go);
        }

        public override async UniTask<int> LoadSceneAsync(string location, IProgress<float> progress,
            LoadSceneMode sceneMode = LoadSceneMode.Single, bool activateOnLoad = true)
        {
            return await YooAssetShim.LoadSceneAsync(location, progress, sceneMode, activateOnLoad);
        }

        public override void UnloadSceneAsync(int sceneId)
        {
            YooAssetShim.UnloadSceneAsync(sceneId);
        }
    }
}

