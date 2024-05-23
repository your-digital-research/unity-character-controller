using UnityEngine;

namespace Core.Gameplay.Character
{
    public class CharacterFallState : CharacterBaseState, IRootState
    {
        #region CONSTRUCTOR

        public CharacterFallState(CharacterStateMachine currentContext, CharacterStateFactory stateFactory) : base(currentContext, stateFactory)
        {
            Type = CharacterState.Fall;

            IsRootState = true;
        }

        #endregion

        #region OVERRIDDEN_FUNCTIONS

        public override void EnterState()
        {
            base.EnterState();

            InitializeSubStates();

            Context.IsFalling = true;

            Context.Animator.SetBool(Context.IsFallingHash, true);
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

            Context.IsFalling = false;

            Context.Animator.SetBool(Context.IsFallingHash, false);
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

        #region PUBLIC_FUNCTIONS

        public void HandleGravity()
        {
            float previousVelocityY = Context.CurrentMovementY;

            Context.CurrentMovementY += Context.JumpSettings.JumpProperties.Gravity * Time.deltaTime;
            Context.AppliedMovementY = (previousVelocityY + Context.CurrentMovementY) * 0.5f;
        }

        #endregion
    }
}