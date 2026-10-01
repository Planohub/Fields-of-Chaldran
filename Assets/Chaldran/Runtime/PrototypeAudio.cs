using UnityEngine;

namespace Chaldran
{
    public enum TrialSound { Strike, Burst, Hit, Pickup, Unlock, Complete }

    public sealed class PrototypeAudio : MonoBehaviour
    {
        private AudioSource source;
        private AudioClip[] clips;

        public void Initialize(AudioClip[] sounds)
        {
            clips = sounds;
            source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.spatialBlend = 0f;
            source.volume = 0.35f;
            AudioListener.pause = false;
        }

        public void Play(TrialSound sound)
        {
            int index = (int)sound;
            if (clips != null && index < clips.Length && clips[index] != null)
                source.PlayOneShot(clips[index]);
        }
    }
}
