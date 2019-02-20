using System;
using FluentEvents.Infrastructure;
using FluentEvents.Transmission;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FluentEvents
{
    /// <summary>
    ///     Extension methods for configuring an <see cref="EventsContext"/> with Dependency Injection.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        ///     Registers the given <see cref="EventsContext"/> as a service in the <see cref="IServiceCollection" />.
        /// </summary>
        /// <typeparam name="T">The type of context to be registered.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> to register with.</param>
        /// <param name="optionsBuilder">
        ///     <para>
        ///         An action to configure the <see cref="EventsContextOptions" /> for the context. This provides an
        ///         alternative to performing configuration of the context by overriding the
        ///         <see cref="EventsContext.OnConfiguring" /> method in your derived context.
        ///     </para>
        ///     <para>
        ///         If an action is supplied here, the <see cref="EventsContext.OnConfiguring" /> method will still be run if it has
        ///         been overridden on the derived context. <see cref="EventsContext.OnConfiguring" /> configuration will be applied
        ///         in addition to configuration performed here.
        ///     </para>
        /// </param>
        /// <returns>The original <see cref="IServiceCollection"/>.</returns>
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
                return context;
            });

            services.AddSingleton<EventsContext, T>(x => x.GetRequiredService<T>());
            services.AddSingleton<IHostedService, EventReceiversHostedService>();
        
            return services;
        }

        /// <summary>
        ///     This method adds multiple services to a <see cref="IServiceCollection"/> and makes sure that
        ///     they are attached to the <see cref="EventsContext"/> when they are requested from
        ///     the <see cref="IServiceProvider"/>.
        /// </summary>
        /// <example>
        ///     <code>
        ///         public void ConfigureServices(IServiceCollection services)
        ///         {
        ///             services
        ///                 .AddWithEventsAttachedTo&lt;MyEventsContext&gt;(trackedServices => {
        ///                     trackedServices.AddScoped&lt;IMyService, MyService&gt;();
        ///                     trackedServices.AddScoped&lt;IMyService2, MyService2&gt;();
        ///                 });
        ///         }
        ///     </code>
        /// </example>
        /// <typeparam name="TEventsContext">The <see cref="EventsContext"/> where the services are attached.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> to register with.</param>
        /// <param name="configurator">An action to add the services to the <see cref="IServiceCollection"/>.</param>
        /// <returns>The original <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddWithEventsAttachedTo<TEventsContext>(
            this IServiceCollection services,
            Action<IServiceCollection> configurator
        )
            where TEventsContext : EventsContext
        {
            var trackedServices = new ServiceCollection();
            configurator(trackedServices);

            foreach (var trackedService in trackedServices)
                services.AddWithEventsAttachedTo<TEventsContext>(trackedService);

            return services;
        }

        private static void AddWithEventsAttachedTo<TEventsContext>(
            this IServiceCollection services, 
            ServiceDescriptor serviceDescriptor
        )
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