using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace WillExe {
    public class CorruptionTimer : MonoBehaviour {
        public Image fillBar;
        public TMP_Text label;
        public float fullFillWidth = 520f;
        public GameObject deadManPanel;
        public TMP_Text deadManTimerLabel;

        bool deadManArmed;

        float total;
        float remaining;
        bool running;

        void Start() {
            var m = GameManager.Instance != null ? GameManager.Instance.manifest : null;
            total = ResolveFor(GameManager.SelectedDifficulty, m);
            remaining = total;
            if (GameManager.Instance != null) {
                GameManager.Instance.OnWin += Stop;
                GameManager.Instance.OnLose += Stop;
            }
            if (total <= 0f) {
                // Easy mode: no timer. Hide HUD but keep this component alive
                // so ForceRemaining (dead-man-switch) still works.
                if (fillBar != null) fillBar.transform.parent.gameObject.SetActive(false);
                running = false;
                return;
            }
            running = true;
        }

        void OnDestroy() {
            if (GameManager.Instance != null) {
                GameManager.Instance.OnWin -= Stop;
                GameManager.Instance.OnLose -= Stop;
            }
        }

        void Update() {
            if (!running) return;
            remaining -= Time.deltaTime;
            float ratio = total > 0 ? Mathf.Clamp01(remaining / total) : 0f;
            if (fillBar != null) {
                var rt = fillBar.rectTransform;
                rt.offsetMax = new Vector2(fullFillWidth * ratio, rt.offsetMax.y);
                fillBar.color = Color.Lerp(new Color(0.85f, 0.15f, 0.15f), new Color(0.3f, 0.8f, 0.3f), ratio);
            }
            if (label != null) label.text = $"DRIVE HEALTH {Mathf.CeilToInt(ratio * 100f)}%";
            if (deadManArmed && deadManTimerLabel != null) {
                int secs = Mathf.CeilToInt(Mathf.Max(0f, remaining));
                deadManTimerLabel.text = $"DEAD MAN SWITCH :: {secs / 60}:{secs % 60:00}";
            }
            if (remaining <= 0f) {
                running = false;
                if (GameManager.Instance != null) GameManager.Instance.TriggerLose();
            }
        }

        public void ApplyPenalty(float seconds) {
            remaining -= seconds;
            AudioOneShot.Play(AudioOneShot.Sfx.Glitch);
        }

        public void ForceRemaining(float seconds) {
            if (!running) {
                running = true;
                if (total <= 0f) total = seconds;
                gameObject.SetActive(true);
            }
            // clamp down only (never grants extra time), preserves the fill-bar story
            remaining = Mathf.Min(remaining > 0 ? remaining : seconds, seconds);
            deadManArmed = true;
            if (deadManPanel != null) deadManPanel.SetActive(true);
            AudioOneShot.Play(AudioOneShot.Sfx.Glitch);
        }

        void Stop() {
            running = false;
            deadManArmed = false;
            if (deadManPanel != null) deadManPanel.SetActive(false);
        }

        static float ResolveFor(Difficulty d, DesktopManifest m) {
            if (m == null) return d == Difficulty.Hard ? 480f : (d == Difficulty.Normal ? 900f : 0f);
            switch (d) {
                case Difficulty.Easy: return m.easyTimerSeconds;
                case Difficulty.Hard: return m.hardTimerSeconds;
                default: return m.normalTimerSeconds;
            }
        }
    }
}
