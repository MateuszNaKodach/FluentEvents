using System;
using FluentEvents.Model;

namespace FluentEvents.Subscriptions
{
    /// <inheritdoc />
    public class SubscriptionsFactory : ISubscriptionsFactory
    {
        private readonly ISourceModelsService _sourceModelsService;
        private readonly ISubscriptionScanService _subscriptionScanService;

        /// <summary>
        ///     This API supports the FluentEvents infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public SubscriptionsFactory(ISourceModelsService sourceModelsService, ISubscriptionScanService subscriptionScanService)
        {
            _sourceModelsService = sourceModelsService;
            _subscriptionScanService = subscriptionScanService;
        }

        /// <inheritdoc />
        public Subscription CreateSubscription<TSource>(SubscribedHandler subscribedHandler)
        {
            var subscription = new Subscription(typeof(TSource));

            subscription.AddHandler(subscribedHandler.EventName, subscribedHandler.EventsHandler);

            return subscription;
        }

        /// <inheritdoc />
        public Subscription CreateSubscription<TSource>(Action<TSource> subscriptionAction)
        {
            var sourceType = typeof(TSource);
            var sourceModel = _sourceModelsService.GetSourceModel(sourceType);
            if (sourceModel == null)
                throw new SourceIsNotConfiguredException(sourceType);

            var subscription = new Subscription(sourceType);
            var subscribedHandlers = _subscriptionScanService.GetSubscribedHandlers(
                sourceModel,
                x => subscriptionAction((TSource) x)
            );

            foreach (var subscribedHandler in subscribedHandlers)
                subscription.AddHandler(subscribedHandler.EventName, subscribedHandler.EventsHandler);

            return subscription;
        }
    }
}