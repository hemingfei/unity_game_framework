//
//  UIFollowLogic.cs 
//
//  Author: He Mingfei <hemingfei@outlook.com> 
//
//  Copyright (c) 2021 hegametech.com 
//

using GameFramework;
using UnityEngine;

namespace CustomGameFramework.Runtime
{
    public class UIFollowLogic : IReference
    {
        /// <summary>
        ///     绑定模式
        /// </summary>
        public enum BindMode
        {
            None,
            Position,
            Transform
        }

        /// <summary>
        ///     场景相机
        /// </summary>
        public Camera SceneCamera { get; set; }

        /// <summary>
        ///     UI相机
        /// </summary>
        public Camera UICamera { get; set; }

        /// <summary>
        ///     位置偏移量
        /// </summary>
        public Vector3 UIOffset
        {
            get => _uiOffset;
            set
            {
                _uiOffset = value;
                _isDirty = true;
            }
        }

        /// <summary>
        ///     位置偏移量
        /// </summary>
        public Vector3 PositionOffset
        {
            get => _positionOffset;
            set
            {
                _positionOffset = value;
                _isDirty = true;
            }
        }

        public void Clear()
        {
            _bindMode = BindMode.None;
            _targetUI = null;
            _targetTransform = null;
            _targetPosition = Vector3.zero;
            _uiOffset = Vector3.zero;
            _positionOffset = Vector3.zero;
            _oldPos = Vector3.zero;
            _isDirty = false;
            SceneCamera = null;
            UICamera = null;
        }

        public void SetDirty()
        {
            _isDirty = true;
        }

        public void Set(Transform ui, Transform followTransform, Vector3 uiOffset, Vector3 positionOffset)
        {
            _bindMode = BindMode.None;
            if (ui == null || followTransform == null) return;

            _targetUI = ui;
            _targetTransform = followTransform;
            _uiOffset = uiOffset;
            _positionOffset = positionOffset;
            _isDirty = true;
            _bindMode = BindMode.Transform;
        }

        public void Set(Transform ui, Vector3 followPosition, Vector3 uiOffset, Vector3 positionOffset)
        {
            _bindMode = BindMode.None;
            if (ui == null) return;

            _targetUI = ui;
            _targetPosition = followPosition;
            _uiOffset = uiOffset;
            _positionOffset = positionOffset;
            _isDirty = true;
            _bindMode = BindMode.Position;
        }

        public void Update()
        {
            if (SceneCamera == null || UICamera == null) return;

            if (!SceneCamera.gameObject.activeInHierarchy)
            {
                if (_targetUI.position == Vector3.zero) return;

                _targetUI.position = Vector3.zero;
                return;
            }

            if (_bindMode == BindMode.None) return;

            var newPos = _targetPosition;

            if (_bindMode == BindMode.Transform && _targetTransform != null) newPos = _targetTransform.position;

            newPos += _positionOffset;
            ScenePosition2UIPosition(SceneCamera, UICamera, newPos, _targetUI);
            _targetUI.localPosition += _uiOffset;
        }

        private void ScenePosition2UIPosition(Camera sceneCamera, Camera uiCamera, Vector3 posInScene,
            Transform uiTarget)
        {
            var viewportPos = sceneCamera.WorldToViewportPoint(posInScene);
            var worldPos = uiCamera.ViewportToWorldPoint(viewportPos);

            if (_isDirty || _oldPos != worldPos)
            {
                _isDirty = false;
                _oldPos = worldPos;
                uiTarget.position = worldPos;
                var localPos = uiTarget.localPosition;
                localPos.z = 0f;
                uiTarget.localPosition = localPos;
            }
        }

        #region 字段

        private BindMode _bindMode = BindMode.None;
        private Transform _targetUI;
        private Transform _targetTransform;
        private Vector3 _targetPosition;
        private Vector3 _uiOffset;
        private Vector3 _positionOffset;
        private Vector3 _oldPos;
        private bool _isDirty;

        #endregion
    }
}