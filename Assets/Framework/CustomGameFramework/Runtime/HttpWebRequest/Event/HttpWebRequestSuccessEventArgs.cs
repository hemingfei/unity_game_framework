/****************************************************
*	文件：HttpWebRequestSuccessEventArgs.cs
*	作者：何明飞
*	邮箱：hemingfei@outlook.com
*	日期：2023/02/08 19:02:10
*	功能：暂无
*****************************************************/

using GameFramework;
using GameFramework.Event;
using UnityGameFramework.Runtime;

namespace CustomGameFramework.Runtime
{
    /// <summary>
    ///     Web 请求成功事件。
    /// </summary>
    public sealed class HttpWebRequestSuccessEventArgs : GameEventArgs
    {
        private byte[] m_WebResponseBytes;

        /// <summary>
        ///     初始化 Web 请求成功事件的新实例。
        /// </summary>
        public HttpWebRequestSuccessEventArgs()
        {
            UID = default;
            SerialId = 0;
            WebRequestUri = null;
            m_WebResponseBytes = null;
            UserData = null;
        }

        /// <summary>
        ///     Web 请求成功事件编号。
        /// </summary>
        public int EventId { get; private set; }

        /// <summary>
        ///     获取 Web 请求成功事件编号。
        /// </summary>
        public override int Id => EventId;

        /// <summary>
        ///     获取用户自定义数据的UID。
        /// </summary>
        public string UID { get; private set; }

        /// <summary>
        ///     获取 Web 请求任务的序列编号。
        /// </summary>
        public int SerialId { get; private set; }

        /// <summary>
        ///     获取 Web 请求地址。
        /// </summary>
        public string WebRequestUri { get; private set; }

        /// <summary>
        ///     获取用户自定义数据。
        /// </summary>
        public object UserData { get; private set; }

        /// <summary>
        ///     获取 Web 响应的数据流。
        /// </summary>
        /// <returns>Web 响应的数据流。</returns>
        public byte[] GetWebResponseBytes()
        {
            return m_WebResponseBytes;
        }

        /// <summary>
        ///     创建 Web 请求成功事件。
        /// </summary>
        /// <param name="e">内部事件。</param>
        /// <returns>创建的 Web 请求成功事件。</returns>
        public static HttpWebRequestSuccessEventArgs Create(WebRequestSuccessEventArgs e)
        {
            var requestInfo = (HttpWebRequestInfo)e.UserData;
            var httpWebRequestSuccessEventArgs = ReferencePool.Acquire<HttpWebRequestSuccessEventArgs>();
            httpWebRequestSuccessEventArgs.SerialId = e.SerialId;
            httpWebRequestSuccessEventArgs.WebRequestUri = e.WebRequestUri;
            httpWebRequestSuccessEventArgs.m_WebResponseBytes = e.GetWebResponseBytes();
            httpWebRequestSuccessEventArgs.UserData = requestInfo;
            httpWebRequestSuccessEventArgs.UID = requestInfo.UID;
            httpWebRequestSuccessEventArgs.EventId = requestInfo.EventId_Success;
            ReferencePool.Release(requestInfo);
            return httpWebRequestSuccessEventArgs;
        }

        /// <summary>
        ///     清理 Web 请求成功事件。
        /// </summary>
        public override void Clear()
        {
            UID = default;
            SerialId = 0;
            WebRequestUri = null;
            m_WebResponseBytes = null;
            UserData = null;
        }
    }
}