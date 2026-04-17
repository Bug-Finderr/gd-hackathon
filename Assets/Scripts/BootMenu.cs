using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

namespace WillExe {
    public class BootMenu : MonoBehaviour {
        public TMP_Dropdown difficultyDropdown; // legacy, can be null
        public Button bootButton;
        public TMP_Text tagline;

        [Header("Difficulty pill buttons (preferred over dropdown)")]
        public Button easyButton;
        public Button normalButton;
        public Button hardButton;
        public Image easyPill;
        public Image normalPill;
        public Image hardPill;
        public TMPro.TMP_Text easyText;
        public TMPro.TMP_Text normalText;
        public TMPro.TMP_Text hardText;

        Difficulty selected = Difficulty.Normal;

        void Start() {
            if (difficultyDropdown != null) {
                difficultyDropdown.ClearOptions();
                difficultyDropdown.AddOptions(new System.Collections.Generic.List<string> { "EASY", "NORMAL", "HARD" });
                difficultyDropdown.value = 1;
                difficultyDropdown.onValueChanged.AddListener(v => selected = (Difficulty)v);
            }
            if (easyButton != null) easyButton.onClick.AddListener(() => Select(Difficulty.Easy));
            if (normalButton != null) normalButton.onClick.AddListener(() => Select(Difficulty.Normal));
            if (hardButton != null) hardButton.onClick.AddListener(() => Select(Difficulty.Hard));
            if (bootButton != null) bootButton.onClick.AddListener(Boot);
            Refresh();
        }

        void Select(Difficulty d) {
            selected = d;
            Refresh();
        }

        void Refresh() {
            SetRadio(easyText, "Easy  -  No time limit", selected == Difficulty.Easy);
            SetRadio(normalText, "Normal  -  15 min", selected == Difficulty.Normal);
            SetRadio(hardText, "Hard  -  8 min + penalty", selected == Difficulty.Hard);
        }

        static void SetRadio(TMPro.TMP_Text t, string label, bool on) {
            if (t == null) return;
            string glyph = on ? "\u25CF" : "\u25CB";
            t.text = on ? $"<b>{glyph}  {label}</b>" : $"{glyph}  {label}";
            t.color = on ? Color.white : new Color(0.75f, 0.75f, 0.75f);
        }

        void Boot() {
            GameManager.SelectedDifficulty = selected;
            AudioOneShot.Play(AudioOneShot.Sfx.DialUp);
            SceneManager.LoadScene("Desktop");
        }
    }
}
