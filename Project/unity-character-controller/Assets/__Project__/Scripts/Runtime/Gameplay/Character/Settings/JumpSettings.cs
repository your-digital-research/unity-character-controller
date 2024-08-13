using System;
using UnityEngine;

namespace Gameplay
{
    [Serializable]
    public class JumpProperties
    {
        #region PUBLIC_VARIABLES

        public float JumpTime;
        public float MaxJumpHeight;

        #endregion

        #region PROPERTIES

        public float Gravity => (-2 * MaxJumpHeight) / Mathf.Pow(TimeToApex, 2);
        public float Velocity => (2 * MaxJumpHeight) / TimeToApex;
        public float TimeToApex => JumpTime / 2;

        #endregion
    }

    [CreateAssetMenu(fileName = "JumpSettings", menuName = "Scriptable Objects/Character/Settings/Jump")]
    public class JumpSettings : ScriptableObject
    {
        #region SERIALIZED_VARIABLES

        [Header("Settings")]
        public int MaxJumpVarieties;
        public JumpProperties JumpProperties;

        #endregion
    }
}