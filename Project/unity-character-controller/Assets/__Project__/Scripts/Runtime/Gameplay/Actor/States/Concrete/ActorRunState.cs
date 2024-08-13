namespace Gameplay
{
    public class ActorRunState : ActorBaseState
    {
        #region CONSTRUCTOR

        public ActorRunState(ActorStateMachine currentContext, ActorStateFactory stateFactory) : base(currentContext, stateFactory)
        {
            Type = ActorState.Run;
        }

        #endregion

        #region OVERRIDDEN_FUNCTIONS

        public override void EnterState()
        {
            base.EnterState();

            Context.Animator.SetBool(Context.IsWalkingHash, true);
            Context.Animator.SetBool(Context.IsRunningHash, true);
        }

        public override void UpdateState()
        {
            base.UpdateState();

            Context.AppliedMovementX = Context.RelativeInput.x * Context.MovementSettings.RunMultiplier;
            Context.AppliedMovementZ = Context.RelativeInput.y * Context.MovementSettings.RunMultiplier;

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
                case true when !Context.IsRunPressed:
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