using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

namespace Gameplay
{
    [RequireComponent(typeof(CustomCharacterController))]
    public class CharacterMovementController : MonoBehaviour
    {
        #region SERIALIZED_VARIABLES

        [Header("References")]
        [SerializeField] private CharacterController characterController;
        [SerializeField] private MovementSettings movementSettings;
        [SerializeField] private JumpSettings jumpSettings;

        #endregion

        #region PRIVATE_VARIABLES

        private CharacterInput _characterInput;

        private Vector3 _appliedMovement;
        private Vector3 _currentMovement;
        private Vector3 _currentRunMovement;
        private Vector2 _currentMovementInput;

        private int _jumpIndex;
        private bool _isAtJump;

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

        private void OnEnable()
        {
            ToggleCharacterControls(true);
        }

        private void Update()
        {
            HandleRotation();
            UpdateMovement();
            HandleGravity();
            HandleJump();
        }

        private void OnDisable()
        {
            ToggleCharacterControls(false);
        }

        private void OnDestroy()
        {
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

        private void InitInput()
        {
            _characterInput = new CharacterInput();

            _characterInput.CharacterControls.Move.started += OnMovementInput;
            _characterInput.CharacterControls.Move.canceled += OnMovementInput;
            _characterInput.CharacterControls.Move.performed += OnMovementInput;

            _characterInput.CharacterControls.Run.started += OnRun;
            _characterInput.CharacterControls.Run.canceled += OnRun;

            _characterInput.CharacterControls.Jump.started += OnJump;
            _characterInput.CharacterControls.Jump.canceled += OnJump;
        }

        private void ResetInput()
        {
            _characterInput.CharacterControls.Move.started -= OnMovementInput;
            _characterInput.CharacterControls.Move.canceled -= OnMovementInput;
            _characterInput.CharacterControls.Move.performed -= OnMovementInput;

            _characterInput.CharacterControls.Run.started -= OnRun;
            _characterInput.CharacterControls.Run.canceled -= OnRun;

            _characterInput.CharacterControls.Jump.started -= OnJump;
            _characterInput.CharacterControls.Jump.canceled -= OnJump;

            _characterInput = null;
        }

        private void ToggleCharacterControls(bool value)
        {
            switch (value)
            {
                case true:
                    _characterInput.CharacterControls.Enable();
                    break;
                case false:
                    _characterInput.CharacterControls.Disable();
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

        #endregion
    }
}