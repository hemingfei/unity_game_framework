/****************************************************
*	文件：CustomWebRequestInfo.cs
*	作者：何明飞
*	邮箱：hemingfei@outlook.com
*	日期：2023/02/07 16:41:52
*	功能：暂无
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using GameFramework;
using UnityEngine;
using UnityGameFramework.Runtime;
using Random = UnityEngine.Random;

namespace CustomGameFramework.Runtime
{
	/// <summary>
	/// Description of CustomWebRequestInfo
	/// </summary>
	public class CustomWebRequestInfo : IReference
	{
		public enum HTTPType
		{
			POST,
			GET,
		}

		/// <summary>
		/// 初始化网络请求数据的新实例。
		/// </summary>
		public CustomWebRequestInfo()
		{
			UID = string.Empty;
			HttpType = HTTPType.GET;
			Headers = null;
			BodyJson = string.Empty;
			Query = null;
			EventId_Start = 0;
			EventId_Success = 0;
			EventId_Failure = 0;
		}

		/// <summary>
		/// UID
		/// </summary>
		public string UID { get; private set; }

		/// <summary>
		/// EventId Start
		/// </summary>
		public int EventId_Start { get; private set; }
		
		/// <summary>
		/// EventId Success
		/// </summary>
		public int EventId_Success { get; private set; }
		
		/// <summary>
		/// EventId Failure
		/// </summary>
		public int EventId_Failure { get; private set; }
		
		/// <summary>
		/// 请求类型
		/// </summary>
		public HTTPType HttpType { get; private set; }

		/// <summary>
		/// Headers的字典数据
		/// </summary>
		public Dictionary<string, string> Headers { get; private set; }

		/// <summary>
		/// Body的Json数据
		/// </summary>
		public string BodyJson { get; private set; }

		/// <summary>
		/// Query数据
		/// </summary>
		public SortedDictionary<string, string> Query { get; private set; }

		/// <summary>
		/// 创建网络请求数据。
		/// </summary>
		/// <param name="httpType">请求类型</param>
		/// <param name="headers">Headers的字典数据</param>
		/// <param name="bodyJson">Body的Json数据</param>
		/// <param name="query">Query数据</param>
		/// <returns>创建的网络请求数据。</returns>
		public static CustomWebRequestInfo Create(string uid, int eventId_start, int eventId_success, int eventId_failure, HTTPType httpType, Dictionary<string, string> headers = null,
			string bodyJson = "", SortedDictionary<string, string> query = null)
		{
			CustomWebRequestInfo e = ReferencePool.Acquire<CustomWebRequestInfo>();
			e.UID = uid;
			e.EventId_Start = eventId_start;
			e.EventId_Success = eventId_success;
			e.EventId_Failure = eventId_failure;
			e.HttpType = httpType;
			e.Headers = headers;
			e.BodyJson = bodyJson;
			e.Query = query;
			return e;
		}

		/// <summary>
		/// 清理网络请求数据。
		/// </summary>
		public void Clear()
		{
			UID = string.Empty;
			HttpType = HTTPType.GET;
			Headers = null;
			BodyJson = string.Empty;
			Query = null;
			EventId_Start = 0;
			EventId_Success = 0;
			EventId_Failure = 0;
		}
	}
}