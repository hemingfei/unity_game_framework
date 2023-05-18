using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace CustomGameFramework.Runtime
{
    public partial class UIManager
    {
        private static bool CheckCanOpenAgain(string UIName)
        {
            var maxNum = s_DefaultUINum; // -1代表无线

            if (s_uiRestrict.ContainsKey(UIName))
            {
                maxNum = s_uiRestrict[UIName];

                if (maxNum < 0) maxNum = -1;
            }

            var isOpen = s_UIs.ContainsKey(UIName) && (s_UIs[UIName].Count < maxNum || maxNum == -1);
            var isNotOpen = !s_UIs.ContainsKey(UIName) && (maxNum > 0 || maxNum == -1);
            return isOpen || isNotOpen;
        }

        private static async UniTask<UIWindowBase> CreateUIWindow(string UIName, bool isFromAseetBundle, bool isAsync)
        {
            Log.Debug("UI ->创建窗口 " + UIName);
            if (!CheckCanOpenAgain(UIName))
            {
                Log.Debug($"创建面板失败 已创建到最大个数，无法再继续创建 {UIName}");
                return null;
            }

            var assetFullPath = string.Empty;
            GameObject ui = null;

            if (isFromAseetBundle)
            {
                assetFullPath = s_uiCustomPath.TryGetValue(UIName, out var value) ? value : GetUIPath(UIName);
                if (isAsync)
                    ui = await ResourceMgr.LoadGameObjectAsync(assetFullPath, UIManagerGo.transform);
                else
                    ui = ResourceMgr.LoadGameObjectSync(assetFullPath, UIManagerGo.transform);
            }
            else
            {
                assetFullPath = s_uiCustomPath.TryGetValue(UIName, out var value)
                    ? value
                    : GetUIPathInResources(UIName);
                if (isAsync)
                {
                    var goAsync = await Resources.LoadAsync(assetFullPath);
                    if (Instantiate(goAsync, UIManagerGo.transform) is not GameObject go)
                        throw new Exception($"[Resource] Failed to instantiate asset: {assetFullPath}");
                    go.transform.localPosition = Vector3.zero;
                    go.transform.localScale = Vector3.one;
                    ui = go;
                    s_resourceUIs.Add(go, goAsync);
                }
                else
                {
                    var goSync = Resources.Load(assetFullPath);
                    if (Instantiate(goSync, UIManagerGo.transform) is not GameObject go)
                        throw new Exception($"[Resource] Failed to instantiate asset: {assetFullPath}");
                    go.transform.localPosition = Vector3.zero;
                    go.transform.localScale = Vector3.one;
                    ui = go;
                    s_resourceUIs.Add(go, goSync);
                }
            }

            ui.gameObject.name = UIName;

            var uiWindowBase = ui.GetComponent<UIWindowBase>();
            uiWindowBase.windowStatus = UIWindowBase.WindowStatus.Create;
            try
            {
                uiWindowBase.InitWindow(GetUIID(UIName));
            }
            catch (Exception e)
            {
                Log.Error(UIName + "OnInit Exception: " + e);
            }

            AddHideUI(uiWindowBase);
            UILayerManager.SetLayer(uiWindowBase);
            UIEvent.FireNow(uiWindowBase, UIEventType.OnInit); //派发OnInit事件
            Log.Debug("UI ->创建窗口完成 " + UIName);
            return uiWindowBase;
        }


        private static async UniTask<UIWindowBase> OpenUIWindow(bool isFromAssetBundle, bool isAsync, string UIName,
            UIEventCallBack callback = null, params object[] objs)
        {
            Log.Debug("UI ->打开窗口 " + UIName);
            if (!CheckCanOpenAgain(UIName))
            {
                Log.Debug($"打开面板失败 已达最大数量无法创建新UI {UIName}");
                return null;
            }

            var UIbase = GetHideUI(UIName);

            if (UIbase == null) UIbase = await CreateUIWindow(UIName, isFromAssetBundle, isAsync);

            // 继续打开面板
            RemoveHideUI(UIbase);
            AddUI(UIbase);
            UIStackManager.OnUIOpen(UIbase);
            UILayerManager.SetLayer(UIbase); //设置层级
            UIbase.SetOpenParam(objs);
            UIbase.OnOpen();
            UIEvent.FireNow(UIbase, UIEventType.OnOpen, objs); //派发OnOpened事件
            StartEnterAnim(UIbase, callback, objs); //播放动画
            return UIbase;
        }

        private static void StartEnterAnim(UIWindowBase UIbase, UIEventCallBack callBack, params object[] objs)
        {
            UIbase.windowStatus = UIWindowBase.WindowStatus.OpenAnim;
            UIbase.OnStartEnterAnim();
            UIEvent.FireNow(UIbase, UIEventType.OnStartEnterAnim, objs);
            UIbase.EnterAnim(EndEnterAnim, callBack, objs);
        }

        //进入动画播放完毕回调
        private static void EndEnterAnim(UIWindowBase UIbase, UIEventCallBack callBack, params object[] objs)
        {
            UIbase.OnCompleteEnterAnim();
            UIEvent.FireNow(UIbase, UIEventType.OnCompleteEnterAnim, objs);
            UIbase.windowStatus = UIWindowBase.WindowStatus.Open;

            try
            {
                if (callBack != null) callBack(UIbase, objs);
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
            }
        }


        public static void CloseUIWindow(UIWindowBase UI, bool isPlayAnim = true, UIEventCallBack callback = null,
            params object[] objs)
        {
            Log.Debug("UI ->关闭窗口 " + UI.UIName);
            UI.SetCloseParam(objs);

            //动画播放完毕删除UI
            if (callback != null)
                callback += CloseUIWindowCallBack;
            else
                callback = CloseUIWindowCallBack;

            if (isPlayAnim)
                StartExitAnim(UI, callback, objs);
            else
                callback(UI, objs);
        }

        private static void StartExitAnim(UIWindowBase UI, UIEventCallBack callBack, params object[] objs)
        {
            UI.windowStatus = UIWindowBase.WindowStatus.CloseAnim;
            UI.OnStartExitAnim();
            UIEvent.FireNow(UI, UIEventType.OnStartExitAnim, objs);
            UI.ExitAnim(EndExitAnim, callBack, objs);
        }

        //退出动画播放完毕回调
        private static void EndExitAnim(UIWindowBase UIbase, UIEventCallBack callBack, params object[] objs)
        {
            UIbase.OnCompleteExitAnim();
            UIEvent.FireNow(UIbase, UIEventType.OnCompleteExitAnim, objs);
            UIbase.windowStatus = UIWindowBase.WindowStatus.Close;

            try
            {
                callBack?.Invoke(UIbase, objs);
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
            }
        }

        private static void CloseUIWindowCallBack(UIWindowBase UI, params object[] objs)
        {
            RemoveUI(UI); //移除UI引用
            UI.windowStatus = UIWindowBase.WindowStatus.Close;
            try
            {
                UI.OnClose();
            }
            catch (Exception e)
            {
                Log.Error(UI.UIName + " OnClose Exception: " + e);
            }

            UIStackManager.OnUIClose(UI);
            AddHideUI(UI);
            UIEvent.FireNow(UI, UIEventType.OnClose, objs); //派发OnOpened事件
        }


        public static void DestroyUIWindow(UIWindowBase UI)
        {
            if (GetIsExitsHide(UI))
                RemoveHideUI(UI);
            else if (GetIsExits(UI)) RemoveUI(UI);

            UIEvent.FireNow(UI, UIEventType.OnDestroy); //派发OnDestroy事件

            try
            {
                UI.Dispose();
            }
            catch (Exception e)
            {
                Log.Error("OnDestroy :" + e);
            }

            if (s_resourceUIs.ContainsKey(UI.gameObject))
            {
                var obj = s_resourceUIs[UI.gameObject];
                s_resourceUIs.Remove(UI.gameObject);
                Destroy(UI.gameObject);
                obj = null;
                Resources.UnloadUnusedAssets();
            }
            else
            {
                ResourceMgr.ReleaseGameObject(UI.gameObject);
            }
        }

        /// <summary>
        ///     关闭UI之后彻底销毁为跟从未打开过此UI一样
        /// </summary>
        public static void CloseAndDestroyUIWindow(string UIname, bool isPlayAnim = true,
            UIEventCallBack callback = null,
            params object[] objs)
        {
            if (GetUI(UIname) != null) UIEvent.Subscribe(UIname, UIEventType.OnClose, OnClosedToDestroy);

            CloseUIWindow(UIname, isPlayAnim, callback, objs);
        }

        private static void OnClosedToDestroy(UIWindowBase uiClosed, params object[] objs)
        {
            var uiClosedName = uiClosed.UIName;
            UIEvent.UnSubscribe(uiClosedName, UIEventType.OnClose, OnClosedToDestroy);

            Log.Debug($"UI ->销毁窗口 {uiClosedName}");
            DestroyUIWindow(uiClosed);
        }
    }
}