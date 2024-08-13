namespace Gameplay
{
    public class ActorIdleState : ActorBaseState
    {
        #region CONSTRUCTOR

        public ActorIdleState(ActorStateMachine currentContext, ActorStateFactory stateFactory) : base(currentContext, stateFactory)
        {
            Type = ActorState.Idle;
        }

        #endregion

        #region OVERRIDDEN_FUNCTIONS

        public override void EnterState()
        {
            base.EnterState();

            Context.Animator.SetBool(Context.IsWalkingHash, false);
            Context.Animator.SetBool(Context.IsRunningHash, false);

            Context.AppliedMovementX = 0;
            Context.AppliedMovementZ = 0;
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
            switch (Context.IsMovementPressed)
            {
                case true when Context.IsRunPressed:
                    SwitchState(StateFactory.Run);
                    break;
                case true:
                    SwitchState(StateFactory.Walk);
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