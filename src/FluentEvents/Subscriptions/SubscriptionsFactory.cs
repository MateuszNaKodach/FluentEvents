using System;
using FluentEvents.Config;
using FluentEvents.Model;

namespace FluentEvents.Subscriptions
{
    public class SubscriptionsFactory : ISubscriptionsFactory
    {
        private readonly ISourceModelsService m_SourceModelsService;

        public SubscriptionsFactory(ISourceModelsService sourceModelsService)
        {
            m_SourceModelsService = sourceModelsService;
        }

        public Subscription CreateSubscription<TSource>(Action<TSource> subscriptionAction)
        {
            return CreateSubscription(typeof(TSource), x => subscriptionAction((TSource)x));
        }

        public Subscription CreateSubscription(Type sourceType, Action<object> subscriptionAction)
        {
            var sourceModel = m_SourceModelsService.GetSourceModel(sourceType);
            if (sourceModel == null)
                throw new SourceIsNotConfiguredException(sourceType);

            var mockSource = Activator.CreateInstance(sourceType, true);
            subscriptionAction.Invoke(mockSource);

            var subscription = new Subscription(sourceType);

            foreach (var eventField in sourceModel.EventFields)
            {
                var eventsHandler = (Delegate)eventField.FieldInfo.GetValue(mockSource);

                if (eventsHandler != null)
                    subscription.AddHandler(eventField.EventInfo.Name, eventsHandler);
            }

            return subscription;
        }
    }
}