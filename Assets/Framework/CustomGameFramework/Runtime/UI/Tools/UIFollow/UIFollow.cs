//
//  UIFollow.cs 
//
//  Author: He Mingfei <hemingfei@outlook.com> 
//
//  Copyright (c) 2021 hegametech.com 
//

using UnityEngine;
using GameFramework;

namespace CustomGameFramework.Runtime
{
    public class UIFollow : MonoBehaviour
    {
        #region 字段

        [SerializeField] private Transform _targetUI;
        [SerializeField] private Transform _followTransform;
        [SerializeField] private Vector3 _followPosition;
        [SerializeField] private Vector3 _positionOffset;
        [SerializeField] private Vector3 _uiOffset;

        private bool _isRefreshedOutsideFrame = false;
        private bool _isDirty = false;
        private Vector3 _viewPos;
        private UIFollowLogic.BindMode _bindMode;
        private UIFollowLogic _uiFollowLogic;
        private Camera _sceneCamera;
        private Camera _uiCamera;

        #endregion

        #region GET SET 方法

        public Transform TargetUI
        {
            get { return _targetUI; }
            set
            {
                _targetUI = value;
                UpdateBinding();
            }
        }

        public Transform FollowTransform
        {
            get { return _followTransform; }
            set
            {
                _followTransform = value;
                _followPosition = Vector3.zero;
                _bindMode = UIFollowLogic.BindMode.Transform;
                UpdateBinding();
            }
        }

        public Vector3 FollowPosition
        {
            get { return _followPosition; }
            set
            {
                _followPosition = value;
                _followTransform = null;
                _bindMode = UIFollowLogic.BindMode.Position;
                UpdateBinding();
            }
        }

        public Vector3 PositionOffset
        {
            get { return _positionOffset; }
            set
            {
                _positionOffset = value;
                UpdateBinding();
            }
        }
        
        public Vector3 UIOffset
        {
            get { return _uiOffset; }
            set
            {
                _uiOffset = value;
                UpdateBinding();
            }
        }

        public Camera SceneCamera
        {
            get { return _sceneCamera; }
            set
            {
                _sceneCamera = value;
                if (_uiFollowLogic != null)
                {
                    _uiFollowLogic.SceneCamera = _sceneCamera;
                }
            }
        }

        public Camera UICamera
        {
            get { return _uiCamera; }
            set
            {
                _uiCamera = value;
                if (_uiFollowLogic != null)
                {
                    _uiFollowLogic.UICamera = _uiCamera;
                }
            }
        }

        #endregion

        #region Mono 方法

        private void OnValidate()
        {
            UpdateBinding();
        }

        private void LateUpdate()
        {
            if (_uiFollowLogic == null)
            {
                return;
            }

            if (IsNeedUpdate() || _isDirty)
            {
                _isDirty = false;
                _uiFollowLogic.Update();
            }

            if (_isRefreshedOutsideFrame)
            {
                TargetUI.position = Vector3.zero;
            }
        }

        private void OnDestroy()
        {
            _isDirty = false;
            _followPosition = Vector3.zero;
            _positionOffset = Vector3.zero;
            _uiOffset = Vector3.zero;
            _viewPos = Vector3.zero;
            _bindMode = UIFollowLogic.BindMode.None;
            _followTransform = null;
            _targetUI = null;
            _sceneCamera = null;
            _uiCamera = null;
            _uiFollowLogic = null;
        }

        #endregion

        #region public 方法

        public void SetDirty()
        {
            _isDirty = true;
            _uiFollowLogic?.SetDirty();
        }

        /// <summary>
        /// 绑定固定坐标
        /// </summary>
        public void BindPosition(Transform targetUI, Vector3 followPosition, Camera uiCamera, Camera sceneCamera, Vector3 uiOffset, Vector3 positionOffset)
        {
            if (_uiFollowLogic != null)
            {
                ReferencePool.Release(_uiFollowLogic);
            }
            _uiFollowLogic = ReferencePool.Acquire<UIFollowLogic>();
            SetDirty();
            TargetUI = targetUI;
            FollowPosition = followPosition;
            UICamera = uiCamera;
            SceneCamera = sceneCamera;
            UIOffset = uiOffset;
            PositionOffset = positionOffset;
            _isRefreshedOutsideFrame = false;
        }
        
        /// <summary>
        /// 绑定物体
        /// </summary>
        public void BindTransform(Transform targetUI, Transform followTransform, Camera uiCamera, Camera sceneCamera, Vector3 uiOffset, Vector3 positionOffset)
        {
            if (_uiFollowLogic == null)
            {
                _uiFollowLogic = ReferencePool.Acquire<UIFollowLogic>();
            }
            SetDirty();
            TargetUI = targetUI;
            FollowTransform = followTransform;
            UICamera = uiCamera;
            SceneCamera = sceneCamera;
            UIOffset = uiOffset;
            PositionOffset = positionOffset;
            _isRefreshedOutsideFrame = false;
        }

        #endregion

        #region private 方法

        private void UpdateBinding()
        {
            _isDirty = true;
            if (_targetUI == null)
            {
                return;
            }

            if (SceneCamera == null)
            {
                return;
            }

            if (_uiFollowLogic == null)
            {
                _uiFollowLogic = ReferencePool.Acquire<UIFollowLogic>();
            }

            if (_uiFollowLogic.SceneCamera == null || _uiFollowLogic.UICamera == null)
            {
                _uiFollowLogic.SceneCamera = SceneCamera;
                _uiFollowLogic.UICamera = UICamera;
            }

            switch (_bindMode)
            {
                case UIFollowLogic.BindMode.Transform when _followTransform == null:
                    return;
                case UIFollowLogic.BindMode.Transform:
                    _uiFollowLogic.Set(_targetUI, _followTransform, _uiOffset, _positionOffset);
                    break;
                case UIFollowLogic.BindMode.Position:
                    _uiFollowLogic.Set(_targetUI, _followPosition, _uiOffset, _positionOffset);
                    break;
                default:
                    return;
            }
        }

        private bool IsNeedUpdate()
        {
            if (SceneCamera == null)
            {
                return false;
            }

            switch (_bindMode)
            {
                case UIFollowLogic.BindMode.Transform when _followTransform == null:
                    return false;
                case UIFollowLogic.BindMode.Transform:
                    return IsWorldPositionInView(_followTransform.position);
                case UIFollowLogic.BindMode.Position:
                    return IsWorldPositionInView(_followPosition);
                default:
                    return false;
            }
        }

        private bool IsWorldPositionInView(Vector3 pos)
        {
            if (!SceneCamera.gameObject.activeInHierarchy)
            {
                if (TargetUI.position == Vector3.zero)
                {
                    return false;
                }

                TargetUI.position = Vector3.zero;
                return true;
            }

            _viewPos = SceneCamera.WorldToViewportPoint(pos);

            // 当Z坐标（前后）在场景相机之外的时候
            if (_viewPos.z < 0 || _viewPos.z > _sceneCamera.farClipPlane)
            {
                if (!_isRefreshedOutsideFrame)
                {
                    _isRefreshedOutsideFrame = true;
                    return true;
                }
                return false;
            }
            // 当在1.5个屏幕之内的时候 刷新
            if (_viewPos.x < -0.5f || _viewPos.x > 1.5f || _viewPos.y < -0.5f || _viewPos.y > 1.5f)
            {
                if (!_isRefreshedOutsideFrame)
                {
                    _isRefreshedOutsideFrame = true;
                    return true;
                }
                return false;
            }

            _isRefreshedOutsideFrame = false;
            return true;
        }

        #endregion
    }
}
