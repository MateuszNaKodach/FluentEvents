using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace FluentEvents.Subscriptions
{
    internal class ServiceHandlerSubscriptionCreationTask<TService, TEvent> : ISubscriptionCreationTask
        where TService : class, IAsyncEventHandler<TEvent>
        where TEvent : class
    {
        private readonly bool _isOptional;

        public ServiceHandlerSubscriptionCreationTask(bool isOptional)
        {
            _isOptional = isOptional;
        }

        /// <inheritdoc />
        public IEnumerable<Subscription> CreateSubscriptions(IServiceProvider appServiceProvider)
        {
            var services = appServiceProvider.GetService<IEnumerable<TService>>().ToArray();

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