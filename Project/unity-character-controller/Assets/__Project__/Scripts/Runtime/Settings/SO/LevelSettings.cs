using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Settings
{
    [CreateAssetMenu(fileName = "LevelSettings", menuName = "Scriptable Objects/Level/Settings")]
    public class LevelSettings : ScriptableObject
    {
        #region SERIALIZED_VARIABLES

        [Header("References")]
        public AssetReference Level;

        #endregion
    }
}