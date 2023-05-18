//
//  UIWindowBaseComponmentEditor.cs 
//
//  Author: HGT 
//
//  Copyright (c) 2021 hegametech.com 
//

using System;
using System.Collections.Generic;
using System.Reflection;
using CustomGameFramework.Runtime;
using Doozy.Engine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace CustomGameFramework.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(UIWindowBase), true)]
    public class UIWindowBaseComponmentEditor : UnityEditor.Editor
    {
        private Canvas canvas;
        private string[] list;
        private bool m_hasUIAnimView;
        private string m_HelperTypeName;
        private int m_HelperTypeNameIndex;

        private string[] m_HelperTypeNames;

        private bool m_isLock = true;

        private List<GameObject> m_objectNeedScaleAccording2Screen = new();
        private UIWindowBase m_ui;
        private int selectIndex;

        public void OnEnable()
        {
            m_ui = (UIWindowBase)target;
            m_HelperTypeNames = GetTypeNames(typeof(IAutoBindRuleHelper),
                UIEditorConstant.S_AutoBindRuleHelperAssemblyNames);
            CheckHasUIAnimViewComponent();
        }

        public override void OnInspectorGUI()
        {
            if (m_ui == null) m_ui = (UIWindowBase)target;

            if (canvas == null) canvas = m_ui.gameObject.GetComponent<Canvas>();

            if (m_hasUIAnimView == false) DrawUIAnimViewButton();

            list = UIManager.GetCameraNames();
            selectIndex = GetIndex(m_ui.cameraKey);
            EditorGUILayout.Space();
            EditorUtil.DrawUILine(Color.grey);
            EditorGUILayout.LabelField("UI相机", EditorStyles.whiteLargeLabel);
            selectIndex = EditorGUILayout.Popup("Camera Key", selectIndex, list);

            if (list.Length != 0) m_ui.cameraKey = list[selectIndex];

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("组件绑定", EditorStyles.whiteLargeLabel);
            DrawHelperSelect();
            DrawTopButton();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("组件自适应改变 Scale", EditorStyles.whiteLargeLabel);
            DrawAutoFixPart();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("锁定下方", EditorStyles.whiteLargeLabel);
            m_isLock = GUILayout.Toggle(m_isLock, "isLock");
            EditorGUILayout.Space();
            EditorUtil.DrawUILine(Color.grey);

            if (m_isLock)
            {
                GUI.enabled = false;
                DrawDefaultInspector();
                GUI.enabled = true;
            }
            else
            {
                base.OnInspectorGUI();
            }

            if (!Application.isPlaying && GUI.changed)
            {
                EditorUtility.SetDirty(target);
                EditorSceneManager.MarkAllScenesDirty();
                canvas.overrideSorting = true;
                canvas.sortingLayerName = m_ui.m_UIType.ToString();
            }
        }

        public int GetIndex(string current)
        {
            if (string.IsNullOrEmpty(current)) return 0;

            for (var i = 0; i < list.Length; i++)
                if (current.Equals(list[i]))
                    return i;

            return 0;
        }

        private void DrawUIAnimViewButton()
        {
            if (GUILayout.Button("添加UI动画组件"))
            {
                m_ui.gameObject.AddComponent<UIAnimView>();
                CheckHasUIAnimViewComponent();
            }
        }

        private void CheckHasUIAnimViewComponent()
        {
            m_hasUIAnimView = m_ui.gameObject.GetComponent<UIAnimView>() != null;
        }

        /// <summary>
        ///     绘制辅助器选择框
        /// </summary>
        private void DrawHelperSelect()
        {
            m_HelperTypeName = m_HelperTypeNames[0];

            if (m_ui.RuleHelper != null)
            {
                m_HelperTypeName = m_ui.RuleHelper.GetType().Name;

                for (var i = 0; i < m_HelperTypeNames.Length; i++)
                    if (m_HelperTypeName == m_HelperTypeNames[i])
                        m_HelperTypeNameIndex = i;
            }
            else
            {
                var helper = (IAutoBindRuleHelper)CreateHelperInstance(m_HelperTypeName,
                    UIEditorConstant.S_AutoBindRuleHelperAssemblyNames);
                m_ui.RuleHelper = helper;
            }

            foreach (var go in Selection.gameObjects)
            {
                var autoBindTool = go.GetComponent<UIWindowBase>();

                if (autoBindTool != null && autoBindTool.RuleHelper == null)
                {
                    var helper = (IAutoBindRuleHelper)CreateHelperInstance(m_HelperTypeName,
                        UIEditorConstant.S_AutoBindRuleHelperAssemblyNames);
                    autoBindTool.RuleHelper = helper;
                }
            }

            var showNames = m_HelperTypeNames;

            for (var i = 0; i < showNames.Length; i++)
            {
                var s = showNames[i].Split('.');

                if (s.Length > 1) showNames[i] = s[s.Length - 1];
            }

            var selectedIndex = EditorGUILayout.Popup("Rule Helper Class", m_HelperTypeNameIndex, showNames);

            if (selectedIndex != m_HelperTypeNameIndex)
            {
                m_HelperTypeNameIndex = selectedIndex;
                m_HelperTypeName = m_HelperTypeNames[selectedIndex];
                var helper = (IAutoBindRuleHelper)CreateHelperInstance(m_HelperTypeName,
                    UIEditorConstant.S_AutoBindRuleHelperAssemblyNames);
                m_ui.RuleHelper = helper;
            }
        }

        /// <summary>
        ///     创建辅助器实例
        /// </summary>
        private object CreateHelperInstance(string helperTypeName, string[] assemblyNames)
        {
            foreach (var assemblyName in assemblyNames)
            {
                var assembly = Assembly.Load(assemblyName);
                var instance = assembly.CreateInstance(helperTypeName);

                if (instance != null) return instance;
            }

            return null;
        }

        /// <summary>
        ///     获取指定基类在指定程序集中的所有子类名称
        /// </summary>
        private string[] GetTypeNames(Type typeBase, string[] assemblyNames)
        {
            var typeNames = new List<string>();

            foreach (var assemblyName in assemblyNames)
            {
                Assembly assembly = null;

                try
                {
                    assembly = Assembly.Load(assemblyName);
                }
                catch
                {
                    continue;
                }

                if (assembly == null) continue;

                var types = assembly.GetTypes();

                foreach (var type in types)
                    if (type.IsClass && !type.IsAbstract && typeBase.IsAssignableFrom(type))
                        typeNames.Add(type.FullName);
            }

            typeNames.Sort();
            return typeNames.ToArray();
        }

        /// <summary>
        ///     绘制顶部按钮
        /// </summary>
        private void DrawTopButton()
        {
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("全部删除")) ClearObjectList();

            if (GUILayout.Button("自动绑定组件"))
            {
                AutoBindComponent();
                Sort();
            }

            if (GUILayout.Button("排序&去重")) Sort();

            if (GUILayout.Button("生成绑定代码")) GenerateComponentCode();

            EditorGUILayout.EndHorizontal();

            var serializedObject = new SerializedObject(m_ui);
            var property = serializedObject.FindProperty("m_objectList");
            serializedObject.Update();
            EditorGUILayout.PropertyField(property, true);
            serializedObject.ApplyModifiedProperties();
        }

        private void ClearObjectList()
        {
            m_ui.m_objectList.Clear();
        }

        private void Sort()
        {
            m_ui.ClearObject();
        }

        public virtual void GenerateComponentCode()
        {
            UICreateService.CreateObservedScript();
        }

        /// <summary>
        ///     自动绑定组件
        /// </summary>
        private void AutoBindComponent()
        {
            m_ui.m_objectList.Clear();
            var m_TempFiledNames = new List<string>();
            var m_TempComponentTypeNames = new List<string>();
            var childs = m_ui.gameObject.GetComponentsInChildren<Transform>(true);

            foreach (var child in childs)
            {
                m_TempFiledNames.Clear();
                m_TempComponentTypeNames.Clear();

                if (m_ui.RuleHelper.IsValidBind(child, m_TempFiledNames, m_TempComponentTypeNames))
                    m_ui.m_objectList.Add(child.gameObject);
            }
        }

        private void DrawAutoFixPart()
        {
            if (GUILayout.Button("自动获取带有 auto_ 的字段")) AutoBindAutoScaleComponent();

            var serializedObject = new SerializedObject(m_ui);
            var property = serializedObject.FindProperty("m_objectNeedAutoScaleWithScreen");
            serializedObject.Update();
            EditorGUILayout.PropertyField(property, true);
            serializedObject.ApplyModifiedProperties();
        }

        private void AutoBindAutoScaleComponent()
        {
            m_ui.m_objectNeedAutoScaleWithScreen.Clear();
            var m_TempFiledNames = new List<string>();
            var m_TempComponentTypeNames = new List<string>();
            var childs = m_ui.gameObject.GetComponentsInChildren<Transform>(true);

            foreach (var child in childs)
                if (child.name.Contains("auto_"))
                    m_ui.m_objectNeedAutoScaleWithScreen.Add(child.gameObject);
        }
    }
}