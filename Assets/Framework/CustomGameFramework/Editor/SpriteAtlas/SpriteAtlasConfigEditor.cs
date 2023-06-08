/****************************************************
*	文件：SpriteAtlasConfigEditor.cs
*	作者：何明飞
*	邮箱：hemingfei@outlook.com
*	日期：2022/12/12 10:23:55
*	功能：暂无
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CustomGameFramework.Editor.SpriteAtlas
{
    [CustomEditor(typeof(SpriteAtlasConfig), false)]
    public class SpriteAtlasConfigEditor : UnityEditor.Editor
    {
        public const string SpriteAtlasFilePath = "Assets/Res/SpriteAtlas";
        private static SpriteAtlasConfig _instance;
        public static SpriteAtlasConfig Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = LoadInstance();
                }
                return _instance;
            }
        }
        
        private static SpriteAtlasConfig LoadInstance()
        {
            UnityEngine.Object obj = UnityEditor.AssetDatabase.LoadAssetAtPath(SpriteAtlasFilePath + "/SpriteAtlasConfig.asset", typeof(SpriteAtlasConfig));
            if (obj == null)
            {
                BuildProjectConfig();
                obj = UnityEditor.AssetDatabase.LoadAssetAtPath(SpriteAtlasFilePath + "/SpriteAtlasConfig.asset", typeof(SpriteAtlasConfig));
                if (obj == null)
                {
                    return null;
                }
            }
            _instance = obj as SpriteAtlasConfig;
            return _instance;
        }
        
        [MenuItem("Tools/Config SpriteAtlas \t 配置图集", false, 40)]
        public static void BuildProjectConfig()
        {
            SpriteAtlasConfig data = null;
            if (!Directory.Exists(SpriteAtlasFilePath))
            {
                Directory.CreateDirectory(SpriteAtlasFilePath);
            }
            string dataPath = SpriteAtlasFilePath + "/SpriteAtlasConfig.asset";
            data = AssetDatabase.LoadAssetAtPath<SpriteAtlasConfig>(dataPath);
            if (data == null)
            {
                data = ScriptableObject.CreateInstance<SpriteAtlasConfig>();
                AssetDatabase.CreateAsset(data, dataPath);
            }
            EditorUtility.SetDirty(data);
            AssetDatabase.SaveAssets();
            {
                // 选中
                Selection.activeObject = data;
                EditorGUIUtility.PingObject(data);
                GUIUtility.keyboardControl = 0;
                GUIUtility.hotControl = 0;
            }
        }
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("创建一个新的配置"))
                {
                    CreateNewSpriteAtlasUserConfig();
                }
                if (GUILayout.Button("获取文件夹下的配置文件"))
                {
                    AutoImportAllSpriteAtlasUserConfig();
                }
            }
            GUILayout.EndHorizontal();
            if (GUILayout.Button("修改保存"))
            {
                SaveInstance();
            }
            GUILayout.Space(30);
            GUILayout.Label("使用说明");
            GUILayout.Space(10);
            GUILayout.Label("1. 按模块等创建配置，并将配置文件拖入上方" + "\n2. 无需将图集文件打入AssetBundle");
            //
            //
            //
            GUILayout.Space(30);
            GUILayout.Label("图集操作");
            GUILayout.Space(10);
            GUIStyle btnStyle = new GUIStyle(EditorStyles.miniButton);
            btnStyle.fontSize = 20;
            btnStyle.fixedHeight = 40;
            if (GUILayout.Button("刷新现有图集", btnStyle))
            {
                SpriteAtlasBuilder.RefreshAllSpriteAtlas();
            }
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("删除现有图集"))
                {
                    DeleteAllSpriteAtlas();
                }
                if (GUILayout.Button("重新生成图集"))
                {
                    RepackAllSpriteAtlas();
                }
            }
            GUILayout.EndHorizontal();
        }
        
        // 创建一个新的用户配置
        private void CreateNewSpriteAtlasUserConfig()
        {
            string dataPath = SpriteAtlasFilePath + "/SpriteAtlasUserConfig-" + System.DateTime.Now.Ticks + ".asset";
            var dd = ScriptableObject.CreateInstance<SpriteAtlasUserConfig>();
            AssetDatabase.CreateAsset(dd, dataPath);
            var oldConfig = Instance.SpriteFoldersUserConfigs.ToList();
            oldConfig.Add(dd);
            Instance.SpriteFoldersUserConfigs = oldConfig.ToArray();
            EditorUtility.SetDirty(Instance);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void AutoImportAllSpriteAtlasUserConfig()
        {
            var fileFolderPath = SpriteAtlasFilePath.Substring(6);
            string[] allUserConfigPath = Directory.GetFiles(Application.dataPath + fileFolderPath, "*", SearchOption.AllDirectories);
            List<SpriteAtlasUserConfig> exitUserConfigs = new List<SpriteAtlasUserConfig>();
            foreach (string userConfigPath in allUserConfigPath)
            {
                string strTempPath = userConfigPath.Replace(@"\", "/");
                strTempPath = strTempPath.Substring(strTempPath.IndexOf("Assets"));
                //根据路径加载资源
                Object objUserConfig = AssetDatabase.LoadAssetAtPath(@strTempPath, typeof(Object));
                if (objUserConfig is SpriteAtlasUserConfig)
                {
                    exitUserConfigs.Add((SpriteAtlasUserConfig)objUserConfig);
                }
            }
            Instance.SpriteFoldersUserConfigs = exitUserConfigs.ToArray();
            SaveInstance();
        }
        
        private void SaveInstance()
        {
            EditorUtility.SetDirty(Instance);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            {
                // 选中
                Selection.activeObject = Instance;
                EditorGUIUtility.PingObject(Instance);
                GUIUtility.keyboardControl = 0;
                GUIUtility.hotControl = 0;
            }
        }
        
        public static void RepackAllSpriteAtlas()
        {
            int configLeng = SpriteAtlasConfigEditor.Instance.SpriteFoldersUserConfigs.Length;
            string outpath = Path.GetFullPath(SpriteAtlasFilePath);
            for (int i = 0; i < configLeng; i++)
            {
                var c = SpriteAtlasConfigEditor.Instance.SpriteFoldersUserConfigs[i];
                var folders = c.SpriteFolders.Select(AssetDatabase.GetAssetPath).ToArray();
                SpriteAtlasBuilder.PackageAllFoldersSprite(folders,c.name,outpath, SpriteAtlasBuilder.SpriteQualityLevel.Mid);
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static void DeleteAllSpriteAtlas()
        {
            SpriteAtlasBuilder.DeleteSpriteAtlasInSpecificFolders(new []{SpriteAtlasFilePath});
        }
    }
}


