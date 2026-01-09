using System;
using System.Collections.Generic;
using GDEngine.Core.Components;
using GDEngine.Core.Entities;
using GDEngine.Core.Services;
using GDEngine.Core.Systems;
using GDEngine.Core.Timing;
using Microsoft.Xna.Framework;

namespace GDGame.Scripts.Systems
{
    /// <summary>
    /// Creates Scenes, Stores Scenes and allows other classes to add Systems and Game Objects to the 
    /// current scene.
    /// </summary>
    public class SceneController : DrawableGameComponent
    {
        #region Fields
        private Dictionary<string, Scene> _scenesDict;
        private static Scene _currentScene;
        #endregion

        #region Constructors
        public SceneController(Game game) : base (game) 
        { 
            _scenesDict = new();
        }
        #endregion

        #region Accessors
        public Scene CurrentScene => _currentScene;
        public static Scene GetCurrentScene=> _currentScene;
        #endregion

        #region Methods
        /// <summary>
        /// Initialise the base scene of the game (Only 1 for now) 
        /// </summary>
        public void Initialise()
        {
            CreateScene(AppData.MAIN_SCENE_NAME);
            SetCurrentScene(AppData.MAIN_SCENE_NAME);
        }

        /// <summary>
        /// Create a scene and add it to the dictionary
        /// </summary>
        /// <param name="name">Name of the Scene to Create</param>
        /// <returns>Returns the created Scene</returns>
        private Scene CreateScene(string name)
        {
            if (_scenesDict.TryGetValue(name, out var existing))
                return existing;

            var s = new Scene(EngineContext.Instance, name);
            _scenesDict.Add(name, s);
            return s;
        }

        /// <summary>
        /// Set the current scene in the game
        /// </summary>
        /// <param name="name">Name of the Scene</param>
        /// <exception cref="InvalidOperationException">The Scene Doesn't Exist</exception>
        private void SetCurrentScene(string name)
        {
            if (_scenesDict.TryGetValue(name, out var existing))
                _currentScene = existing;
            else
                throw new InvalidOperationException($"Scene '{name}' doest not exist.");
        }

        /// <summary>
        /// Add a Game Object to the Current Scene
        /// </summary>
        /// <param name="go">Game Object to Add</param>
        public static void AddToCurrentScene(GameObject go) => _currentScene.Add(go);

        /// <summary>
        /// Add a Game System to the Current Scene
        /// </summary>
        /// <param name="sys">Game System to Add</param>
        public static void AddToCurrentScene(SystemBase sys) => _currentScene.Add(sys);

        /// <summary>
        /// Set the Active Camera in the Current Scene
        /// </summary>
        /// <param name="cam">Camera to Set</param>
        public static void SetActiveCamera(string camName)
        {
            var go = _currentScene?.Find(go => go.Name.Equals(camName));
            var _camera = go?.GetComponent<Camera>();
            if (_camera != null)
                _currentScene?.SetActiveCamera(_camera);
        }

        /// <summary>
        /// Set the active camera in the current scene
        /// </summary>
        /// <param name="camera">Camera to Set</param>
        public static void SetActiveCamera(Camera camera) => _currentScene?.SetActiveCamera(camera);


        public override void Update(GameTime gameTime)
        {
            if (!Enabled)
            {
                base.Update(gameTime);
                return;
            }

            _currentScene?.Update(Time.DeltaTimeSecs);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (!Visible)
            {
                base.Draw(gameTime);
                return;
            }

            _currentScene?.Draw(Time.DeltaTimeSecs);

            base.Draw(gameTime);
        }

        #endregion
    }
}
