//
//  HeapTimerHelper.TimeItem.cs 
//
//  Author: He Mingfei <hemingfei@outlook.com> 
//
//  Copyright (c) 2021 hegametech.com 
//

using System;
using System.Collections.Generic;
using GameFramework;

namespace CustomGameFramework.Runtime
{
    internal sealed partial class HeapTimerHelper
    {
        public class TimeItem : IReference, IBinaryHeapElement
        {
            /*
             * tick:当前第几次
             *
             */
            private static int s_NextID;
            private static readonly Dictionary<int, TimeItem> s_TimeItemMap = new();
            private int m_CallbackTick;
            private float m_DelayTime;
            private int m_RepeatCount;

            public int Id { get; private set; } = -1;

            public Action<int> callback { get; private set; }

            public bool isEnable { get; private set; } = true;

            public bool CacheFlag { get; set; }

            public float sortScore { get; set; }

            public int heapIndex { get; set; }

            public void RebuildHeap<T>(BinaryHeap<T> heap) where T : IBinaryHeapElement
            {
                heap.RebuildAtIndex(heapIndex);
            }

            public void Clear()
            {
                m_CallbackTick = 0;
                callback = null;
                isEnable = true;
                heapIndex = 0;
                UnRegisterActiveTimeItem(this);
                Id = -1;
            }

            public static TimeItem Spawn(Action<int> callback, float delayTime, int repeatCount = 1)
            {
                var item = ReferencePool.Acquire<TimeItem>();
                item.Set(callback, delayTime, repeatCount);
                return item;
            }

            public void Set(Action<int> callback, float delayTime, int repeatCount)
            {
                m_CallbackTick = 0;
                this.callback = callback;
                m_DelayTime = delayTime;
                m_RepeatCount = repeatCount;
                RegisterActiveTimeItem(this);
            }

            public void OnTimeTick()
            {
                if (callback != null) callback(++m_CallbackTick);

                if (m_RepeatCount > 0) --m_RepeatCount;
            }

            public void Cancel(bool isInvokeComplete = false)
            {
                if (isEnable)
                {
                    isEnable = false;
                    if (isInvokeComplete) OnTimeTick();
                    callback = null;
                }
            }

            public static TimeItem GetTimeItemByID(int id)
            {
                TimeItem unit;

                if (s_TimeItemMap.TryGetValue(id, out unit)) return unit;

                return null;
            }

            public bool NeedRepeat()
            {
                if (m_RepeatCount == 0) return false;

                return true;
            }

            public float DelayTime()
            {
                return m_DelayTime;
            }


            private static void RegisterActiveTimeItem(TimeItem unit)
            {
                unit.Id = ++s_NextID;
                s_TimeItemMap.Add(unit.Id, unit);
            }

            private static void UnRegisterActiveTimeItem(TimeItem unit)
            {
                if (s_TimeItemMap.ContainsKey(unit.Id)) s_TimeItemMap.Remove(unit.Id);

                unit.Id = -1;
            }

            public void Recycle2Cache()
            {
                //超出缓存最大值
                ReferencePool.Release(this);
            }
        }
    }
}