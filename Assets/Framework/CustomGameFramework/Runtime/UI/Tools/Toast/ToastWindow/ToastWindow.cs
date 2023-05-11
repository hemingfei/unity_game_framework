/****************************************************
*	文件：ToastWindow.cs
*	作者：何明飞
*	邮箱：hemingfei@outlook.com
*	日期：2023/01/07 17:14:31
*	功能：暂无
*****************************************************/

using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using GameFramework;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace CustomGameFramework.Runtime
{
	/// <summary>
	/// 提示文案
	/// </summary>
	public partial class ToastWindow : UIAnimWindowBase //  /*使用 UIAnimView 动画组件 则继承 UIAnimWindowBase, 不使用动画则继承 UIWindowBase*/ 
	{
		public class OpenToastParam : IReference
		{
			public string Msg;
			public float Duration;
			public System.Action ClickCallback;
			public int ImageIndex;
			public string ImageUrl;
			public OpenToastParam()
			{
				
			}
			public static OpenToastParam Create(string msg, float duration, System.Action clickCallback = null, int imageIndex = 0, string imageUrl = "")
			{
				var p = ReferencePool.Acquire<ToastWindow.OpenToastParam>();
				p.Msg = msg;
				p.Duration = duration;
				p.ClickCallback = clickCallback;
				p.ImageIndex = imageIndex;
				p.ImageUrl = imageUrl;
				return p;
			}

			public void Clear()
			{
				Msg = string.Empty;
				Duration = 0;
				ClickCallback = null;
				ImageIndex = 0;
				ImageUrl = string.Empty;
			}
		}

		#region 变量

        private Vector3 _contentInitLocalPos;
        private float _showTime = 2f;
        private WebTextureComponent _webTexture;
        private UpdaterComponent _updater;
        
        private ContentSizeFitter _msgContentSizeFitter;
        private RectTransform _msgContentRectTransform;
        private RectTransform _bgImgRectTransform;
        private RectTransform _goToastRectTransform;
        
        private bool _hasImage = false;
        private int _imageIndex = 0;
        private string _imageUrl;

        private System.Action _clickCallback;

        private Texture _localTexture;
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
			if (_localTexture != null)
			{
				UIManager.ResourceMgr.ReleaseAsset(_localTexture);
			}
		}

		// 动画进入完成
		public override void OnCompleteEnterAnim()
		{
			PlayToastAnim();
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
			_webTexture = UnityGameFramework.Runtime.GameEntry.GetComponent<WebTextureComponent>();
			_updater = UnityGameFramework.Runtime.GameEntry.GetComponent<UpdaterComponent>();
		}

		// 重置变量
		private void ResetValues()
		{
			_localTexture = null;
			
			m_RawImg_pic.gameObject.SetActive(false);
			
			var toastOpenParam = (OpenToastParam)m_OpenUIParams[0];

			m_Txt_Msg.text = toastOpenParam.Msg;
			_showTime = toastOpenParam.Duration;
			Log.Debug("Toast: {0}, Duration: {1}",m_Txt_Msg.text,_showTime);

			_clickCallback = null;
			if (toastOpenParam.ClickCallback != null)
			{
				_clickCallback = toastOpenParam.ClickCallback;
			}
			
			_hasImage = false;
			if (!string.IsNullOrEmpty(toastOpenParam.ImageUrl))
			{
				_imageIndex = toastOpenParam.ImageIndex;
				_imageUrl = toastOpenParam.ImageUrl;
				_hasImage = true;
				if (_imageUrl.StartsWith("Assets"))
				{
					_localTexture = UIManager.ResourceMgr.LoadAssetSync<Texture>(_imageUrl);
					m_RawImg_pic.texture = _localTexture;
					m_RawImg_pic.gameObject.SetActive(true);
				}
				else
				{
					_webTexture.Get(_imageUrl, (s, b, tex) =>
					{
						if (b)
						{
							m_RawImg_pic.texture = tex;
							m_RawImg_pic.gameObject.SetActive(true);
						}
					});
				}
			}
			
			ReferencePool.Release(toastOpenParam);

			if( _msgContentSizeFitter.horizontalFit == ContentSizeFitter.FitMode.Unconstrained)
			{
				_msgContentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
				_msgContentSizeFitter.SetLayoutVertical();
				_msgContentSizeFitter.SetLayoutHorizontal();
			}

			var bgSizeDelta = Vector2.one;
			
			if (m_Txt_Msg.preferredWidth>=_goToastRectTransform.sizeDelta.x)
			{
				_msgContentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
				float contentY = _msgContentRectTransform.sizeDelta.y;
				_msgContentRectTransform.sizeDelta = new Vector2(_goToastRectTransform.sizeDelta.x, contentY);
				_msgContentSizeFitter.SetLayoutVertical();
				_msgContentSizeFitter.SetLayoutHorizontal();
				m_Txt_Msg.alignment = TextAnchor.MiddleLeft;
				bgSizeDelta = _msgContentRectTransform.sizeDelta;
			}
			else
			{
				_msgContentSizeFitter.SetLayoutVertical();
				_msgContentSizeFitter.SetLayoutHorizontal();
				m_Txt_Msg.alignment = TextAnchor.MiddleCenter;
				bgSizeDelta.x = m_Txt_Msg.preferredWidth;
				bgSizeDelta.y = m_Txt_Msg.preferredHeight;
			}
			
			Vector2 bgSize = Vector2.one;
			bgSize.x = bgSizeDelta.x + 50;
			bgSize.y = bgSizeDelta.y + 30;
			_bgImgRectTransform.sizeDelta = bgSize;
			_bgImgRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, bgSize.x);

			var msgY = (m_Txt_Msg.preferredHeight - bgSize.y) / 2;
			_msgContentRectTransform.anchoredPosition = new Vector2(_msgContentRectTransform.anchoredPosition.x, msgY);

			
			_updater.ExecuteUpdateAction(SetImagePosition);
		}

		// 获取自定义组件
		private void GetCustomComponents()
		{
			_msgContentSizeFitter = m_Txt_Msg.GetComponent<ContentSizeFitter>();
			_msgContentRectTransform = m_Txt_Msg.GetComponent<RectTransform>();
			_bgImgRectTransform = m_Img_Bg.GetComponent<RectTransform>();
			_goToastRectTransform = m_Go_Toast.GetComponent<RectTransform>();
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
			UIManager.CloseUIWindow<ToastWindow>(this);
        }

		#endregion

		#endregion

		private async void PlayToastAnim()
		{
			SetImagePosition();
			await UniTask.Delay(TimeSpan.FromSeconds(_showTime), ignoreTimeScale: false);
			OnToastAnimFinish();
		}

		private void OnToastAnimFinish()
		{
			CloseSelfWinodw();
		}

		/// <summary>
		/// 得到Text中字符的位置；canvas:所在的Canvas，text:需要定位的Text,charIndex:Text中的字符位置
		/// </summary>
		public Vector3 GetPosAtText(Canvas canvas,Text text,int charIndex)
		{
			string textStr = text.text;
			Vector3 charPos = Vector3.zero;
			if (charIndex <= textStr.Length && charIndex > 0)
			{
				TextGenerator textGen = new TextGenerator(textStr.Length);
				Vector2 extents = text.gameObject.GetComponent<RectTransform>().rect.size;
				textGen.Populate(textStr, text.GetGenerationSettings(extents));
 
				int newLine = textStr.Substring(0, charIndex).Split('\n').Length - 1;
				int whiteSpace = textStr.Substring(0, charIndex).Split(' ').Length - 1;
				int indexOfTextQuad = (charIndex * 4) + (newLine * 4) - 4;
				if (indexOfTextQuad < textGen.vertexCount)
				{
					charPos = (textGen.verts[indexOfTextQuad].position +
					           textGen.verts[indexOfTextQuad + 1].position +
					           textGen.verts[indexOfTextQuad + 2].position +
					           textGen.verts[indexOfTextQuad + 3].position) / 4f;
                
 
				}
			}
			charPos /= canvas.scaleFactor;//适应不同分辨率的屏幕
			charPos = text.transform.TransformPoint(charPos);//转换为世界坐标
			return charPos;
		}

		private void SetImagePosition()
		{
			if (_hasImage)
			{
				var canvas = UIManager.GetCanvas("BuildinUI");
				Vector3 stringPos = GetPosAtText(canvas, m_Txt_Msg, _imageIndex + 3);
				m_RawImg_pic.transform.position = stringPos;
			}
		}
		
		/// <summary>
		/// Btn_Toast 点击事件
		/// </summary>
		private void On_Btn_Toast_Clicked()
		{
			_clickCallback?.Invoke();
		}
		
	}
}

