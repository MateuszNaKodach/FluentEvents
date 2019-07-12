using FluentEvents.Model;
using FluentEvents.Subscriptions;

namespace FluentEvents.Config
{
    /// <summary>
    ///     Provides a simple API surface to select a service and configure it fluently.
    /// </summary>
    public class SubscriptionsBuilder
    {
        private readonly IGlobalSubscriptionsService _globalSubscriptionsService;
        private readonly IScopedSubscriptionsService _scopedSubscriptionsService;

        /// <summary>
        ///     This API supports the FluentEvents infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public SubscriptionsBuilder(
            IGlobalSubscriptionsService globalSubscriptionsService,
            IScopedSubscriptionsService scopedSubscriptionsService
        )
        {
            _globalSubscriptionsService = globalSubscriptionsService;
            _scopedSubscriptionsService = scopedSubscriptionsService;
        }

        /// <summary>
        ///     Returns an object that can be used to configure subscriptions for
        ///     an <see cref="IEventHandler{TEvent}.HandleEventAsync"/> method.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <typeparam name="TEvent">The type of the event</typeparam>
        /// <returns></returns>
        public ServiceHandlerConfigurator<TService, TEvent> ServiceHandler<TService, TEvent>()
            where TService : class, IEventHandler<TEvent>
            where TEvent : class
        {
            return new ServiceHandlerConfigurator<TService, TEvent>(
                _scopedSubscriptionsService,
                _globalSubscriptionsService
            );
        }
    }
}