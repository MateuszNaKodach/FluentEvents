using FluentEvents.Subscriptions;

namespace FluentEvents.Configuration
{
    /// <summary>
    ///     Provides an API surface to configure the subscriptions of a service event handler.
    /// </summary>
    public class ServiceHandlerConfiguration<TService, TEvent>
        where TService : class, IAsyncEventHandler<TEvent>
        where TEvent : class
    {
        private readonly IScopedSubscriptionsService _scopedSubscriptionsService;
        private readonly IGlobalSubscriptionsService _globalSubscriptionsService;
        private readonly bool _isOptional;

        internal ServiceHandlerConfiguration(
            IScopedSubscriptionsService scopedSubscriptionsService, 
            IGlobalSubscriptionsService globalSubscriptionsService,
            bool isOptional
        )
        {
            _scopedSubscriptionsService = scopedSubscriptionsService;
            _globalSubscriptionsService = globalSubscriptionsService;
            _isOptional = isOptional;
        }

        /// <summary>
        ///     Subscribes the <see cref="IAsyncEventHandler{TEvent}.HandleEventAsync"/> to global events.
        /// </summary>
        /// <returns>The configuration object to add more subscriptions.</returns>
        public ServiceHandlerConfiguration<TService, TEvent> HasGlobalSubscription()
        {
            _globalSubscriptionsService.AddGlobalServiceHandlerSubscription<TService, TEvent>(_isOptional);

            return this;
        }

        /// <summary>
        ///     Subscribes the <see cref="IAsyncEventHandler{TEvent}.HandleEventAsync"/> to scoped events.
        /// </summary>
        /// <returns>The configuration object to add more subscriptions.</returns>
        public ServiceHandlerConfiguration<TService, TEvent> HasScopedSubscription()
        {
            _scopedSubscriptionsService.ConfigureScopedServiceHandlerSubscription<TService, TEvent>(_isOptional);

            return this;
        }
    }
}