using UnityEngine;

namespace Gameplay
{
    public abstract class ActorBaseState
    {
        #region PROTECTED_VARIABLES

        protected readonly ActorStateMachine Context;
        protected readonly ActorStateFactory StateFactory;

        protected bool IsRootState;

        #endregion

        #region PRIVATE_VARIABLES

        private ActorBaseState _currentSubState;
        private ActorBaseState _currentSuperState;

        #endregion

        #region CONSTRUCTOR

        protected ActorBaseState(ActorStateMachine currentContext, ActorStateFactory stateFactory)
        {
            Context = currentContext;
            StateFactory = stateFactory;
        }

        #endregion

        #region PROPERTIES

        protected ActorState Type { get; set; } = ActorState.Unknown;

        #endregion

        #region ABSTRACT_FUNCTIONS

        public abstract void CheckSwitchStates();
        public abstract void InitializeSubStates();

        #endregion

        #region VIRTUAL_FUNCTIONS

        public virtual void EnterState()
        {
            // Debug.Log("Enter - > " + Type);
        }

        public virtual void UpdateState()
        {
            // Debug.Log("Update - > " + Type);
        }

        public virtual void ExitState()
        {
            // Debug.Log("Exit - > " + Type);
        }

        #endregion

        #region PUBLIC_FUNCTIONS

        public void UpdateStates()
        {
            UpdateState();

            if (_currentSubState == null) return;

            _currentSubState.UpdateStates();
        }

        public void ExitStates()
        {
            ExitState();

            if (_currentSubState == null) return;

            _currentSubState.ExitStates();
        }

        #endregion

        #region PROTECTED_FUNCTIONS

        protected void SwitchState(ActorBaseState state)
        {
            ExitState();

            state.EnterState();

            if (IsRootState)
            {
                Context.CurrentState = state;
                Context.RootState = state.Type;
            }
            else if (_currentSuperState != null)
            {
                Context.SubState = state.Type;

                _currentSuperState.SetSubState(state);
            }
        }

        protected void SetSubState(ActorBaseState state)
        {
            _currentSubState = state;

            state.SetSuperState(this);
        }

        protected void SetSuperState(ActorBaseState state)
        {
            _currentSuperState = state;
        }

        #endregion
    }
}