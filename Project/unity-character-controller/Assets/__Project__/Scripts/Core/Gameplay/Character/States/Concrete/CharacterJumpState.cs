using UnityEngine;

namespace Core.Gameplay.Character
{
    public class CharacterJumpState : CharacterBaseState, IRootState
    {
        #region CONSTRUCTOR

        public CharacterJumpState(CharacterStateMachine currentContext, CharacterStateFactory stateFactory) : base(currentContext, stateFactory)
        {
            Type = CharacterState.Jump;

            IsRootState = true;
        }

        #endregion

        #region OVERRIDDEN_FUNCTIONS

        public override void EnterState()
        {
            base.EnterState();

            InitializeSubStates();

            HandleJump();
        }

        public override void UpdateState()
        {
            base.UpdateState();

            HandleGravity();

            CheckSwitchStates();
        }

        public override void ExitState()
        {
            base.ExitState();

            if (Context.IsJumpPressed) Context.RequireNewJumpPress = true;

            Context.IsJumping = false;
        }

        public override void CheckSwitchStates()
        {
            if (!Context.IsGrounded) return;

            SwitchState(StateFactory.Grounded);
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

        #region PRIVATE_FUNCTIONS

        public void HandleGravity()
        {
            float gravity = Context.JumpSettings.JumpProperties.Gravity;
            float previousVelocityY = Context.CurrentMovementY;
            float additionalVelocity = Context.IsFalling
                ? gravity * Context.MovementSettings.FallMultiplier * Time.deltaTime
                : gravity * Time.deltaTime;

            Context.AppliedMovementY = (previousVelocityY + Context.CurrentMovementY) * 0.5f;
            Context.CurrentMovementY = previousVelocityY + additionalVelocity;
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        private void HandleJump()
        {
            int maxJumpVarieties = Context.JumpSettings.MaxJumpVarieties;
            float velocity = Context.JumpSettings.JumpProperties.Velocity;

            Context.IsJumping = true;
            Context.JumpIndex = Random.Range(0, maxJumpVarieties);

            Context.Animator.SetBool(Context.IsJumpingHash, true);
            Context.Animator.SetInteger(Context.JumpIndexHash, Context.JumpIndex);

            Context.CurrentMovementY = velocity;
            Context.AppliedMovementY = velocity;
        }

        #endregion
    }
}