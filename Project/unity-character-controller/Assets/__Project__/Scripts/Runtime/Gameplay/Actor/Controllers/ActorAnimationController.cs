using System;
using R3;
using UnityEngine;

namespace Gameplay
{
    [RequireComponent(typeof(ActorMovementController))]
    public class ActorAnimationController : MonoBehaviour
    {
        #region SERIALIZED_VARIABLES

        [Header("References")]
        [SerializeField] private Animator actorAnimator;

        #endregion

        #region PRIVATE_VARIABLES

        private ActorMovementController _movementController;

        private IDisposable _updateDisposable;

        private static readonly int IsWalkingHash = Animator.StringToHash("IsWalking");
        private static readonly int IsRunningHash = Animator.StringToHash("IsRunning");
        private static readonly int IsJumpingHash = Animator.StringToHash("IsJumping");
        private static readonly int JumpIndexHash = Animator.StringToHash("JumpIndex");

        #endregion

        #region MONO

        private void Start()
        {
            Init();
        }

        private void OnDestroy()
        {
            StopUpdate();

            RemoveListeners();
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        private void OnJump(int index)
        {
            actorAnimator.SetBool(IsJumpingHash, true);
            actorAnimator.SetInteger(JumpIndexHash, index);
        }

        private void OnGrounded()
        {
            actorAnimator.SetBool(IsJumpingHash, false);
            actorAnimator.SetInteger(JumpIndexHash, 0);
        }

        private void Init()
        {
            InitMovement();

            AddListeners();

            StartUpdate();
        }

        private void InitMovement()
        {
            _movementController = GetComponent<ActorMovementController>();
        }

        private void HandleAnimations()
        {
            bool isWalking = actorAnimator.GetBool(IsWalkingHash);
            bool isRunning = actorAnimator.GetBool(IsRunningHash);

            switch (_movementController.IsMovementPressed)
            {
                case true when !isWalking:
                    actorAnimator.SetBool(IsWalkingHash, true);
                    break;
                case false when isWalking:
                    actorAnimator.SetBool(IsWalkingHash, false);
                    break;
            }

            if ((_movementController.IsMovementPressed && _movementController.IsRunPressed) && !isRunning)
            {
                actorAnimator.SetBool(IsRunningHash, true);
            }
            else if ((!_movementController.IsMovementPressed || !_movementController.IsRunPressed) && isRunning)
            {
                actorAnimator.SetBool(IsRunningHash, false);
            }
        }

        private void StartUpdate()
        {
            _updateDisposable ??= Observable
                .EveryUpdate(UnityFrameProvider.Update)
                .Subscribe(_ =>
                {
                    HandleAnimations();
                });
        }

        private void StopUpdate()
        {
            _updateDisposable?.Dispose();
            _updateDisposable = null;
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