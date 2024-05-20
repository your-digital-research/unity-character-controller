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
        private static readonly int IsJumpingHash = Animator.StringToHash("IsJumping");
        private static readonly int JumpIndexHash = Animator.StringToHash("JumpIndex");

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

        private void OnDestroy()
        {
            RemoveListeners();
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        private void OnJump(int index)
        {
            characterAnimator.SetBool(IsJumpingHash, true);
            characterAnimator.SetInteger(JumpIndexHash, index);
        }

        private void OnGrounded()
        {
            characterAnimator.SetBool(IsJumpingHash, false);
            characterAnimator.SetInteger(JumpIndexHash, 0);
        }

        private void Init()
        {
            InitMovement();
            AddListeners();
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

        private void AddListeners()
        {
            _movementController.Jump += OnJump;
            _movementController.Grounded += OnGrounded;
        }

        private void RemoveListeners()
        {
            _movementController.Jump -= OnJump;
            _movementController.Grounded -= OnGrounded;
        }

        #endregion
    }
}