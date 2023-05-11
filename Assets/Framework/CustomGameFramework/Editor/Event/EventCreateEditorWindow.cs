/****************************************************
*	文件：EventCreateEditorWindow.cs
*	作者：何明飞
*	邮箱：hemingfei@outlook.com
*	日期：2022/11/24 20:23:26
*	功能：暂无
*****************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace CustomGameFramework.Editor.Event
{
    /// <summary>
/// 自动生成事件代码
/// </summary>
public class EventCreateEditorWindow : EditorWindow
{
    [Serializable]
    public struct EventArgs
    {
        public string ArgName;
        public string ArgDes;
        public string ArgType;
    }

    // 模板的路径
    private const string _templateFilePath =
        "Template/EventTemplate";

    // 文件生成的路径
    private const string _defaultGeneratedFilePath = "Assets/";

    // 默认的命名空间
    private const string _defaultNamespace = "GameFramework.Event";

    // 填写的内容
    private string _eventName;
    private string _eventDes;
    [SerializeField] private List<EventArgs> _eventArgs = new List<EventArgs>();
    private string _generatedFilePath;
    private string _eventNamespace;

    // 序列化的内容
    private TextAsset _templateTextAsset;
    private SerializedProperty _serializedArgs;
    private SerializedObject _serializedObject;

    // editor pref
    private const string EditorPrefsEventGeneratedFilePath = "EventCreate_generatedFilePath";
    private const string EditorPrefsEventNameSpace = "EventCreate_namespace";

    private Vector2 _scrollValue;
    
    [MenuItem("Tools/Create Event \t\t\t 创建事件", false, 30)]
    private static void ShowWindow()
    {
        EventCreateEditorWindow window = (EventCreateEditorWindow)GetWindow(typeof(EventCreateEditorWindow));
        window.titleContent = new GUIContent("Event编辑器");
        window.OnOpen();
        window.Show();
    }

    private void OnOpen()
    {
        _serializedObject = new SerializedObject(this);
        _serializedArgs = _serializedObject.FindProperty("_eventArgs");
        _templateTextAsset = Resources.Load<TextAsset>(_templateFilePath);
        _generatedFilePath = EditorPrefs.GetString(EditorPrefsEventGeneratedFilePath);
        if (string.IsNullOrEmpty(_generatedFilePath))
        {
            _generatedFilePath = _defaultGeneratedFilePath;
        }

        _eventNamespace = EditorPrefs.GetString(EditorPrefsEventNameSpace);
        if (string.IsNullOrEmpty(_eventNamespace))
        {
            _eventNamespace = _defaultNamespace;
        }

    }

    private void OnGUI()
    {
        //_serializedObject = new SerializedObject(this);
        //_serializedArgs = _serializedObject.FindProperty("_eventArgs");
        
        _scrollValue = EditorGUILayout.BeginScrollView(_scrollValue);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("事件参数", EditorStyles.largeLabel);
        _eventName = EditorGUILayout.TextField("事件名", _eventName);
        _eventDes = EditorGUILayout.TextField("事件描述", _eventDes);
        EditorGUILayout.PropertyField(_serializedArgs);
        _serializedObject.ApplyModifiedProperties();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("生成信息", EditorStyles.largeLabel);
        //EditorGUILayout.ObjectField("模板", _templateTextAsset, typeof(TextAsset), false);
        _generatedFilePath = EditorGUILayout.TextField("生成路径", _generatedFilePath);
        _eventNamespace = EditorGUILayout.TextField("命名空间", _eventNamespace);

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        // final
        EditorGUILayout.LabelField("最终文件", EditorStyles.largeLabel);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("文件名", _eventName + "EventArgs.cs");
        if (_generatedFilePath.Length > 7)
        {
            EditorGUILayout.LabelField("文件路径", Application.dataPath + "/" + _generatedFilePath.Substring(7));
        }
        else
        {
            EditorGUILayout.LabelField("文件路径", Application.dataPath + "/" + _generatedFilePath.Replace("Assets/",string.Empty));
        }
        
        // button
        EditorGUILayout.Space();
        Color bc = GUI.backgroundColor;
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("生成"))
        {
            if (string.IsNullOrEmpty(_eventName))
            {
                ShowNotification(new GUIContent("事件名称不能为空"));
                EditorGUILayout.EndScrollView();
                return;
            }
            if (string.IsNullOrEmpty(_eventDes))
            {
                ShowNotification(new GUIContent("事件描述不能为空"));
                EditorGUILayout.EndScrollView();
                return;
            }
            FormatParams();
            EventCreate();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Close();
        }

        GUI.backgroundColor = bc;
        EditorGUILayout.EndScrollView();
    }

    private void FormatParams()
    {
        _eventName = _eventName.Trim();
        _eventDes = _eventDes.Trim();
        _generatedFilePath = _generatedFilePath.Trim();
        _eventNamespace = _eventNamespace.Trim();

        EditorPrefs.SetString(EditorPrefsEventGeneratedFilePath, _generatedFilePath);
        EditorPrefs.SetString(EditorPrefsEventNameSpace, _eventNamespace);
    }

    private void EventCreate()
    {

        string scriptPath = "";
        if (_generatedFilePath.Length > 7)
        {
            scriptPath = Application.dataPath + "/" + _generatedFilePath.Substring(7) + "/" + _eventName +
                         "EventArgs.cs";
        }
        else
        {
            scriptPath = Application.dataPath + "/" + _generatedFilePath.Replace("Assets/",string.Empty) + "/" + _eventName + "EventArgs.cs";
        }

        string classStr = "";

        if (File.Exists(scriptPath))
        {
            File.Delete(scriptPath);
            File.Delete(scriptPath.Replace(".cs", ".meta"));
        }

        classStr = _templateTextAsset.text;

        //Namespace
        classStr = classStr.Replace("{EventNameSpace}", _eventNamespace);
        //EventName
        classStr = classStr.Replace("{EventName}", _eventName + "EventArgs");
        //EventDes
        classStr = classStr.Replace("{EventDes}", _eventDes);
        //EventArgs
        string s = "";
        foreach (EventArgs arg in _eventArgs)
        {
            s += ("\n\t\t\t" + arg.ArgName.Substring(0, 1).ToUpper() + arg.ArgName.Substring(1) + " = default;");
        }

        classStr = classStr.Replace("{EventArgs}", s);
        //EventGetSet
        s = "";
        foreach (EventArgs arg in _eventArgs)
        {
            s += "\n";
            string _argname = arg.ArgName;
            _argname = _argname.Substring(0, 1).ToUpper() + _argname.Substring(1);
            s += "\n		" + "/// <summary>\n";
            s += "		" + "/// " + arg.ArgDes + "\n";
            s += "		" + "/// </summary>" + "\n";
            s += "		" + "public " + arg.ArgType + " " + _argname + " { get; private set; }";
        }
        classStr = classStr.Replace("{EventGetSet}", s);
        //EventParams
        s = "";
        for (int i = 0; i < _eventArgs.Count; i++)
        {
            string _argname = _eventArgs[i].ArgName;
            _argname = _argname.Substring(0, 1).ToLower() + _argname.Substring(1);
            string _argDes = _eventArgs[i].ArgDes;
            s += "\n\t\t/// <param name=\"" + _argname + "\">" + _argDes + "</param>";
        }

        classStr = classStr.Replace("{EventParams}", s);
        //EventCreateArgs
        s = "";
        for (int i = 0; i < _eventArgs.Count; i++)
        {
            string _argname = _eventArgs[i].ArgName;
            _argname = _argname.Substring(0, 1).ToLower() + _argname.Substring(1);
            s += _eventArgs[i].ArgType + " " + _argname;
            if (i != _eventArgs.Count - 1)
            {
                s += ", ";
            }
        }

        classStr = classStr.Replace("{EventCreateArgs}", s);
        //EventCreate
        s = "";
        foreach (EventArgs arg in _eventArgs)
        {
            s += "\n";
            string _argname = arg.ArgName;
            string upper, lower;
            upper = _argname.Substring(0, 1).ToUpper() + _argname.Substring(1);
            lower = _argname.Substring(0, 1).ToLower() + _argname.Substring(1);
            s += "			e." + upper + " = " + lower + ";";
        }

        classStr = classStr.Replace("{EventCreate}", s);

        string dirPath = scriptPath.Substring(0, scriptPath.LastIndexOf("/"));
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }
        
        // write
        using (FileStream file = new FileStream(scriptPath, FileMode.CreateNew))
        {
            using (StreamWriter fileW = new StreamWriter(file, System.Text.Encoding.UTF8))
            {
                fileW.Write(classStr);
                fileW.Flush();
                fileW.Close();
            }
            file.Close();
        }
    }
}
}

