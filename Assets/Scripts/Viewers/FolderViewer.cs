using UnityEngine;

namespace WillExe.Viewers {
    public class FolderViewer : MonoBehaviour {
        public IconBehavior iconPrefab;
        public RectTransform grid;

        public void Populate(WindowDef def) {
            if (def.childIcons == null) return;
            var prefab = iconPrefab != null ? iconPrefab : DesktopManager.IconPrefab;
            if (prefab == null) { Debug.LogError("FolderViewer: no icon prefab available"); return; }
            foreach (var iconDef in def.childIcons) {
                if (iconDef == null) continue;
                if (GameManager.Instance != null && !GameManager.Instance.IsUnlocked(iconDef.requiredUnlockId)) continue;
                var icon = Instantiate(prefab, grid);
                icon.Bind(iconDef);
            }
        }
    }
}
