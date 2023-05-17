/****************************************************
*	文件：CustomWebRequestFailureEventArgs.cs
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
    /// Web 请求失败事件。
    /// </summary>
    public sealed class CustomWebRequestFailureEventArgs : GameEventArgs
    {
        /// <summary>
        /// Web 请求失败事件编号。
        /// </summary>
        public int EventId { get; private set; }

        /// <summary>
        /// 初始化 Web 请求失败事件的新实例。
        /// </summary>
        public CustomWebRequestFailureEventArgs()
        {
            UID = default;
            SerialId = 0;
            WebRequestUri = null;
            ErrorMessage = null;
            UserData = null;
        }

        /// <summary>
        /// 获取 Web 请求失败事件编号。
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
        /// 获取错误信息。
        /// </summary>
        public string ErrorMessage
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
        /// 创建 Web 请求失败事件。
        /// </summary>
        /// <param name="e">内部事件。</param>
        /// <returns>创建的 Web 请求失败事件。</returns>
        public static CustomWebRequestFailureEventArgs Create(UnityGameFramework.Runtime.WebRequestFailureEventArgs e)
        {
            CustomWebRequestInfo requestInfo = (CustomWebRequestInfo)e.UserData;
            CustomWebRequestFailureEventArgs customWebRequestFailureEventArgs = ReferencePool.Acquire<CustomWebRequestFailureEventArgs>();
            customWebRequestFailureEventArgs.SerialId = e.SerialId;
            customWebRequestFailureEventArgs.WebRequestUri = e.WebRequestUri;
            customWebRequestFailureEventArgs.ErrorMessage = e.ErrorMessage;
            customWebRequestFailureEventArgs.UserData = requestInfo;
            customWebRequestFailureEventArgs.UID = requestInfo.UID;
            customWebRequestFailureEventArgs.EventId = requestInfo.EventId_Failure;
            ReferencePool.Release(requestInfo);
            return customWebRequestFailureEventArgs;
        }

        /// <summary>
        /// 清理 Web 请求失败事件。
        /// </summary>
        public override void Clear()
        {
            UID = default;
            SerialId = 0;
            WebRequestUri = null;
            ErrorMessage = null;
            UserData = null;
        }
    }
}

