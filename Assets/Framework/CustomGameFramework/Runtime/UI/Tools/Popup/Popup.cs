/****************************************************
*	文件：Popup.cs
*	作者：何明飞
*	邮箱：hemingfei@outlook.com
*	日期：2023/01/19 11:25:38
*	功能：暂无
*****************************************************/

/****************************************************
*	文件：Popup.cs
*	作者：何明飞
*	邮箱：hemingfei@outlook.com
*	日期：2023/01/07 21:24:45
*	功能：暂无
*****************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using GameFramework;
using UnityEngine;

namespace CustomGameFramework.Runtime
{
    public class Popup : IReference
    {
        private const int MaxWindowNum = 5;

        public Popup()
        {
            Init();
        }

        public void Init(int maxWindowNum = MaxWindowNum)
        {
            string[] dialogWindowNames = new string[]
            {
                nameof(PopupWindow),
            };
            for (int i = 0; i < dialogWindowNames.Length; i++)
            {
                var dialogWindowName = dialogWindowNames[i];
                if (!UIManager.s_uiRestrict.ContainsKey(dialogWindowName))
                {
                    UIManager.s_uiRestrict.Add(dialogWindowName, 1);
                }

                if (!UIManager.s_uiCustomPath.ContainsKey(dialogWindowName))
                {
                    UIManager.s_uiCustomPath.Add(dialogWindowName, "");
                }

                UIManager.s_uiRestrict[dialogWindowName] = maxWindowNum;
                UIManager.s_uiCustomPath[dialogWindowName] = UIManager.GetUIBuildinPath(dialogWindowName);
            }
        }

        /// <summary>
        /// 显示弹窗
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="content">内容</param>
        /// <param name="completed">按钮点击回调</param>
        /// <param name="popupStyle">弹窗样式</param>
        /// <param name="ok">确定按钮的文案</param>
        /// <param name="no">取消按钮的文案</param>
        public PopupWindow Show(string title, string content, Action<bool> completed,
            PopupStyle popupStyle = PopupStyle.Default, string ok = "确定",
            string no = "取消")
        {
            var window = UIManager.OpenUIWindowSync<PopupWindow>(null, title, content, completed, popupStyle, ok, no);
            return window;
        }
        
        /// <summary>
        /// 关闭指定的UI
        /// </summary>
        /// <param name="refreshLoadingWindow">UI窗口，也就是打开Show的返回值</param>
        public void Close(PopupWindow refreshLoadingWindow)
        {
            UIManager.CloseUIWindow(refreshLoadingWindow);
        }

        /// <summary>
        /// 关闭最上面的一个UI
        /// </summary>
        public void Close()
        {
            UIManager.CloseUIWindow<PopupWindow>();
        }

        public void Clear()
        {
            
        }
    }
}

