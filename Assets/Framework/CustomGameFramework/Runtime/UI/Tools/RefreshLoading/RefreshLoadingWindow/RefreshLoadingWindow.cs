/****************************************************
*	文件：RefreshWindow.cs
*	作者：何明飞
*	邮箱：hemingfei@outlook.com
*	日期：2023/01/09 09:19:04
*	功能：暂无
*****************************************************/

using System;
using CustomGameFramework.Runtime;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace CustomGameFramework.Runtime
{
	/// <summary>
	/// 等待
	/// </summary>
	public partial class RefreshLoadingWindow : UIAnimWindowBase //  /*使用 UIAnimView 动画组件 则继承 UIAnimWindowBase, 不使用动画则继承 UIWindowBase*/ 
	{
        #region 变量
        
        private string _tip;
        private bool _needBg;
        private Tweener _loadingTweener;
        
        #endregion

		#region 内置方法

        #region 流海屏适配
        /// <summary>
        /// 是否自动流海屏适配
        /// </summary>
        public override bool IsAutoFixNotchScreen
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// 流海屏适配后向上的偏移。填入大于0的值
        /// </summary>
        public override float OffsetIfNotchScreen
        {
            get
            {
                return 0;
            }
        }
        #endregion

		#region UI重载方法

		//UI第一次创建
		public override void OnUIInit()
		{
			base.OnUIInit();
			GetObservedComponents();
			GetCustomComponents();
			InitializeValues();
		}

		//UI打开
		public override void OnOpen()
		{
			base.OnOpen();
			ResetValues();
			SubscribeEvents();
		}

		//UI关闭
		public override void OnClose()
		{
            base.OnClose();
			UnSubscribeEvents();
			_loadingTweener.Kill();
		}

		// 动画进入完成
		public override void OnCompleteEnterAnim()
        {
        }

		// 动画退出完成
		public override void OnCompleteExitAnim()
        {
        }

		/*
		// 打开和ShowUI时调用
	    public override void OnShow()
        {
        }

		// 关闭和HideUI时调用
        public override void OnHide()
        {
        }

		//请在这里写UI的更新逻辑，当该UI监听的事件触发时，该函数会被调用
		public override void OnRefresh()
		{
		}

		*/

		#endregion

		#region 常用方法

		// 初始化变量
		private void InitializeValues()
		{

		}

		// 重置变量
		private void ResetValues()
		{
			_tip = string.Empty;
			if (m_OpenUIParams.Length > 0)
			{
				_tip = (string)m_OpenUIParams[0];
			}
			if (m_OpenUIParams.Length > 1)
			{
				_needBg = (bool)m_OpenUIParams[1];
			}

			m_Txt_loading.text = _tip;
			if (_needBg)
			{
				m_Img_bg.gameObject.SetActive(true);
			}
			else
			{
				m_Img_bg.gameObject.SetActive(false);
			}
			
			// m_Img_loading.transform.localRotation = Quaternion.identity;
			// _loadingTweener = m_Img_loading.transform.DOLocalRotate(Vector3.back * 360, 1f, RotateMode.LocalAxisAdd)
			// 	.SetLoops(-1).SetEase(Ease.Linear);
		}

		// 获取自定义组件
		private void GetCustomComponents()
		{

		}

		// 注册事件
		private void SubscribeEvents()
		{
			SubscribeObservedBtnEvents(); // 注册自动绑定的Btn的点击事件

		}
		
        // 取消注册事件
        private void UnSubscribeEvents()
        {
            UnSubscribeObservedBtnEvents(); // 取消注册自动绑定的Btn的点击事件

        }

		/// <summary>
		/// 关闭自己窗口
		/// </summary>
		public void CloseSelfWinodw()
        {
			UIManager.CloseUIWindow<RefreshLoadingWindow>(this);
        }

		#endregion

		#endregion

		#region 自写方法

		#region 公共方法 (public function)

		public void UpdateRefreshTip(string tip)
		{
			_tip = tip;
			m_Txt_loading.text = tip;
		}

		#endregion

		#region 私有方法 (private function)
		private void OnRefreshFinish()
		{
			throw new NotImplementedException();
		}

		#endregion

		#endregion

	}
}

