using UnityEngine;
using Cinemachine;

namespace Gameplay
{
    public class CameraController : MonoBehaviour
    {
        #region SERIALIZED_VARIABLES

        [Header("Cameras")]
        [SerializeField] private CinemachineBrain brain;
        [SerializeField] private CinemachineFreeLook main;

        #endregion

        #region PROPERTIES

        public CinemachineBrain Brain => brain;
        public CinemachineFreeLook Main => main;

        #endregion
    }
}