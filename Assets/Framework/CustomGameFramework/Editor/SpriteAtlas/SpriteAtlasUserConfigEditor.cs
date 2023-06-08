/****************************************************
*	文件：SpriteAtlasConfigEditor.cs
*	作者：何明飞
*	邮箱：hemingfei@outlook.com
*	日期：2022/12/12 10:23:55
*	功能：暂无
*****************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CustomGameFramework.Editor.SpriteAtlas
{
    [CustomEditor(typeof(SpriteAtlasUserConfig), false)]
    public class SpriteAtlasUserConfigEditor : UnityEditor.Editor
    {
        private float _pngSize = 0;
        private float _pngCount = 0;
        private float _textureSize = 0;
        
        private Dictionary<string, Dictionary<string, string>> _spriteAtlasSizeData = new Dictionary<string, Dictionary<string, string>>();

        public void OnEnable()
        {
            PreCheckAsset();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("修改保存"))
            {
                EditorUtility.SetDirty(target);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                {
                    // 选中
                    Selection.activeObject = target;
                    EditorGUIUtility.PingObject(target);
                    GUIUtility.keyboardControl = 0;
                    GUIUtility.hotControl = 0;
                }
            }
            
            GUILayout.Space(30);
            GUILayout.Label("使用说明");
            GUILayout.Label("\n1. 将图片文件夹拖入" +
                            "\n2. 工具会自动遍历文件夹里的图片" +
                            "\n3. 每一个 SpriteAtlasUserConfig 会被打成一个图集");
            GUILayout.Space(30);
            
            GUILayout.Space(30);
            GUILayout.Label("图片个数：" + _pngCount);
            GUILayout.Label("源文件大小：" + _pngSize + "MB");
            GUILayout.Label("Texture大小：" + _textureSize + "MB");
            if (GUILayout.Button("检测图集资源"))
            {
                PreCheckAsset();
            }
            // 显示嵌套的字典
            GUILayout.BeginHorizontal();
            GUILayout.Label("File", GUILayout.Width(100));
            GUILayout.FlexibleSpace();
            GUILayout.Label("Texture Size", GUILayout.ExpandWidth(true));
            GUILayout.EndHorizontal();

            foreach (string key in _spriteAtlasSizeData.Keys)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(key);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                foreach (string innerKey in _spriteAtlasSizeData[key].Keys)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(20);
                    GUILayout.Label(innerKey);
                    GUILayout.FlexibleSpace();
                    GUILayout.Label(_spriteAtlasSizeData[key][innerKey]);
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.Space(30);
        }

        public void PreCheckAsset()
        {
            var folders = ((SpriteAtlasUserConfig)target).SpriteFolders.Select(AssetDatabase.GetAssetPath).ToArray();
            int totalCount = 0;
            long totalSize = 0;
            int totalTextureSize = 0;
            _spriteAtlasSizeData = new Dictionary<string, Dictionary<string, string>>();
            foreach (string folder in folders)
            {
                string folderName = folder.Split('/').Last();
                _spriteAtlasSizeData.Add(folderName, new Dictionary<string, string>());
            }
            foreach (string folder in folders)
            {
                if (Directory.Exists(folder))
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(folder);
                    FileInfo[] files = directoryInfo.GetFiles("*.png", SearchOption.AllDirectories);
                    string folderName = folder.Split('/').Last();
                    string folderPre = folder.Replace(folderName, "");
                    foreach (var file in files)
                    {
                        totalCount++;
                        totalSize += file.Length;
                        
                        string assetPath = file.FullName.Replace("\\", "/").Replace(Application.dataPath, "Assets");
                        TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
                        if (importer != null)
                        {
                            int width = 0;
                            int height = 0;
                            importer.GetSourceTextureWidthAndHeight(out width, out height);
                            int size = width * height;
                            totalTextureSize += size;
                            _spriteAtlasSizeData[folderName].Add(assetPath.Replace(folderPre, ""), (size/ 1024f).ToString("F2") + "KB");
                        }
                    }
                }
            }
            float sizeMB = totalSize / 1048576f;

            _pngCount = totalCount;
            _pngSize = sizeMB;
            _textureSize = totalTextureSize / 1048576f;
        }
    }
}


