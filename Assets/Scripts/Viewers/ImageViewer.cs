using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace WillExe.Viewers {
    public class ImageViewer : MonoBehaviour {
        public Image picture;
        public TMP_Text caption;

        public void Populate(WindowDef def) {
            if (picture != null) {
                picture.sprite = def.imageSprite;
                picture.color = def.imageSprite != null ? Color.white : new Color(0.3f, 0.3f, 0.3f);
                picture.enabled = true;
            }
            if (caption != null) caption.text = def.caption;
        }
    }
}
