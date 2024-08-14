using UnityEngine;

namespace Settings
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "Scriptable Objects/Game/Settings")]
    public class GameSettings : ScriptableObject
    {
        #region SERIALIZED_VARIABLES

        [Header("Settings")]
        public bool MultiTouch;
        public int TargetFramerate;
        public int TweenersCapacity;
        public int SequencesCapacity;

        #endregion
    }
}