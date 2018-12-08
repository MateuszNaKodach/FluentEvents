using System;
using FluentEvents.Infrastructure;
using FluentEvents.Subscriptions;

namespace FluentEvents
{
    public interface IEventsContext : IInfrastructure<IServiceProvider>
    {
        void Attach(object source, EventsScope eventsScope);
        Subscription MakeGlobalSubscriptionTo<TSource>(Action<TSource> subscriptionAction);
        void CancelGlobalSubscription(Subscription subscription);
    }
}