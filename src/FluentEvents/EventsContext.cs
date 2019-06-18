using System;
using System.Threading;
using System.Threading.Tasks;
using FluentEvents.Config;
using FluentEvents.Infrastructure;
using FluentEvents.Subscriptions;
using Microsoft.Extensions.DependencyInjection;

namespace FluentEvents
{
    /// <summary>
    ///     The EventsContext provides the API surface to configure how events are handled and to create global subscriptions.
    ///     An EventsContext should be treated as a singleton.
    /// </summary>
    public abstract class EventsContext : IInfrastructure<IServiceProvider>
    {
        IServiceProvider IInfrastructure<IServiceProvider>.Instance => InternalServiceProvider;

        private EventsContextOptions _options;
        private IInternalServiceCollection _internalServices;

        private IServiceProvider _internalServiceProvider;
        private IEventsContextDependencies _dependencies;

        internal bool IsInitializing => _internalServiceProvider != null && _dependencies == null;

        private IServiceProvider InternalServiceProvider
        {
            get
            {
                if (_internalServiceProvider == null)
                {
                    OnConfiguring(_options);
                    _internalServiceProvider = _internalServices.BuildServiceProvider(this, _options);
                    Build();
                }

                return _internalServiceProvider;
            }
        }

        private IEventsContextDependencies Dependencies =>
            _dependencies ??
            (_dependencies = InternalServiceProvider.GetRequiredService<IEventsContextDependencies>());

        /// <summary>
        ///     This constructor can be used when the <see cref="EventsContext" /> is configured with
        ///     the <see cref="ServiceCollectionExtensions.AddEventsContext{T}(IServiceCollection, Action{EventsContextOptions})" />
        ///     extension method.
        /// </summary>
        protected EventsContext()
            : this(new EventsContextOptions())
        {
        }

        /// <summary>
        ///     This constructor can be used when the <see cref="EventsContext" /> is not configured with
        ///     the <see cref="IServiceCollection" /> extension method.
        /// </summary>
        protected EventsContext(EventsContextOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));

            var emptyAppServiceProvider = new ServiceCollection().BuildServiceProvider();
            _internalServices = new InternalServiceCollection(new AppServiceProvider(emptyAppServiceProvider));
        }

        internal void Configure(
            EventsContextOptions options, 
            IInternalServiceCollection internalServices
        )
        {
#pragma warning disable IDE0016 // Use 'throw' expression
            if (options == null) throw new ArgumentNullException(nameof(options));
            if (internalServices == null) throw new ArgumentNullException(nameof(internalServices));
#pragma warning restore IDE0016 // Use 'throw' expression

            _options = options;
            _internalServices = internalServices;
        }

        private void Build()
        {
            OnBuildingPipelines(InternalServiceProvider.GetRequiredService<PipelinesBuilder>());
            OnBuildingSubscriptions(InternalServiceProvider.GetRequiredService<SubscriptionsBuilder>());

            foreach (var validableConfig in InternalServiceProvider.GetServices<IValidableConfig>())
                validableConfig.Validate();
        }

        /// <summary>
        ///     The default implementation of this method does nothing, but it can be overridden in a derived class
        ///     to override the options supplied in the constructor or with DI.
        /// </summary>
        /// <param name="options">The options of the <see cref="EventsContext"/>.</param>
        protected virtual void OnConfiguring(EventsContextOptions options) { }

        /// <summary>
        ///     The default implementation of this method does nothing, but it can be overridden in a derived class
        ///     to configure the subscriptions that should be created automatically.
        /// </summary>
        /// <remarks>
        ///     This method is called only once when the instance of a derived context is created.
        /// </remarks>
        /// <param name="subscriptionsBuilder">The builder that defines the model for the context being created.</param>
        protected virtual void OnBuildingSubscriptions(SubscriptionsBuilder subscriptionsBuilder) { }

        /// <summary>
        ///     The default implementation of this method does nothing, but it can be overridden in a derived class
        ///     to configure the pipelines needed for handling the events.
        /// </summary>
        /// <remarks>
        ///     This method is called only once when the instance of a derived context is created.
        /// </remarks>
        /// <param name="pipelinesBuilder">The builder that defines the model for the context being created.</param>
        protected abstract void OnBuildingPipelines(PipelinesBuilder pipelinesBuilder);

        /// <summary>
        ///     Starts the registered event receivers manually.
        /// </summary>
        /// <remarks>
        ///     Event receivers lifetime is controlled by an <see cref="Microsoft.Extensions.Hosting.IHostedService"/>.
        ///     This method should to be called only if the application isn't using the <see cref="Microsoft.Extensions.Hosting.HostBuilder"/>.
        /// </remarks>
        /// <param name="cancellationToken">The cancellation token for the async operation.</param>
        public Task StartEventReceiversAsync(CancellationToken cancellationToken = default) 
            => Dependencies.EventReceiversService.StartReceiversAsync(cancellationToken);

        /// <summary>
        ///     Stops the registered event receivers manually.
        /// </summary>
        /// <remarks>
        ///     Event receivers lifetime is controlled by an <see cref="Microsoft.Extensions.Hosting.IHostedService"/>.
        ///     This method should to be called only if the application isn't using
        ///     the <see cref="Microsoft.Extensions.Hosting.HostBuilder"/>.
        /// </remarks>
        /// <param name="cancellationToken">The cancellation token for the async operation.</param>
        public Task StopEventReceiversAsync(CancellationToken cancellationToken = default) 
            => Dependencies.EventReceiversService.StopReceiversAsync(cancellationToken);

        /// <summary>
        ///     Manually attach an event source to the context in order to forward it's events to the
        ///     configured pipelines.
        /// </summary>
        /// <param name="source">The event source.</param>
        /// <param name="eventsScope">The scope where the events should be queued and published.</param>
        public virtual void Attach(object source, EventsScope eventsScope)
            => Dependencies.AttachingService.Attach(source, eventsScope);

        /// <summary>
        ///     Forward the events of a queue to the corresponding pipelines.
        /// </summary>
        /// <param name="eventsScope">The scope of the queue.</param>
        /// <param name="queueName">The name of the queue.</param>
        public virtual Task ProcessQueuedEventsAsync(EventsScope eventsScope, string queueName = null) 
            => Dependencies.EventsQueuesService.ProcessQueuedEventsAsync(eventsScope, queueName);

        /// <summary>
        ///     Discards all the events of a queue.
        /// </summary>
        /// <param name="eventsScope">The scope of the queue.</param>
        /// <param name="queueName">The name of the queue.</param>
        public virtual void DiscardQueuedEvents(EventsScope eventsScope, string queueName = null) 
            => Dependencies.EventsQueuesService.DiscardQueuedEvents(eventsScope, queueName);

        /// <summary>
        ///     Creates one ore more subscriptions in the global scope.
        /// </summary>
        /// <example>
        ///     <code>
        ///         eventsContext.MakeGlobalSubscriptionsTo&lt;ExampleEntity&gt;(exampleEntity =&gt;
        ///             {
        ///                 exampleEntity.ExampleEvent1 += ExampleEventHandlerMethod1;
        ///                 exampleEntity.ExampleEvent2 += ExampleEventHandlerMethod2;
        ///             }
        ///         );
        ///     </code>
        /// </example>
        /// <remarks>You can call <see cref="Unsubscribe"/> to stop receiving the events.</remarks>
        /// <typeparam name="TSource">The type of the events source.</typeparam>
        /// <param name="subscriptionAction">A delegate with the subscriptions to the events of the source.</param>
        /// <returns>
        ///     The <see cref="ISubscriptionsCancellationToken"/> that should be passed to
        ///     <see cref="Unsubscribe"/> to stop receiving the events.
        /// </returns>
        public ISubscriptionsCancellationToken SubscribeGloballyTo<TSource>(Action<TSource> subscriptionAction)
            => Dependencies.GlobalSubscriptionsService.AddGlobalSubscription(subscriptionAction);

        /// <summary>
        ///     Cancels a global subscription.
        /// </summary>
        /// <param name="subscriptionsCancellationToken">The token of the subscription(s) to cancel.</param>
        public void Unsubscribe(ISubscriptionsCancellationToken subscriptionsCancellationToken)
            => Dependencies.GlobalSubscriptionsService.RemoveGlobalSubscription((Subscription)subscriptionsCancellationToken);
    }
}
