using UnityEngine;

namespace Core.Gameplay.Character
{
    public abstract class CharacterBaseState
    {
        #region PROTECTED_VARIABLES

        protected readonly CharacterStateMachine Context;
        protected readonly CharacterStateFactory StateFactory;

        protected bool IsRootState;

        #endregion

        #region PRIVATE_VARIABLES

        private CharacterBaseState _currentSubState;
        private CharacterBaseState _currentSuperState;

        #endregion

        #region CONSTRUCTOR

        protected CharacterBaseState(CharacterStateMachine currentContext, CharacterStateFactory stateFactory)
        {
            Context = currentContext;
            StateFactory = stateFactory;
        }

        #endregion

        #region PROPERTIES

        protected CharacterState Type { get; set; } = CharacterState.Unknown;

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

        protected void SwitchState(CharacterBaseState state)
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

        protected void SetSubState(CharacterBaseState state)
        {
            _currentSubState = state;

            state.SetSuperState(this);
        }

        protected void SetSuperState(CharacterBaseState state)
        {
            _currentSuperState = state;
        }

        #endregion
    }
}