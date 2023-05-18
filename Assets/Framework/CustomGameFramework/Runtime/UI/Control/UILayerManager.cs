//
//  UILayerManager.cs 
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
    public class UILayerManager : MonoBehaviour
    {
        public List<UICameraData> UICameraList = new();

        public void Awake()
        {
            for (var i = 0; i < UICameraList.Count; i++)
            {
                var data = UICameraList[i];

                data.m_root.SetActive(true);

                data.m_root.transform.localPosition = new Vector3(0, 0, (i + 1) * -2000);

                if (data.m_root == null)
                    Log.Error("UILayerManager :Root is null! " + " key : " + data.m_key + " index : " + i);

                if (data.m_camera == null)
                    Log.Error("UILayerManager :Camera is null! " + " key : " + data.m_key + " index : " + i);

                if (data.m_GameUILayerParent == null)
                    Log.Error("UILayerManager :GameUILayerParent is null!" + " key : " + data.m_key + " index : " + i);

                if (data.m_FixedLayerParent == null)
                    Log.Error("UILayerManager :FixedLayerParent is null!" + " key : " + data.m_key + " index : " + i);

                if (data.m_NormalLayerParent == null)
                    Log.Error("UILayerManager :NormalLayerParent is null!" + " key : " + data.m_key + " index : " + i);

                if (data.m_TopbarLayerParent == null)
                    Log.Error("UILayerManager :TopbarLayerParent is null!" + " key : " + data.m_key + " index : " + i);

                if (data.m_PopUpLayerParent == null)
                    Log.Error("UILayerManager :popUpLayerParent is null!" + " key : " + data.m_key + " index : " + i);
            }
        }

        public void SetLayer(UIWindowBase ui, string cameraKey = null)
        {
            var data = GetUICameraDataByKey(cameraKey);

            if (cameraKey == null)
                data = GetUICameraDataByKey(ui.cameraKey);
            else
                data = GetUICameraDataByKey(cameraKey);

            var rt = ui.GetComponent<RectTransform>();

            switch (ui.m_UIType)
            {
                case UIType.GameUI:
                    ui.transform.SetParent(data.m_GameUILayerParent);
                    break;

                case UIType.Fixed:
                    ui.transform.SetParent(data.m_FixedLayerParent);
                    break;

                case UIType.Normal:
                    ui.transform.SetParent(data.m_NormalLayerParent);
                    break;

                case UIType.TopBar:
                    ui.transform.SetParent(data.m_TopbarLayerParent);
                    break;

                case UIType.PopUp:
                    ui.transform.SetParent(data.m_PopUpLayerParent);
                    break;
            }

            rt.localScale = Vector3.one;
            rt.sizeDelta = Vector2.zero;

            if (ui.m_UIType != UIType.GameUI)
            {
                rt.anchorMin = Vector2.zero;
                rt.anchorMax = Vector3.one;
                rt.sizeDelta = Vector2.zero;
                rt.transform.localPosition = new Vector3(0, 0, ui.m_PosZ);
                rt.anchoredPosition3D = new Vector3(0, 0, ui.m_PosZ);
                rt.SetAsLastSibling();
            }
            else
            {
                var lp = rt.transform.localPosition;
                lp.z = 0;
                rt.transform.localPosition = lp;
            }
        }

        public UICameraData GetUICameraDataByKey(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                if (UICameraList.Count > 0) return UICameraList[0];

                throw new Exception("UICameraList is null ! " + key);
            }

            for (var i = 0; i < UICameraList.Count; i++)
                if (UICameraList[i].m_key == key)
                    return UICameraList[i];

            throw new Exception("Dont Find UILayerData by " + key);
        }

        [Serializable]
        public struct UICameraData
        {
            public string m_key;
            public GameObject m_root;
            public Camera m_camera;
            public Canvas m_canvas;
            public Transform m_GameUILayerParent;
            public Transform m_FixedLayerParent;
            public Transform m_NormalLayerParent;
            public Transform m_TopbarLayerParent;
            public Transform m_PopUpLayerParent;
        }
    }
}