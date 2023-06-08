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
    [CustomEditor(typeof(SpriteAtlasUserConfig), false)]
    public class SpriteAtlasUserConfigEditor : UnityEditor.Editor
    {
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
            GUILayout.Label("\n1. 将图片文件夹拖入上方" +
                            "\n2. 工具会自动遍历文件夹里包含的子文件夹" +
                            "\n3. 每一个[最终的子文件夹]会被打成一个图集" +
                            "\n\n注意： 若一个文件夹既包含子文件夹又包含图片，\n            那么只有子文件夹会被打图集，\n            该文件夹和里面的图片不会被打图集");
        }
    }
}


