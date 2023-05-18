/****************************************************
*	文件：TimerComponent.Extension.cs
*	作者：何明飞
*	邮箱：hemingfei@outlook.com
*	日期：2022/12/06 14:25:14
*	功能：暂无
*****************************************************/

using System;
using System.Collections.Generic;

namespace CustomGameFramework.Runtime
{
    /// <summary>
    ///     计时器组件。
    /// </summary>
    public sealed partial class TimerComponent
    {
        private readonly Dictionary<int, Stack<int>> _groupStacks = new();

        public int Post(int groupId, Action callback, float delay, bool isTimeScaled = true)
        {
            if (!_groupStacks.ContainsKey(groupId)) _groupStacks.Add(groupId, new Stack<int>());
            var timerId = m_TimerHelper.Post(callback, delay, isTimeScaled);
            _groupStacks[groupId].Push(timerId);
            return timerId;
        }

        public int Post(int groupId, Action<int> callback, float delay, int repeatCount, bool isTimeScaled = true)
        {
            if (!_groupStacks.ContainsKey(groupId)) _groupStacks.Add(groupId, new Stack<int>());
            var timerId = m_TimerHelper.Post(callback, delay, isTimeScaled, repeatCount);
            _groupStacks[groupId].Push(timerId);
            return timerId;
        }

        public void CancelGroup(int groupId, bool isInvokeComplete = false)
        {
            if (!_groupStacks.ContainsKey(groupId)) return;

            var timerStack = _groupStacks[groupId];
            var stackLength = timerStack.Count;
            for (var i = 0; i < stackLength; i++)
            {
                var timerId = timerStack.Pop();
                m_TimerHelper.Cancel(timerId);
            }

            timerStack.Clear();
            timerStack = null;
            _groupStacks.Remove(groupId);
        }
    }
}