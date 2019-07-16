using System;
using FluentEvents.Model;
using FluentEvents.Subscriptions;

namespace FluentEvents.Configuration
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
        ///     Maps required services to the <see cref="EventsContext"/>.
        ///     If the <see cref="IServiceProvider"/> doesn't return any service of this type an exception is thrown during publishing.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <typeparam name="TEvent">The type of the event</typeparam>
        /// <returns>
        ///     Returns an object that can be used to configure subscriptions for
        ///     an <see cref="IEventHandler{TEvent}.HandleEventAsync"/> method.
        /// </returns>
        public ServiceHandlerConfigurator<TService, TEvent> ServiceHandler<TService, TEvent>()
            where TService : class, IEventHandler<TEvent>
            where TEvent : class
        {
            return new ServiceHandlerConfigurator<TService, TEvent>(
                _scopedSubscriptionsService,
                _globalSubscriptionsService,
                false
            );
        }

        /// <summary>
        ///     Maps optional services to the <see cref="EventsContext"/>.
        ///     If the <see cref="IServiceProvider"/> doesn't return any service of this type no exceptions will be thrown during publishing.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <typeparam name="TEvent">The type of the event</typeparam>
        /// <returns>
        ///     Returns an object that can be used to configure subscriptions for
        ///     an <see cref="IEventHandler{TEvent}.HandleEventAsync"/> method.
        /// </returns>
        public ServiceHandlerConfigurator<TService, TEvent> OptionalServiceHandler<TService, TEvent>()
            where TService : class, IEventHandler<TEvent>
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