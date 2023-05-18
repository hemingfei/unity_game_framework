//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

namespace UnityGameFramework.Runtime
{
    /// <summary>
    ///     默认声音辅助器。
    /// </summary>
    public class DefaultSoundHelper : SoundHelperBase
    {
        private FrameworkResourceComponent m_FrameworkResourceComponent;

        private void Start()
        {
            m_FrameworkResourceComponent = GameEntry.GetComponent<FrameworkResourceComponent>();
            if (m_FrameworkResourceComponent == null)
            {
                Log.Fatal("Resource component is invalid.");
            }
        }

        /// <summary>
        ///     释放声音资源。
        /// </summary>
        /// <param name="soundAsset">要释放的声音资源。</param>
        public override void ReleaseSoundAsset(object soundAsset)
        {
            m_FrameworkResourceComponent.UnloadAsset(soundAsset);
        }
    }
}