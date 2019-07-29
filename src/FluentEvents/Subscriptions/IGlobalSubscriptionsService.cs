using System.Collections.Generic;

namespace FluentEvents.Subscriptions
{
    internal interface IGlobalSubscriptionsService
    {
        void AddGlobalServiceHandlerSubscription<TService, TEvent>(bool isOptional)
            where TService : class, IAsyncEventHandler<TEvent>
            where TEvent : class;

        IEnumerable<Subscription> GetGlobalSubscriptions();
    }
}