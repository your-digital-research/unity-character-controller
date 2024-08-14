using UnityEngine;
using VContainer;

namespace Gameplay
{
    public class LevelController : MonoBehaviour
    {
        #region SERIALIZED_VARIABLES

        [Header("Points")]
        [SerializeField] private Transform playerStartPoint;

        #endregion

        #region PRIVATE_VARIABLES

        private ActorController _playerController;
        private CameraController _cameraController;
        private TransitionView _transitionView;

        #endregion

        #region PROPERTIES

        public Transform PlayerStartPoint => playerStartPoint;

        #endregion

        #region CONSTRUCTORS

        [Inject]
        private void Constructor(ActorController playerController, CameraController cameraController, TransitionView transitionView)
        {
            _playerController = playerController;
            _cameraController = cameraController;
            _transitionView = transitionView;
        }

        #endregion

        #region MONO

        private void Start()
        {
            Init();
        }

        #endregion

        #region PUBLIC_FUNCTIONS

        //

        #endregion

        #region PRIVATE_FUNCTIONS

        private void Init()
        {
            InitPlayer();
            InitCamera();
        }

        private void InitPlayer()
        {
            _playerController.SetPosition(playerStartPoint.position);
            _playerController.SetRotation(Quaternion.identity);
        }

        private void InitCamera()
        {
            _cameraController.SetTargets(_playerController.Center);
        }

        #endregion
    }
}