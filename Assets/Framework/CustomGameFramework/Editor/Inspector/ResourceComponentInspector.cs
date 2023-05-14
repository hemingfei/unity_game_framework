//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using System.Collections.Generic;
using GameFramework;
using GameFramework.Resource;
using UnityEditor;
using UnityGameFramework.Runtime;
using CustomGameFramework.Runtime;
using UnityGameFramework.Editor;

namespace CustomGameFramework.Editor
{
    [CustomEditor(typeof(ResourceComponent))]
    internal sealed class ResourceComponentInspector : GameFrameworkInspector
    {
        private HelperInfo<ResourceHelperBase> m_ResourceHelperInfo = new HelperInfo<ResourceHelperBase>("Resource");
        private SerializedProperty m_resouceMode = null;
        
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

        private void OnEnable()
        {
            m_ResourceHelperInfo.Init(serializedObject);
            m_resouceMode = serializedObject.FindProperty("m_ResourceMode");
            RefreshTypeNames();
        }

        private void RefreshTypeNames()
        {
            m_ResourceHelperInfo.Refresh();
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}
