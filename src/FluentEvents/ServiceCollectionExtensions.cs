using System;
using System.Linq;
using FluentEvents.Configuration;
using FluentEvents.Infrastructure;
using FluentEvents.ServiceProviders;
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
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="services"/> or <paramref name="optionsBuilder"/> is <see langword="null"/>.
        /// </exception>
        public static IServiceCollection AddEventsContext<T>(
            this IServiceCollection services,
            Action<EventsContextOptions> optionsBuilder
        )
            where T : EventsContext
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (optionsBuilder == null) throw new ArgumentNullException(nameof(optionsBuilder));

            var options = new EventsContextOptions();
            optionsBuilder(options);

            services.TryAddScoped<EventsScope>();

            var eventsContextFactory = ActivatorUtilities.CreateFactory(
                typeof(T),
                new[] {typeof(EventsContextOptions)}
            );
            var eventsContextFactoryArgs = new object[] {options};
            services.AddSingleton(x => (T) eventsContextFactory(x, eventsContextFactoryArgs));

            services.AddScoped<IScopedAppServiceProvider, AppServiceProvider>();
            services.AddSingleton<IRootAppServiceProvider, AppServiceProvider>();

            services.AddTransient(x => x.GetRequiredService<T>().GetEventReceiversHostedService());
        
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
        /// <param name="addServices">An <see cref="Action"/> that add services to the <see cref="IServiceCollection"/>.</param>
        /// <returns>The original <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddWithEventsAttachedTo<TEventsContext>(
            this IServiceCollection services,
            Action addServices
        )
            where TEventsContext : EventsContext
        {
            if (addServices == null) throw new ArgumentNullException(nameof(addServices));

            var originalServices = services.ToArray();
            addServices();
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
            ServiceImplementation serviceImplementation;
            if (serviceDescriptor.ImplementationType != null)
                serviceImplementation = ServiceImplementation.Type;
            else if (serviceDescriptor.ImplementationFactory != null)
                serviceImplementation = ServiceImplementation.Factory;
            else if (serviceDescriptor.ImplementationInstance != null)
                serviceImplementation = ServiceImplementation.Instance;
            else
                throw new NotSupportedException();

            if (!serviceDescriptor.ServiceType.IsGenericTypeDefinition)
                services.Replace(new ServiceDescriptor(serviceDescriptor.ServiceType, x =>
                {
                    object service = null;

                    switch (serviceImplementation)
                    {
                        case ServiceImplementation.Type:
                            service = ActivatorUtilities.CreateInstance(x, serviceDescriptor.ImplementationType);
                            break;
                        case ServiceImplementation.Factory:
                            service = serviceDescriptor.ImplementationFactory(x);
                            break;
                        case ServiceImplementation.Instance:
                            service = serviceDescriptor.ImplementationInstance;
                            break;
                    }

                    AttachService<TEventsContext>(x, service, serviceDescriptor.ServiceType);

                    return service;
                }, serviceDescriptor.Lifetime));
        }

        private static void AttachService<TEventsContext>(IServiceProvider serviceProvider, object service, Type serviceType)
            where TEventsContext : EventsContext
        {

            if (InternalServiceCollection.ServicesToIgnoreWhenAttaching.All(x => x != serviceType))
            {
                var eventsContext = serviceProvider.GetRequiredService<TEventsContext>();
                var eventsScope = serviceProvider.GetRequiredService<EventsScope>();
                eventsContext.WatchSourceEvents(service, eventsScope);
            }
        }

        private enum ServiceImplementation
        {
            Type,
            Factory,
            Instance
        }
    }
}