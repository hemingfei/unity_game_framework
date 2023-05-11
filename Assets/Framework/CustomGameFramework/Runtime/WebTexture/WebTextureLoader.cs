/****************************************************
*	文件：WebTextureLoader.cs
*	作者：何明飞
*	邮箱：hemingfei@outlook.com
*	日期：2023/01/12 19:29:34
*	功能：网络图加载器
*****************************************************/

using System;
using GameFramework;
using GameFramework.WebRequest;
using UnityEngine;
using UnityGameFramework.Runtime;
using WebRequestFailureEventArgs = GameFramework.WebRequest.WebRequestFailureEventArgs;
using WebRequestSuccessEventArgs = GameFramework.WebRequest.WebRequestSuccessEventArgs;

namespace CustomGameFramework.Runtime
{
    /// <summary>
    /// 网络图加载器
    /// </summary>
    public class WebTextureLoader : IReference
    {
        public WebTextureLoader()
        {
            LoadAction = null;
            Url = string.Empty;
            WebRequestManager = null;
        }

        private string Url { get; set; }
        private Action<string, bool, Texture> LoadAction { get; set; }
        private IWebRequestManager WebRequestManager { get; set; }
        private WebTextureComponent WebTextureComponent { get; set; }
        private WebRequestComponent WebRequestComponent { get; set; }

        public static void Create(string url, Action<string, bool, Texture> loadAction)
        {
            WebTextureLoader e = ReferencePool.Acquire<WebTextureLoader>();
            e.LoadAction = loadAction;
            e.Url = url;
            e.StartLoadWebTexture();
        }

        public void Clear()
        {
            LoadAction = null;
            Url = string.Empty;
        }

        public void StartLoadWebTexture()
        {
            WebRequestManager = GameFrameworkEntry.GetModule<IWebRequestManager>();

            WebTextureComponent = UnityGameFramework.Runtime.GameEntry.GetComponent<WebTextureComponent>();
            
            WebRequestComponent = UnityGameFramework.Runtime.GameEntry.GetComponent<WebRequestComponent>();

            SubscribeEvents();
            WebRequestComponent.AddWebRequest(Url);
        }

        private void FinishLoadWebTexture()
        {
            UnsubscribeEvents();
            
            ReferencePool.Release(this);//回收
        }

        private void SubscribeEvents()
        {
            WebRequestManager.WebRequestSuccess += OnWebRequestSuccess;
            WebRequestManager.WebRequestFailure += OnWebRequestFailure;
        }

        private void UnsubscribeEvents()
        {
            WebRequestManager.WebRequestSuccess -= OnWebRequestSuccess;
            WebRequestManager.WebRequestFailure -= OnWebRequestFailure;
        }

        private void OnWebRequestSuccess(object sender, WebRequestSuccessEventArgs args)
        {
            var url = args.WebRequestUri;
            if (url == Url)
            {
                if (!WebTextureComponent.CacheTextures.ContainsKey(Url))
                {
                    var texture = new Texture2D(100, 100);
                    texture.LoadImage(args.GetWebResponseBytes());
                    WebTextureComponent.CacheTextures.Add(Url, texture);
                    LoadAction?.Invoke(url, true, texture);
                    if (WebTextureComponent.GettingTextures.TryGetValue(url, out var gettingTexures))
                    {
                        var count = gettingTexures.Count;
                        for (int i = 0; i < count; i++)
                        {
                            gettingTexures.Dequeue()?.Invoke(url, true, texture);
                        }

                        WebTextureComponent.GettingTextures.Remove(url);
                    }
                }
                else
                {
                    var texture = WebTextureComponent.CacheTextures[Url];
                    LoadAction?.Invoke(url, true, texture);
                }
                FinishLoadWebTexture();
            }
        }

        private void OnWebRequestFailure(object sender, WebRequestFailureEventArgs args)
        {
            var url = args.WebRequestUri;
            if (url == Url)
            {
                LoadAction?.Invoke(url, false, null);
                if (WebTextureComponent.GettingTextures.TryGetValue(url, out var gettingTexures))
                {
                    var count = gettingTexures.Count;
                    for (int i = 0; i < count; i++)
                    {
                        gettingTexures.Dequeue()?.Invoke(url, false, null);
                    }

                    WebTextureComponent.GettingTextures.Remove(url);
                }
                FinishLoadWebTexture();
            }
        }
    }
}