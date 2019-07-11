using System;
using FluentEvents.Subscriptions;

namespace FluentEvents.Config
{
    /// <summary>
    ///     Provides an API surface to configure the subscriptions of a service event handler.
    /// </summary>
    public class ServiceHandlerConfigurator<TService, TEvent>
        where TService : class, IEventHandler<TEvent>
        where TEvent : class
    {
        private readonly IScopedSubscriptionsService _scopedSubscriptionsService;
        private readonly IGlobalSubscriptionsService _globalSubscriptionsService;

        internal ServiceHandlerConfigurator(
            IScopedSubscriptionsService scopedSubscriptionsService, 
            IGlobalSubscriptionsService globalSubscriptionsService
        )
        {
            _scopedSubscriptionsService = scopedSubscriptionsService;
            _globalSubscriptionsService = globalSubscriptionsService;
        }

        /// <summary>
        ///     Subscribes the <see cref="IEventHandler{TEvent}.HandleEventAsync"/> to global events.
        /// </summary>
        /// <returns>The configuration object to add more subscriptions.</returns>
        /// <exception cref="EventArgsTypeMismatchException">
        ///     The specified event args type is different from the event args type of the event being selected.
        /// </exception>
        public ServiceHandlerConfigurator<TService, TEvent> HasGlobalSubscription()
        {
            _globalSubscriptionsService.AddGlobalServiceHandlerSubscription<TService, TEvent>();

            return this;
        }

        /// <summary>
        ///     Subscribes the <see cref="IEventHandler{TEvent}.HandleEventAsync"/> to scoped events.
        /// </summary>
        /// <returns>The configuration object to add more subscriptions.</returns>
        /// <exception cref="EventArgsTypeMismatchException">
        ///     The specified event args type is different from the event args type of the event being selected.
        /// </exception>
        public ServiceHandlerConfigurator<TService, TEvent> HasScopedSubscription()
        {
            _scopedSubscriptionsService.ConfigureScopedServiceHandlerSubscription<TService, TEvent>();

            return this;
        }
    }
}