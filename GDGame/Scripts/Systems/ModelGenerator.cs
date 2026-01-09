using GDEngine.Core.Collections;
using GDEngine.Core.Components;
using GDEngine.Core.Entities;
using GDEngine.Core.Factories;
using GDEngine.Core.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace GDGame.Scripts.Systems
{
    /// <summary>
    /// Creates Models off of given data
    /// </summary>
    public class ModelGenerator : IDisposable
    {
        #region Fields
        private static ModelGenerator _instance;
        private ContentDictionary<Texture2D> _textures;
        private ContentDictionary<Model> _models;
        private Material _material;
        private GraphicsDeviceManager _graphics;
        private bool disposedValue;
        #endregion

        #region Constructors
        public ModelGenerator(ContentDictionary<Texture2D> tex, ContentDictionary<Model> models,
            Material mat, GraphicsDeviceManager g) 
        { 
            _textures = tex;
            _models = models;
            _material = mat;
            _graphics = g;
            Initialise();
        }
        #endregion

        #region Accessors
        public static ModelGenerator Instance
        {
            get
            {
                if (_instance == null)
                    throw new InvalidOperationException("Ensure you call Initialise() first");

                return _instance;
            }
        }
        #endregion

        #region Methods

        private void Initialise()
        {
            if (_instance != null) return;

            _instance = this;
        }

        /// <summary>
        /// Generate a GameObject with a textured model attached
        /// </summary>
        /// <param name="position">Spawn Position</param>
        /// <param name="eulerRotationDegrees">Spawn Rotation</param>
        /// <param name="scale">Spawn Scale</param>
        /// <param name="textureName">Name of the Texture</param>
        /// <param name="modelName">Name of the Model</param>
        /// <param name="objectName">Name of the Game Object</param>
        /// <returns></returns>
        public GameObject GenerateModel( Vector3 position,
            Vector3 eulerRotationDegrees, Vector3 scale,
            string textureName, string modelName, string objectName)
        {
            GameObject gameObject = null;

            gameObject = new GameObject(objectName);
            gameObject.Transform.TranslateTo(position);
            gameObject.Transform.RotateEulerBy(eulerRotationDegrees * MathHelper.Pi / 180f);
           // gameObject.Transform.ScaleTo(scale / 100);

            var model = _models.Get(modelName);
            var texture = _textures.Get(textureName);
            var meshFilter = MeshFilterFactory.CreateFromModel(model, _graphics.GraphicsDevice, 0, 0);
            gameObject.AddComponent(meshFilter);

            var meshRenderer = gameObject.AddComponent<MeshRenderer>();

            meshRenderer.Material = _material;
            meshRenderer.Overrides.MainTexture = texture;

            var rb = new RigidBody
            {
                BodyType = BodyType.Static,
            };
            gameObject.AddComponent(rb);

            var collider = new BoxCollider
            {
                Size = scale
            };
            gameObject.AddComponent(collider);

            return gameObject;
        }

        /// <summary>
        /// Generate a GameObject with a textured cube attached
        /// </summary>
        /// <param name="position">Spawn Position</param>
        /// <param name="eulerRotationDegrees">Spawn Rotation</param>
        /// <param name="scale">Spawn Scale</param>
        /// <param name="textureName">Name of the Texture</param>
        /// <param name="objectName">Name of the Game Object</param>
        /// <returns></returns>
        public GameObject GenerateCube(Vector3 position,
            Vector3 eulerRotationDegrees, Vector3 scale,
            string textureName, string objectName)
        {
            GameObject gameObject = null;

            gameObject = new GameObject(objectName);
            gameObject.Transform.TranslateTo(position);
            gameObject.Transform.RotateEulerBy(eulerRotationDegrees * MathHelper.Pi / 180f);
            gameObject.Transform.ScaleTo(scale);

            var texture = _textures.Get(textureName);
            var meshFilter = MeshFilterFactory.CreateCubeTextured(_graphics.GraphicsDevice);
            gameObject.AddComponent(meshFilter);

            var meshRenderer = gameObject.AddComponent<MeshRenderer>();

            meshRenderer.Material = _material;
            meshRenderer.Overrides.MainTexture = texture;

            var rb = new RigidBody
            {
                BodyType = BodyType.Static,
            };
            gameObject.AddComponent(rb);

            var collider = new BoxCollider
            {
                Size = scale
            };
            gameObject.AddComponent(collider);

            return gameObject;
        }

        private void Clear()
        {
            _textures?.Dispose();
            _textures = null;

            _models?.Dispose();
            _models = null;

            _material?.Dispose();
            _material = null;

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

        ~ModelGenerator()
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
