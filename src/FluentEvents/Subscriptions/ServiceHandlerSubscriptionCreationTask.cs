using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentEvents.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace FluentEvents.Subscriptions
{
    internal class ServiceHandlerSubscriptionCreationTask<TService, TEvent> : ISubscriptionCreationTask
        where TService : class, IEventHandler<TEvent>
        where TEvent : class
    {
        private readonly bool _isOptional;

        public ServiceHandlerSubscriptionCreationTask(bool isOptional)
        {
            _isOptional = isOptional;
        }

        public IEnumerable<Subscription> CreateSubscriptions(IAppServiceProvider appServiceProvider)
        {
            var services = appServiceProvider.GetService<IEnumerable<TService>>();

            if (!_isOptional && !services.Any())
                throw new SubscribingServiceNotFoundException(typeof(TService));

            foreach (var service in services)
            {
                Func<TEvent, Task> handler = service.HandleEventAsync;
                var handlerDelegate = Delegate.CreateDelegate(handler.GetType(), service, handler.Method);

                yield return new Subscription(typeof(TEvent), handlerDelegate);
            }
        }
    }
}