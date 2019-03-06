using System;
using FluentEvents.Azure.ServiceBus.Receiving;
using FluentEvents.Azure.ServiceBus.Sending;
using FluentEvents.Plugins;
using Microsoft.Extensions.Configuration;

namespace FluentEvents.Azure.ServiceBus
{
    /// <summary>
    ///     Extensions for <see cref="IFluentEventsPluginOptions"/>
    /// </summary>
    public static class FluentEventsPluginOptionsExtensions
    {
        /// <summary>
        ///     Adds a plugin that sends publishes events to different instances of the application
        ///     using an Azure Service Bus topic.
        /// </summary>
        /// <param name="pluginOptions">The <see cref="EventsContext"/> options.</param>
        /// <param name="configureOptions">
        ///     An <see cref="Action"/> to configure the <see cref="AzureTopicEventSenderConfig"/> for
        ///     the topic sender plugin.
        /// </param>
        /// <returns>The same instance of <see cref="IFluentEventsPluginOptions"/> for chaining.</returns>
        public static IFluentEventsPluginOptions UseAzureTopicEventSender(
            this IFluentEventsPluginOptions pluginOptions,
            Action<AzureTopicEventSenderConfig> configureOptions
        )
        {
            pluginOptions.AddPlugin(new AzureTopicEventSenderPlugin(configureOptions));
            return pluginOptions;
        }

        /// <summary>
        ///     Adds a plugin that sends publishes events to different instances of the application
        ///     using an Azure Service Bus topic.
        /// </summary>
        /// <param name="pluginOptions">The <see cref="EventsContext"/> options.</param>
        /// <param name="configuration">
        ///     A configuration section with the same structure of the <see cref="AzureTopicEventSenderConfig"/> type.
        /// </param>
        /// <returns>The same instance of <see cref="IFluentEventsPluginOptions"/> for chaining.</returns>
        public static IFluentEventsPluginOptions UseAzureTopicEventSender(
            this IFluentEventsPluginOptions pluginOptions,
            IConfiguration configuration
        )
        {
            pluginOptions.AddPlugin(new AzureTopicEventSenderPlugin(configuration));
            return pluginOptions;
        }

        /// <summary>
        ///     Adds a plugin that receives events published from other instances of the application
        ///     using an Azure Service Bus topic.
        /// </summary>
        /// <remarks>
        ///     The management connection string is required to dynamically create topic subscriptions.
        /// </remarks>
        /// <param name="pluginOptions">The <see cref="EventsContext"/> options.</param>
        /// <param name="configureOptions">
        ///     An <see cref="Action"/> to configure the <see cref="AzureTopicEventReceiverConfig"/> for the receiving plugin.
        /// </param>
        /// <returns>The same instance of <see cref="IFluentEventsPluginOptions"/> for chaining.</returns>
        public static IFluentEventsPluginOptions UseAzureTopicEventReceiver(
            this IFluentEventsPluginOptions pluginOptions,
            Action<AzureTopicEventReceiverConfig> configureOptions
        )
        {
            pluginOptions.AddPlugin(new AzureTopicEventReceiverPlugin(configureOptions));
            return pluginOptions;
        }

        /// <summary>
        ///     Adds a plugin that receives events published from other instances of the application
        ///     using an Azure Service Bus topic.
        /// </summary>
        /// <remarks>
        ///     The management connection string is required to dynamically create topic subscriptions.
        /// </remarks>
        /// <param name="pluginOptions">The <see cref="EventsContext"/> options.</param>
        /// <param name="configuration">
        ///     A configuration section with the same structure of the <see cref="AzureTopicEventReceiverConfig"/> type.
        /// </param>
        /// <returns>The same instance of <see cref="IFluentEventsPluginOptions"/> for chaining.</returns>
        public static IFluentEventsPluginOptions UseAzureTopicEventReceiver(
            this IFluentEventsPluginOptions pluginOptions,
            IConfiguration configuration
        )
        {
            pluginOptions.AddPlugin(new AzureTopicEventReceiverPlugin(configuration));
            return pluginOptions;
        }
    }
}
