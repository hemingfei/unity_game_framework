/****************************************************
*	文件：CloseUIFormCompleteEventArgs.cs
*	作者：何明飞
*	邮箱：hemingfei@outlook.com
*	日期：2023/01/03 15:45:11
*	功能：暂无
*****************************************************/

using GameFramework;
using GameFramework.Event;

namespace CustomGameFramework.Runtime
{
    /// <summary>
    ///     关闭UI完成事件
    /// </summary>
    public class CloseUIFormCompleteEventArgs : GameEventArgs
    {
        /// <summary>
        ///     关闭UI完成事件编号。
        /// </summary>
        public static readonly int EventId = typeof(CloseUIFormCompleteEventArgs).GetHashCode();

        /// <summary>
        ///     初始化关闭UI完成事件的新实例。
        /// </summary>
        public CloseUIFormCompleteEventArgs()
        {
            UiName = string.Empty;
            Objs = null;
        }

        /// <summary>
        ///     获取关闭UI完成事件编号。
        /// </summary>
        public override int Id => EventId;

        /// <summary>
        ///     UI名称
        /// </summary>
        public string UiName { get; private set; }

        /// <summary>
        ///     参数
        /// </summary>
        public object[] Objs { get; private set; }

        /// <summary>
        ///     创建关闭UI完成事件。
        /// </summary>
        /// <param name="uiName">UI名称</param>
        /// <param name="objs">参数</param>
        /// <returns>创建的关闭UI完成事件。</returns>
        public static CloseUIFormCompleteEventArgs Create(string uiName, object[] objs)
        {
            var e = ReferencePool.Acquire<CloseUIFormCompleteEventArgs>();
            e.UiName = uiName;
            e.Objs = objs;
            return e;
        }

        /// <summary>
        ///     清理关闭UI完成事件。
        /// </summary>
        public override void Clear()
        {
            UiName = string.Empty;
            Objs = null;
        }
    }
}