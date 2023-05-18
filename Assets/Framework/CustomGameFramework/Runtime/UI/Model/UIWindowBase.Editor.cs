//
//  UIWindowBase.cs 
//
//  Author: HGT 
//
//  Copyright (c) 2021 hegametech.com 
//

using System.Collections.Generic;
using UnityEngine;

namespace CustomGameFramework.Runtime
{
    public abstract partial class UIWindowBase
    {
#if UNITY_EDITOR
        public IAutoBindRuleHelper RuleHelper { get; set; }

        [ContextMenu("UIWindow 快速提取字段 private TYPE m_xxx;")]
        public string WindowEditorFastGetMembers()
        {
            ClearObject();
            var len = m_objectList.Count;
            var final = string.Empty;
            var m_TempFiledNames = new List<string>();
            var m_TempComponentTypeNames = new List<string>();
            m_TempFiledNames.Clear();
            m_TempComponentTypeNames.Clear();

            foreach (var obje in m_objectList)
                if (!RuleHelper.IsValidBind(obje.transform, m_TempFiledNames, m_TempComponentTypeNames))
                {
                    m_TempFiledNames.Add("Go_" + obje.name);
                    m_TempComponentTypeNames.Add("GameObject");
                }

            for (var i = 0; i < m_TempFiledNames.Count; i++)
            {
                var finalOutput = "private " + m_TempComponentTypeNames[i] + " m_" + m_TempFiledNames[i] + ";";

                if (i != m_TempFiledNames.Count - 1) finalOutput += "\r\n\t\t";

                final += finalOutput;
            }

            var textEditor = new TextEditor();
            textEditor.text = final;
            textEditor.SelectAll();
            textEditor.Copy();
            return final;
        }


        [ContextMenu("UIWindow 快速提取组件 GetComponents")]
        public virtual string WindowEditorFastGetComponents()
        {
            ClearObject();
            var len = m_objectList.Count;
            var final = string.Empty;
            var m_TempFiledNames = new List<string>();
            var m_TempComponentTypeNames = new List<string>();
            var m_TempRawName = new List<string>();
            m_TempFiledNames.Clear();
            m_TempComponentTypeNames.Clear();
            m_TempRawName.Clear();

            foreach (var obje in m_objectList)
                if (!RuleHelper.IsValidBind(obje.transform, m_TempFiledNames, m_TempComponentTypeNames, m_TempRawName))
                {
                    m_TempFiledNames.Add("Go_" + obje.name);
                    m_TempComponentTypeNames.Add("GameObject");
                    m_TempRawName.Add(obje.name);
                }

            for (var i = 0; i < m_TempFiledNames.Count; i++)
            {
                var memberName = "m_" + m_TempFiledNames[i];
                var getComponentName = string.Empty;
                getComponentName = "Get" + m_TempComponentTypeNames[i] + "(\"" + m_TempRawName[i] + "\")";
                var finalOutput = memberName + " = " + getComponentName + ";";

                if (i != m_TempFiledNames.Count - 1) finalOutput += "\r\n\t\t\t";

                final += finalOutput;
            }

            var textEditor = new TextEditor();
            textEditor.text = final;
            textEditor.SelectAll();
            textEditor.Copy();
            return final;
        }


        [ContextMenu("UIWindow 快速提取字段 按钮 Btn 的添加注册代码")]
        public string WindowEditorFastGetBtnScripts()
        {
            ClearObject();
            var len = m_objectList.Count;
            var final = string.Empty;
            var m_TempFiledNames = new List<string>();
            var m_TempComponentTypeNames = new List<string>();
            m_TempFiledNames.Clear();
            m_TempComponentTypeNames.Clear();

            foreach (var obje in m_objectList)
                if (!RuleHelper.IsValidBind(obje.transform, m_TempFiledNames, m_TempComponentTypeNames))
                {
                    m_TempFiledNames.Add("Go_" + obje.name);
                    m_TempComponentTypeNames.Add("GameObject");
                }

            for (var i = 0; i < m_TempFiledNames.Count; i++)
                if (m_TempFiledNames[i].StartsWith("Btn"))
                {
                    var mBtn = "m_" + m_TempFiledNames[i];
                    var mFunc = "On_" + m_TempFiledNames[i] + "_Clicked";
                    var finalOutput = $"{mBtn}.onClick.AddListener({mFunc});" + "\r\n\t\t";
                    final += finalOutput;
                }

            var textEditor = new TextEditor();
            textEditor.text = final;
            textEditor.SelectAll();
            textEditor.Copy();
            return final;
        }

        [ContextMenu("UIWindow 快速提取字段 按钮 Btn 的取消注册代码")]
        public string WindowEditorFastGetBtnUnSubscribeScripts()
        {
            ClearObject();
            var len = m_objectList.Count;
            var final = string.Empty;
            var m_TempFiledNames = new List<string>();
            var m_TempComponentTypeNames = new List<string>();
            m_TempFiledNames.Clear();
            m_TempComponentTypeNames.Clear();

            foreach (var obje in m_objectList)
                if (!RuleHelper.IsValidBind(obje.transform, m_TempFiledNames, m_TempComponentTypeNames))
                {
                    m_TempFiledNames.Add("Go_" + obje.name);
                    m_TempComponentTypeNames.Add("GameObject");
                }

            for (var i = 0; i < m_TempFiledNames.Count; i++)
                if (m_TempFiledNames[i].StartsWith("Btn"))
                {
                    var mBtn = "m_" + m_TempFiledNames[i];
                    var mFunc = "On_" + m_TempFiledNames[i] + "_Clicked";
                    var finalOutput = $"{mBtn}.onClick.RemoveListener({mFunc});" + "\r\n\t\t";
                    final += finalOutput;
                }

            var textEditor = new TextEditor();
            textEditor.text = final;
            textEditor.SelectAll();
            textEditor.Copy();
            return final;
        }

        [ContextMenu("UIWindow 快速提取字段 按钮 Btn 的注册函数")]
        public string WindowEditorFastGetBtnFunctionScripts()
        {
            ClearObject();
            var len = m_objectList.Count;
            var final = string.Empty;
            var m_TempFiledNames = new List<string>();
            var m_TempComponentTypeNames = new List<string>();
            m_TempFiledNames.Clear();
            m_TempComponentTypeNames.Clear();

            foreach (var obje in m_objectList)
                if (!RuleHelper.IsValidBind(obje.transform, m_TempFiledNames, m_TempComponentTypeNames))
                {
                    m_TempFiledNames.Add("Go_" + obje.name);
                    m_TempComponentTypeNames.Add("GameObject");
                }

            for (var i = 0; i < m_TempFiledNames.Count; i++)
                if (m_TempFiledNames[i].StartsWith("Btn"))
                {
                    var mBtn = "m_" + m_TempFiledNames[i];
                    var mFunc = "On_" + m_TempFiledNames[i] + "_Clicked";
                    var finalOutput = "\t\t/// <summary>\n"
                                      + $"\t\t/// {m_TempFiledNames[i]} 点击事件\n"
                                      + "\t\t/// </summary>\n" + $"\t\tprivate void {mFunc}()" + "\n\t\t{" + "\n\t\t}" +
                                      "\n";

                    final += finalOutput;
                }

            var textEditor = new TextEditor();
            textEditor.text = final;
            textEditor.SelectAll();
            textEditor.Copy();
            return final;
        }
#endif
    }
}