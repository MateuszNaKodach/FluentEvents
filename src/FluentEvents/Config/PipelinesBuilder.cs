using System;
using FluentEvents.Model;

namespace FluentEvents.Config
{
    /// <summary>
    ///     Provides a simple API surface to select an event and configure it fluently.
    /// </summary>
    public class PipelinesBuilder
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        ///     This API supports the FluentEvents infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public PipelinesBuilder(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        ///     Returns an object that can be used to configure fluently how the event is handled.
        ///     This method can be called multiple times for the same event to
        ///     configure multiple pipelines.
        /// </summary>
        /// <typeparam name="TEvent">The type of the event.</typeparam>
        /// <returns>The configuration object for the specified event.</returns>
        public EventConfigurator<TEvent> Event<TEvent>()
            where TEvent : class
        {
            return new EventConfigurator<TEvent>(_serviceProvider);
        }
    }
}
