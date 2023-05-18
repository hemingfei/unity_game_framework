/****************************************************
*	文件：OpenUIFormSuccessEventArgs.cs
*	作者：何明飞
*	邮箱：hemingfei@outlook.com
*	日期：2023/01/03 15:13:04
*	功能：暂无
*****************************************************/

using GameFramework;
using GameFramework.Event;

namespace CustomGameFramework.Runtime
{
    /// <summary>
    ///     打开UI窗口成功事件
    /// </summary>
    public class OpenUIFormSuccessEventArgs : GameEventArgs
    {
        /// <summary>
        ///     打开UI窗口成功事件编号。
        /// </summary>
        public static readonly int EventId = typeof(OpenUIFormSuccessEventArgs).GetHashCode();

        /// <summary>
        ///     初始化打开UI窗口成功事件的新实例。
        /// </summary>
        public OpenUIFormSuccessEventArgs()
        {
            UiForm = null;
            Objs = null;
        }

        /// <summary>
        ///     获取打开UI窗口成功事件编号。
        /// </summary>
        public override int Id => EventId;

        /// <summary>
        ///     UI
        /// </summary>
        public UIWindowBase UiForm { get; private set; }

        /// <summary>
        ///     参数
        /// </summary>
        public object[] Objs { get; private set; }

        /// <summary>
        ///     创建打开UI窗口成功事件。
        /// </summary>
        /// <param name="uiForm">UI</param>
        /// <param name="objs">参数</param>
        /// <returns>创建的打开UI窗口成功事件。</returns>
        public static OpenUIFormSuccessEventArgs Create(UIWindowBase uiForm, object[] objs)
        {
            var e = ReferencePool.Acquire<OpenUIFormSuccessEventArgs>();
            e.UiForm = uiForm;
            e.Objs = objs;
            return e;
        }

        /// <summary>
        ///     清理打开UI窗口成功事件。
        /// </summary>
        public override void Clear()
        {
            UiForm = null;
            Objs = null;
        }
    }
}