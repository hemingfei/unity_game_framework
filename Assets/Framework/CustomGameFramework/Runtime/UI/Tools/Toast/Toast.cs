/****************************************************
*	文件：Toast.cs
*	作者：何明飞
*	邮箱：hemingfei@outlook.com
*	日期：2023/01/07 16:29:17
*	功能：暂无
*****************************************************/

using System;
using GameFramework;

namespace CustomGameFramework.Runtime
{
    public class Toast : IReference
    {
        private const int MaxWindowNum = 5;

        public Toast()
        {
            Init();
        }

        public void Init(int maxWindowNum = MaxWindowNum)
        {
            string[] toastWindowNames = new string[]
            {
                nameof(ToastWindow),
            };
            for (int i = 0; i < toastWindowNames.Length; i++)
            {
                var toastWindowName = toastWindowNames[i];
                if (!UIManager.s_uiRestrict.ContainsKey(toastWindowName))
                {
                    UIManager.s_uiRestrict.Add(toastWindowName, 1);
                }

                if (!UIManager.s_uiCustomPath.ContainsKey(toastWindowName))
                {
                    UIManager.s_uiCustomPath.Add(toastWindowName, "");
                }

                UIManager.s_uiRestrict[toastWindowName] = maxWindowNum;
                UIManager.s_uiCustomPath[toastWindowName] = UIManager.GetUIBuildinPath(toastWindowName);
            }
        }

        /// <summary>
        /// 显示Toast
        /// </summary>
        /// <param name="msg">内容</param>
        /// <param name="duration">显示秒数</param>
        public void Show(string msg, float duration, Action clickCallback = null)
        {
            var openParam = ToastWindow.OpenToastParam.Create(msg, duration, clickCallback);
            UIManager.OpenUIWindow<ToastWindow>(null, openParam);
        }

        /// <summary>
        /// 显示Toast，建议手动使用颜色富文本直接调用Show方法，颜色会被  <color=#85aeffff>colorMsg</color> 替换
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="colorMsg"></param>
        /// <param name="duration"></param>
        public void ShowColor(string msg, string colorMsg, float duration, Action clickCallback = null)
        {
            string finalColorMsg = "<color=#85aeffff>" + colorMsg + "</color>";
            string finalMsg = msg.Replace(colorMsg, finalColorMsg);
            Show(finalMsg, duration, clickCallback);
        }

        /// <summary>
        /// 带上icon的toast，icon的位置用 [] 这个字符代替
        /// </summary>
        /// <param name="msg">内容，需要icon的地方用字符[]代替</param>
        /// <param name="duration"></param>
        /// <param name="imageUrl">图片地址,本地图片用Assets的全路径，网络图片用网页url</param>
        public void ShowImage(string msg, float duration, string imageUrl, Action clickCallback = null)
        {
            if (!msg.Contains("[]"))
            {
                Show(msg, duration, clickCallback);
            }
            else
            {
                int index = msg.IndexOf("[]", StringComparison.Ordinal);
                msg = msg.Replace("[]", "<color=#85aeff00>-----</color>");

                var openParam = ToastWindow.OpenToastParam.Create(msg, duration, clickCallback, index, imageUrl);
                UIManager.OpenUIWindow<ToastWindow>(null, openParam);
            }
        }

        public void Clear()
        {
            
        }
    }
}

