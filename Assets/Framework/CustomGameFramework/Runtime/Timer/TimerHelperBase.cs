/****************************************************
*	文件：TimerHelperBase.cs
*	作者：hemingfei
*	邮箱：hemingfei@outlook.com
*	日期：2023/05/11 12:22:46
*	功能：暂无
*****************************************************/

using System;
using UnityEngine;

namespace CustomGameFramework.Runtime
{
    public abstract class TimerHelperBase : MonoBehaviour, ITimerHelper
    {
        public abstract void Init();

        public abstract int Post(Action callback, float delay, bool isTimeScaled);

        public abstract int Post(Action<int> callback, float delay, bool isTimeScaled, int repeatCount);

        public abstract bool Cancel(int id);
    }
}