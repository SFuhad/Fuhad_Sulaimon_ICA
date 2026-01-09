using System;

namespace GDGame.Scripts.Events.Channels
{
    /// <summary>
    /// Used in <see cref="EventChannelManager"/> channels to store subscribers and raise events.
    /// Contains Raise, Subscribe and Unsubscribe functions with an <see cref="Action"/> as the event. 
    /// </summary>
    public class EventBase
    {
        private event Action Handlers;

        /// <summary>
        /// Call the Event
        /// </summary>
        public void Raise() => Handlers?.Invoke();

        /// <summary>
        /// Add a function to run when the event is called
        /// </summary>
        /// <param name="a">Function to Run</param>
        public void Subscribe(Action a) => Handlers += a;

        /// <summary>
        /// Remove a function from the list to be run when event is called
        /// </summary>
        /// <param name="a">Function to Remove</param>
        public void Unsubscribe(Action a) => Handlers -= a;

        /// <summary>
        /// Clear all functions from the event and set it to null
        /// </summary>
        public void UnsubscribeAll() => Handlers = null;
    }

    /// <summary>
    /// Used in <see cref="EventChannelManager"/> channels to store subscribers and raise events.
    /// Contains Raise, Subscribe and Unsubscribe functions with an <see cref="Action"/> as the event. 
    /// </summary>
    /// <typeparam name="T">Generic Type to pass through event</typeparam>
    public class EventBase<T>
    {
        private event Action<T> Handlers;

        /// <summary>
        /// Call the Event
        /// </summary>
        public void Raise(T var) => Handlers?.Invoke(var);

        /// <summary>
        /// Add a function to run when the event is called
        /// </summary>
        /// <param name="a">Function to Run</param>
        public void Subscribe(Action<T> a) => Handlers += a;

        /// <summary>
        /// Remove a function from the list to be run when event is called
        /// </summary>
        /// <param name="a">Function to Remove</param>
        public void Unsubscribe(Action<T> a) => Handlers -= a;

        /// <summary>
        /// Clear all functions from the event and set it to null
        /// </summary>
        public void UnsubscribeAll() => Handlers = null;
    }
}
