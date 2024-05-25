using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace Core.Gameplay.Camera
{
    public class CameraController : MonoBehaviour
    {
        #region SERIALIZED_VARIABLES

        [Header("Cameras")]
        [SerializeField] private CinemachineBrain brain;
        [SerializeField] private List<CameraInstance> instances;

        #endregion

        #region PROPERTIES

        public CinemachineBrain Brain => brain;

        #endregion

        #region PUBLIC_FUNCTIONS

        public CinemachineVirtualCamera GetCameraByType(CameraKind kind)
        {
            return instances.Find(instance => instance.Kind == kind).VirtualCamera;
        }

        #endregion
    }
}