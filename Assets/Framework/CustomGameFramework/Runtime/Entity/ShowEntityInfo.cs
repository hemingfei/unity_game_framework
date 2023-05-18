//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using System;
using GameFramework;

namespace UnityGameFramework.Runtime
{
    internal sealed class ShowEntityInfo : IReference
    {
        public ShowEntityInfo()
        {
            EntityLogicType = null;
            UserData = null;
        }

        public Type EntityLogicType { get; private set; }

        public object UserData { get; private set; }

        public void Clear()
        {
            EntityLogicType = null;
            UserData = null;
        }

        public static ShowEntityInfo Create(Type entityLogicType, object userData)
        {
            var showEntityInfo = ReferencePool.Acquire<ShowEntityInfo>();
            showEntityInfo.EntityLogicType = entityLogicType;
            showEntityInfo.UserData = userData;
            return showEntityInfo;
        }
    }
}