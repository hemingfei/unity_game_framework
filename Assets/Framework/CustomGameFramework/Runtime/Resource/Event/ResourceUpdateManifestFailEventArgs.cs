/****************************************************
*	文件：ResourceUpdateManifestFailEventArgs.cs
*	作者：hemingfei
*	邮箱：hemingfei@outlook.com
*	日期：2023/05/13 00:13:07
*	功能：暂无
*****************************************************/

using GameFramework;
using GameFramework.Event;

namespace CustomGameFramework.Runtime
{
    /// <summary>
    ///     更新资源版本清单失败事件
    /// </summary>
    public class ResourceUpdateManifestFailEventArgs : GameEventArgs
    {
        /// <summary>
        ///     更新资源版本清单失败事件编号。
        /// </summary>
        public static readonly int EventId = typeof(ResourceUpdateManifestFailEventArgs).GetHashCode();

        /// <summary>
        ///     初始化更新资源版本清单失败事件的新实例。
        /// </summary>
        public ResourceUpdateManifestFailEventArgs()
        {
            ErrorMsg = default;
        }

        /// <summary>
        ///     获取更新资源版本清单失败事件编号。
        /// </summary>
        public override int Id => EventId;

        /// <summary>
        ///     错误信息
        /// </summary>
        public string ErrorMsg { get; private set; }

        /// <summary>
        ///     创建更新资源版本清单失败事件。
        /// </summary>
        /// <param name="errorMsg">错误信息</param>
        /// <returns>创建的更新资源版本清单失败事件。</returns>
        public static ResourceUpdateManifestFailEventArgs Create(string errorMsg)
        {
            var e = ReferencePool.Acquire<ResourceUpdateManifestFailEventArgs>();
            e.ErrorMsg = errorMsg;
            return e;
        }

        /// <summary>
        ///     清理更新资源版本清单失败事件。
        /// </summary>
        public override void Clear()
        {
            ErrorMsg = default;
        }
    }
}