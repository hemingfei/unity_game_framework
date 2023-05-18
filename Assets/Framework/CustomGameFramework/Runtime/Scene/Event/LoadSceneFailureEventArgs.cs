/****************************************************
*	文件：LoadSceneFailureEventArgs.cs
*	作者：hemingfei
*	邮箱：hemingfei@outlook.com
*	日期：2023/05/11 17:05:06
*	功能：暂无
*****************************************************/

using GameFramework;
using GameFramework.Event;

namespace CustomGameFramework.Runtime.Event
{
    /// <summary>
    ///     加载场景失败事件
    /// </summary>
    public class LoadSceneFailureEventArgs : GameEventArgs
    {
        /// <summary>
        ///     加载场景失败事件编号。
        /// </summary>
        public static readonly int EventId = typeof(LoadSceneFailureEventArgs).GetHashCode();

        /// <summary>
        ///     初始化加载场景失败事件的新实例。
        /// </summary>
        public LoadSceneFailureEventArgs()
        {
            ErrorMessage = string.Empty;
        }

        /// <summary>
        ///     获取加载场景失败事件编号。
        /// </summary>
        public override int Id => EventId;

        /// <summary>
        ///     错误信息
        /// </summary>
        public string ErrorMessage { get; private set; }

        /// <summary>
        ///     创建加载场景失败事件。
        /// </summary>
        /// <param name="errorMessage">错误信息</param>
        /// <returns>创建的加载场景失败事件。</returns>
        public static LoadSceneFailureEventArgs Create(string errorMessage)
        {
            var e = ReferencePool.Acquire<LoadSceneFailureEventArgs>();
            e.ErrorMessage = errorMessage;
            return e;
        }

        /// <summary>
        ///     清理加载场景失败事件。
        /// </summary>
        public override void Clear()
        {
            ErrorMessage = string.Empty;
        }
    }
}