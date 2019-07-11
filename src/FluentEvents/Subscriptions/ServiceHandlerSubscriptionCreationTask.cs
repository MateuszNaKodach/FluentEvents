using System;
using System.Threading.Tasks;
using FluentEvents.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace FluentEvents.Subscriptions
{
    internal class ServiceHandlerSubscriptionCreationTask<TService, TEvent> : ISubscriptionCreationTask
        where TService : class, IEventHandler<TEvent>
        where TEvent : class
    {
        public Subscription CreateSubscription(IAppServiceProvider appServiceProvider)
        {
            var service = appServiceProvider.GetService<TService>();
            if (service == null)
                throw new SubscribingServiceNotFoundException(typeof(TService));

            Func<TEvent, Task> handler = service.HandleEventAsync;
            var handlerDelegate = Delegate.CreateDelegate(handler.GetType(), service, handler.Method);

            return new Subscription(typeof(TEvent), handlerDelegate);
        }
    }
}