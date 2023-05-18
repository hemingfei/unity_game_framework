//
//  UIWindowBase.cs 
//
//  Author: HGT 
//
//  Copyright (c) 2021 hegametech.com 
//

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace CustomGameFramework.Runtime
{
    public abstract partial class UIWindowBase : UIBase
    {
        public enum WindowStatus
        {
            Create,
            OpenAnim,
            Open,
            CloseAnim,
            Close,
            Hide
        }

        //[HideInInspector]
        public string cameraKey;
        public UIType m_UIType;

        public WindowStatus windowStatus;

        public GameObject m_bgMask;
        public GameObject m_uiRoot;

        [HideInInspector] public string OpenTimeStamp;
        [HideInInspector] public string CloseTimeStamp;

        public float m_PosZ; //Z轴偏移

        /// <summary>
        ///     需要根据屏幕高度缩放的物体
        /// </summary>
        public List<GameObject> m_objectNeedAutoScaleWithScreen = new();

        public object[] m_CloseUIParams;

        public object[] m_OpenUIParams;


        /// <summary>
        ///     是否自动流海屏适配
        /// </summary>
        public virtual bool IsAutoFixNotchScreen => false;

        /// <summary>
        ///     流海屏适配后的偏移。填入大于0的值
        /// </summary>
        public virtual float OffsetIfNotchScreen => 0;

        #region 重载方法

        public override void OnUIInit()
        {
            if (IsAutoFixNotchScreen)
                if (UIManager.SafeScreenTopOffset() != 0)
                {
                    var offSizeY = Mathf.Max(100f, UIManager.SafeScreenTopOffset());
                    (m_uiRoot.transform as RectTransform).offsetMax = new Vector2(
                        (m_uiRoot.transform as RectTransform).offsetMax.x, -(offSizeY - OffsetIfNotchScreen));
                }

            if (m_objectNeedAutoScaleWithScreen.Count > 0)
            {
                var vv3 = UIManager.GetSafeHeightScale();
                foreach (var gg in m_objectNeedAutoScaleWithScreen) gg.transform.localScale *= vv3;
            }
        }

        public override void OnUIDispose()
        {
        }

        public virtual void OnOpen()
        {
        }

        public virtual void OnClose()
        {
        }

        public virtual void EnterAnim(UIAnimCallBack animComplete, UIEventCallBack callBack, params object[] objs)
        {
            //默认无动画
            animComplete(this, callBack, objs);
        }

        public virtual void OnStartEnterAnim()
        {
        }

        public virtual void OnCompleteEnterAnim()
        {
        }

        public virtual void ExitAnim(UIAnimCallBack animComplete, UIEventCallBack callBack, params object[] objs)
        {
            //默认无动画
            animComplete(this, callBack, objs);
        }

        public virtual void OnStartExitAnim()
        {
        }

        public virtual void OnCompleteExitAnim()
        {
        }

        public virtual void OnHide()
        {
        }

        public virtual void OnShow()
        {
        }

        public virtual void OnRefresh()
        {
        }

        public virtual void Show()
        {
            gameObject.SetActive(true);
        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }

        #endregion

        #region 继承方法

        public void InitWindow(int id)
        {
            var list = new List<IUILifeCycle>();
            Init(null, id);
            RecursionInitUI(null, this, id, list);
        }

        /// <summary>
        ///     递归初始化UI
        /// </summary>
        /// <param name="uiBase"></param>
        public void RecursionInitUI(UIBase parentUI, UIBase uiBase, int id, List<IUILifeCycle> UIList)
        {
            var childIndex = 0;

            for (var i = 0; i < uiBase.m_objectList.Count; i++)
            {
                var go = uiBase.m_objectList[i];

                if (go != null)
                {
                    var tmp = go.GetComponent<IUILifeCycle>();

                    if (tmp != null)
                    {
                        if (!UIList.Contains(tmp))
                        {
                            uiBase.AddLifeCycleComponent(tmp);
                            UIList.Add(tmp);
                            var subUI = uiBase.m_objectList[i].GetComponent<UIBase>();

                            if (subUI != null) RecursionInitUI(uiBase, subUI, childIndex++, UIList);
                        }
                        else
                        {
                            Log.Error("InitWindow 重复的引用 " + uiBase.UIEventKey + " " + uiBase.m_objectList[i].name);
                        }
                    }
                }
                else
                {
                    Log.Warning("InitWindow objectList[" + i + "] is null !: " + uiBase.UIEventKey);
                }
            }
        }

        //刷新是主动调用
        public void Refresh()
        {
            UIEvent.FireNow(this, UIEventType.OnRefresh);
            OnRefresh();
        }

        public void SetOpenParam(params object[] objs)
        {
            m_OpenUIParams = objs;
            OpenTimeStamp = GetLocalTimeStampMillisecondsString();
            CloseTimeStamp = string.Empty;
        }

        public void SetCloseParam(params object[] objs)
        {
            m_CloseUIParams = objs;
            CloseTimeStamp = GetLocalTimeStampMillisecondsString();
        }

        private string GetLocalTimeStampMillisecondsString()
        {
            var ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalMilliseconds).ToString();
        }

        #endregion
    }
}