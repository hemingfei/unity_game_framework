//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using GameFramework;
using UnityEngine;

namespace UnityGameFramework.Runtime
{
    internal sealed class AttachEntityInfo : IReference
    {
        public AttachEntityInfo()
        {
            ParentTransform = null;
            UserData = null;
        }

        public Transform ParentTransform { get; private set; }

        public object UserData { get; private set; }

        public void Clear()
        {
            ParentTransform = null;
            UserData = null;
        }

        public static AttachEntityInfo Create(Transform parentTransform, object userData)
        {
            var attachEntityInfo = ReferencePool.Acquire<AttachEntityInfo>();
            attachEntityInfo.ParentTransform = parentTransform;
            attachEntityInfo.UserData = userData;
            return attachEntityInfo;
        }
    }
}