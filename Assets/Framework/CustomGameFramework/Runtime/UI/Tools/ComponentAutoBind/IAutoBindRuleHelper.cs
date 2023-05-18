//
//  IAutoBindRuleHelper.cs 
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
    ///     自动绑定规则辅助器接口
    /// </summary>
    public interface IAutoBindRuleHelper
    {
        /// <summary>
        ///     是否为有效绑定
        /// </summary>
        bool IsValidBind(Transform target, List<string> filedNames, List<string> componentTypeNames,
            List<string> rawNames = null);
    }
}

#endif