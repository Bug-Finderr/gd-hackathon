using UnityEngine;
using TMPro;

namespace WillExe.Viewers {
    public class WillFragmentViewer : MonoBehaviour {
        public TMP_Text fragmentBody;
        public TMP_Text stamp;

        public void Populate(WindowDef def) {
            if (fragmentBody != null) fragmentBody.text = def.fragmentText;
            if (stamp != null) stamp.text = $"WILL FRAGMENT :: {def.id}";
            if (GameManager.Instance != null) GameManager.Instance.CollectFragment(def.id, def.fragmentText);
            if (GameManager.Instance != null && !string.IsNullOrEmpty(def.unlocksOnSolve))
                GameManager.Instance.Unlock(def.unlocksOnSolve);
            AudioOneShot.Play(AudioOneShot.Sfx.Success);
        }
    }
}
