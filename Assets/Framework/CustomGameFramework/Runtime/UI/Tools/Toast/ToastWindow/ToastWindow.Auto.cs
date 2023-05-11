/****************************************************
*	文件：ToastWindow.Auto.cs
*	作者：hemingfei
*	邮箱：hemingfei@outlook.com
*	日期：2023/03/22 10:57:20
*	功能：暂无
*****************************************************/

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace CustomGameFramework.Runtime
{
	/// <summary>
	/// 这是关注的字段和获取组件
	/// </summary>
	public partial class ToastWindow
	{
		#region 字段 (int,string...)
		private GameObject m_Go_Toast;
		private UnityEngine.UI.Button m_Btn_Toast;
		private Image m_Img_Bg;
		private RawImage m_RawImg_pic;
		private Text m_Txt_Msg;
		#endregion

		#region Get Componets

		// 获取设定的组件
		private void GetObservedComponents()
		{
			m_Go_Toast = GetGameObject("Go_Btn_Toast");
			m_Btn_Toast = GetButton("Go_Btn_Toast");
			m_Img_Bg = GetImage("Img_Bg");
			m_RawImg_pic = GetRawImage("RawImg_pic");
			m_Txt_Msg = GetText("Txt_Msg");
		}
		#endregion

		#region Btn OnClick

		/// <summary>
		/// 注册按钮点击的事件
		/// </summary>
		private void SubscribeObservedBtnEvents()
		{
			m_Btn_Toast.onClick.AddListener(On_Btn_Toast_Clicked);
		
		}
		
        /// <summary>
        /// 取消注册按钮点击的事件
        /// </summary>
        private void UnSubscribeObservedBtnEvents()
        {
            m_Btn_Toast.onClick.RemoveListener(On_Btn_Toast_Clicked);
		
        }
		#endregion

		#region Btn Func Demo

		/*
		/// <summary>
		/// Btn_Toast 点击事件
		/// </summary>
		private void On_Btn_Toast_Clicked()
		{
		}

		*/
		#endregion

	}
}

