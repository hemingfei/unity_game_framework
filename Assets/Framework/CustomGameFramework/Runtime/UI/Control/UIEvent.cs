//
//  UIEvent.cs 
//
//  Author: HGT 
//
//  Copyright (c) 2021 hegametech.com 
//

using System;
using System.Collections.Generic;
using UnityGameFramework.Runtime;

namespace CustomGameFramework.Runtime
{
    public delegate void UIEventCallBack(UIWindowBase UI, params object[] objs);

    public delegate void UIAnimCallBack(UIWindowBase UIbase, UIEventCallBack callBack, params object[] objs);

    public class UIEvent
    {
        public static Dictionary<UIEventType, UIEventCallBack> s_allUIEvents = new();
        public static Dictionary<string, Dictionary<UIEventType, UIEventCallBack>> s_singleUIEvents = new();

        public static void FireNow(UIWindowBase ui, UIEventType uiEventType, params object[] objs)
        {
            if (ui == null)
            {
                Log.Error("FireNow ui is null!");
                return;
            }

            if (s_allUIEvents.ContainsKey(uiEventType))
                try
                {
                    if (s_allUIEvents[uiEventType] != null) s_allUIEvents[uiEventType](ui, objs);
                }
                catch (Exception e)
                {
                    Log.Error("UIEvent FireNow error:" + e);
                }

            if (s_singleUIEvents.ContainsKey(ui.name))
                if (s_singleUIEvents[ui.name].ContainsKey(uiEventType))
                    try
                    {
                        if (s_singleUIEvents[ui.name][uiEventType] != null)
                            s_singleUIEvents[ui.name][uiEventType](ui, objs);
                    }
                    catch (Exception e)
                    {
                        Log.Error("UIEvent FireNow error:" + e);
                    }

            switch (uiEventType)
            {
                case UIEventType.OnOpen:
                    UIManager.UIEventMgr.Fire(OpenUIFormSuccessEventArgs.EventId,
                        OpenUIFormSuccessEventArgs.Create(ui, objs));
                    break;
                case UIEventType.OnClose:
                    UIManager.UIEventMgr.Fire(CloseUIFormCompleteEventArgs.EventId,
                        CloseUIFormCompleteEventArgs.Create(ui.UIName, objs));
                    break;
            }
        }

        /// <summary>
        ///     注册单个UI派发的事件
        /// </summary>
        /// <param name="uiName">窗口名称</param>
        /// <param name="uiEventType">事件类型</param>
        /// <param name="callBack">回调函数</param>
        public static void Subscribe(string uiName, UIEventType uiEventType, UIEventCallBack callBack)
        {
            if (s_singleUIEvents.ContainsKey(uiName))
            {
                if (s_singleUIEvents[uiName].ContainsKey(uiEventType))
                    s_singleUIEvents[uiName][uiEventType] += callBack;
                else
                    s_singleUIEvents[uiName].Add(uiEventType, callBack);
            }
            else
            {
                s_singleUIEvents.Add(uiName, new Dictionary<UIEventType, UIEventCallBack>());
                s_singleUIEvents[uiName].Add(uiEventType, callBack);
            }
        }

        public static void UnSubscribe(string uiName, UIEventType uiEventType, UIEventCallBack callBack)
        {
            if (s_singleUIEvents.ContainsKey(uiName))
            {
                if (s_singleUIEvents[uiName].ContainsKey(uiEventType))
                    s_singleUIEvents[uiName][uiEventType] -= callBack;
                else
                    Log.Error("RemoveEvent 不存在的事件！ UIName " + uiName + " UIEvent " + uiEventType);
            }
            else
            {
                Log.Error("RemoveEvent 不存在的事件！ UIName " + uiName + " UIEvent " + uiEventType);
            }
        }

        /// <summary>
        ///     注册所有UI派发的事件类型
        /// </summary>
        /// <param name="uiEventType">事件类型</param>
        /// <param name="callBack">回调函数</param>
        public static void SubscribeAll(UIEventType uiEventType, UIEventCallBack callBack)
        {
            if (s_allUIEvents.ContainsKey(uiEventType))
                s_allUIEvents[uiEventType] += callBack;
            else
                s_allUIEvents.Add(uiEventType, callBack);
        }

        public static void UnSubscribeAll(UIEventType uiEventType, UIEventCallBack callBack)
        {
            if (s_allUIEvents.ContainsKey(uiEventType))
                s_allUIEvents[uiEventType] -= callBack;
            else
                Log.Error("RemoveAllUIEvent don't exits: " + uiEventType);
        }
    }
}