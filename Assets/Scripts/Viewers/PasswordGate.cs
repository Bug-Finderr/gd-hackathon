using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace WillExe.Viewers {
    public class PasswordGate : MonoBehaviour {
        public TMP_Text promptLabel;
        public TMP_InputField input;
        public Button submit;
        public TMP_Text feedback;

        WindowDef def;

        public void Populate(WindowDef d) {
            def = d;
            if (promptLabel != null) promptLabel.text = d.passwordPrompt;
            if (submit != null) submit.onClick.AddListener(TrySubmit);
            if (input != null) input.onSubmit.AddListener(_ => TrySubmit());
            if (feedback != null) feedback.text = "";
            if (input != null) input.Select();
            if (d.forceRemainingSecondsOnOpen) {
                float seconds = d.onOpenRemainingSeconds;
                switch (GameManager.SelectedDifficulty) {
                    case Difficulty.Easy: seconds = Mathf.Max(seconds, 90f); break;
                    case Difficulty.Hard: seconds = Mathf.Max(30f, seconds * 0.5f); break;
                }
                var timer = FindObjectOfType<CorruptionTimer>();
                if (timer != null) timer.ForceRemaining(seconds);
            }
        }

        void TrySubmit() {
            if (def == null || input == null) return;
            string attempt = (input.text ?? "").Trim();
            if (string.IsNullOrEmpty(attempt)) return;

            foreach (var accepted in def.acceptedAnswers) {
                if (string.Equals(attempt.Replace(" ", ""), (accepted ?? "").Replace(" ", ""),
                    System.StringComparison.OrdinalIgnoreCase)) {
                    OnSuccess();
                    return;
                }
            }
            OnFailure();
        }

        void OnSuccess() {
            if (feedback != null) {
                feedback.text = def.onSolveMessage;
                feedback.color = new Color(0.15f, 0.65f, 0.25f);
            }
            AudioOneShot.Play(AudioOneShot.Sfx.Unlock);
            if (GameManager.Instance != null && !string.IsNullOrEmpty(def.unlocksOnSolve))
                GameManager.Instance.Unlock(def.unlocksOnSolve);
            if (def.isWillFragment && GameManager.Instance != null)
                GameManager.Instance.CollectFragment(def.id, def.fragmentText);
            if (submit != null) submit.interactable = false;
            if (input != null) input.interactable = false;
        }

        void OnFailure() {
            if (feedback != null) {
                feedback.text = "> ACCESS DENIED";
                feedback.color = new Color(0.85f, 0.15f, 0.15f);
            }
            AudioOneShot.Play(AudioOneShot.Sfx.Error);
            if (GameManager.SelectedDifficulty == Difficulty.Hard) {
                var timer = FindObjectOfType<CorruptionTimer>();
                if (timer != null) timer.ApplyPenalty(30f);
            }
        }
    }
}
