using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace WillExe {
    public enum Difficulty { Easy, Normal, Hard }

    public class GameManager : MonoBehaviour {
        public static GameManager Instance { get; private set; }

        public static Difficulty SelectedDifficulty = Difficulty.Normal;

        [Header("Refs")]
        public DesktopManifest manifest;

        public readonly HashSet<string> unlocks = new HashSet<string>();
        public readonly HashSet<string> collectedFragments = new HashSet<string>();

        public System.Action OnUnlocksChanged;
        public System.Action<string> OnFragmentCollected;
        public System.Action OnWin;
        public System.Action OnLose;

        bool ended;

        void Awake() {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        void OnDestroy() { if (Instance == this) Instance = null; }

        public bool IsUnlocked(string id) {
            return string.IsNullOrEmpty(id) || unlocks.Contains(id);
        }

        public void Unlock(string id) {
            if (string.IsNullOrEmpty(id)) return;
            if (unlocks.Add(id)) OnUnlocksChanged?.Invoke();
        }

        public void CollectFragment(string id, string fragmentText) {
            if (string.IsNullOrEmpty(id)) return;
            if (collectedFragments.Add(id)) {
                OnFragmentCollected?.Invoke(fragmentText);
                if (manifest != null && collectedFragments.Count >= manifest.requiredFragments)
                    TriggerWin();
            }
        }

        public int FragmentCount => collectedFragments.Count;
        public int RequiredFragments => manifest != null ? manifest.requiredFragments : 4;

        public void TriggerWin() {
            if (ended) return;
            ended = true;
            OnWin?.Invoke();
        }

        public void TriggerLose() {
            if (ended) return;
            ended = true;
            OnLose?.Invoke();
        }

        public void Restart() {
            unlocks.Clear();
            collectedFragments.Clear();
            ended = false;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void LoadBoot() {
            unlocks.Clear();
            collectedFragments.Clear();
            ended = false;
            SceneManager.LoadScene("Boot");
        }
    }
}
