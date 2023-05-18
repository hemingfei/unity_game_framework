/****************************************************
*	文件：UIModelShowTool.cs
*	作者：何明飞
*	邮箱：hemingfei@outlook.com
*	日期：2023/01/08 00:19:28
*	功能：暂无
*****************************************************/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

//using UnityEngine.Rendering.Universal;

namespace CustomGameFramework.Runtime
{
    public static class UIModelShowTool
    {
        private const bool c_defaultOrthographic = true;
        private const float c_defaultOrthographicSize = 0.72f;
        private const float c_defaultFOV = 60;

        /// <summary>
        ///     每个UImodelCamera的间隔
        /// </summary>
        private static readonly Vector3 s_ShowSpace = new(0, 20, 0);

        private static readonly string s_defaultLayer = "UI";
        private static readonly Color s_defaultBackgroundColor = new(0, 0, 0, 0 / 255f);
        private static readonly Vector3 s_StartPosition = new(5000, 5000, 5000);
        private static readonly Vector3 s_defaultLocationPosition = new(0, 0, 10);
        private static readonly Vector3 s_defaultEulerAngles = new(0, 180, 0);
        private static readonly Vector3 s_defaultLocalScale = Vector3.one;
        private static readonly Vector3 s_defaultTexSize = new(512, 512, 100);
        private static readonly Vector2 s_clippingPlanes = new(0.2f, 20);
        private static readonly Vector3 s_lightEulerAngles = new(30, 180, 0);

        private static readonly List<UIModelShowData> modelShowList = new();

        private static List<GameObject> _assetHandles;

        private static void ResetModelShowPosition()
        {
            for (var i = 0; i < modelShowList.Count; i++)
                if (modelShowList[i].top != null)
                    modelShowList[i].top.transform.position = s_StartPosition + i * s_ShowSpace;
        }


        public static void DisposeModelShow(UIModelShowData data)
        {
            ResetModelShowPosition();
            data.Dispose();
            modelShowList.Remove(data);
        }

        public static UIModelShowData CreateModelData(string prefabName,
            string layerName = null,
            bool? orthographic = null,
            float? orthographicSize = null,
            Color? backgroundColor = null,
            Vector3? localPosition = null,
            Vector3? eulerAngles = null,
            Vector3? localScale = null,
            Vector3? texSize = null,
            float? nearClippingPlane = null,
            float? farClippingPlane = null,
            Vector3? lightEulerAngles = null)
        {
            //默认值设置
            layerName = layerName ?? s_defaultLayer;
            var localPositionTmp = localPosition ?? s_defaultLocationPosition;
            var eulerAnglesTmp = eulerAngles ?? s_defaultEulerAngles;
            var texSizeTmp = texSize ?? s_defaultTexSize;
            var localScaleTmp = localScale ?? s_defaultLocalScale;
            var backgroundColorTmp = backgroundColor ?? s_defaultBackgroundColor;
            var orthographicSizeTmp = orthographicSize ?? c_defaultOrthographicSize;
            var orthographicTmp = orthographic ?? c_defaultOrthographic;
            var fieldOfView = orthographicSize ?? c_defaultFOV;
            var nearClippingPlaneTmp = nearClippingPlane ?? s_clippingPlanes.x;
            var farClippingPlaneTmp = farClippingPlane ?? s_clippingPlanes.y;
            var lightEulerAnglesTmp = lightEulerAngles ?? s_lightEulerAngles;
            //构造Camera
            var data = new UIModelShowData();

            var uiModelShow = new GameObject("UIShowModelCamera");
            Object.DontDestroyOnLoad(uiModelShow);
            data.top = uiModelShow;

            var camera = new GameObject("Camera");

            camera.transform.SetParent(uiModelShow.transform);
            camera.transform.localPosition = Vector3.zero;
            var ca = camera.AddComponent<Camera>();
            data.camera = ca;

            ca.clearFlags = CameraClearFlags.SolidColor;
            ca.backgroundColor = backgroundColorTmp;
            ca.orthographic = orthographicTmp;
            ca.orthographicSize = orthographicSizeTmp;
            ca.fieldOfView = fieldOfView;
            ca.depth = 100;
            ca.nearClipPlane = nearClippingPlaneTmp;
            ca.farClipPlane = farClippingPlaneTmp;
            ca.cullingMask = 1 << LayerMask.NameToLayer(layerName);

            var root = new GameObject("Root");
            data.root = root;

            root.transform.SetParent(camera.transform);
            root.transform.localPosition = localPositionTmp;
            root.transform.eulerAngles = eulerAnglesTmp;
            root.transform.localScale = localScaleTmp;

            // 创建灯光
            var light = new GameObject("Light").AddComponent<Light>();
            data.light = light;
            light.transform.SetParent(root.transform);
            light.transform.localPosition = Vector3.zero;
            light.transform.localEulerAngles = lightEulerAnglesTmp;
            light.transform.localScale = Vector3.one;
            light.type = LightType.Directional;
            light.cullingMask = 1 << LayerMask.NameToLayer(layerName);

            //创建模型
            var obj = CreateModelGameObject(prefabName);
            data.model = obj;

            obj.transform.SetParent(root.transform);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localEulerAngles = Vector3.zero;
            obj.transform.localScale = Vector3.one;

            obj.SetLayerRecursively(LayerMask.NameToLayer(layerName));

            var mes = obj.GetComponentsInChildren<SkinnedMeshRenderer>();
            for (var i = 0; i < mes.Length; i++)
            {
                mes[i].shadowCastingMode = ShadowCastingMode.Off;
                mes[i].receiveShadows = false;
            }

            //设置randerTexture
            var tex = new RenderTexture((int)texSizeTmp.x, (int)texSizeTmp.y, (int)texSizeTmp.z);
            data.renderTexture = tex;

            tex.autoGenerateMips = false;
            tex.anisoLevel = 1;

            ca.targetTexture = tex;

            modelShowList.Add(data);
            ResetModelShowPosition();

            return data;
        }

        //     public static UIModelShowData CreateModelDataURP(string prefabName, Camera baseCamera,
        //     string layerName = null,
        //     bool? orthographic = null,
        //     float? orthographicSize = null,
        //     Vector3? localPosition = null,
        //     Vector3? eulerAngles = null,
        //     Vector3? localScale = null,
        //     Vector3? texSize = null,
        //     float? nearClippingPlane = null,
        //     float? farClippingPlane = null)
        // {
        //     //默认值设置
        //     layerName = layerName ?? s_defaultLayer;
        //     Vector3 localPositionTmp = localPosition ?? s_defaultLocationPosition;
        //     Vector3 eulerAnglesTmp = eulerAngles ?? s_defaultEulerAngles;
        //     Vector3 texSizeTmp = texSize ?? s_defaultTexSize;
        //     Vector3 localScaleTmp = localScale ?? s_defaultLocalScale;
        //     //Color backgroundColorTmp = backgroundColor ?? s_defaultBackgroundColor;
        //     float orthographicSizeTmp = orthographicSize ?? c_defaultOrthographicSize;
        //     bool orthographicTmp = orthographic??c_defaultOrthographic;
        //     float fieldOfView = orthographicSize ?? c_defaultFOV;
        //     float nearClippingPlaneTmp = nearClippingPlane ?? s_clippingPlanes.x;
        //     float farClippingPlaneTmp = farClippingPlane ?? s_clippingPlanes.y;
        //     //构造Camera
        //     UIModelShowData data = new UIModelShowData();
        //
        //     GameObject uiModelShow = new GameObject("UIShowModelCamera");
        //     UnityEngine.Object.DontDestroyOnLoad(uiModelShow);
        //     data.top = uiModelShow;
        //
        //     GameObject camera = new GameObject("Camera");
        //
        //     camera.transform.SetParent(uiModelShow.transform);
        //     camera.transform.localPosition = Vector3.zero;
        //     Camera ca = camera.AddComponent<Camera>();
        //     data.camera = ca;
        //     data.baseCamerainURP = baseCamera;
        //
        //     // URP 支持
        //     var cameraData = ca.GetUniversalAdditionalCameraData();
        //     cameraData.renderType = CameraRenderType.Overlay;
        //     var cameraStack = baseCamera.GetUniversalAdditionalCameraData().cameraStack;
        //     if (!cameraStack.Contains(ca))
        //     {
        //         cameraStack.Add(ca);
        //     }
        //     
        //
        //
        //     //ca.clearFlags = CameraClearFlags.SolidColor;
        //     //ca.backgroundColor = backgroundColorTmp;
        //     ca.orthographic = orthographicTmp;
        //     ca.orthographicSize = orthographicSizeTmp;
        //     if (!orthographicTmp)
        //     {
        //         ca.fieldOfView = fieldOfView;
        //     }
        //     //ca.depth = 100;
        //     ca.nearClipPlane = nearClippingPlaneTmp;
        //     ca.farClipPlane = farClippingPlaneTmp;
        //     ca.cullingMask = 1 << LayerMask.NameToLayer(layerName);
        //
        //     GameObject root = new GameObject("Root");
        //     data.root = root;
        //
        //     root.transform.SetParent(camera.transform);
        //     root.transform.localPosition = localPositionTmp;
        //     root.transform.eulerAngles = eulerAnglesTmp;
        //     root.transform.localScale = localScaleTmp;
        //
        //     //创建模型
        //     GameObject obj = CreateModelGameObject(prefabName);
        //     data.model = obj;
        //
        //     obj.transform.SetParent(root.transform);
        //     obj.transform.localPosition = new Vector3(0, 0, 0);
        //     obj.transform.localEulerAngles = Vector3.zero;
        //     obj.transform.localScale = Vector3.one;
        //
        //     obj.SetLayerRecursively(LayerMask.NameToLayer(layerName));
        //
        //     SkinnedMeshRenderer[] mes = obj.GetComponentsInChildren<SkinnedMeshRenderer>();
        //     for (int i = 0; i < mes.Length; i++)
        //     {
        //         mes[i].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        //         mes[i].receiveShadows = false;
        //     }
        //     
        //     //设置randerTexture
        //     RenderTexture tex = new RenderTexture((int)texSizeTmp.x, (int)texSizeTmp.y, (int)texSizeTmp.z);
        //     data.renderTexture = tex;
        //     
        //     tex.autoGenerateMips = false;
        //     tex.anisoLevel = 1;
        //
        //     //ca.targetTexture = tex;
        //
        //     modelShowList.Add(data);
        //     ResetModelShowPosition();
        //
        //     return data;
        // }

        public static GameObject Create(string prefabName, out RenderTexture tex)
        {
            var temp0 = new GameObject("UIModelShow");
            var temp1 = new GameObject("Camera");
            temp1.transform.SetParent(temp0.transform);
            temp1.transform.localPosition = new Vector3(0, 5000, 0);
            var ca = temp1.AddComponent<Camera>();
            ca.clearFlags = CameraClearFlags.SolidColor;
            ca.backgroundColor = new Color(0, 0, 0, 5 / 255f);
            ca.orthographic = true;
            ca.orthographicSize = 0.72f;
            ca.depth = 100;
            ca.cullingMask = 1 << LayerMask.NameToLayer("UI");

            var root = new GameObject("Root");
            root.transform.SetParent(temp1.transform);
            root.transform.localPosition = new Vector3(0, 0, 100);
            root.transform.eulerAngles = new Vector3(0, 180, 0);

            var obj = CreateModelGameObject(prefabName);
            obj.transform.SetParent(root.transform);
            obj.transform.localPosition = new Vector3(0, 0, 0);
            obj.transform.localEulerAngles = Vector3.zero;

            var trans = obj.GetComponentsInChildren<Transform>();
            for (var i = 0; i < trans.Length; i++) trans[i].gameObject.layer = LayerMask.NameToLayer("UI");

            var mes = obj.GetComponentsInChildren<SkinnedMeshRenderer>();
            for (var i = 0; i < mes.Length; i++)
            {
                mes[i].shadowCastingMode = ShadowCastingMode.Off;
                mes[i].receiveShadows = false;
            }

            tex = new RenderTexture(512, 512, 100);
            tex.autoGenerateMips = false;
            tex.anisoLevel = 1;

            //  tex.antiAliasing = 2

            ca.targetTexture = tex;
            return obj;
        }

        public static void AddDrag(GameObject UIObj, GameObject modelObj)
        {
            var mro = modelObj.GetComponent<UIModelShowRotate>();
            if (mro == null) mro = modelObj.AddComponent<UIModelShowRotate>();

            EventTrigger trigger;
            trigger = UIObj.GetComponent<EventTrigger>();
            if (trigger)
                trigger.triggers.Clear();
            else
                trigger = UIObj.AddComponent<EventTrigger>();

            trigger.triggers.Add(GetEvent(EventTriggerType.Drag, mro.OnDrag));
            trigger.triggers.Add(GetEvent(EventTriggerType.PointerUp, mro.OnPointerUp));
        }

        public static void RemoveDrag(GameObject UIObj)
        {
            EventTrigger trigger;
            trigger = UIObj.GetComponent<EventTrigger>();
            if (trigger) trigger.triggers.Clear();
        }

        private static EventTrigger.Entry GetEvent(EventTriggerType type, UnityAction<BaseEventData> eventFun)
        {
            var eventDrag = eventFun;
            var myclick = new EventTrigger.Entry();
            myclick.eventID = type;
            myclick.callback.AddListener(eventDrag);
            return myclick;
        }

        public static GameObject CreateModelGameObject(string assetPath)
        {
            if (_assetHandles == null) _assetHandles = new List<GameObject>();

            var go = UIManager.ResourceMgr.LoadGameObjectSync(assetPath);
            _assetHandles.Add(go);
            return go;
        }

        public static void DestroyModelGameObject(GameObject go)
        {
            _assetHandles.Remove(go);
            UIManager.ResourceMgr.ReleaseGameObject(go);
        }
    }
}