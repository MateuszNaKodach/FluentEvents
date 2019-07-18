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
    /// <summary>
    ///     The <see cref="EventsContext"/> provides the API surface to configure the event pipelines and the event subscriptions.
    ///     An <see cref="EventsContext"/> should be treated as a singleton.
    /// </summary>
    public abstract class EventsContext : IEventsContext
    {
        private readonly Lazy<InternalEventsContext> _internalEventsContext;

        IServiceProvider IInfrastructure<IServiceProvider>.Instance => GetCurrentInternalServiceProvider();

        /// <summary>
        ///     Creates a new <see cref="EventsContext"/>
        /// </summary>
        /// <param name="options">The options for this context.</param>
        /// <param name="rootAppServiceProvider">The application root service provider.</param>
        protected EventsContext(
            EventsContextOptions options,
            IRootAppServiceProvider rootAppServiceProvider
        )
        {
            _internalEventsContext = new Lazy<InternalEventsContext>(() => new InternalEventsContext(
                options,
                OnConfiguring,
                OnBuildingPipelines,
                OnBuildingSubscriptions,
                rootAppServiceProvider,
                this
            ), LazyThreadSafetyMode.ExecutionAndPublication);
        }

        private InternalEventsContext GetCurrentContext() => _internalEventsContext.Value;

        private IServiceProvider GetCurrentInternalServiceProvider() => GetCurrentContext().InternalServiceProvider;

        /// <summary>
        ///     Override this method to override the options supplied in the constructor.
        ///     The resulting configuration may be cached and re-used during the entire lifespan of the context.
        /// </summary> 
        /// <remarks>The default implementation of this method does nothing.</remarks>
        /// <param name="options">The options of the <see cref="EventsContext"/>.</param>
        protected virtual void OnConfiguring(EventsContextOptions options) { }

        /// <summary>
        ///     Override this method to configure the event subscriptions.
        ///     The resulting configuration may be cached and re-used during the entire lifespan of the context.
        /// </summary>
        /// <remarks>The default implementation of this method does nothing.</remarks>
        /// <param name="subscriptionsBuilder">The builder that defines the model for the context being created.</param>
        protected virtual void OnBuildingSubscriptions(SubscriptionsBuilder subscriptionsBuilder) { }

        /// <summary>
        ///     Override this method to configure the pipelines needed for handling the events.
        ///     The resulting configuration may be cached and re-used during the entire lifespan of the context.
        /// </summary>
        /// <remarks>The default implementation of this method does nothing.</remarks>
        /// <param name="pipelinesBuilder">The builder that defines the model for the context being created.</param>
        protected virtual void OnBuildingPipelines(PipelinesBuilder pipelinesBuilder) { }

        /// <summary>
        ///     Attach an event source to the context in order to forward it's events to the configured pipelines.
        /// </summary>
        /// <param name="source">The event source.</param>
        /// <param name="eventsScope">The scope of the events published from this source.</param>
        public virtual void Attach(object source, EventsScope eventsScope)
            => GetCurrentInternalServiceProvider()
                .GetRequiredService<IAttachingService>()
                .Attach(source, eventsScope);

        /// <summary>
        ///     Continues the processing of the events that have been queued.
        /// </summary>
        /// <param name="eventsScope">The scope where the queued events were published.</param>
        /// <param name="queueName">
        ///     The name of the queue.
        ///     If null all the events will be processed.
        /// </param>
        public virtual Task ProcessQueuedEventsAsync(EventsScope eventsScope, string queueName = null)
            => GetCurrentInternalServiceProvider()
                .GetRequiredService<IEventsQueuesService>()
                .ProcessQueuedEventsAsync(eventsScope, queueName);

        /// <summary>
        ///     Discards all the events of a queue.
        /// </summary>
        /// <param name="eventsScope">The scope where the queued events were published.</param>
        /// <param name="queueName">
        ///     The name of the queue.
        ///     If null all the events will be discarded.
        /// </param>
        public virtual void DiscardQueuedEvents(EventsScope eventsScope, string queueName = null)
            => GetCurrentInternalServiceProvider()
                .GetRequiredService<IEventsQueuesService>()
                .DiscardQueuedEvents(eventsScope, queueName);
    }
}
