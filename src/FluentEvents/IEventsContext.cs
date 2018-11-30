using System;
using FluentEvents.Subscriptions;

namespace FluentEvents
{
    public interface IEventsContext
    {
        void Attach(object source, EventsScope eventsScope);
        Subscription MakeGlobalSubscriptionTo<TSource>(Action<TSource> subscriptionAction);
        void CancelGlobalSubscription(Subscription subscription);
    }
}