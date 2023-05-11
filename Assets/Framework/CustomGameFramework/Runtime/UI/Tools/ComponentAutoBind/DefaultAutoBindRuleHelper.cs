//
//  DefaultAutoBindRuleHelper.cs 
//
//  Author: HGT 
//
//  Copyright (c) 2021 hegametech.com 
//

#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;

namespace CustomGameFramework.Runtime
{
    /// <summary>
    /// 默认自动绑定规则辅助器
    /// </summary>
    public class DefaultAutoBindRuleHelper : IAutoBindRuleHelper
    {
    
        /// <summary>
        /// 命名前缀与类型的映射
        /// </summary>
        private Dictionary<string, string> m_PrefixesDict = new Dictionary<string, string>()
        {
            {"Go", "GameObject" },
            {"auto", "GameObject" },
            {"Trans", "Transform" },
            {"Animation", "Animation"},
            {"Animator", "Animator"},
            {"RectTrans", "RectTransform"},
            {"Canvas", "Canvas"},
            {"CGroup", "CanvasGroup"},
            {"VGroup", "VerticalLayoutGroup"},
            {"HGroup", "HorizontalLayoutGroup"},
            {"GGroup", "GridLayoutGroup"},
            {"TGroup", "ToggleGroup"},
            {"Btn", "Button"},
            {"Img", "Image"},
            {"RawImg", "RawImage"},
            {"Txt", "Text"},
            {"Input", "InputField"},
            {"Slider", "Slider"},
            {"Mask", "Mask"},
            {"Mask2D", "RectMask2D"},
            {"Tog", "Toggle"},
            {"Sbar", "Scrollbar"},
            {"SRect", "ScrollRect"},
            {"Drop", "Dropdown"},
        };
        
        public bool IsValidBind(Transform target, List<string> filedNames, List<string> componentTypeNames, List<string> rawNames = null)
        {
            string[] strArray = target.name.Split('_');
            
            if (strArray.Length == 1)
            {
                return false;
            }
            
            string filedName = strArray[strArray.Length - 1];
            
            for (int i = 0; i < strArray.Length - 1; i++)
            {
                string str = strArray[i];
                string comName;
                
                if (m_PrefixesDict.TryGetValue(str, out comName))
                {
                    filedNames.Add($"{str}_{filedName}");
                    componentTypeNames.Add(comName);
                    
                    if(rawNames != null)
                    {
                        rawNames.Add(target.name);
                    }
                }
                else
                {
                    Debug.LogWarning($"{target.name}的命名中{str}不存在对应的组件类型，绑定跳过");
                    
                    if (componentTypeNames.Count > 0)
                    {
                        int startIndex = i;
                        string finalFiledName = string.Empty;
                        
                        for(int j = i; j < strArray.Length; j++)
                        {
                            finalFiledName += strArray[j];
                        }
                        
                        for(int h = 0; h < filedNames.Count; h++)
                        {
                            filedNames[h] = filedNames[h].Replace(filedName, finalFiledName);
                        }
                        
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            
            return true;
        }
    }
}

#endif