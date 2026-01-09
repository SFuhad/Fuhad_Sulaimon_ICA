using GDEngine.Core.Components;
using GDGame.Scripts.Systems;
using Microsoft.Xna.Framework;
using System;

namespace GDGame.Scripts.Traps
{
    /// <summary>
    /// Moving Obstacle Trap which inherits from <see cref="TrapBase"/>.
    /// </summary>
    public class MovingTrap : TrapBase
    {
        #region Fields
        private float _moveSpeed = 5f;
        private Vector3 _startPosition = new Vector3(0, 0, 0);
        #endregion

        #region Constructors
        public MovingTrap(int id, float moveSpeed) : base(id)
        {
            _trapGO = ModelGenerator.Instance.GenerateCube(new Vector3(10, 10, 10), Vector3.Zero, new Vector3(10, 10, 10), "ground_grass", AppData.TRAP_NAME + id);
            _trapGO.AddComponent<BoxCollider>();
            SceneController.AddToCurrentScene(_trapGO);
            _moveSpeed = moveSpeed;
        }
        #endregion

        #region Methods
        public override void UpdateTrap()
        {
            _trapGO.Transform.TranslateBy(new Vector3(0, _moveSpeed, 0));
            if (_trapGO.Transform.Position.Y > _startPosition.Y+5f || _trapGO.Transform.Position.Y < _startPosition.Y -5f)
            {
                flip();
            }
        }

        public override void InitTrap()
        {
            _startPosition = _trapGO.Transform.Position;
        }

        public override void HandlePlayerHit()
        {
            throw new NotImplementedException();
        }
        #endregion

        public void flip()
        {
            _moveSpeed = -_moveSpeed;
        }
    }
}
