//
//  ITimerHelper.cs 
//
//  Author: He Mingfei <hemingfei@outlook.com> 
//
//  Copyright (c) 2021 hegametech.com 
//

using System;

namespace CustomGameFramework.Runtime
{
    /// <summary>
    ///     计时器管理器接口。
    /// </summary>
    public interface ITimerHelper
    {
        /// <summary>
        ///     初始化
        /// </summary>
        void Init();

        /// <summary>
        ///     投递定时器
        /// </summary>
        /// <param name="callback">回调函数</param>
        /// <param name="delay">延迟几秒</param>
        /// <param name="isTimeScaled">是否受时间缩放影响</param>
        /// <returns>计时器ID</returns>
        int Post(Action callback, float delay, bool isTimeScaled);

        /// <summary>
        ///     投递定时器
        /// </summary>
        /// <param name="callback">回调函数</param>
        /// <param name="delay">延迟几秒</param>
        /// <param name="isTimeScaled">是否受时间缩放影响</param>
        /// <param name="repeatCount">重复几次</param>
        /// <returns>计时器ID</returns>
        int Post(Action<int> callback, float delay, bool isTimeScaled, int repeatCount);

        /// <summary>
        ///     取消特定id的计时器
        /// </summary>
        /// <param name="id">计时器ID</param>
        /// <returns>是否成功取消</returns>
        bool Cancel(int id);
    }
}