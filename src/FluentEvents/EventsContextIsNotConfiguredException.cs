using System;
using Microsoft.Extensions.DependencyInjection;

namespace FluentEvents
{
    /// <summary>
    ///     An exception thrown when the <see cref="EventsContext"/> wasn't configured
    ///     with the <see cref="ServiceCollectionExtensions.AddEventsContext{TEventsContext}"/> extension method
    ///     or the <see cref="EventsContext(EventsContextOptions, IServiceProvider)"/> constructor.
    /// </summary>
    public class EventsContextIsNotConfiguredException : FluentEventsException
    {
        /// <summary>
        ///     Creates a new instance of <see cref="EventsContextIsNotConfiguredException"/>.
        /// </summary>
        public EventsContextIsNotConfiguredException() 
            : base($"Please configure the {nameof(EventsScope)} with the {nameof(IServiceCollection)} extension method or use the constructor with arguments.")
        {
        }
    }
}