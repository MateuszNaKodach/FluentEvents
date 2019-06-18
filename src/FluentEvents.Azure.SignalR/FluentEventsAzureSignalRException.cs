using System;

namespace FluentEvents.Azure.SignalR
{
    /// <inheritdoc />
    /// <summary>
    ///     An exception thrown by the FluentEvents.Azure.SignalR plugin.
    /// </summary>
    public abstract class FluentEventsAzureSignalRException : FluentEventsException
    {
        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="FluentEventsAzureSignalRException" /> class.
        /// </summary>
        protected FluentEventsAzureSignalRException(string message) : base(message)
        {
        }

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="FluentEventsAzureSignalRException" /> class.
        /// </summary>
        protected FluentEventsAzureSignalRException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
