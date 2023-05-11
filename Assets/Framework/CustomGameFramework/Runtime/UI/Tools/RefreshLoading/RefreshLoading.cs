/****************************************************
*	文件：RefreshLoading.cs
*	作者：何明飞
*	邮箱：hemingfei@outlook.com
*	日期：2023/01/09 09:48:29
*	功能：暂无
*****************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using GameFramework;
using UnityEngine;

namespace CustomGameFramework.Runtime
{
    public class RefreshLoading : IReference
    {
        private const int MaxWindowNum = 5;

        public RefreshLoading()
        {
            Init();
        }

        public void Init(int maxWindowNum = MaxWindowNum)
        {
            string[] refreshLoadingWindowNames = new string[]
            {
                nameof(RefreshLoadingWindow),
            };
            for (int i = 0; i < refreshLoadingWindowNames.Length; i++)
            {
                var refreshLoadingWindowName = refreshLoadingWindowNames[i];
                if (!UIManager.s_uiRestrict.ContainsKey(refreshLoadingWindowName))
                {
                    UIManager.s_uiRestrict.Add(refreshLoadingWindowName, 1);
                }

                if (!UIManager.s_uiCustomPath.ContainsKey(refreshLoadingWindowName))
                {
                    UIManager.s_uiCustomPath.Add(refreshLoadingWindowName, "");
                }

                UIManager.s_uiRestrict[refreshLoadingWindowName] = maxWindowNum;
                UIManager.s_uiCustomPath[refreshLoadingWindowName] = UIManager.GetUIBuildinPath(refreshLoadingWindowName);
            }
        }

        /// <summary>
        /// 打开刷新加载UI
        /// </summary>
        /// <param name="content">是否要显示的内容</param>
        /// <param name="needRaycastBg">是否需要背景，用于遮挡屏幕内容防止点击</param>
        /// <returns>刷新加载UI，用于关闭等操作</returns>
        public RefreshLoadingWindow Show(string content = "", bool needRaycastBg = true)
        {
            var window = UIManager.OpenUIWindowSync<RefreshLoadingWindow>(null, content, needRaycastBg);
            return window;
        }

        /// <summary>
        /// 关闭指定的刷新加载UI
        /// </summary>
        /// <param name="refreshLoadingWindow">刷新页窗口，也就是打开Show的返回值</param>
        public void Close(RefreshLoadingWindow refreshLoadingWindow)
        {
            UIManager.CloseUIWindow(refreshLoadingWindow);
        }

        /// <summary>
        /// 关闭最上面的一个刷新加载UI
        /// </summary>
        public void Close()
        {
            UIManager.CloseUIWindow<RefreshLoadingWindow>();
        }

        public void Clear()
        {
            
        }
    }
}

