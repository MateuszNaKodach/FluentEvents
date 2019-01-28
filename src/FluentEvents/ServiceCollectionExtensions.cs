using System;
using FluentEvents.Transmission;
using FluentEvents.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FluentEvents
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEventsContext<T>(
            this IServiceCollection services,
            Action<EventsContextOptions> optionsBuilder
        )
            where T : EventsContext
        {
            var options = new EventsContextOptions();
            optionsBuilder(options);

            services.AddScoped<EventsScope>();
            services.AddSingleton(x =>
            {
                var context = ActivatorUtilities.CreateInstance<T>(x);
                context.Configure(options, new InternalServiceCollection(x));
                context.Build();
                return context;
            });

            services.AddSingleton<EventsContext, T>(x => x.GetRequiredService<T>());
            services.AddSingleton<IHostedService, EventReceiversHostedService>();
        
            return services;
        }

        public static IServiceCollection AddWithEventsAttachedTo<TEventsContext>(this IServiceCollection services, Action<IServiceCollection> configurator)
            where TEventsContext : EventsContext
        {
            var trackedServices = new ServiceCollection();
            configurator(trackedServices);

            foreach (var trackedService in trackedServices)
                services.AddWithEventsAttachedTo<TEventsContext>(trackedService);

            return services;
        }

        private static void AddWithEventsAttachedTo<TEventsContext>(this IServiceCollection services, ServiceDescriptor serviceDescriptor)
            where TEventsContext : EventsContext
        {
            services.Add(new ServiceDescriptor(serviceDescriptor.ServiceType, x =>
            {
                var service = ActivatorUtilities.CreateInstance(x, serviceDescriptor.ImplementationType);
                var eventsContext = x.GetRequiredService<TEventsContext>();
                var eventsScope = x.GetRequiredService<EventsScope>();
                eventsContext.Attach(service, eventsScope);

                return service;

            }, serviceDescriptor.Lifetime));
        }
    }
}