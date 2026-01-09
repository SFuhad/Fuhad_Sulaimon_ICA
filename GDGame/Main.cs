using System.Windows.Forms;
using GDEngine.Core;
using GDEngine.Core.Collections;
using GDEngine.Core.Entities;
using GDEngine.Core.Factories;
using GDEngine.Core.Screen;
using GDEngine.Core.Serialization;
using GDEngine.Core.Services;
using GDEngine.Core.Systems;
using GDEngine.Core.Timing;
using GDEngine.Core.Utilities;
using GDGame.Scripts.Audio;
using GDGame.Scripts.Demo_Game;
using GDGame.Scripts.DemoGame;
using GDGame.Scripts.Events.Channels;
using GDGame.Scripts.Player;
using GDGame.Scripts.Systems;
using GDGame.Scripts.Traps;
using GDGame.Scripts.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Color = Microsoft.Xna.Framework.Color;

namespace GDGame
{
    public class Main : Game
    {
        #region Fields

        // Core
        private readonly GraphicsDeviceManager _graphics;
        private PhysicsSystem _physicsSystem;
        private PhysicsDebugSystem _physicsDebugRenderer;
        private Vector2 _screenCentre;
        private bool _disposed = false;

        // Graphics 
        private SceneGenerator _sceneGenerator;
        private ModelGenerator _modelGenerator;
        private MaterialGenerator _materialGenerator;

        // Game
        private AudioController _audioController;
        private SceneController _sceneController;
        private UserInterfaceController _uiController;
        private readonly TrapManager _trapManager;
        private TimeController _timeController;
        private CinematicCamController _cineCamController;
        private GameStateManager _gameStateManager;
        private GameOverObject _gameOver;
        private GameWonObject _gameWon;

        // Player
        private PlayerController _playerController;
        private InputManager _inputManager;

        // Events
        private InputEventChannel _inputEventChannel;

        #endregion

        #region Constructor

        public Main()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = AppData.CONTENT_ROOT;
        }

        #endregion

        #region Initialisation

        protected override void Initialize()
        {
            Window.Title = AppData.GAME_WINDOW_TITLE;

            InitCoreSystems();
            InitGameSystems();

            base.Initialize();
        }

        #region Core Systems

        /// <summary>
        /// Init the Games Graphics such as DPI and Resolutions
        /// </summary>
        /// <param name="resolution"></param>
        private void InitGraphics(Integer2 resolution)
        {
            Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
            ScreenResolution.SetResolution(_graphics, resolution);
            WindowUtility.CenterOnMonitor(this, 1);
            _screenCentre = new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2);
        }

        /// <summary>
        /// Init the Mouse for the Game on launch
        /// </summary>
        private void InitMouse()
        {
            Mouse.SetPosition((int)_screenCentre.X, (int) _screenCentre.Y);
            IsMouseVisible = true;
        }

        /// <summary>
        /// Init the Engines Core Context which stores important systems
        /// </summary>
        private void InitContext()
        {
            EngineContext.Initialize(GraphicsDevice, Content);
        }

        /// <summary>
        /// Init the Localisation Controller which stores Keys for each language
        /// That can be accessed globally through its Instance
        /// </summary>
        private static void InitLocalisation()
        {
            LocalisationController.Initialise();
        }


        /// <summary>
        /// Generate the materials used on models and textures
        /// </summary>
        private void GenerateMaterials()
        {
            _materialGenerator = new MaterialGenerator(_graphics);
        }

        /// <summary>
        /// Init the Games base scene
        /// </summary>
        private void InitScene()
        {
            _sceneController = new SceneController(this);
            _sceneController.Initialise();
            Components.Add(_sceneController);
        }

        /// <summary>
        /// Init the Event Channel Manager and add the Event Bus to the scene
        /// </summary>
        private void InitEvents()
        {
            EventChannelManager.Initialise();
            _inputEventChannel = EventChannelManager.Instance.InputEvents;

            SceneController.AddToCurrentScene(new EventSystem(EngineContext.Instance.Events));
        }

        /// <summary>
        /// Load all the assets in the 'asset.manifest' JSON into Content Dictionaries and pass them through
        /// to the game controls
        /// </summary>
        /// <param name="relativeFilePathAndName">Path to the JSON File</param>
        private void LoadAssetsFromJSON(string relativeFilePathAndName)
        {
            var textures = new ContentDictionary<Texture2D>();
            var models = new ContentDictionary<Model>();
            var fonts = new ContentDictionary<SpriteFont>();
            var sounds = new ContentDictionary<SoundEffect>();

            var manifests = JSONSerializationUtility.LoadData<AssetManifest>(Content, relativeFilePathAndName);

            if (manifests.Count > 0)
            {
                foreach (var m in manifests)
                {
                    models.LoadFromManifest(m.Models, e => e.Name, e => e.ContentPath, overwrite: true);
                    textures.LoadFromManifest(m.Textures, e => e.Name, e => e.ContentPath, overwrite: true);
                    fonts.LoadFromManifest(m.Fonts, e => e.Name, e => e.ContentPath, overwrite: true);
                    sounds.LoadFromManifest(m.Sounds, e => e.Name, e => e.ContentPath, overwrite: true);
                }
            }

            _audioController = new AudioController(sounds);
            _uiController = new UserInterfaceController(EngineContext.Instance.SpriteBatch ,fonts, textures, _screenCentre, this);
            _sceneGenerator = new SceneGenerator(textures, _materialGenerator.MatBasicLit, _materialGenerator.MatBasicUnlit,
                _materialGenerator.MatBasicUnlitGround, _graphics);
            _modelGenerator = new ModelGenerator(textures, models, _materialGenerator.MatBasicUnlit, _graphics);
        }

        /// <summary>
        /// Init the Physics System in the current Scene
        /// </summary>
        private void InitPhysicsSystem()
        {
            _physicsSystem = new PhysicsSystem()
            {
                Gravity = AppData.GRAVITY
            };

            InitPhysicsDebugSystem(true);

            SceneController.GetCurrentScene.AddSystem(_physicsDebugRenderer);
            SceneController.GetCurrentScene.AddSystem(_physicsSystem);
        }

        /// <summary>
        /// Init the Physics Debug System which shows the boundaries for colliders in game
        /// </summary>
        /// <param name="isEnabled">Show Debug</param>
        private void InitPhysicsDebugSystem(bool isEnabled)
        {
            _physicsDebugRenderer = new PhysicsDebugSystem()
            {
                Enabled = isEnabled,
                StaticColor = Color.Green,
                KinematicColor = Color.Blue,
                DynamicColor = Color.Yellow,
                TriggerColor = Color.Red
            };
        }

        /// <summary>
        /// Init the Camera and Rendering Systems then add them to the current scene
        /// </summary>
        private void InitCameraAndRenderSystems()
        {
            var cameraSystem = new CameraSystem(_graphics.GraphicsDevice, -AppData.RENDER_ORDER);
            SceneController.GetCurrentScene.Add(cameraSystem);

            var renderSystem = new RenderSystem(-AppData.RENDER_ORDER);
            SceneController.GetCurrentScene.Add(renderSystem);

            var uiRenderSystem = new UIRenderSystem(AppData.RENDER_ORDER);
            SceneController.GetCurrentScene.Add(uiRenderSystem);
        }

        /// <summary>
        /// Load the objects from the multi model spawn JSON file
        /// </summary>
        private void LoadModelsFromJSON()
        {
            foreach (var m in JSONSerializationUtility.LoadData<ModelSpawnData>(Content, AppData.MULTI_MODEL_SPAWN_PATH))
            {
                var modelGO = _modelGenerator.GenerateModel(
                    m.Position, m.RotationDegrees, m.Scale, m.TextureName, m.ModelName, m.ObjectName);

                SceneController.GetCurrentScene.Add(modelGO);
            }
        }

        /// <summary>
        /// Init the Core Systems of the Game Engine
        /// </summary>
        private void InitCoreSystems()
        {
            InitGraphics(ScreenResolution.R_FHD_16_9_1920x1080);
            InitContext();
            InitMouse();
            GenerateMaterials();
            InitScene();
            InitEvents();
            InitPhysicsSystem();
            InitCameraAndRenderSystems();
            LoadAssetsFromJSON(AppData.ASSET_MANIFEST_PATH);
        }

        #endregion

        #region Game Systems

        /// <summary>
        /// Init the Audio System in the game which controls Music, Ambience and SFX
        /// </summary>
        private void InitAudioSystem()
        {
            _audioController.Initialise();
        }

        /// <summary>
        /// Init the Input System which stores all Key Variables and
        /// Passes events through the Input Event Channel when called
        /// </summary>
        private void InitInputSystem()
        {
            _inputManager = new InputManager();
            _inputManager.Initialise();
            _inputEventChannel.OnFullscreenToggle.Subscribe(HandleFullscreenToggle);
            _inputEventChannel.OnApplicationExit.Subscribe(HandleGameExit);
        }

        /// <summary>
        /// Generate the base skybox and floor through the Scene Generator
        /// </summary>
        private void GenerateBaseScene()
        {
            _sceneGenerator.GenerateScene(SceneController.GetCurrentScene);
        }

        /// <summary>
        /// Init the games User Interface
        /// </summary>
        private void InitUserInterface()
        {
            SceneController.AddToCurrentScene(new UIEventSystem());
            _uiController.Initialise(_playerController.Stats);
        }

        /// <summary>
        /// Init the Player Controller which contains the movement, camera and stats scripts
        /// </summary>
        private void InitPlayer()
        {
            var aspectRatio = (float)_graphics.PreferredBackBufferWidth / _graphics.PreferredBackBufferHeight;
            _playerController = new PlayerController(aspectRatio, _audioController);
            _playerController.Stats.StartTimer();
        }

        /// <summary>
        /// Init the Trap Manager which controls the creation and updating of
        /// Traps and Obstacles in the scene
        /// </summary>
        private static void InitTraps()
        {
            //_trapManager = new TrapManager();
            var trapManagerGO = new GameObject("TrapManagerGO");
            trapManagerGO.AddComponent<TrapManager>();
            SceneController.AddToCurrentScene(trapManagerGO);
        }

        /// <summary>
        /// Init the Time Controller which controls game time
        /// </summary>
        private void InitTime()
        {
            _timeController = new TimeController(_physicsDebugRenderer, _physicsSystem);
        }

        /// <summary>
        /// Initiate the Cinematic Camera Controller that creates the moving Camera intro
        /// that the player sees at the start of the game.
        /// </summary>
        private void InitCineCam()
        {
            var aspectRatio = (float)_graphics.PreferredBackBufferWidth / _graphics.PreferredBackBufferHeight;
            _cineCamController = new CinematicCamController(aspectRatio);
            _cineCamController.Initialise();
        }


        /// <summary>
        /// Initiate the Game State Manager
        /// </summary>
        private void InitGameStateManager()
        {
            _gameStateManager = new GameStateManager();
        }

        /// <summary>
        /// Create the Two Demo Objects in the scene that control Game Over and Game Won requirements
        /// </summary>
        private void InitDemoGameEvents()
        {
            _gameOver = new GameOverObject();
            _gameWon = new GameWonObject();
        }

        /// <summary>
        /// Init the Game Specific systems in the Engine
        /// </summary>
        private void InitGameSystems()
        {
            GenerateBaseScene();
            InitInputSystem();
            InitLocalisation();
            InitPlayer();
            InitUserInterface();
            InitAudioSystem();
            InitCineCam();
            InitTraps();
            InitTime();
            InitGameStateManager();
            LoadModelsFromJSON();
            InitDemoGameEvents();
        }

        #endregion

        #endregion

        #region Game Loop

        protected override void Update(GameTime gameTime)
        {
            Time.Update(gameTime);
            _playerController?.Update();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            base.Draw(gameTime);
        }

        #endregion

        #region Event Handlers

        private void HandleFullscreenToggle() => _graphics.ToggleFullScreen();
        private void HandleGameExit() => Application.Exit();

        #endregion

        #region Disposal

        /// <summary>
        /// Clear all of the event channels
        /// </summary>
        private static void UnsubscribeFromEvents()
        {
            EventChannelManager.Instance.ClearEventChannels();
        }

        /// <summary>
        /// Main Game Disposal
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (_disposed)
            {
                base.Dispose(disposing);
                return;
            }

            if (disposing)
            {
                System.Diagnostics.Debug.WriteLine("Disposing Main...");

                System.Diagnostics.Debug.WriteLine("Disposing UI");
                _uiController?.Dispose();
                _uiController = null;

                System.Diagnostics.Debug.WriteLine("Disposing Scene");
                _sceneController?.Dispose();
                _sceneController = null;

                System.Diagnostics.Debug.WriteLine("Disposing Graphics");
                MeshFilterFactory.ClearRegistry();
                
                _modelGenerator?.Dispose();
                _modelGenerator = null;

                _materialGenerator?.Dispose();
                _materialGenerator = null;

                _sceneGenerator?.Dispose();
                _sceneGenerator = null;

                System.Diagnostics.Debug.WriteLine("Disposing Physics");
                _physicsSystem?.Dispose();
                _physicsSystem = null;

                _physicsDebugRenderer = null;

                System.Diagnostics.Debug.WriteLine("Disposing Audio");
                _audioController?.Dispose();
                _audioController = null;

                System.Diagnostics.Debug.WriteLine("Disposing EngineContext");
                EngineContext.Instance?.Dispose();

                System.Diagnostics.Debug.WriteLine("Main disposal complete");
            }

            UnsubscribeFromEvents();
            _disposed = true;

            base.Dispose(disposing);
        }

        #endregion
    }
}
