/****************************************************
*	文件：HttpWebRequestAgentHelper.cs
*	作者：何明飞
*	邮箱：hemingfei@outlook.com
*	日期：2023/02/07 16:07:41
*	功能：暂无
*****************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using GameFramework;
using GameFramework.WebRequest;
using UnityGameFramework.Runtime;
using Utility = GameFramework.Utility;
#if UNITY_5_4_OR_NEWER
using UnityEngine.Networking;

#else
using UnityEngine.Experimental.Networking;
#endif

namespace CustomGameFramework.Runtime
{
    /// <summary>
    ///     Description of HttpWebRequestAgentHelper
    /// </summary>
    public class HttpWebRequestAgentHelper : WebRequestAgentHelperBase, IDisposable
    {
        private bool m_Disposed;
        private UnityWebRequest m_UnityWebRequest;

        private EventHandler<WebRequestAgentHelperCompleteEventArgs> m_WebRequestAgentHelperCompleteEventHandler;
        private EventHandler<WebRequestAgentHelperErrorEventArgs> m_WebRequestAgentHelperErrorEventHandler;

        /// <summary>
        ///     重置 Web 请求代理辅助器。
        /// </summary>
        public override void Reset()
        {
            if (m_UnityWebRequest != null)
            {
                m_UnityWebRequest.Dispose();
                m_UnityWebRequest = null;
            }
        }

        private void Update()
        {
            if (m_UnityWebRequest == null || !m_UnityWebRequest.isDone) return;

            var isError = false;
#if UNITY_2020_2_OR_NEWER
            isError = m_UnityWebRequest.result != UnityWebRequest.Result.Success;
#elif UNITY_2017_1_OR_NEWER
            isError = m_UnityWebRequest.isNetworkError || m_UnityWebRequest.isHttpError;
#else
            isError = m_UnityWebRequest.isError;
#endif
            if (isError)
            {
                var webRequestAgentHelperErrorEventArgs =
                    WebRequestAgentHelperErrorEventArgs.Create(m_UnityWebRequest.error);
                m_WebRequestAgentHelperErrorEventHandler(this, webRequestAgentHelperErrorEventArgs);
                ReferencePool.Release(webRequestAgentHelperErrorEventArgs);
            }
            else if (m_UnityWebRequest.downloadHandler.isDone)
            {
                var webRequestAgentHelperCompleteEventArgs =
                    WebRequestAgentHelperCompleteEventArgs.Create(m_UnityWebRequest.downloadHandler.data);
                m_WebRequestAgentHelperCompleteEventHandler(this, webRequestAgentHelperCompleteEventArgs);
                ReferencePool.Release(webRequestAgentHelperCompleteEventArgs);
            }
        }

        /// <summary>
        ///     释放资源。
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Web 请求代理辅助器完成事件。
        /// </summary>
        public override event EventHandler<WebRequestAgentHelperCompleteEventArgs> WebRequestAgentHelperComplete
        {
            add => m_WebRequestAgentHelperCompleteEventHandler += value;
            remove => m_WebRequestAgentHelperCompleteEventHandler -= value;
        }

        /// <summary>
        ///     Web 请求代理辅助器错误事件。
        /// </summary>
        public override event EventHandler<WebRequestAgentHelperErrorEventArgs> WebRequestAgentHelperError
        {
            add => m_WebRequestAgentHelperErrorEventHandler += value;
            remove => m_WebRequestAgentHelperErrorEventHandler -= value;
        }

        /// <summary>
        ///     通过 Web 请求代理辅助器发送请求。
        /// </summary>
        /// <param name="webRequestUri">要发送的远程地址。</param>
        /// <param name="userData">用户自定义数据。</param>
        public override void Request(string webRequestUri, object userData)
        {
            if (m_WebRequestAgentHelperCompleteEventHandler == null || m_WebRequestAgentHelperErrorEventHandler == null)
            {
                Log.Fatal("Web request agent helper handler is invalid.");
                return;
            }

            var wwwFormInfo = (WWWFormInfo)userData;
            if (wwwFormInfo == null || wwwFormInfo.UserData == null ||
                wwwFormInfo.UserData.GetType() != typeof(HttpWebRequestInfo))
            {
                if (wwwFormInfo == null || wwwFormInfo.WWWForm == null)
                    m_UnityWebRequest = UnityWebRequest.Get(webRequestUri);
                else
                    m_UnityWebRequest = UnityWebRequest.Post(webRequestUri, wwwFormInfo.WWWForm);
            }
            else
            {
                var info = (HttpWebRequestInfo)wwwFormInfo.UserData;
                if (info.Query != null) webRequestUri += "?" + GetQueryPath(info.Query);

                if (info.HttpType == HttpWebRequestInfo.HTTPType.POST)
                {
                    m_UnityWebRequest = new UnityWebRequest(webRequestUri, UnityWebRequest.kHttpVerbPOST);
                    m_UnityWebRequest.downloadHandler = new DownloadHandlerBuffer();
                    var bytes = Encoding.UTF8.GetBytes(info.BodyJson);
                    m_UnityWebRequest.uploadHandler = new UploadHandlerRaw(bytes);
                    m_UnityWebRequest.uploadHandler.contentType = "application/x-www-form-urlencoded";
                }
                else
                {
                    m_UnityWebRequest = UnityWebRequest.Get(webRequestUri);
                }

                if (info.Headers != null)
                    foreach (var header in info.Headers)
                        if (!string.IsNullOrEmpty(header.Value))
                            m_UnityWebRequest.SetRequestHeader(header.Key, header.Value);
            }

#if UNITY_2017_2_OR_NEWER
            m_UnityWebRequest.SendWebRequest();
#else
            m_UnityWebRequest.Send();
#endif
        }

        /// <summary>
        ///     通过 Web 请求代理辅助器发送请求。
        /// </summary>
        /// <param name="webRequestUri">要发送的远程地址。</param>
        /// <param name="postData">要发送的数据流。</param>
        /// <param name="userData">用户自定义数据。</param>
        public override void Request(string webRequestUri, byte[] postData, object userData)
        {
            if (m_WebRequestAgentHelperCompleteEventHandler == null || m_WebRequestAgentHelperErrorEventHandler == null)
            {
                Log.Fatal("Web request agent helper handler is invalid.");
                return;
            }

            var wwwFormInfo = (WWWFormInfo)userData;
            HttpWebRequestInfo info = null;
            if (wwwFormInfo.UserData.GetType() != typeof(HttpWebRequestInfo))
            {
#if UNITY_2022_3_OR_NEWER
                m_UnityWebRequest = UnityWebRequest.PostWwwForm(webRequestUri, Utility.Converter.GetString(postData));
#else
                m_UnityWebRequest = UnityWebRequest.Post(webRequestUri, Utility.Converter.GetString(postData)); 
#endif
            }
            else
            {
                info = (HttpWebRequestInfo)wwwFormInfo.UserData;
                if (info.Query != null) webRequestUri += "?" + GetQueryPath(info.Query);

                if (info.HttpType == HttpWebRequestInfo.HTTPType.POST)
                {
                    m_UnityWebRequest = new UnityWebRequest(webRequestUri, UnityWebRequest.kHttpVerbPOST);
                    m_UnityWebRequest.downloadHandler = new DownloadHandlerBuffer();
                    var bytes = Encoding.UTF8.GetBytes(info.BodyJson);
                    m_UnityWebRequest.uploadHandler = new UploadHandlerRaw(bytes);
                    m_UnityWebRequest.uploadHandler.contentType = "application/x-www-form-urlencoded";
                }
                else
                {
                    m_UnityWebRequest = UnityWebRequest.Get(webRequestUri);
                }

                if (info.Headers != null)
                    foreach (var header in info.Headers)
                        if (!string.IsNullOrEmpty(header.Value))
                            m_UnityWebRequest.SetRequestHeader(header.Key, header.Value);
            }

#if UNITY_2017_2_OR_NEWER
            m_UnityWebRequest.SendWebRequest();
#else
            m_UnityWebRequest.Send();
#endif
        }

        /// <summary>
        ///     释放资源。
        /// </summary>
        /// <param name="disposing">释放资源标记。</param>
        protected virtual void Dispose(bool disposing)
        {
            if (m_Disposed) return;

            if (disposing)
                if (m_UnityWebRequest != null)
                {
                    m_UnityWebRequest.Dispose();
                    m_UnityWebRequest = null;
                }

            m_Disposed = true;
        }

        // 获取网址参数
        private string GetQueryPath(SortedDictionary<string, string> query)
        {
            var sb = new StringBuilder();
            foreach (var item in query)
            {
                if (sb.Length > 0) sb.Append("&");
                sb.Append(item.Key);
                sb.Append("=");
                sb.Append(item.Value);
            }

            return sb.ToString();
        }
    }
}