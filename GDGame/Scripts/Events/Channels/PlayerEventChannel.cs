using BepuPhysics.CollisionDetection.CollisionTasks;
using GDEngine.Core.Events;
using GDGame.Scripts.Systems;

namespace GDGame.Scripts.Events.Channels
{
    /// <summary>
    /// Controls Player Events in the game such as Winning, Losing and Game State Changes.
    /// Uses <see cref="EventBase"/> to host events.
    /// </summary>
    public class PlayerEventChannel
    {
        public EventBase OnPlayerLose = new();
        public EventBase OnPlayerWin = new();
        public EventBase<GameState> OnGameStateChange = new();
        public EventBase<int> OnPlayerDamaged = new();
        public EventBase OnOrbCollected = new();
        public EventBase<CollisionEvent> OnPlayerCollision = new();

        /// <summary>
        /// Unsubscribe from all events in the Channel
        /// </summary>
        public void ClearEventChannel()
        {
            OnPlayerLose.UnsubscribeAll();
            OnPlayerWin.UnsubscribeAll();
            OnGameStateChange.UnsubscribeAll();
            OnOrbCollected.UnsubscribeAll();
            OnPlayerDamaged.UnsubscribeAll();
            OnPlayerCollision.UnsubscribeAll();
        }
    }
}
