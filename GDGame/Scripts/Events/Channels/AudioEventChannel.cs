namespace GDGame.Scripts.Events.Channels
{
    /// <summary>
    /// Controls Audio Events in the game such as Playing Music and Sound Effects.
    /// Uses <see cref="EventBase"/> to host events.
    /// </summary>
    public class AudioEventChannel
    {
        public EventBase<string> OnMusicRequested = new();
        public EventBase<string> OnSFXRequested = new();
        public EventBase<float> OnMusicVolumeChanged = new();
        public EventBase<float> OnSFXVolumeChanged = new();

        /// <summary>
        /// Unsubscribe from all events in the Channel
        /// </summary>
        public void ClearEventChannel()
        {
            OnMusicRequested.UnsubscribeAll();
            OnSFXRequested.UnsubscribeAll();
            OnMusicVolumeChanged.UnsubscribeAll();
            OnSFXVolumeChanged.UnsubscribeAll();
        }
    }
}
