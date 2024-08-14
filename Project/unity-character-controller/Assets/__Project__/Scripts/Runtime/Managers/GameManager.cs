using DG.Tweening;
using Settings;
using UnityEngine;
using VContainer.Unity;

namespace Managers
{
    public class GameManager : IStartable
    {
        #region PRIVATE_VARIABLES

        private readonly GameSettings _gameSettings;

        #endregion

        #region CONSTRUCTORS

        public GameManager(GameSettings gameSettings)
        {
            _gameSettings = gameSettings;
        }

        #endregion

        #region PUBLIC_FUNCTIONS

        public void Start()
        {
            Init();
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        private void Init()
        {
            Input.multiTouchEnabled = _gameSettings.MultiTouch;
            Application.targetFrameRate = _gameSettings.TargetFramerate;
            DOTween.SetTweensCapacity(_gameSettings.TweenersCapacity, _gameSettings.SequencesCapacity);
        }

        #endregion
    }
}