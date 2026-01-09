using System;
using System.Windows.Forms;
using GDEngine.Core.Collections;
using GDEngine.Core.Entities;
using GDEngine.Core.Enums;
using GDEngine.Core.Managers;
using GDEngine.Core.Rendering.UI;
using GDEngine.Core.Systems;
using GDGame.Scripts.Events.Channels;
using GDGame.Scripts.Player;
using GDGame.Scripts.Systems;
using GDGame.Scripts.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Scripts.UI
{
    /// <summary>
    /// Controls the User Interface for the game, creating and storing 
    /// <see cref="CursorController"/> for the reticle in the centre of the screen, 
    /// <see cref="MenuController"/> for the start menu and options menus, 
    /// <see cref="PlayerHUD"/> for the stats displayed in the top left of the screen.
    /// </summary>
    public class UserInterfaceController : SystemBase, IDisposable
    {
        #region Fields

        // Core
        private SpriteBatch _spriteBatch;
        private ContentDictionary<SpriteFont> _fonts;
        private ContentDictionary<Texture2D> _interfaceTextures;
        private CursorController _cursorController;
        private MenuController _menuController;
        private PlayerHUD _playerHUD;
        private PauseMenu _pauseMenu;
        private Vector2 _screenCentre;
        private Game _game;
        private bool disposedValue;

        // Event Channels
        private GameEventChannel _gameEvents;
        private AudioEventChannel _audioEvents;

        #endregion

        #region Constructors
        public UserInterfaceController(SpriteBatch batch, 
            ContentDictionary<SpriteFont> fonts, ContentDictionary<Texture2D> textures, Vector2 centre, Game game) : 
            base(FrameLifecycle.PostRender, order: 10)
        {
            _spriteBatch = batch;
            _fonts = fonts;
            _interfaceTextures = textures;
            _screenCentre = centre;
            _game = game;
            _gameEvents = EventChannelManager.Instance.GameEvents;
            _audioEvents = EventChannelManager.Instance.AudioEvents;
        }
        #endregion

        #region Methods

        /// <summary>
        /// Create the cursor displayed in the centre of the screen
        /// </summary>
        private void InitCursor()
        {
            _cursorController = new CursorController(_interfaceTextures.Get(AppData.RETICLE_NAME));
            SceneController.AddToCurrentScene(_cursorController.Reticle);
        }

        /// <summary>
        /// Create the HUD in the top left of the screen
        /// </summary>
        /// <param name="stats">Player Stats to display</param>
        private void InitHUD(PlayerStats stats)
        {
            _playerHUD = new PlayerHUD(_fonts.Get(AppData.FONT_NAME), stats);
            _playerHUD.Initialise();
        }

        private void InitPauseMenu()
        {
            _pauseMenu = new PauseMenu(_interfaceTextures, _fonts.Get(AppData.FONT_NAME), _screenCentre);
        }

        /// <summary>
        /// Create the Menu Controller that is displayed on start and on pause
        /// </summary>
        private void InitMenuController()
        {
            _menuController = new MenuController(_game);
            _game.Components.Add(_menuController);

            Texture2D btnTex = _interfaceTextures.Get(AppData.BUTTON_TEXTURE);
            Texture2D trackTex = _interfaceTextures.Get(AppData.HYPHON_TEXTURE);
            Texture2D handleTex = _interfaceTextures.Get(AppData.TOGGLE_TEXTURE);
            Texture2D controlsTx = _interfaceTextures.Get(AppData.TOGGLE_TEXTURE);
            SpriteFont uiFont = _fonts.Get(AppData.FONT_NAME);

            // Wire UIManager to the menu scene
            _menuController.Initialize(SceneController.GetCurrentScene,
                btnTex, trackTex, handleTex, controlsTx, uiFont,
                _interfaceTextures.Get(AppData.MAIN_MENU_TEXTURE),
                 _interfaceTextures.Get(AppData.AUDIO_MENU_TEXTURE),
                  _interfaceTextures.Get(AppData.CONTROL_MENU_TEXTURE));

            // Subscribe to high-level events
            _menuController.PlayRequested += () =>
            {
                _menuController.HideMenus();
                //fade out menu sound
            };

            _menuController.ExitRequested += () =>
            {
                Application.Exit();
            };

            _menuController.MusicVolumeChanged += v =>
            {
                _audioEvents.OnMusicVolumeChanged.Raise(v);
            };

            _menuController.SfxVolumeChanged += v =>
            {
                _audioEvents.OnSFXVolumeChanged.Raise(v);
            };
        }

        /// <summary>
        /// Initialise the Games User Interface
        /// </summary>
        /// <param name="stats">Player Stats to display in the HUD</param>
        public void Initialise(PlayerStats stats)
        {
            InitCursor();
            InitHUD(stats);
            // InitPauseMenu();
            InitMenuController();
        }

        public override void Draw(float deltaTime)
        {
            _spriteBatch.Begin();
            _spriteBatch.End();
        }

        private void Clear()
        {
            _menuController?.Dispose();
            _menuController = null;

            _cursorController?.Dispose();
            _cursorController = null;

            _spriteBatch?.Dispose();
            _spriteBatch = null;

            _playerHUD?.Dispose();
            _playerHUD = null;

        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposedValue) return;

            if (disposing)
                Clear();

            disposedValue = true;
        }

        ~UserInterfaceController()
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
