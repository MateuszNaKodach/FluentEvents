using System;
using System.Threading;
using System.Threading.Tasks;
using FluentEvents.Config;
using FluentEvents.Infrastructure;
using FluentEvents.Subscriptions;
using Microsoft.Extensions.DependencyInjection;

namespace FluentEvents
{
    public abstract class EventsContext : IInfrastructure<IServiceProvider>
    {
        private readonly EventsContextOptions m_Options;
        IServiceProvider IInfrastructure<IServiceProvider>.Instance => m_InternalServiceProvider;
        private IServiceProvider m_InternalServiceProvider;

        private Lazy<IEventsContextDependencies> m_Dependencies;

        protected EventsContext()
            : this(null)
        {
        }

        protected EventsContext(EventsContextOptions options)
        {
            m_Options = options;

            var emptyServiceCollection = new ServiceCollection();
            var emptyServiceProvider = emptyServiceCollection.BuildServiceProvider();
            var internalServices = new InternalServiceCollection(emptyServiceProvider);

            m_Dependencies = new Lazy<IEventsContextDependencies>(() => Build(options, internalServices));

        }

        internal void Configure(
            EventsContextOptions options, 
            IInternalServiceCollection internalServices
        )
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            options = m_Options ?? options;

            m_Dependencies = new Lazy<IEventsContextDependencies>(() => Build(options, internalServices));
        }

        private IEventsContextDependencies Build(EventsContextOptions options, IInternalServiceCollection internalServices)
        {
            if (options == null)
                throw new EventsContextOptionsAreMissingException();

            m_InternalServiceProvider = internalServices.BuildServiceProvider(this, options);

            OnBuildingSubscriptions(m_InternalServiceProvider.GetRequiredService<SubscriptionsBuilder>());
            OnBuildingPipelines(m_InternalServiceProvider.GetRequiredService<PipelinesBuilder>());

            return m_InternalServiceProvider.GetRequiredService<IEventsContextDependencies>();
        }

        /// <summary>
        /// The default implementation of this method does nothing, but it can be overridden in a derived class
        /// to configure the subscriptions that should be created automatically.
        /// </summary>
        /// <remarks>
        /// This method is called only once when the instance of a derived context is created.
        /// </remarks>
        /// <param name="subscriptionsBuilder">The builder that defines the model for the context being created.</param>
        protected virtual void OnBuildingSubscriptions(SubscriptionsBuilder subscriptionsBuilder) { }

        /// <summary>
        /// The default implementation of this method does nothing, but it can be overridden in a derived class
        /// to configure the pipelines needed for handling the events.
        /// </summary>
        /// <remarks>
        /// This method is called only once when the instance of a derived context is created.
        /// </remarks>
        /// <param name="pipelinesBuilder">The builder that defines the model for the context being created.</param>
        protected abstract void OnBuildingPipelines(PipelinesBuilder pipelinesBuilder);
        
        /// <summary>
        /// Starts the registered event receivers manually.
        /// </summary>
        /// <remarks>
        /// Usually it's not necessary to call this method because the event receivers lifetime controlled by an IHostedService so
        /// this method should to be called only if the context hasn't been configured inside of a HostBuilder.
        /// </remarks>
        /// <param name="cancellationToken">The cancellation token for the async operation.</param>
        public Task StartEventReceivers(CancellationToken cancellationToken = default) 
            => m_Dependencies.Value.EventReceiversService.StartReceiversAsync(cancellationToken);

        /// <summary>
        /// Stops the registered event receivers manually.
        /// </summary>
        /// <remarks>
        /// Usually it's not necessary to call this method because the event receivers lifetime controlled by an IHostedService so
        /// this method should to be called only if the context hasn't been configured inside of a HostBuilder.
        /// </remarks>
        /// <param name="cancellationToken">The cancellation token for the async operation.</param>
        public Task StopEventReceivers(CancellationToken cancellationToken = default) 
            => m_Dependencies.Value.EventReceiversService.StopReceiversAsync(cancellationToken);

        /// <summary>
        /// Manually attach an event source to the context in order to forward it's events to the configured pipelines.
        /// </summary>
        /// <param name="source">The event source.</param>
        /// <param name="eventsScope">The scope where the events should be queued and published.</param>
        public void Attach(object source, EventsScope eventsScope)
            => m_Dependencies.Value.AttachingService.Attach(source, eventsScope);

        /// <summary>
        /// Forward the events of a queue to the corresponding pipelines.
        /// </summary>
        /// <param name="eventsScope">The scope of the queue.</param>
        /// <param name="queueName">The name of the queue.</param>
        public Task ProcessQueuedEventsAsync(EventsScope eventsScope, string queueName = null) 
            => m_Dependencies.Value.EventsQueuesService.ProcessQueuedEventsAsync(eventsScope, queueName);

        /// <summary>
        /// Discards all the events of a queue.
        /// </summary>
        /// <param name="eventsScope">The scope of the queue.</param>
        /// <param name="queueName">The name of the queue.</param>
        public void DiscardQueuedEvents(EventsScope eventsScope, string queueName = null) 
            => m_Dependencies.Value.EventsQueuesService.DiscardQueuedEvents(eventsScope, queueName);

        /// <summary>
        /// Makes a subscription in the global scope.
        /// </summary>
        /// <remarks>You can call CancelGlobalSubscription() to stop receiving the events.</remarks>
        /// <typeparam name="TSource">The type of the events source.</typeparam>
        /// <param name="subscriptionAction">A delegate with the subscriptions to the events of the source.</param>
        /// <returns>The subscription that should be passed to CancelGlobalSubscription() to stop receiving the events.</returns>
        public Subscription MakeGlobalSubscriptionTo<TSource>(Action<TSource> subscriptionAction)
            => m_Dependencies.Value.GlobalSubscriptionCollection.AddGlobalScopeSubscription(subscriptionAction);

        /// <summary>
        /// Cancels a global subscription.
        /// </summary>
        /// <param name="subscription">The subscription to cancel.</param>
        public void CancelGlobalSubscription(Subscription subscription)
            => m_Dependencies.Value.GlobalSubscriptionCollection.RemoveGlobalScopeSubscription(subscription);
    }
}
