/****************************************************
*	文件：IResourceHelper.cs
*	作者：hemingfei
*	邮箱：hemingfei@outlook.com
*	日期：2023/05/10 09:55:27
*	功能：暂无
*****************************************************/

using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace CustomGameFramework.Runtime
{
    public interface IResourceHelper
    {
        public void Init(IResourceMode mode, Action success, Action<string> fail);

        public void UpdateVersionAndManifest(Action success, Action<string> fail);

        public long GetDownloadSize();

        public void StartDownload(Action success, Action<string> fail, IProgress<float> progress = null);

        public void UnloadUnusedAssets();

        public UniTask<T> LoadAssetAsync<T>(string location) where T : Object;

        public void LoadAssetAsync<T>(string location, Action<T> callback) where T : Object;

        public T LoadAssetSync<T>(string location) where T : Object;

        public void ReleaseAsset(Object obj);

        public UniTask<GameObject> LoadGameObjectAsync(string location, Transform parentTransform = null);

        public void LoadGameObjectAsync(string location, Action<GameObject> callback, Transform parentTransform = null);

        public GameObject LoadGameObjectSync(string location, Transform parentTransform = null);

        public void ReleaseGameObject(GameObject go);

        public UniTask<int> LoadSceneAsync(string location, IProgress<float> progress,
            LoadSceneMode sceneMode = LoadSceneMode.Single, bool activateOnLoad = true);

        public void LoadSceneAsync(string location, IProgress<float> progress, Action<int> callback,
            LoadSceneMode sceneMode = LoadSceneMode.Single, bool activateOnLoad = true);

        public void UnloadSceneAsync(int sceneId);
    }
}