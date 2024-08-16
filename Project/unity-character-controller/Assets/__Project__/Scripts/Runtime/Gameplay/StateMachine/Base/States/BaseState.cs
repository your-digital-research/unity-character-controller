using System;
using UnityEngine;

namespace Gameplay
{
    public abstract class BaseState<T> where T : Enum
    {
        #region CONSTRUCTORS

        public BaseState(T key)
        {
            Key = key;
        }

        #endregion

        #region ABSTRACT_FUNCTIONS

        public abstract void EnterState();
        public abstract void ExitState();
        public abstract void UpdateState();
        public abstract T GetNextState();
        public abstract void OnTriggerEnter(Collider other);
        public abstract void OnTriggerStay(Collider other);
        public abstract void OnTriggerExit(Collider other);

        #endregion

        #region PROPERTIES

        public T Key { get; protected set; }

        #endregion
    }
}