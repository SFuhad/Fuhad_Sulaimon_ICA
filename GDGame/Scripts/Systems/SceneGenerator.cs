using System;
using System.Collections.Generic;
using GDEngine.Core.Collections;
using GDEngine.Core.Components;
using GDEngine.Core.Entities;
using GDEngine.Core.Factories;
using GDEngine.Core.Rendering;
using GDGame.Demos.Controllers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Scripts.Systems
{
    /// <summary>
    /// Generates the base skybox and ground for the main scene
    /// </summary>
    public class SceneGenerator : IDisposable
    {
        #region Fields
        private ContentDictionary<Texture2D> _textures;
        private GameObject _skyBoxParent;
        private GraphicsDeviceManager _graphics;
        private Material _matBasicUnlit, _matBasicLit, _matBasicUnlitGround;

        private const int SKY_SCALE = 500;
        private const int GROUND_SCALE = 500;
        private Vector3 _skyScaleVector = new(SKY_SCALE, SKY_SCALE, 1);
        private bool disposedValue;
        private readonly Dictionary<string, Vector3> _skyPositions =
            new()
            {
                { "back",  new Vector3(0, 0, -SKY_SCALE / 2) },
                { "left",  new Vector3(-SKY_SCALE / 2, 0, 0) },
                { "right", new Vector3(SKY_SCALE / 2, 0, 0) },
                { "front", new Vector3(0, 0, SKY_SCALE / 2) },
                { "top",   new Vector3(0, SKY_SCALE / 2, 0) }
            };

        private readonly Dictionary<string, Vector3> _skyRotations =
            new()
            {
                { "back",  Vector3.Zero },
                { "left",  new Vector3(0, MathHelper.ToRadians(90), 0) },
                { "right", new Vector3(0, MathHelper.ToRadians(-90), 0) },
                { "front", new Vector3(0, MathHelper.ToRadians(180), 0) },
                { "top",   new Vector3(MathHelper.ToRadians(90), 0, MathHelper.ToRadians(90)) }
            };
        #endregion

        #region Constructors
        public SceneGenerator(ContentDictionary<Texture2D> tex, Material matUnlit, Material matLit, Material matGround
            , GraphicsDeviceManager graphics)
        {
            _textures = tex;
            _matBasicLit = matLit;
            _matBasicUnlit = matUnlit;
            _matBasicUnlitGround = matGround;
            _graphics = graphics;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Generate the Base Scene, involves the skybox and ground plane with collisions
        /// </summary>
        /// <param name="currentScene"></param>
        public void GenerateScene(Scene currentScene)
        {
            GenerateSkyBox(currentScene);
            GenerateGround(currentScene);
        }

        #endregion

        #region Private Methods
        /// <summary>
        /// Generate a Sky Box inside of a Parent Object that Rotates
        /// </summary>
        /// <param name="currentScene">The scene the sky box is being added to</param>
        private void GenerateSkyBox(Scene currentScene)
        {
            _skyBoxParent = new GameObject("SkyParent");
            var rot = _skyBoxParent.AddComponent<RotationController>();
            rot._rotationAxisNormalized = Vector3.Up;
            rot._rotationSpeedInRadiansPerSecond = MathHelper.ToRadians(2f);
            currentScene.Add(_skyBoxParent);

            currentScene.Add(CreateSkySegment(AppData.SKYBOX_BACK_NAME, _skyRotations["back"],
                _skyPositions["back"], AppData.SKYBOX_BACK_TEXTURE_KEY));

            currentScene.Add(CreateSkySegment(AppData.SKYBOX_LEFT_NAME, _skyRotations["left"],
                _skyPositions["left"], AppData.SKYBOX_LEFT_TEXTURE_KEY));

            currentScene.Add(CreateSkySegment(AppData.SKYBOX_RIGHT_NAME, _skyRotations["right"],
                _skyPositions["right"], AppData.SKYBOX_RIGHT_TEXTURE_KEY));

            currentScene.Add(CreateSkySegment(AppData.SKYBOX_FRONT_NAME, _skyRotations["front"],
                _skyPositions["front"], AppData.SKYBOX_FRONT_TEXTURE_KEY));

            currentScene.Add(CreateSkySegment(AppData.SKYBOX_SKY_NAME, _skyRotations["top"],
                _skyPositions["top"], AppData.SKYBOX_SKY_TEXTURE_KEY));
        }

        private GameObject CreateSkySegment(string name, Vector3 rotation, Vector3 position, string texture)
        {
            var skySegment = new GameObject(name);
            skySegment.Transform.ScaleTo(_skyScaleVector);
            skySegment.Transform.RotateEulerBy(rotation, true);
            skySegment.Transform.TranslateTo(position);

            var meshFilter = MeshFilterFactory.CreateQuadTexturedLit(_graphics.GraphicsDevice);
            skySegment.AddComponent(meshFilter);

            var meshRenderer = skySegment.AddComponent<MeshRenderer>();
            meshRenderer.Material = _matBasicLit;
            meshRenderer.Overrides.MainTexture = _textures.Get(texture);

            skySegment.Transform.SetParent(_skyBoxParent.Transform);

            return skySegment;
        }

        /// <summary>
        /// Generates a box ground with a collider in the given scene
        /// </summary>
        /// <param name="currentScene">Scene to add it to</param>
        private void GenerateGround(Scene currentScene)
        {
            GameObject gameObject = null;
            MeshFilter meshFilter = null;
            MeshRenderer meshRenderer = null;

            gameObject = new GameObject("ground");
            meshFilter = MeshFilterFactory.CreateQuadTexturedLit(_graphics.GraphicsDevice);

            meshFilter = MeshFilterFactory.CreateQuadGridTexturedUnlit(_graphics.GraphicsDevice,
                 1,
                 1,
                 1,
                 1,
                 20,
                 20);


            gameObject.Transform.ScaleBy(new Vector3(GROUND_SCALE, GROUND_SCALE, 1));
            gameObject.Transform.RotateEulerBy(new Vector3(MathHelper.ToRadians(-90), 0, 0), true);
            gameObject.Transform.TranslateTo(new Vector3(0, -0.5f, 0));

            gameObject.AddComponent(meshFilter);
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
            meshRenderer.Material = _matBasicUnlitGround;
            meshRenderer.Overrides.MainTexture = _textures.Get(AppData.SAND_TEXTURE);

            // Add a box collider matching the ground size
            var collider = gameObject.AddComponent<BoxCollider>();
            collider.Size = new Vector3(GROUND_SCALE, GROUND_SCALE, 0.025f);
            collider.Center = new Vector3(0, 0, -0.0125f);

            // Add rigidbody as Static (immovable)
            var rigidBody = gameObject.AddComponent<RigidBody>();
            rigidBody.BodyType = BodyType.Static;
            gameObject.IsStatic = true;

            currentScene.Add(gameObject);
        }

        private void Clear()
        {
            _textures?.Dispose();
            _textures = null;

            _matBasicLit?.Dispose();
            _matBasicLit = null;

            _matBasicUnlit?.Dispose();
            _matBasicUnlit = null;

            _matBasicUnlitGround?.Dispose();
            _matBasicUnlitGround = null;

            _graphics?.Dispose();
            _graphics = null;

        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposedValue) return;

            if (disposing)
                Clear();

            disposedValue = true;
        }

        ~SceneGenerator()
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
