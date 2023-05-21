/****************************************************
*	文件：SceneComponent.cs
*	作者：hemingfei
*	邮箱：hemingfei@outlook.com
*	日期：2023/05/11 17:05:06
*	功能：暂无
*****************************************************/

using System;
using System.Collections.Generic;
using CustomGameFramework.Runtime.Event;
using Cysharp.Threading.Tasks;
using GameFramework;
using UnityEngine.SceneManagement;
using UnityGameFramework.Runtime;

namespace CustomGameFramework.Runtime
{
    public class SceneComponent : GameFrameworkComponent
    {
        private ProgressReporter _currentProgressReporter;
        private bool _isLoadingScene;

        private Dictionary<int, ProgressReporter> _sceneLoadingIdInfos;
        public static EventComponent Event { get; private set; }
        public static ResourceComponent Resource { get; private set; }

        private void Start()
        {
            Event = GameEntry.GetComponent<EventComponent>();
            Resource = GameEntry.GetComponent<ResourceComponent>();
            _sceneLoadingIdInfos = new Dictionary<int, ProgressReporter>();
        }

        private void Update()
        {
            if (_isLoadingScene && _currentProgressReporter != null)
                OnLoadSceneUpdate(_currentProgressReporter.Progress);
        }

        /// <summary>
        ///     加载场景,仅供切换场景流程使用
        /// </summary>
        /// <param name="sceneLocation">场景文件地址</param>
        /// <returns></returns>
        public async void LoadScene(string sceneLocation)
        {
            var sceneMode = LoadSceneMode.Additive;
            var progress = ProgressReporter.Create(null);
            _currentProgressReporter = progress;
            try
            {
                _isLoadingScene = true;
                var sceneId = await Resource.LoadSceneAsync(sceneLocation, progress, sceneMode);
                _sceneLoadingIdInfos.Add(sceneId, progress);
                _isLoadingScene = false;
                OnLoadSceneUpdate(1);
                OnLoadSceneSuccess(sceneLocation);
            }
            catch (Exception e)
            {
                OnLoadSceneFailure(e.Message + "-" + sceneLocation);
                _isLoadingScene = false;
            }
        }

        /// <summary>
        ///     卸载上一个场景,仅供切换场景流程使用
        /// </summary>
        public void UnloadLastScene()
        {
            if (_sceneLoadingIdInfos.Count > 0)
                foreach (var sceneLoaded in _sceneLoadingIdInfos)
                {
                    var sceneId = sceneLoaded.Key;
                    Resource.UnloadSceneAsync(sceneId);
                    var progress = sceneLoaded.Value;
                    ReferencePool.Release(progress);
                }

            _sceneLoadingIdInfos.Clear();
        }

        /// <summary>
        ///     手动加载场景，自己管理。例如 .Completed 监听成功失败等
        /// </summary>
        /// <param name="sceneLocation"></param>
        /// <returns></returns>
        public async UniTask<int> Handle_LoadScene(string sceneLocation, ProgressReporter progress = null)
        {
            return await Handle_LoadScene(sceneLocation, true, progress);
        }
        
        /// <summary>
        ///     手动加载场景，自己管理。例如 .Completed 监听成功失败等
        /// </summary>
        /// <param name="sceneLocation"></param>
        public async void Handle_LoadScene(string sceneLocation, Action<int> callback, ProgressReporter progress = null)
        {
            int sceneId = await Handle_LoadScene(sceneLocation, true, progress);
            callback?.Invoke(sceneId);
        }

        /// <summary>
        ///     手动加载场景，自己管理。例如 .Completed 监听成功失败等
        /// </summary>
        /// <param name="sceneLocation"></param>
        /// <returns></returns>
        public async UniTask<int> Handle_LoadScene(string sceneLocation, bool activateOnLoad,
            ProgressReporter progress = null)
        {
            var sceneMode = LoadSceneMode.Additive;
            var sceneId = await Resource.LoadSceneAsync(sceneLocation, progress, sceneMode, activateOnLoad);
            return sceneId;
        }

        /// <summary>
        ///     手动卸载场景
        /// </summary>
        /// <param name="handle"></param>
        public void Handle_UnloadScene(int sceneId)
        {
            Resource.UnloadSceneAsync(sceneId);
        }

        private void OnLoadSceneSuccess(string sceneName)
        {
            //Log.Debug($"Load scene success, scene name is {sceneName}");
            Event.Fire(this, LoadSceneSuccessEventArgs.Create(sceneName));
        }

        private void OnLoadSceneFailure(string errorMessage)
        {
            //Log.Error("Load scene failure, error message '{0}'.", errorMessage);
            Event.Fire(this, LoadSceneFailureEventArgs.Create(errorMessage));
        }

        private void OnLoadSceneUpdate(float progress)
        {
            //Log.Info("Load scene update, progress '{0}'.", progress.ToString("P2"));
            Event.Fire(this, LoadSceneUpdateEventArgs.Create(progress));
        }
    }
}