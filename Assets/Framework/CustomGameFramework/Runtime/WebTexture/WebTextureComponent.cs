/****************************************************
*	文件：WebTextureComponent.cs
*	作者：何明飞
*	邮箱：hemingfei@outlook.com
*	日期：2023/01/12 17:37:12
*	功能：获取网络图片组件
*****************************************************/

using System;
using System.Collections.Generic;
using GameFramework;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace CustomGameFramework.Runtime
{
    /// <summary>
    ///     获取网络图片，加载的是 Texture，最好使用rawImage赋值结果性能高
    /// </summary>
    public class WebTextureComponent : GameFrameworkComponent, IReference
    {
        public readonly Dictionary<string, Texture> CacheTextures = new();

        public readonly Dictionary<string, Queue<Action<string, bool, Texture>>> GettingTextures = new();

        /// <summary>
        ///     将一些静态变量重置等操作，真正销毁请调用 OnDestroy
        /// </summary>
        private void OnDestroy()
        {
            Clear();
        }

        /// <summary>
        ///     清除所有缓存
        /// </summary>
        public void Clear()
        {
            foreach (var texture in CacheTextures.Values) Destroy(texture);
            CacheTextures.Clear();
        }

        /// <summary>
        ///     加载网络上的texture
        /// </summary>
        /// <param name="url">网址</param>
        /// <param name="onLoadFinish">成功失败回调， 参数分别为 图片地址，是否成功，Texture内容</param>
        public void Get(string url, Action<string, bool, Texture> onLoadFinish)
        {
            if (string.IsNullOrEmpty(url))
            {
                onLoadFinish?.Invoke(url, false, null);
                return;
            }

            if (CacheTextures.TryGetValue(url, out var cachedTexture))
            {
                onLoadFinish?.Invoke(url, true, cachedTexture);
            }
            else
            {
                if (GettingTextures.TryGetValue(url, out var gettingTextures))
                {
                    if (!gettingTextures.Contains(onLoadFinish)) gettingTextures.Enqueue(onLoadFinish);
                }
                else
                {
                    GettingTextures.Add(url, new Queue<Action<string, bool, Texture>>());
                    WebTextureLoader.Create(url, onLoadFinish);
                }
            }
        }

        /// <summary>
        ///     立刻获取缓存里的texture，但是如果没有缓存的话会返回null
        /// </summary>
        /// <param name="url"></param>
        /// <returns>如果缓存中没有会返回null</returns>
        public Texture GetSync(string url)
        {
            if (string.IsNullOrEmpty(url)) return null;
            if (CacheTextures.TryGetValue(url, out var cachedTexture))
                return cachedTexture;
            return null;
        }

        /// <summary>
        ///     清除某个url的texture缓存
        /// </summary>
        /// <param name="url">网址</param>
        public void Clear(string url)
        {
            if (CacheTextures.ContainsKey(url))
            {
                var texture = CacheTextures[url];
                Destroy(texture);
                CacheTextures.Remove(url);
            }
        }
    }
}