using UnityEngine;

namespace WillExe {
    /// Global one-shot audio helper. Attach component to any scene root; Play() looks up clips by enum.
    public class AudioOneShot : MonoBehaviour {
        public enum Sfx { Click, Error, Success, Unlock, DialUp, Win, Lose, Glitch }

        public AudioClip click, error, success, unlock, dialUp, win, lose, glitch;
        [Range(0f, 1f)] public float volume = 0.7f;

        static AudioOneShot instance;
        AudioSource source;

        void Awake() {
            if (instance != null && instance != this) { Destroy(gameObject); return; }
            instance = this;
            source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
        }

        void OnDestroy() { if (instance == this) instance = null; }

        public static void Play(Sfx sfx) {
            if (instance == null) return;
            var clip = instance.Resolve(sfx);
            if (clip != null) instance.source.PlayOneShot(clip, instance.volume);
        }

        AudioClip Resolve(Sfx sfx) {
            switch (sfx) {
                case Sfx.Click: return click;
                case Sfx.Error: return error;
                case Sfx.Success: return success;
                case Sfx.Unlock: return unlock;
                case Sfx.DialUp: return dialUp;
                case Sfx.Win: return win;
                case Sfx.Lose: return lose;
                case Sfx.Glitch: return glitch;
            }
            return null;
        }
    }
}
