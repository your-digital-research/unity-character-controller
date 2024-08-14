using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace Gameplay
{
    public class CameraController : MonoBehaviour
    {
        #region SERIALIZED_VARIABLES

        [Header("References")]
        [SerializeField] private CinemachineBlenderSettings blenderSettings;
        [SerializeField] private CameraSettings cameraSettings;

        [Header("Cameras")]
        [SerializeField] private CinemachineBrain brain;
        [SerializeField] private List<CameraInstance> cameraInstances;

        #endregion

        #region PRIVATE_VARIABLES

        //

        #endregion

        #region PROPERTIES

        public CinemachineBrain Brain => brain;
        public CameraInstance Current { get; private set; }

        #endregion

        #region EVENTS

        //

        #endregion

        #region MONO

        private void Start()
        {
            Init();
        }

        #endregion

        #region PUBLIC_FUNCTIONS

        public CameraInstance GetCameraByType(CameraKind kind)
        {
            return cameraInstances.Find(instance => instance.Kind == kind);
        }

        public void SetTargets(Transform target)
        {
            cameraInstances.ForEach(instance =>
            {
                UpdateCameraTarget(instance, target);
            });
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        private void Init()
        {
            SwitchCameraTo(CameraKind.Default);
        }

        private void SwitchCameraTo(CameraKind kind)
        {
            cameraInstances.ForEach(instance =>
            {
                int priority;

                if (instance.Kind == kind)
                {
                    Current = instance;
                    priority = (int)CameraPriority.Enabled;
                }
                else
                {
                    priority = (int)CameraPriority.Disabled;
                }

                instance.VirtualCamera.Priority = priority;
            });
        }

        private void UpdateCameraTarget(CameraInstance cameraInstance, Transform target)
        {
            cameraInstance.VirtualCamera.Follow = target;
            cameraInstance.VirtualCamera.LookAt = target;
        }

        private void UpdateCameraTarget(CameraKind kind, Transform target)
        {
            var cameraInstance = GetCameraByType(kind);

            cameraInstance.VirtualCamera.Follow = target;
            cameraInstance.VirtualCamera.LookAt = target;
        }

        #endregion
    }
}