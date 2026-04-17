using UnityEngine;

namespace WillExe {
    [CreateAssetMenu(fileName = "IconDef", menuName = "WillExe/Icon Definition")]
    public class IconDef : ScriptableObject {
        public string id;
        public string label = "New Item";
        public Sprite iconSprite;
        public WindowDef window;

        [Tooltip("If non-empty, this icon only appears after this unlock id is set in GameManager.")]
        public string requiredUnlockId;

        [Tooltip("Desktop position (used only for top-level icons).")]
        public Vector2 desktopPosition;
    }
}
