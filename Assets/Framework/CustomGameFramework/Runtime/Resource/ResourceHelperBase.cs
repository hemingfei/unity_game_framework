/****************************************************
*	文件：ResourceHelperBase.cs
*	作者：hemingfei
*	邮箱：hemingfei@outlook.com
*	日期：2023/05/10 09:55:03
*	功能：暂无
*****************************************************/

using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace CustomGameFramework.Runtime
{
    public abstract class ResourceHelperBase : MonoBehaviour, IResourceHelper
    {
        public abstract void InitResourceSystem();
        
        public abstract void DestroyResourceSystem();
        
        public abstract void InitPackage(IResourceMode mode, Action success, Action<string> fail);

        public abstract void DestroyPackage();
        
        public abstract void UpdateVersionAndManifest(Action success, Action<string> fail);

        public abstract long GetDownloadSize();

        public abstract void StartDownload(Action success, Action<string> fail, IProgress<float> progress = null);

        public abstract void UnloadUnusedAssets();

        public abstract UniTask<T> LoadAssetAsync<T>(string location) where T : Object;

        public abstract void LoadAssetAsync<T>(string location, Action<T> callback) where T : Object;

        public abstract T LoadAssetSync<T>(string location) where T : Object;

        public abstract void ReleaseAsset(Object obj);

        public abstract UniTask<GameObject> LoadGameObjectAsync(string location, Transform parentTransform = null);

        public abstract void LoadGameObjectAsync(string location, Action<GameObject> callback,
            Transform parentTransform = null);

        public abstract GameObject LoadGameObjectSync(string location, Transform parentTransform = null);

        public abstract void ReleaseGameObject(GameObject go);

        public abstract UniTask<int> LoadSceneAsync(string location, IProgress<float> progress,
            LoadSceneMode sceneMode = LoadSceneMode.Single, bool activateOnLoad = true);

        public abstract void LoadSceneAsync(string location, IProgress<float> progress, Action<int> callback,
            LoadSceneMode sceneMode = LoadSceneMode.Single, bool activateOnLoad = true);

        public abstract void UnloadSceneAsync(int sceneId);
    }
}