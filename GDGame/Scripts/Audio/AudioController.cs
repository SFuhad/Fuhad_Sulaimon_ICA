using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using GDEngine.Core.Audio;
using GDEngine.Core.Collections;
using GDEngine.Core.Components;
using GDEngine.Core.Entities;
using GDEngine.Core.Systems;
using GDGame.Scripts.Events.Channels;
using GDGame.Scripts.Systems;
using Microsoft.Xna.Framework.Audio;
using static GDEngine.Core.Audio.AudioMixer;

namespace GDGame.Scripts.Audio
{
    /// <summary>
    /// Controls all Audio in the game and stores the Sound files
    /// </summary>
    public class AudioController : Component, IDisposable
    {

        #region Fields
        private AudioSystem _audioSystem;
        private ContentDictionary<SoundEffect> _sounds;
        private List<GameObject> _3DsoundsList;
        private AudioEventChannel _audioEventChannel;
        private float _musicVolume = 0.25f;
        private float _sfxVolume = 0.8f;
        private float _musicFade = 0;
        private bool _musicLooped = true;
        private bool disposedValue;
        #endregion

        #region Constructors
        public AudioController(ContentDictionary<SoundEffect> sounds)
        {
            _sounds = sounds;
            _3DsoundsList = new();
            _audioSystem = new AudioSystem(_sounds);
        }
        #endregion

        #region Accessors
        public List<GameObject> SoundsList => _3DsoundsList;
        #endregion

        #region Methods
        /// <summary>
        /// Set the Games main music based off of a given key
        /// </summary>
        /// /// <param name="key">Audio File Key</param>
        private void PlayMusic(string key)
        {
            _audioSystem.PlayMusic(key, _musicVolume, _musicFade, _musicLooped);
        }

        /// <summary>
        /// Play a sound effect in the game off of a given key
        /// </summary>
        /// <param name="key">Audio File Key</param>
        private void PlaySFX(string key)
        {
            _audioSystem.PlayOneShot(key, _sfxVolume);
        }

        /// <summary>
        /// Generate the 3D Audio Objects for the scene
        /// </summary>
        private void Generate3DAudio()
        {
            var obj1 = Generate3DAudioObject(AppData.RATTLE_KEY, new Vector3(1, 1, 1));
            _3DsoundsList.Add(obj1);

            var obj2 = Generate3DAudioObject(AppData.CAT_AUDIO_NAME, new Vector3(1, 1, 1));
            _3DsoundsList.Add(obj2);

            var obj3 = Generate3DAudioObject(AppData.DUNGEON_AUDIO_NAME, new Vector3(1, 1, 1));
            _3DsoundsList.Add(obj3);
        }

        /// <summary>
        /// Add the 3D Audio Objects to the scene
        /// </summary>
        private void Add3DAudioToScene()
        {
            foreach(var obj in _3DsoundsList)
                SceneController.AddToCurrentScene(obj);
        }

        /// <summary>
        /// Generate a 3D Audio object off of the audio name and position in world
        /// </summary>
        /// <param name="name">Audio File key</param>
        /// <param name="position">Position in world</param>
        /// <returns>GameObject with 3D Audio attached</returns>
        private GameObject Generate3DAudioObject(string name, Vector3 position)
        {
            var soundGO = new GameObject(name);
            soundGO.Transform.TranslateTo(position);
            var audio = new _3DAudioController(_audioSystem.Listener, soundGO.Transform, _sounds.Get(name));
            soundGO.AddComponent(audio);
            return soundGO;
        }

        /// <summary>
        /// Change the volume of the music
        /// </summary>
        /// <param name="volume">New Music Volume</param>
        private void HandleMusicVolumeChange(float volume)
        {
            _musicVolume = volume;
            _audioSystem.Mixer.SetVolume(AudioChannel.Music, _musicVolume);
            _audioSystem.CurrentMusic.Volume = _musicVolume;
        }

        /// <summary>
        /// Change the volume of the SFX
        /// </summary>
        /// <param name="volume">New SFX Volume</param>
        private void HandleSFXVolumeChange(float volume)
        {
            _sfxVolume = volume;
            _audioSystem.SetChannelVolume(AudioChannel.Sfx, _sfxVolume);
        }

        /// <summary>
        /// Initialise the Audio Events to be called in other scripts
        /// </summary>
        private void InitEventHandlers()
        {
            _audioEventChannel.OnMusicRequested.Subscribe(PlayMusic);
            _audioEventChannel.OnMusicVolumeChanged.Subscribe(HandleMusicVolumeChange);
            _audioEventChannel.OnSFXRequested.Subscribe(PlaySFX);
            _audioEventChannel.OnSFXVolumeChanged.Subscribe(HandleSFXVolumeChange);
        }

        /// <summary>
        /// Initialise the Audio Controller.
        /// Sets the games main music, generates 3D Audio Objects and initialise the audio events
        /// </summary>
        public void Initialise()
        {
            _audioEventChannel = EventChannelManager.Instance.AudioEvents;
            PlayMusic(AppData.MAIN_THEME_KEY);
            Generate3DAudio();
            Add3DAudioToScene();
            InitEventHandlers();
        }

        private void Clear()
        {
            _audioSystem?.Dispose();
            _audioSystem = null;

            _sounds?.Dispose();
            _sounds = null;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposedValue) return;

            if (disposing)
                Clear();

            disposedValue = true;
        }

        ~AudioController()
        {
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion

    }
}
