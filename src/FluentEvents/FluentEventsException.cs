using System;
using System.Runtime.Serialization;

namespace FluentEvents
{
    /// <inheritdoc />
    /// <summary>
    ///     A base exception inherited by all exceptions thrown by FluentEvents (Except for <see cref="AggregateException"/>s).
    /// </summary>
    [Serializable]
    public abstract class FluentEventsException : Exception
    {
        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="FluentEventsException" /> class.
        /// </summary>
        protected FluentEventsException()
        {
        }

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="FluentEventsException" /> class.
        /// </summary>
        protected FluentEventsException(string message) : base(message)
        {
        }

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:FluentEvents.FluentEventsException" /> class.
        /// </summary>
        protected FluentEventsException(string message, Exception inner) : base(message, inner)
        {
        }

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="FluentEventsException" /> class.
        /// </summary>
        protected FluentEventsException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
