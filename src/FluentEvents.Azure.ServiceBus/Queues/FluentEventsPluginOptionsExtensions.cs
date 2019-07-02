using System;
using FluentEvents.Azure.ServiceBus.Queues.Receiving;
using FluentEvents.Azure.ServiceBus.Queues.Sending;
using FluentEvents.Plugins;
using Microsoft.Extensions.Configuration;

namespace FluentEvents.Azure.ServiceBus.Queues
{
    /// <summary>
    ///     Extensions for <see cref="IFluentEventsPluginOptions"/>
    /// </summary>
    public static class FluentEventsPluginOptionsExtensions
    {
        /// <summary>
        ///     Adds a plugin that sends publishes events to different instances of the application
        ///     using an Azure Service Bus queue.
        /// </summary>
        /// <param name="pluginOptions">The <see cref="EventsContext"/> options.</param>
        /// <param name="configureOptions">
        ///     An <see cref="Action"/> to configure the <see cref="AzureServiceBusQueueEventSenderConfig"/> for
        ///     the queue sender plugin.
        /// </param>
        /// <returns>The same instance of <see cref="IFluentEventsPluginOptions"/> for chaining.</returns>
        public static IFluentEventsPluginOptions UseAzureQueueEventSender(
            this IFluentEventsPluginOptions pluginOptions,
            Action<AzureServiceBusQueueEventSenderConfig> configureOptions
        )
        {
            pluginOptions.AddPlugin(new AzureServiceBusQueueEventSenderPlugin(configureOptions));
            return pluginOptions;
        }

        /// <summary>
        ///     Adds a plugin that sends publishes events to different instances of the application
        ///     using an Azure Service Bus queue.
        /// </summary>
        /// <param name="pluginOptions">The <see cref="EventsContext"/> options.</param>
        /// <param name="configuration">
        ///     A configuration section with the same structure of the <see cref="AzureServiceBusQueueEventSenderConfig"/> type.
        /// </param>
        /// <returns>The same instance of <see cref="IFluentEventsPluginOptions"/> for chaining.</returns>
        public static IFluentEventsPluginOptions UseAzureQueueEventSender(
            this IFluentEventsPluginOptions pluginOptions,
            IConfiguration configuration
        )
        {
            pluginOptions.AddPlugin(new AzureServiceBusQueueEventSenderPlugin(configuration));
            return pluginOptions;
        }

        /// <summary>
        ///     Adds a plugin that receives events published from other instances of the application
        ///     using an Azure Service Bus queue.
        /// </summary>
        /// <remarks>
        ///     The management connection string is required to dynamically create queue subscriptions.
        /// </remarks>
        /// <param name="pluginOptions">The <see cref="EventsContext"/> options.</param>
        /// <param name="configureOptions">
        ///     An <see cref="Action"/> to configure the <see cref="AzureServiceBusQueueEventReceiverConfig"/> for the receiving plugin.
        /// </param>
        /// <returns>The same instance of <see cref="IFluentEventsPluginOptions"/> for chaining.</returns>
        public static IFluentEventsPluginOptions UseAzureQueueEventReceiver(
            this IFluentEventsPluginOptions pluginOptions,
            Action<AzureServiceBusQueueEventReceiverConfig> configureOptions
        )
        {
            pluginOptions.AddPlugin(new AzureServiceBusQueueEventReceiverPlugin(configureOptions));
            return pluginOptions;
        }

        /// <summary>
        ///     Adds a plugin that receives events published from other instances of the application
        ///     using an Azure Service Bus queue.
        /// </summary>
        /// <remarks>
        ///     The management connection string is required to dynamically create queue subscriptions.
        /// </remarks>
        /// <param name="pluginOptions">The <see cref="EventsContext"/> options.</param>
        /// <param name="configuration">
        ///     A configuration section with the same structure of the <see cref="AzureServiceBusQueueEventReceiverConfig"/> type.
        /// </param>
        /// <returns>The same instance of <see cref="IFluentEventsPluginOptions"/> for chaining.</returns>
        public static IFluentEventsPluginOptions UseAzureQueueEventReceiver(
            this IFluentEventsPluginOptions pluginOptions,
            IConfiguration configuration
        )
        {
            pluginOptions.AddPlugin(new AzureServiceBusQueueEventReceiverPlugin(configuration));
            return pluginOptions;
        }
    }
}
