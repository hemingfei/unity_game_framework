//
//  UIStackManager.cs 
//
//  Author: HGT 
//
//  Copyright (c) 2021 hegametech.com 
//

using System;
using System.Collections.Generic;
using UnityEngine;

namespace CustomGameFramework.Runtime
{
    public class UIStackManager : MonoBehaviour
    {
        public List<UIWindowBase> m_normalStack = new();
        public List<UIWindowBase> m_fixedStack = new();
        public List<UIWindowBase> m_popupStack = new();
        public List<UIWindowBase> m_topBarStack = new();

        public void OnUIOpen(UIWindowBase ui)
        {
            switch (ui.m_UIType)
            {
                case UIType.Fixed:
                    m_fixedStack.Add(ui);
                    SetSortingOrders(m_fixedStack);
                    break;

                case UIType.Normal:
                    m_normalStack.Add(ui);
                    SetSortingOrders(m_normalStack);
                    break;

                case UIType.PopUp:
                    m_popupStack.Add(ui);
                    SetSortingOrders(m_popupStack);
                    break;

                case UIType.TopBar:
                    m_topBarStack.Add(ui);
                    SetSortingOrders(m_topBarStack);
                    break;
            }
        }

        public void OnUIClose(UIWindowBase ui)
        {
            switch (ui.m_UIType)
            {
                case UIType.Fixed:
                    m_fixedStack.Remove(ui);
                    SetSortingOrders(m_fixedStack);
                    break;

                case UIType.Normal:
                    m_normalStack.Remove(ui);
                    SetSortingOrders(m_normalStack);
                    break;

                case UIType.PopUp:
                    m_popupStack.Remove(ui);
                    SetSortingOrders(m_popupStack);
                    break;

                case UIType.TopBar:
                    m_topBarStack.Remove(ui);
                    SetSortingOrders(m_topBarStack);
                    break;
            }
        }

        private void SetSortingOrders(List<UIWindowBase> uis)
        {
            var index = 0;
            foreach (var ui in uis)
            {
                index++;
                ui.m_canvas.sortingOrder = index * 100;
            }
        }

        // public void CloseLastUIWindow(UIType uiType = UIType.Normal)
        // {
        //     UIWindowBase ui = GetLastUI(uiType);
        //     
        //     if (ui != null)
        //     {
        //         UIManager.CloseUIWindow(ui);
        //     }
        // }

        public UIWindowBase GetLastUI(UIType uiType)
        {
            switch (uiType)
            {
                case UIType.Fixed:
                    if (m_fixedStack.Count > 0)
                        return m_fixedStack[m_fixedStack.Count - 1];
                    return null;

                case UIType.Normal:
                    if (m_normalStack.Count > 0)
                        return m_normalStack[m_normalStack.Count - 1];
                    return null;

                case UIType.PopUp:
                    if (m_popupStack.Count > 0)
                        return m_popupStack[m_popupStack.Count - 1];
                    return null;

                case UIType.TopBar:
                    if (m_topBarStack.Count > 0)
                        return m_topBarStack[m_topBarStack.Count - 1];
                    return null;
            }

            throw new Exception("CloseLastUIWindow does not support GameUI");
        }
    }
}