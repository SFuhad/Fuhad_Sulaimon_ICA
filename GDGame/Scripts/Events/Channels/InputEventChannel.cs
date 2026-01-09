namespace GDGame.Scripts.Events.Channels
{
    /// <summary>
    /// Controls Input Events in the game such as Toggling Pause, Language Swapping, Exit and Fullscreen.
    /// Uses <see cref="EventBase"/> to host events.
    /// </summary>
    public class InputEventChannel
    {
        public EventBase OnFullscreenToggle = new();
        public EventBase OnPauseToggle = new();
        public EventBase OnApplicationExit = new();
        public EventBase<int> OnMovementInput = new();
        public EventBase OnLanguageSwap = new();
        public EventBase OnIntroSkip = new();
        /// <summary>
        /// Unsubscribe from all events in the Channel
        /// </summary>
        public void ClearEventChannel()
        {
            OnFullscreenToggle.UnsubscribeAll();
            OnPauseToggle.UnsubscribeAll();
            OnApplicationExit.UnsubscribeAll();
            OnMovementInput.UnsubscribeAll();
            OnLanguageSwap.UnsubscribeAll();
            OnIntroSkip.UnsubscribeAll();
        }
    }
}
