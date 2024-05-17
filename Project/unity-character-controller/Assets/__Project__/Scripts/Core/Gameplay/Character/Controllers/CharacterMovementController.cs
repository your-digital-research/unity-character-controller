using UnityEngine;
using UnityEngine.InputSystem;

namespace Core.Gameplay.Character
{
    [RequireComponent(typeof(CustomCharacterController))]
    public class CharacterMovementController : MonoBehaviour
    {
        #region SERIALIZED_VARIABLES

        [Header("References")]
        [SerializeField] private CharacterController characterController;

        [Header("Settings")]
        [SerializeField] private float airGravity;
        [SerializeField] private float groundGravity;
        [SerializeField] private float movementSpeed;
        [SerializeField] private float runMultiplier;
        [SerializeField] private float rotationFactorPerFrame;

        #endregion

        #region PRIVATE_VARIABLES

        private CharacterInput _characterInput;

        private Vector3 _currentMovement;
        private Vector3 _currentRunMovement;
        private Vector2 _currentMovementInput;

        #endregion

        #region PROPERTIES

        public bool IsMovementPressed { get; private set; }
        public bool IsRunPressed { get; private set; }

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
            UpdateMovement();
            HandleRotation();
            HandleGravity();
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
            // Debug.Log("OnMovementInput() : " + obj.ReadValue<Vector2>());

            _currentMovementInput = obj.ReadValue<Vector2>();

            _currentMovement.x = _currentMovementInput.x;
            _currentMovement.z = _currentMovementInput.y;

            _currentRunMovement.x = _currentMovementInput.x * runMultiplier;
            _currentRunMovement.z = _currentMovementInput.y * runMultiplier;

            IsMovementPressed = _currentMovementInput.x != 0 || _currentMovementInput.y != 0;
        }

        private void OnRun(InputAction.CallbackContext obj)
        {
            // Debug.Log("OnRun() : " + obj.ReadValueAsButton());

            IsRunPressed = obj.ReadValueAsButton();
        }

        private void InitInput()
        {
            _characterInput = new CharacterInput();

            _characterInput.CharacterControls.Move.started += OnMovementInput;
            _characterInput.CharacterControls.Move.canceled += OnMovementInput;
            _characterInput.CharacterControls.Move.performed += OnMovementInput;

            _characterInput.CharacterControls.Run.started += OnRun;
            _characterInput.CharacterControls.Run.canceled += OnRun;
        }

        private void ResetInput()
        {
            _characterInput.CharacterControls.Move.started -= OnMovementInput;
            _characterInput.CharacterControls.Move.canceled -= OnMovementInput;
            _characterInput.CharacterControls.Move.performed -= OnMovementInput;

            _characterInput.CharacterControls.Run.started -= OnRun;
            _characterInput.CharacterControls.Run.canceled -= OnRun;

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
            Vector3 moveVector = IsRunPressed ? _currentRunMovement : _currentMovement;

            moveVector *= movementSpeed;

            characterController.Move(moveVector * Time.deltaTime);
        }

        private void HandleRotation()
        {
            Vector3 positionToLookAt;

            positionToLookAt.x = _currentMovement.x;
            positionToLookAt.y = 0;
            positionToLookAt.z = _currentMovement.z;

            if (IsMovementPressed)
            {
                float t = rotationFactorPerFrame * Time.deltaTime;

                Quaternion currentRotation = transform.rotation;
                Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
                Quaternion finalRotation = Quaternion.Slerp(currentRotation, targetRotation, t);

                transform.rotation = finalRotation;
            }
        }

        private void HandleGravity()
        {
            if (characterController.isGrounded)
            {
                _currentMovement.y = -groundGravity;
                _currentRunMovement.y = -groundGravity;
            }
            else
            {
                _currentMovement.y = -airGravity;
                _currentRunMovement.y = -airGravity;
            }
        }

        #endregion
    }
}