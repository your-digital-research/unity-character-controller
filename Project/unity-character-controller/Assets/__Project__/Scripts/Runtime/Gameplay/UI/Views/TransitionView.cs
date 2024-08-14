using System;
using DG.Tweening;
using Settings;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Gameplay
{
    public class TransitionView : MonoBehaviour
    {
        #region SERIALIZED_VARIABLES

        [Header("References")]
        [SerializeField] private Image background;
        [SerializeField] private Image loadingIcon;
        [SerializeField] private CanvasGroup canvasGroup;

        #endregion

        #region PRIVATE_VARIABLES

        private TransitionSettings _settings;

        #endregion

        #region EVENTS

        public event Action ShowStart;
        public event Action ShowComplete;
        public event Action HideStart;
        public event Action HideComplete;

        #endregion

        #region CONSTRUCTORS

        [Inject]
        private void Constructor(TransitionSettings settings)
        {
            _settings = settings;
        }

        #endregion

        #region MONO

        private void Start()
        {
            Init();
        }

        private void OnDestroy()
        {
            StopLoadingIconAnimation();

            KillTweens();

            RemoveListeners();
        }

        #endregion

        #region PUBLIC_FUNCTIONS

        public void Show(bool force = false, Action onComplete = null)
        {
            ShowStart?.Invoke();

            canvasGroup.DOKill();

            if (force)
            {
                ShowLoadingIcon();

                canvasGroup.alpha = 1;
                background.raycastTarget = true;

                onComplete?.Invoke();

                ShowComplete?.Invoke();
            }
            else
            {
                canvasGroup
                    .DOFade(1, _settings.TransitionDuration)
                    .SetDelay(_settings.ViewShowDelay)
                    .SetEase(Ease.Linear)
                    .OnStart(() => { background.raycastTarget = true; })
                    .OnComplete(() =>
                    {
                        ShowLoadingIcon();

                        onComplete?.Invoke();

                        ShowComplete?.Invoke();
                    });
            }
        }

        public void Hide(bool force = false, Action onComplete = null)
        {
            HideStart?.Invoke();

            HideLoadingIcon(force);

            canvasGroup.DOKill();

            if (force)
            {
                background.raycastTarget = false;
                canvasGroup.alpha = 0;

                onComplete?.Invoke();

                HideComplete?.Invoke();
            }
            else
            {
                canvasGroup
                    .DOFade(0, _settings.TransitionDuration)
                    .SetDelay(_settings.ViewHideDelay)
                    .SetEase(Ease.Linear)
                    .OnComplete(() =>
                    {
                        background.raycastTarget = false;

                        onComplete?.Invoke();

                        HideComplete?.Invoke();
                    });
            }
        }


        #endregion

        #region PRIVATE_FUNCTIONS

        private void Init()
        {
            background.raycastTarget = true;
            canvasGroup.alpha = 1;

            ShowLoadingIcon();

            AddListeners();
        }

        private void ShowLoadingIcon(bool force = false)
        {
            Transform iconTransform = loadingIcon.transform;

            iconTransform.localScale = Vector3.zero;

            if (force)
            {
                iconTransform.localScale = Vector3.one;

                loadingIcon.gameObject.SetActive(true);

                StartLoadingIconAnimation();
            }
            else
            {
                iconTransform
                    .DOScale(Vector3.one, _settings.IconShowHideDuration)
                    .SetDelay(_settings.IconShowDelay)
                    .SetEase(Ease.OutBack, 3)
                    .OnStart(() =>
                    {
                        loadingIcon.gameObject.SetActive(true);

                        StartLoadingIconAnimation();
                    });
            }
        }

        private void HideLoadingIcon(bool force = false)
        {
            Transform iconTransform = loadingIcon.transform;

            iconTransform.localScale = Vector3.one;

            if (force)
            {
                iconTransform.localScale = Vector3.zero;

                loadingIcon.gameObject.SetActive(false);

                StopLoadingIconAnimation();
            }
            else
            {
                iconTransform
                    .DOScale(Vector3.zero, _settings.IconShowHideDuration)
                    .SetDelay(_settings.IconHideDelay)
                    .SetEase(Ease.InBack, 3)
                    .OnComplete(() =>
                    {
                        loadingIcon.gameObject.SetActive(false);

                        StopLoadingIconAnimation();
                    });
            }
        }

        private void StartLoadingIconAnimation()
        {
            Transform iconTransform = loadingIcon.transform;

            iconTransform
                .DORotate(new Vector3(0, 0, -360), _settings.IconRotationDuration, RotateMode.LocalAxisAdd)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Restart);
        }

        private void StopLoadingIconAnimation()
        {
            Transform iconTransform = loadingIcon.transform;

            iconTransform.DOKill();
        }

        private void KillTweens()
        {
            canvasGroup.DOKill();
        }

        private void AddListeners()
        {
            //
        }

        private void RemoveListeners()
        {
            //
        }

        #endregion
    }
}