using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using GDEngine.Core.Components;
using GDEngine.Core.Entities;
using GDGame.Scripts.Systems;

namespace GDGame.Scripts.DemoGame
{
    /// <summary>
    /// Triggers defeat when colliding with the player
    /// </summary>
    public class GameOverObject
    {
        private GameObject _go;
        private RigidBody _rb;
        private Vector3 _pos = new Vector3(55, 35, 50);
        private Vector3 _scale = new Vector3(20, 20, 20);

        public GameOverObject()
        {
            _go = ModelGenerator.Instance.GenerateCube(_pos, Vector3.Zero, _scale, "ground_grass", "Game_Over");
            _rb = new RigidBody();
            _go.AddComponent(_rb);
            var collider = new BoxCollider
            {
                Size = _scale
            };
            _go.AddComponent(collider);
            SceneController.AddToCurrentScene(_go);
        }
    }
}
