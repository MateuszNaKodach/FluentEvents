using System;
using FluentEvents.Config;
using FluentEvents.Model;

namespace FluentEvents.Subscriptions
{
    /// <inheritdoc />
    public class SubscriptionsFactory : ISubscriptionsFactory
    {
        private readonly ISourceModelsService m_SourceModelsService;
        private readonly ISubscriptionScanService m_SubscriptionScanService;

        /// <summary>
        ///     This API supports the FluentEvents infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public SubscriptionsFactory(ISourceModelsService sourceModelsService, ISubscriptionScanService subscriptionScanService)
        {
            m_SourceModelsService = sourceModelsService;
            m_SubscriptionScanService = subscriptionScanService;
        }

        /// <inheritdoc />
        public Subscription CreateSubscription<TSource>(Action<TSource> subscriptionAction)
        {
            var sourceType = typeof(TSource);
            var sourceModel = m_SourceModelsService.GetSourceModel(sourceType);
            if (sourceModel == null)
                throw new SourceIsNotConfiguredException(sourceType);

            var subscription = new Subscription(sourceType);
            var subscribedHandlers = m_SubscriptionScanService.GetSubscribedHandlers(
                sourceModel, 
                x => subscriptionAction((TSource)x)
            );

            foreach (var subscribedHandler in subscribedHandlers)
                subscription.AddHandler(subscribedHandler.EventName, subscribedHandler.EventsHandler);

            return subscription;
        }
    }
}