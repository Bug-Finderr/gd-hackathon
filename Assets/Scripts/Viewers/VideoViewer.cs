using UnityEngine;
using UnityEngine.UI;

namespace WillExe.Viewers {
    public class VideoViewer : MonoBehaviour {
        public Image staticImage;

        Sprite[] frames;
        float fps = 12f;
        float frameTimer;
        int index;

        public void Populate(WindowDef def) {
            frames = def.videoFrames;
            fps = def.videoFps > 0 ? def.videoFps : 12f;
            index = 0;
            frameTimer = 0f;
            if (staticImage != null) {
                if (frames != null && frames.Length > 0) {
                    staticImage.sprite = frames[0];
                    staticImage.color = Color.white;
                } else {
                    staticImage.sprite = def.imageSprite;
                    staticImage.color = def.imageSprite != null ? Color.white : new Color(0.2f, 0.2f, 0.2f);
                }
                staticImage.preserveAspect = true;
            }
        }

        void Update() {
            if (frames == null || frames.Length <= 1 || staticImage == null) return;
            frameTimer += Time.unscaledDeltaTime;
            float spf = 1f / fps;
            while (frameTimer >= spf) {
                frameTimer -= spf;
                index = (index + 1) % frames.Length;
                staticImage.sprite = frames[index];
            }
        }
    }
}
