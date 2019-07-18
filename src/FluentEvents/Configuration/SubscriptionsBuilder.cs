using FluentEvents.Subscriptions;

namespace FluentEvents.Configuration
{
    internal class SubscriptionsBuilder : ISubscriptionsBuilder
    {
        private readonly IGlobalSubscriptionsService _globalSubscriptionsService;
        private readonly IScopedSubscriptionsService _scopedSubscriptionsService;

        public SubscriptionsBuilder(
            IGlobalSubscriptionsService globalSubscriptionsService,
            IScopedSubscriptionsService scopedSubscriptionsService
        )
        {
            _globalSubscriptionsService = globalSubscriptionsService;
            _scopedSubscriptionsService = scopedSubscriptionsService;
        }

        public ServiceHandlerConfigurator<TService, TEvent> ServiceHandler<TService, TEvent>()
            where TService : class, IAsyncEventHandler<TEvent>
            where TEvent : class
        {
            return new ServiceHandlerConfigurator<TService, TEvent>(
                _scopedSubscriptionsService,
                _globalSubscriptionsService,
                false
            );
        }

        public ServiceHandlerConfigurator<TService, TEvent> OptionalServiceHandler<TService, TEvent>()
            where TService : class, IAsyncEventHandler<TEvent>
            where TEvent : class
        {
            return new ServiceHandlerConfigurator<TService, TEvent>(
                _scopedSubscriptionsService,
                _globalSubscriptionsService,
                true
            );
        }
    }
}