using System;
using System.Net.Http;
using FluentEvents.Configuration;
using FluentEvents.Plugins;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FluentEvents.Azure.SignalR
{
    /// <summary>
    ///     Extensions for <see cref="IEventsContextOptions"/>
    /// </summary>
    public static class EventsContextOptionsExtensions
    {
        /// <summary>
        ///     Adds a plugin that invokes hub methods on an Azure SignalR Service when events are published.
        /// </summary>
        /// <param name="options">The <see cref="EventsContext"/> options.</param>
        /// <param name="configureAction">
        ///     An <see cref="Action"/> to configure the <see cref="AzureSignalRServiceOptions"/> for
        ///     the topic sender plugin.
        /// </param>
        /// <returns>The same instance of <see cref="IEventsContextOptions"/> for chaining.</returns>
        public static IEventsContextOptions UseAzureSignalRService(
            this IEventsContextOptions options,
            Action<AzureSignalRServiceOptions> configureAction
        )
        {
            return options.UseAzureSignalRService(configureAction, null);
        }

        /// <summary>
        ///     Adds a plugin that invokes hub methods on an Azure SignalR Service when events are published.
        /// </summary>
        /// <param name="options">The <see cref="EventsContext"/> options.</param>
        /// <param name="configureAction">
        ///     An <see cref="Action"/> to configure the <see cref="AzureSignalRServiceOptions"/> for
        ///     the topic sender plugin.
        /// </param>
        /// <param name="httpClientBuilder">
        ///     An <see cref="Action{T}"/> to further configure the <see cref="HttpClient"/> used to make requests to
        ///     the Azure SignalR Service.
        /// </param>
        /// <returns>The same instance of <see cref="IEventsContextOptions"/> for chaining.</returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="options"/> and/or <paramref name="configureAction"/> are <see langword="null"/>.
        /// </exception>
        public static IEventsContextOptions UseAzureSignalRService(
            this IEventsContextOptions options,
            Action<AzureSignalRServiceOptions> configureAction,
            Action<IHttpClientBuilder> httpClientBuilder
        )
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            if (configureAction == null) throw new ArgumentNullException(nameof(configureAction));

            options.AddPlugin(new AzureSignalRPlugin(configureAction, httpClientBuilder));

            return options;
        }

        /// <summary>
        ///     Adds a plugin that invokes hub methods on an Azure SignalR Service when events are published.
        /// </summary>
        /// <param name="options">The <see cref="EventsContext"/> options.</param>
        /// <param name="configuration">
        ///     A configuration section with the same structure of the <see cref="AzureSignalRServiceOptions"/> type.
        /// </param>
        /// <returns>The same instance of <see cref="IEventsContextOptions"/> for chaining.</returns>
        public static IEventsContextOptions UseAzureSignalRService(
            this IEventsContextOptions options,
            IConfiguration configuration
        )
        {
            return options.UseAzureSignalRService(configuration, null);
        }

        /// <summary>
        ///     Adds a plugin that invokes hub methods on an Azure SignalR Service when events are published.
        /// </summary>
        /// <param name="options">The <see cref="EventsContext"/> options.</param>
        /// <param name="configuration">
        ///     A configuration section with the same structure of the <see cref="AzureSignalRServiceOptions"/> type.
        /// </param>
        /// <param name="httpClientBuilder">
        ///     An <see cref="Action{T}"/> to further configure the <see cref="HttpClient"/> used to make requests to
        ///     the Azure SignalR Service.
        /// </param>
        /// <returns>The same instance of <see cref="IEventsContextOptions"/> for chaining.</returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="options"/> and/or <paramref name="configuration"/> are <see langword="null"/>.
        /// </exception>
        public static IEventsContextOptions UseAzureSignalRService(
            this IEventsContextOptions options,
            IConfiguration configuration,
            Action<IHttpClientBuilder> httpClientBuilder
        )
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            options.AddPlugin(new AzureSignalRPlugin(configuration, httpClientBuilder));

            return options;
        }
    }
}
