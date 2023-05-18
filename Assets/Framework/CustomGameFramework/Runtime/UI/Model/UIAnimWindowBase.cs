//
//  UIAnimWindowBase.cs 
//
//  Author: HGT 
//
//  Copyright (c) 2021 hegametech.com 
//

using Doozy.Engine.UI;
using UnityEngine;

namespace CustomGameFramework.Runtime
{
    /// <summary>
    ///     /*使用 UIAnimView 动画组件 则继承 UIAnimWindowBase, 不使用动画则继承 UIWindowBase*/
    /// </summary>
    public abstract class UIAnimWindowBase : UIWindowBase
    {
        private UIAnimView[] _childDoozyViews;
        private UIAnimView _doozyView;

        public override void OnUIInit()
        {
            base.OnUIInit();
            _doozyView = GetComponent<UIAnimView>();
            if (_doozyView != null)
            {
                var childDoozyViews = GetComponentsInChildren<UIAnimView>();
                if (childDoozyViews != null && childDoozyViews.Length > 1)
                {
                    _childDoozyViews = new UIAnimView[childDoozyViews.Length - 1];
                    for (var i = 0; i < _childDoozyViews.Length; i++)
                    {
                        _childDoozyViews[i] = childDoozyViews[i + 1];
                        _childDoozyViews[i].InstantHide();
                    }
                }

                _doozyView.InstantHide();
            }
        }

        public override void EnterAnim(UIAnimCallBack animComplete, UIEventCallBack callBack, params object[] objs)
        {
            if (_doozyView != null)
            {
                if (_childDoozyViews != null)
                    for (var i = 0; i < _childDoozyViews.Length; i++)
                    {
                        _childDoozyViews[i].Show();
                        Debug.LogError(_childDoozyViews[i].gameObject.name);
                    }

                _doozyView.Show(false, () => { animComplete(this, callBack, objs); });
            }
            else
            {
                animComplete(this, callBack, objs);
            }
        }

        public override void ExitAnim(UIAnimCallBack animComplete, UIEventCallBack callBack, params object[] objs)
        {
            if (_doozyView != null)
            {
                if (_childDoozyViews != null)
                    for (var i = 0; i < _childDoozyViews.Length; i++)
                        _childDoozyViews[i].Hide();
                _doozyView.Hide(false, () => { animComplete(this, callBack, objs); });
            }
            else
            {
                animComplete(this, callBack, objs);
            }
        }
    }
}