using UnityEngine;

namespace Core.Gameplay.Character
{
    [RequireComponent(typeof(CharacterMovementController))]
    public class CharacterAnimationController : MonoBehaviour
    {
        #region SERIALIZED_VARIABLES

        [Header("References")]
        [SerializeField] private Animator characterAnimator;

        #endregion

        #region PRIVATE_VARIABLES

        private CharacterMovementController _movementController;

        private static readonly int IsWalkingHash = Animator.StringToHash("IsWalking");
        private static readonly int IsRunningHash = Animator.StringToHash("IsRunning");

        #endregion

        #region MONO

        private void Awake()
        {
            Init();
        }

        private void Update()
        {
            HandleAnimations();
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        private void Init()
        {
            InitMovement();
        }

        private void InitMovement()
        {
            _movementController = GetComponent<CharacterMovementController>();
        }

        private void HandleAnimations()
        {
            bool isWalking = characterAnimator.GetBool(IsWalkingHash);
            bool isRunning = characterAnimator.GetBool(IsRunningHash);

            switch (_movementController.IsMovementPressed)
            {
                case true when !isWalking:
                    characterAnimator.SetBool(IsWalkingHash, true);
                    break;
                case false when isWalking:
                    characterAnimator.SetBool(IsWalkingHash, false);
                    break;
            }

            if ((_movementController.IsMovementPressed && _movementController.IsRunPressed) && !isRunning)
            {
                characterAnimator.SetBool(IsRunningHash, true);
            }
            else if ((!_movementController.IsMovementPressed || !_movementController.IsRunPressed) && isRunning)
            {
                characterAnimator.SetBool(IsRunningHash, false);
            }
        }

        #endregion
    }
}