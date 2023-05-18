/****************************************************
*	文件：LoadSceneSuccessEventArgs.cs
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
    ///     加载场景成功事件
    /// </summary>
    public class LoadSceneSuccessEventArgs : GameEventArgs
    {
        /// <summary>
        ///     加载场景成功事件编号。
        /// </summary>
        public static readonly int EventId = typeof(LoadSceneSuccessEventArgs).GetHashCode();

        /// <summary>
        ///     初始化加载场景成功事件的新实例。
        /// </summary>
        public LoadSceneSuccessEventArgs()
        {
            SceneName = string.Empty;
        }

        /// <summary>
        ///     获取加载场景成功事件编号。
        /// </summary>
        public override int Id => EventId;

        /// <summary>
        ///     场景名称
        /// </summary>
        public string SceneName { get; private set; }

        /// <summary>
        ///     创建加载场景成功事件。
        /// </summary>
        /// <param name="sceneName">场景名称</param>
        /// <returns>创建的加载场景成功事件。</returns>
        public static LoadSceneSuccessEventArgs Create(string sceneName)
        {
            var e = ReferencePool.Acquire<LoadSceneSuccessEventArgs>();
            e.SceneName = sceneName;
            return e;
        }

        /// <summary>
        ///     清理加载场景成功事件。
        /// </summary>
        public override void Clear()
        {
            SceneName = string.Empty;
        }
    }
}