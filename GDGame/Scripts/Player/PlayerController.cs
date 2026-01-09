using System.Numerics;
using GDEngine.Core.Components;
using GDEngine.Core.Entities;
using GDEngine.Core.Events;
using GDEngine.Core.Rendering.Base;
using GDEngine.Core.Services;
using GDGame.Scripts.Audio;
using GDGame.Scripts.Events.Channels;
using GDGame.Scripts.Systems;

namespace GDGame.Scripts.Player
{
    /// <summary>
    /// Central controller for the player.
    /// Uses <see cref="PlayerCamera"/>, <see cref="PlayerStats"/> and <see cref="PlayerMovement"/>.
    /// </summary>
    public class PlayerController
    {
        #region Fields
        private readonly GameObject _playerGO;
        private readonly PlayerMovement _playerMovement;
        private readonly PlayerCamera _playerCamera;
        private readonly PlayerStats _playerStats;
        private PlayerEventChannel _playerEventChannel;
        private Vector3 _startPos = new (-3, 5, 5);
        private Vector3 _startRot = new (0, 0, 0);
        private bool _timeUpTriggered = false;
        #endregion

        #region Constructors
        public PlayerController(float aspectRatio, AudioController audio) 
        {
            _playerGO = new GameObject(AppData.PLAYER_NAME);

            _playerCamera = new PlayerCamera(aspectRatio);
            _playerMovement = new PlayerMovement();

            _playerGO.AddComponent(_playerCamera);
            _playerGO.AddComponent(_playerCamera.Cam);
            _playerGO.AddComponent(_playerMovement);
            _playerGO.AddComponent(_playerMovement.RB);
            _playerGO.AddComponent(_playerMovement.Collider);
            _playerGO.AddComponent(audio);

            _playerGO.Transform.TranslateTo(_startPos);
            _playerGO.Transform.RotateEulerBy(_startRot);

            _playerStats = new PlayerStats();
            _playerStats.Initialise();

            InitPlayerEvents();

            SceneController.AddToCurrentScene(_playerGO);

        }
        #endregion

        #region Accessors
        public GameObject PlayerGO => _playerGO;
        public Camera PlayerCam => _playerCamera.Cam;
        public PlayerStats Stats => _playerStats;
        #endregion

        #region Methods

        /// <summary>
        /// Update the player controller each frame
        /// </summary>
        public void Update()
        {
            _playerStats.HandleTimeCountdown();

            if (_playerStats.IsTimeUp && !_timeUpTriggered)
            {
                _timeUpTriggered = true;
                _playerEventChannel.OnPlayerLose.Raise();
            }
        }

        #endregion

        #region Events

        private void InitPlayerEvents()
        {
            _playerEventChannel = EventChannelManager.Instance.PlayerEvents;
            _playerEventChannel.OnOrbCollected.Subscribe(_playerStats.HandleOrbCollection);
            _playerEventChannel.OnPlayerDamaged.Subscribe(_playerStats.TakeDamage);
            EngineContext.Instance.Events.Subscribe<CollisionEvent>(_playerMovement.HandlePlayerCollision);
        }
        #endregion
    }
}
