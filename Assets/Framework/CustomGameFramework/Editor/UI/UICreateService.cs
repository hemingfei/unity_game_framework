//
//  UICreateService.cs 
//
//  Author: HGT 
//
//  Copyright (c) 2021 hegametech.com 
//

using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.UI;
using System;
using System.Text;
using System.IO;
using System.Reflection;
using CustomGameFramework.Runtime;

namespace CustomGameFramework.Editor
{
    public class UICreateService
    {
        public static void CreatUIManager(Vector2 referenceResolution, CanvasScaler.ScreenMatchMode MatchMode,
            bool isOnlyUICamera, bool isVertical)
        {
            //新增五个层级
            EditorUtil.AddSortLayerIfNotExist("GameUI");
            EditorUtil.AddSortLayerIfNotExist("Fixed");
            EditorUtil.AddSortLayerIfNotExist("Normal");
            EditorUtil.AddSortLayerIfNotExist("TopBar");
            EditorUtil.AddSortLayerIfNotExist("PopUp");
            //UIManager
            GameObject UIManagerGo = new GameObject("UI");
            UIManagerGo.layer = LayerMask.NameToLayer("UI");
            UIManager UIManager = UIManagerGo.AddComponent<UIManager>();
            CreateUICamera(UIManager, "DefaultUI", 1, referenceResolution, MatchMode, isOnlyUICamera, isVertical);
            ProjectWindowUtil.ShowCreatedAsset(UIManagerGo);
            //保存UIManager
            ReSaveUIManager(UIManagerGo);
        }

        public static void CreateUICamera(UIManager UIManager, string key, float cameraDepth,
            Vector2 referenceResolution, CanvasScaler.ScreenMatchMode MatchMode, bool isOnlyUICamera, bool isVertical)
        {
            UILayerManager.UICameraData uICameraData = new UILayerManager.UICameraData();
            uICameraData.m_key = key;
            GameObject UIManagerGo = UIManager.gameObject;
            GameObject canvas = new GameObject(key);
            RectTransform canvasRt = canvas.AddComponent<RectTransform>();
            canvasRt.SetParent(UIManagerGo.transform);
            uICameraData.m_root = canvas;
            //UIcamera
            GameObject cameraGo = new GameObject("UICamera");
            cameraGo.transform.SetParent(canvas.transform);
            cameraGo.transform.localPosition = new Vector3(0, 0, -5000);
            Camera camera = cameraGo.AddComponent<Camera>();
            camera.cullingMask = LayerMask.GetMask("UI");
            camera.orthographic = true;
            camera.depth = cameraDepth;
            uICameraData.m_camera = camera;
            //Canvas
            Canvas canvasComp = canvas.AddComponent<Canvas>();
            canvasComp.renderMode = RenderMode.ScreenSpaceCamera;
            canvasComp.worldCamera = camera;
            uICameraData.m_canvas = canvasComp;
            //UI Raycaster
            canvas.AddComponent<GraphicRaycaster>();
            //CanvasScaler
            CanvasScaler scaler = canvas.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = referenceResolution;
            scaler.screenMatchMode = MatchMode;

            if (!isOnlyUICamera)
            {
                camera.clearFlags = CameraClearFlags.Depth;
                camera.depth = 1;
            }
            else
            {
                camera.clearFlags = CameraClearFlags.SolidColor;
                camera.backgroundColor = Color.black;
            }

            if (isVertical)
            {
                scaler.matchWidthOrHeight = 1;
            }
            else
            {
                scaler.matchWidthOrHeight = 0;
            }

            //挂载点
            GameObject goTmp = null;
            RectTransform rtTmp = null;
            UILayerManager UILayerManager = UIManagerGo.GetComponent<UILayerManager>();
            goTmp = new GameObject("GameUI");
            goTmp.layer = LayerMask.NameToLayer("UI");
            goTmp.transform.SetParent(canvas.transform);
            goTmp.transform.localScale = Vector3.one;
            rtTmp = goTmp.AddComponent<RectTransform>();
            rtTmp.anchorMax = new Vector2(1, 1);
            rtTmp.anchorMin = new Vector2(0, 0);
            rtTmp.anchoredPosition3D = Vector3.zero;
            rtTmp.sizeDelta = Vector2.zero;
            uICameraData.m_GameUILayerParent = goTmp.transform;
            goTmp = new GameObject("Fixed");
            goTmp.layer = LayerMask.NameToLayer("UI");
            goTmp.transform.SetParent(canvas.transform);
            goTmp.transform.localScale = Vector3.one;
            rtTmp = goTmp.AddComponent<RectTransform>();
            rtTmp.anchorMax = new Vector2(1, 1);
            rtTmp.anchorMin = new Vector2(0, 0);
            rtTmp.anchoredPosition3D = Vector3.zero;
            rtTmp.sizeDelta = Vector2.zero;
            uICameraData.m_FixedLayerParent = goTmp.transform;
            goTmp = new GameObject("Normal");
            goTmp.layer = LayerMask.NameToLayer("UI");
            goTmp.transform.SetParent(canvas.transform);
            goTmp.transform.localScale = Vector3.one;
            rtTmp = goTmp.AddComponent<RectTransform>();
            rtTmp.anchorMax = new Vector2(1, 1);
            rtTmp.anchorMin = new Vector2(0, 0);
            rtTmp.anchoredPosition3D = Vector3.zero;
            rtTmp.sizeDelta = Vector2.zero;
            uICameraData.m_NormalLayerParent = goTmp.transform;
            goTmp = new GameObject("TopBar");
            goTmp.layer = LayerMask.NameToLayer("UI");
            goTmp.transform.SetParent(canvas.transform);
            goTmp.transform.localScale = Vector3.one;
            rtTmp = goTmp.AddComponent<RectTransform>();
            rtTmp.anchorMax = new Vector2(1, 1);
            rtTmp.anchorMin = new Vector2(0, 0);
            rtTmp.anchoredPosition3D = Vector3.zero;
            rtTmp.sizeDelta = Vector2.zero;
            uICameraData.m_TopbarLayerParent = goTmp.transform;
            goTmp = new GameObject("PopUp");
            goTmp.layer = LayerMask.NameToLayer("UI");
            goTmp.transform.SetParent(canvas.transform);
            goTmp.transform.localScale = Vector3.one;
            rtTmp = goTmp.AddComponent<RectTransform>();
            rtTmp.anchorMax = new Vector2(1, 1);
            rtTmp.anchorMin = new Vector2(0, 0);
            rtTmp.anchoredPosition3D = Vector3.zero;
            rtTmp.sizeDelta = Vector2.zero;
            uICameraData.m_PopUpLayerParent = goTmp.transform;
            UILayerManager.UICameraList.Add(uICameraData);
            //重新保存
            ReSaveUIManager(UIManagerGo);
        }

        /// <summary>
        /// 保存UIManager
        /// </summary>
        /// <param name="UIManagerGo"></param>
        static void ReSaveUIManager(GameObject UIManagerGo)
        {
            string Path = UIEditorConstant.S_UiTemplatePath + "/UIManager/UIManager.prefab";
            EditorUtil.CreatFilePath(Application.dataPath + "/" + Path);
            PrefabUtility.SaveAsPrefabAssetAndConnect(UIManagerGo, "Assets/" + Path, InteractionMode.UserAction);
            ProjectWindowUtil.ShowCreatedAsset(UIManagerGo);
        }

        #region 创建ui预制体

        /// <summary>
        /// 创建ui预制体
        /// </summary>
        public static void CreateUIPrefab(string UIWindowName, string UIcameraKey, UIType UIType,
            UILayerManager UILayerManager, bool isAutoCreatePrefab)
        {
            GameObject uiGo = new GameObject(UIWindowName);
            string assem = UIEditorConstant.S_AppNamespace + "." + UIWindowName + "," +
                           UIEditorConstant.S_UIAssemblyName +
                           ", Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"; // typeof(HGT.AppComponent.AppBoot).Assembly.FullName;
            Type type = Type.GetType(assem, false);
            UIWindowBase uiBaseTmp = uiGo.AddComponent(type) as UIWindowBase;
            uiGo.layer = LayerMask.NameToLayer("UI");
            uiBaseTmp.m_UIType = UIType;
            uiBaseTmp.cameraKey = UIcameraKey;
            Canvas canvas = uiGo.AddComponent<Canvas>();

            uiGo.AddComponent<GraphicRaycaster>();
            RectTransform ui = uiGo.GetComponent<RectTransform>();
            ui.sizeDelta = Vector2.zero;
            ui.anchorMin = Vector2.zero;
            ui.anchorMax = Vector2.one;
            GameObject BgGo = new GameObject("BG");
            BgGo.layer = LayerMask.NameToLayer("UI");
            RectTransform Bg = BgGo.AddComponent<RectTransform>();
            Bg.SetParent(ui);
            Bg.sizeDelta = Vector2.zero;
            Bg.anchorMin = Vector2.zero;
            Bg.anchorMax = Vector2.one;
            GameObject rootGo = new GameObject("root");
            rootGo.layer = LayerMask.NameToLayer("UI");
            RectTransform root = rootGo.AddComponent<RectTransform>();
            root.SetParent(ui);
            root.sizeDelta = Vector2.zero;
            root.anchorMin = Vector2.zero;
            root.anchorMax = Vector2.one;
            uiBaseTmp.m_bgMask = BgGo;
            uiBaseTmp.m_uiRoot = rootGo;

            if (UILayerManager)
            {
                UILayerManager.SetLayer(uiBaseTmp, UIcameraKey);
            }
            
            if (EditorUtil.isExistSortingLayer(UIType.ToString()))
            {
                canvas.overrideSorting = true;
                canvas.sortingLayerName = UIType.ToString();
            }

            if (isAutoCreatePrefab)
            {
                string Path = UIEditorConstant.S_ResFolderModePath + "/UI/" + UIWindowName + "/" + UIWindowName +
                              ".prefab";
                EditorUtil.CreatFilePath(Application.dataPath + "/" + Path);
                PrefabUtility.SaveAsPrefabAssetAndConnect(uiGo, "Assets/" + Path, InteractionMode.UserAction);
                {
                    // 选中
                    Selection.activeObject = uiGo;
                    EditorGUIUtility.PingObject(uiGo);
                    GUIUtility.keyboardControl = 0;
                    GUIUtility.hotControl = 0;
                }
            }

            ProjectWindowUtil.ShowCreatedAsset(uiGo);
        }

        /// <summary>
        /// 创建热更ui预制体
        /// </summary>
        public static void CreateHotfixUIPrefab(string UIWindowName, string UIcameraKey, UIType UIType,
            UILayerManager UILayerManager, bool isAutoCreatePrefab)
        {
            GameObject uiGo = new GameObject(UIWindowName);
            string assem = UIEditorConstant.S_AppNamespace + "." + "HotfixWindow" + "," +
                           UIEditorConstant.S_HotfixUIAssemblyName +
                           ", Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"; // typeof(HGT.AppComponent.AppBoot).Assembly.FullName;
            Type type = Type.GetType(assem, false);
            UIWindowBase uiBaseTmp = uiGo.AddComponent(type) as UIWindowBase;
            uiGo.layer = LayerMask.NameToLayer("UI");
            uiBaseTmp.m_UIType = UIType;
            Canvas canvas = uiGo.AddComponent<Canvas>();

            if (EditorUtil.isExistSortingLayer(UIType.ToString()))
            {
                canvas.overrideSorting = true;
                canvas.sortingLayerName = UIType.ToString();
            }

            uiGo.AddComponent<GraphicRaycaster>();
            RectTransform ui = uiGo.GetComponent<RectTransform>();
            ui.sizeDelta = Vector2.zero;
            ui.anchorMin = Vector2.zero;
            ui.anchorMax = Vector2.one;
            GameObject BgGo = new GameObject("BG");
            BgGo.layer = LayerMask.NameToLayer("UI");
            RectTransform Bg = BgGo.AddComponent<RectTransform>();
            Bg.SetParent(ui);
            Bg.sizeDelta = Vector2.zero;
            Bg.anchorMin = Vector2.zero;
            Bg.anchorMax = Vector2.one;
            GameObject rootGo = new GameObject("root");
            rootGo.layer = LayerMask.NameToLayer("UI");
            RectTransform root = rootGo.AddComponent<RectTransform>();
            root.SetParent(ui);
            root.sizeDelta = Vector2.zero;
            root.anchorMin = Vector2.zero;
            root.anchorMax = Vector2.one;
            uiBaseTmp.m_bgMask = BgGo;
            uiBaseTmp.m_uiRoot = rootGo;

            if (UILayerManager)
            {
                UILayerManager.SetLayer(uiBaseTmp);
            }

            MethodInfo magicMethod = type.GetMethod("SetHotfixWindowName");
            object magicValue = magicMethod.Invoke(uiBaseTmp, new object[] { UIWindowName });

            if (isAutoCreatePrefab)
            {
                string Path = UIEditorConstant.S_ResFolderModePath + "/UI/" + UIWindowName + "/" + UIWindowName +
                              ".prefab";
                EditorUtil.CreatFilePath(Application.dataPath + "/" + Path);
                PrefabUtility.SaveAsPrefabAssetAndConnect(uiGo, "Assets/" + Path, InteractionMode.UserAction);
                {
                    // 选中
                    var aa = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/" + Path);
                    Selection.activeObject = aa;
                    EditorGUIUtility.PingObject(aa);
                    GUIUtility.keyboardControl = 0;
                    GUIUtility.hotControl = 0;
                }
            }

            ProjectWindowUtil.ShowCreatedAsset(uiGo);
        }

        #endregion

        #region 创建UI脚本

        /// <summary>
        /// 按照模板创建UI窗口脚本
        /// </summary>
        /// <param name="UIWindowName"></param>
        public static void CreatUIScript(string UIWindowName, string description)
        {
            string LoadPath = UIEditorConstant.S_UiTemplatePath +
                              "/UIWindowClassTemplate";
            string UItemplate = Resources.Load<TextAsset>(LoadPath)?.text;
            
            string SavePath = Application.dataPath + "/" + UIEditorConstant.S_UiWindowClassPath + "/" + UIWindowName +
                              "/" + UIWindowName + ".cs";
            
            string classContent = UItemplate.Replace("{0}", description);
            classContent = classContent.Replace("{1}", UIEditorConstant.S_AppNamespace);
            classContent = classContent.Replace("{2}", UIWindowName);
            EditorUtil.WriteStringByFile(SavePath, classContent);
            string partialLoadPath = UIEditorConstant.S_UiTemplatePath +
                                     "/UIWindowClassObservedTemplate";
            string partialUItemplate = Resources.Load<TextAsset>(partialLoadPath)?.text;
            
            
            string partialSavePath = Application.dataPath + "/" + UIEditorConstant.S_UiWindowClassPath + "/" +
                                     UIWindowName + "/" + UIWindowName + ".Auto.cs";
            
            string partialclassContent = partialUItemplate.Replace("{0}", "这是关注的字段和获取组件");
            partialclassContent = partialclassContent.Replace("{1}", UIEditorConstant.S_AppNamespace);
            partialclassContent = partialclassContent.Replace("{2}", UIWindowName);
            partialclassContent = partialclassContent.Replace("{3}", string.Empty);
            partialclassContent = partialclassContent.Replace("{4}", string.Empty);
            partialclassContent = partialclassContent.Replace("{5}", string.Empty);
            partialclassContent = partialclassContent.Replace("{6}", string.Empty);
            partialclassContent = partialclassContent.Replace("{7}", string.Empty);
            EditorUtil.WriteStringByFile(partialSavePath, partialclassContent);
            string editorLoadPath = UIEditorConstant.S_UiTemplatePath +
                                    "/UIWindowComponentEditorTemplate";
            string editorUItemplate = Resources.Load<TextAsset>(editorLoadPath)?.text;
            
            string editorSavePath = Application.dataPath + "/" +
                                    UIEditorConstant.S_UiWindowClassPath.Replace("Scripts", "ScriptsEditor") + "/" +
                                    UIWindowName + "/" + UIWindowName + "ComponentEditor.cs";

            string editorclassContent = editorUItemplate.Replace("{0}", "这是 CustomEditor");
            editorclassContent = editorclassContent.Replace("{1}", UIEditorConstant.S_AppNamespace + "Editor");
            editorclassContent = editorclassContent.Replace("{2}", UIWindowName);
            editorclassContent = editorclassContent.Replace("{3}", UIWindowName + "ComponentEditor");
            // TODO Editor的UI代码暂时注释掉
            //EditorUtil.WriteStringByFile(editorSavePath, editorclassContent);
            AssetDatabase.Refresh();
        }


        /// <summary>
        /// 按照模板创建UI窗口热更脚本
        /// </summary>
        /// <param name="UIWindowName"></param>
        public static void CreatHotfixUIScript(string UIWindowName, string description)
        {
            string LoadPath = Application.dataPath + "/" + UIEditorConstant.S_HotfixUiTemplatePath +
                              "/HotfixUIWindowClassTemplate.txt";
            string SavePath = Application.dataPath + "/" + UIEditorConstant.S_HotfixUiWindowClassPath + "/" +
                              UIWindowName + "/" + UIWindowName + ".cs";
            string UItemplate = EditorUtil.ReadStringByFile(LoadPath);
            string classContent = UItemplate.Replace("{0}", description);
            classContent = classContent.Replace("{1}", UIEditorConstant.S_HotfixNamespace);
            classContent = classContent.Replace("{2}", UIWindowName);
            EditorUtil.WriteStringByFile(SavePath, classContent);
            string partialLoadPath = Application.dataPath + "/" + UIEditorConstant.S_HotfixUiTemplatePath +
                                     "/HotfixUIWindowClassObservedTemplate.txt";
            string partialSavePath = Application.dataPath + "/" + UIEditorConstant.S_HotfixUiWindowClassPath + "/" +
                                     UIWindowName + "/" + UIWindowName + ".Auto.cs";
            string partialUItemplate = EditorUtil.ReadStringByFile(partialLoadPath);
            string partialclassContent = partialUItemplate.Replace("{0}", "这是关注的字段和获取组件");
            partialclassContent = partialclassContent.Replace("{1}", UIEditorConstant.S_HotfixNamespace);
            partialclassContent = partialclassContent.Replace("{2}", UIWindowName);
            partialclassContent = partialclassContent.Replace("{3}", string.Empty);
            partialclassContent = partialclassContent.Replace("{4}", string.Empty);
            partialclassContent = partialclassContent.Replace("{5}", string.Empty);
            partialclassContent = partialclassContent.Replace("{6}", string.Empty);
            partialclassContent = partialclassContent.Replace("{7}", string.Empty);
            EditorUtil.WriteStringByFile(partialSavePath, partialclassContent);
            AssetDatabase.Refresh();
        }


        #endregion

        [MenuItem("GameObject/UI/快速构建字段和组件")]
        public static void CreateObservedScript()
        {
            UIWindowBase uiGo = Selection.gameObjects[0].GetComponent<UIWindowBase>();

            if (uiGo == null)
            {
                return;
            }

            string UIWindowName = uiGo.name;
            string members = uiGo.WindowEditorFastGetMembers();
            string components = uiGo.WindowEditorFastGetComponents();
            string btnCodeSub = uiGo.WindowEditorFastGetBtnScripts();
            string btnCodeUnSub = uiGo.WindowEditorFastGetBtnScripts().Replace("AddListener", "RemoveListener");
            string btnFunc = uiGo.WindowEditorFastGetBtnFunctionScripts();

            string partialLoadPath = UIEditorConstant.S_UiTemplatePath +
                                     "/UIWindowClassObservedTemplate";
            string partialUItemplate = Resources.Load<TextAsset>(partialLoadPath)?.text;
            
            string partialSavePath = Application.dataPath + "/" + UIEditorConstant.S_UiWindowClassPath + "/" +
                                     UIWindowName + "/" + UIWindowName + ".Auto.cs";

            string partialclassContent = partialUItemplate.Replace("{0}", "这是关注的字段和获取组件");
            partialclassContent = partialclassContent.Replace("{1}", UIEditorConstant.S_AppNamespace);
            partialclassContent = partialclassContent.Replace("{2}", UIWindowName);
            partialclassContent = partialclassContent.Replace("{3}", members);
            partialclassContent = partialclassContent.Replace("{4}", components);
            partialclassContent = partialclassContent.Replace("{5}", btnCodeSub);
            partialclassContent = partialclassContent.Replace("{6}", btnCodeUnSub);
            partialclassContent = partialclassContent.Replace("{7}", btnFunc);
            EditorUtil.WriteStringByFile(partialSavePath, partialclassContent);
            AssetDatabase.Refresh();
        }

        [MenuItem("GameObject/UI/快速构建ComponentEditor")]
        public static void CreateConmponentEditorScript()
        {
            UIBase uiGo = Selection.gameObjects[0].GetComponent<UIBase>();

            if (uiGo == null)
            {
                return;
            }

            string UIWindowName = uiGo.name;

            string editorLoadPath = UIEditorConstant.S_UiTemplatePath +
                                    "/UIWindowComponentEditorTemplate";
            string editorUItemplate = Resources.Load<TextAsset>(editorLoadPath)?.text;
            
            string editorSavePath = Application.dataPath + "/" +
                                    UIEditorConstant.S_UiWindowClassPath.Replace("Scripts", "ScriptsEditor") + "/" +
                                    UIWindowName + "/" + UIWindowName + "ComponentEditor.cs";

            string editorclassContent = editorUItemplate.Replace("{0}", "这是 CustomEditor");
            editorclassContent = editorclassContent.Replace("{1}", UIEditorConstant.S_AppNamespace + "Editor");
            editorclassContent = editorclassContent.Replace("{2}", UIWindowName);
            editorclassContent = editorclassContent.Replace("{3}", UIWindowName + "ComponentEditor");
            EditorUtil.WriteStringByFile(editorSavePath, editorclassContent);
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 按照模板创建Lua窗口脚本
        /// </summary>
        /// <param name="UIWindowName"></param>
        public static void CreatUILuaScript(string UIWindowName)
        {
            string LoadPath = UIEditorConstant.S_UiTemplatePath +
                              "/UILuaScriptTemplate";
            string UItemplate = Resources.Load<TextAsset>(LoadPath)?.text;
            
            string SavePath = Application.dataPath + "/Resources/Lua/UI/Lua" + UIWindowName + ".txt";

            string classContent = UItemplate.Replace("{0}", UIWindowName);
            EditorUtil.WriteStringByFile(SavePath, classContent);
            AssetDatabase.Refresh();
        }

        /*
        /// <summary>
        /// 创建新手引导预设
        /// </summary>
        public static void CreateGuideWindow()
        {
            string UIWindowName = "GuideWindow";
            UIType UIType = UIType.TopBar;
            GameObject uiGo = new GameObject(UIWindowName);
            Debug.Log("uiGo:" + uiGo.name);
            string assem = UIEditorConstant.S_AppNamespace + ".GuideWindow" + "," + UIEditorConstant.S_AppNamespace + ", Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";// typeof(HGT.AppComponent.AppBoot).Assembly.FullName;
            Type guideClass = Type.GetType(assem, false);
            GuideWindowBase guideBaseTmp = uiGo.AddComponent(guideClass) as GuideWindowBase;
            uiGo.layer = LayerMask.NameToLayer("UI");
            guideBaseTmp.m_UIType = UIType;
            Canvas can = uiGo.AddComponent<Canvas>();
            uiGo.AddComponent<GraphicRaycaster>();
            can.overrideSorting = true;
            can.sortingLayerName = "Guide";
            RectTransform ui = uiGo.GetComponent<RectTransform>();
            ui.sizeDelta = Vector2.zero;
            ui.anchorMin = Vector2.zero;
            ui.anchorMax = Vector2.one;
            GameObject BgGo = new GameObject("BG");
            BgGo.layer = LayerMask.NameToLayer("UI");
            RectTransform Bg = BgGo.AddComponent<RectTransform>();
            Bg.SetParent(ui);
            Bg.sizeDelta = Vector2.zero;
            Bg.anchorMin = Vector2.zero;
            Bg.anchorMax = Vector2.one;
            GameObject rootGo = new GameObject("root");
            rootGo.layer = LayerMask.NameToLayer("UI");
            RectTransform root = rootGo.AddComponent<RectTransform>();
            root.SetParent(ui);
            root.sizeDelta = Vector2.zero;
            root.anchorMin = Vector2.zero;
            root.anchorMax = Vector2.one;
            GameObject mask = new GameObject("mask");
            mask.layer = LayerMask.NameToLayer("UI");
            RectTransform maskrt = mask.AddComponent<RectTransform>();
            Image img = mask.AddComponent<Image>();
            mask.AddComponent<Button>();
            img.color = new Color(0, 0, 0, 0.75f);
            maskrt.SetParent(root);
            maskrt.sizeDelta = Vector2.zero;
            maskrt.anchorMin = Vector2.zero;
            maskrt.anchorMax = Vector2.one;
            guideBaseTmp.m_objectList.Add(mask);
            GameObject tips = new GameObject("Tips");
            tips.layer = LayerMask.NameToLayer("UI");
            RectTransform tipsrt = tips.AddComponent<RectTransform>();
            tipsrt.SetParent(root);
            guideBaseTmp.m_objectList.Add(tips);
            GameObject Text_tips = new GameObject("Text_tip");
            Text_tips.layer = LayerMask.NameToLayer("UI");
            RectTransform txt_tipsrt = Text_tips.AddComponent<RectTransform>();
            Text text = Text_tips.AddComponent<Text>();
            txt_tipsrt.SetParent(tipsrt);
            guideBaseTmp.m_objectList.Add(Text_tips);
            guideBaseTmp.m_bgMask = BgGo;
            guideBaseTmp.m_uiRoot = rootGo;
            string Path = UIEditorConstant.S_ResFolderModeFolder + "/UI/GuideWindow/GuideWindow.prefab";
            EditorUtil.CreatFilePath(Application.dataPath + "/" + Path);
            PrefabUtility.SaveAsPrefabAssetAndConnect(uiGo, "Assets/" + Path, InteractionMode.UserAction);
            ProjectWindowUtil.ShowCreatedAsset(uiGo);
            {
                // 选中
                var aa = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/" + Path);
                Selection.activeObject = aa;
                EditorGUIUtility.PingObject(aa);
                GUIUtility.keyboardControl = 0;
                GUIUtility.hotControl = 0;
            }
        }
        */
    }
}

