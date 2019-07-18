using System;

namespace FluentEvents.Configuration
{
    internal class PipelinesBuilder : IPipelinesBuilder
    {
        private readonly IServiceProvider _serviceProvider;

        public PipelinesBuilder(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public EventConfigurator<TEvent> Event<TEvent>()
            where TEvent : class
        {
            return new EventConfigurator<TEvent>(_serviceProvider);
        }
    }
}
