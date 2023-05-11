/****************************************************
*	文件：UpdaterComponent.cs
*	作者：何明飞
*	邮箱：hemingfei@outlook.com
*	日期：2023/02/06 14:29:39
*	功能：暂无
*****************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using CustomGameFramework.Runtime;
using UnityEngine;
using UnityGameFramework.Runtime;
using Random = UnityEngine.Random;

namespace CustomGameFramework
{
    /// <summary>
    /// Description of UpdaterComponent
    /// </summary>
    public class UpdaterComponent : GameFrameworkComponent
    {
        private EventHandler<UpdateEventArgs> _updateEventHandler;
        private readonly UpdateEventArgs _updateArgs = new UpdateEventArgs(0, 0);

        private bool _noUpdate = true;
        private readonly List<Action> _updateQueue = new List<Action>();
        private readonly List<Action> _updateRunQueue = new List<Action>();

        public void ExecuteUpdateAction(Action action)
        {
            if (action == null)
            {
                return;
            }

            lock (_updateQueue)
            {
                _updateQueue.Add(action);
                _noUpdate = false;
            }
        }

        /// <summary>
        /// Update事件。
        /// </summary>
        public event EventHandler<UpdateEventArgs> UpdateHandler
        {
            add => _updateEventHandler += value;
            remove => _updateEventHandler -= value;
        }

        private void Update()
        {
            if (_updateEventHandler != null)
            {
                _updateArgs.DeltaTime = Time.deltaTime;
                _updateArgs.UnscaledDeltaTime = Time.unscaledDeltaTime;
                _updateEventHandler(this, _updateArgs);
            }

            if (!_noUpdate)
            {
                lock (_updateQueue)
                {
                    if (_noUpdate) return;
                    _updateRunQueue.AddRange(_updateQueue);
                    _updateQueue.Clear();
                    _noUpdate = true;
                    for (var i = 0; i < _updateRunQueue.Count; i++)
                    {
                        var action = _updateRunQueue[i];
                        if (action == null) continue;
                        action();
                    }

                    _updateRunQueue.Clear();
                }
            }
        }
    }
}