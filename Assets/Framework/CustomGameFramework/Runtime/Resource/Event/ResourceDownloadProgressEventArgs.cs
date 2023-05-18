/****************************************************
*	文件：ResourceDownloadProgressEventArgs.cs
*	作者：hemingfei
*	邮箱：hemingfei@outlook.com
*	日期：2023/05/12 16:17:41
*	功能：暂无
*****************************************************/

using GameFramework;
using GameFramework.Event;

namespace CustomGameFramework.Runtime
{
    /// <summary>
    ///     资源更新版本清单的进度事件
    /// </summary>
    public class ResourceDownloadProgressEventArgs : GameEventArgs
    {
        /// <summary>
        ///     资源更新版本清单的进度事件编号。
        /// </summary>
        public static readonly int EventId = typeof(ResourceDownloadProgressEventArgs).GetHashCode();

        /// <summary>
        ///     初始化资源更新版本清单的进度事件的新实例。
        /// </summary>
        public ResourceDownloadProgressEventArgs()
        {
            Progress = default;
        }

        /// <summary>
        ///     获取资源更新版本清单的进度事件编号。
        /// </summary>
        public override int Id => EventId;

        /// <summary>
        ///     进度
        /// </summary>
        public float Progress { get; private set; }

        /// <summary>
        ///     创建资源更新版本清单的进度事件。
        /// </summary>
        /// <param name="progress">进度</param>
        /// <returns>创建的资源更新版本清单的进度事件。</returns>
        public static ResourceDownloadProgressEventArgs Create(float progress)
        {
            var e = ReferencePool.Acquire<ResourceDownloadProgressEventArgs>();
            e.Progress = progress;
            return e;
        }

        /// <summary>
        ///     清理资源更新版本清单的进度事件。
        /// </summary>
        public override void Clear()
        {
            Progress = default;
        }
    }
}