using System;

namespace GDGame.Scripts.Events.Channels
{
    /// <summary>
    /// Stores all event channels for the game. 
    /// Creates an Instance that can be accessed by other scripts.
    /// Consists of event channels using <see cref="EventBase"/>.
    /// </summary>
    public class EventChannelManager
    {
        // Instance
        private static EventChannelManager _instance;

        // Channels
        private readonly InputEventChannel _inputEvents;
        private readonly PlayerEventChannel _playerEvents;
        private readonly AudioEventChannel _audioEvents;
        private readonly GameEventChannel _gameEvents;

        public EventChannelManager() 
        {
            _inputEvents = new InputEventChannel();
            _playerEvents = new PlayerEventChannel();
            _audioEvents = new AudioEventChannel();
            _gameEvents = new GameEventChannel();
        }

        #region Accessors
        public static EventChannelManager Instance
        {
            get
            {
                if (_instance == null)
                    throw new InvalidOperationException("Ensure you call Initialise() first");

                return _instance;
            }
        }
        public InputEventChannel InputEvents => _inputEvents;
        public PlayerEventChannel PlayerEvents => _playerEvents;
        public AudioEventChannel AudioEvents => _audioEvents;
        public GameEventChannel GameEvents => _gameEvents;
        #endregion

        #region Methods
        public static void Initialise()
        {
            if(_instance != null) return;

            _instance = new EventChannelManager();
        }

        public void ClearEventChannels()
        {
            _inputEvents.ClearEventChannel();
            _playerEvents.ClearEventChannel();
            _audioEvents.ClearEventChannel();
            _gameEvents.ClearEventChannel();
        }

        #endregion
    }
}
