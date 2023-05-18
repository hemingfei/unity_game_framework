/****************************************************
*	文件：HeapTimerHelper.cs
*	作者：hemingfei
*	邮箱：hemingfei@outlook.com
*	日期：2023/05/11 12:32:27
*	功能：暂无
*****************************************************/

using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace CustomGameFramework.Runtime
{
    public class UniTaskTimerHelper : TimerHelperBase
    {
        private int _timerId;
        private Dictionary<int, CancellationTokenSource> _tokens;

        public override void Init()
        {
            _timerId = 0;
            _tokens = new Dictionary<int, CancellationTokenSource>();
        }

        public override int Post(Action callback, float delay, bool isTimeScaled)
        {
            return Post(a => callback?.Invoke(), delay, isTimeScaled, 0);
        }

        public override int Post(Action<int> callback, float delay, bool isTimeScaled, int repeatCount)
        {
            var timerId = _timerId++;
            var token = new CancellationTokenSource();
            _tokens.Add(timerId, token);
            StartTimerTask(timerId, token, callback, delay, isTimeScaled, repeatCount);
            return timerId;
        }

        private async void StartTimerTask(int timerId, CancellationTokenSource token, Action<int> callback, float delay,
            bool isTimeScaled, int repeatCount)
        {
            for (var i = 0; i < repeatCount + 1; i++)
            {
                var count = i + 1;

                var isCanceled = await UniTask
                    .Delay(TimeSpan.FromSeconds(delay), !isTimeScaled, cancellationToken: token.Token)
                    .SuppressCancellationThrow();
                if (isCanceled) break;
                callback?.Invoke(count);
            }

            if (_tokens.ContainsKey(timerId))
            {
                var cancelToken = _tokens[timerId];
                cancelToken?.Dispose();
                cancelToken = null;
                _tokens.Remove(timerId);
            }
        }

        public override bool Cancel(int timerId)
        {
            if (_tokens.TryGetValue(timerId, out var token))
                if (token != null && !token.IsCancellationRequested)
                {
                    token.Cancel();
                    return true;
                }

            return false;
        }
    }
}