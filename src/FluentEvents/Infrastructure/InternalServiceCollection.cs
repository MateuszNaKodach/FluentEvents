using System;
using System.Collections.Generic;
using FluentEvents.Config;
using FluentEvents.Model;
using FluentEvents.Pipelines;
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
        private readonly IAppServiceProvider _appServiceProvider;

        public InternalServiceCollection(IAppServiceProvider appServiceProvider)
        {
            _appServiceProvider = appServiceProvider;
        }

        public IServiceProvider BuildServiceProvider(EventsContext eventsContext, IFluentEventsPluginOptions options)
        {
            Func<IServiceProvider, object> GetLoggerFactory() =>
                x => _appServiceProvider.GetService<ILoggerFactory>() ??
                     new LoggerFactory(x.GetService<IEnumerable<ILoggerProvider>>());

            var services = new ServiceCollection();
            services.AddLogging();
            services.Replace(new ServiceDescriptor(
                typeof(ILoggerFactory),
                GetLoggerFactory(),
                ServiceLifetime.Singleton
            ));

            services.AddSingleton(_appServiceProvider);
            services.AddSingleton(eventsContext);
            services.AddSingleton<EventsQueuesContext>();
            services.AddSingleton<PipelinesBuilder>();
            services.AddSingleton<SubscriptionsBuilder>();
            services.AddSingleton<IEventsQueuesService, EventsQueuesService>();
            services.AddSingleton<IEventsContextDependencies, EventsContextDependencies>();
            services.AddSingleton<IScopedSubscriptionsService, ScopedSubscriptionsService>();
            services.AddSingleton<IRoutingService, RoutingService>();
            services.AddSingleton<IForwardingService, ForwardingService>();
            services.AddSingleton<IEventsQueueNamesService, EventsQueueNamesService>();
            services.AddSingleton<IEventsSerializationService, JsonEventsSerializationService>();
            services.AddSingleton<IGlobalSubscriptionsService, GlobalSubscriptionsService>();
            services.AddSingleton<ISourceModelsService, SourceModelsService>();
            services.AddSingleton<IPublishingService, PublishingService>();
            services.AddSingleton<IEventReceiversService, EventReceiversService>();
            services.AddSingleton<IAttachingService, AttachingService>();
            services.AddSingleton<ISubscriptionsMatchingService, SubscriptionsMatchingService>();
            services.AddSingleton<IPipelinesService, PipelinesService>();
            services.AddSingleton<EnqueuePipelineModule>();
            services.AddSingleton<FilterPipelineModule>();
            services.AddSingleton<GlobalPublishPipelineModule>();
            services.AddSingleton<ScopedPublishPipelineModule>();
            services.AddSingleton<ProjectionPipelineModule>();

            foreach (var plugin in options.Plugins)
                plugin.ApplyServices(services);

            return services.BuildServiceProvider();
        }
    }
}
