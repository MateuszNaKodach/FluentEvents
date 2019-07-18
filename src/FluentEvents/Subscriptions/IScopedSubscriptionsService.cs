using System.Collections.Generic;
using FluentEvents.Infrastructure;

namespace FluentEvents.Subscriptions
{
    internal interface IScopedSubscriptionsService
    {
        void ConfigureScopedServiceHandlerSubscription<TService, TEvent>(bool isOptional)
            where TService : class, IAsyncEventHandler<TEvent>
            where TEvent : class;

        IEnumerable<Subscription> SubscribeServices(IScopedAppServiceProvider scopedAppServiceProvider);
    }
}