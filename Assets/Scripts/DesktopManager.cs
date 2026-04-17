using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WillExe {
    public class DesktopManager : MonoBehaviour {
        public static DesktopManager Instance { get; private set; }
        public static IconBehavior IconPrefab => Instance != null ? Instance.iconPrefab : null;

        public DesktopManifest manifest;
        public IconBehavior iconPrefab;
        public RectTransform iconLayer;

        [Header("Default Win98 icon sprites by WindowType")]
        public Sprite folderIcon;
        public Sprite textIcon;
        public Sprite imageIcon;
        public Sprite passwordIcon;
        public Sprite videoIcon;
        public Sprite fragmentIcon;
        public Sprite myComputerIcon;
        public Sprite recycleBinIcon;
        public Sprite mailIcon;
        public Sprite encryptedIcon;

        public static Sprite DefaultIconFor(WindowType t, string label) {
            var inst = Instance;
            if (inst == null) return null;
            string l = (label ?? "").ToLowerInvariant();
            if (l.Contains("my computer")) return inst.myComputerIcon;
            if (l.Contains("recycle")) return inst.recycleBinIcon;
            if (l.Contains("inbox")) return inst.mailIcon;
            if (l.EndsWith(".enc") || l.Contains(".locked") || l.Contains(".enc")) return inst.encryptedIcon;
            switch (t) {
                case WindowType.Folder: return inst.folderIcon;
                case WindowType.Text: return inst.textIcon;
                case WindowType.Image: return inst.imageIcon;
                case WindowType.Password: return inst.passwordIcon;
                case WindowType.Video: return inst.videoIcon;
                case WindowType.WillFragment: return inst.fragmentIcon;
                default: return null;
            }
        }

        readonly Dictionary<string, IconBehavior> spawned = new Dictionary<string, IconBehavior>();

        void Awake() {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        void Start() {
            if (GameManager.Instance != null) {
                GameManager.Instance.manifest = manifest;
                GameManager.Instance.OnUnlocksChanged += Refresh;
            }
            Refresh();
            StartCoroutine(ForceCanvasRebuild());
        }

        IEnumerator ForceCanvasRebuild() {
            yield return null;
            var canvas = GetComponentInParent<Canvas>();
            if (canvas == null) canvas = FindObjectOfType<Canvas>();
            foreach (var g in FindObjectsByType<Graphic>(FindObjectsSortMode.None)) g.SetAllDirty();
            Canvas.ForceUpdateCanvases();
            if (canvas != null) { canvas.enabled = false; canvas.enabled = true; }
        }

        void OnDestroy() {
            if (GameManager.Instance != null)
                GameManager.Instance.OnUnlocksChanged -= Refresh;
            if (Instance == this) Instance = null;
        }

        public void Refresh() {
            if (manifest == null) return;
            foreach (var def in manifest.topLevelIcons) {
                if (def == null) continue;
                bool visible = GameManager.Instance == null || GameManager.Instance.IsUnlocked(def.requiredUnlockId);
                if (visible && !spawned.ContainsKey(def.id))
                    Spawn(def);
            }
        }

        void Spawn(IconDef def) {
            var icon = Instantiate(iconPrefab, iconLayer);
            var rt = (RectTransform)icon.transform;
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(0, 1);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = def.desktopPosition;
            icon.Bind(def);
            spawned[def.id] = icon;
        }
    }
}
