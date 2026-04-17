using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace WillExe {
    /// HUD showing fragment progress. Reacts to GameManager events. Wires Win/Lose panels + restart.
    public class WillAssembler : MonoBehaviour {
        [Header("HUD")]
        public TMP_Text fragmentCounter;
        public GameObject hintToast;
        public TMP_Text hintToastText;

        [Header("Panels")]
        public GameObject winPanel;
        public TMP_Text winWillText;
        public GameObject losePanel;

        [Header("Buttons")]
        public Button winRestartButton;
        public Button winBootButton;
        public Button loseRestartButton;

        [Header("Final will text shown on WIN")]
        [TextArea(10, 40)] public string fullWillText;

        void Start() {
            if (winPanel != null) winPanel.SetActive(false);
            if (losePanel != null) losePanel.SetActive(false);
            if (hintToast != null) hintToast.SetActive(false);

            if (GameManager.Instance != null) {
                GameManager.Instance.OnFragmentCollected += ShowFragmentToast;
                GameManager.Instance.OnWin += ShowWin;
                GameManager.Instance.OnLose += ShowLose;
            }

            if (winRestartButton != null) winRestartButton.onClick.AddListener(Restart);
            if (winBootButton != null) winBootButton.onClick.AddListener(GoBoot);
            if (loseRestartButton != null) loseRestartButton.onClick.AddListener(Restart);

            UpdateCounter();
        }

        void OnDestroy() {
            if (GameManager.Instance != null) {
                GameManager.Instance.OnFragmentCollected -= ShowFragmentToast;
                GameManager.Instance.OnWin -= ShowWin;
                GameManager.Instance.OnLose -= ShowLose;
            }
        }

        void UpdateCounter() {
            if (fragmentCounter == null || GameManager.Instance == null) return;
            fragmentCounter.text = $"WILL FRAGMENTS {GameManager.Instance.FragmentCount}/{GameManager.Instance.RequiredFragments}";
        }

        void ShowFragmentToast(string fragmentText) {
            UpdateCounter();
            if (hintToast != null && hintToastText != null) {
                hintToastText.text = "> FRAGMENT RECOVERED";
                hintToast.SetActive(true);
                CancelInvoke(nameof(HideToast));
                Invoke(nameof(HideToast), 3f);
            }
        }

        void HideToast() { if (hintToast != null) hintToast.SetActive(false); }

        void ShowWin() {
            if (winPanel != null) winPanel.SetActive(true);
            if (winWillText != null) winWillText.text = fullWillText;
            AudioOneShot.Play(AudioOneShot.Sfx.Win);
        }

        void ShowLose() {
            if (losePanel != null) losePanel.SetActive(true);
            AudioOneShot.Play(AudioOneShot.Sfx.Lose);
        }

        void Restart() { if (GameManager.Instance != null) GameManager.Instance.Restart(); }
        void GoBoot() { if (GameManager.Instance != null) GameManager.Instance.LoadBoot(); }
    }
}
