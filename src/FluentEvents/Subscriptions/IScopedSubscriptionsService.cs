using System.Collections.Generic;
using FluentEvents.Infrastructure;
using FluentEvents.ServiceProviders;

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