using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace CustomGameFramework.Runtime
{
	/// <summary>
	/// 这是关注的字段和获取组件
	/// </summary>
	public partial class RefreshLoadingWindow
	{
		#region 字段 (int,string...)
		private Image m_Img_bg;
		private Image m_Img_loading;
		private Text m_Txt_loading;
		#endregion

		#region Get Componets

		// 获取设定的组件
		private void GetObservedComponents()
		{
			m_Img_bg = GetImage("Img_bg");
			m_Img_loading = GetImage("Img_loading");
			m_Txt_loading = GetText("Txt_loading");
		}
		#endregion

		#region Btn OnClick

		/// <summary>
		/// 注册按钮点击的事件
		/// </summary>
		private void SubscribeObservedBtnEvents()
		{
			
		}
		
        /// <summary>
        /// 取消注册按钮点击的事件
        /// </summary>
        private void UnSubscribeObservedBtnEvents()
        {
            
        }
		#endregion

		#region Btn Func Demo

		/*

		*/
		#endregion

	}
}
