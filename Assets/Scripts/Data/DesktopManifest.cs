using UnityEngine;

namespace WillExe {
    [CreateAssetMenu(fileName = "DesktopManifest", menuName = "WillExe/Desktop Manifest")]
    public class DesktopManifest : ScriptableObject {
        [Header("Top-level desktop icons")]
        public IconDef[] topLevelIcons;

        [Header("Difficulty - corruption timer")]
        public float easyTimerSeconds = 0f;
        public float normalTimerSeconds = 900f;
        public float hardTimerSeconds = 480f;

        [Header("Win condition")]
        public int requiredFragments = 4;
    }
}
