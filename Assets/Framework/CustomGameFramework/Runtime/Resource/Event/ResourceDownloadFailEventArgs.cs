/****************************************************
*	文件：ResourceDownloadFailEventArgs.cs
*	作者：hemingfei
*	邮箱：hemingfei@outlook.com
*	日期：2023/05/12 15:43:22
*	功能：暂无
*****************************************************/

using GameFramework;
using GameFramework.Event;

namespace CustomGameFramework.Runtime
{
	/// <summary>
	/// 资源下载失败事件
	/// </summary>
	public class ResourceDownloadFailEventArgs : GameEventArgs
	{
		/// <summary>
		/// 资源下载失败事件编号。
		/// </summary>
		public static readonly int EventId = typeof(ResourceDownloadFailEventArgs).GetHashCode();

		/// <summary>
		/// 初始化资源下载失败事件的新实例。
		/// </summary>
		public ResourceDownloadFailEventArgs()
		{
			ErrorMsg = default;
		}

		/// <summary>
		/// 获取资源下载失败事件编号。
		/// </summary>
		public override int Id => EventId;

		/// <summary>
		/// 错误信息
		/// </summary>
		public string ErrorMsg { get; private set; }
	
		/// <summary>
		/// 创建资源下载失败事件。
		/// </summary>
		/// <param name="errorMsg">错误信息</param>
		/// <returns>创建的资源下载失败事件。</returns>
		public static ResourceDownloadFailEventArgs Create(string errorMsg)
		{
			ResourceDownloadFailEventArgs e = ReferencePool.Acquire<ResourceDownloadFailEventArgs>();
			e.ErrorMsg = errorMsg;
			return e;
		}

		/// <summary>
		/// 清理资源下载失败事件。
		/// </summary>
		public override void Clear()
		{
			ErrorMsg = default;
		}
	}
}

