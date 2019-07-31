using FluentEvents.Subscriptions;

namespace FluentEvents.Configuration
{
    /// <summary>
    ///     Allows configuration to be performed for a service type.
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
        ///     Subscribes the service handler to global events.
        /// </summary>
        /// <returns>The configuration object to add more subscriptions.</returns>
        public ServiceHandlerConfiguration<TService, TEvent> HasGlobalSubscription()
        {
            _globalSubscriptionsService.AddGlobalServiceHandlerSubscription<TService, TEvent>(_isOptional);

            return this;
        }

        /// <summary>
        ///     Subscribes the service handler to scoped events.
        /// </summary>
        /// <returns>The configuration object to add more subscriptions.</returns>
        public ServiceHandlerConfiguration<TService, TEvent> HasScopedSubscription()
        {
            _scopedSubscriptionsService.ConfigureScopedServiceHandlerSubscription<TService, TEvent>(_isOptional);

            return this;
        }
    }
}