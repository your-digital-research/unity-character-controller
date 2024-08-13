namespace Gameplay
{
    public class ActorWalkState : ActorBaseState
    {
        #region CONSTRUCTOR

        public ActorWalkState(ActorStateMachine currentContext, ActorStateFactory stateFactory) : base(currentContext, stateFactory)
        {
            Type = ActorState.Walk;
        }

        #endregion

        #region OVERRIDDEN_FUNCTIONS

        public override void EnterState()
        {
            base.EnterState();

            Context.Animator.SetBool(Context.IsWalkingHash, true);
            Context.Animator.SetBool(Context.IsRunningHash, false);
        }

        public override void UpdateState()
        {
            base.UpdateState();

            Context.AppliedMovementX = Context.RelativeInput.x;
            Context.AppliedMovementZ = Context.RelativeInput.y;

            CheckSwitchStates();
        }

        public override void ExitState()
        {
            base.ExitState();
        }

        public override void CheckSwitchStates()
        {
            switch (Context.IsMovementPressed)
            {
                case false:
                    SwitchState(StateFactory.Idle);
                    break;
                case true when Context.IsRunPressed:
                    SwitchState(StateFactory.Run);
                    break;
            }
        }

        public override void InitializeSubStates()
        {
            //
        }

        #endregion
    }
}