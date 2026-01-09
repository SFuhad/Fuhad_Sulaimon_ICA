using System.Diagnostics;
using GDEngine.Core.Components;
using GDEngine.Core.Entities;
using GDEngine.Core.Timing;
using GDGame.Scripts.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GDGame.Scripts.Player
{
    /// <summary>
    /// Controls the player camera movement. Code taken from <see cref="MouseYawPitchController"/>. 
    /// Created in <see cref="PlayerController"/>.
    /// </summary>
    public class PlayerCamera : Component
    {
        #region Fields
        private readonly Camera _camera;
        private readonly float _cameraFOV = 0.9f;
        private const int FAR_PLANE_LIMIT = 1000;

        private MouseState _newMouseState;
        private Quaternion _lastPos;
        private MouseState _oldMouseState;

        private float _mouseSensitivity = 0.4f;

        // Accumulated yaw/pitch in radians.
        // Yaw: rotation around world Y axis.
        // Pitch: rotation around local right axis (clamped).
        private float _yaw;
        private float _pitch;

        private bool _initializedAngles;
        #endregion

        #region Constructors
        public PlayerCamera(float aspectRatio)
        {
            _camera = new()
            {
                FarPlane = FAR_PLANE_LIMIT,
                AspectRatio = aspectRatio,
                FieldOfView = _cameraFOV,
            };
        }
        #endregion

        #region Accessors
        public Camera Cam => _camera;
        #endregion

        #region Game Loop
        protected override void Awake()
        {
            _oldMouseState = Mouse.GetState();
            InitializeAnglesFromCurrentTransform();
        }
        protected override void Update(float deltaTime)
        {
            HandleCameraMovement();
        }
        #endregion

        #region Methods

        /// <summary>
        /// Logic for moving the camera to the mouse position.
        /// Code taken from MousePitchYawController.
        /// </summary>
        private void HandleCameraMovement()
        {
            _newMouseState = Mouse.GetState();

            float dX = _newMouseState.X - _oldMouseState.X;
            float dY = _newMouseState.Y - _oldMouseState.Y;

            float yawDelta = dX * _mouseSensitivity * Time.DeltaTimeSecs;
            float pitchDelta = dY * _mouseSensitivity * Time.DeltaTimeSecs;

            _yaw -= yawDelta;
            _pitch -= pitchDelta;

            const float maxPitchDeg = 89f;
            float maxPitchRad = MathHelper.ToRadians(maxPitchDeg);
            _pitch = MathHelper.Clamp(_pitch, -maxPitchRad, maxPitchRad);

            Quaternion desiredWorldRotation = Quaternion.CreateFromYawPitchRoll(_yaw, _pitch, 0f);

            Quaternion currentWorldRotation = Transform.Rotation;
            Quaternion invCurrent = Quaternion.Inverse(currentWorldRotation);
            Quaternion delta = Quaternion.Normalize(Quaternion.Concatenate(desiredWorldRotation, invCurrent));

            Transform.RotateBy(delta, worldSpace: true);
            _lastPos = delta;

            _oldMouseState = _newMouseState;
        }

        /// <summary>
        /// Initialize yaw/pitch from the current Transform.Forward, so that
        /// mouse-look starts from the existing camera orientation.
        /// </summary>
        private void InitializeAnglesFromCurrentTransform()
        {
            if (Transform == null || _initializedAngles)
                return;

            Vector3 f = Vector3.Normalize(Transform.Forward);

            _pitch = (float)System.Math.Asin(f.Y);
            _yaw = (float)System.Math.Atan2(f.X, -f.Z);
            _initializedAngles = true;
        }
        #endregion

    }
}
