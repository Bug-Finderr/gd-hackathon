using UnityEngine;
using TMPro;

namespace WillExe.Viewers {
    public class TextViewer : MonoBehaviour {
        public TMP_Text body;

        public void Populate(WindowDef def) {
            if (body != null) body.text = def.textBody;
            if (def.isWillFragment && GameManager.Instance != null && !string.IsNullOrEmpty(def.id))
                GameManager.Instance.CollectFragment(def.id, def.fragmentText);
            if (GameManager.Instance != null && !string.IsNullOrEmpty(def.unlocksOnSolve))
                GameManager.Instance.Unlock(def.unlocksOnSolve);
        }
    }
}
