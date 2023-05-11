//
//  UIBase.cs 
//
//  Author: HGT 
//
//  Copyright (c) 2021 hegametech.com 
//

using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace CustomGameFramework.Runtime
{
    public class UIBase : MonoBehaviour, IUILifeCycle
    {
        [HideInInspector] public Canvas m_canvas;

        #region GET SET UIName  UIID

        string m_UIName = null;

        public string UIName
        {
            get
            {
                if (m_UIName == null)
                {
                    m_UIName = name;
                }

                return m_UIName;
            }
            set { m_UIName = value; }
        }

        public int UIID { get; private set; } = -1;

        public string UIEventKey
        {
            get { return UIName + "@" + UIID; }
        }

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
                Log.Error("UIBase Dispose Exception -> UIEventKey: " + UIEventKey + " Exception: " + e.ToString());
            }

            DisposeLifeComponent();
        }

        #endregion

        #region 获取对象

        public List<GameObject> m_objectList = new List<GameObject>();

        //生成对象表，便于快速获取对象，并忽略层级
        void CreateObjectTable()
        {
            if (m_objects == null)
            {
                m_objects = new Dictionary<string, GameObject>();
            }

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

            for (int i = 0; i < m_objectList.Count; i++)
            {
                if (m_objectList[i] != null)
                {
                    if (m_objects.ContainsKey(m_objectList[i].name))
                    {
                        Log.Error(name + " CreateObjectTable Contains Duplicate Key ->" + m_objectList[i].name + "<-");
                    }
                    else
                    {
                        m_objects.Add(m_objectList[i].name, m_objectList[i]);
                    }
                }
                else
                {
                    Log.Error(name + " CreateObjectTable m_objectList[" + i + "] is Null !");
                }
            }
        }

        Dictionary<string, GameObject> m_objects = null;
        Dictionary<string, UIBase> m_uiBases = new Dictionary<string, UIBase>();

        Dictionary<string, Image> m_images = new Dictionary<string, Image>();
        Dictionary<string, Sprite> m_Sprites = new Dictionary<string, Sprite>();
        Dictionary<string, Text> m_texts = new Dictionary<string, Text>();
        Dictionary<string, TextMesh> m_textmeshs = new Dictionary<string, TextMesh>();
        Dictionary<string, UnityEngine.UI.Button> m_buttons = new Dictionary<string, UnityEngine.UI.Button>();
        Dictionary<string, ScrollRect> m_scrollRects = new Dictionary<string, ScrollRect>();
        Dictionary<string, RawImage> m_rawImages = new Dictionary<string, RawImage>();
        Dictionary<string, Transform> m_transforms = new Dictionary<string, Transform>();
        Dictionary<string, RectTransform> m_rectTransforms = new Dictionary<string, RectTransform>();
        Dictionary<string, InputField> m_inputFields = new Dictionary<string, InputField>();
        Dictionary<string, Slider> m_Sliders = new Dictionary<string, Slider>();
        Dictionary<string, Canvas> m_Canvas = new Dictionary<string, Canvas>();
        Dictionary<string, CanvasGroup> m_CanvasGroup = new Dictionary<string, CanvasGroup>();
        Dictionary<string, Toggle> m_Toggle = new Dictionary<string, Toggle>();

        public bool HaveObject(string name)
        {
            bool has = false;
            has = m_objects.ContainsKey(name);
            return has;
        }

        public Vector3 GetPosition(string name, bool islocal)
        {
            Vector3 tmp = Vector3.zero;
            GameObject go = GetGameObject(name);

            if (go != null)
            {
                if (islocal)
                {
                    tmp = GetGameObject(name).transform.localPosition;
                }
                else
                {
                    tmp = GetGameObject(name).transform.position;
                }
            }

            return tmp;
        }

        public GameObject GetGameObject(string name)
        {
            if (m_objects == null)
            {
                CreateObjectTable();
            }

            if (m_objects.ContainsKey(name))
            {
                GameObject go = m_objects[name];

                if (go == null)
                {
                    throw new Exception("UIWindowBase GetGameObject error: " + UIName + " m_objects[" + name +
                                        "] is null !!");
                }

                return go;
            }
            else
            {
                throw new Exception("UIWindowBase GetGameObject error: " + UIName + " dont find ->" + name + "<-");
            }
        }

        public Transform GetTransform(string name)
        {
            if (m_transforms.ContainsKey(name))
            {
                return m_transforms[name];
            }

            Transform tmp = GetGameObject(name).GetComponent<Transform>();

            if (tmp == null)
            {
                throw new Exception(UIEventKey + " GetTransform ->" + name + "<- is Null !");
            }

            m_transforms.Add(name, tmp);
            return tmp;
        }

        public RectTransform GetRectTransform(string name)
        {
            if (m_rectTransforms.ContainsKey(name))
            {
                return m_rectTransforms[name];
            }

            RectTransform tmp = GetGameObject(name).GetComponent<RectTransform>();

            if (tmp == null)
            {
                throw new Exception(UIEventKey + " GetRectTransform ->" + name + "<- is Null !");
            }

            m_rectTransforms.Add(name, tmp);
            return tmp;
        }

        public UIBase GetUIBase(string name)
        {
            if (m_uiBases.ContainsKey(name))
            {
                return m_uiBases[name];
            }

            UIBase tmp = GetGameObject(name).GetComponent<UIBase>();

            if (tmp == null)
            {
                throw new Exception(UIEventKey + " GetUIBase ->" + name + "<- is Null !");
            }

            m_uiBases.Add(name, tmp);
            return tmp;
        }

        public Sprite GetSprite(string name)
        {
            if (m_Sprites.ContainsKey(name))
            {
                return m_Sprites[name];
            }

            Sprite tmp = GetGameObject(name).GetComponent<Sprite>();

            if (tmp == null)
            {
                throw new Exception(UIEventKey + " GetImage ->" + name + "<- is Null !");
            }

            m_Sprites.Add(name, tmp);
            return tmp;
        }

        public Text GetText(string name)
        {
            if (m_texts.ContainsKey(name))
            {
                return m_texts[name];
            }

            Text tmp = GetGameObject(name).GetComponent<Text>();

            if (tmp == null)
            {
                throw new Exception(UIEventKey + " GetText ->" + name + "<- is Null !");
            }

            m_texts.Add(name, tmp);
            return tmp;
        }

        public TextMesh GetTextMesh(string name)
        {
            if (m_textmeshs.ContainsKey(name))
            {
                return m_textmeshs[name];
            }

            TextMesh tmp = GetGameObject(name).GetComponent<TextMesh>();

            if (tmp == null)
            {
                throw new Exception(UIEventKey + " GetTextMesh ->" + name + "<- is Null !");
            }

            m_textmeshs.Add(name, tmp);
            return tmp;
        }

        public Toggle GetToggle(string name)
        {
            if (m_Toggle.ContainsKey(name))
            {
                return m_Toggle[name];
            }

            Toggle tmp = GetGameObject(name).GetComponent<Toggle>();

            if (tmp == null)
            {
                throw new Exception(UIEventKey + " GetToggle ->" + name + "<- is Null !");
            }

            m_Toggle.Add(name, tmp);
            return tmp;
        }

        public UnityEngine.UI.Button GetButton(string name)
        {
            if (m_buttons.ContainsKey(name))
            {
                return m_buttons[name];
            }

            UnityEngine.UI.Button tmp = GetGameObject(name).GetComponent<UnityEngine.UI.Button>();

            if (tmp == null)
            {
                throw new Exception(UIEventKey + " GetButton ->" + name + "<- is Null !");
            }

            m_buttons.Add(name, tmp);
            return tmp;
        }

        public InputField GetInputField(string name)
        {
            if (m_inputFields.ContainsKey(name))
            {
                return m_inputFields[name];
            }

            InputField tmp = GetGameObject(name).GetComponent<InputField>();

            if (tmp == null)
            {
                throw new Exception(UIEventKey + " GetInputField ->" + name + "<- is Null !");
            }

            m_inputFields.Add(name, tmp);
            return tmp;
        }

        public ScrollRect GetScrollRect(string name)
        {
            if (m_scrollRects.ContainsKey(name))
            {
                return m_scrollRects[name];
            }

            ScrollRect tmp = GetGameObject(name).GetComponent<ScrollRect>();

            if (tmp == null)
            {
                throw new Exception(UIEventKey + " GetScrollRect ->" + name + "<- is Null !");
            }

            m_scrollRects.Add(name, tmp);
            return tmp;
        }

        public Image GetImage(string name)
        {
            if (m_images.ContainsKey(name))
            {
                return m_images[name];
            }

            Image tmp = GetGameObject(name).GetComponent<Image>();

            if (tmp == null)
            {
                throw new Exception(UIEventKey + " GetImage ->" + name + "<- is Null !");
            }

            m_images.Add(name, tmp);
            return tmp;
        }

        public RawImage GetRawImage(string name)
        {
            if (m_rawImages.ContainsKey(name))
            {
                return m_rawImages[name];
            }

            RawImage tmp = GetGameObject(name).GetComponent<RawImage>();

            if (tmp == null)
            {
                throw new Exception(UIEventKey + " GetRawImage ->" + name + "<- is Null !");
            }

            m_rawImages.Add(name, tmp);
            return tmp;
        }

        public Slider GetSlider(string name)
        {
            if (m_Sliders.ContainsKey(name))
            {
                return m_Sliders[name];
            }

            Slider tmp = GetGameObject(name).GetComponent<Slider>();

            if (tmp == null)
            {
                throw new Exception(UIEventKey + " GetSlider ->" + name + "<- is Null !");
            }

            m_Sliders.Add(name, tmp);
            return tmp;
        }

        public Canvas GetCanvas(string name)
        {
            if (m_Canvas.ContainsKey(name))
            {
                return m_Canvas[name];
            }

            Canvas tmp = GetGameObject(name).GetComponent<Canvas>();

            if (tmp == null)
            {
                throw new Exception(UIEventKey + " GetCanvas ->" + name + "<- is Null !");
            }

            m_Canvas.Add(name, tmp);
            return tmp;
        }

        public CanvasGroup GetCanvasGroup(string name)
        {
            if (m_CanvasGroup.ContainsKey(name))
            {
                return m_CanvasGroup[name];
            }

            CanvasGroup tmp = GetGameObject(name).GetComponent<CanvasGroup>();

            if (tmp == null)
            {
                throw new Exception(UIEventKey + " GetCanvasGroup ->" + name + "<- is Null !");
            }

            m_CanvasGroup.Add(name, tmp);
            return tmp;
        }

        private RectTransform m_rectTransform;

        public RectTransform RectTransform
        {
            get
            {
                if (m_rectTransform == null)
                {
                    m_rectTransform = transform as RectTransform;
                }

                return m_rectTransform;
            }
            set { m_rectTransform = value; }
        }

        public void SetSizeDelta(float w, float h)
        {
            RectTransform.sizeDelta = new Vector2(w, h);
        }

        #endregion

        #region 模型展示到UI上，UIModelShow

        private List<UIModelShowData> modelList = new List<UIModelShowData>();

        /// <summary>
        /// 创建UI的模型展示数据，可以设置相机数据
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
                    backgroundColor, localPosition, eulerAngles, localScale, texSize, nearClippingPlane, farClippingPlane, lightEulerAngles);
                modelList.Add(model);
            return model;
        }
        
        /// <summary>
        /// UI模型展示到UI上
        /// </summary>
        /// <param name="uiModelShowData">展示模型数据</param>
        /// <param name="showAreaRawImage">显示区域RawImage组件</param>
        /// <param name="dragAreaGameObject">拖拽区域GameObejct</param>
        /// <param name="isResetRotation">是否重置旋转</param>
        /// <param name="hideOtherUiModels">是否隐藏其他的模型，默认TRUE，性能会得到优化</param>
        public void UIModelShow(UIModelShowData uiModelShowData, RawImage showAreaRawImage, GameObject dragAreaGameObject, bool isResetRotation = true, bool hideOtherUiModels = true)
        {
            if (hideOtherUiModels)
            {
                foreach (var model in modelList)
                {
                    if (model != uiModelShowData)
                    {
                        model.camera.gameObject.SetActive(false);
                    }
                }
            }

            if (isResetRotation)
            {
                uiModelShowData.model.transform.localRotation = Quaternion.identity;
            }
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
        /// UI模型展示 添加 转动功能
        /// </summary>
        /// <param name="uiModelShowData"></param>
        /// <param name="dragAreaGameObject"></param>
        public void UIModelShowAddDrag(UIModelShowData uiModelShowData, GameObject dragAreaGameObject)
        {
            if (dragAreaGameObject != null)
            {
                UIModelShowTool.AddDrag(dragAreaGameObject, uiModelShowData.model);
            }
        }

        /// <summary>
        /// 清除UI模型展示的数据和资源
        /// </summary>
        /// <param name="data"></param>
        /// <param name="dragAreaGameObject">对应的拖拽区域，需要取消注册拖拽事件，不传入会报错</param>
        public void CleanUIModelShowData(UIModelShowData data, GameObject dragAreaGameObject)
        {
            modelList.Remove(data);
            UIModelShowTool.DisposeModelShow(data);
            if (dragAreaGameObject != null)
            {
                UIModelShowTool.RemoveDrag(dragAreaGameObject);
            }
        }
        
        /// <summary>
        /// 清除UI模型展示的全部数据和资源，销毁窗口会自动调用,不要手动调用
        /// </summary>
        public void CleanUIModelShowDatas()
        {
            for (int i = 0; i < modelList.Count; i++)
            {
                UIModelShowTool.DisposeModelShow(modelList[i]);
            }

            modelList.Clear();
        }

        #endregion

        #region 生命周期管理

        protected List<IUILifeCycle> m_lifeComponent = new List<IUILifeCycle>();

        public void AddLifeCycleComponent(IUILifeCycle comp)
        {
            comp.Init(UIEventKey, m_lifeComponent.Count);
            m_lifeComponent.Add(comp);
        }

        void DisposeLifeComponent()
        {
            for (int i = 0; i < m_lifeComponent.Count; i++)
            {
                try
                {
                    m_lifeComponent[i].Dispose();
                }
                catch (Exception e)
                {
                    Log.Error("UIBase DisposeLifeComponent Exception -> UIEventKey: " + UIEventKey + " Exception: " +
                              e.ToString());
                }
            }

            m_lifeComponent.Clear();
        }

        #endregion

        #region Editor 工具方法

#if UNITY_EDITOR

        [ContextMenu("ObjectList 去重")]
        public void ClearObject()
        {
            List<GameObject> ls = new List<GameObject>();
            int len = m_objectList.Count;

            for (int i = 0; i < len; i++)
            {
                GameObject go = m_objectList[i];

                if (go != null)
                {
                    if (!ls.Contains(go))
                    {
                        ls.Add(go);
                    }
                }
            }

            ls.Sort((a, b) => { return a.name.CompareTo(b.name); });
            m_objectList = ls;
        }

        [ContextMenu("UIBase 快速提取字段 private TYPE m_xxx;")]
        public string EditorFastGetMembers()
        {
            ClearObject();
            int len = m_objectList.Count;
            string final = string.Empty;

            for (int i = 0; i < len; i++)
            {
                GameObject go = m_objectList[i];
                string name = go.name;
                bool hasComponent = false;

                if (name.Contains("Image") || name.Contains("image"))
                {
                    string memberName = "mi_" + name;
                    string typeName = string.Empty;
                    typeName = "private Image ";
                    string finalOutput = typeName + memberName + "; \n\t\t";
                    final += finalOutput;
                    hasComponent = true;
                }

                if (name.Contains("Text") || name.Contains("text"))
                {
                    string memberName = "mt_" + name;
                    string typeName = string.Empty;
                    typeName = "private Text ";
                    string finalOutput = typeName + memberName + "; \n\t\t";
                    final += finalOutput;
                    hasComponent = true;
                }

                if (name.Contains("Button") || name.Contains("button"))
                {
                    string memberName = "mb_" + name;
                    string typeName = string.Empty;
                    typeName = "private Button ";
                    string finalOutput = typeName + memberName + "; \n\t\t";
                    final += finalOutput;
                    hasComponent = true;
                }

                if (name.Contains("ListView") || name.Contains("listview"))
                {
                    string memberName = "ml_" + name;
                    string typeName = string.Empty;
                    typeName = "private IUListView ";
                    string finalOutput = typeName + memberName + "; \n\t\t";
                    final += finalOutput;
                    hasComponent = true;
                }

                if (hasComponent == false || name.Contains("Go") || name.Contains("go"))
                {
                    string memberName = "m_" + name;
                    string typeName = string.Empty;
                    typeName = "private GameObject ";
                    string finalOutput = typeName + memberName + "; \n\t\t";
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
            int len = m_objectList.Count;
            string final = string.Empty;

            for (int i = 0; i < len; i++)
            {
                GameObject go = m_objectList[i];
                string name = go.name;
                bool hasComponent = false;

                if (name.Contains("Image") || name.Contains("image"))
                {
                    string memberName = "mi_" + name;
                    string getComponentName = string.Empty;
                    getComponentName = "GetImage(\"" + name + "\")";
                    string finalOutput = memberName + " = " + getComponentName + "; \n\t\t\t";
                    final += finalOutput;
                    hasComponent = true;
                }

                if (name.Contains("Text") || name.Contains("text"))
                {
                    string memberName = "mt_" + name;
                    string getComponentName = string.Empty;
                    getComponentName = "GetText(\"" + name + "\")";
                    string finalOutput = memberName + " = " + getComponentName + "; \n\t\t\t";
                    final += finalOutput;
                    hasComponent = true;
                }

                if (name.Contains("Button") || name.Contains("button"))
                {
                    string memberName = "mb_" + name;
                    string getComponentName = string.Empty;
                    getComponentName = "GetButton(\"" + name + "\")";
                    string finalOutput = memberName + " = " + getComponentName + "; \n\t\t\t";
                    final += finalOutput;
                    hasComponent = true;
                }

                if (name.Contains("ListView") || name.Contains("listview"))
                {
                    string memberName = "ml_" + name;
                    string getComponentName = string.Empty;
                    getComponentName = "GetScrollRectListView(\"" + name + "\")";
                    string finalOutput = memberName + " = " + getComponentName + "; \n\t\t\t";
                    final += finalOutput;
                    hasComponent = true;
                }

                if (hasComponent == false || name.Contains("Go") || name.Contains("go"))
                {
                    string memberName = "m_" + name;
                    string getComponentName = string.Empty;
                    getComponentName = "GetGameObject(\"" + name + "\")";
                    string finalOutput = memberName + " = " + getComponentName + "; \n\t\t\t";
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
