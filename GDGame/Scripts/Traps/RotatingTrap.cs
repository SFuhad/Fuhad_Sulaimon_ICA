using GDEngine.Core.Components;
using GDEngine.Core.Entities;
using GDGame.Scripts.Systems;
using Microsoft.Xna.Framework;
using System;

namespace GDGame.Scripts.Traps
{
    /// <summary>
    /// Moving Obstacle Trap which inherits from <see cref="TrapBase"/>.
    /// </summary>
    public class RotatingTrap : TrapBase
    {
        #region Fields
        private float _rotSpeed = 5f;
        private float _startAngle = -90f;
        private float _endAngle = 90f;
        private float _currentAngle = 0f;
        private bool _rotatingClockwise = true;
        #endregion

        #region Constructors
        public RotatingTrap(int id, float rotSpeed) : base(id)
        {
            _trapGO = ModelGenerator.Instance.GenerateCube(
                new Vector3(-3, 5, 0),
                Vector3.Zero,
                new Vector3(0.5f, 0.5f, 0.5f),
                "ground_grass",
                AppData.TRAP_NAME + id);

            _trapModelGO = ModelGenerator.Instance.GenerateModel(
                Vector3.Zero, Vector3.Zero, new Vector3(3,3,3),
                "Guilitinne_openPBR_shader1_BaseColor",
                "Guilitinne_final",
                AppData.TRAP_NAME + id);

            _trapModelGO.AddComponent<BoxCollider>();

            SceneController.AddToCurrentScene(_trapGO);
            //SceneController.AddToCurrentScene(_trapModelGO);


            // Make sure parent has no scale applied
            _trapGO.Transform.ScaleTo(Vector3.One);

            // Then generate the model with normal scale
            _trapModelGO.Transform.TranslateTo (new Vector3(1, 1, 1));

            // Then parent it
            _trapModelGO.Transform.SetParent(_trapGO.Transform);

            // now place the model locally
            _trapModelGO.Transform.TranslateTo(
                new Vector3(-118.5f, 0.05f, 4)
                //new Vector3(0, 10, 0)
                );

            // Only add the parent to the scene

            _rotSpeed = rotSpeed;
        }
        #endregion

        #region Methods
        public override void UpdateTrap()
        {
            _trapGO.Transform.RotateBy(Quaternion.CreateFromAxisAngle(Vector3.Forward, MathHelper.ToRadians(_rotSpeed)));
            if (_rotatingClockwise)
            {
                _currentAngle++;
            }
            else
            {
                _currentAngle--;
            }

            if (_currentAngle >= _endAngle)
            {
                _rotatingClockwise = false;
                flip();
            }
            if (_currentAngle <= _startAngle)
            {
                _rotatingClockwise = true;
                flip();
            }


        }

        public override void InitTrap()
        {
            
        }

        public override void HandlePlayerHit()
        {
            throw new NotImplementedException();
        }
        #endregion

        public void flip()
        {
            _rotSpeed = -_rotSpeed;
        }
    }
}
