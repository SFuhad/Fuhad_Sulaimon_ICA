using GDEngine.Core.Components;
using GDEngine.Core.Entities;
using GDEngine.Core.Input.Data;
using GDEngine.Core.Input.Devices;
using GDEngine.Core.Systems;
using GDGame.Scripts.Events.Channels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GDGame.Scripts.Systems
{
    /// <summary>
    /// Controls the input for the game, storing the <see cref="Keys"/> for each event and the <see cref="KeyboardState"/>.
    /// Uses the <see cref="InputSystem"/> and <see cref="InputEventChannel"/> to pass them through the game.
    /// </summary>
    public class InputManager : Component
    {
        #region Fields
        private readonly InputEventChannel _inputEventChannel;
        private readonly PlayerEventChannel _playerEventChannel;
        private readonly InputSystem _inputSystem;
        private GameObject _inputGO;
        private KeyboardState _newKBState, _oldKBState;
        private readonly float _mouseSensitivity = 0.12f;
        private readonly int _debounceMs = 60;
        private readonly bool _enableKeyRepeat = true;
        private readonly int _keyRepeatMs = 300;
        private bool _isPaused;
        #endregion

        #region Input Keys
        private readonly Keys _skipKey = Keys.T;
        private readonly Keys _pauseKey = Keys.Q;
        private readonly Keys _fullscreenKey = Keys.F11;
        private readonly Keys _exitKey = Keys.E;
        private readonly Keys _forwardKey = Keys.W;
        private readonly Keys _backwardKey = Keys.S;
        private readonly Keys _leftKey = Keys.A;
        private readonly Keys _rightKey = Keys.D;
        private readonly Keys _languageSwitchKey = Keys.L;
        private readonly Keys _orbTestKey = Keys.O;
        private readonly Keys _damageTestKey = Keys.P;
        private static MovementKeys _movementKeys;

        #endregion

        #region Constructors
        public InputManager() 
        {
            var bindings = InputBindings.Default;

            // Set Inititial Parameters
            bindings.MouseSensitivity = _mouseSensitivity;  
            bindings.DebounceMs = _debounceMs;          
            bindings.EnableKeyRepeat = _enableKeyRepeat;   
            bindings.KeyRepeatMs = _keyRepeatMs;       

            _inputSystem = new InputSystem();

            // Register Devices
            _inputSystem.Add(new GDKeyboardInput(bindings));
            _inputSystem.Add(new GDMouseInput(bindings));
            _inputSystem.Add(new GDGamepadInput(PlayerIndex.One, AppData.GAMEPAD_P1_NAME));

            _inputEventChannel = EventChannelManager.Instance.InputEvents;
            _playerEventChannel = EventChannelManager.Instance.PlayerEvents;
        }
        #endregion

        #region Accessors
        public InputSystem Input => _inputSystem;
        public static MovementKeys MoveKeys => _movementKeys;
        #endregion

        #region Methods

        /// <summary>
        /// Create the Input Manager game object,
        /// Create the Movement Keys struct to pass through to Player Movement,
        /// Add the Game Object and Input System to the current Scene
        /// </summary>
        public void Initialise()
        {
            _inputGO = new GameObject(AppData.INPUT_NAME);
            _inputGO.AddComponent(this);

            _movementKeys = new MovementKeys()
            {
                right = _rightKey,
                forward = _forwardKey,
                left = _leftKey,
                back = _backwardKey
            };

            SceneController.AddToCurrentScene(_inputGO);
            SceneController.AddToCurrentScene(_inputSystem);
        }
        #endregion

        #region Input Methods

        /// <summary>
        /// Check to see if the player has pressed the pause key then send out the event
        /// </summary>
        
        private void CheckForPause()
        {
            bool isPressed = _newKBState.IsKeyDown(_pauseKey) && !_oldKBState.IsKeyDown(_pauseKey);
            if (!isPressed) return;

            _inputEventChannel.OnPauseToggle.Raise();    
        }

        private void CheckForSkip()
        {
            bool isPressed = _newKBState.IsKeyDown(_skipKey) && !_oldKBState.IsKeyDown(_skipKey);
            if (!isPressed) return;

            _inputEventChannel.OnIntroSkip.Raise();
        }

        /// <summary>
        /// Check to see if the player has pressed the full screen toggle key and send out the event
        /// </summary>
        private void CheckForFullscreen()
        {
            bool isPressed = _newKBState.IsKeyDown(_fullscreenKey) && !_oldKBState.IsKeyDown(_fullscreenKey);
            if (!isPressed) return;

            _inputEventChannel.OnFullscreenToggle.Raise();
        }

        /// <summary>
        /// Check to see if the application exit key has been pressed then call the exit event
        /// </summary>
        private void CheckForExit()
        {
            bool isPressed = _newKBState.IsKeyDown(_exitKey) && !_oldKBState.IsKeyDown(_exitKey);
            if (!isPressed) return;

            _inputEventChannel.OnApplicationExit.Raise();
        }

        /// <summary>
        /// Check to see if the language swap key has been pressed then call the language swap event
        /// </summary>
        private void CheckForLanguageSwap()
        {
            bool isPressed = _newKBState.IsKeyDown(_languageSwitchKey) && !_oldKBState.IsKeyDown(_languageSwitchKey);
            if (!isPressed) return;

            _inputEventChannel.OnLanguageSwap.Raise();
        }

        /// <summary>
        /// Check to see if the Orb Test Key has been pressed then send out the orb collected event
        /// </summary>
        private void CheckForOrbTest()
        {
            bool isPressed = _newKBState.IsKeyDown(_orbTestKey) && !_oldKBState.IsKeyDown(_orbTestKey);
            if (!isPressed) return;

            _playerEventChannel.OnOrbCollected.Raise();
        }

        /// <summary>
        /// Check to see if the Damage Test Key has been pressed then send out the player damage event
        /// </summary>
        private void CheckForDamageTest()
        {
            bool isPressed = _newKBState.IsKeyDown(_damageTestKey) && !_oldKBState.IsKeyDown(_damageTestKey);
            if (!isPressed) return;

            _playerEventChannel.OnPlayerDamaged.Raise(5);
        }
        
        /// <summary>
        /// Check for any player Inputs then call the related events
        /// </summary>
        private void CheckForInputs()
        {
            CheckForFullscreen();
            CheckForExit();
            CheckForLanguageSwap();
            CheckForOrbTest();
            CheckForDamageTest();
            CheckForPause();
            CheckForSkip();
        }
        #endregion

        #region Engine Methods
        protected override void Update(float deltaTime)
        {
            _newKBState = Keyboard.GetState();
            CheckForInputs();
            _oldKBState = _newKBState;

            base.Update(deltaTime);
        }
        #endregion
    }

    public struct MovementKeys 
    { 
        public Keys forward, right, back, left; 
    }
}
