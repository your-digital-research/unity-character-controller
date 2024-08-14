using Gameplay;
using Settings;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using VContainer.Unity;

namespace Managers
{
    public class LevelManager : IStartable
    {
        #region PRIVATE_VARIABLES

        private readonly LevelSettings _levelSettings;
        private readonly TransitionView _transitionView;

        #endregion

        #region CONSTRUCTORS

        public LevelManager(LevelSettings levelSettings, TransitionView transitionView)
        {
            _levelSettings = levelSettings;
            _transitionView = transitionView;
        }

        #endregion

        #region PUBLIC_FUNCTIONS

        public void Start()
        {
            LoadLevelAsync();
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        private void LoadLevelAsync()
        {
            var handler = Addressables.LoadSceneAsync(_levelSettings.Level, LoadSceneMode.Additive, false);

            handler.Completed += handle =>
            {
                var activeAsync = handle.Result.ActivateAsync();

                activeAsync.completed += _ =>
                {
                    // Debug.Log("Level scene load completed.");

                    _transitionView.Hide();
                };
            };
        }

        #endregion
    }
}