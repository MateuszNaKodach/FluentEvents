using System;
using System.Threading;
using System.Threading.Tasks;
using FluentEvents.Config;
using FluentEvents.Infrastructure;
using FluentEvents.Subscriptions;
using Microsoft.Extensions.DependencyInjection;

namespace FluentEvents
{
    public abstract class EventsContext : IEventsContext
    {
        IServiceProvider IInfrastructure<IServiceProvider>.Instance => m_InternalServiceProvider;
        private IServiceProvider m_InternalServiceProvider;
        private IEventsContextDependencies m_Dependencies;

        internal void Configure(
            EventsContextOptions options, 
            IInternalServiceCollection internalServices
        )
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            m_InternalServiceProvider = internalServices.BuildServiceProvider(this, options);
            m_Dependencies = m_InternalServiceProvider.GetRequiredService<IEventsContextDependencies>();
            OnBuildingSubscriptions(m_InternalServiceProvider.GetService<SubscriptionsBuilder>());
            OnBuildingPipelines(m_InternalServiceProvider.GetService<PipelinesBuilder>());
        }

        protected virtual void OnBuildingSubscriptions(SubscriptionsBuilder subscriptionsBuilder) { }
        protected abstract void OnBuildingPipelines(PipelinesBuilder pipelinesBuilder);
        
        public Task StartEventReceivers(CancellationToken cancellationToken = default) 
            => m_Dependencies.EventReceiversService.StartReceiversAsync(cancellationToken);

        public Task StopEventReceivers(CancellationToken cancellationToken = default) 
            => m_Dependencies.EventReceiversService.StopReceiversAsync(cancellationToken);

        public void Attach(object source, EventsScope eventsScope)
            => m_Dependencies.AttachingService.Attach(source, eventsScope);

        public async Task ProcessQueuedEventsAsync(EventsScope eventsScope, string queueName = null) 
            => await eventsScope.ProcessQueuedEventsAsync(this, queueName);

        public void DiscardQueuedEvents(EventsScope eventsScope, string queueName = null) 
            => eventsScope.DiscardQueuedEvents(this, queueName);
        
        public Subscription MakeGlobalSubscriptionTo<TSource>(Action<TSource> subscriptionAction)
            => m_Dependencies.GlobalSubscriptionCollection.AddGlobalScopeSubscription(subscriptionAction);

        public void CancelGlobalSubscription(Subscription subscription)
            => m_Dependencies.GlobalSubscriptionCollection.RemoveGlobalScopeSubscription(subscription);
    }
}
