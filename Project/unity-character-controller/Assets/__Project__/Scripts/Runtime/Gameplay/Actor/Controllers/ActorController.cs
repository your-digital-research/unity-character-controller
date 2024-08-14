using UnityEngine;
using VContainer;

namespace Gameplay
{
    [RequireComponent(typeof(ActorStateMachine))]
    public class ActorController : MonoBehaviour
    {
        #region SERIALIZED_VARIABLES

        [Header("References")]
        [SerializeField] private ActorStateMachine _actorStateMachine;

        [Header("Points")]
        [SerializeField] private Transform center;

        #endregion

        #region PROPERTIES

        public Transform Center => center;
        public Vector3 Position => transform.position;
        public Quaternion Rotation => transform.rotation;

        #endregion

        #region CONSTRUCTORS

        [Inject]
        private void Constructor(CameraController cameraController)
        {
            _actorStateMachine.CameraController = cameraController;
        }

        #endregion

        #region MONO

        private void Start()
        {
            InitStateMachine();
        }

        #endregion

        #region PUBLIC_FUNCTIONS

        public virtual void SetPosition(Vector3 position)
        {
            transform.position = position;
        }

        public virtual void SetRotation(Quaternion rotation)
        {
            transform.rotation = rotation;
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        private void InitStateMachine()
        {
            _actorStateMachine.Init();
        }

        #endregion
    }
}