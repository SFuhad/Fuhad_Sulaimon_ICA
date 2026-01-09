using GDEngine.Core.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace GDGame.Scripts.Audio
{
    /// <summary>
    /// Creates a 3D Audio Object in the game environment
    /// </summary>
    public class _3DAudioController : Component
    {
        #region Fields
        private AudioEmitter _emitter;
        private AudioListener _listener;
        private SoundEffectInstance _soundInstance;
        private Transform _soundTransform;
        private float _soundRadius;
        private float _volume;
        private bool _looped;
        private bool _active;
        #endregion


        #region Constructors
        public _3DAudioController(AudioListener listener, Transform soundTransform, 
            SoundEffect sound, float volume = 1f, float radius = 0.25f, bool looped = true) 
        {
            _soundInstance = sound.CreateInstance();
            _soundTransform = soundTransform;
            _emitter = new AudioEmitter();
            _listener = listener;
            _looped = looped;
            _volume = volume;
            _soundRadius = radius;
            _active = true;

            Init3DSound();
        }
        #endregion

        #region Methods

        /// <summary>
        /// Initialise the 3D Sound Object
        /// </summary>
        private void Init3DSound()
        {
            _emitter.Position = _soundTransform.Position;
            _emitter.Forward = _soundTransform.Forward;
            _emitter.Up = _soundTransform.Up;
            _emitter.Velocity = Vector3.Zero;
            _emitter.DopplerScale = _soundRadius;

            _soundInstance.IsLooped = _looped;
            _soundInstance.Volume = _volume;
            _soundInstance.Apply3D(_listener, _emitter);
            _soundInstance.Play();
        }

        /// <summary>
        /// Turn the 3D Sound Instance on or off
        /// </summary>
        public void Toggle3DSound()
        {
            if(_active)
                _soundInstance.Stop();
            else
                _soundInstance.Play();

            _active = !_active;
        }
        #endregion
    }
}
