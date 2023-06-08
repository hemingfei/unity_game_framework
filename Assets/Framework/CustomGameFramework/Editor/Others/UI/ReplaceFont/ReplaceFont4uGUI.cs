/****************************************************
*	文件：ReplaceFont4uGUI.cs
*	作者：hemingfei
*	邮箱：hemingfei@outlook.com
*	日期：2023/05/22 09:16:55
*	功能：暂无
*****************************************************/

/****************************************************
*	文件：ReplaceFont4uGUI.cs
*	作者：hemingfei
*	邮箱：hemingfei@outlook.com
*	日期：2023/05/12 11:16:11
*	功能：暂无
*****************************************************/

//
//  ReplaceFont4uGUI.cs 
//
//  Author: He Mingfei <hemingfei@outlook.com> 
//
//  Copyright (c) 2021 hegametech.com 
//

using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Hegametech.Framework.Editor
{
    public class ReplaceFont4uGUI : EditorWindow
    {
        private Font matchingfont;
        private Font replaceFont;

        private void OnGUI()
        {
            var selectObjs = Selection.GetFiltered(typeof(GameObject), SelectionMode.DeepAssets);
            GUILayout.Label("选中数目：" + selectObjs.Length);
            matchingfont = (Font)EditorGUILayout.ObjectField("需要替换的字体：", matchingfont, typeof(Font), true);
            EditorGUILayout.Separator();
            replaceFont = (Font)EditorGUILayout.ObjectField("替换的字体：", replaceFont, typeof(Font), true);
            EditorGUILayout.Separator();
            GUILayout.Space(15);

            if (GUILayout.Button("替换"))
            {
                var num = CorrectionPublicFont(replaceFont, matchingfont);
                EditorUtility.DisplayDialog("提示", "成功替换" + num + "处", "OK");
            }
        }

        [MenuItem("Tools/Others \t\t\t\t 其他工具/Font Quick Replace \t\t 字体快速替换", false, 120)]
        public static void OpenWindow()
        {
            GetWindow<ReplaceFont4uGUI>("字体替换工具");
        }

        private static int CorrectionPublicFont(Font replace, Font matching)
        {
            var replaceNum = 0;
            var selectObjs = Selection.GetFiltered(typeof(GameObject), SelectionMode.DeepAssets);

            foreach (var selectObj in selectObjs)
            {
                var obj = (GameObject)selectObj;

                if (obj == null || selectObj == null)
                {
                    Debug.LogWarning("ERROR:Obj Is Null !!!");
                    continue;
                }

                var path = AssetDatabase.GetAssetPath(selectObj);

                if (path.Length < 1 || path.EndsWith(".prefab") == false)
                {
                    Debug.LogWarning("ERROR:Folder=" + path);
                }
                else
                {
                    Debug.Log("Selected Folder=" + path);
                    var clone = Instantiate(obj);
                    var labels = clone.GetComponentsInChildren<Text>(true);

                    foreach (var label in labels)
                        if (label.font == matching)
                        {
                            label.font = replace;
                            replaceNum++;
                        }

                    SaveDealFinishPrefab(clone, path);
                    DestroyImmediate(clone);
                    Debug.Log("Connect Font Success=" + path);
                }
            }

            AssetDatabase.Refresh();
            return replaceNum;
        }

        private static void SaveDealFinishPrefab(GameObject go, string path)
        {
            if (File.Exists(path))
                //Object prefab = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject));
                PrefabUtility.SaveAsPrefabAssetAndConnect(go, path, InteractionMode.UserAction);
            else
                PrefabUtility.SaveAsPrefabAsset(go, path);
        }
    }
}
