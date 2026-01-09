using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GDEngine.Core.Components;
using GDEngine.Core.Entities;
using GDGame.Scripts.Systems;
using Microsoft.Xna.Framework;

namespace GDGame.Scripts.Demo_Game
{
    /// <summary>
    /// Triggers victory when colliding with the player
    /// </summary>
    public class GameWonObject
    {
        private GameObject _go;
        private RigidBody _rb;
        private Vector3 _pos = new Vector3(-55, 35, -50);
        private Vector3 _scale = new Vector3(20, 20, 20);

        public GameWonObject()
        {
            _go = ModelGenerator.Instance.GenerateCube(_pos, Vector3.Zero, _scale, "ground_grass", "Game_Won");
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
