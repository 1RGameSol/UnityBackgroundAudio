using UnityEngine;
using BackgroundAudio.Base;
using System;

namespace BackgroundAudio.UnsupportedPlatform
{
    [Serializable]
    public class UnsupportedPlatformBackgroundAudio : BackgroundAudioImplementation
    {
        public override event Action OnAudioStarted;
        public override event Action OnAudioStopped;
        public override event Action OnAudioPaused;
        public override event Action OnAudioResumed;

        private AudioSource audioSource;

        private bool isPause;

        protected override void Initialize()
        {
            audioSource = new GameObject().AddComponent<AudioSource>();
            audioSource.gameObject.name = "CurAudioObj";
        }

        public override void Play(string path)
        {
            throw new PlatformNotSupportedException();
        }

        public override void Play(AudioClip clip)
        {
            if (audioSource)
            {
                audioSource.clip = clip;
                audioSource.Play();
            }
        }

        public override void Stop()
        {
            if (audioSource)
            {
                if (audioSource.isPlaying) audioSource.Stop();
            }
        }

        public override void Pause()
        {
            isPause = true;
        }

        public override void Resume()
        {
            isPause = false;
        }

        public override void Seek(float seconds)
        {
            if (audioSource)
            {
                if (audioSource.isPlaying) audioSource.time += seconds;
            }
        }

        public override void SetVolume(float volume)
        {
            if (audioSource)
            {
                audioSource.volume = volume;
            }
        }

        public override void SetLoop(bool value)
        {
            if (audioSource)
                audioSource.loop = value;
            else Debug.LogError("No audio source in the scene");
        }

        public override float GetCurrentPosition()
        {
            if (audioSource)
                return audioSource.time;
            return 0;
        }

        public override float GetDuration()
        {
            if (audioSource)
                return audioSource.clip.length;
            return 0;
        }

        public override float GetVolume()
        {
            if (audioSource)
                return audioSource.volume;
            return 0;
        }

        public override bool IsLooping()
        {
            if (audioSource)
                return audioSource.loop;
            return false;
        }

        public override bool IsPlaying()
        {
            if (audioSource)
                return audioSource.isPlaying;
            return false;
        }

        public override bool IsPaused()
        {
            return isPause;
        }
    }
}