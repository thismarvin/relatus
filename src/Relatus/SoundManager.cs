using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Relatus.Maths;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus
{
    /// <summary>
    /// Provides functionality to play registered <see cref="SoundEffect"/>'s, and handles any additional logic necessary to achieve said functionality.
    /// </summary>
    public static class SoundManager
    {
        public static float MasterVolume
        {
            get => SoundEffect.MasterVolume;
            set { SoundEffect.MasterVolume = value; }
        }

        private static readonly AudioEmitter audioEmitter;
        private static readonly AudioListener audioListener;
        private static SoundEffectInstance soundEffectInstance;

        private static SoundEffectInstance currentSongInstance;
        private static string currentSongName;

        private static bool songQueued;
        private static string nextSongName;
        private static float nextSongVolume;
        private static bool nextSongLooped;

        private static float fadeAcceleration;
        private static float fadeVelocity;

        static SoundManager()
        {
            MasterVolume = 0.75f;

            audioEmitter = new AudioEmitter();
            audioListener = new AudioListener();
        }

        /// <summary>
        /// Plays a <see cref="SoundEffect"/>.
        /// </summary>
        /// <param name="sound">The name of the <see cref="SoundEffect"/> that was loaded via <see cref="AssetManager.LoadSoundEffect(string, string)"/>.</param>
        /// <param name="volume">The <see cref="SoundEffect"/>'s unique volume constrained within 0.0f and 1.0f.</param>
        public static void PlaySoundEffect(string sound, float volume)
        {
            AssetManager.GetSoundEffect(sound).Play(volume, 0, 0);
        }

        /// <summary>
        /// Plays a <see cref="SoundEffect"/> that is processed in 3D space.
        /// </summary>
        /// <param name="sound">The name of the <see cref="SoundEffect"/> that was loaded via <see cref="AssetManager.LoadSoundEffect(string, string)"/>.</param>
        /// <param name="volume">The <see cref="SoundEffect"/>'s unique volume constrained within 0.0f and 1.0f.</param>
        /// <param name="theta">The vertical angle from the z-axis.</param>
        /// <param name="azimuth">The horizontal angle from the x-axis.</param>
        /// <param name="distance">How far away the <see cref="SoundEffect"/> is from the listener.</param>
        public static void PlaySoundEffect3D(string sound, float volume, float theta, float azimuth, float distance)
        {
            PlaySoundEffect3D(sound, volume, Vector3.Zero, Vector3Ext.SphericalToCartesian(distance, theta, azimuth));
        }

        /// <summary>
        /// Plays a <see cref="SoundEffect"/> that is processed in 3D space.
        /// </summary>
        /// <param name="sound">The name of the <see cref="SoundEffect"/> that was loaded via <see cref="AssetManager.LoadSoundEffect(string, string)"/>.</param>
        /// <param name="volume">The <see cref="SoundEffect"/>'s unique volume constrained within 0.0f and 1.0f.</param>
        /// <param name="listenerPosition">The position of the audio listener.</param>
        /// <param name="emitterPosition">The position of the audio emitter.</param>
        public static void PlaySoundEffect3D(string sound, float volume, Vector3 listenerPosition, Vector3 emitterPosition)
        {
            soundEffectInstance = AssetManager.GetSoundEffect(sound).CreateInstance();

            audioListener.Position = listenerPosition;
            audioEmitter.Position = emitterPosition;

            float constrainedVolume = ConstrainVolume(volume);

            soundEffectInstance.Volume = constrainedVolume;
            soundEffectInstance.Apply3D(audioListener, audioEmitter);
            soundEffectInstance.Play();
        }

        /// <summary>
        /// Plays a <see cref="SoundEffect"/>, but with additional functionality of a music player.
        /// </summary>
        /// <param name="song">The name of the <see cref="SoundEffect"/> that was loaded via <see cref="AssetManager.LoadSoundEffect(string, string)"/>.</param>
        /// <param name="volume">The <see cref="SoundEffect"/>'s unique volume constrained within 0.0f and 1.0f.</param>
        /// <param name="looped">Whether or not the song should loop or not.</param>
        public static void PlaySong(string song, float volume, bool looped)
        {
            if (currentSongName == song.ToLowerInvariant() && currentSongInstance.Volume == volume && currentSongInstance.IsLooped == looped)
                return;

            currentSongName = song.ToLowerInvariant();

            currentSongInstance?.Pause();
            currentSongInstance = AssetManager.GetSoundEffect(song).CreateInstance();

            float constrainedVolume = ConstrainVolume(volume);

            currentSongInstance.Volume = constrainedVolume;
            currentSongInstance.IsLooped = looped;
            currentSongInstance.Play();
        }

        /// <summary>
        /// Queues a song to be played next. Unlike <see cref="PlaySong(string, float, bool)"/>, the queued song will start playing after the current song finishes fading out.
        /// </summary>
        /// <param name="song">The name of the <see cref="SoundEffect"/> that was loaded via <see cref="AssetManager.LoadSoundEffect(string, string)"/>.</param>
        /// <param name="volume">The <see cref="SoundEffect"/>'s unique volume constrained within 0.0f and 1.0f.</param>
        /// <param name="looped">Whether or not the song should loop or not.</param>
        /// <param name="fadeAcceleration">The acceleration of how fast the current song should fade away.</param>
        public static void QueueSong(string song, float volume, bool looped, float fadeAcceleration)
        {
            if (songQueued)
                return;

            if (currentSongName == song.ToLowerInvariant() && currentSongInstance.Volume == volume && currentSongInstance.IsLooped == looped)
                return;

            songQueued = true;
            nextSongName = song.ToLowerInvariant();
            nextSongVolume = volume;
            nextSongLooped = looped;

            SoundManager.fadeAcceleration = -fadeAcceleration;
        }

        /// <summary>
        /// Pauses the current song if it is playing, or unpauses the current song if it is paused.
        /// </summary>
        public static void ToggleCurrentSong()
        {
            if (currentSongInstance == null)
                return;

            if (currentSongInstance.State == SoundState.Playing)
            {
                currentSongInstance.Pause();
            }
            else if (currentSongInstance.State == SoundState.Paused)
            {
                currentSongInstance.Resume();
            }
        }

        private static float ConstrainVolume(float volume)
        {
            float constrainedVolume = volume;
            constrainedVolume = constrainedVolume < 0 ? 0 : constrainedVolume;
            constrainedVolume = constrainedVolume > 1 ? 1 : constrainedVolume;

            return constrainedVolume;
        }

        private static void UpdateSongTransitions()
        {
            if (!songQueued)
                return;

            if (currentSongInstance == null)
            {
                PlaySong(nextSongName, nextSongVolume, nextSongLooped);
            }
            else
            {
                float currentVolume = currentSongInstance.Volume;

                currentVolume += fadeVelocity * Engine.DeltaTime + 0.5f * fadeAcceleration * Engine.DeltaTime * Engine.DeltaTime;
                fadeVelocity += fadeAcceleration * Engine.DeltaTime;

                if (currentVolume <= 0)
                {
                    songQueued = false;
                    PlaySong(nextSongName, nextSongVolume, nextSongLooped);
                }
                else
                {
                    currentSongInstance.Volume = currentVolume;
                }
            }
        }

        internal static void Update()
        {
            UpdateSongTransitions();
        }
    }
}
