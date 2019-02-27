using System;
using System.Collections.Generic;
using FluentEvents.Config;
using FluentEvents.Model;
using FluentEvents.Pipelines.Filters;
using FluentEvents.Pipelines.Projections;
using FluentEvents.Pipelines.Publication;
using FluentEvents.Pipelines.Queues;
using FluentEvents.Plugins;
using FluentEvents.Queues;
using FluentEvents.Routing;
using FluentEvents.Subscriptions;
using FluentEvents.Transmission;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace FluentEvents.Infrastructure
{
    internal class InternalServiceCollection : IInternalServiceCollection
    {
        private readonly IAppServiceProvider m_AppServiceProvider;

        public InternalServiceCollection(IAppServiceProvider appServiceProvider)
        {
            m_AppServiceProvider = appServiceProvider;
        }

        public IServiceProvider BuildServiceProvider(EventsContext eventsContext, IFluentEventsPluginOptions options)
        {
            Func<IServiceProvider, object> GetLoggerFactory() =>
                x => m_AppServiceProvider.GetService<ILoggerFactory>() ??
                     new LoggerFactory(x.GetService<IEnumerable<ILoggerProvider>>());

            var services = new ServiceCollection();
            services.AddLogging();
            services.Replace(new ServiceDescriptor(
                typeof(ILoggerFactory),
                GetLoggerFactory(),
                ServiceLifetime.Singleton
            ));

            services.AddSingleton(m_AppServiceProvider);
            services.AddSingleton(eventsContext);
            services.AddSingleton<EventsQueuesContext>();
            services.AddSingleton<PipelinesBuilder>();
            services.AddSingleton<SubscriptionsBuilder>();
            services.AddSingleton<ISubscriptionScanService, SubscriptionScanService>();
            services.AddSingleton<IEventsQueuesService, EventsQueuesService>();
            services.AddSingleton<IEventsContextDependencies, EventsContextDependencies>();
            services.AddSingleton<IScopedSubscriptionsService, ScopedSubscriptionsService>();
            services.AddSingleton<ISubscriptionsFactory, SubscriptionsFactory>();
            services.AddSingleton<IRoutingService, RoutingService>();
            services.AddSingleton<IForwardingService, ForwardingService>();
            services.AddSingleton<IEventsQueueNamesService, EventsQueueNamesService>();
            services.AddSingleton<IEventsSerializationService, JsonEventsSerializationService>();
            services.AddSingleton<IGlobalSubscriptionCollection, GlobalSubscriptionCollection>();
            services.AddSingleton<ISourceModelsService, SourceModelsService>();
            services.AddSingleton<IPublishingService, PublishingService>();
            services.AddSingleton<IEventReceiversService, EventReceiversService>();
            services.AddSingleton<IAttachingService, AttachingService>();
            services.AddSingleton<ISubscriptionsMatchingService, SubscriptionsMatchingService>();
            services.AddSingleton<EnqueuePipelineModule>();
            services.AddSingleton<FilterPipelineModule>();
            services.AddSingleton<GlobalPublishPipelineModule>();
            services.AddSingleton<ScopedPublishPipelineModule>();
            services.AddSingleton<ProjectionPipelineModule>();

            foreach (var extension in options.Plugins)
                extension.ApplyServices(services);

            return services.BuildServiceProvider();
        }
    }
}
