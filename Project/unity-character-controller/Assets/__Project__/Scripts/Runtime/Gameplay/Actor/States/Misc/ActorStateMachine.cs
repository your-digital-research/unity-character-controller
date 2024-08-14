using System;
using R3;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Gameplay
{
    [RequireComponent(typeof(ActorController))]
    public class ActorStateMachine : MonoBehaviour
    {
        #region SERIALIZED_VARIABLES

        [Header("References")]
        [SerializeField] private CharacterController characterController;
        [SerializeField] private Animator actorAnimator;
        [SerializeField] private MovementSettings movementSettings;
        [SerializeField] private JumpSettings jumpSettings;

        #endregion

        #region PRIVATE_VARIABLES

        private static readonly int _isWalkingHash = Animator.StringToHash("IsWalking");
        private static readonly int _isRunningHash = Animator.StringToHash("IsRunning");
        private static readonly int _isJumpingHash = Animator.StringToHash("IsJumping");
        private static readonly int _isFallingHash = Animator.StringToHash("IsFalling");
        private static readonly int _jumpIndexHash = Animator.StringToHash("JumpIndex");

        private ActorStateFactory _stateFactory;
        private ActorInput _actorInput;

        private Vector3 _appliedMovement;
        private Vector3 _currentMovement;
        private Vector3 _currentRunMovement;
        private Vector2 _currentMovementInput;
        private Vector2 _cameraRelativeInput;

        private IDisposable _updateDisposable;

        #endregion

        #region PROPERTIES

        public MovementSettings MovementSettings => movementSettings;
        public JumpSettings JumpSettings => jumpSettings;
        public Animator Animator => actorAnimator;

        public ActorState RootState { get; set; }
        public ActorState SubState { get; set; }
        public ActorBaseState CurrentState { get; set; }

        public CameraController CameraController { get; set; }

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
        public Vector2 RelativeInput => _cameraRelativeInput;

        #endregion

        #region MONO

        private void Awake()
        {
            InitInput();
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

        #region PUBLIC_FUNCTIONS

        public void Init()
        {
            InitFactory();
            InitState();

            AddListeners();

            StartUpdate();
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        private Vector2 GetCameraRelativeInput()
        {
            Transform cameraTransform = CameraController.Current.transform;
            Transform targetTransform = CameraController.Current.VirtualCamera.Follow;

            Vector3 targetDirection = (targetTransform.position - cameraTransform.position).normalized;

            Vector3 forward = Vector3.ProjectOnPlane(targetDirection, Vector3.up).normalized;
            Vector3 right = Vector3.Cross(Vector3.up, forward).normalized;

            Vector3 cameraRelativeMovement = forward * _currentMovementInput.y + right * _currentMovementInput.x;

            return new Vector2(cameraRelativeMovement.x, cameraRelativeMovement.z);
        }

        private void OnMovementInput(InputAction.CallbackContext obj)
        {
            _currentMovementInput = obj.ReadValue<Vector2>();
        }

        private void UpdateInput()
        {
            _cameraRelativeInput = GetCameraRelativeInput();

            _currentMovement.x = _cameraRelativeInput.x;
            _currentMovement.z = _cameraRelativeInput.y;

            _currentRunMovement.x = _currentMovementInput.x * movementSettings.RunMultiplier;
            _currentRunMovement.z = _currentMovementInput.y * movementSettings.RunMultiplier;

            IsMovementPressed = _cameraRelativeInput.x != 0 || _cameraRelativeInput.y != 0;
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

        private void InitFactory()
        {
            _stateFactory = new ActorStateFactory(this);
        }

        private void InitState()
        {
            CurrentState = _stateFactory.Grounded;

            RootState = ActorState.Grounded;
            SubState = ActorState.Idle;

            CurrentState.EnterState();
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

        private void HandleMovement()
        {
            Vector3 motion = _appliedMovement * (movementSettings.MovementSpeed * Time.deltaTime);

            characterController.Move(motion);
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

        private void StartUpdate()
        {
            _updateDisposable ??= Observable
                .EveryUpdate(UnityFrameProvider.Update)
                .Subscribe(_ =>
                {
                    UpdateInput();

                    HandleRotation();
                    HandleMovement();

                    CurrentState.UpdateStates();
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