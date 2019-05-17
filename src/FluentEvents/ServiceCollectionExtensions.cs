using System;
using System.Linq;
using FluentEvents.Infrastructure;
using FluentEvents.Transmission;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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
                context.Configure(options, new InternalServiceCollection(new AppServiceProvider(x)));
                return context;
            });

            services.AddSingleton<EventsContext, T>(x => x.GetRequiredService<T>());
            services.AddSingleton<IHostedService>(x =>
            {
                var eventReceiversService = x.GetRequiredService<T>()
                    .Get<IServiceProvider>()
                    .GetRequiredService<IEventReceiversService>();

                return new EventReceiversHostedService(eventReceiversService);
            });
        
            return services;
        }

        /// <summary>
        ///     This method watches the services registered inside of the addServicesAction
        ///     and makes sure that they are attached to the <see cref="EventsContext"/>
        ///     when they are resolved by the <see cref="IServiceProvider"/>.
        /// </summary>
        /// <example>
        ///     <code>
        ///         public void ConfigureServices(IServiceCollection services)
        ///         {
        ///             services
        ///                 .AddWithEventsAttachedTo&lt;MyEventsContext&gt;(() => {
        ///                     services.AddScoped&lt;IMyService, MyService&gt;();
        ///                     services.AddSingleton&lt;IMyService2, MyService2&gt;();
        ///                 });
        ///         }
        ///     </code>
        /// </example>
        /// <typeparam name="TEventsContext">The <see cref="EventsContext"/> where the services are attached.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> to register with.</param>
        /// <param name="addServicesAction">An <see cref="Action"/> that add services to the <see cref="IServiceCollection"/>.</param>
        /// <returns>The original <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddWithEventsAttachedTo<TEventsContext>(
            this IServiceCollection services,
            Action addServicesAction
        )
            where TEventsContext : EventsContext
        {
            var originalServices = services.ToArray();
            addServicesAction();
            var addedServices = services.Where(x => originalServices.All(y => y != x)).ToArray();

            foreach (var addedService in addedServices)
                services.AddWithEventsAttachedTo<TEventsContext>(addedService);

            return services;
        }


        private static void AddWithEventsAttachedTo<TEventsContext>(
            this IServiceCollection services, 
            ServiceDescriptor serviceDescriptor
        )
            where TEventsContext : EventsContext
        {
            if (!serviceDescriptor.ServiceType.IsGenericTypeDefinition)
                services.Replace(new ServiceDescriptor(serviceDescriptor.ServiceType, x =>
                {
                    object service;

                    if (serviceDescriptor.ImplementationType != null)
                        service = ActivatorUtilities.CreateInstance(x, serviceDescriptor.ImplementationType);
                    else if (serviceDescriptor.ImplementationFactory != null)
                        service = serviceDescriptor.ImplementationFactory(x);
                    else if (serviceDescriptor.ImplementationInstance != null)
                        service = serviceDescriptor.ImplementationInstance;
                    else
                        throw new NotSupportedException();

                    AttachService<TEventsContext>(x, service);

                    return service;
                }, serviceDescriptor.Lifetime));
        }

        private static void AttachService<TEventsContext>(IServiceProvider x, object service)
            where TEventsContext : EventsContext
        {
            var eventsContext = x.GetRequiredService<TEventsContext>();
            var eventsScope = x.GetRequiredService<EventsScope>();

            if (!eventsContext.IsInitializing)
                eventsContext.Attach(service, eventsScope);
        }
    }
}