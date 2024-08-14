using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gameplay;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using VContainer.Unity;

namespace Context
{
    public class MainSceneLoader : IStartable, ITickable
    {
        #region PRIVATE_VARIABLES

        private bool _isRestarting;

        private readonly LifetimeScope _parent;
        private readonly AssetReference _mainScene;
        private readonly TransitionView _transitionView;

        #endregion

        #region CONSTRUCTORS

        public MainSceneLoader(LifetimeScope lifetimeScope, AssetReference mainScene, TransitionView transitionView)
        {
            _parent = lifetimeScope;
            _mainScene = mainScene;
            _transitionView = transitionView;
        }

        #endregion

        #region PUBLIC_FUNCTIONS

        public void Start()
        {
            Init();
        }

        public void Tick()
        {
            CheckForRestart();
        }

        #endregion

        #region PRIVATE_VARIABLES

        private bool IsAllLevelsLoaded()
        {
            int sceneCount = SceneManager.sceneCount;

            for (int i = sceneCount - 1; i > 0; i--)
            {
                Scene scene = SceneManager.GetSceneAt(i);

                if (!scene.isLoaded)
                {
                    return false;
                }
            }

            return true;
        }

        private void Init()
        {
            _transitionView.Show(true);

            StartSceneLoading();
        }

        private void StartSceneLoading()
        {
            // NOTE - Loading with Coroutines changes the parent of a LifetimeScope for a new scene
            // LoadSceneAsync().StartCoroutine();

            // Note - Loading with Addressables do not change the parent of a LifetimeScope for a new scene
            LoadMainScene();
        }

        // NOTE - Load scene with Addressables (have to manually assign Parent lifetimeScope from Editor)
        private void LoadMainScene()
        {
            using (LifetimeScope.EnqueueParent(_parent))
            {
                var handler = Addressables.LoadSceneAsync(_mainScene, LoadSceneMode.Additive, false);

                handler.Completed += handle =>
                {
                    var activeAsync = handle.Result.ActivateAsync();

                    activeAsync.completed += _ =>
                    {
                        // Debug.Log("Main scene load completed.");

                        _isRestarting = false;
                    };
                };
            }
        }

        // NOTE - Another approach to asynchronously load a scene (From VContainer documentation)
        private IEnumerator LoadSceneAsync()
        {
            // LifetimeScope generated in this block will be parented by `this.lifetimeScope`
            using (LifetimeScope.EnqueueParent(_parent))
            {
                // If this scene has a LifetimeScope, its parent will be `parent`.
                var loading = SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);

                while (!loading.isDone)
                {
                    yield return null;
                }

                // Debug.Log("Main scene load completed.");

                _isRestarting = false;
            }
        }

        private async void Restart()
        {
            if (_isRestarting || !IsAllLevelsLoaded()) return;

            _transitionView.Show(true);

            _isRestarting = true;

            await UnloadAllScenesAsync();

            StartSceneLoading();
        }

        private async Task UnloadAllScenesAsync()
        {
            int sceneCount = SceneManager.sceneCount;

            List<AsyncOperation> unloadOperations = new List<AsyncOperation>();

            for (int i = sceneCount - 1; i > 0; i--)
            {
                Scene scene = SceneManager.GetSceneAt(i);

                if (scene.isLoaded)
                {
                    unloadOperations.Add(SceneManager.UnloadSceneAsync(scene));
                }
            }

            await Task.WhenAll(unloadOperations.Select(WaitForSceneUnload));

            // Debug.Log("All scenes have been unloaded.");
        }

        private async Task WaitForSceneUnload(AsyncOperation asyncOperation)
        {
            while (!asyncOperation.isDone)
            {
                await Task.Yield();
            }
        }

        #endregion

        #region DEBUG

        private void CheckForRestart()
        {
            if (!Input.GetKeyDown(KeyCode.R)) return;

            Restart();
        }

        #endregion
    }
}