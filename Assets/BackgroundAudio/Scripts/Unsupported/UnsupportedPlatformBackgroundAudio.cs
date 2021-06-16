using System;
using System.Collections;

using BackgroundAudio.Base;

using UnityEditor.PackageManager.UI;

using UnityEngine;
using UnityEngine.Networking;

namespace BackgroundAudio.UnsupportedPlatform
{
    [Serializable]
    public class UnsupportedPlatformBackgroundAudio : BackgroundAudioImplementation
    {
        public override event Action OnAudioStarted;
        public override event Action OnAudioStopped;
        public override event Action OnAudioPaused;
        public override event Action OnAudioResumed;

        [SerializeField]
        private AudioSource audioSource;
        private bool isPause;

        protected override void Initialize()
        {
            audioSource = new GameObject().AddComponent<AudioSource>();
            audioSource.gameObject.name = "CurAudioObj";
            //throw new PlatformNotSupportedException();
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
            throw new PlatformNotSupportedException();
        }

        public override void Pause()
        {
            isPause = true;
            // throw new PlatformNotSupportedException();
        }

        public override void Resume()
        {
            isPause = false;
            //throw new PlatformNotSupportedException();
        }

        public override void Seek(float seconds)
        {
            throw new PlatformNotSupportedException();
        }

        public override void SetVolume(float volume)
        {
            throw new PlatformNotSupportedException();
        }

        public override void SetLoop(bool value)
        {
            if (audioSource)
                audioSource.loop = value;
            else Debug.LogError("No audio source in the scene");
            //throw new PlatformNotSupportedException();
        }

        public override float GetCurrentPosition()
        {
            if (audioSource)
                return audioSource.time;
            return 0;
            //throw new PlatformNotSupportedException();
        }

        public override float GetDuration()
        {
            if (audioSource)
                return audioSource.clip.length;
            return 0;
            //throw new PlatformNotSupportedException();
        }

        public override float GetVolume()
        {
            if (audioSource)
                return audioSource.volume;
            return 0;
            //throw new PlatformNotSupportedException();
        }

        public override bool IsLooping()
        {
            if (audioSource)
                return audioSource.loop;
            return false;
            //  throw new PlatformNotSupportedException();
        }

        public override bool IsPlaying()
        {
            if (audioSource)
                return audioSource.isPlaying;
            return false;
            // throw new PlatformNotSupportedException();
        }

        public override bool IsPaused()
        {
            return isPause;
            //throw new PlatformNotSupportedException();
        }
    }
}