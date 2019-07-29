using System;

namespace FluentEvents
{
    /// <summary>
    ///     A base exception inherited by all exceptions thrown by FluentEvents (Except for <see cref="AggregateException"/>s).
    /// </summary>
    [Serializable]
    public abstract class FluentEventsException : Exception
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="FluentEventsException" /> class.
        /// </summary>
        protected FluentEventsException(string message) : base(message)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="FluentEventsException" /> class.
        /// </summary>
        protected FluentEventsException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
