using System;
using GDEngine.Core.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Scripts.Systems
{
    /// <summary>
    /// Creates the game materials
    /// </summary>
    public class MaterialGenerator : IDisposable
    {
        #region Fields
        private Material _matBasicUnlit, _matBasicLit, _matBasicUnlitGround;
        private bool disposedValue;
        #endregion

        #region Constructors
        public MaterialGenerator(GraphicsDeviceManager graphics) 
        {
            #region Unlit Textured BasicEffect 
            var unlitBasicEffect = new BasicEffect(graphics.GraphicsDevice)
            {
                TextureEnabled = true,
                LightingEnabled = false,
                VertexColorEnabled = false
            };

            _matBasicUnlit = new Material(unlitBasicEffect);
            _matBasicUnlit.StateBlock = RenderStates.Opaque3D();      // depth on, cull CCW
            _matBasicUnlit.SamplerState = SamplerState.LinearClamp;   // helps avoid texture seams on sky

            //ground texture where UVs above [0,0]-[1,1]
            _matBasicUnlitGround = new Material(unlitBasicEffect.Clone());
            _matBasicUnlitGround.StateBlock = RenderStates.Opaque3D();      // depth on, cull CCW
            _matBasicUnlitGround.SamplerState = SamplerState.AnisotropicWrap;   // wrap texture based on UV values

            #endregion

            #region Lit Textured BasicEffect 
            var litBasicEffect = new BasicEffect(graphics.GraphicsDevice)
            {
                TextureEnabled = true,
                LightingEnabled = true,
                PreferPerPixelLighting = true,
                VertexColorEnabled = false
            };

            litBasicEffect.EnableDefaultLighting();
            _matBasicLit = new Material(litBasicEffect);
            _matBasicLit.StateBlock = RenderStates.Opaque3D();
            #endregion
        }
        #endregion

        #region Accessors
        public Material MatBasicLit => _matBasicLit;
        public Material MatBasicUnlit => _matBasicUnlit;
        public Material MatBasicUnlitGround => _matBasicUnlitGround;

        private void Clear()
        {
            _matBasicLit?.Dispose();
            _matBasicLit = null;

            _matBasicUnlit?.Dispose();
            _matBasicUnlit = null;

            _matBasicUnlitGround?.Dispose();
            _matBasicUnlitGround = null;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposedValue) return;

            if (disposing)
                Clear();

            disposedValue = true;
        }

        ~MaterialGenerator()
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
