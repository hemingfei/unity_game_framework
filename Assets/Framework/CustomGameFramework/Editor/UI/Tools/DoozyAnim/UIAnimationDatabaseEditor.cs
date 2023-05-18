using Doozy.Engine.UI.Animation;
using UnityEditor;
using UnityEngine;

namespace Doozy.Editor.UI
{
    [CustomEditor(typeof(UIAnimationDatabase))]
    public class UIAnimationDatabaseEditor : UnityEditor.Editor
    {
        private UIAnimationDatabase m_target;

        private void OnEnable()
        {
            m_target = target as UIAnimationDatabase;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (m_target.DatabaseName == "Custom")
            {
                GUILayout.Space(50);
                GUILayout.Label("删除动画");
                GUILayout.BeginVertical();
                foreach (var name in m_target.AnimationNames)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(name);
                    //GUILayout.Space(20);
                    if (GUILayout.Button("delete"))
                    {
                        m_target.Delete(name, true);
                        break;
                    }

                    GUILayout.EndHorizontal();
                }

                GUILayout.EndVertical();
            }
        }
    }
}