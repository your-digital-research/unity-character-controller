using UnityEngine;
using Cinemachine;

namespace Core.Gameplay.Camera
{
    public class CameraInstance : MonoBehaviour
    {
        #region SERIALIZED_VARIABLES

        [Header("Settings")]
        [SerializeField] private CameraKind kind;

        [Header("References")]
        [SerializeField] private CinemachineVirtualCamera virtualCamera;

        #endregion

        #region PROPERTIES

        public CameraKind Kind => kind;
        public CinemachineVirtualCamera VirtualCamera => virtualCamera;

        #endregion
    }
}