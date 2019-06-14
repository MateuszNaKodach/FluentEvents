using System;
using System.Threading.Tasks;
using FluentEvents.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace FluentEvents.Subscriptions
{
    internal class ServiceHandlerSubscriptionCreationTask<TService, TSource, TEventArgs> : ISubscriptionCreationTask
        where TService : class, IEventHandler<TSource, TEventArgs>
        where TSource : class
        where TEventArgs : class
    {
        private readonly string _eventName;
        private readonly ISubscriptionsFactory _subscriptionsFactory;

        public ServiceHandlerSubscriptionCreationTask(string eventName, ISubscriptionsFactory subscriptionsFactory)
        {
            _eventName = eventName;
            _subscriptionsFactory = subscriptionsFactory;
        }

        public Subscription CreateSubscription(IAppServiceProvider appServiceProvider)
        {
            var service = appServiceProvider.GetService<TService>();
            if (service == null)
                throw new SubscribingServiceNotFoundException(typeof(TService));

            Func<TSource, TEventArgs, Task> handler = service.HandleEventAsync;
            var handlerDelegate = Delegate.CreateDelegate(handler.GetType(), service, handler.Method);

            return _subscriptionsFactory.CreateSubscription<TSource>(new SubscribedHandler(_eventName, handlerDelegate));
        }
    }
}