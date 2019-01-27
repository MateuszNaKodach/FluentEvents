using System;
using System.Collections.Generic;
using FluentEvents.Config;
using FluentEvents.Plugins;
using FluentEvents.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using FluentEvents.Model;
using FluentEvents.Pipelines.Filters;
using FluentEvents.Pipelines.Publication;
using FluentEvents.Pipelines.Projections;
using FluentEvents.Queues;
using FluentEvents.Routing;
using FluentEvents.Subscriptions;
using FluentEvents.Transmission;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace FluentEvents
{
    internal class InternalServiceCollection : IInternalServiceCollection
    {
        private readonly IServiceProvider m_AppServiceProvider;

        public InternalServiceCollection(IServiceProvider appServiceProvider)
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

            services.AddSingleton(eventsContext);
            services.AddSingleton<IInfrastructureEventsContext>(eventsContext);
            services.AddSingleton<EventsQueuesContext>();
            services.AddSingleton<PipelinesBuilder>();
            services.AddSingleton<SubscriptionsBuilder>();
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
            services.AddSingleton<ITypesResolutionService, DefaultTypesResolutionService>();
            services.AddSingleton<IAttachingService, AttachingService>();
            services.AddSingleton<ISubscriptionsMatchingService, SubscriptionsMatchingService>();
            services.AddSingleton<FilterPipelineModule>();
            services.AddSingleton<GlobalPublishPipelineModule>();
            services.AddSingleton<ScopedPublishPipelineModule>();
            services.AddSingleton<ProjectionPipelineModule>();

            foreach (var extension in options.Plugins)
                extension.ApplyServices(services, m_AppServiceProvider);

            return services.BuildServiceProvider();
        }
    }
}
