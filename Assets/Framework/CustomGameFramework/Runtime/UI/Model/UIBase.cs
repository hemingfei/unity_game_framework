//
//  UIBase.cs 
//
//  Author: HGT 
//
//  Copyright (c) 2021 hegametech.com 
//

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace CustomGameFramework.Runtime
{
    public class UIBase : MonoBehaviour, IUILifeCycle
    {
        [HideInInspector] public Canvas m_canvas;

        #region GET SET UIName  UIID

        private string m_UIName;

        public string UIName
        {
            get
            {
                if (m_UIName == null) m_UIName = name;

                return m_UIName;
            }
            set => m_UIName = value;
        }

        public int UIID { get; private set; } = -1;

        public string UIEventKey => UIName + "@" + UIID;

        #endregion

        #region 重载方法

        //当UI第一次打开时调用OnInit方法，调用时机在OnOpen之前
        public virtual void OnUIInit()
        {
        }

        public virtual void OnUIDispose()
        {
        }

        public void Init(string UIEventKey, int id)
        {
            if (UIEventKey != null)
            {
                UIName = null;
                UIName = UIEventKey + "_" + UIName;
            }

            UIID = id;
            m_canvas = GetComponent<Canvas>();
            CreateObjectTable();
            OnUIInit();
        }

        public void Dispose()
        {
            CleanUIModelShowDatas();
            try
            {
                OnUIDispose();
            }
            catch (Exception e)
            {
                Log.Error("UIBase Dispose Exception -> UIEventKey: " + UIEventKey + " Exception: " + e);
            }

            DisposeLifeComponent();
        }

        #endregion

        #region 获取对象

        public List<GameObject> m_objectList = new();

        //生成对象表，便于快速获取对象，并忽略层级
        private void CreateObjectTable()
        {
            if (m_objects == null) m_objects = new Dictionary<string, GameObject>();

            m_objects.Clear();
            m_uiBases.Clear();

            m_images.Clear();
            m_Sprites.Clear();
            m_texts.Clear();
            m_textmeshs.Clear();
            m_buttons.Clear();
            m_scrollRects.Clear();
            m_rawImages.Clear();
            m_transforms.Clear();
            m_rectTransforms.Clear();
            m_inputFields.Clear();
            m_Sliders.Clear();
            m_Canvas.Clear();
            m_CanvasGroup.Clear();
            m_Toggle.Clear();

            for (var i = 0; i < m_objectList.Count; i++)
                if (m_objectList[i] != null)
                {
                    if (m_objects.ContainsKey(m_objectList[i].name))
                        Log.Error(name + " CreateObjectTable Contains Duplicate Key ->" + m_objectList[i].name + "<-");
                    else
                        m_objects.Add(m_objectList[i].name, m_objectList[i]);
                }
                else
                {
                    Log.Error(name + " CreateObjectTable m_objectList[" + i + "] is Null !");
                }
        }

        private Dictionary<string, GameObject> m_objects;
        private readonly Dictionary<string, UIBase> m_uiBases = new();

        private readonly Dictionary<string, Image> m_images = new();
        private readonly Dictionary<string, Sprite> m_Sprites = new();
        private readonly Dictionary<string, Text> m_texts = new();
        private readonly Dictionary<string, TextMesh> m_textmeshs = new();
        private readonly Dictionary<string, Button> m_buttons = new();
        private readonly Dictionary<string, ScrollRect> m_scrollRects = new();
        private readonly Dictionary<string, RawImage> m_rawImages = new();
        private readonly Dictionary<string, Transform> m_transforms = new();
        private readonly Dictionary<string, RectTransform> m_rectTransforms = new();
        private readonly Dictionary<string, InputField> m_inputFields = new();
        private readonly Dictionary<string, Slider> m_Sliders = new();
        private readonly Dictionary<string, Canvas> m_Canvas = new();
        private readonly Dictionary<string, CanvasGroup> m_CanvasGroup = new();
        private readonly Dictionary<string, Toggle> m_Toggle = new();

        public bool HaveObject(string name)
        {
            var has = false;
            has = m_objects.ContainsKey(name);
            return has;
        }

        public Vector3 GetPosition(string name, bool islocal)
        {
            var tmp = Vector3.zero;
            var go = GetGameObject(name);

            if (go != null)
            {
                if (islocal)
                    tmp = GetGameObject(name).transform.localPosition;
                else
                    tmp = GetGameObject(name).transform.position;
            }

            return tmp;
        }

        public GameObject GetGameObject(string name)
        {
            if (m_objects == null) CreateObjectTable();

            if (m_objects.ContainsKey(name))
            {
                var go = m_objects[name];

                if (go == null)
                    throw new Exception("UIWindowBase GetGameObject error: " + UIName + " m_objects[" + name +
                                        "] is null !!");

                return go;
            }

            throw new Exception("UIWindowBase GetGameObject error: " + UIName + " dont find ->" + name + "<-");
        }

        public Transform GetTransform(string name)
        {
            if (m_transforms.ContainsKey(name)) return m_transforms[name];

            var tmp = GetGameObject(name).GetComponent<Transform>();

            if (tmp == null) throw new Exception(UIEventKey + " GetTransform ->" + name + "<- is Null !");

            m_transforms.Add(name, tmp);
            return tmp;
        }

        public RectTransform GetRectTransform(string name)
        {
            if (m_rectTransforms.ContainsKey(name)) return m_rectTransforms[name];

            var tmp = GetGameObject(name).GetComponent<RectTransform>();

            if (tmp == null) throw new Exception(UIEventKey + " GetRectTransform ->" + name + "<- is Null !");

            m_rectTransforms.Add(name, tmp);
            return tmp;
        }

        public UIBase GetUIBase(string name)
        {
            if (m_uiBases.ContainsKey(name)) return m_uiBases[name];

            var tmp = GetGameObject(name).GetComponent<UIBase>();

            if (tmp == null) throw new Exception(UIEventKey + " GetUIBase ->" + name + "<- is Null !");

            m_uiBases.Add(name, tmp);
            return tmp;
        }

        public Sprite GetSprite(string name)
        {
            if (m_Sprites.ContainsKey(name)) return m_Sprites[name];

            var tmp = GetGameObject(name).GetComponent<Sprite>();

            if (tmp == null) throw new Exception(UIEventKey + " GetImage ->" + name + "<- is Null !");

            m_Sprites.Add(name, tmp);
            return tmp;
        }

        public Text GetText(string name)
        {
            if (m_texts.ContainsKey(name)) return m_texts[name];

            var tmp = GetGameObject(name).GetComponent<Text>();

            if (tmp == null) throw new Exception(UIEventKey + " GetText ->" + name + "<- is Null !");

            m_texts.Add(name, tmp);
            return tmp;
        }

        public TextMesh GetTextMesh(string name)
        {
            if (m_textmeshs.ContainsKey(name)) return m_textmeshs[name];

            var tmp = GetGameObject(name).GetComponent<TextMesh>();

            if (tmp == null) throw new Exception(UIEventKey + " GetTextMesh ->" + name + "<- is Null !");

            m_textmeshs.Add(name, tmp);
            return tmp;
        }

        public Toggle GetToggle(string name)
        {
            if (m_Toggle.ContainsKey(name)) return m_Toggle[name];

            var tmp = GetGameObject(name).GetComponent<Toggle>();

            if (tmp == null) throw new Exception(UIEventKey + " GetToggle ->" + name + "<- is Null !");

            m_Toggle.Add(name, tmp);
            return tmp;
        }

        public Button GetButton(string name)
        {
            if (m_buttons.ContainsKey(name)) return m_buttons[name];

            var tmp = GetGameObject(name).GetComponent<Button>();

            if (tmp == null) throw new Exception(UIEventKey + " GetButton ->" + name + "<- is Null !");

            m_buttons.Add(name, tmp);
            return tmp;
        }

        public InputField GetInputField(string name)
        {
            if (m_inputFields.ContainsKey(name)) return m_inputFields[name];

            var tmp = GetGameObject(name).GetComponent<InputField>();

            if (tmp == null) throw new Exception(UIEventKey + " GetInputField ->" + name + "<- is Null !");

            m_inputFields.Add(name, tmp);
            return tmp;
        }

        public ScrollRect GetScrollRect(string name)
        {
            if (m_scrollRects.ContainsKey(name)) return m_scrollRects[name];

            var tmp = GetGameObject(name).GetComponent<ScrollRect>();

            if (tmp == null) throw new Exception(UIEventKey + " GetScrollRect ->" + name + "<- is Null !");

            m_scrollRects.Add(name, tmp);
            return tmp;
        }

        public Image GetImage(string name)
        {
            if (m_images.ContainsKey(name)) return m_images[name];

            var tmp = GetGameObject(name).GetComponent<Image>();

            if (tmp == null) throw new Exception(UIEventKey + " GetImage ->" + name + "<- is Null !");

            m_images.Add(name, tmp);
            return tmp;
        }

        public RawImage GetRawImage(string name)
        {
            if (m_rawImages.ContainsKey(name)) return m_rawImages[name];

            var tmp = GetGameObject(name).GetComponent<RawImage>();

            if (tmp == null) throw new Exception(UIEventKey + " GetRawImage ->" + name + "<- is Null !");

            m_rawImages.Add(name, tmp);
            return tmp;
        }

        public Slider GetSlider(string name)
        {
            if (m_Sliders.ContainsKey(name)) return m_Sliders[name];

            var tmp = GetGameObject(name).GetComponent<Slider>();

            if (tmp == null) throw new Exception(UIEventKey + " GetSlider ->" + name + "<- is Null !");

            m_Sliders.Add(name, tmp);
            return tmp;
        }

        public Canvas GetCanvas(string name)
        {
            if (m_Canvas.ContainsKey(name)) return m_Canvas[name];

            var tmp = GetGameObject(name).GetComponent<Canvas>();

            if (tmp == null) throw new Exception(UIEventKey + " GetCanvas ->" + name + "<- is Null !");

            m_Canvas.Add(name, tmp);
            return tmp;
        }

        public CanvasGroup GetCanvasGroup(string name)
        {
            if (m_CanvasGroup.ContainsKey(name)) return m_CanvasGroup[name];

            var tmp = GetGameObject(name).GetComponent<CanvasGroup>();

            if (tmp == null) throw new Exception(UIEventKey + " GetCanvasGroup ->" + name + "<- is Null !");

            m_CanvasGroup.Add(name, tmp);
            return tmp;
        }

        private RectTransform m_rectTransform;

        public RectTransform RectTransform
        {
            get
            {
                if (m_rectTransform == null) m_rectTransform = transform as RectTransform;

                return m_rectTransform;
            }
            set => m_rectTransform = value;
        }

        public void SetSizeDelta(float w, float h)
        {
            RectTransform.sizeDelta = new Vector2(w, h);
        }

        #endregion

        #region 模型展示到UI上，UIModelShow

        private readonly List<UIModelShowData> modelList = new();

        /// <summary>
        ///     创建UI的模型展示数据，可以设置相机数据
        /// </summary>
        /// <param name="modelName">模型路径</param>
        /// <returns>模型展示类</returns>
        public UIModelShowData CreateUIModelShowData(string modelName,
            string layerName = null,
            bool? orthographic = null,
            float? orthographicSize = null,
            Color? backgroundColor = null,
            Vector3? localPosition = null,
            Vector3? localScale = null,
            Vector3? eulerAngles = null,
            Vector3? texSize = null,
            float? nearClippingPlane = null,
            float? farClippingPlane = null,
            Vector3? lightEulerAngles = null)
        {
            UIModelShowData model;
            model = UIModelShowTool.CreateModelData(modelName, layerName, orthographic, orthographicSize,
                backgroundColor, localPosition, eulerAngles, localScale, texSize, nearClippingPlane, farClippingPlane,
                lightEulerAngles);
            modelList.Add(model);
            return model;
        }

        /// <summary>
        ///     UI模型展示到UI上
        /// </summary>
        /// <param name="uiModelShowData">展示模型数据</param>
        /// <param name="showAreaRawImage">显示区域RawImage组件</param>
        /// <param name="dragAreaGameObject">拖拽区域GameObejct</param>
        /// <param name="isResetRotation">是否重置旋转</param>
        /// <param name="hideOtherUiModels">是否隐藏其他的模型，默认TRUE，性能会得到优化</param>
        public void UIModelShow(UIModelShowData uiModelShowData, RawImage showAreaRawImage,
            GameObject dragAreaGameObject, bool isResetRotation = true, bool hideOtherUiModels = true)
        {
            if (hideOtherUiModels)
                foreach (var model in modelList)
                    if (model != uiModelShowData)
                        model.camera.gameObject.SetActive(false);

            if (isResetRotation) uiModelShowData.model.transform.localRotation = Quaternion.identity;
            uiModelShowData.camera.gameObject.SetActive(true);
            showAreaRawImage.texture = uiModelShowData.renderTexture;
            showAreaRawImage.color = Color.white;
            UIModelShowAddDrag(uiModelShowData, dragAreaGameObject);
        }

        // /// <summary>
        // /// 创建UI的模型展示数据，可以设置相机数据
        // /// </summary>
        // /// <param name="modelName">模型路径</param>
        // /// <param name="baseCamera">主相机</param>
        // /// <param name="localPosition">XY为相机的orthographicSize大小，Y轴中间0上下正负orthographicSize（默认为1），X轴跟Y差不多，不过最大值为orthographicSize 相对于屏幕宽高</param>
        // /// <returns>模型展示类</returns>
        // public UIModelShowData CreateUIModelShowDataURP(string modelName, Camera baseCamera,
        //     string layerName = null,
        //     bool? orthographic = null,
        //     float? orthographicSize = null,
        //     Vector3? localPosition = null,
        //     Vector3? localScale = null,
        //     Vector3? eulerAngles = null,
        //     Vector3? texSize = null,
        //     float? nearClippingPlane = null,
        //     float? farClippingPlane = null)
        // {
        //     UIModelShowData model;
        //     model = UIModelShowTool.CreateModelDataURP(modelName, baseCamera, layerName, orthographic, orthographicSize,
        //         localPosition, eulerAngles, localScale, texSize, nearClippingPlane, farClippingPlane);
        //     modelList.Add(model);
        //     return model;
        // }

        // /// <summary>
        // /// UI模型展示到UI上
        // /// </summary>
        // /// <param name="uiModelShowData">展示模型数据</param>
        // /// <param name="showAreaRawImage">显示区域RawImage组件</param>
        // /// <param name="dragAreaGameObject">拖拽区域GameObejct</param>
        // /// <param name="isResetRotation">是否重置旋转</param>
        // /// <param name="hideOtherUiModels">是否隐藏其他的模型，默认TRUE，性能会得到优化</param>
        // public void UIModelShowURP(UIModelShowData uiModelShowData, RawImage showAreaRawImage, GameObject dragAreaGameObject, bool isResetRotation = true, bool hideOtherUiModels = true)
        // {
        //     if (hideOtherUiModels)
        //     {
        //         foreach (var model in modelList)
        //         {
        //             if (model != uiModelShowData)
        //             {
        //                 model.camera.gameObject.SetActive(false);
        //             }
        //         }
        //     }
        //
        //     var cameraSize = uiModelShowData.camera.orthographicSize;
        //     Vector3[] v = new Vector3[4];
        //     (showAreaRawImage.transform as RectTransform)?.GetWorldCorners(v);
        //     var midPos = (v[2] - v[0]) / 2f + v[0];
        //     var rawImgScreenPos = UIManager.GetCamera().WorldToScreenPoint(midPos);
        //     var normX = rawImgScreenPos.x / (Screen.width * 1.0f);
        //     var normY = rawImgScreenPos.y / (Screen.height * 1.0f);
        //
        //     var normXCam = (-2 * normX + 1) * cameraSize / (Screen.height * 1.0f) * (Screen.width * 1.0f);
        //     var normYCam = (2 * normY - 1) * cameraSize;
        //
        //     var oldPos = uiModelShowData.model.transform.localPosition;
        //     oldPos.x = normXCam;
        //     oldPos.y = normYCam;
        //     uiModelShowData.model.transform.localPosition = oldPos;
        //
        //     if (isResetRotation)
        //     {
        //         uiModelShowData.model.transform.localRotation = Quaternion.identity;
        //     }
        //     uiModelShowData.camera.gameObject.SetActive(true);
        //     showAreaRawImage.texture = uiModelShowData.renderTexture;
        //     var c = Color.white;
        //     c.a = 0.001f;
        //     showAreaRawImage.color = c;
        //     UIModelShowAddDrag(uiModelShowData, dragAreaGameObject);
        // }

        /// <summary>
        ///     UI模型展示 添加 转动功能
        /// </summary>
        /// <param name="uiModelShowData"></param>
        /// <param name="dragAreaGameObject"></param>
        public void UIModelShowAddDrag(UIModelShowData uiModelShowData, GameObject dragAreaGameObject)
        {
            if (dragAreaGameObject != null) UIModelShowTool.AddDrag(dragAreaGameObject, uiModelShowData.model);
        }

        /// <summary>
        ///     清除UI模型展示的数据和资源
        /// </summary>
        /// <param name="data"></param>
        /// <param name="dragAreaGameObject">对应的拖拽区域，需要取消注册拖拽事件，不传入会报错</param>
        public void CleanUIModelShowData(UIModelShowData data, GameObject dragAreaGameObject)
        {
            modelList.Remove(data);
            UIModelShowTool.DisposeModelShow(data);
            if (dragAreaGameObject != null) UIModelShowTool.RemoveDrag(dragAreaGameObject);
        }

        /// <summary>
        ///     清除UI模型展示的全部数据和资源，销毁窗口会自动调用,不要手动调用
        /// </summary>
        public void CleanUIModelShowDatas()
        {
            for (var i = 0; i < modelList.Count; i++) UIModelShowTool.DisposeModelShow(modelList[i]);

            modelList.Clear();
        }

        #endregion

        #region 生命周期管理

        protected List<IUILifeCycle> m_lifeComponent = new();

        public void AddLifeCycleComponent(IUILifeCycle comp)
        {
            comp.Init(UIEventKey, m_lifeComponent.Count);
            m_lifeComponent.Add(comp);
        }

        private void DisposeLifeComponent()
        {
            for (var i = 0; i < m_lifeComponent.Count; i++)
                try
                {
                    m_lifeComponent[i].Dispose();
                }
                catch (Exception e)
                {
                    Log.Error("UIBase DisposeLifeComponent Exception -> UIEventKey: " + UIEventKey + " Exception: " +
                              e);
                }

            m_lifeComponent.Clear();
        }

        #endregion

        #region Editor 工具方法

#if UNITY_EDITOR

        [ContextMenu("ObjectList 去重")]
        public void ClearObject()
        {
            var ls = new List<GameObject>();
            var len = m_objectList.Count;

            for (var i = 0; i < len; i++)
            {
                var go = m_objectList[i];

                if (go != null)
                    if (!ls.Contains(go))
                        ls.Add(go);
            }

            ls.Sort((a, b) => { return a.name.CompareTo(b.name); });
            m_objectList = ls;
        }

        [ContextMenu("UIBase 快速提取字段 private TYPE m_xxx;")]
        public string EditorFastGetMembers()
        {
            ClearObject();
            var len = m_objectList.Count;
            var final = string.Empty;

            for (var i = 0; i < len; i++)
            {
                var go = m_objectList[i];
                var name = go.name;
                var hasComponent = false;

                if (name.Contains("Image") || name.Contains("image"))
                {
                    var memberName = "mi_" + name;
                    var typeName = string.Empty;
                    typeName = "private Image ";
                    var finalOutput = typeName + memberName + "; \n\t\t";
                    final += finalOutput;
                    hasComponent = true;
                }

                if (name.Contains("Text") || name.Contains("text"))
                {
                    var memberName = "mt_" + name;
                    var typeName = string.Empty;
                    typeName = "private Text ";
                    var finalOutput = typeName + memberName + "; \n\t\t";
                    final += finalOutput;
                    hasComponent = true;
                }

                if (name.Contains("Button") || name.Contains("button"))
                {
                    var memberName = "mb_" + name;
                    var typeName = string.Empty;
                    typeName = "private Button ";
                    var finalOutput = typeName + memberName + "; \n\t\t";
                    final += finalOutput;
                    hasComponent = true;
                }

                if (name.Contains("ListView") || name.Contains("listview"))
                {
                    var memberName = "ml_" + name;
                    var typeName = string.Empty;
                    typeName = "private IUListView ";
                    var finalOutput = typeName + memberName + "; \n\t\t";
                    final += finalOutput;
                    hasComponent = true;
                }

                if (hasComponent == false || name.Contains("Go") || name.Contains("go"))
                {
                    var memberName = "m_" + name;
                    var typeName = string.Empty;
                    typeName = "private GameObject ";
                    var finalOutput = typeName + memberName + "; \n\t\t";
                    final += finalOutput;
                }
            }

            var textEditor = new TextEditor();
            textEditor.text = final;
            textEditor.SelectAll();
            textEditor.Copy();
            return final;
        }

        [ContextMenu("UIBase 快速提取组件 GetComponents")]
        public string EditorFastGetComponents()
        {
            ClearObject();
            var len = m_objectList.Count;
            var final = string.Empty;

            for (var i = 0; i < len; i++)
            {
                var go = m_objectList[i];
                var name = go.name;
                var hasComponent = false;

                if (name.Contains("Image") || name.Contains("image"))
                {
                    var memberName = "mi_" + name;
                    var getComponentName = string.Empty;
                    getComponentName = "GetImage(\"" + name + "\")";
                    var finalOutput = memberName + " = " + getComponentName + "; \n\t\t\t";
                    final += finalOutput;
                    hasComponent = true;
                }

                if (name.Contains("Text") || name.Contains("text"))
                {
                    var memberName = "mt_" + name;
                    var getComponentName = string.Empty;
                    getComponentName = "GetText(\"" + name + "\")";
                    var finalOutput = memberName + " = " + getComponentName + "; \n\t\t\t";
                    final += finalOutput;
                    hasComponent = true;
                }

                if (name.Contains("Button") || name.Contains("button"))
                {
                    var memberName = "mb_" + name;
                    var getComponentName = string.Empty;
                    getComponentName = "GetButton(\"" + name + "\")";
                    var finalOutput = memberName + " = " + getComponentName + "; \n\t\t\t";
                    final += finalOutput;
                    hasComponent = true;
                }

                if (name.Contains("ListView") || name.Contains("listview"))
                {
                    var memberName = "ml_" + name;
                    var getComponentName = string.Empty;
                    getComponentName = "GetScrollRectListView(\"" + name + "\")";
                    var finalOutput = memberName + " = " + getComponentName + "; \n\t\t\t";
                    final += finalOutput;
                    hasComponent = true;
                }

                if (hasComponent == false || name.Contains("Go") || name.Contains("go"))
                {
                    var memberName = "m_" + name;
                    var getComponentName = string.Empty;
                    getComponentName = "GetGameObject(\"" + name + "\")";
                    var finalOutput = memberName + " = " + getComponentName + "; \n\t\t\t";
                    final += finalOutput;
                    hasComponent = true;
                }
            }

            var textEditor = new TextEditor();
            textEditor.text = final;
            textEditor.SelectAll();
            textEditor.Copy();
            return final;
        }
#endif

        #endregion
    }
}