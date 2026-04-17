using UnityEngine;

namespace WillExe {
    public enum WindowType { Text, Image, Folder, Password, Video, WillFragment }

    [CreateAssetMenu(fileName = "WindowDef", menuName = "WillExe/Window Definition")]
    public class WindowDef : ScriptableObject {
        [Header("Identity")]
        public string id;
        public string title = "Untitled";
        public WindowType type;
        public Vector2 defaultSize = new Vector2(640, 420);

        [Header("Text / Video")]
        [TextArea(5, 30)] public string textBody;

        [Header("Image / Video")]
        public Sprite imageSprite;
        [TextArea(1, 4)] public string caption;
        public Sprite[] videoFrames;
        public float videoFps = 12f;
        public bool forceRemainingSecondsOnOpen;
        public float onOpenRemainingSeconds = 60f;

        [Header("Folder")]
        public IconDef[] childIcons;

        [Header("Password Gate")]
        public string[] acceptedAnswers;
        public string passwordPrompt = "Enter password:";
        public string unlocksOnSolve;
        public string onSolveMessage = "ACCESS GRANTED";

        [Header("Will Fragment (if this is a reveal)")]
        public bool isWillFragment;
        [TextArea(3, 10)] public string fragmentText;

        [Header("Visibility")]
        public string requiredUnlockId;
    }
}
