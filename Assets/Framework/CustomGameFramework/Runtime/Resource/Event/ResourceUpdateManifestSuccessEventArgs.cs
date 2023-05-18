/****************************************************
*	文件：ResourceUpdateManifestSuccessEventArgs.cs
*	作者：hemingfei
*	邮箱：hemingfei@outlook.com
*	日期：2023/05/12 15:37:37
*	功能：暂无
*****************************************************/

using GameFramework;
using GameFramework.Event;

namespace CustomGameFramework.Runtime
{
	/// <summary>
	///     更新版本清单成功事件
	/// </summary>
	public class ResourceUpdateManifestSuccessEventArgs : GameEventArgs
    {
	    /// <summary>
	    ///     更新版本清单成功事件编号。
	    /// </summary>
	    public static readonly int EventId = typeof(ResourceUpdateManifestSuccessEventArgs).GetHashCode();

	    /// <summary>
	    ///     初始化更新版本清单成功事件的新实例。
	    /// </summary>
	    public ResourceUpdateManifestSuccessEventArgs()
        {
        }

	    /// <summary>
	    ///     获取更新版本清单成功事件编号。
	    /// </summary>
	    public override int Id => EventId;

	    /// <summary>
	    ///     创建更新版本清单成功事件。
	    /// </summary>
	    /// <returns>创建的更新版本清单成功事件。</returns>
	    public static ResourceUpdateManifestSuccessEventArgs Create()
        {
            var e = ReferencePool.Acquire<ResourceUpdateManifestSuccessEventArgs>();
            return e;
        }

	    /// <summary>
	    ///     清理更新版本清单成功事件。
	    /// </summary>
	    public override void Clear()
        {
        }
    }
}