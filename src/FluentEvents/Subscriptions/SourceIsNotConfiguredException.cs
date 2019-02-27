using System;

namespace FluentEvents.Subscriptions
{
    /// <summary>
    ///     An exception that is thrown when trying to subscribe to a source that
    ///     is not configured in the <see cref="EventsContext"/>.
    /// </summary>
    public class SourceIsNotConfiguredException : FluentEventsException
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="SourceIsNotConfiguredException" /> class.
        /// </summary>
        /// <param name="sourceType">The type of the source that is not configured.</param>
        public SourceIsNotConfiguredException(Type sourceType) 
            : base($"Events source with type \"{sourceType.FullName}\" is not configured")
        {
            
        }
    }
}