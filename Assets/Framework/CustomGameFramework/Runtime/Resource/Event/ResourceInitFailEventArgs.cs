using GameFramework;
using GameFramework.Event;

namespace CustomGameFramework.Runtime
{
    /// <summary>
    ///     资源初始化失败事件
    /// </summary>
    public class ResourceInitFailEventArgs : GameEventArgs
    {
        /// <summary>
        ///     资源初始化失败事件编号。
        /// </summary>
        public static readonly int EventId = typeof(ResourceInitFailEventArgs).GetHashCode();

        /// <summary>
        ///     初始化资源初始化失败事件的新实例。
        /// </summary>
        public ResourceInitFailEventArgs()
        {
            ErrorMsg = default;
        }

        /// <summary>
        ///     获取资源初始化失败事件编号。
        /// </summary>
        public override int Id => EventId;

        /// <summary>
        ///     错误信息
        /// </summary>
        public string ErrorMsg { get; private set; }

        /// <summary>
        ///     创建资源初始化失败事件。
        /// </summary>
        /// <param name="errorMsg">错误信息</param>
        /// <returns>创建的资源初始化失败事件。</returns>
        public static ResourceInitFailEventArgs Create(string errorMsg)
        {
            var e = ReferencePool.Acquire<ResourceInitFailEventArgs>();
            e.ErrorMsg = errorMsg;
            return e;
        }

        /// <summary>
        ///     清理资源初始化失败事件。
        /// </summary>
        public override void Clear()
        {
            ErrorMsg = default;
        }
    }
}