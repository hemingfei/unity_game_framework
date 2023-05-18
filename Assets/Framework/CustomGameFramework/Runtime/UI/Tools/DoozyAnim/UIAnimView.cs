using System;
using System.Collections;
using Doozy.Engine.UI.Animation;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Doozy.Engine.UI
{
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(GraphicRaycaster))]
    [RequireComponent(typeof(CanvasGroup))]
    [DisallowMultipleComponent]
    public class UIAnimView : MonoBehaviour
    {
        private void Awake()
        {
            m_initialized = false;

            Canvas.enabled = false; //disable the canvas
            GraphicRaycaster.enabled = false; //disable the graphic raycaster

            UIAnimationOnShow = new UIAnimation(AnimationType.Show, MoveOnShow, RotateOnShow, ScaleOnShow, FadeOnShow);
            UIAnimationOnHide = new UIAnimation(AnimationType.Hide, MoveOnHide, RotateOnHide, ScaleOnHide, FadeOnHide);
            Initialize();
        }

        private void Start()
        {
            // UIAnimationOnShow = new UIAnimation(AnimationType.Show, MoveOnShow, RotateOnShow, ScaleOnShow, FadeOnShow);
            // UIAnimationOnHide = new UIAnimation(AnimationType.Hide, MoveOnHide, RotateOnHide, ScaleOnHide, FadeOnHide);
            // Initialize();
        }

        private void OnDisable()
        {
            StopHide();
            StopShow();
            UIAnimator.StopAnimations(RectTransform, AnimationType.Hide);
            UIAnimator.StopAnimations(RectTransform, AnimationType.Show);
            UIAnimator.StopAnimations(RectTransform, AnimationType.Loop);
            ResetToStartValues();
        }

        private void StopHide()
        {
            if (m_hideCoroutine == null) return;
            StopCoroutine(m_hideCoroutine);
            m_hideCoroutine = null;
            Visibility = VisibilityState.NotVisible;
            UIAnimator.StopAnimations(RectTransform, AnimationType.Hide);
        }

        private void StopShow()
        {
            if (m_showCoroutine == null) return;
            StopCoroutine(m_showCoroutine);
            m_showCoroutine = null;
            Visibility = VisibilityState.Visible;
            UIAnimator.StopAnimations(RectTransform, AnimationType.Show);
        }

        private void Initialize()
        {
            switch (StartBehavior)
            {
                case UIViewStartBehavior.DoNothing:
                    Canvas.enabled = true;
                    GraphicRaycaster.enabled = true;
                    m_initialized = true;
                    break;
                case UIViewStartBehavior.Hide:
                    InstantHide();
                    break;
                case UIViewStartBehavior.PlayShowAnimation:
                    InstantHide();
                    Show();
                    break;
            }
        }

        /// <summary>
        ///     直接显示UI
        /// </summary>
        public void InstantShow()
        {
            StopShow();
            StopHide();
            ResetToStartValues();
            Canvas.enabled = true; //enable the canvas
            GraphicRaycaster.enabled = true; //enable the graphic raycaster
            gameObject.SetActive(true); //set the active state to true (in case it has been disabled when hidden)
            Visibility = VisibilityState.Visible;
        }

        /// <summary>
        ///     直接隐藏UI
        /// </summary>
        public void InstantHide()
        {
            StopShow();
            StopHide();
            ResetToStartValues();
            Canvas.enabled = false;
            GraphicRaycaster.enabled = false;
            gameObject.SetActive(false);
            Visibility = VisibilityState.NotVisible;
            if (!m_initialized) m_initialized = true;
        }

        /// <summary>
        ///     显示UI
        /// </summary>
        /// <param name="instantAction">是否直接完成</param>
        /// <param name="onShowComplete">显示完成事件</param>
        public void Show(bool instantAction = false, Action onShowComplete = null)
        {
            gameObject.SetActive(true);
            StopHide();
            if (!UIAnimationOnShow.Enabled && !instantAction)
            {
                Debug.LogWarning("you are trying to Show UIView,but you did not enable any Show animations");
                return;
            }

            if (Visibility == VisibilityState.Showing)
                return;
            m_ActionOnShowComplete = onShowComplete;
            m_showCoroutine = StartCoroutine(ShowEnumerator(instantAction));
        }

        private IEnumerator ShowEnumerator(bool instantAction)
        {
            yield return new WaitForSeconds(0.1f);
            UIAnimator.StopAnimations(RectTransform, AnimationType.Show);
            Canvas.enabled = true;
            GraphicRaycaster.enabled = true;

            //MOVE
            var moveFrom = UIAnimator.GetAnimationMoveFrom(RectTransform, UIAnimationOnShow, CurrentStartPosition);
            var moveTo = UIAnimator.GetAnimationMoveTo(RectTransform, UIAnimationOnShow, CurrentStartPosition);
            if (!UIAnimationOnShow.Move.Enabled || instantAction) ResetPosition();
            UIAnimator.Move(RectTransform, UIAnimationOnShow, moveFrom, moveTo,
                instantAction); //initialize and play the SHOW Move tween

            //ROTATE
            var rotateFrom = UIAnimator.GetAnimationRotateFrom(UIAnimationOnShow, StartRotation);
            var rotateTo = UIAnimator.GetAnimationRotateTo(UIAnimationOnShow, StartRotation);
            if (!UIAnimationOnShow.Rotate.Enabled || instantAction) ResetRotation();
            UIAnimator.Rotate(RectTransform, UIAnimationOnShow, rotateFrom, rotateTo,
                instantAction); //initialize and play the SHOW Rotate tween

            //SCALE
            var scaleFrom = UIAnimator.GetAnimationScaleFrom(UIAnimationOnShow, StartScale);
            var scaleTo = UIAnimator.GetAnimationScaleTo(UIAnimationOnShow, StartScale);
            if (!UIAnimationOnShow.Scale.Enabled || instantAction) ResetScale();
            UIAnimator.Scale(RectTransform, UIAnimationOnShow, scaleFrom, scaleTo,
                instantAction); //initialize and play the SHOW Scale tween

            //FADE
            var fadeFrom = UIAnimator.GetAnimationFadeFrom(UIAnimationOnShow, StartAlpha);
            var fadeTo = UIAnimator.GetAnimationFadeTo(UIAnimationOnShow, StartAlpha);
            if (!UIAnimationOnShow.Fade.Enabled || instantAction) ResetAlpha();
            UIAnimator.Fade(RectTransform, UIAnimationOnShow, fadeFrom, fadeTo,
                instantAction); //initialize and play the SHOW Fade tween
            Visibility = VisibilityState.Showing; //update the visibility state

            var startTime = Time.realtimeSinceStartup;
            if (!instantAction) //wait for the animation to finish
            {
//                yield return new WaitForSecondsRealtime(ShowBehavior.Animation.TotalDuration);

                var totalDuration = UIAnimationOnShow.TotalDuration;
                var elapsedTime = startTime - Time.realtimeSinceStartup;
                var startDelay = UIAnimationOnShow.StartDelay;
                var invokedOnStart = false;
                while (elapsedTime <= totalDuration) //wait for seconds realtime (ignore Unity's Time.Timescale)
                {
                    elapsedTime = Time.realtimeSinceStartup - startTime;

                    if (!invokedOnStart && elapsedTime > startDelay) invokedOnStart = true;
                    //开始
                    VisibilityProgress = elapsedTime / totalDuration;
                    yield return null;
                }
            }

            //结束
            m_ActionOnShowComplete?.Invoke();
            Visibility = VisibilityState.Visible;
        }

        /// <summary>
        ///     隐藏UI
        /// </summary>
        /// <param name="instantAction">是否直接隐藏</param>
        /// <param name="onHideComplete">隐藏完成事件</param>
        public void Hide(bool instantAction = false, Action onHideComplete = null)
        {
            if (!UIAnimationOnHide.Enabled && !instantAction)
            {
                Debug.LogWarning("you are trying to Hide UIView,but you did not enable any Hide animations");
                return;
            }

            if (Visibility == VisibilityState.Hiding) return;

            m_ActionOnHideComplete = onHideComplete;
            if (gameObject.activeInHierarchy) m_hideCoroutine = StartCoroutine(HideEnumerator(instantAction));
        }

        private IEnumerator HideEnumerator(bool instantAction)
        {
            UIAnimator.StopAnimations(RectTransform, UIAnimationOnHide.AnimationType);

            //MOVE
            if (m_controlledByLayoutGroup) UpdateStartPosition();
            var moveFrom = UIAnimator.GetAnimationMoveFrom(RectTransform, UIAnimationOnHide, CurrentStartPosition);
            var moveTo = UIAnimator.GetAnimationMoveTo(RectTransform, UIAnimationOnHide, CurrentStartPosition);
            if (!UIAnimationOnHide.Move.Enabled || instantAction) ResetPosition();
            UIAnimator.Move(RectTransform, UIAnimationOnHide, moveFrom, moveTo,
                instantAction); //initialize and play the HIDE Move tween

            //ROTATE
            var rotateFrom = UIAnimator.GetAnimationRotateFrom(UIAnimationOnHide, StartRotation);
            var rotateTo = UIAnimator.GetAnimationRotateTo(UIAnimationOnHide, StartRotation);
            if (!UIAnimationOnHide.Rotate.Enabled || instantAction) ResetRotation();
            UIAnimator.Rotate(RectTransform, UIAnimationOnHide, rotateFrom, rotateTo,
                instantAction); //initialize and play the HIDE Rotate tween

            //SCALE
            var scaleFrom = UIAnimator.GetAnimationScaleFrom(UIAnimationOnHide, StartScale);
            var scaleTo = UIAnimator.GetAnimationScaleTo(UIAnimationOnHide, StartScale);
            if (!UIAnimationOnHide.Scale.Enabled || instantAction) ResetScale();
            UIAnimator.Scale(RectTransform, UIAnimationOnHide, scaleFrom, scaleTo,
                instantAction); //initialize and play the HIDE Scale tween

            //FADE
            var fadeFrom = UIAnimator.GetAnimationFadeFrom(UIAnimationOnHide, StartAlpha);
            var fadeTo = UIAnimator.GetAnimationFadeTo(UIAnimationOnHide, StartAlpha);
            if (!UIAnimationOnHide.Fade.Enabled || instantAction) ResetAlpha();
            UIAnimator.Fade(RectTransform, UIAnimationOnHide, fadeFrom, fadeTo,
                instantAction); //initialize and play the HIDE Fade tween

            Visibility = VisibilityState.Hiding;
            if (m_initialized)
                if (instantAction)
                {
                    //开始隐藏
                }

            var startTime = Time.realtimeSinceStartup;
            if (!instantAction) //wait for the animation to finish
            {
//                    yield return new WaitForSecondsRealtime(HideBehavior.Animation.TotalDuration + UIViewSettings.DISABLE_WHEN_HIDDEN_TIME_BUFFER); //wait for seconds realtime (ignore Unity's Time.Timescale)

                var totalDuration = UIAnimationOnHide.TotalDuration;
                var elapsedTime = startTime - Time.realtimeSinceStartup;
                var startDelay = UIAnimationOnHide.StartDelay;
                var invokedOnStart = false;
                while (elapsedTime <= totalDuration) //wait for seconds realtime (ignore Unity's Time.Timescale)
                {
                    elapsedTime = Time.realtimeSinceStartup - startTime;

                    if (!invokedOnStart && elapsedTime > startDelay)
                        //开始隐藏
                        invokedOnStart = true;
                    VisibilityProgress = 1 - elapsedTime / totalDuration; //operation is reversed in hide than in show
                    yield return null;
                }

                yield return
                    new WaitForSecondsRealtime(0.05f); //wait for seconds realtime (ignore Unity's Time.Timescale)
            }

            if (m_initialized)
            {
                //隐藏结束
            }

            Visibility = VisibilityState.NotVisible;
            Canvas.enabled = false; //disable the canvas, if the option is enabled
            GraphicRaycaster.enabled = false; //disable the graphic raycaster, if the option is enabled
            gameObject.SetActive(false); //disable the Source the UIView is attached to, if the option is enabled
            m_hideCoroutine = null; //clear the coroutine reference
            m_ActionOnHideComplete?.Invoke();
            if (!m_initialized) m_initialized = true;
        }

        public void UpdateStartValues()
        {
            UpdateStartPosition();
            UpdateStartRotation();
            UpdateStartScale();
            UpdateStartAlpha();
        }

        public void ResetToStartValues()
        {
            UIAnimator.ResetCanvasGroup(RectTransform);
            ResetPosition();
            ResetRotation();
            ResetScale();
            ResetAlpha();
        }

        /// <summary> Updates the StartPosition to the RectTransform.anchoredPosition3D value </summary>
        private void UpdateStartPosition()
        {
            StartPosition = RectTransform.anchoredPosition3D;
        }

        /// <summary> Updates the StartRotation to the RectTransform.localEulerAngles value </summary>
        private void UpdateStartRotation()
        {
            StartRotation = RectTransform.localEulerAngles;
        }

        /// <summary> Updates the StartScale to the RectTransform.localScale value </summary>
        private void UpdateStartScale()
        {
            var localScale = RectTransform.localScale;
            StartScale = new Vector3(localScale.x, localScale.y, 1f);
        }

        /// <summary> Updates the StartAlpha to the CanvasGroup.alpha value (if a CanvasGroup is attached) </summary>
        private void UpdateStartAlpha()
        {
            StartAlpha = RectTransform.GetComponent<CanvasGroup>() == null
                ? 1
                : RectTransform.GetComponent<CanvasGroup>().alpha;
        }

        #region 变量

        public UIViewStartBehavior StartBehavior = UIViewStartBehavior.DoNothing;

        /// <summary>
        ///     打开界面动画的 组
        /// </summary>
        public string OnShowPresetCategory;

        /// <summary>
        ///     打开界面动画的 名称
        /// </summary>
        public string OnShowPresetName;

        public Move MoveOnShow;

        public Rotate RotateOnShow;

        public Scale ScaleOnShow;

        public Fade FadeOnShow;

        public UIAnimation UIAnimationOnShow;

        /// <summary>
        ///     关闭界面动画的 组
        /// </summary>
        public string OnHidePresetCategory;

        /// <summary>
        ///     关闭界面动画的 名称
        /// </summary>
        public string OnHidePresetName;

        public Move MoveOnHide;

        public Rotate RotateOnHide;

        public Scale ScaleOnHide;

        public Fade FadeOnHide;

        public UIAnimation UIAnimationOnHide;

        #endregion

        #region Properties

        /// <summary>
        ///     可见 进度改变事件
        /// </summary>
        public UnityEvent<float> OnVisibilityChanged = new();

        /// <summary>
        ///     不可见 进度改变事件
        /// </summary>
        public UnityEvent<float> OnInverseVisibilityChanged = new();

        /// <summary>
        ///     UI可见 进度
        /// </summary>
        public float VisibilityProgress
        {
            get => m_visibilityProgress;
            set
            {
                m_visibilityProgress = Mathf.Clamp01(value);
                OnVisibilityChanged.Invoke(VisibilityProgress);
                OnInverseVisibilityChanged.Invoke(InverseVisibility);
            }
        }

        /// <summary>
        ///     UI不可见 进度
        /// </summary>
        public float InverseVisibility => 1 - VisibilityProgress;

        public RectTransform RectTransform
        {
            get
            {
                if (m_rectTransform == null) m_rectTransform = GetComponent<RectTransform>();

                return m_rectTransform;
            }
        }

        public Canvas Canvas
        {
            get
            {
                if (m_canvas == null) m_canvas = GetComponent<Canvas>();
                return m_canvas;
            }
        }

        public GraphicRaycaster GraphicRaycaster
        {
            get
            {
                if (m_graphicRaycaster == null) m_graphicRaycaster = GetComponent<GraphicRaycaster>();
                return m_graphicRaycaster;
            }
        }

        public VisibilityState Visibility = VisibilityState.Visible;

        public Vector3 CurrentStartPosition =>
            UseCustomStartAnchoredPosition ? CustomStartAnchoredPosition : StartPosition;

        public bool UseCustomStartAnchoredPosition = true;
        public Vector3 CustomStartAnchoredPosition = new(0, 0, 0);
        public Vector3 StartPosition = UIAnimator.DEFAULT_START_POSITION;

        public Vector3 StartRotation = UIAnimator.DEFAULT_START_ROTATION;
        public Vector3 StartScale = UIAnimator.DEFAULT_START_SCALE;
        public float StartAlpha = UIAnimator.DEFAULT_START_ALPHA;

        public void ResetRotation()
        {
            RectTransform.localEulerAngles = StartRotation;
        }

        public void ResetPosition()
        {
            RectTransform.anchoredPosition3D = CurrentStartPosition;
        }

        public void ResetScale()
        {
            RectTransform.localScale = new Vector3(StartScale.x, StartScale.y, 1f);
        }

        public void ResetAlpha()
        {
            if (RectTransform.GetComponent<CanvasGroup>() != null)
                RectTransform.GetComponent<CanvasGroup>().alpha = StartAlpha;
        }

        #endregion

        #region Private Variables

        private float m_visibilityProgress = 1f;
        private Canvas m_canvas;
        private GraphicRaycaster m_graphicRaycaster;
        private bool m_initialized;
        private bool m_controlledByLayoutGroup;
        private Coroutine m_showCoroutine;
        private Coroutine m_hideCoroutine;
        private Coroutine m_autoHideCoroutine;
        private RectTransform m_rectTransform;
        private Action m_ActionOnShowComplete;
        private Action m_ActionOnHideComplete;

        #endregion
    }

    public enum VisibilityState
    {
        /// <summary> Is visible </summary>
        Visible = 0,

        /// <summary> Is NOT visible </summary>
        NotVisible = 1,

        /// <summary> Is playing the HIDE animation (in transition exit view) </summary>
        Hiding = 2,

        /// <summary> Is playing the SHOW animation (in transition enter view) </summary>
        Showing = 3
    }

    public enum UIViewStartBehavior
    {
        /// <summary>
        ///     Do Nothing.
        ///     <para />
        ///     Used when the UIView is visible and should not do anything else.
        /// </summary>
        DoNothing,

        /// <summary>
        ///     Start hidden.
        ///     <para />
        ///     Used when the UIView should be out of view at Start.
        ///     <para />
        ///     This triggers an instant auto HIDE animation, thus the UIView hides in zero seconds.
        /// </summary>
        Hide,

        /// <summary>
        ///     Start by playing the SHOW animation.
        ///     <para />
        ///     Used when the UIView should animate becoming visible at Start.
        ///     <para />
        ///     This triggers and instant HIDE and then an automated SHOW animation immediately after that.
        /// </summary>
        PlayShowAnimation
    }
}