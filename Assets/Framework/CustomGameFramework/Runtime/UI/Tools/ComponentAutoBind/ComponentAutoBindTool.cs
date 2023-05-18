//
//  ComponentAutoBindTool.cs 
//
//  Author: HGT 
//
//  Copyright (c) 2021 hegametech.com 
//

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace CustomGameFramework.Runtime
{
    /// <summary>
    ///     组件自动绑定工具
    /// </summary>
    public class ComponentAutoBindTool : MonoBehaviour
    {
        [SerializeField] public List<Component> m_BindComs = new();

#if UNITY_EDITOR
        private void Awake()
        {
            if (m_ClassName == null)
            {
                m_ClassName = null;
                Log.Warning("m_ClassName is null");
            }

            if (m_Namespace == null)
            {
                m_Namespace = null;
                Log.Warning("m_Namespace is null");
            }

            if (m_CodePath == null)
            {
                m_CodePath = null;
                Log.Warning("m_CodePath is null");
            }
        }
#endif

        public T GetBindComponent<T>(int index) where T : Component
        {
            if (index >= m_BindComs.Count)
            {
                Log.Error("索引无效");
                return null;
            }

            var bindCom = m_BindComs[index] as T;

            if (bindCom == null)
            {
                Log.Error("类型无效");
                return null;
            }

            return bindCom;
        }
#if UNITY_EDITOR
        [Serializable]
        public class BindData
        {
            public string Name;
            public Component BindCom;

            public BindData()
            {
            }

            public BindData(string name, Component bindCom)
            {
                Name = name;
                BindCom = bindCom;
            }
        }

        public List<BindData> BindDatas = new();

        [SerializeField] private string m_ClassName;

        [SerializeField] private string m_Namespace;

        [SerializeField] private string m_CodePath;

        public string ClassName => m_ClassName;

        public string Namespace => m_Namespace;

        public string CodePath => m_CodePath;

        public IAutoBindRuleHelper RuleHelper { get; set; }
#endif
    }
}