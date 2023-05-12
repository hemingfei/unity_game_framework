/****************************************************
*	文件：ProgressReporter.cs
*	作者：hemingfei
*	邮箱：hemingfei@outlook.com
*	日期：2023/05/12 16:32:46
*	功能：暂无
*****************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GameFramework;
using UnityEngine;

namespace CustomGameFramework.Runtime
{
    public class ProgressReporter : IProgress<float>, IReference
    {
        public float Progress;

        public Action<float> OnReport;
        public ProgressReporter()
        {
            Progress = 0;
            OnReport = null;
        }

        public void Report(float value)
        {
            Progress = value;
            OnReport?.Invoke(value);
        }

        public static ProgressReporter Create(Action<float> onReport)
        {
            var reporter = ReferencePool.Acquire<ProgressReporter>();
            reporter.Progress = 0;
            reporter.OnReport = onReport;
            return reporter;
        }

        public void Clear()
        {
            Progress = 0;
            OnReport = null;
        }
    }
}

