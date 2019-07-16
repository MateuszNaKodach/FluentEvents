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
        ///         <see cref="EventsScope.OnConfiguring" /> method in your derived context.
        ///     </para>
        ///     <para>
        ///         If an action is supplied here, the <see cref="EventsScope.OnConfiguring" /> method will still be run if it has
        ///         been overridden on the derived context. <see cref="EventsScope.OnConfiguring" /> configuration will be applied
        ///         in addition to configuration performed here.
        ///     </para>
        /// </param>
        /// <returns>The original <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddEventsContext<T>(
            this IServiceCollection services,
            Action<EventsContextOptions> optionsBuilder
        )
            where T : EventsScope
        {
            var options = new EventsContextOptions();
            optionsBuilder(options);

            services.AddScoped<IScopedAppServiceProvider, AppServiceProvider>();
            services.AddSingleton<IAppServiceProvider, AppServiceProvider>();
            services.AddScoped(x => ActivatorUtilities.CreateInstance<T>(x, options));

            services.AddTransient<IHostedService>(x =>
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
            where TEventsContext : EventsScope
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
            where TEventsContext : EventsScope
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

                    AttachService<TEventsContext>(x, service);

                    return service;
                }, serviceDescriptor.Lifetime));
        }

        private static void AttachService<TEventsContext>(IServiceProvider x, object service)
            where TEventsContext : EventsScope
        {
            var eventsContext = x.GetRequiredService<TEventsContext>();

            if (!eventsContext.GetCurrentContext().IsInitializing)
                eventsContext.Attach(service);
        }

        private enum ServiceImplementation
        {
            Type,
            Factory,
            Instance
        }
    }
}