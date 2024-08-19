using Managers;
using Settings;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Context
{
    public class RootLifetimeScope : LifetimeScope
    {
        #region SERIALIZED_VARIABLES

        [Header("Settings")]
        [SerializeField] private GameSettings gameSettings;

        #endregion

        #region PROTECTED_FUNCTIONS

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(gameSettings);
            builder.RegisterEntryPoint<GameManager>();
        }

        #endregion
    }
}