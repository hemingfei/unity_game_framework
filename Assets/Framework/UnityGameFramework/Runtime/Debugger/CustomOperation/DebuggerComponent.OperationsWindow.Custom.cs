/****************************************************
*	文件：DebuggerComponent.OperationsWindow.Custom.cs
*	作者：何明飞
*	邮箱：hemingfei@outlook.com
*	日期：2023/01/05 19:26:38
*	功能：暂无
*****************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace UnityGameFramework.Runtime
{
    public sealed partial class DebuggerComponent : GameFrameworkComponent
    {
        /// <summary>
        /// 被注册方法的Object
        /// </summary>
        public object MethodsRegisterObject { get; private set; }

        /// <summary>
        /// 注册调用方法
        /// </summary>
        /// <param name="ob"></param>
        public void RegistgerMethodsObject(object ob)
        {
            MethodsRegisterObject = ob;
            UnregisterDebuggerWindow("Other/Operations");
            RegisterDebuggerWindow("Other/Operations", m_OperationsWindow, this);
        }

        [SerializeField]
        [Range(1, 8)]
        [Tooltip("自定义调试页中每行最多显示几个按钮")]
        private int m_BtnNumPerLineInOperation = 2;

        private sealed partial class OperationsWindow : ScrollableDebuggerWindowBase
        {
            private DebuggerComponent _debuggerComponent;
            private int m_numberOfButtonsPerLine = 3;
            private int m_debugButtonMethodNum = 0;
            private Dictionary<int, string> m_debugButtonMethodDict = new Dictionary<int, string>();
            private Dictionary<int, string> m_debugButtonDescritionDict = new Dictionary<int, string>();

            public override void Initialize(params object[] args)
            {
                _debuggerComponent = ((DebuggerComponent)args[0]);
                m_numberOfButtonsPerLine = _debuggerComponent.m_BtnNumPerLineInOperation;
                if (_debuggerComponent.MethodsRegisterObject == null)
                {
                    return;
                }
                Type type = _debuggerComponent.MethodsRegisterObject.GetType();
                Dictionary<int, int> sortPriorityDict = new Dictionary<int, int>();
                foreach (MethodInfo method in type.GetMethods())
                {
                    foreach (Attribute attr in method.GetCustomAttributes(true))
                    {
                        if (attr is DebuggerOperationButtonAttribute)
                        {
                            m_debugButtonMethodDict.Add(m_debugButtonMethodNum, method.Name);
                            m_debugButtonDescritionDict.Add(m_debugButtonMethodNum,
                                ((DebuggerOperationButtonAttribute)attr).Description);
                            sortPriorityDict.Add(m_debugButtonMethodNum,
                                ((DebuggerOperationButtonAttribute)attr).Priority);
                            m_debugButtonMethodNum++;
                        }
                    }
                }

                // Priority

                // ... Use LINQ to specify sorting by value.
                var items = from pair in sortPriorityDict
                    orderby pair.Value ascending
                    select pair;

                Dictionary<int, string> m_newMethodDict = new Dictionary<int, string>();
                Dictionary<int, string> m_newDescritionDict = new Dictionary<int, string>();
                int index = 0;
                // re value
                foreach (KeyValuePair<int, int> pair in items)
                {
                    m_newMethodDict.Add(index, m_debugButtonMethodDict[pair.Key]);
                    m_newDescritionDict.Add(index, m_debugButtonDescritionDict[pair.Key]);
                    index++;
                }

                m_debugButtonMethodDict = m_newMethodDict;
                m_debugButtonDescritionDict = m_newDescritionDict;

                // just in cast the SerializeField parameter set wrong in awake
                if (m_numberOfButtonsPerLine <= 0)
                {
                    m_numberOfButtonsPerLine = 1;
                }
            }

            /// <summary>
            /// invode custom methods
            /// </summary>
            /// <param name="index"></param>
            private void DealWithButton(int index)
            {
                MethodInfo method = _debuggerComponent.MethodsRegisterObject.GetType()
                    .GetMethod(m_debugButtonMethodDict[index]);
                if (method.GetParameters().Length > 0)
                {
                    UnityEngine.Debug.LogWarning(method.Name +
                                                 " Method has unwanted parameters, should not be registered as button, RETURN.");
                    return;
                }

                method.Invoke(_debuggerComponent.MethodsRegisterObject, null);
            }

            private void OnDrawOperationButtonScrollableWindow()
            {
                GUILayout.Label("<b>Custom Operations</b>");
                {
                    int lines;
                    if (m_debugButtonMethodNum % m_numberOfButtonsPerLine > 0)
                    {
                        lines = m_debugButtonMethodNum / m_numberOfButtonsPerLine + 1;
                    }
                    else
                    {
                        lines = m_debugButtonMethodNum / m_numberOfButtonsPerLine;
                    }

                    for (int i = 0; i < lines; i++)
                    {
                        GUILayout.BeginVertical("box");
                        {
                            GUILayout.BeginHorizontal();
                            {
                                for (int j = 0; j < m_numberOfButtonsPerLine; j++)
                                {
                                    if ((m_numberOfButtonsPerLine * i + j) < m_debugButtonMethodNum)
                                    {
                                        if (GUILayout.Button(
                                                m_debugButtonDescritionDict[m_numberOfButtonsPerLine * i + j],
                                                GUILayout.Height(30f)))
                                        {
                                            DealWithButton(m_numberOfButtonsPerLine * i + j);
                                        }

                                        GUILayout.Label("", GUILayout.Width(10f));
                                    }
                                }
                            }
                            GUILayout.EndHorizontal();
                        }
                        GUILayout.EndVertical();
                    }
                }
            }
        }
    }
}