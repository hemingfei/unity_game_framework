//
//  UIManager.cs 
//
//  Author: HGT 
//
//  Copyright (c) 2021 hegametech.com 
//

using UnityEngine;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;
using Cysharp.Threading.Tasks;
using GameFramework;
using UnityGameFramework.Runtime;
using Object = UnityEngine.Object;

namespace CustomGameFramework.Runtime
{
    [RequireComponent(typeof(UIStackManager))]
    [RequireComponent(typeof(UILayerManager))]
    public partial class UIManager : GameFrameworkComponent, IReference
    {
        private static GameObject s_UIManagerGo;
        private static UILayerManager s_UILayerManager; //UI层级管理器
        private static UIStackManager s_UIStackManager; //UI栈管理器

        static public Dictionary<string, List<UIWindowBase>> s_UIs = new Dictionary<string, List<UIWindowBase>>(); //打开的UI

        static public Dictionary<string, List<UIWindowBase>> s_hideUIs = new Dictionary<string, List<UIWindowBase>>(); //隐藏的UI

        static public Dictionary<string, int> s_uiRestrict = new Dictionary<string, int>(); // 限制窗口最大个数
        static public Dictionary<string, string> s_uiCustomPath = new Dictionary<string, string>(); // 自定义窗口的加载路径

        static public Dictionary<GameObject, Object> s_resourceUIs = new Dictionary<GameObject, Object>();

        #region 设定值
        private const int s_DefaultUINum = 1; // 同一个UI的同时打开个数，默认值
        
        /// <summary>
        /// UI prefab 在 resources
        /// </summary>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public static string GetUIPathInResources(string assetName)
        {
            return Utility.Text.Format("UI/{0}/{1}", assetName, assetName);
        }

        /// <summary>
        /// UI prefab 在 assetbundle
        /// </summary>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public static string GetUIPath(string assetName)
        {
            return Utility.Text.Format("Assets/Res/AssetBundle/UI/{0}/{1}.prefab", assetName, assetName);
        }
        
        // 框架内置的UI，Toast等
        public static string GetUIBuildinPath(string assetName)
        {
            return Utility.Text.Format("Assets/Res/AssetBundle/UIBuildin/{0}/{1}.prefab", assetName, assetName);
        }
        #endregion
        
        #region GameEntry 组件获取

        private void Start()
        {
            UIEventMgr = UnityGameFramework.Runtime.GameEntry.GetComponent<EventComponent>();
            ResourceMgr = UnityGameFramework.Runtime.GameEntry.GetComponent<ResourceComponent>();
        }

        public static EventComponent UIEventMgr { get; private set; }

        public static ResourceComponent ResourceMgr { get; private set; }

        #endregion

        #region 初始化

        static bool isInit = false;

        public static void Init()
        {
            if (!isInit)
            {
                isInit = true;
                GameObject instance = GameObject.Find("Game Framework/CustomFramework/UI");

                if (instance == null)
                {
                    instance = GameObject.Instantiate(Resources.Load("UI/UIManager") as GameObject, Vector3.zero,
                        Quaternion.identity);
                    instance.name = "UI";
                }

                UIManagerGo = instance;
                s_UILayerManager = instance.GetComponent<UILayerManager>();
                s_UIStackManager = instance.GetComponent<UIStackManager>();
            }
        }

        public static UILayerManager UILayerManager
        {
            get
            {
                if (s_UILayerManager == null)
                {
                    Init();
                }

                return s_UILayerManager;
            }
            set { s_UILayerManager = value; }
        }

        public static UIStackManager UIStackManager
        {
            get
            {
                if (s_UIStackManager == null)
                {
                    Init();
                }

                return s_UIStackManager;
            }
            set { s_UIStackManager = value; }
        }

        public static GameObject UIManagerGo
        {
            get
            {
                if (s_UIManagerGo == null)
                {
                    Init();
                }

                return s_UIManagerGo;
            }
            set { s_UIManagerGo = value; }
        }

        #endregion

        #region UICamera

        public static string[] GetCameraNames()
        {
            string[] list = new string[UILayerManager.UICameraList.Count];

            for (int i = 0; i < UILayerManager.UICameraList.Count; i++)
            {
                list[i] = UILayerManager.UICameraList[i].m_key;
            }

            return list;
        }

        public static Camera GetCamera(string CameraKey = null)
        {
            var data = UILayerManager.GetUICameraDataByKey(CameraKey);
            return data.m_camera;
        }

        public static Canvas GetCanvas(string CameraKey = null)
        {
            var data = UILayerManager.GetUICameraDataByKey(CameraKey);
            return data.m_canvas;
        }

        /// <summary>
        /// 将一个UI移动到另一个UICamera下
        /// </summary>
        /// <param name="ui"></param>
        /// <param name="cameraKey"></param>
        public static void ChangeUICamera(UIWindowBase ui, string cameraKey)
        {
            UILayerManager.SetLayer(ui, cameraKey);
        }

        /// <summary>
        /// 将一个UI重新放回它原本的UICamera下
        /// </summary>
        /// <param name="ui"></param>
        /// <param name="cameraKey"></param>
        public static void ResetUICamera(UIWindowBase ui)
        {
            UILayerManager.SetLayer(ui, ui.cameraKey);
        }

        #endregion

        #region 非静态的常用操作
        /// <summary>
        /// 打开窗口
        /// </summary>
        /// <param name="callback">打开动画播放完毕的回调</param>
        /// <param name="objs">自定义参数</param>
        /// <typeparam name="T">窗口类型</typeparam>
        /// <returns>窗口类型，可能为null</returns>
        public async UniTask<UIWindowBase> OpenUIForm<T>(UIEventCallBack callback = null, params object[] objs) where T : UIWindowBase
        {
            return await OpenUIWindow<T>(callback, objs);
        }
        
        /// <summary>
        /// 关闭窗口
        /// </summary>
        /// <param name="isPlayAnim">是否播放关闭动画</param>
        /// <param name="callback">关闭窗口完毕的回调</param>
        /// <param name="objs">自定义参数</param>
        /// <typeparam name="T">窗口类型</typeparam>
        public void CloseUIForm<T>(bool isPlayAnim = true, UIEventCallBack callback = null, params object[] objs)
            where T : UIWindowBase
        {
            CloseUIWindow<T>(isPlayAnim, callback, objs);
        }
        
        /// <summary>
        /// 获取打开了的UI窗口
        /// </summary>
        /// <typeparam name="T">窗口类型</typeparam>
        /// <returns>窗口类型，如果没有已打开的这个窗口则返回null</returns>
        public T GetUIForm<T>() where T : UIWindowBase
        {
            return GetUIWindow<T>();
        }

        public void CloseAndDestroyUIForm<T>(bool isPlayAnim = true, UIEventCallBack callback = null,
            params object[] objs) where T : UIWindowBase
        {
            CloseAndDestroyUIWindow<T>(isPlayAnim, callback, objs);
        }
        
        public void DestroyClosedUIForm<T>()
        {
            DestroyClosedUIWindow<T>();
        }


        public void DestroyAllClosedUIForms()
        {
            DestroyAllClosedUIWindows();
        }

        public void DestroyUIForm(UIWindowBase UI)
        {
            DestroyUIWindow(UI);
        }
        #endregion

        #region UI的创建、打开、关闭方法

        #region 缩略

        public static UIWindowBase ShowUI<T>()
        {
            return ShowUI(typeof(T).Name);
        }

        public static UIWindowBase HideUI<T>()
        {
            return HideUI(typeof(T).Name);
        }

        public static void ShowOtherUI<T>()
        {
            ShowOtherUI(typeof(T).Name);
        }

        public static void HideOtherUI<T>()
        {
            HideOtherUI(typeof(T).Name);
        }

        #endregion

        #region 创建

        #region 公共方法
        
        /// <summary>
        /// 创建UI,若为创建过则从ab包异步加载，肯定会新创建一个放入HideUI列表
        /// </summary>
        /// <param name="T">UIWindowBase</param>
        /// <returns>返回加载此UI资源的 Asset，若为 null 则说明早已加载过此资源</returns>
        public static async UniTask<T> CreateUIWindow<T>() where T : UIWindowBase
        {
            return (T)await CreateUIWindow(typeof(T).Name);
        }
        
        /// <summary>
        /// 创建UI,若为创建过则从resouces异步加载，肯定会新创建一个放入HideUI列表
        /// </summary>
        /// <param name="T">UIWindowBase</param>
        /// <returns>返回加载此UI资源的 ResourceRequest，若为 null 则说明早已加载过此资源</returns>
        public static async UniTask<T> CreateUIWindowFromResources<T>() where T : UIWindowBase
        {
            return (T)await CreateUIWindowFromResources(typeof(T).Name);
        }
        
        public static T CreateUIWindowSync<T>() where T : UIWindowBase
        {
            return (T)CreateUIWindowSync(typeof(T).Name);
        }
        
        /// <summary>
        /// 创建UI,若为创建过则从resouces同步加载，肯定会新创建一个放入HideUI列表
        /// </summary>
        /// <param name="T">UIWindowBase</param>
        /// <returns>返回创建的UI</returns>
        public static T CreateUIWindowFromResourcesSync<T>() where T : UIWindowBase
        {
            return (T)CreateUIWindowFromResourcesSync(typeof(T).Name);
        }
        
        #endregion
        
        #region 异步创建
        
        /// <summary>
        /// 创建UI,若为创建过则从ab包异步加载，肯定会新创建一个放入HideUI列表
        /// </summary>
        /// <param name="UIName">UI名</param>
        /// <returns>返回加载此UI资源的 Asset，若为 null 则说明早已加载过此资源</returns>
        public static async UniTask<UIWindowBase> CreateUIWindow(string UIName)
        {
            return await CreateUIWindow(UIName, true, true);
        }
        
        /// <summary>
        /// 创建UI,若为创建过则从resouces异步加载，肯定会新创建一个放入HideUI列表
        /// </summary>
        /// <param name="UIName">UI名</param>
        /// <returns>返回加载此UI资源的 ResourceRequest，若为 null 则说明早已加载过此资源</returns>
        public static async UniTask<UIWindowBase> CreateUIWindowFromResources(string UIName)
        {
            return await CreateUIWindow(UIName, false, true);
        }
        
        #endregion
        
        #region 同步创建
        
        /// <summary>
        /// 创建UI,若为创建过则从ab包异步加载，肯定会新创建一个放入HideUI列表
        /// </summary>
        /// <param name="UIName">UI名</param>
        /// <returns>返回加载此UI资源的 Asset，若为 null 则说明早已加载过此资源</returns>
        public static UIWindowBase CreateUIWindowSync(string UIName)
        {
            return CreateUIWindow(UIName, true, false).GetAwaiter().GetResult();
        }
        
        /// <summary>
        /// 创建UI,若为创建过则从resouces异步加载，肯定会新创建一个放入HideUI列表
        /// </summary>
        /// <param name="UIName">UI名</param>
        /// <returns>返回加载此UI资源的 ResourceRequest，若为 null 则说明早已加载过此资源</returns>
        public static UIWindowBase CreateUIWindowFromResourcesSync(string UIName)
        {
            return CreateUIWindow(UIName, false, false).GetAwaiter().GetResult();
        }
        
        #endregion
        
        #endregion

        #region 打开

        #region 公共方法

        /// <summary>
        /// 打开UI,肯定会打开。 若没有创建则会自动创建， 从assetbundle里异步创建
        /// </summary>
        /// <param name="T">UIWindowBase</param>
        /// <param name="callback">动画播放完毕回调</param>
        /// <param name="objs">回调传参</param>`
        /// <returns>返回打开的UI,返回null则说明需要从ab包异步加载完成后再打开</returns>
        public static async UniTask<UIWindowBase> OpenUIWindow<T>(UIEventCallBack callback = null, params object[] objs) where T : UIWindowBase
        {
            return await OpenUIWindow(typeof(T).Name, callback, objs);
        }

        /// <summary>
        /// 打开UI,肯定会打开。 若没有创建则会自动创建， 从assetbundle里同步创建
        /// </summary>
        /// <param name="T">UIWindowBase</param>
        /// <param name="callback">动画播放完毕回调</param>
        /// <param name="objs">回调传参</param>`
        /// <returns>返回打开的UI,返回null则说明需要从Asset.complete完成后再打开</returns>
        public static T OpenUIWindowSync<T>(UIEventCallBack callback = null, params object[] objs) where T : UIWindowBase
        {
            return (T)OpenUIWindowSync(typeof(T).Name, callback, objs);
        }

        /// <summary>
        /// 打开UI,肯定会打开。 若没有创建则会自动创建， 从Resources里同步创建
        /// </summary>
        /// <param name="T">UIWindowBase</param>
        /// <param name="callback">动画播放完毕回调</param>
        /// <param name="objs">回调传参</param>`
        /// <returns>返回打开的UI,返回null则说明需要从Asset.complete完成后再打开</returns>
        public static T OpenUIWindowFromResourcesSync<T>(UIEventCallBack callback = null, params object[] objs)
            where T : UIWindowBase
        {
            return (T)OpenUIWindowFromResourcesSync(typeof(T).Name, callback, objs);
        }

        /// <summary>
        /// 打开UI,肯定会打开。 若没有创建则会自动创建， 从Resources里异步创建
        /// </summary>
        /// <param name="T">UIWindowBase</param>
        /// <param name="callback">动画播放完毕回调</param>
        /// <param name="objs">回调传参</param>`
        /// <returns>返回打开的UI,返回null则说明需要从Asset.complete完成后再打开</returns>
        public static async UniTask<UIWindowBase> OpenUIWindowFromResourcesAsync<T>(UIEventCallBack callback = null, params object[] objs)
            where T : UIWindowBase
        {
            return await OpenUIWindowFromResourcesAsync(typeof(T).Name, callback, objs);
        }

        #endregion

        #region 内部方法

        /// <summary>
        /// 打开UI,肯定会打开,若没有创建则会自动创建，默认从assetbundle异步创建并且之后打开
        /// </summary>
        /// <param name="UIName">UI名</param>
        /// <param name="callback">动画播放完毕回调</param>
        /// <param name="objs">回调传参</param>`
        /// <returns>返回打开的UI,返回null则说明需要从ab包异步加载完成后再打开</returns>
        public static async UniTask<UIWindowBase> OpenUIWindow(string UIName, UIEventCallBack callback = null, params object[] objs)
        {
            return await OpenUIWindow(true, true, UIName, callback, objs);
        }

        public static UIWindowBase OpenUIWindowSync(string UIName, UIEventCallBack callback = null, params object[] objs)
        {
            return OpenUIWindow(true, false, UIName, callback, objs).GetAwaiter().GetResult();
        }

        public static UIWindowBase OpenUIWindowFromResourcesSync(string UIName, UIEventCallBack callback = null,
            params object[] objs)
        {
            return OpenUIWindow(false, false, UIName, callback, objs).GetAwaiter().GetResult();
        }

        public static async UniTask<UIWindowBase> OpenUIWindowFromResourcesAsync(string UIName, UIEventCallBack callback = null,
            params object[] objs)
        {
            return await OpenUIWindow(false, true, UIName, callback, objs);
        }

        #endregion

        #endregion

        #region 关闭
        
        /// <summary>
        /// 关闭UI
        /// </summary>
        /// <param name="T">UIWindowBase</param>
        /// <param name="isPlayAnim"></param>
        /// <param name="callback"></param>
        /// <param name="objs"></param>
        public static void CloseUIWindow<T>(bool isPlayAnim = true, UIEventCallBack callback = null, params object[] objs)
            where T : UIWindowBase
        {
            CloseUIWindow(typeof(T).Name, isPlayAnim, callback, objs);
        }

        /// <summary>
        /// 关闭UI
        /// </summary>
        public static void CloseUIWindow<T>(UIWindowBase UI, bool isPlayAnim = true, UIEventCallBack callback = null,
            params object[] objs)
        {
            CloseUIWindow(UI, isPlayAnim, callback, objs);
        }

        /// <summary>
        /// 关闭UI
        /// </summary>
        /// <param name="UIname"></param>
        /// <param name="isPlayAnim"></param>
        /// <param name="callback"></param>
        /// <param name="objs"></param>
        public static void CloseUIWindow(string UIname, bool isPlayAnim = true, UIEventCallBack callback = null,
            params object[] objs)
        {
            UIWindowBase ui = GetUI(UIname);

            if (ui == null)
            {
                Log.Error("CloseUIWindow Error UI ->" + UIname + "<-  not Exist!");
            }
            else
            {
                CloseUIWindow(ui, isPlayAnim, callback, objs);
            }
        }

        /// <summary>
        /// 关闭UI
        /// </summary>
        /// <param name="UI">目标UI</param>
        /// <param name="isPlayAnim">是否播放关闭动画</param>
        /// <param name="callback">动画播放完毕回调</param>
        /// <param name="objs">回调传参</param>


        /// <summary>
        /// 关闭全部UI
        /// </summary>
        public static void CloseAllUI(bool isPlayerAnim = false)
        {
            List<string> keys = new List<string>(s_UIs.Keys);

            for (int i = 0; i < keys.Count; i++)
            {
                var list = s_UIs[keys[i]].ToArray();

                foreach (var ui in list)
                {
                    CloseUIWindow(ui, isPlayerAnim);
                }
            }
        }

        #endregion
        
        #region 销毁
        
        #region 关闭并销毁 打开了的UI
        /// <summary>
        /// 关闭UI之后彻底销毁为跟从未打开过此UI一样
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void CloseAndDestroyUIWindow<T>(bool isPlayAnim = true, UIEventCallBack callback = null,
            params object[] objs) where T : UIWindowBase
        {
            CloseAndDestroyUIWindow(typeof(T).Name, isPlayAnim, callback, objs);
        }
        

        #endregion

        #region 销毁 关闭了的UI
        public static void DestroyClosedUIWindow<T>()
        {
            DestroyClosedUIWindow(typeof(T).Name);
        }
        
        public static void DestroyClosedUIWindow(string uiName)
        {
            if (s_hideUIs.ContainsKey(uiName) && s_hideUIs[uiName].Count > 0)
            {
                var uiList = s_hideUIs[uiName].ToArray();
                for (int i = 0; i < uiList.Length; i++)
                {
                    var ui = uiList[i];
                    if (ui != null)
                    {
                        DestroyUIWindow(ui);
                    }
                }
            }
        }
        
        #endregion
        
        #region 销毁 UIs
        
        public static void DestroyAllUI()
        {
            DestroyAllActiveUIWindows();
            DestroyAllHideUIWindows();
        }

        public static void DestroyAllClosedUIWindows()
        {
            DestroyAllHideUIWindows();
        }

        /// <summary>
        /// 删除所有隐藏的UI
        /// </summary>
        public static void DestroyAllHideUIWindows()
        {
            List<UIWindowBase> allHideUIs = new List<UIWindowBase>();
            foreach (List<UIWindowBase> uis in s_hideUIs.Values)
            {
                for (int i = 0; i < uis.Count; i++)
                {
                    allHideUIs.Add(uis[i]);
                }
            }

            var allHideUIArray = allHideUIs.ToArray();
            for (int i = 0; i < allHideUIArray.Length; i++)
            {
                var ui = allHideUIArray[i];
                DestroyUIWindow(ui);
            }

            s_hideUIs.Clear();
        }
        
        /// <summary>
        /// 删除所有打开的UI
        /// </summary>
        public static void DestroyAllActiveUIWindows()
        {
            List<UIWindowBase> allActiveUIs = new List<UIWindowBase>();
            foreach (List<UIWindowBase> uis in s_UIs.Values)
            {
                for (int i = 0; i < uis.Count; i++)
                {
                    allActiveUIs.Add(uis[i]);
                }
            }
            
            var allActiveUIArray = allActiveUIs.ToArray();
            for (int i = 0; i < allActiveUIArray.Length; i++)
            {
                var ui = allActiveUIArray[i];
                DestroyUIWindow(ui);
            }

            s_UIs.Clear();
        }


        
        #endregion


        #endregion

        #region 显示、隐藏打开的UI

        public static UIWindowBase ShowUI(string UIname)
        {
            UIWindowBase ui = GetUI(UIname);
            return ShowUI(ui);
        }

        private static UIWindowBase ShowUI(UIWindowBase ui)
        {
            ui.windowStatus = UIWindowBase.WindowStatus.Open;

            try
            {
                ui.Show();
                ui.OnShow();
            }
            catch (Exception e)
            {
                Log.Error(ui.UIName + " OnShow Exception: " + e.ToString());
            }
            UIEvent.FireNow(ui, UIEventType.OnShow); //派发OnShow事件
            return ui;
        }

        public static UIWindowBase HideUI(string UIname)
        {
            UIWindowBase ui = GetUI(UIname);
            return HideUI(ui);
        }

        private static UIWindowBase HideUI(UIWindowBase ui)
        {
            ui.windowStatus = UIWindowBase.WindowStatus.Hide;

            try
            {
                ui.Hide();
                ui.OnHide();
            }
            catch (Exception e)
            {
                Log.Error(ui.UIName + " OnShow Exception: " + e.ToString());
            }
            UIEvent.FireNow(ui, UIEventType.OnHide); //派发OnHide事件
            return ui;
        }

        public static void ShowOtherUI(string UIName)
        {
            List<string> keys = new List<string>(s_UIs.Keys);

            for (int i = 0; i < keys.Count; i++)
            {
                List<UIWindowBase> list = s_UIs[keys[i]];

                for (int j = 0; j < list.Count; j++)
                {
                    if (list[j].UIName != UIName)
                    {
                        ShowUI(list[j]);
                    }
                }
            }
        }

        public static void HideOtherUI(string UIName)
        {
            List<string> keys = new List<string>(s_UIs.Keys);

            for (int i = 0; i < keys.Count; i++)
            {
                List<UIWindowBase> list = s_UIs[keys[i]];

                for (int j = 0; j < list.Count; j++)
                {
                    if (list[j].UIName != UIName)
                    {
                        HideUI(list[j]);
                    }
                }
            }
        }

        #endregion

        #region UI动画

        //开始调用进入动画


        //开始调用退出动画


 

        #endregion

        #endregion
        
        #region 打开UI列表的管理

        public static T GetUIWindow<T>() where T : UIWindowBase
        {
            return (T)GetUI(typeof(T).Name);
        }

        public static UIWindowBase GetUI(string UIname)
        {
            if (!s_UIs.ContainsKey(UIname))
            {
                Log.Debug("!ContainsKey " + UIname);
                return null;
            }
            else
            {
                if (s_UIs[UIname].Count == 0)
                {
                    Log.Debug("s_UIs[UIname].Count == 0");
                    return null;
                }
                else
                {
                    //默认返回最后创建的那一个
                    return s_UIs[UIname][s_UIs[UIname].Count - 1];
                }
            }
        }

        static Regex uiKey = new Regex(@"(\S+)\d+");

        static UIWindowBase GetUIWindowByEventKey(string eventKey)
        {
            string UIname = uiKey.Match(eventKey).Groups[1].Value;

            if (!s_UIs.ContainsKey(UIname))
            {
                Log.Error("UIManager: GetUIWindowByEventKey error dont find UI name: ->" + eventKey + "<-  " + UIname);
            }

            List<UIWindowBase> list = s_UIs[UIname];

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].UIEventKey == eventKey)
                {
                    return list[i];
                }
            }

            Log.Error("UIManager: GetUIWindowByEventKey error dont find UI name: ->" + eventKey + "<-  " + UIname);
            return null;
        }

        static bool GetIsExits(UIWindowBase UI)
        {
            if (!s_UIs.ContainsKey(UI.UIName))
            {
                return false;
            }
            else
            {
                return s_UIs[UI.name].Contains(UI);
            }
        }

        static bool GetIsExits(string uiName)
        {
            if (!s_UIs.ContainsKey(uiName) || s_UIs[uiName].Count==0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        static void AddUI(UIWindowBase UI)
        {
            if (!s_UIs.ContainsKey(UI.name))
            {
                s_UIs.Add(UI.name, new List<UIWindowBase>());
            }

            s_UIs[UI.name].Add(UI);
            UI.Show();
        }

        static void RemoveUI(UIWindowBase UI)
        {
            if (UI == null)
            {
                Log.Error("UIManager: RemoveUI error UI is null: !");
                return;
            }

            if (!s_UIs.ContainsKey(UI.name))
            {
                Log.Error("UIManager: RemoveUI error dont find UI name: ->" + UI.name + "<-  " + UI);
            }

            if (!s_UIs[UI.name].Contains(UI))
            {
                Log.Error("UIManager: RemoveUI error dont find UI: ->" + UI.name + "<-  " + UI);
            }
            else
            {
                s_UIs[UI.name].Remove(UI);
            }
        }

        static int GetUIID(string UIname)
        {
            if (!s_UIs.ContainsKey(UIname))
            {
                return 0;
            }
            else
            {
                int id = s_UIs[UIname].Count;

                for (int i = 0; i < s_UIs[UIname].Count; i++)
                {
                    if (s_UIs[UIname][i].UIID == id)
                    {
                        id++;
                        i = 0;
                    }
                }

                return id;
            }
        }

        public static int GetNormalUICount()
        {
            return UIStackManager.m_normalStack.Count;
        }

        #endregion

        #region 隐藏UI列表的管理

        public static T GetHideUI<T>() where T : UIWindowBase
        {
            string UIname = typeof(T).Name;
            return (T)GetHideUI(UIname);
        }

        /// <summary>
        /// 获取一个隐藏的UI,如果有多个同名UI，则返回最后创建的那一个
        /// </summary>
        /// <param name="UIname">UI名</param>
        /// <returns></returns>
        public static UIWindowBase GetHideUI(string UIname)
        {
            if (!s_hideUIs.ContainsKey(UIname))
            {
                return null;
            }
            else
            {
                if (s_hideUIs[UIname].Count == 0)
                {
                    return null;
                }
                else
                {
                    UIWindowBase ui = s_hideUIs[UIname][s_hideUIs[UIname].Count - 1];
                    //默认返回最后创建的那一个
                    return ui;
                }
            }
        }

        static bool GetIsExitsHide(UIWindowBase UI)
        {
            if (!s_hideUIs.ContainsKey(UI.name))
            {
                return false;
            }
            else
            {
                return s_hideUIs[UI.name].Contains(UI);
            }
        }

        static void AddHideUI(UIWindowBase UI)
        {
            if (!s_hideUIs.ContainsKey(UI.name))
            {
                s_hideUIs.Add(UI.name, new List<UIWindowBase>());
            }

            s_hideUIs[UI.name].Add(UI);
            UI.Hide();
        }


        static void RemoveHideUI(UIWindowBase UI)
        {
            if (UI == null)
            {
                Log.Error("UIManager: RemoveUI error l_UI is null: !");
                return;
            }

            if (!s_hideUIs.ContainsKey(UI.name))
            {
                Log.Error("UIManager: RemoveUI error dont find: " + UI.name + "  " + UI);
            }

            if (!s_hideUIs[UI.name].Contains(UI))
            {
                Log.Error("UIManager: RemoveUI error dont find: " + UI.name + "  " + UI);
            }
            else
            {
                s_hideUIs[UI.name].Remove(UI);
            }
        }

        #endregion

        #region 流海屏相关

        private static float _safeScreenTopOffsetFloat = -1f;

        /// <summary>
        /// 刘海屏修正
        /// </summary>
        public static float SafeScreenTopOffset()
        {
            if (_safeScreenTopOffsetFloat < 0)
            {
                var rawHeight = Screen.height;
                var safeHeight = Screen.safeArea.yMax;
                _safeScreenTopOffsetFloat = rawHeight - safeHeight;
            }

            return _safeScreenTopOffsetFloat;
        }

        private static float _heightScale = -1;

        /// <summary>
        /// 屏幕宽度需要的比例缩放尺度
        /// </summary>
        /// <returns></returns>
        public static float GetSafeHeightScale()
        {
            if (_heightScale <= 0)
            {
                _heightScale = 1;
                var scaleFactor = GetCanvas().scaleFactor;
                var referenceScreenY = GetCanvas().GetComponent<UnityEngine.UI.CanvasScaler>().referenceResolution.y;
                var scaleWidth = Screen.height / referenceScreenY;
                if (scaleWidth > scaleFactor)
                {
                    _heightScale = scaleWidth / scaleFactor;
                }
            }

            if (_heightScale <= 0)
            {
                _heightScale = 1;
            }

            return _heightScale;
        }
        #endregion

        public void Clear()
        {
            DestroyAllUI();
            OnDestroy();
        }

        private void OnDestroy()
        {
            isInit = false;
            s_UIs.Clear();
            s_hideUIs.Clear();
            s_uiRestrict.Clear();
            s_uiCustomPath.Clear();
        }
    }
}
