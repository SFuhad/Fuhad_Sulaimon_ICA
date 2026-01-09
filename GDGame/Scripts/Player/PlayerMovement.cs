using System.Diagnostics;
using GDEngine.Core.Components;
using GDEngine.Core.Events;
using GDEngine.Core.Rendering.Base;
using GDEngine.Core.Timing;
using GDGame.Scripts.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SharpDX.Direct3D9;

namespace GDGame.Scripts.Player
{
    /// <summary>
    /// Controls the player physics movement.
    /// Created in <see cref="PlayerController"/>.
    /// Takes parts of code from <see cref="PhysicsWASDController"/>.
    /// Input keys gotten from <see cref="InputManager"/>
    /// </summary>
    public class PlayerMovement : Component
    {
        #region Fields
        private readonly float _moveSpeed = 15f;
        private RigidBody _rb;
        private BoxCollider _collider;
        private readonly LayerMask _playerLayerMask = LayerMask.All;
        private Keys _forwardKey, _rightKey, _backKey, _leftKey;
        private KeyboardState _keyboardState;
        #endregion

        #region Accessors
        public RigidBody RB => _rb;
        public BoxCollider Collider => _collider;
        #endregion

        #region Constructors
        public PlayerMovement()
        {
            InitMovementKeys();
            InitRB();
        }
        #endregion

        #region Methods

        /// <summary>
        /// Set the player movement keys from the Input Manager
        /// </summary>
        private void InitMovementKeys()
        {
            var keys = InputManager.MoveKeys;
            _rightKey = keys.right;
            _leftKey = keys.left;
            _backKey = keys.back;
            _forwardKey = keys.forward;
        }

        /// <summary>
        /// Create the Players Rigidbody
        /// </summary>
        private void InitRB()
        {
            _collider = new BoxCollider
            {
                Size = new Vector3(3,3,3),
            };

            _rb = new RigidBody
            {
                BodyType = BodyType.Dynamic,
            };
        }
        
        /// <summary>
        /// Logic To Move the Player modified from PhysicsWASDController
        /// </summary>
        /// <param name="moveDir">Direction to move in</param>
        /// <param name="speed">Speed to move at</param>
        private void Move(Vector3 moveDir, float speed)
        {
            var velocity = _rb.LinearVelocity;

            if (moveDir.LengthSquared() > 0f && speed > 0f)
            {
                moveDir.Normalize();

                // Set horizontal velocity; keep Y as-is.
                velocity.X = moveDir.X * speed;
                velocity.Z = moveDir.Z * speed;
            }
            else
            {
                // No movement input: stop horizontal motion, let damping & gravity handle the rest.
                velocity.X = 0f;
                velocity.Z = 0.1f;
            }

            _rb.LinearVelocity = velocity;
        }

        /// <summary>
        /// Handles the player movement logic
        /// </summary>
        private void HandleMovement()
        {

            // Set the Base Direction Vectors
            var forward = _rb.Transform.Forward;
            var right = _rb.Transform.Right;

            forward.Y = 0;
            right.Y = 0;

            if (forward.LengthSquared() > 0f)
                forward.Normalize();
            else
                forward = Vector3.Forward;

            if (right.LengthSquared() > 0f)
                right.Normalize();
            else
                right = Vector3.Right;

            _keyboardState = Keyboard.GetState();

            var moveDir = Vector3.Zero;

            // Apply Direction based on the input
            if (_keyboardState.IsKeyDown(_forwardKey))
                moveDir += forward;
            if (_keyboardState.IsKeyDown(_backKey))
                moveDir -= forward;
            if (_keyboardState.IsKeyDown(_rightKey))
                moveDir += right;
            if (_keyboardState.IsKeyDown(_leftKey))
                moveDir -= right;

            var speed = _moveSpeed;

            // Move the players rigid body
            Move(moveDir, speed);
        }
        #endregion

        #region Game Loop
        protected override void Update(float deltaTime)
        {
            HandleMovement();
        }
        #endregion

        #region Events
        /// <summary>
        /// Handle the players collision with objects in the world
        /// </summary>
        /// <param name="collision">Collision Event</param>
        public void HandlePlayerCollision(CollisionEvent collision)
        {
            // Early Exit if the Collsiion doesnt match the player layer mask
            // Or if it doesn't involve the player
            var a = collision.BodyA;
            var b = collision.BodyB;

            // Try keep A as player when calling events

            if (!collision.Matches(_playerLayerMask)) return;
            if (a != _rb && b != _rb) return;

            var colName = b.GameObject.Name;

            switch (colName)
            {
                case "Game_Over":
                    Debug.WriteLine("Game Over");
                    break;
                case "Game_Won":
                    Debug.WriteLine("Game Won");
                    break;
                default:
                    // Debug.WriteLine("Collison not Set Up");
                    break;
            }
        }
        #endregion
    }
}
