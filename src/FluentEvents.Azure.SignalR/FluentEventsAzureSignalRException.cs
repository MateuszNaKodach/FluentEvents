using System;

namespace FluentEvents.Azure.SignalR
{
    /// <summary>
    ///     An exception thrown by the FluentEvents.Azure.SignalR plugin.
    /// </summary>
    [Serializable]
    public abstract class FluentEventsAzureSignalRException : FluentEventsException
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="FluentEventsAzureSignalRException" /> class.
        /// </summary>
        protected FluentEventsAzureSignalRException(string message) : base(message)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="FluentEventsAzureSignalRException" /> class.
        /// </summary>
        protected FluentEventsAzureSignalRException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
