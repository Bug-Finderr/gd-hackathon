using UnityEngine;
using UnityEngine.UI;
using WillExe.Viewers;

namespace WillExe {
    public class WindowSpawner : MonoBehaviour {
        public static WindowSpawner Instance { get; private set; }

        [Header("Prefabs")]
        public WindowController windowPrefab;
        public TextViewer textViewerPrefab;
        public ImageViewer imageViewerPrefab;
        public FolderViewer folderViewerPrefab;
        public PasswordGate passwordGatePrefab;
        public VideoViewer videoViewerPrefab;
        public WillFragmentViewer fragmentViewerPrefab;
        public IconBehavior iconPrefabForFolders;

        [Header("Parent")]
        public RectTransform windowLayer;

        void Awake() {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        void OnDestroy() { if (Instance == this) Instance = null; }

        public static WindowController Open(WindowDef def) {
            if (Instance == null || def == null) return null;
            var win = Instantiate(Instance.windowPrefab, Instance.windowLayer);
            win.SetTitle(def.title);
            var rt = win.transform as RectTransform;
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = def.defaultSize;
            rt.anchoredPosition = new Vector2(Random.Range(-80, 80), Random.Range(-60, 60));

            AudioOneShot.Play(AudioOneShot.Sfx.Click);

            switch (def.type) {
                case WindowType.Text: {
                    var v = Instantiate(Instance.textViewerPrefab, win.contentSlot);
                    v.Populate(def);
                    break;
                }
                case WindowType.Image: {
                    var v = Instantiate(Instance.imageViewerPrefab, win.contentSlot);
                    v.Populate(def);
                    break;
                }
                case WindowType.Folder: {
                    var v = Instantiate(Instance.folderViewerPrefab, win.contentSlot);
                    if (v.iconPrefab == null) v.iconPrefab = Instance.iconPrefabForFolders;
                    v.Populate(def);
                    break;
                }
                case WindowType.Password: {
                    var v = Instantiate(Instance.passwordGatePrefab, win.contentSlot);
                    v.Populate(def);
                    break;
                }
                case WindowType.Video: {
                    var v = Instantiate(Instance.videoViewerPrefab, win.contentSlot);
                    v.Populate(def);
                    break;
                }
                case WindowType.WillFragment: {
                    var v = Instantiate(Instance.fragmentViewerPrefab, win.contentSlot);
                    v.Populate(def);
                    break;
                }
            }

            win.BringToFront();
            ForceCanvasRepaint(win);
            return win;
        }

        static void ForceCanvasRepaint(WindowController win) {
            foreach (var g in win.GetComponentsInChildren<Graphic>(true)) {
                g.SetAllDirty();
            }
            var canvas = win.GetComponentInParent<Canvas>();
            if (canvas != null) {
                Canvas.ForceUpdateCanvases();
                canvas.enabled = false;
                canvas.enabled = true;
            }
        }
    }
}
