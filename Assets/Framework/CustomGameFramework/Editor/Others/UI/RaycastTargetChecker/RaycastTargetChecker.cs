/****************************************************
*	文件：RaycastTargetChecker.cs
*	作者：hemingfei
*	邮箱：hemingfei@outlook.com
*	日期：2023/05/22 09:16:55
*	功能：暂无
*****************************************************/

/****************************************************
*	文件：RaycastTargetChecker.cs
*	作者：hemingfei
*	邮箱：hemingfei@outlook.com
*	日期：2023/05/12 11:16:11
*	功能：暂无
*****************************************************/

//
//  RaycastTargetChecker.cs 
//
//  Author: He Mingfei <hemingfei@outlook.com> 
//
//  Copyright (c) 2021 hegametech.com 
//

using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Hegametech.Framework.Editor
{
    public class RaycastTargetChecker : EditorWindow
    {
        private static RaycastTargetChecker instance;
        private Color borderColor = Color.blue;
        private MaskableGraphic[] graphics;
        private bool hideUnchecked;
        private Vector2 scrollPosition = Vector2.zero;
        private bool showBorders = true;

        private void OnEnable()
        {
            instance = this;
        }

        private void OnDisable()
        {
            instance = null;
        }

        private void OnGUI()
        {
            using (var horizontalScope = new EditorGUILayout.HorizontalScope())
            {
                showBorders = EditorGUILayout.Toggle("Show Gizmos", showBorders, GUILayout.Width(200.0f));
                borderColor = EditorGUILayout.ColorField(borderColor);
            }

            hideUnchecked = EditorGUILayout.Toggle("Hide Unchecked", hideUnchecked);
            GUILayout.Space(12.0f);
            var rect = GUILayoutUtility.GetLastRect();
            GUI.color = new Color(0.0f, 0.0f, 0.0f, 0.25f);
            GUI.DrawTexture(new Rect(0.0f, rect.yMin + 6.0f, Screen.width, 4.0f), EditorGUIUtility.whiteTexture);
            GUI.DrawTexture(new Rect(0.0f, rect.yMin + 6.0f, Screen.width, 1.0f), EditorGUIUtility.whiteTexture);
            GUI.DrawTexture(new Rect(0.0f, rect.yMin + 9.0f, Screen.width, 1.0f), EditorGUIUtility.whiteTexture);
            GUI.color = Color.white;
            graphics = FindObjectsOfType<MaskableGraphic>();

            using (var scrollViewScope = new GUILayout.ScrollViewScope(scrollPosition))
            {
                scrollPosition = scrollViewScope.scrollPosition;

                for (var i = 0; i < graphics.Length; i++)
                {
                    var graphic = graphics[i];

                    if (hideUnchecked == false || graphic.raycastTarget) DrawElement(graphic);
                }
            }

            foreach (var item in graphics) EditorUtility.SetDirty(item);

            Repaint();
        }

        [MenuItem("Tools/Others \t\t\t\t 其他工具/RaycastTarget Checker \t UI的射线投射目标检查器", false, 110)]
        public static void Open()
        {
            instance = instance ?? GetWindow<RaycastTargetChecker>("RaycastTargets");
            instance.Show();
        }

        private void DrawElement(MaskableGraphic graphic)
        {
            using (var horizontalScope = new EditorGUILayout.HorizontalScope())
            {
                Undo.RecordObject(graphic, "Modify RaycastTarget");
                graphic.raycastTarget = EditorGUILayout.Toggle(graphic.raycastTarget, GUILayout.Width(20));
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.ObjectField(graphic, typeof(MaskableGraphic), true);
                EditorGUI.EndDisabledGroup();
            }
        }

        [DrawGizmo(GizmoType.Selected | GizmoType.NonSelected)]
        private static void DrawGizmos(MaskableGraphic source, GizmoType gizmoType)
        {
            if (instance != null && instance.showBorders && source.raycastTarget)
            {
                var corners = new Vector3[4];
                source.rectTransform.GetWorldCorners(corners);
                Gizmos.color = instance.borderColor;

                for (var i = 0; i < 4; i++) Gizmos.DrawLine(corners[i], corners[(i + 1) % 4]);

                if (Selection.activeGameObject == source.gameObject)
                {
                    Gizmos.DrawLine(corners[0], corners[2]);
                    Gizmos.DrawLine(corners[1], corners[3]);
                }
            }

            SceneView.RepaintAll();
        }
    }
}
