//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using CustomGameFramework.Runtime;
using UnityEditor;
using UnityGameFramework.Editor;

namespace CustomGameFramework.Editor
{
    [CustomEditor(typeof(ResourceComponent))]
    internal sealed class ResourceComponentInspector : GameFrameworkInspector
    {
        private SerializedProperty m_resouceMode;
        private readonly HelperInfo<ResourceHelperBase> m_ResourceHelperInfo = new("Resource");

        private void OnEnable()
        {
            m_ResourceHelperInfo.Init(serializedObject);
            m_resouceMode = serializedObject.FindProperty("m_ResourceMode");
            RefreshTypeNames();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            EditorGUI.BeginDisabledGroup(EditorApplication.isPlayingOrWillChangePlaymode);
            {
                m_ResourceHelperInfo.Draw();
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("资源系统运行模式");
                EditorGUILayout.PropertyField(m_resouceMode);
            }
            EditorGUI.EndDisabledGroup();

            serializedObject.ApplyModifiedProperties();

            Repaint();
        }

        protected override void OnCompileComplete()
        {
            base.OnCompileComplete();

            RefreshTypeNames();
        }

        private void RefreshTypeNames()
        {
            m_ResourceHelperInfo.Refresh();

            serializedObject.ApplyModifiedProperties();
        }
    }
}