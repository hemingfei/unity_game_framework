//
//  UIWindowBaseObject.cs 
//
//  Author: HGT 
//
//  Copyright (c) 2021 hegametech.com 
//

using GameFramework;
using GameFramework.ObjectPool;
using UnityEngine;

namespace CustomGameFramework.Runtime
{
    public class UIWindowBaseObject : ObjectBase
    {
        public GameObject goTrans;

        public static UIWindowBaseObject Create(object target, GameObject trans)
        {
            var uiWindowBaseObject = ReferencePool.Acquire<UIWindowBaseObject>();
            uiWindowBaseObject.Initialize(target);
            uiWindowBaseObject.goTrans = trans;
            return uiWindowBaseObject;
        }

        protected override void Release(bool isShutdown)
        {
            var uiWindowBase = (UIWindowBase)Target;
            if (uiWindowBase == null) return;
            Object.Destroy(uiWindowBase.gameObject);
        }
    }
}