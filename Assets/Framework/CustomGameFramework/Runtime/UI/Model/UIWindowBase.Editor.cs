//
//  UIWindowBase.cs 
//
//  Author: HGT 
//
//  Copyright (c) 2021 hegametech.com 
//

using UnityEngine;
using System.Collections.Generic;

namespace CustomGameFramework.Runtime
{
    public abstract partial class UIWindowBase
    {
#if UNITY_EDITOR
        public IAutoBindRuleHelper RuleHelper
        {
            get;
            set;
        }

        [ContextMenu("UIWindow 快速提取字段 private TYPE m_xxx;")]
        public string WindowEditorFastGetMembers()
        {
            ClearObject();
            int len = m_objectList.Count;
            string final = string.Empty;
            List<string> m_TempFiledNames = new List<string>();
            List<string> m_TempComponentTypeNames = new List<string>();
            m_TempFiledNames.Clear();
            m_TempComponentTypeNames.Clear();
            
            foreach (var obje in m_objectList)
            {
                if (!RuleHelper.IsValidBind(obje.transform, m_TempFiledNames, m_TempComponentTypeNames))
                {
                    m_TempFiledNames.Add("Go_" + obje.name);
                    m_TempComponentTypeNames.Add("GameObject");
                }
            }
            
            for(int i = 0; i < m_TempFiledNames.Count; i++)
            {
                string finalOutput = "private " + m_TempComponentTypeNames[i] + " m_" + m_TempFiledNames[i] + ";";
                
                if (i != m_TempFiledNames.Count - 1)
                {
                    finalOutput += "\r\n\t\t";
                }
                
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
            int len = m_objectList.Count;
            string final = string.Empty;
            List<string> m_TempFiledNames = new List<string>();
            List<string> m_TempComponentTypeNames = new List<string>();
            List<string> m_TempRawName = new List<string>();
            m_TempFiledNames.Clear();
            m_TempComponentTypeNames.Clear();
            m_TempRawName.Clear();
            
            foreach (var obje in m_objectList)
            {
                if (!RuleHelper.IsValidBind(obje.transform, m_TempFiledNames, m_TempComponentTypeNames, m_TempRawName))
                {
                    m_TempFiledNames.Add("Go_" + obje.name);
                    m_TempComponentTypeNames.Add("GameObject");
                    m_TempRawName.Add(obje.name);
                }
            }
            
            for (int i = 0; i < m_TempFiledNames.Count; i++)
            {
                string memberName = "m_" + m_TempFiledNames[i];
                string getComponentName = string.Empty;
                getComponentName = "Get" + m_TempComponentTypeNames[i] + "(\"" + m_TempRawName[i] + "\")";
                string finalOutput = memberName + " = " + getComponentName + ";";
                
                if(i != m_TempFiledNames.Count - 1)
                {
                    finalOutput += "\r\n\t\t\t";
                }
                
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
            int len = m_objectList.Count;
            string final = string.Empty;
            List<string> m_TempFiledNames = new List<string>();
            List<string> m_TempComponentTypeNames = new List<string>();
            m_TempFiledNames.Clear();
            m_TempComponentTypeNames.Clear();

            foreach (var obje in m_objectList)
            {
                if (!RuleHelper.IsValidBind(obje.transform, m_TempFiledNames, m_TempComponentTypeNames))
                {
                    m_TempFiledNames.Add("Go_" + obje.name);
                    m_TempComponentTypeNames.Add("GameObject");
                }
            }

            for (int i = 0; i < m_TempFiledNames.Count; i++)
            {
                if(m_TempFiledNames[i].StartsWith("Btn"))
                {
                    string mBtn = "m_" + m_TempFiledNames[i];
                    string mFunc = "On_" + m_TempFiledNames[i] + "_Clicked";
                    string finalOutput = $"{mBtn}.onClick.AddListener({mFunc});" + "\r\n\t\t";
                    final += finalOutput;
                }
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
            int len = m_objectList.Count;
            string final = string.Empty;
            List<string> m_TempFiledNames = new List<string>();
            List<string> m_TempComponentTypeNames = new List<string>();
            m_TempFiledNames.Clear();
            m_TempComponentTypeNames.Clear();

            foreach (var obje in m_objectList)
            {
                if (!RuleHelper.IsValidBind(obje.transform, m_TempFiledNames, m_TempComponentTypeNames))
                {
                    m_TempFiledNames.Add("Go_" + obje.name);
                    m_TempComponentTypeNames.Add("GameObject");
                }
            }

            for (int i = 0; i < m_TempFiledNames.Count; i++)
            {
                if(m_TempFiledNames[i].StartsWith("Btn"))
                {
                    string mBtn = "m_" + m_TempFiledNames[i];
                    string mFunc = "On_" + m_TempFiledNames[i] + "_Clicked";
                    string finalOutput = $"{mBtn}.onClick.RemoveListener({mFunc});" + "\r\n\t\t";
                    final += finalOutput;
                }
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
            int len = m_objectList.Count;
            string final = string.Empty;
            List<string> m_TempFiledNames = new List<string>();
            List<string> m_TempComponentTypeNames = new List<string>();
            m_TempFiledNames.Clear();
            m_TempComponentTypeNames.Clear();

            foreach (var obje in m_objectList)
            {
                if (!RuleHelper.IsValidBind(obje.transform, m_TempFiledNames, m_TempComponentTypeNames))
                {
                    m_TempFiledNames.Add("Go_" + obje.name);
                    m_TempComponentTypeNames.Add("GameObject");
                }
            }

            for (int i = 0; i < m_TempFiledNames.Count; i++)
            {
                if (m_TempFiledNames[i].StartsWith("Btn"))
                {
                    string mBtn = "m_" + m_TempFiledNames[i];
                    string mFunc = "On_" + m_TempFiledNames[i] + "_Clicked";
                    string finalOutput = "\t\t/// <summary>\n"
                + $"\t\t/// {m_TempFiledNames[i]} 点击事件\n"
                + "\t\t/// </summary>\n" + $"\t\tprivate void {mFunc}()" + "\n\t\t{" + "\n\t\t}" + "\n";

                    final += finalOutput;
                }
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
