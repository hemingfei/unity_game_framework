/****************************************************
*	文件：ResourceInitSuccessEventArgs.cs
*	作者：hemingfei
*	邮箱：hemingfei@outlook.com
*	日期：2023/05/12 15:34:08
*	功能：暂无
*****************************************************/

using GameFramework;
using GameFramework.Event;

namespace CustomGameFramework.Runtime
{
    /// <summary>
    ///     资源初始化成功事件
    /// </summary>
    public class ResourceInitSuccessEventArgs : GameEventArgs
    {
        /// <summary>
        ///     资源初始化成功事件编号。
        /// </summary>
        public static readonly int EventId = typeof(ResourceInitSuccessEventArgs).GetHashCode();

        /// <summary>
        ///     初始化资源初始化成功事件的新实例。
        /// </summary>
        public ResourceInitSuccessEventArgs()
        {
        }

        /// <summary>
        ///     获取资源初始化成功事件编号。
        /// </summary>
        public override int Id => EventId;

        /// <summary>
        ///     创建资源初始化成功事件。
        /// </summary>
        /// <returns>创建的资源初始化成功事件。</returns>
        public static ResourceInitSuccessEventArgs Create()
        {
            var e = ReferencePool.Acquire<ResourceInitSuccessEventArgs>();
            return e;
        }

        /// <summary>
        ///     清理资源初始化成功事件。
        /// </summary>
        public override void Clear()
        {
        }
    }
}