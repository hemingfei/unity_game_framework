//
//  TimerComponent.cs 
//
//  Author: HGT 
//
//  Copyright (c) 2021 hegametech.com 
//

using System;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace CustomGameFramework.Runtime
{
    /// <summary>
    /// 计时器组件。
    /// </summary>
    public sealed partial class TimerComponent : GameFrameworkComponent
    {
        [SerializeField]
        private string m_TimerHelperTypeName = "CustomGameFramework.Runtime.TimerHelperBase";

        [SerializeField]
        private TimerHelperBase m_CustomTimerHelper = null;

        private ITimerHelper m_TimerHelper = null;
        
        /// <summary>
        /// 游戏框架组件初始化。
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            TimerHelperBase timerHelper = Helper.CreateHelper(m_TimerHelperTypeName, m_CustomTimerHelper);
            if (timerHelper == null)
            {
                Log.Error("Can not create timer helper.");
                return;
            }

            timerHelper.name = "Timer Helper";
            Transform transform = timerHelper.transform;
            transform.SetParent(this.transform);
            transform.localScale = Vector3.one;

            m_TimerHelper = timerHelper;
        }

        private void Start()
        {
            Init();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            m_TimerHelper.Init();
        }

        /// <summary>
        /// 投递定时器
        /// </summary>
        /// <param name="callback">回调函数</param>
        /// <param name="delay">延迟几秒</param>
        /// <param name="isTimeScaled">是否受时间缩放影响</param>
        /// <returns>计时器ID</returns>
        public int Post(Action callback, float delay, bool isTimeScaled = true)
        {
            return m_TimerHelper.Post(callback, delay, isTimeScaled);
        }

        /// <summary>
        /// 投递定时器
        /// </summary>
        /// <param name="callback">回调函数</param>
        /// <param name="delay">延迟几秒</param>
        /// <param name="repeatCount">重复几次</param>
        /// <param name="isTimeScaled">是否受时间缩放影响</param>
        /// <returns>计时器ID</returns>
        public int Post(Action<int> callback, float delay, int repeatCount, bool isTimeScaled = true)
        {
            return m_TimerHelper.Post(callback, delay, isTimeScaled, repeatCount);
        }

        /// <summary>
        /// 取消特定id的计时器
        /// </summary>
        /// <param name="id">计时器ID</param>
        /// <returns>是否成功取消</returns>
        public bool Cancel(int id)
        {
            return m_TimerHelper.Cancel(id);
        }
    }
}
