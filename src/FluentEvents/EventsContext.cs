using System;
using System.Threading;
using System.Threading.Tasks;
using FluentEvents.Configuration;
using FluentEvents.Infrastructure;
using FluentEvents.Queues;
using FluentEvents.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace FluentEvents
{
    /// <inheritdoc cref="IEventsContext"/>
    public abstract class EventsContext : IEventsContext
    {
        private readonly EventsContextOptions _options;
        private readonly EventsContextsRoot _eventsContextsRoot;

        private readonly Lazy<IEventsScope> _eventsScope;

        IServiceProvider IInfrastructure<IServiceProvider>.Instance => GetCurrentInternalServiceProvider();

        /// <summary>
        ///     This constructor can be used when the <see cref="EventsContext" /> is not configured with
        ///     the <see cref="IServiceCollection" /> extension method.
        /// </summary>
        /// <param name="eventsContextsRoot">
        ///     The events context root used for sharing the context configuration across different instances.
        /// </param>
        /// <param name="options">The options for this context.</param>
        /// <param name="scopedAppServiceProvider">The scoped app service provider.</param>
        protected EventsContext(
            EventsContextsRoot eventsContextsRoot, 
            EventsContextOptions options,
            IScopedAppServiceProvider scopedAppServiceProvider
        )
        {
            _options = options;
            _eventsContextsRoot = eventsContextsRoot;

            _eventsScope = new Lazy<IEventsScope>(
                () => new EventsScope(GetCurrentInternalServiceProvider(), scopedAppServiceProvider),
                LazyThreadSafetyMode.PublicationOnly
            );
        }

        private InternalEventsContext GetCurrentContext()
        {
            return _eventsContextsRoot.GetOrCreateContext(
                GetType(), _options,
                () => new InternalEventsContext(
                    _options,
                    OnConfiguring,
                    OnBuildingPipelines,
                    OnBuildingSubscriptions,
                    _eventsContextsRoot.AppServiceProvider
                )
            );
        }

        private IServiceProvider GetCurrentInternalServiceProvider() => GetCurrentContext().InternalServiceProvider;

        /// <summary>
        ///     Override this method to override the options supplied in the constructor. The resulting configuration may be cached
        ///     and re-used for subsequent instances of your derived context with the same <see cref="EventsContextsRoot"/>.
        /// </summary> 
        /// <remarks>The default implementation of this method does nothing.</remarks>
        /// <param name="options">The options of the <see cref="EventsContext"/>.</param>
        protected virtual void OnConfiguring(EventsContextOptions options) { }

        /// <summary>
        ///     Override this method to configure the event subscriptions. The resulting configuration may be cached
        ///     and re-used for subsequent instances of your derived context with the same <see cref="EventsContextsRoot"/>.
        /// </summary>
        /// <remarks>The default implementation of this method does nothing.</remarks>
        /// <param name="subscriptionsBuilder">The builder that defines the model for the context being created.</param>
        protected virtual void OnBuildingSubscriptions(SubscriptionsBuilder subscriptionsBuilder) { }

        /// <summary>
        ///     Override this method to configure the pipelines needed for handling the events.
        ///     The resulting configuration may be cached and re-used for subsequent instances of
        ///     your derived context with the same <see cref="EventsContextsRoot"/>.
        /// </summary>
        /// <remarks>The default implementation of this method does nothing.</remarks>
        /// <param name="pipelinesBuilder">The builder that defines the model for the context being created.</param>
        protected virtual void OnBuildingPipelines(PipelinesBuilder pipelinesBuilder) { }

        /// <summary>
        ///     Attach an event source to the context in order to forward it's events to the configured pipelines.
        /// </summary>
        /// <param name="source">The event source.</param>
        public virtual void Attach(object source)
            => GetCurrentInternalServiceProvider()
                .GetRequiredService<IAttachingService>()
                .Attach(source, _eventsScope.Value);

        /// <summary>
        ///     Continues the processing of the events that have been queued.
        /// </summary>
        /// <param name="queueName">
        ///     The name of the queue.
        ///     If null all the events will be processed.
        /// </param>
        public virtual Task ProcessQueuedEventsAsync(string queueName = null)
            => GetCurrentInternalServiceProvider()
                .GetRequiredService<IEventsQueuesService>()
                .ProcessQueuedEventsAsync(_eventsScope.Value, queueName);

        /// <summary>
        ///     Discards all the events of a queue.
        /// </summary>
        /// <param name="queueName">
        ///     The name of the queue.
        ///     If null all the events will be discarded.
        /// </param>
        public virtual void DiscardQueuedEvents(string queueName = null)
            => GetCurrentInternalServiceProvider()
                .GetRequiredService<IEventsQueuesService>()
                .DiscardQueuedEvents(_eventsScope.Value, queueName);
    }
}
