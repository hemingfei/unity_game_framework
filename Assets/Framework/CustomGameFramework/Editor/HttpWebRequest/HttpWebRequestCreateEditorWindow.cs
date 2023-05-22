/****************************************************
*	文件：HttpWebRequestCreateEditorWindow.cs
*	作者：何明飞
*	邮箱：hemingfei@outlook.com
*	日期：2023/02/08 10:21:48
*	功能：暂无
*****************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace CustomGameFramework.Editor
{
    /// <summary>
    ///     Description of HttpWebRequestCreateEditorWindow
    /// </summary>
    public partial class HttpWebRequestCreateEditorWindow : EditorWindow
    {
        public enum HTTPType
        {
            POST,
            GET
        }

        public const string GenerateRootFolder = "Assets/Scripts/WebRequest/Generate";

        public static WebRequestCheckDataEditorWindow _currentDataEditorWindow;
        [SerializeField] private HTTPType _webRequestType;

        [SerializeField] private List<StringArgs> _headers = new();

        [SerializeField] private List<MemberArgs> _body = new();

        [SerializeField] private List<StringArgs> _query = new();

        [SerializeField] private List<MemberArgs> _resultData = new();

        [SerializeField] private List<ClassArgs> _resultInnerData = new();
        private string _apiUpTime;
        private string _apiUrlCode;
        private bool _modifyBody;

        private bool _modifyHeaders;
        private bool _modifyQuery;

        private string _resultDataType;

        private Vector2 _scrollValue;
        private SerializedProperty _serializedBody;
        private SerializedProperty _serializedHeaders;

        private SerializedObject _serializedObject;
        private SerializedProperty _serializedQuery;
        private SerializedProperty _serializedResultData;
        private SerializedProperty _serializedResultInnerData;
        private SerializedProperty _serializedWebRequestType;

        // 填写的内容
        private string _webRequestName;
        private string _webRequestPath;

        private void OnEnable()
        {
            _serializedObject = new SerializedObject(this);
            _serializedWebRequestType = _serializedObject.FindProperty("_webRequestType");
            _serializedHeaders = _serializedObject.FindProperty("_headers");
            _serializedBody = _serializedObject.FindProperty("_body");
            _serializedQuery = _serializedObject.FindProperty("_query");
            _serializedResultData = _serializedObject.FindProperty("_resultData");
            _serializedResultInnerData = _serializedObject.FindProperty("_resultInnerData");
        }

        private void OnDisable()
        {
            _currentDataEditorWindow?.Close();
        }

        private void OnGUI()
        {
            if (GUILayout.Button("查看现有接口配置"))
                _currentDataEditorWindow = WebRequestCheckDataEditorWindow.ShowWindow(this);
            if (!string.IsNullOrEmpty(_webRequestPath))
            {
                EditorGUILayout.Space(10);
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("读取现有请求参数配置"))
                {
                    ReadCurrentSendConfig();
                    GUIUtility.keyboardControl = 0;
                    GUIUtility.hotControl = 0;
                }

                if (GUILayout.Button("读取现有返回数据配置"))
                {
                    ReadCurrentReceiveConfig();
                    GUIUtility.keyboardControl = 0;
                    GUIUtility.hotControl = 0;
                }

                GUILayout.EndHorizontal();
            }

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("基本信息", EditorStyles.boldLabel);

            EditorGUILayout.Space(10);
            _webRequestName = EditorGUILayout.TextField("接口名称", _webRequestName);
            _webRequestPath = EditorGUILayout.TextField("接口路径", _webRequestPath);
            EditorGUILayout.PropertyField(_serializedWebRequestType);
            EditorGUILayout.Space(15);

            _scrollValue = EditorGUILayout.BeginScrollView(_scrollValue);

            EditorGUILayout.LabelField("请求参数", EditorStyles.boldLabel);

            EditorGUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();
            var originalValue = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 50;

            _modifyHeaders = EditorGUILayout.ToggleLeft("Add Headers", _modifyHeaders);

            _modifyBody = EditorGUILayout.ToggleLeft("Add Body", _modifyBody);

            _modifyQuery = EditorGUILayout.ToggleLeft("Add Query", _modifyQuery);

            EditorGUIUtility.labelWidth = originalValue;
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(10);
            if (_modifyHeaders) EditorGUILayout.PropertyField(_serializedHeaders);
            if (_modifyBody) EditorGUILayout.PropertyField(_serializedBody);
            if (_modifyQuery) EditorGUILayout.PropertyField(_serializedQuery);

            EditorGUILayout.Space(10);
            var bc = GUI.backgroundColor;
            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("生成请求代码"))
            {
                OnGenerateRequestCodeBtnClicked();
                AssetDatabase.Refresh();
                GUIUtility.keyboardControl = 0;
                GUIUtility.hotControl = 0;
            }

            GUI.backgroundColor = bc;

            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("生成封装的快速操作代码（需要已经生成返回数据后再操作此步）"))
            {
                OnGenerateRequestCreateCodeBtnClicked();
                AssetDatabase.Refresh();
                GUIUtility.keyboardControl = 0;
                GUIUtility.hotControl = 0;
            }

            GUI.backgroundColor = bc;

            EditorGUILayout.Space(15);
            EditorGUILayout.LabelField("返回数据", EditorStyles.boldLabel);
            EditorGUILayout.Space(10);
            EditorGUILayout.PropertyField(_serializedResultData);
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("返回数据中的其他类型", EditorStyles.label);
            EditorGUILayout.PropertyField(_serializedResultInnerData);
            EditorGUILayout.Space(10);

            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("生成返回数据"))
            {
                OnGenerateReturnDataBtnClicked();
                AssetDatabase.Refresh();
                GUIUtility.keyboardControl = 0;
                GUIUtility.hotControl = 0;
            }

            GUI.backgroundColor = bc;

            EditorGUILayout.Space(15);
            EditorGUILayout.EndScrollView();
            _serializedObject.ApplyModifiedProperties();
        }

        [MenuItem("Tools/Create WebRequest \t 创建接口", false, 30)]
        public static HttpWebRequestCreateEditorWindow ShowWindow()
        {
            var window = (HttpWebRequestCreateEditorWindow)GetWindow(typeof(HttpWebRequestCreateEditorWindow));
            window.titleContent = new GUIContent("WebRequest接口代码创建");
            window.Show();
            return window;
        }

        public void ReadCurrentConfig(string webRequestPath)
        {
            _webRequestPath = webRequestPath;
            ReadCurrentSendConfig();
            ReadCurrentReceiveConfig();
        }

        public void WriteCurrentSendConfig(string webRequestPath)
        {
            _webRequestPath = webRequestPath;
            ReadCurrentSendConfig();
            OnGenerateRequestCodeBtnClicked();
        }

        public void WriteCurrentCreateConfig(string webRequestPath)
        {
            _webRequestPath = webRequestPath;
            ReadCurrentSendConfig();
            ReadCurrentReceiveConfig();
            OnGenerateRequestCreateCodeBtnClicked();
        }

        public void WriteCurrentReceiveConfig(string webRequestPath)
        {
            _webRequestPath = webRequestPath;
            ReadCurrentReceiveConfig();
            OnGenerateReturnDataBtnClicked();
        }

        private void ReadCurrentSendConfig()
        {
            var folderNames = _webRequestPath.Split('/');
            var folderName = folderNames[^1];
            var exportPath = GenerateRootFolder + _webRequestPath + "/" + folderName + ".data.send.data";
            if (File.Exists(exportPath))
            {
                var json = Encoding.UTF8.GetString(File.ReadAllBytes(exportPath));
                var data = JsonUtility.FromJson<RequestData_data>(json);
                if (data != null)
                {
                    _webRequestPath = data.webRequestPath;
                    _webRequestName = data.webRequestName;
                    _webRequestType = data.webRequestType;
                    _modifyHeaders = data.modifyHeaders;
                    _modifyBody = data.modifyBody;
                    _modifyQuery = data.modifyQuery;
                    _headers = data.headers;
                    _body = data.body;
                    _query = data.query;
                    _apiUrlCode = data.urlId;
                    _apiUpTime = data.upTime;
                    OnEnable();
                    ShowNotification(new GUIContent("读取成功"));
                }
                else
                {
                    ShowNotification(new GUIContent("读取失败"));
                }
            }
            else
            {
                ShowNotification(new GUIContent("不存在请求参数配置"));
            }
        }

        private void ReadCurrentReceiveConfig()
        {
            var folderNames = _webRequestPath.Split('/');
            var folderName = folderNames[^1];
            var exportPath = GenerateRootFolder + _webRequestPath + "/" + folderName + ".data.receive.data";
            if (File.Exists(exportPath))
            {
                var json = Encoding.UTF8.GetString(File.ReadAllBytes(exportPath));
                var data = JsonUtility.FromJson<ReturnData_data>(json);
                if (data != null)
                {
                    _webRequestPath = data.webRequestPath;
                    _resultData = data.resultData;
                    _resultInnerData = data.resultInnerData;
                    _resultDataType = data.resultType;
                    _apiUrlCode = data.urlId;
                    _apiUpTime = data.upTime;
                    OnEnable();
                    ShowNotification(new GUIContent("读取成功"));
                }
                else
                {
                    ShowNotification(new GUIContent("读取失败"));
                }
            }
            else
            {
                ShowNotification(new GUIContent("不存在返回数据配置"));
            }
        }

        /// <summary>
        ///     生成请求代码
        /// </summary>
        private void OnGenerateRequestCodeBtnClicked()
        {
            if (string.IsNullOrEmpty(_webRequestPath) ||
                string.IsNullOrEmpty(Regex.Replace(_webRequestPath, @"\s", "")))
            {
                ShowNotification(new GUIContent("接口路径不存在"));
                return;
            }

            GenerateRequestData(_webRequestPath, _webRequestName, _webRequestType, _modifyHeaders, _modifyBody,
                _modifyQuery, _headers, _body, _query, _apiUrlCode, _apiUpTime);

            GenerateRequestData_data(_webRequestPath, _webRequestName, _webRequestType, _modifyHeaders, _modifyBody,
                _modifyQuery, _headers, _body, _query, _apiUrlCode, _apiUpTime);
        }

        /// <summary>
        ///     生成返回数据
        /// </summary>
        private void OnGenerateReturnDataBtnClicked()
        {
            if (string.IsNullOrEmpty(_webRequestPath) ||
                string.IsNullOrEmpty(Regex.Replace(_webRequestPath, @"\s", "")))
            {
                ShowNotification(new GUIContent("接口路径不存在"));
                return;
            }

            GenerateReturnData(_webRequestPath, _resultData, _resultInnerData, _resultDataType, _apiUrlCode,
                _apiUpTime);

            GenerateReturnData_data(_webRequestPath, _resultData, _resultInnerData, _resultDataType, _apiUrlCode,
                _apiUpTime);
        }

        /// <summary>
        ///     生成请求代码
        /// </summary>
        private void OnGenerateRequestCreateCodeBtnClicked()
        {
            if (string.IsNullOrEmpty(_webRequestPath) ||
                string.IsNullOrEmpty(Regex.Replace(_webRequestPath, @"\s", "")))
            {
                ShowNotification(new GUIContent("接口路径不存在"));
                return;
            }

            GenerateRequestData_cs_create(_webRequestPath, _webRequestName, _webRequestType, _modifyHeaders,
                _modifyBody,
                _modifyQuery, _headers, _body, _query, _resultDataType, _apiUrlCode, _apiUpTime);
        }

        [Serializable]
        public struct StringArgs
        {
            public string Name;
            public string Des;
        }

        [Serializable]
        public struct MemberArgs
        {
            public string Name;
            public string Type;
            public string Des;
        }

        [Serializable]
        public struct ClassArgs
        {
            public string TypeName;
            public List<MemberArgs> Members;
        }
    }
}