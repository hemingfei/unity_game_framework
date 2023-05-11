/****************************************************
*	文件：PopupWindow.cs
*	作者：何明飞
*	邮箱：hemingfei@outlook.com
*	日期：2023/01/07 20:58:03
*	功能：暂无
*****************************************************/

using System;
using CustomGameFramework.Runtime;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace CustomGameFramework.Runtime
{
	/// <summary>
	/// 弹框
	/// </summary>
	public partial class PopupWindow : UIAnimWindowBase //  /*使用 UIAnimView 动画组件 则继承 UIAnimWindowBase, 不使用动画则继承 UIWindowBase*/ 
	{
		#region 变量

        private Text _content;
        private Text _textNo;
        private Text _textOk;
        private Text _title;
        private GameObject _titlePart;
        private GameObject _contentPart;
        private UnityEngine.UI.Button _buttonNo;
        private UnityEngine.UI.Button _buttonOk;

        private PopupStyle _showType = PopupStyle.Default;

        private Action<bool> _completed
        {
	        get;
	        set;
        }
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
			//GetObservedComponents();
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
			_title.text = (string)m_OpenUIParams[0];
			_content.text = (string)m_OpenUIParams[1];
			_completed = (Action<bool>)m_OpenUIParams[2];
			_showType = (PopupStyle)m_OpenUIParams[3];
			_textOk.text = (string)m_OpenUIParams[4];
			_textNo.text = (string)m_OpenUIParams[5];
			UpdateShowStyle();
			Log.Debug("Dialog, title: {0}, content: {1}, style: {2}", _title.text, _content.text, _showType);
		}

		// 获取自定义组件
		private void GetCustomComponents()
		{
			_title = GetComponentFromPath<Text>("root/Title/Text");
			_titlePart = GetComponentFromPath<Transform>("root/Title").gameObject;
			_content = GetComponentFromPath<Text>("root/Content/Text");
			_textOk = GetComponentFromPath<Text>("root/Buttons/Ok/Text");
			_textNo = GetComponentFromPath<Text>("root/Buttons/No/Text");
			_contentPart = GetComponentFromPath<Transform>("root/Content").gameObject;
			_buttonOk = GetComponentFromPath<UnityEngine.UI.Button>("root/Buttons/Ok");
			_buttonNo = GetComponentFromPath<UnityEngine.UI.Button>("root/Buttons/No");
		}

		// 注册事件
		private void SubscribeEvents()
		{
			//SubscribeObservedBtnEvents(); // 注册自动绑定的Btn的点击事件
			_buttonOk.onClick.AddListener(OnClickOk);
			_buttonNo.onClick.AddListener(OnClickNo);
		}
		
        // 取消注册事件
        private void UnSubscribeEvents()
        {
            //UnSubscribeObservedBtnEvents(); // 取消注册自动绑定的Btn的点击事件
            _buttonOk.onClick.RemoveListener(OnClickOk);
            _buttonNo.onClick.RemoveListener(OnClickNo);
        }

		/// <summary>
		/// 关闭自己窗口
		/// </summary>
		public void CloseSelfWinodw()
        {
			UIManager.CloseUIWindow<PopupWindow>(this);
        }

		#endregion

		#endregion

		#region 自写方法

		private T GetComponentFromPath<T>(string path) where T : Component
		{
			var trans = gameObject.transform.Find(path);
			return trans.GetComponent<T>();
		}
        
		private void OnClickNo()
		{
			HandleEvent(false);
		}
        
		private void OnClickOk()
		{
			HandleEvent(true);
		}
        
		private void HandleEvent(bool isOk)
		{
			CloseSelfWinodw();
            
			if (_completed == null) { return; }
            
			_completed(isOk);
			_completed = null;
		}

		
		private void UpdateShowStyle()
		{
			if (CheckEnable(PopupStyle.ShowTitle))
			{
				_titlePart.SetActive(true);
			}
			else
			{
				_titlePart.SetActive(false); 
			}

			if (CheckEnable(PopupStyle.ShowContent))
			{
				_contentPart.SetActive(true);
			}
			else
			{
				_contentPart.SetActive(false);
			}

			if (CheckEnable(PopupStyle.ShowOkNo))
			{
				_buttonOk.gameObject.SetActive(true);
				_buttonNo.gameObject.SetActive(true);
			}
			else
			{
				if (CheckEnable(PopupStyle.ShowOk))
				{
					_buttonOk.gameObject.SetActive(true);
				}
				else
				{
					_buttonOk.gameObject.SetActive(false);
				}
				_buttonNo.gameObject.SetActive(false);
			}
		}

		private bool CheckEnable(PopupStyle mask)
		{
			return CheckBit(_showType, mask);
		}

		private bool CheckBit(PopupStyle target, PopupStyle mask)
		{
			return CheckBit((int)target, (int)mask);
		}

		private bool CheckBit(int target, int mask)
		{
			if (((target ^ mask) & mask) == 0)
			{
				return true;
			}
			return false;
		}
		#endregion

	}
}

