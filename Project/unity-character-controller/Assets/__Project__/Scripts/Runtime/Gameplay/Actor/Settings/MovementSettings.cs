using UnityEngine;

namespace Gameplay
{
    [CreateAssetMenu(fileName = "MovementSettings", menuName = "Scriptable Objects/Actor/Settings/Movement")]
    public class MovementSettings : ScriptableObject
    {
        #region SERIALIZED_VARIABLES

        [Header("Settings")]
        public float GroundGravity;
        public float MovementSpeed;
        public float RunMultiplier;
        public float FallMultiplier;
        public float RotationFactorPerFrame;

        #endregion
    }
}