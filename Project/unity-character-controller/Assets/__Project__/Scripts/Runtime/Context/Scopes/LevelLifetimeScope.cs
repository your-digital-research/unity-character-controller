using Gameplay;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Context
{
    public class LevelLifetimeScope : LifetimeScope
    {
        #region SERIALIZED_VARIABLES

        [Header("Controllers")]
        [SerializeField] private LevelController levelController;

        #endregion

        #region PROTECTED_FUNCTIONS

        protected override void Configure(IContainerBuilder builder)
        {
            builder.UseComponents(components =>
            {
                components.AddInstance(levelController);
            });
        }

        #endregion
    }
}