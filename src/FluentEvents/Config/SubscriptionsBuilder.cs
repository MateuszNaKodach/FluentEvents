using FluentEvents.Subscriptions;

namespace FluentEvents.Config
{
    /// <summary>
    ///     Provides a simple API surface to select a service and configure it fluently.
    /// </summary>
    public class SubscriptionsBuilder
    {
        private readonly IGlobalSubscriptionCollection _globalSubscriptionCollection;
        private readonly IScopedSubscriptionsService _scopedSubscriptionsService;

        /// <summary>
        ///     This API supports the FluentEvents infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public SubscriptionsBuilder(
            IGlobalSubscriptionCollection globalSubscriptionCollection,
            IScopedSubscriptionsService scopedSubscriptionsService
        )
        {
            _globalSubscriptionCollection = globalSubscriptionCollection;
            _scopedSubscriptionsService = scopedSubscriptionsService;
        }

        /// <summary>
        ///     Returns an object that can be used to configure subscriptions for a service.
        ///     configure multiple pipelines.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <returns>The configuration object for the specified service.</returns>
        public ServiceConfigurator<TService> Service<TService>()
            where TService : class
        {
            return new ServiceConfigurator<TService>(
                _scopedSubscriptionsService,
                _globalSubscriptionCollection
            );
        }
    }
}