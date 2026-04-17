using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

namespace WillExe {
    /// Attached to DesktopWindow prefab root. Handles drag (titlebar child), z-order, close.
    public class WindowController : MonoBehaviour, IPointerDownHandler {
        public RectTransform titleBar;
        public TMP_Text titleLabel;
        public Button closeButton;
        public Transform contentSlot;

        RectTransform rt;
        Canvas rootCanvas;

        void Awake() {
            rt = transform as RectTransform;
            rootCanvas = GetComponentInParent<Canvas>();
            if (closeButton != null) closeButton.onClick.AddListener(Close);
            var drag = titleBar != null ? titleBar.gameObject : gameObject;
            var handler = drag.GetComponent<WindowDragHandler>();
            if (handler == null) handler = drag.AddComponent<WindowDragHandler>();
            handler.target = rt;
        }

        public void SetTitle(string t) { if (titleLabel != null) titleLabel.text = t; }

        public void BringToFront() { transform.SetAsLastSibling(); }

        public void OnPointerDown(PointerEventData eventData) { BringToFront(); }

        public void Close() { Destroy(gameObject); AudioOneShot.Play(AudioOneShot.Sfx.Click); }
    }

    public class WindowDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler {
        public RectTransform target;
        Canvas canvas;
        Vector2 pointerOffset;

        void Awake() { canvas = GetComponentInParent<Canvas>(); }

        public void OnBeginDrag(PointerEventData eventData) {
            if (target == null) return;
            target.SetAsLastSibling();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                target.parent as RectTransform, eventData.position, eventData.pressEventCamera, out var localPoint);
            pointerOffset = (Vector2)target.localPosition - localPoint;
        }

        public void OnDrag(PointerEventData eventData) {
            if (target == null) return;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                target.parent as RectTransform, eventData.position, eventData.pressEventCamera, out var localPoint);
            target.localPosition = localPoint + pointerOffset;
        }
    }
}
