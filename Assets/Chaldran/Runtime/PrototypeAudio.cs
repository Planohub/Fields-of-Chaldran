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
        private AudioSource typing;
        private AudioClip typingClip;
        private float nextTyping;

        public void Initialize(PresentationProfile presentation)
        {
            clips = presentation.sounds;
            source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.spatialBlend = 0f;
            source.volume = 0.35f;
            typing = gameObject.AddComponent<AudioSource>();
            typing.playOnAwake = false;
            typing.spatialBlend = 0f;
            typing.volume = presentation.typingVolume;
            typingClip = presentation.typingSound;
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
        public void StopTyping() { if (typing != null) typing.Stop(); }
        public void PlayTyping()
        {
            if (typingClip == null || typing.volume <= 0 || Time.unscaledTime < nextTyping) return;
            nextTyping = Time.unscaledTime + 0.04f;
            typing.PlayOneShot(typingClip);
        }

        public void Play(TrialSound sound)
        {
            int index = (int)sound;
            if (clips != null && index < clips.Length && clips[index] != null)
                source.PlayOneShot(clips[index]);
        }
    }
}
