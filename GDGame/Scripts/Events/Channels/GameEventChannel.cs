using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDGame.Scripts.Events.Channels
{
    /// <summary>
    /// Game Event Channel for Starting, Pausing and Unpausing. 
    /// Unused at the moment.
    /// </summary>
    public class GameEventChannel
    {
        public EventBase OnGameStarted = new();
        public EventBase OnGamePaused = new();
        public EventBase OnGameUnpaused = new();

        /// <summary>
        /// Unsubscribe from all events in the Channel
        /// </summary>
        public void ClearEventChannel()
        {
            OnGameStarted.UnsubscribeAll();
            OnGamePaused.UnsubscribeAll();
            OnGameUnpaused.UnsubscribeAll();
        }
    }
}
