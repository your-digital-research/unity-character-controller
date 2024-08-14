using System;
using R3;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

namespace Gameplay
{
    [RequireComponent(typeof(ActorController))]
    public class ActorMovementController : MonoBehaviour
    {
        #region SERIALIZED_VARIABLES

        [Header("References")]
        [SerializeField] private CharacterController characterController;
        [SerializeField] private MovementSettings movementSettings;
        [SerializeField] private JumpSettings jumpSettings;

        #endregion

        #region PRIVATE_VARIABLES

        private ActorInput _actorInput;

        private Vector3 _appliedMovement;
        private Vector3 _currentMovement;
        private Vector3 _currentRunMovement;
        private Vector2 _currentMovementInput;

        private int _jumpIndex;
        private bool _isAtJump;

        private IDisposable _updateDisposable;

        #endregion

        #region PROPERTIES

        public bool IsMovementPressed { get; private set; }
        public bool IsRunPressed { get; private set; }
        public bool IsJumpPressed { get; private set; }
        public bool IsJumping { get; private set; }
        public bool IsGrounded => characterController.isGrounded;
        public bool IsFalling => (_currentMovement.y <= 0 || !IsJumpPressed) && !IsGrounded;

        #endregion

        #region EVENTS

        public event Action<int> Jump;
        public event Action Grounded;

        #endregion

        #region MONO

        private void Awake()
        {
            InitInput();
        }

        private void Start()
        {
            Init();
        }

        private void OnEnable()
        {
            ToggleActorControls(true);
        }

        private void OnDisable()
        {
            ToggleActorControls(false);
        }

        private void OnDestroy()
        {
            StopUpdate();

            RemoveListeners();
            ResetInput();
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        private void OnMovementInput(InputAction.CallbackContext obj)
        {
            _currentMovementInput = obj.ReadValue<Vector2>();

            _currentMovement.x = _currentMovementInput.x;
            _currentMovement.z = _currentMovementInput.y;

            _currentRunMovement.x = _currentMovementInput.x * movementSettings.RunMultiplier;
            _currentRunMovement.z = _currentMovementInput.y * movementSettings.RunMultiplier;

            IsMovementPressed = _currentMovementInput.x != 0 || _currentMovementInput.y != 0;
        }

        private void OnRun(InputAction.CallbackContext obj)
        {
            IsRunPressed = obj.ReadValueAsButton();
        }

        private void OnJump(InputAction.CallbackContext obj)
        {
            IsJumpPressed = obj.ReadValueAsButton();
        }

        private void Init()
        {
            InitInput();

            AddListeners();

            StartUpdate();
        }

        private void InitInput()
        {
            _actorInput = new ActorInput();
        }

        private void ResetInput()
        {
            _actorInput = null;
        }

        private void ToggleActorControls(bool value)
        {
            switch (value)
            {
                case true:
                    _actorInput.ActorControls.Enable();
                    break;
                case false:
                    _actorInput.ActorControls.Disable();
                    break;
            }
        }

        private void UpdateMovement()
        {
            if (IsRunPressed)
            {
                _appliedMovement.x = _currentRunMovement.x;
                _appliedMovement.z = _currentRunMovement.z;
            }
            else
            {
                _appliedMovement.x = _currentMovement.x;
                _appliedMovement.z = _currentMovement.z;
            }

            _appliedMovement *= movementSettings.MovementSpeed;

            characterController.Move(_appliedMovement * Time.deltaTime);
        }

        private void HandleRotation()
        {
            Vector3 positionToLookAt;

            positionToLookAt.x = _currentMovement.x;
            positionToLookAt.y = 0;
            positionToLookAt.z = _currentMovement.z;

            if (IsMovementPressed)
            {
                float t = movementSettings.RotationFactorPerFrame * Time.deltaTime;

                Quaternion currentRotation = transform.rotation;
                Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
                Quaternion finalRotation = Quaternion.Slerp(currentRotation, targetRotation, t);

                transform.rotation = finalRotation;
            }
        }

        private void HandleGravity()
        {
            if (IsGrounded)
            {
                if (_isAtJump)
                {
                    _isAtJump = false;

                    Grounded?.Invoke();
                }

                _currentMovement.y = movementSettings.GroundGravity;
                _appliedMovement.y = movementSettings.GroundGravity;
            }
            else if (IsFalling)
            {
                float gravity = jumpSettings.JumpProperties.Gravity;
                float previousVelocityY = _currentMovement.y;

                _currentMovement.y = previousVelocityY + (gravity * movementSettings.FallMultiplier * Time.deltaTime);
                _appliedMovement.y = (previousVelocityY + _currentMovement.y) * 0.5f;
            }
            else
            {
                float gravity = jumpSettings.JumpProperties.Gravity;
                float previousVelocityY = _currentMovement.y;

                _currentMovement.y = previousVelocityY + (gravity * Time.deltaTime);
                _appliedMovement.y = (previousVelocityY + _currentMovement.y) * 0.5f;
            }
        }

        private void HandleJump()
        {
            if (!IsJumping && IsGrounded && IsJumpPressed)
            {
                int maxJumpVarieties = jumpSettings.MaxJumpVarieties;
                float velocity = jumpSettings.JumpProperties.Velocity;

                IsJumping = true;
                _isAtJump = true;

                _jumpIndex = Random.Range(0, maxJumpVarieties);

                Jump?.Invoke(_jumpIndex);

                _currentMovement.y = velocity;
                _appliedMovement.y = velocity;
            }
            else if (!IsJumpPressed && IsGrounded && IsJumping)
            {
                IsJumping = false;
            }
        }

        private void StartUpdate()
        {
            _updateDisposable ??= Observable
                .EveryUpdate(UnityFrameProvider.Update)
                .Subscribe(_ =>
                {
                    HandleRotation();
                    UpdateMovement();
                    HandleGravity();
                    HandleJump();
                });
        }

        private void StopUpdate()
        {
            _updateDisposable?.Dispose();
            _updateDisposable = null;
        }

        private void AddListeners()
        {
            _actorInput.ActorControls.Move.started += OnMovementInput;
            _actorInput.ActorControls.Move.canceled += OnMovementInput;
            _actorInput.ActorControls.Move.performed += OnMovementInput;

            _actorInput.ActorControls.Run.started += OnRun;
            _actorInput.ActorControls.Run.canceled += OnRun;

            _actorInput.ActorControls.Jump.started += OnJump;
            _actorInput.ActorControls.Jump.canceled += OnJump;
        }

        private void RemoveListeners()
        {
            _actorInput.ActorControls.Move.started -= OnMovementInput;
            _actorInput.ActorControls.Move.canceled -= OnMovementInput;
            _actorInput.ActorControls.Move.performed -= OnMovementInput;

            _actorInput.ActorControls.Run.started -= OnRun;
            _actorInput.ActorControls.Run.canceled -= OnRun;

            _actorInput.ActorControls.Jump.started -= OnJump;
            _actorInput.ActorControls.Jump.canceled -= OnJump;
        }

        #endregion
    }
}