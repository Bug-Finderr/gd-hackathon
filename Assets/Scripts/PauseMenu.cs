using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace WillExe {
    public class PauseMenu : MonoBehaviour {
        public Button startButton;
        public GameObject panel;
        public Button scrimButton;
        public Button restartButton;
        public Button mainMenuButton;

        void Start() {
            if (startButton != null) startButton.onClick.AddListener(Toggle);
            if (scrimButton != null) scrimButton.onClick.AddListener(Close);
            if (restartButton != null) restartButton.onClick.AddListener(Restart);
            if (mainMenuButton != null) mainMenuButton.onClick.AddListener(ToMainMenu);
            if (panel != null) panel.SetActive(false);
        }

        public void Toggle() {
            if (panel == null) return;
            panel.SetActive(!panel.activeSelf);
        }

        public void Close() {
            if (panel != null) panel.SetActive(false);
        }

        void Restart() {
            if (GameManager.Instance != null) GameManager.Instance.Restart();
            else SceneManager.LoadScene("Desktop");
        }

        void ToMainMenu() {
            SceneManager.LoadScene("Boot");
        }
    }
}
