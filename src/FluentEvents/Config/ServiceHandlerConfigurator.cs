using System;
using System.Threading.Tasks;
using FluentEvents.Model;
using FluentEvents.Subscriptions;

namespace FluentEvents.Config
{
    /// <summary>
    ///     Provides an API surface to configure the subscriptions of a service event handler.
    /// </summary>
    public class ServiceHandlerConfigurator<TService, TSource, TEventArgs>
        where TService : class, IEventHandler<TSource, TEventArgs>
        where TSource : class
        where TEventArgs : class
    {
        private readonly IScopedSubscriptionsService _scopedSubscriptionsService;
        private readonly IGlobalSubscriptionCollection _globalSubscriptionCollection;
        private readonly ISourceModelsService _sourceModelsService;

        internal ServiceHandlerConfigurator(
            IScopedSubscriptionsService scopedSubscriptionsService, 
            IGlobalSubscriptionCollection globalSubscriptionCollection,
            ISourceModelsService sourceModelsService
        )
        {
            _scopedSubscriptionsService = scopedSubscriptionsService;
            _globalSubscriptionCollection = globalSubscriptionCollection;
            _sourceModelsService = sourceModelsService;
        }

        /// <summary>
        ///     Subscribes the <see cref="IEventHandler{TSource,TEventArgs}.HandleEventAsync"/> to global events.
        /// </summary>
        /// <param name="eventName"></param>
        /// <returns>The configuration object to add more subscriptions.</returns>
        /// <exception cref="EventNotConfiguredException">
        ///     An event with the provided name wasn't configured
        ///     in the <see cref="EventsContext.OnBuildingPipelines"/> method.
        /// </exception>
        public ServiceHandlerConfigurator<TService, TSource, TEventArgs> HasGlobalSubscription(string eventName)
        {
            var sourceModel = _sourceModelsService.GetOrCreateSourceModel(typeof(TSource));
            var eventField = sourceModel.GetEventField(eventName);

            if (eventField == null)
                throw new EventNotConfiguredException();

            _globalSubscriptionCollection.AddGlobalServiceHandlerSubscription<TService, TSource, TEventArgs>(
                eventName
            );

            return this;
        }

        /// <summary>
        ///     Subscribes the <see cref="IEventHandler{TSource,TEventArgs}.HandleEventAsync"/> to scoped events.
        /// </summary>
        /// <param name="eventName"></param>
        /// <returns>The configuration object to add more subscriptions.</returns>
        /// <exception cref="EventNotConfiguredException">
        ///     An event with the provided name wasn't configured
        ///     in the <see cref="EventsContext.OnBuildingPipelines"/> method.
        /// </exception>
        public ServiceHandlerConfigurator<TService, TSource, TEventArgs> HasScopedSubscription(string eventName)
        {
            var sourceModel = _sourceModelsService.GetOrCreateSourceModel(typeof(TSource));
            var eventField = sourceModel.GetEventField(eventName);

            if (eventField == null)
                throw new EventNotConfiguredException();

            _scopedSubscriptionsService.ConfigureScopedServiceSubscription<TService, TSource, TEventArgs>(
                eventName
            );

            return this;
        }
    }
}