/****************************************************
*	文件：ResourceDownloadSuccessEventArgs.cs
*	作者：hemingfei
*	邮箱：hemingfei@outlook.com
*	日期：2023/05/12 15:42:42
*	功能：暂无
*****************************************************/

using GameFramework;
using GameFramework.Event;

namespace CustomGameFramework.Runtime
{
    /// <summary>
    ///     资源下载成功事件
    /// </summary>
    public class ResourceDownloadSuccessEventArgs : GameEventArgs
    {
        /// <summary>
        ///     资源下载成功事件编号。
        /// </summary>
        public static readonly int EventId = typeof(ResourceDownloadSuccessEventArgs).GetHashCode();

        /// <summary>
        ///     初始化资源下载成功事件的新实例。
        /// </summary>
        public ResourceDownloadSuccessEventArgs()
        {
        }

        /// <summary>
        ///     获取资源下载成功事件编号。
        /// </summary>
        public override int Id => EventId;

        /// <summary>
        ///     创建资源下载成功事件。
        /// </summary>
        /// <returns>创建的资源下载成功事件。</returns>
        public static ResourceDownloadSuccessEventArgs Create()
        {
            var e = ReferencePool.Acquire<ResourceDownloadSuccessEventArgs>();
            return e;
        }

        /// <summary>
        ///     清理资源下载成功事件。
        /// </summary>
        public override void Clear()
        {
        }
    }
}