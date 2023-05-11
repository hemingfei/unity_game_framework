/****************************************************
*	文件：LoadSceneUpdateEventArgs.cs
*	作者：hemingfei
*	邮箱：hemingfei@outlook.com
*	日期：2023/05/11 17:05:06
*	功能：暂无
*****************************************************/

/****************************************************
*	文件：LoadSceneUpdateEventArgs.cs
*	作者：何明飞
*	邮箱：hemingfei@outlook.com
*	日期：2023/02/02 11:28:52
*	功能：暂无
*****************************************************/

using GameFramework;
using GameFramework.Event;

namespace CustomGameFramework.Runtime.Event
{
	/// <summary>
	/// 加载场景更新事件
	/// </summary>
	public class LoadSceneUpdateEventArgs : GameEventArgs
	{
		/// <summary>
		/// 加载场景更新事件编号。
		/// </summary>
		public static readonly int EventId = typeof(LoadSceneUpdateEventArgs).GetHashCode();

		/// <summary>
		/// 初始化加载场景更新事件的新实例。
		/// </summary>
		public LoadSceneUpdateEventArgs()
		{
			Progress = 0;
		}

		/// <summary>
		/// 获取加载场景更新事件编号。
		/// </summary>
		public override int Id => EventId;

		/// <summary>
		/// 进度
		/// </summary>
		public float Progress { get; private set; }
	
		/// <summary>
		/// 创建加载场景更新事件。
		/// </summary>
		/// <param name="progress">进度</param>
		/// <returns>创建的加载场景更新事件。</returns>
		public static LoadSceneUpdateEventArgs Create(float progress)
		{
			LoadSceneUpdateEventArgs e = ReferencePool.Acquire<LoadSceneUpdateEventArgs>();
			e.Progress = progress;
			return e;
		}

		/// <summary>
		/// 清理加载场景更新事件。
		/// </summary>
		public override void Clear()
		{
			Progress = 0;
		}
	}
}


