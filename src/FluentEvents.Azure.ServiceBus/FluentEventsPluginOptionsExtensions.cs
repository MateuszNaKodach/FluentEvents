using System;
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
        ///     An <see cref="Action"/> to configure the <see cref="TopicEventSenderConfig"/> for
        ///     the topic sender plugin.
        /// </param>
        /// <returns>The same instance of <see cref="IFluentEventsPluginOptions"/> for chaining.</returns>
        public static IFluentEventsPluginOptions AddAzureTopicSender(
            this IFluentEventsPluginOptions pluginOptions,
            Action<TopicEventSenderConfig> configureOptions
        )
        {
            pluginOptions.AddPlugin(new TopicSenderPlugin(configureOptions));
            return pluginOptions;
        }

        /// <summary>
        ///     Adds a plugin that sends publishes events to different instances of the application
        ///     using an Azure Service Bus topic.
        /// </summary>
        /// <param name="pluginOptions">The <see cref="EventsContext"/> options.</param>
        /// <param name="configuration">
        ///     A configuration section with the same structure of the <see cref="TopicEventSenderConfig"/> type.
        /// </param>
        /// <returns>The same instance of <see cref="IFluentEventsPluginOptions"/> for chaining.</returns>
        public static IFluentEventsPluginOptions AddAzureTopicSender(
            this IFluentEventsPluginOptions pluginOptions,
            IConfiguration configuration
        )
        {
            pluginOptions.AddPlugin(new TopicSenderPlugin(configuration));
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
        ///     An <see cref="Action"/> to configure the <see cref="TopicEventReceiverConfig"/> for the receiving plugin.
        /// </param>
        /// <returns>The same instance of <see cref="IFluentEventsPluginOptions"/> for chaining.</returns>
        public static IFluentEventsPluginOptions AddAzureTopicReceiver(
            this IFluentEventsPluginOptions pluginOptions,
            Action<TopicEventReceiverConfig> configureOptions
        )
        {
            pluginOptions.AddPlugin(new TopicReceiverPlugin(configureOptions));
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
        ///     A configuration section with the same structure of the <see cref="TopicEventReceiverConfig"/> type.
        /// </param>
        /// <returns>The same instance of <see cref="IFluentEventsPluginOptions"/> for chaining.</returns>
        public static IFluentEventsPluginOptions AddAzureTopicReceiver(
            this IFluentEventsPluginOptions pluginOptions,
            IConfiguration configuration
        )
        {
            pluginOptions.AddPlugin(new TopicReceiverPlugin(configuration));
            return pluginOptions;
        }
    }
}
