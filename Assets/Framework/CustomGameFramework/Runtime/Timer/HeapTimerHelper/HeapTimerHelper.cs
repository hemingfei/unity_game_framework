//
//  HeapTimerHelper.cs 
//
//  Author: He Mingfei <hemingfei@outlook.com> 
//
//  Copyright (c) 2021 hegametech.com 
//

using System;
using UnityEngine;

namespace CustomGameFramework.Runtime
{
    internal sealed partial class HeapTimerHelper : TimerHelperBase
    {
        private const int CHECK_FREQUENCY = 16; //精确到16ms timer的最小精度
        private readonly BinaryHeap<TimeItem> m_scaleTimeHeap = new(128, eBinaryHeapSortMode.kMin);
        private readonly BinaryHeap<TimeItem> m_unScaleTimeHeap = new(128, eBinaryHeapSortMode.kMin);


        private TimeItem cachedItem;
        private float m_currentScaleTime = -1;
        private float m_currentUnScaleTime = -1;
        private float m_dwLastCheckTick; // 最后一次检查的时间

        private void Update()
        {
            var now = Time.realtimeSinceStartup * 1000;
            if (now - m_dwLastCheckTick < CHECK_FREQUENCY) return;
            m_dwLastCheckTick = now;

            // 不受缩放影响定时器更新
            if (m_unScaleTimeHeap.Top() != null)
            {
                m_currentUnScaleTime = Time.unscaledTime;

                #region 不受缩放影响定时器更新

                while ((cachedItem = m_unScaleTimeHeap.Top()) != null)
                {
                    if (!cachedItem.isEnable)
                    {
                        m_unScaleTimeHeap.Pop();
                        cachedItem.Recycle2Cache();
                        continue;
                    }

                    if (cachedItem.sortScore < m_currentUnScaleTime)
                    {
                        m_unScaleTimeHeap.Pop();
                        cachedItem.OnTimeTick();

                        if (cachedItem.isEnable && cachedItem.NeedRepeat())
                            Post2Real(cachedItem);
                        else
                            cachedItem.Recycle2Cache();
                    }
                    else
                    {
                        break;
                    }
                }

                #endregion
            }

            // 受缩放影响定时器更新
            if (m_scaleTimeHeap.Top() != null)
            {
                m_currentScaleTime = Time.time;

                #region 受缩放影响定时器更新

                while ((cachedItem = m_scaleTimeHeap.Top()) != null)
                {
                    if (!cachedItem.isEnable)
                    {
                        m_scaleTimeHeap.Pop();
                        cachedItem.Recycle2Cache();
                        continue;
                    }

                    if (cachedItem.sortScore < m_currentScaleTime)
                    {
                        m_scaleTimeHeap.Pop();
                        cachedItem.OnTimeTick();

                        if (cachedItem.isEnable && cachedItem.NeedRepeat())
                            Post2Scale(cachedItem);
                        else
                            cachedItem.Recycle2Cache();
                    }
                    else
                    {
                        break;
                    }
                }

                #endregion
            }
        }

        /// <summary>
        ///     初始化
        /// </summary>
        public override void Init()
        {
            m_unScaleTimeHeap.Clear();
            m_scaleTimeHeap.Clear();
            m_currentUnScaleTime = Time.unscaledTime;
            m_currentScaleTime = Time.time;
        }


        public void ResetMgr()
        {
            m_unScaleTimeHeap.Clear();
            m_scaleTimeHeap.Clear();
        }

        public void StartMgr()
        {
            m_currentUnScaleTime = Time.unscaledTime;
            m_currentScaleTime = Time.time;
        }

        #region 投递受缩放影响定时器

        private void Post2Scale(TimeItem item)
        {
            m_currentScaleTime = Time.time;
            item.sortScore = m_currentScaleTime + item.DelayTime();
            m_scaleTimeHeap.Insert(item);
        }

        #endregion

        #region 投递真实时间定时器

        private void Post2Real(TimeItem item)
        {
            m_currentUnScaleTime = Time.unscaledTime;
            item.sortScore = m_currentUnScaleTime + item.DelayTime();
            m_unScaleTimeHeap.Insert(item);
        }

        #endregion

        /// <summary>
        ///     取消特定id的计时器
        /// </summary>
        /// <param name="id">计时器ID</param>
        /// <returns>是否成功取消</returns>
        public override bool Cancel(int id)
        {
            var item = TimeItem.GetTimeItemByID(id);

            if (item == null) return false;

            item.Cancel();
            return true;
        }

        internal void Shutdown()
        {
            ResetMgr();
        }

        #region 投递定时器

        /// <summary>
        ///     投递定时器
        /// </summary>
        /// <param name="callback">回调函数</param>
        /// <param name="delay">延迟几秒</param>
        /// <param name="isScaled">是否受时间缩放影响</param>
        /// <returns>计时器ID</returns>
        public override int Post(Action callback, float delay, bool isTimeScaled = true)
        {
            var item = TimeItem.Spawn(a => callback?.Invoke(), delay);
            if (isTimeScaled)
                Post2Scale(item);
            else
                Post2Real(item);
            return item.Id;
        }

        /// <summary>
        ///     投递定时器
        /// </summary>
        /// <param name="callback">回调函数</param>
        /// <param name="delay">延迟几秒</param>
        /// <param name="isTimeScaled">是否受时间缩放影响</param>
        /// <param name="repeatCount">重复几次</param>
        /// <returns>计时器ID</returns>
        public override int Post(Action<int> callback, float delay, bool isTimeScaled, int repeatCount)
        {
            var item = TimeItem.Spawn(callback, delay, repeatCount);
            if (isTimeScaled)
                Post2Scale(item);
            else
                Post2Real(item);
            return item.Id;
        }

        #endregion
    }
}