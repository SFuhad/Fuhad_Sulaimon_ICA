using System;
using System.Diagnostics;
using GDEngine.Core.Entities;
using GDEngine.Core.Managers;
using GDEngine.Core.Rendering;
using GDEngine.Core.Rendering.UI;
using GDGame.Scripts.Events.Channels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GDGame.Scripts.UI
{
    /// <summary>
    /// High-level menu controller that creates and manages three UI panels:
    /// 1) Main menu (Play, Audio, Controls, Exit).
    /// 2) Audio menu (Music + SFX sliders, Back).
    /// 3) Controls menu (controls layout texture + Back).
    /// This class is a MonoGame <see cref="GameComponent"/> so it
    /// can live alongside a <c>SceneManager : DrawableGameComponent</c>
    /// and configure a dedicated "menu scene" that is separate from
    /// gameplay scenes. Based off of <see cref="MenuManager"/>.
    /// </summary>
    /// <see cref="Scene"/>
    /// <see cref="UIMenuPanel"/>
    /// <see cref="UIButton"/>
    /// <see cref="UISlider"/>
    /// <see cref="UITexture"/>
    public sealed class MenuController : GameComponent
    {
        #region Fields
        private Scene _menuScene;

        // Panels
        private UIMenuPanel _mainMenuPanel;
        private UIMenuPanel _audioMenuPanel;
        private UIMenuPanel _controlsMenuPanel;

        // Main menu buttons
        private UIButton _playButton;
        private UIButton _audioButton;
        private UIButton _controlsButton;
        private UIButton _exitButton;

        // Audio menu controls
        private UIButton _audioBackButton;
        private UISlider _musicSlider;
        private UISlider _sfxSlider;

        // Controls menu controls
        private UIButton _controlsBackButton;
        private UITexture _controlsLayoutTexture;

        // Assets
        private Texture2D _buttonTexture;
        private Texture2D _sliderTrackTexture;
        private Texture2D _sliderHandleTexture;
        private Texture2D _controlsLayout;
        private SpriteFont _font;

        private bool _gameStarted;
        private bool _configured;
        private bool _built;
        private bool _menuVisible;
        private Texture2D _mainPanelBackground;
        private Texture2D _audioPanelBackground;
        private Texture2D _controlsPanelBackground;

        //Event Channels
        private InputEventChannel _inputEvents;
        private GameEventChannel _gameEvents;
        private KeyboardState _newKBState;
        private KeyboardState _oldKBState;
        #endregion

        #region Properties
        /// <summary>
        /// Returns true if any menu panel is currently visible.
        /// </summary>
        public bool IsMenuVisible
        {
            get { return _menuVisible; }
        }
        /// <summary>
        /// Raised when the user presses the Play button on the main menu.
        /// The game should subscribe and start gameplay / unpause.
        /// </summary>
        public event Action? PlayRequested;

        /// <summary>
        /// Raised when the user presses the Exit button on the main menu.
        /// The game (or a higher-level system) should subscribe and call Game.Exit().
        /// </summary>
        public event Action? ExitRequested;

        /// <summary>
        /// Raised when the Music slider value changes (0-1 by default).
        /// </summary>
        public event Action<float>? MusicVolumeChanged;

        /// <summary>
        /// Raised when the SFX slider value changes (0-1 by default).
        /// </summary>
        public event Action<float>? SfxVolumeChanged;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a <see cref="MenuController"/> as a MonoGame <see cref="GameComponent"/>.
        /// Add this to <c>Game.Components</c> in your Game subclass.
        /// </summary>
        public MenuController(Game game)
            : base(game)
        {
            _gameStarted = false;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Configure the manager with:
        /// - The dedicated menu <see cref="Scene"/> (separate from gameplay scenes).
        /// - Textures and font for the UI.
        /// 
        /// Once called, the manager builds the three menu panels into the menu scene.
        /// </summary>
        public void Initialize(
         Scene menuScene,
         Texture2D buttonTexture,
         Texture2D sliderTrackTexture,
         Texture2D sliderHandleTexture,
         Texture2D controlsLayoutTexture,
         SpriteFont font,
         Texture2D mainPanelBackground,
         Texture2D audioPanelBackground,
         Texture2D controlsPanelBackground)
        {
            if (menuScene == null)
                throw new ArgumentNullException(nameof(menuScene));
            if (buttonTexture == null)
                throw new ArgumentNullException(nameof(buttonTexture));
            if (sliderTrackTexture == null)
                throw new ArgumentNullException(nameof(sliderTrackTexture));
            if (sliderHandleTexture == null)
                throw new ArgumentNullException(nameof(sliderHandleTexture));
            if (controlsLayoutTexture == null)
                throw new ArgumentNullException(nameof(controlsLayoutTexture));
            if (font == null)
                throw new ArgumentNullException(nameof(font));
            if (mainPanelBackground == null)
                throw new ArgumentNullException(nameof(mainPanelBackground));
            if (audioPanelBackground == null)
                throw new ArgumentNullException(nameof(audioPanelBackground));
            if (controlsPanelBackground == null)
                throw new ArgumentNullException(nameof(controlsPanelBackground));

            _menuScene = menuScene;
            _buttonTexture = buttonTexture;
            _sliderTrackTexture = sliderTrackTexture;
            _sliderHandleTexture = sliderHandleTexture;
            _controlsLayout = controlsLayoutTexture;
            _font = font;

            _mainPanelBackground = mainPanelBackground;
            _audioPanelBackground = audioPanelBackground;
            _controlsPanelBackground = controlsPanelBackground;

            _configured = true;


            _inputEvents = EventChannelManager.Instance.InputEvents;
            _gameEvents = EventChannelManager.Instance.GameEvents;
            _inputEvents.OnPauseToggle.Subscribe(ToggleMenu);

            TryBuildMenus();
        }

        private void HandleLanguageSwap()
        {
            // System is too convoluted button text is stored in created buttons in menu panel
        }

        /// <summary>
        /// Show the main menu and hide the other panels.
        /// This assumes the menu scene is currently active in the SceneManager.
        /// </summary>
        public void ShowMainMenu()
        {
            if (_mainMenuPanel == null || _audioMenuPanel == null || _controlsMenuPanel == null)
                return;

            SetActivePanel(_mainMenuPanel, _audioMenuPanel, _controlsMenuPanel);

            // HUD/reticle should be hidden while menu is up
            SetReticleVisible(false);
        }


        /// <summary>
        /// Show the audio menu and hide the other panels.
        /// </summary>
        public void ShowAudioMenu()
        {
            if (_mainMenuPanel == null ||
                _audioMenuPanel == null ||
                _controlsMenuPanel == null)
                return;

            SetActivePanel(_audioMenuPanel, _mainMenuPanel, _controlsMenuPanel);

            // Hide reticle while in menu
            SetReticleVisible(false);
        }

        /// <summary>
        /// Show the controls menu and hide the other panels.
        /// </summary>
        public void ShowControlsMenu()
        {
            if (_mainMenuPanel == null ||
                _audioMenuPanel == null ||
                _controlsMenuPanel == null)
                return;

            SetActivePanel(_controlsMenuPanel, _mainMenuPanel, _audioMenuPanel);

            // Hide reticle while in menu
            SetReticleVisible(false);
        }

        private void TryBuildMenus()
        {
            if (_built)
                return;

            if (!_configured)
                return;

            if (_menuScene == null)
                return;

            if (_buttonTexture == null ||
                _sliderTrackTexture == null ||
                _sliderHandleTexture == null ||
                _controlsLayout == null ||
                _font == null)
                return;

            BuildPanels(_menuScene);
            _built = true;
       
            ShowMainMenu();
        }

        private void BuildPanels(Scene scene)
        {
            int backBufferWidth = Game.GraphicsDevice.PresentationParameters.BackBufferWidth;
            int backBufferHeight = Game.GraphicsDevice.PresentationParameters.BackBufferHeight;
            Vector2 viewportSize = new Vector2(backBufferWidth, backBufferHeight);

            // Basic layout: top-left-ish anchor + consistent item size
            Vector2 panelPosition = new Vector2((backBufferWidth - 390)/2, 200f);
            Vector2 itemSize = new Vector2(390, 96f);
            float spacing = 20f;

            // Main menu panel
            GameObject mainRoot = new GameObject(AppData.MAIN_PANEL_NAME);
            scene.Add(mainRoot);

            _mainMenuPanel = mainRoot.AddComponent<UIMenuPanel>();
            _mainMenuPanel.PanelPosition = panelPosition;
            _mainMenuPanel.ItemSize = itemSize;
            _mainMenuPanel.VerticalSpacing = spacing;
            _mainMenuPanel.IsVisible = true;

            if (_mainPanelBackground != null)
            {
                GameObject mainBgRoot = new GameObject(AppData.UI_MAIN_BG);
                scene.Add(mainBgRoot);
                mainBgRoot.Transform.SetParent(_mainMenuPanel.Transform);

                var mainBg = mainBgRoot.AddComponent<UITexture>();
                mainBg.Texture = _mainPanelBackground;
                mainBg.Size = viewportSize;        // cover screen
                mainBg.Position = Vector2.Zero;
                mainBg.Tint = Color.White;
                mainBg.LayerDepth = UILayer.MenuBack;  // above global dim, below buttons
            }

            _playButton = _mainMenuPanel.AddButton(
                AppData.PLAY_BUTTON_TEXT,
                _buttonTexture!,
                _font!,
                OnPlayClicked);

            _audioButton = _mainMenuPanel.AddButton(
                AppData.AUDIO_BUTTON_TEXT,
                _buttonTexture!,
                _font!,
                OnAudioClicked);

            _controlsButton = _mainMenuPanel.AddButton(
                AppData.CONTROLS_BUTTON_TEXT,
                _buttonTexture!,
                _font!,
                OnControlsClicked);

            _exitButton = _mainMenuPanel.AddButton(
        AppData.QUIT_BUTTON_TEXT,
        _buttonTexture!,
        _font!,
        OnExitClicked);

            // Tell the main panel to scan its hierarchy and register
            // all UITexture/UISelectable children (including the background).
            _mainMenuPanel.RefreshChildren();

            // -----------------------------------------------------------------
            // Audio menu panel
            // -----------------------------------------------------------------
            GameObject audioRoot = new GameObject(AppData.AUDIO_PANEL_NAME);
            scene.Add(audioRoot);

            _audioMenuPanel = audioRoot.AddComponent<UIMenuPanel>();
            _audioMenuPanel.PanelPosition = panelPosition;
            _audioMenuPanel.ItemSize = itemSize;
            _audioMenuPanel.VerticalSpacing = spacing;
            _audioMenuPanel.IsVisible = false;

            if (_audioPanelBackground != null)
            {
                GameObject audioBgRoot = new GameObject(AppData.UI_AUDIO_BG);
                scene.Add(audioBgRoot);
                audioBgRoot.Transform.SetParent(_audioMenuPanel.Transform);

                var audioBg = audioBgRoot.AddComponent<UITexture>();
                audioBg.Texture = _audioPanelBackground;
                audioBg.Size = viewportSize;
                audioBg.Position = Vector2.Zero;
                audioBg.Tint = Color.White;
                audioBg.LayerDepth = UILayer.MenuBack;
            }

            _musicSlider = _audioMenuPanel.AddSlider(
                AppData.MUSIC_SLIDER_TEXT,
                _sliderTrackTexture!,
                _sliderHandleTexture!,
                _font!,
                0f,
                1f,
                0.8f,
                OnMusicSliderChanged);

            _sfxSlider = _audioMenuPanel.AddSlider(
                AppData.SFX_SLIDER_TEXT,
                _sliderTrackTexture!,
                _sliderHandleTexture!,
                _font!,
                0f,
                1f,
                0.8f,
                OnSfxSliderChanged);

            _audioBackButton = _audioMenuPanel.AddButton(
                AppData.BACK_BUTTON_TEXT,
                _buttonTexture!,
                _font!,
                OnBackToMainFromAudio);

            // Register audio panel children (including its background)
            _audioMenuPanel.RefreshChildren();

            // -----------------------------------------------------------------
            // Controls menu panel
            // -----------------------------------------------------------------
            GameObject controlsRoot = new GameObject(AppData.CONTROL_PANEL_NAME);
            scene.Add(controlsRoot);

            _controlsMenuPanel = controlsRoot.AddComponent<UIMenuPanel>();
            _controlsMenuPanel.PanelPosition = panelPosition;
            _controlsMenuPanel.ItemSize = itemSize;
            _controlsMenuPanel.VerticalSpacing = spacing;
            _controlsMenuPanel.IsVisible = false;

            if (_controlsPanelBackground != null)
            {
                GameObject controlsBgRoot = new GameObject(AppData.UI_CONTROLS_BG);
                scene.Add(controlsBgRoot);
                controlsBgRoot.Transform.SetParent(_controlsMenuPanel.Transform);

                var controlsBg = controlsBgRoot.AddComponent<UITexture>();
                controlsBg.Texture = _controlsPanelBackground;
                controlsBg.Size = viewportSize;
                controlsBg.Position = Vector2.Zero;
                controlsBg.Tint = Color.White;
                controlsBg.LayerDepth = UILayer.MenuBack;
            }

            _controlsBackButton = _controlsMenuPanel.AddButton(
                AppData.BACK_BUTTON_TEXT,
                _buttonTexture!,
                _font!,
                OnBackToMainFromControls);

            // Already present, keep it
            _controlsMenuPanel.RefreshChildren();
        }

        /// <summary>
        /// Show the full menu (background + main menu).
        /// Use this when opening the menu from the game (e.g. Esc or on startup).
        /// </summary>
        public void ShowMenuRoot()
        {
            _menuVisible = true;
            Game.IsMouseVisible = true;

            ShowMainMenu();
        }

        /// <summary>
        /// Hides all menu panels and the background.
        /// Use this when resuming gameplay (Play button, Esc to close).
        /// </summary>
        public void HideMenus()
        {
            _menuVisible = false;
            Game.IsMouseVisible = false;

            if (_mainMenuPanel != null)
                _mainMenuPanel.IsVisible = false;

            if (_audioMenuPanel != null)
                _audioMenuPanel.IsVisible = false;

            if (_controlsMenuPanel != null)
                _controlsMenuPanel.IsVisible = false;

            if (!_gameStarted)
            {
                _gameStarted = true;
                _gameEvents.OnGameStarted.Raise();
            }

        }

        private void SetActivePanel(UIMenuPanel toShow, UIMenuPanel toHideA, UIMenuPanel toHideB)
        {
            toShow.IsVisible = true;
            toHideA.IsVisible = false;
            toHideB.IsVisible = false;

            _menuVisible = true;
        }
        private void OnPlayClicked()
        {
            PlayClickSound();
            PlayRequested?.Invoke();
        }

        private void OnAudioClicked()
        {
            PlayClickSound();
            ShowAudioMenu();
        }

        private void OnControlsClicked()
        {
            PlayClickSound();
            ShowControlsMenu();
        }

        private void OnExitClicked()
        {
            PlayClickSound();
            ExitRequested?.Invoke();
        }

        private void OnBackToMainFromAudio()
        {
            PlayClickSound();
            ShowMainMenu();
        }

        private void OnBackToMainFromControls()
        {
            PlayClickSound();
            ShowMainMenu();
        }

        private void OnMusicSliderChanged(float value)
        {
            MusicVolumeChanged?.Invoke(value);
        }

        private void OnSfxSliderChanged(float value)
        {
            SfxVolumeChanged?.Invoke(value);
        }

        /// <summary>
        /// Play UI click sound
        /// </summary>
        private void PlayClickSound()
        {
            // Get the audio event channel and trigger a UI click sound
            var audioEvents = EventChannelManager.Instance.AudioEvents;
            audioEvents.OnSFXRequested.Raise(AppData.UI_CLICK_SOUND_KEY);
        }
        #endregion

        #region Lifecycle Methods

        private void ToggleMenu()
        {
            if (IsMenuVisible)
                HideMenus();
            else
                ShowMenuRoot();

        }

        public override void Update(GameTime gameTime)
        {
            // Get new state
            _newKBState = Keyboard.GetState();

            // As a manager, this does not drive any Scene updates itself.
            // The DrawableGameComponent SceneManager should be responsible
            // for calling menuScene.Update/Draw and choosing which scene
            // is currently active.
            if (!_built && _configured)
                TryBuildMenus();

            ShowHideMenu();

            // Store old state (allows us to do was pressed type checks)
            _oldKBState = _newKBState;

            base.Update(gameTime);
        }

        private void ShowHideMenu()
        {
            if (_newKBState.IsKeyDown(Keys.Escape) && !_oldKBState.IsKeyDown(Keys.Escape))
            {
                if (IsMenuVisible)
                {
                    // _sceneManager.Paused = false;
                    HideMenus();
                }
                else
                {
                    // _sceneManager.Paused = true;
                    ShowMenuRoot();
                }
            }
        }

        /// <summary>
        /// Show or hide the HUD reticle and flip OS mouse visibility.
        /// Assumes the reticle GameObject is named "HUD" (created in Main.InitializeUIReticleRenderer).
        /// </summary>
        private void SetReticleVisible(bool visible)
        {
            if (_menuScene == null)
                return;

            // Find the HUD game object created in Main.InitializeUIReticleRenderer
            var hud = _menuScene.Find(go => go.Name == "HUD");
            if (hud == null)
                return;

            // Disable/enable specific UI components on the HUD so the reticle stops drawing and picking.
            var reticle = hud.GetComponent<UIReticle>();
            if (reticle != null)
                reticle.Enabled = visible;

            var text = hud.GetComponent<UIText>();
            if (text != null)
                text.Enabled = visible;

            var picker = hud.GetComponent<UIPickerInfo>();
            if (picker != null)
                picker.Enabled = visible;

            // If there are other UI renderers on the HUD you want toggled, either GetComponents<UIRenderer>()
            // and toggle them, or add explicit toggles above.
            var otherUIRenderers = hud.GetComponents<UIRenderer>();
            for (int i = 0; i < otherUIRenderers.Count; i++)
                otherUIRenderers[i].Enabled = visible;

            // Toggle OS mouse cursor — show mouse when menus visible, hide when playing.
            // Menu visible => reticle hidden => show OS mouse (so Game.IsMouseVisible = !visible)
            Game.IsMouseVisible = !visible;
        }

        #endregion

        #region Housekeeping Methods
        public override string ToString()
        {
            return "UIManager(MenuScene=" + (_menuScene?.Name ?? "null") + ", Built=" + (_built ? "true" : "false") + ")";
        }
        #endregion
    }
}
