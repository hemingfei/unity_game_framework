using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CustomGameFramework.Editor
{
    [CustomEditor(typeof(EasyBuildSetting), false)]
    public class EasyBuildSettingEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            GUILayout.Space(10);
            if (GUILayout.Button("保存设置"))
            {
                EasyBuildSettingData.SaveFile();
            }
        }
    }
}
