/****************************************************
*	文件：TimerComponentInspector.cs
*	作者：hemingfei
*	邮箱：hemingfei@outlook.com
*	日期：2023/05/11 13:09:15
*	功能：暂无
*****************************************************/

using System.Collections.Generic;
using GameFramework;
using UnityEditor;
using UnityGameFramework.Runtime;
using CustomGameFramework.Runtime;
using UnityGameFramework.Editor;

namespace CustomGameFramework.Editor
{
    [CustomEditor(typeof(TimerComponent))]
    internal sealed class TimerComponentInspector : GameFrameworkInspector
    {
        private HelperInfo<TimerHelperBase> m_TimerHelperInfo = new HelperInfo<TimerHelperBase>("Timer");
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            EditorGUI.BeginDisabledGroup(EditorApplication.isPlayingOrWillChangePlaymode);
            {
                m_TimerHelperInfo.Draw();
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
            m_TimerHelperInfo.Init(serializedObject);

            RefreshTypeNames();
        }

        private void RefreshTypeNames()
        {
            m_TimerHelperInfo.Refresh();
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}

