using UnityEngine;

namespace Settings
{
    [CreateAssetMenu(fileName = "TransitionSettings", menuName = "Scriptable Objects/Transition/Settings")]
    public class TransitionSettings : ScriptableObject
    {
        #region SERIALIZED_VARIABLES

        [Header("View Settings")]
        [Range(0, 5)] public float ViewShowDelay;
        [Range(0, 5)] public float ViewHideDelay;
        [Range(0, 5)] public float TransitionDuration;

        [Header("Loading Icon Settings")]
        [Range(0, 5)] public float IconShowDelay;
        [Range(0, 5)] public float IconHideDelay;
        [Range(0, 5)] public float IconRotationDuration;
        [Range(0, 5)] public float IconShowHideDuration;

        #endregion
    }
}