using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentEvents.Config;
using FluentEvents.Infrastructure;
using FluentEvents.Queues;
using FluentEvents.Subscriptions;
using Microsoft.Extensions.DependencyInjection;

namespace FluentEvents
{
    public abstract class EventsScope : IInfrastructure<IServiceProvider>
    {
        private static readonly ConcurrentDictionary<Type, InternalEventsContext> _eventsContexts;

        private readonly EventsContextOptions _options;
        private readonly IAppServiceProvider _appServiceProvider;
        private readonly IScopedAppServiceProvider _scopedAppServiceProvider;

        private readonly object _syncSubscriptions = new object();
        private IEnumerable<Subscription> _subscriptions;

        internal IEventsQueueCollection EventsQueues { get; }

        IServiceProvider IInfrastructure<IServiceProvider>.Instance => GetCurrentContext().Get<IServiceProvider>();

        static EventsScope()
        {
            _eventsContexts = new ConcurrentDictionary<Type, InternalEventsContext>();
        }

        public EventsScope(
            EventsContextOptions options,
            IAppServiceProvider appServiceProvider,
            IScopedAppServiceProvider scopedAppServiceProvider
        )
        {
            _options = options;
            _appServiceProvider = appServiceProvider;
            _scopedAppServiceProvider = scopedAppServiceProvider;
        }

        internal InternalEventsContext GetCurrentContext() => _eventsContexts.GetOrAdd(
            GetType(),
            x => new InternalEventsContext(
                _options,
                OnConfiguring,
                OnBuildingPipelines,
                OnBuildingSubscriptions,
                _appServiceProvider
            )
        );

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
        protected virtual void OnBuildingPipelines(PipelinesBuilder pipelinesBuilder) { }

        /// <summary>
        ///     Manually attach an event source to the context in order to forward it's events to the
        ///     configured pipelines.
        /// </summary>
        /// <param name="source">The event source.</param>
        public virtual void Attach(object source)
            => GetCurrentContext().Dependencies.AttachingService.Attach(source, this);

        /// <summary>
        ///     Forward the events of a queue to the corresponding pipelines.
        /// </summary>
        /// <param name="queueName">The name of the queue.</param>
        public virtual Task ProcessQueuedEventsAsync(string queueName = null)
            => GetCurrentContext().Dependencies.EventsQueuesService.ProcessQueuedEventsAsync(this, queueName);

        /// <summary>
        ///     Discards all the events of a queue.
        /// </summary>
        /// <param name="queueName">The name of the queue.</param>
        public virtual void DiscardQueuedEvents(string queueName = null)
            => GetCurrentContext().Dependencies.EventsQueuesService.DiscardQueuedEvents(this, queueName);

        internal virtual IEnumerable<Subscription> GetSubscriptions()
        {
            lock (_syncSubscriptions)
            {
                if (_subscriptions == null)
                {
                    var subscriptions = new List<Subscription>();
                    var scopedSubscriptionsService = GetCurrentContext()
                        .Get<IServiceProvider>()
                        .GetRequiredService<IScopedSubscriptionsService>();

                    subscriptions.AddRange(
                        scopedSubscriptionsService.SubscribeServices(_scopedAppServiceProvider)
                    );

                    _subscriptions = subscriptions;
                }

                return _subscriptions;
            }
        }
    }
}
