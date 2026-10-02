using UnityEngine;

namespace Chaldran
{
    public enum TrialSound { Strike, Burst, Hit, Pickup, Unlock, Complete }

    public sealed class PrototypeAudio : MonoBehaviour
    {
        private AudioSource source;
        private AudioClip[] clips;
        private AudioSource music;
        private float musicVolume;

        public void Initialize(PresentationProfile presentation)
        {
            clips = presentation.sounds;
            source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.spatialBlend = 0f;
            source.volume = 0.35f;
            AudioListener.pause = false;
            music = gameObject.AddComponent<AudioSource>();
            music.playOnAwake = false;
            music.spatialBlend = 0f;
            music.loop = true;
            musicVolume = presentation.musicVolume;
            music.volume = musicVolume;
            music.clip = presentation.music;
            if (music.clip != null) music.Play();
        }

        public void FadeMusic(float fraction) { if (music != null) music.volume = musicVolume * Mathf.Clamp01(fraction); }

        public void Play(TrialSound sound)
        {
            int index = (int)sound;
            if (clips != null && index < clips.Length && clips[index] != null)
                source.PlayOneShot(clips[index]);
        }
    }
}
