using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

namespace WillExe {
    /// Desktop or folder icon. Double-click opens its associated WindowDef.
    public class IconBehavior : MonoBehaviour, IPointerClickHandler {
        public Image iconImage;
        public TMP_Text labelText;

        IconDef def;
        float lastClickTime = -10f;
        const float DoubleClickWindow = 0.35f;

        public void Bind(IconDef d) {
            def = d;
            if (iconImage != null) {
                Sprite s = d.iconSprite;
                if (s == null && d.window != null) s = DesktopManager.DefaultIconFor(d.window.type, d.label);
                if (s != null) {
                    iconImage.sprite = s;
                    iconImage.color = Color.white;
                    iconImage.preserveAspect = true;
                } else if (d.window != null) {
                    iconImage.color = TintFor(d.window.type, d.window.isWillFragment);
                }
            }
            if (labelText != null) labelText.text = d.label;
        }

        static Color TintFor(WindowType t, bool isFragment) {
            if (isFragment) return new Color(0.85f, 0.25f, 0.25f);
            switch (t) {
                case WindowType.Folder: return new Color(0.95f, 0.80f, 0.35f);
                case WindowType.Password: return new Color(0.80f, 0.30f, 0.30f);
                case WindowType.Text: return new Color(0.95f, 0.95f, 0.90f);
                case WindowType.Image: return new Color(0.45f, 0.80f, 0.55f);
                case WindowType.Video: return new Color(0.70f, 0.40f, 0.85f);
                case WindowType.WillFragment: return new Color(0.95f, 0.55f, 0.15f);
                default: return Color.white;
            }
        }

        public void OnPointerClick(PointerEventData eventData) {
            if (eventData.clickCount >= 2 || Time.unscaledTime - lastClickTime < DoubleClickWindow) {
                Activate();
                lastClickTime = -10f;
            } else {
                lastClickTime = Time.unscaledTime;
            }
        }

        void Activate() {
            if (def == null || def.window == null) return;
            WindowSpawner.Open(def.window);
        }
    }
}
