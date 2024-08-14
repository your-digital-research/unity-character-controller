using Gameplay;
using Settings;
using UnityEngine;
using UnityEngine.AddressableAssets;
using VContainer;
using VContainer.Unity;

namespace Context
{
    public class BootLifetimeScope : LifetimeScope
    {
        #region SERIALIZED_VARIABLES

        [Header("References")]
        [SerializeField] private AssetReference mainScene;

        [Header("Transition")]
        [SerializeField] private TransitionView transitionView;
        [SerializeField] private TransitionSettings transitionSettings;

        #endregion

        #region PROTECTED_FUNCTIONS

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(mainScene);

            builder.RegisterComponent(transitionView);
            builder.RegisterInstance(transitionSettings);

            builder.RegisterEntryPoint<MainSceneLoader>();
        }

        #endregion
    }
}