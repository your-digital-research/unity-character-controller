using System;
using DG.Tweening;
using UnityEngine;

namespace Gameplay
{
    public class MovingPlatform : Platform
    {
        #region SERIALIZED_VARIABLES

        [Header("References")]
        [SerializeField] private GameObject platform;

        [Header("Settings")]
        [SerializeField] [Range(0.25f, 5)] private float duration;

        [Header("Points")]
        [SerializeField] private Transform startPoint;
        [SerializeField] private Transform endPoint;

        #endregion

        #region PRIVATE_VARIABLES

        private Vector3 _startPosition;
        private Vector3 _endPosition;

        #endregion

        #region MONO

        private void Awake()
        {
            Init();
        }

        private void OnEnable()
        {
            ResetPlatformPosition();
            StartMovement();
        }

        private void OnDisable()
        {
            StopMovement();
            ResetPlatformPosition();
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        private void Init()
        {
            InitPositions();
        }

        private void InitPositions()
        {
            _startPosition = startPoint.localPosition;
            _endPosition = endPoint.localPosition;
        }

        private void ResetPlatformPosition()
        {
            platform.transform.localPosition = _startPosition;
        }

        private void StartMovement()
        {
            platform.transform
                .DOLocalMove(_endPosition, duration)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Yoyo);
        }

        private void StopMovement()
        {
            DOTween.Kill(platform.transform);
        }

        #endregion
    }
}