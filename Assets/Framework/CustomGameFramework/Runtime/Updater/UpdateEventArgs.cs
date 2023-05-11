/****************************************************
*	文件：UpdateEventArgs.cs
*	作者：何明飞
*	邮箱：hemingfei@outlook.com
*	日期：2023/02/06 14:27:15
*	功能：暂无
*****************************************************/

using GameFramework;

namespace CustomGameFramework
{
    /// <summary>
    /// Description of UpdateEventArgs
    /// </summary>
    public class UpdateEventArgs : GameFrameworkEventArgs
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="deltaTime">逻辑流逝时间，以秒为单位。</param>
        /// <param name="unscaledDeltaTime">真实流逝时间，以秒为单位。</param>
        public UpdateEventArgs(float deltaTime, float unscaledDeltaTime)
        {
            DeltaTime = deltaTime;
            UnscaledDeltaTime = unscaledDeltaTime;
        }
        
        /// <summary>
        /// 逻辑流逝时间，以秒为单位。
        /// </summary>
        public float DeltaTime
        {
            get;
            set;
        }
        
        /// <summary>
        /// 真实流逝时间，以秒为单位。
        /// </summary>
        public float UnscaledDeltaTime
        {
            get;
            set;
        }
        
        /// <summary>
        /// 清理事件
        /// </summary>
        public override void Clear()
        {
            DeltaTime = default;
            UnscaledDeltaTime = default;
        }
    }
}

