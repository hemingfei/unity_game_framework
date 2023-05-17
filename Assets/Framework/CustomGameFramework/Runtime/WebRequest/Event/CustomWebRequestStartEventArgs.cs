/****************************************************
*	文件：CustomWebRequestStartEventArgs.cs
*	作者：何明飞
*	邮箱：hemingfei@outlook.com
*	日期：2023/02/08 19:02:10
*	功能：暂无
*****************************************************/

using GameFramework;
using GameFramework.Event;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace CustomGameFramework.Runtime
{
    /// <summary>
    /// Web 请求开始事件。
    /// </summary>
    public sealed class CustomWebRequestStartEventArgs : GameEventArgs
    {
        /// <summary>
        /// Web 请求开始事件编号。
        /// </summary>
        public int EventId { get; private set; }

        /// <summary>
        /// 初始化 Web 请求开始事件的新实例。
        /// </summary>
        public CustomWebRequestStartEventArgs()
        {
            UID = default;
            SerialId = 0;
            WebRequestUri = null;
            UserData = null;
        }

        /// <summary>
        /// 获取 Web 请求开始事件编号。
        /// </summary>
        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        /// <summary>
        /// 获取用户自定义数据的UID。
        /// </summary>
        public string UID
        {
            get;
            private set;
        }
        
        /// <summary>
        /// 获取 Web 请求任务的序列编号。
        /// </summary>
        public int SerialId
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取 Web 请求地址。
        /// </summary>
        public string WebRequestUri
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取用户自定义数据。
        /// </summary>
        public object UserData
        {
            get;
            private set;
        }

        /// <summary>
        /// 创建 Web 请求开始事件。
        /// </summary>
        /// <param name="e">内部事件。</param>
        /// <returns>创建的 Web 请求开始事件。</returns>
        public static CustomWebRequestStartEventArgs Create(UnityGameFramework.Runtime.WebRequestStartEventArgs e)
        {
            CustomWebRequestInfo requestInfo = (CustomWebRequestInfo)e.UserData;
            CustomWebRequestStartEventArgs customWebRequestStartEventArgs = ReferencePool.Acquire<CustomWebRequestStartEventArgs>();
            customWebRequestStartEventArgs.SerialId = e.SerialId;
            customWebRequestStartEventArgs.WebRequestUri = e.WebRequestUri;
            customWebRequestStartEventArgs.UserData = requestInfo;
            customWebRequestStartEventArgs.UID = requestInfo.UID;
            customWebRequestStartEventArgs.EventId = requestInfo.EventId_Start;
            return customWebRequestStartEventArgs;
        }

        /// <summary>
        /// 清理 Web 请求开始事件。
        /// </summary>
        public override void Clear()
        {
            UID = default;
            SerialId = 0;
            WebRequestUri = null;
            UserData = null;
        }
    }
}

