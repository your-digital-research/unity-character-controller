using UnityEngine;
using UnityEngine.InputSystem;

namespace Core.Gameplay.Character
{
    [RequireComponent(typeof(CustomCharacterController))]
    public class CharacterStateMachine : MonoBehaviour
    {
        #region SERIALIZED_VARIABLES

        [Header("References")]
        [SerializeField] private CharacterController characterController;
        [SerializeField] private Animator characterAnimator;
        [SerializeField] private MovementSettings movementSettings;
        [SerializeField] private JumpSettings jumpSettings;

        #endregion

        #region PRIVATE_VARIABLES

        private static readonly int _isWalkingHash = Animator.StringToHash("IsWalking");
        private static readonly int _isRunningHash = Animator.StringToHash("IsRunning");
        private static readonly int _isJumpingHash = Animator.StringToHash("IsJumping");
        private static readonly int _isFallingHash = Animator.StringToHash("IsFalling");
        private static readonly int _jumpIndexHash = Animator.StringToHash("JumpIndex");

        private CharacterStateFactory _stateFactory;
        private CharacterInput _characterInput;

        private Vector3 _appliedMovement;
        private Vector3 _currentMovement;
        private Vector3 _currentRunMovement;
        private Vector2 _currentMovementInput;

        #endregion

        #region PROPERTIES

        public MovementSettings MovementSettings => movementSettings;
        public JumpSettings JumpSettings => jumpSettings;
        public Animator Animator => characterAnimator;

        public CharacterState RootState { get; set; }
        public CharacterState SubState { get; set; }
        public CharacterBaseState CurrentState { get; set; }

        public bool IsFalling { get; set; }
        public bool IsJumping { get; set; }
        public bool IsRunPressed { get; private set; }
        public bool IsJumpPressed { get; private set; }
        public bool IsMovementPressed { get; private set; }
        public bool RequireNewJumpPress { get; set; }
        public bool IsGrounded => characterController.isGrounded;

        public int JumpIndex { get; set; }
        public int IsWalkingHash => _isWalkingHash;
        public int IsRunningHash => _isRunningHash;
        public int IsJumpingHash => _isJumpingHash;
        public int IsFallingHash => _isFallingHash;
        public int JumpIndexHash => _jumpIndexHash;

        public float CurrentMovementY
        {
            get => _currentMovement.y;
            set => _currentMovement.y = value;
        }

        public float AppliedMovementX
        {
            get => _appliedMovement.x;
            set => _appliedMovement.x = value;
        }

        public float AppliedMovementY
        {
            get => _appliedMovement.y;
            set => _appliedMovement.y = value;
        }

        public float AppliedMovementZ
        {
            get => _appliedMovement.z;
            set => _appliedMovement.z = value;
        }

        public Vector2 Input => _currentMovementInput;

        #endregion

        #region MONO

        private void Awake()
        {
            Init();
        }

        private void Start()
        {
            UpdateMovement();
        }

        private void OnEnable()
        {
            ToggleCharacterControls(true);
        }

        private void Update()
        {
            HandleRotation();
            UpdateMovement();

            CurrentState.UpdateStates();
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
            RequireNewJumpPress = false;
        }

        private void Init()
        {
            InitFactory();
            InitState();
            InitInput();
        }

        private void InitFactory()
        {
            _stateFactory = new CharacterStateFactory(this);
        }

        private void InitState()
        {
            CurrentState = _stateFactory.Grounded;

            RootState = CharacterState.Grounded;
            SubState = CharacterState.Idle;

            CurrentState.EnterState();
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
            characterController.Move(_appliedMovement * Time.deltaTime);
        }

        private void HandleRotation()
        {
            Vector3 positionToLookAt;

            positionToLookAt.x = _currentMovement.x;
            positionToLookAt.y = 0;
            positionToLookAt.z = _currentMovement.z;

            if (!IsMovementPressed) return;

            float t = movementSettings.RotationFactorPerFrame * Time.deltaTime;

            Quaternion currentRotation = transform.rotation;
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            Quaternion finalRotation = Quaternion.Slerp(currentRotation, targetRotation, t);

            transform.rotation = finalRotation;
        }

        #endregion
    }
}