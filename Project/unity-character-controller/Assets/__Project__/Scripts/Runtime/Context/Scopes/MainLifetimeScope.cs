using Gameplay;
using Managers;
using Settings;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Context
{
    public class MainLifetimeScope : LifetimeScope
    {
        #region SERIALIZED_VARIABLES

        [Header("Settings")]
        [SerializeField] private LevelSettings levelSettings;

        [Header("Controllers")]
        [SerializeField] private ActorController playerController;
        [SerializeField] private CameraController cameraController;

        #endregion

        #region PROTECTED_FUNCTIONS

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(levelSettings);

            builder.Register<LevelManager>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();

            builder.UseComponents(components =>
            {
                components.AddInstance(playerController);
                components.AddInstance(cameraController);
            });
        }

        #endregion
    }
}