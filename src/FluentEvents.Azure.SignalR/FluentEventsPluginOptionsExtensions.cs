using System;
using System.Net.Http;
using FluentEvents.Plugins;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FluentEvents.Azure.SignalR
{
    /// <summary>
    ///     Extensions for <see cref="IFluentEventsPluginOptions"/>
    /// </summary>
    public static class FluentEventsPluginOptionsExtensions
    {
        /// <summary>
        ///     Adds a plugin that invokes hub methods on an Azure SignalR Service when events are published.
        /// </summary>
        /// <param name="pluginOptions">The <see cref="EventsContext"/> options.</param>
        /// <param name="configureAction">
        ///     An <see cref="Action"/> to configure the <see cref="AzureSignalRServiceOptions"/> for
        ///     the topic sender plugin.
        /// </param>
        /// <param name="httpClientBuilderAction">
        ///     An <see cref="Action{T}"/> to further configure the <see cref="HttpClient"/> used to make requests to
        ///     the Azure SignalR Service.
        /// </param>
        /// <returns>The same instance of <see cref="IFluentEventsPluginOptions"/> for chaining.</returns>
        public static IFluentEventsPluginOptions UseAzureSignalRService(
            this IFluentEventsPluginOptions pluginOptions,
            Action<AzureSignalRServiceOptions> configureAction,
            Action<IHttpClientBuilder> httpClientBuilderAction = null
        )
        {
            if (configureAction == null) throw new ArgumentNullException(nameof(configureAction));

            pluginOptions.AddPlugin(new AzureSignalRPlugin(configureAction, httpClientBuilderAction));

            return pluginOptions;
        }

        /// <summary>
        ///     Adds a plugin that invokes hub methods on an Azure SignalR Service when events are published.
        /// </summary>
        /// <param name="pluginOptions">The <see cref="EventsContext"/> options.</param>
        /// <param name="configuration">
        ///     A configuration section with the same structure of the <see cref="AzureSignalRServiceOptions"/> type.
        /// </param>
        /// <param name="httpClientBuilderAction">
        ///     An <see cref="Action{T}"/> to further configure the <see cref="HttpClient"/> used to make requests to
        ///     the Azure SignalR Service.
        /// </param>
        /// <returns>The same instance of <see cref="IFluentEventsPluginOptions"/> for chaining.</returns>
        public static IFluentEventsPluginOptions UseAzureSignalRService(
            this IFluentEventsPluginOptions pluginOptions,
            IConfiguration configuration,
            Action<IHttpClientBuilder> httpClientBuilderAction = null
        )
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            pluginOptions.AddPlugin(new AzureSignalRPlugin(configuration, httpClientBuilderAction));

            return pluginOptions;
        }
    }
}
