using System.Diagnostics;
using GDEngine.Core.Components;
using GDEngine.Core.Entities;
using GDEngine.Core.Systems;
using GDEngine.Core.Timing;
using GDGame.Scripts.Events.Channels;

namespace GDGame.Scripts.Systems
{
    /// <summary>
    /// Controls the time of the game using <see cref="Time.TimeScale"/>.
    /// Should pause the <see cref="PhysicsSystem"/> but its bugged so for now 
    /// it is just a static bool we can check.
    /// </summary>
    public class TimeController : Component
    {
        #region Fields
        private static bool _isPaused;
        private PhysicsDebugSystem _physicsDebugSystem;
        private PhysicsSystem _physicsSystem;
        private InputEventChannel _inputEventsChannel;
        private GameEventChannel _gameEventsChannel;
        #endregion

        #region Constructors
        public TimeController(PhysicsDebugSystem pds, PhysicsSystem ps) 
        {
            _physicsSystem = ps;
            _physicsDebugSystem = pds;
            _gameEventsChannel = EventChannelManager.Instance.GameEvents;
            _inputEventsChannel = EventChannelManager.Instance.InputEvents;
        }
        #endregion

        #region Accessors
        public static bool IsPaused => _isPaused;
        #endregion

        #region Methods
        #endregion
    }
}
