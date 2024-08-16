using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
    public abstract class StateMachine<T> : MonoBehaviour where T : Enum
    {
        #region PROTECTED_VARIABLES

        protected bool IsTransitioning;

        protected BaseState<T> CurrentState;
        protected Dictionary<T, BaseState<T>> States = new();

        #endregion

        #region MONO

        private void Start()
        {
            CurrentState.EnterState();
        }

        private void Update()
        {
            TryToUpdate();
        }

        private void OnTriggerEnter(Collider other)
        {
            CurrentState.OnTriggerEnter(other);
        }

        private void OnTriggerStay(Collider other)
        {
            CurrentState.OnTriggerStay(other);
        }

        private void OnTriggerExit(Collider other)
        {
            CurrentState.OnTriggerExit(other);
        }

        #endregion

        #region PRIVATE_VARIABLES

        private void TryToUpdate()
        {
            T nextStateKey = CurrentState.GetNextState();

            if (!IsTransitioning && nextStateKey.Equals(CurrentState.Key))
            {
                CurrentState.UpdateState();
            }
            else
            {
                TransitionToState(nextStateKey);
            }
        }

        private void TransitionToState(T key)
        {
            IsTransitioning = true;

            CurrentState.ExitState();
            CurrentState = States[key];
            CurrentState.EnterState();

            IsTransitioning = false;
        }

        #endregion
    }
}