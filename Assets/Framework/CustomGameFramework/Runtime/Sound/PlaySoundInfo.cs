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
    internal sealed class PlaySoundInfo : IReference
    {
        public PlaySoundInfo()
        {
            BindingEntity = null;
            WorldPosition = Vector3.zero;
            UserData = null;
        }

        public Entity BindingEntity { get; private set; }

        public Vector3 WorldPosition { get; private set; }

        public object UserData { get; private set; }

        public void Clear()
        {
            BindingEntity = null;
            WorldPosition = Vector3.zero;
            UserData = null;
        }

        public static PlaySoundInfo Create(Entity bindingEntity, Vector3 worldPosition, object userData)
        {
            var playSoundInfo = ReferencePool.Acquire<PlaySoundInfo>();
            playSoundInfo.BindingEntity = bindingEntity;
            playSoundInfo.WorldPosition = worldPosition;
            playSoundInfo.UserData = userData;
            return playSoundInfo;
        }
    }
}