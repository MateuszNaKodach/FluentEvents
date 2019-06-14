using System.Security.Cryptography.X509Certificates;
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
        private readonly ISourceModelsService _sourceModelsService;

        /// <summary>
        ///     This API supports the FluentEvents infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public SubscriptionsBuilder(
            IGlobalSubscriptionsService globalSubscriptionsService,
            IScopedSubscriptionsService scopedSubscriptionsService,
            ISourceModelsService sourceModelsService
        )
        {
            _globalSubscriptionsService = globalSubscriptionsService;
            _scopedSubscriptionsService = scopedSubscriptionsService;
            _sourceModelsService = sourceModelsService;
        }

        /// <summary>
        ///     Returns an object that can be used to configure subscriptions for a service.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <returns>The configuration object for the specified service.</returns>
        public ServiceConfigurator<TService> Service<TService>()
            where TService : class
        {
            return new ServiceConfigurator<TService>(
                _scopedSubscriptionsService,
                _globalSubscriptionsService
            );
        }

        /// <summary>
        ///     Returns an object that can be used to configure subscriptions for
        ///     an <see cref="IEventHandler{TSource,TEventArgs}.HandleEventAsync"/> method.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <typeparam name="TSource">The type of the event source.</typeparam>
        /// <typeparam name="TEventArgs">The type of the event args</typeparam>
        /// <returns></returns>
        public ServiceHandlerConfigurator<TService, TSource, TEventArgs> ServiceHandler<TService, TSource, TEventArgs>()
            where TService : class, IEventHandler<TSource, TEventArgs>
            where TSource : class
            where TEventArgs : class
        {
            return new ServiceHandlerConfigurator<TService, TSource, TEventArgs>(
                _scopedSubscriptionsService,
                _globalSubscriptionsService,
                _sourceModelsService
            );
        }
    }
}