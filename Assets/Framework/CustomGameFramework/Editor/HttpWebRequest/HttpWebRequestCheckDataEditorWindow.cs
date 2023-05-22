/****************************************************
*	文件：WebRequestCheckDataEditorWindow.cs
*	作者：何明飞
*	邮箱：hemingfei@outlook.com
*	日期：2023/02/20 19:32:17
*	功能：暂无
*****************************************************/

using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace CustomGameFramework.Editor
{
    /// <summary>
    ///     Description of WebRequestCheckDataEditorWindow
    /// </summary>
    public class WebRequestCheckDataEditorWindow : EditorWindow
    {
        public static HttpWebRequestCreateEditorWindow CreateWindow;
        private string[] _currentDatas;
        private Dictionary<string, string> _currentDatasDict;
        private Vector2 _scrollValue;

        private void OnEnable()
        {
            if (Directory.Exists(HttpWebRequestCreateEditorWindow.GenerateRootFolder))
            {
                var dir = new DirectoryInfo(HttpWebRequestCreateEditorWindow.GenerateRootFolder);

                var logicEx = ".data.send.data";
                //获取目标路径下的所有文件
                var allFiles = dir.GetFiles("*" + logicEx, SearchOption.AllDirectories);

                var logicExFiles = new List<string>();
                _currentDatasDict = new Dictionary<string, string>();
                foreach (var file in allFiles)
                {
                    //忽略.meta
                    if (file.Name.EndsWith(".meta")) continue;
                    //返回相对路径
                    var assetsName = file.FullName;
                    assetsName = assetsName.Substring(assetsName.IndexOf("Assets")).Replace("\\", "/");
                    var urlPath = assetsName.Replace(HttpWebRequestCreateEditorWindow.GenerateRootFolder, string.Empty)
                        .Replace("/" + file.Name, string.Empty);
                    logicExFiles.Add(urlPath);
                    _currentDatasDict.Add(urlPath, assetsName);
                }

                _currentDatas = logicExFiles.ToArray();
            }
        }

        private void OnGUI()
        {
            EditorGUILayout.Space(10);
            _scrollValue = GUILayout.BeginScrollView(_scrollValue);
            for (var i = 0; i < _currentDatas.Length; i++)
                if (GUILayout.Button(_currentDatas[i]))
                {
                    Debug.Log(_currentDatasDict[_currentDatas[i]]);
                    CreateWindow.ReadCurrentConfig(_currentDatas[i]);
                }

            GUILayout.EndScrollView();
            EditorGUILayout.Space(10);


            GUILayout.BeginHorizontal();
            var bc = GUI.backgroundColor;
            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("全部重新生成请求代码"))
            {
                var ignoreFiles = GetIngorePathes();
                for (var i = 0; i < _currentDatas.Length; i++)
                {
                    var path = _currentDatas[i];
                    if (ignoreFiles.Contains(path))
                    {
                        Debug.LogWarning("自动生成，跳过接口：" + path);
                        continue;
                    }

                    Debug.Log(_currentDatasDict[_currentDatas[i]]);
                    CreateWindow.WriteCurrentSendConfig(_currentDatas[i]);
                }

                AssetDatabase.Refresh();
            }

            if (GUILayout.Button("全部重新生成返回数据"))
            {
                var ignoreFiles = GetIngorePathes();
                for (var i = 0; i < _currentDatas.Length; i++)
                {
                    var path = _currentDatas[i];
                    if (ignoreFiles.Contains(path))
                    {
                        Debug.LogWarning("自动生成，跳过接口：" + path);
                        continue;
                    }

                    Debug.Log(_currentDatasDict[_currentDatas[i]]);
                    CreateWindow.WriteCurrentReceiveConfig(_currentDatas[i]);
                }

                AssetDatabase.Refresh();
            }

            GUILayout.EndHorizontal();
            if (GUILayout.Button("全部重新生成封装的快速操作代码"))
            {
                var ignoreFiles = GetIngorePathes();
                for (var i = 0; i < _currentDatas.Length; i++)
                {
                    var path = _currentDatas[i];
                    if (ignoreFiles.Contains(path))
                    {
                        Debug.LogWarning("自动生成，跳过接口：" + path);
                        continue;
                    }

                    Debug.Log(_currentDatasDict[_currentDatas[i]]);
                    CreateWindow.WriteCurrentCreateConfig(_currentDatas[i]);
                }

                AssetDatabase.Refresh();
            }

            GUI.backgroundColor = bc;
        }

        public static WebRequestCheckDataEditorWindow ShowWindow(HttpWebRequestCreateEditorWindow createWindow)
        {
            var window = (WebRequestCheckDataEditorWindow)GetWindow(typeof(WebRequestCheckDataEditorWindow));
            window.titleContent = new GUIContent("WebRequest现有接口");
            window.Show();
            CreateWindow = createWindow;
            return window;
        }

        private List<string> GetIngorePathes()
        {
            var ignoreFiles = new List<string>();
            using (var oldtxt = new FileStream(HttpWebRequestCreateEditorWindow.GenerateRootFolder + "/ignore.txt",
                       FileMode.OpenOrCreate))
            using (var sr = new StreamReader(oldtxt))
            {
                while (true)
                {
                    var line = sr.ReadLine();
                    if (line == null) break;

                    if (line.Contains("#")) continue;

                    ignoreFiles.Add(line);
                }

                sr.Close();
            }

            return ignoreFiles;
        }
    }
}