//
//  UIEditorWindow.cs 
//
//  Author: HGT 
//
//  Copyright (c) 2021 hegametech.com 
//

using System;
using System.Collections.Generic;
using System.IO;
using CustomGameFramework.Runtime;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UI;

namespace CustomGameFramework.Editor
{
    public class UIEditorWindow : EditorWindow
    {
        private bool m_isShowUIManagerCreate;
        private bool m_isShowUISortingLayerCreate;
        private UILayerManager m_UILayerManager;
        private UIManager m_UIManager;


        private void OnEnable()
        {
            var uiManager = GameObject.Find("UI");

            if (uiManager)
            {
                m_UIManager = uiManager.GetComponent<UIManager>();
                m_UILayerManager = uiManager.GetComponent<UILayerManager>();
            }

            FindAllUI();

            m_isShowUIManagerCreate = false;
        }

        private void OnGUI()
        {
            titleContent.text = "UI编辑器";
            EditorGUILayout.BeginVertical();

            CreateUIGUI();

            EditorGUILayout.Space(10);

            UIToolGUI();
            UIManagerGUI();

            //EditorGUI.indentLevel = 0;
            //isFoldUIQuick = EditorGUILayout.Foldout(isFoldUIQuick, "快速操作:");

            //if (isFoldUIQuick)
            //{
            //    if (GUILayout.Button("构建字段和组件"))
            //    {
            //        UICreateService.CreateObservedScript();
            //    }
            //}

            //UITemplate();
            //UIStyleGUI();
            EditorGUILayout.EndVertical();
        }

        //当工程改变时
        private void OnProjectChange()
        {
            FindAllUI();
        }

        [MenuItem("Tools/Create UI \t\t\t 创建UI", false, 20)]
        public static void ShowWindow()
        {
            GetWindow(typeof(UIEditorWindow));
        }

        #region UIManager

        public Vector2 m_referenceResolution = new(960, 640);
        public CanvasScaler.ScreenMatchMode m_MatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;

        public bool m_isOnlyUICamera;
        public bool m_isVertical;

        private void UIManagerGUI()
        {
            //if (EditorUtil.DrawHeader("创建 UIManager", "CreateUIManager", true, false))
            m_isShowUIManagerCreate = GUILayout.Toggle(m_isShowUIManagerCreate, " 创建 UIManager");
            if (m_isShowUIManagerCreate)
            {
                EditorGUI.indentLevel = 1;
                m_referenceResolution = EditorGUILayout.Vector2Field("参考分辨率", m_referenceResolution);
                m_isOnlyUICamera = EditorGUILayout.Toggle("只有一个UI摄像机", m_isOnlyUICamera);
                m_isVertical = EditorGUILayout.Toggle("是否竖屏", m_isVertical);

                if (GUILayout.Button("创建UIManager"))
                    UICreateService.CreatUIManager(m_referenceResolution, m_MatchMode, m_isOnlyUICamera, m_isVertical);

                CreateUICameraGUI();
            }
        }

        #region CreateUICamera

        private bool isCreateUICamera;
        private string cameraKey;
        private float cameraDepth = 1;

        private void CreateUICameraGUI()
        {
            isCreateUICamera = EditorGUILayout.Foldout(isCreateUICamera, "CreateUICamera:");

            if (isCreateUICamera)
            {
                EditorGUI.indentLevel = 2;
                cameraKey = EditorGUILayout.TextField("Camera Key", cameraKey);
                cameraDepth = EditorGUILayout.FloatField("Camera Depth", cameraDepth);

                if (cameraKey != "")
                {
                    if (GUILayout.Button("CreateUICamera"))
                    {
                        UICreateService.CreateUICamera(m_UIManager, cameraKey, cameraDepth, m_referenceResolution,
                            m_MatchMode, m_isOnlyUICamera, m_isVertical);
                        cameraKey = "";
                    }
                }
                else
                {
                    EditorGUILayout.LabelField("Camera Key 不能为空");
                }
            }
        }

        #endregion

        #endregion

        #region createUI

        private bool isAutoCreatePrefab = true;

        private bool isUseHotfix;

        private string m_UIname = "";
        private string m_description = "";
        private int m_UICameraKeyIndex;
        private string[] cameraKeyList;
        private UIType m_UIType = UIType.Normal;

        private void CreateUIGUI()
        {
            if (EditorUtil.DrawHeader("创建 UI", "CreateUI", true, false))
            {
                cameraKeyList = UIManager.GetCameraNames();
                EditorGUI.indentLevel = 1;
                EditorGUILayout.LabelField("提示： 脚本和 UI 名称会自动添加Window后缀");
                m_UIname = EditorGUILayout.TextField("UI Name:", m_UIname);
                m_UICameraKeyIndex = EditorGUILayout.Popup("Camera", m_UICameraKeyIndex, cameraKeyList);
                m_UIType = (UIType)EditorGUILayout.EnumPopup("UI Type:", m_UIType);

                isAutoCreatePrefab = EditorGUILayout.Toggle("是否同时创建 Prefab", isAutoCreatePrefab);

                //isUseHotfix = EditorGUILayout.Toggle("是否为热更新 UI", isUseHotfix);
                isUseHotfix = false;
                if (!isUseHotfix)
                {
                    if (m_UIname != "")
                    {
                        var l_nameTmp = m_UIname + "Window";
                        var assem = UIEditorConstant.S_AppNamespace + "." + l_nameTmp + "," +
                                    UIEditorConstant.S_UIAssemblyName +
                                    ", Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"; // typeof(HGT.AppComponent.AppBoot).Assembly.GetName();
                        var l_typeTmp = Type.GetType(assem, false);

                        if (l_typeTmp != null)
                        {
                            if (l_typeTmp.BaseType.Equals(typeof(UIWindowBase)))
                            {
                                if (GUILayout.Button("已存在脚本，点击创建Prefab"))
                                {
                                    UICreateService.CreateUIPrefab(l_nameTmp, cameraKeyList[m_UICameraKeyIndex],
                                        m_UIType, m_UILayerManager, isAutoCreatePrefab);
                                    m_UIname = "";
                                }
                            }
                            else
                            {
                                EditorGUILayout.LabelField("该类没有继承UIWindowBase");
                            }
                        }
                        else
                        {
                            m_description = EditorGUILayout.TextField("UI 描述:", m_description);

                            if (GUILayout.Button("创建 UI 脚本"))
                            {
                                EditorPrefs.SetBool("Create_UI", true);
                                EditorPrefs.SetString("Create_UI_Name", m_UIname);
                                EditorPrefs.SetBool("Create_UI_Prefab", isAutoCreatePrefab);
                                EditorPrefs.SetInt("Create_UI_TypePrefab", (int)m_UIType);
                                EditorPrefs.SetInt("Create_UI_CameraIndex", m_UICameraKeyIndex);
                                UICreateService.CreatUIScript(l_nameTmp, m_description);
                                m_UIname = "";
                            }
                        }
                    }
                }
                else
                {
                    if (m_UIname != "")
                    {
                        var l_nameTmp = m_UIname + "Window";
                        var assem = UIEditorConstant.S_HotfixNamespace + "." + l_nameTmp + "," +
                                    UIEditorConstant.S_HotfixUIAssemblyName +
                                    ", Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"; // typeof(HGT.AppComponent.AppBoot).Assembly.GetName();
                        var l_typeTmp = Type.GetType(assem, false);

                        if (l_typeTmp != null)
                        {
                            if (GUILayout.Button("已存在热更脚本，点击创建Prefab"))
                            {
                                UICreateService.CreateHotfixUIPrefab(l_nameTmp, cameraKeyList[m_UICameraKeyIndex],
                                    m_UIType, m_UILayerManager, isAutoCreatePrefab);
                                m_UIname = "";
                            }
                        }
                        else
                        {
                            m_description = EditorGUILayout.TextField("UI 描述:", m_description);

                            if (GUILayout.Button("创建 UI 热更脚本"))
                            {
                                EditorPrefs.SetBool("Create_HotfixUI", true);
                                EditorPrefs.SetString("Create_UI_Name", m_UIname);
                                EditorPrefs.SetBool("Create_UI_Prefab", isAutoCreatePrefab);
                                EditorPrefs.SetInt("Create_UI_TypePrefab", (int)m_UIType);
                                EditorPrefs.SetInt("Create_UI_CameraIndex", m_UICameraKeyIndex);
                                UICreateService.CreatHotfixUIScript(l_nameTmp, m_description);
                                m_UIname = "";
                            }
                        }
                    }
                }
            }
        }

        [DidReloadScripts]
        private static void OnUiScriptCreated()
        {
            #region 创建 Game UI

            if (EditorPrefs.GetBool("Create_UI", false))
            {
                EditorPrefs.SetBool("Create_UI", false);
                var m_UIname = EditorPrefs.GetString("Create_UI_Name");
                var l_nameTmp = m_UIname + "Window";
                var assem = UIEditorConstant.S_AppNamespace + "." + l_nameTmp + "," +
                            UIEditorConstant.S_UIAssemblyName +
                            ", Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"; // typeof(HGT.AppComponent.AppBoot).Assembly.GetName();
                var l_typeTmp = Type.GetType(assem, false);
                if (l_typeTmp != null)
                {
                    if (l_typeTmp.BaseType == typeof(UIWindowBase) || l_typeTmp.BaseType == typeof(UIAnimWindowBase))
                    {
                        var uiManager = GameObject.Find("UI");
                        var m_UILayerManager = uiManager.GetComponent<UILayerManager>();
                        var isAutoCreatePrefab = EditorPrefs.GetBool("Create_UI_Prefab", true);
                        var m_UIType = (UIType)EditorPrefs.GetInt("Create_UI_TypePrefab", 2);
                        var m_UICameraKeyIndex = EditorPrefs.GetInt("Create_UI_CameraIndex", 0);
                        var cameraKeyList = UIManager.GetCameraNames();
                        UICreateService.CreateUIPrefab(l_nameTmp, cameraKeyList[m_UICameraKeyIndex], m_UIType,
                            m_UILayerManager, isAutoCreatePrefab);
                    }
                    else
                    {
                        Debug.LogError("l_typeTmp is not UIWindowBase");
                    }
                }
            }

            #endregion

            #region 创建 Hotfix UI

            if (EditorPrefs.GetBool("Create_HotfixUI", false))
            {
                EditorPrefs.SetBool("Create_HotfixUI", false);
                var m_UIname = EditorPrefs.GetString("Create_UI_Name");
                var l_nameTmp = m_UIname + "Window";
                var assem = UIEditorConstant.S_HotfixNamespace + "." + l_nameTmp + "," +
                            UIEditorConstant.S_HotfixUIAssemblyName +
                            ", Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"; // typeof(HGT.AppComponent.AppBoot).Assembly.GetName();
                var l_typeTmp = Type.GetType(assem, false);

                if (l_typeTmp != null)
                {
                    var uiManager = GameObject.Find("UI");
                    var m_UILayerManager = uiManager.GetComponent<UILayerManager>();
                    var isAutoCreatePrefab = EditorPrefs.GetBool("Create_UI_Prefab", true);
                    var m_UIType = (UIType)EditorPrefs.GetInt("Create_UI_TypePrefab", 2);
                    var m_UICameraKeyIndex = EditorPrefs.GetInt("Create_UI_CameraIndex", 0);
                    var cameraKeyList = UIManager.GetCameraNames();
                    UICreateService.CreateHotfixUIPrefab(l_nameTmp, cameraKeyList[m_UICameraKeyIndex], m_UIType,
                        m_UILayerManager, isAutoCreatePrefab);
                }
            }

            #endregion
        }

        #endregion

        #region UITool

        private void UIToolGUI()
        {
            m_isShowUISortingLayerCreate = GUILayout.Toggle(m_isShowUISortingLayerCreate, " 创建 UISortLayer");
            if (m_isShowUISortingLayerCreate)
            {
                EditorGUI.indentLevel = 1;

                if (GUILayout.Button("重设UI sortLayer")) ResetUISortLayer();
            }
        }

        private void ResetUISortLayer()
        {
            EditorUtil.AddSortLayerIfNotExist("GameUI");
            EditorUtil.AddSortLayerIfNotExist("Fixed");
            EditorUtil.AddSortLayerIfNotExist("Normal");
            EditorUtil.AddSortLayerIfNotExist("TopBar");
            EditorUtil.AddSortLayerIfNotExist("PopUp");
            EditorUtility.DisplayDialog("提示", "添加 sorting layer 完成", "确定");
        }

        #endregion

        #region UI

        //所有UI预设
        public static Dictionary<string, GameObject> allUIPrefab;


        /// <summary>
        ///     获取到所有的UIprefab
        /// </summary>
        public void FindAllUI()
        {
            allUIPrefab = new Dictionary<string, GameObject>();
            FindAllUIResources(Application.dataPath + "/" + UIEditorConstant.S_ResFolderModePath + "/UI/");
        }

        //读取“Resources/UI”目录下所有的UI预设
        public void FindAllUIResources(string path)
        {
            if (!Directory.Exists(path)) return;

            var allUIPrefabName = Directory.GetFiles(path);

            foreach (var item in allUIPrefabName)
            {
                var oneUIPrefabName = GetFileNameByPath(item);

                if (item.EndsWith(".prefab"))
                {
                    var oneUIPrefabPsth = path + "/" + oneUIPrefabName;
                    allUIPrefab.Add(oneUIPrefabName,
                        AssetDatabase.LoadAssetAtPath("Assets/" + oneUIPrefabPsth, typeof(GameObject)) as GameObject);
                }
            }

            var dires = Directory.GetDirectories(path);

            for (var i = 0; i < dires.Length; i++) FindAllUIResources(dires[i]);
        }

        public string GetFileNameByPath(string path)
        {
            var fi = new FileInfo(path);
            return fi.Name; // text.txt
        }

        #endregion
    }
}