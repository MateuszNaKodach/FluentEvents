using System;
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
                () => new EventsScope(GetCurrentInternalServiceProvider(), scopedAppServiceProvider)
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
            => GetCurrentInternalServiceProvider()
                .GetRequiredService<IAttachingService>()
                .Attach(source, _eventsScope.Value);

        /// <summary>
        ///     Forward the events of a queue to the corresponding pipelines.
        /// </summary>
        /// <param name="queueName">The name of the queue.</param>
        public virtual Task ProcessQueuedEventsAsync(string queueName = null)
            => GetCurrentInternalServiceProvider()
                .GetRequiredService<IEventsQueuesService>()
                .ProcessQueuedEventsAsync(_eventsScope.Value, queueName);

        /// <summary>
        ///     Discards all the events of a queue.
        /// </summary>
        /// <param name="queueName">The name of the queue.</param>
        public virtual void DiscardQueuedEvents(string queueName = null)
            => GetCurrentInternalServiceProvider()
                .GetRequiredService<IEventsQueuesService>()
                .DiscardQueuedEvents(_eventsScope.Value, queueName);
    }
}
