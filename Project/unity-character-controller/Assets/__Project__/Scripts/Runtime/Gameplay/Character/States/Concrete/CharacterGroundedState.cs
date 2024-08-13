namespace Gameplay
{
    public class CharacterGroundedState : CharacterBaseState, IRootState
    {
        #region CONSTRUCTOR

        public CharacterGroundedState(CharacterStateMachine currentContext, CharacterStateFactory stateFactory) : base(currentContext, stateFactory)
        {
            Type = CharacterState.Grounded;

            IsRootState = true;
        }

        #endregion

        #region OVERRIDDEN_FUNCTIONS

        public override void EnterState()
        {
            base.EnterState();

            InitializeSubStates();

            HandleGravity();

            Context.Animator.SetBool(Context.IsJumpingHash, false);
            Context.Animator.SetInteger(Context.JumpIndexHash, 0);
        }

        public override void UpdateState()
        {
            base.UpdateState();

            CheckSwitchStates();
        }

        public override void ExitState()
        {
            base.ExitState();
        }

        public override void CheckSwitchStates()
        {
            if (Context.IsJumpPressed && !Context.RequireNewJumpPress)
            {
                SwitchState(StateFactory.Jump);
            }
            else if (!Context.IsGrounded)
            {
                SwitchState(StateFactory.Fall);
            }
        }

        public override void InitializeSubStates()
        {
            switch (Context.IsMovementPressed)
            {
                case false when !Context.IsRunPressed:
                    SetSubState(StateFactory.Idle);
                    break;
                case true when !Context.IsRunPressed:
                    SetSubState(StateFactory.Walk);
                    break;
                default:
                    SetSubState(StateFactory.Run);
                    break;
            }
        }

        #endregion

        #region PUBLIC_FUNCTIONS

        public void HandleGravity()
        {
            // NOTE - Apply air gravity
            // Context.CurrentMovementY = Context.JumpSettings.JumpProperties.Gravity;
            // Context.AppliedMovementY = Context.JumpSettings.JumpProperties.Gravity;

            // NOTE - Apply ground gravity
            Context.CurrentMovementY = Context.MovementSettings.GroundGravity;
            Context.AppliedMovementY = Context.MovementSettings.GroundGravity;
        }

        #endregion
    }
}