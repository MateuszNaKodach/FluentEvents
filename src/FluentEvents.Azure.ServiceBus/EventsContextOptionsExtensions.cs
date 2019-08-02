using System;
using FluentEvents.Azure.ServiceBus.Receiving;
using FluentEvents.Azure.ServiceBus.Sending;
using FluentEvents.Configuration;
using FluentEvents.Plugins;
using Microsoft.Extensions.Configuration;

namespace FluentEvents.Azure.ServiceBus
{
    /// <summary>
    ///     Extensions for <see cref="IEventsContextOptions"/>
    /// </summary>
    public static class EventsContextOptionsExtensions
    {
        /// <summary>
        ///     Adds a plugin that sends publishes events to different instances of the application
        ///     using an Azure Service Bus topic.
        /// </summary>
        /// <param name="options">The <see cref="EventsContext"/> options.</param>
        /// <param name="configureOptions">
        ///     An <see cref="Action"/> to configure the <see cref="AzureTopicEventSenderOptions"/> for
        ///     the topic sender plugin.
        /// </param>
        /// <returns>The same instance of <see cref="IEventsContextOptions"/> for chaining.</returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="options"/> is <see langword="null"/>.
        /// </exception>
        public static IEventsContextOptions UseAzureTopicEventSender(
            this IEventsContextOptions options,
            Action<AzureTopicEventSenderOptions> configureOptions
        )
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            options.AddPlugin(new AzureTopicEventSenderPlugin(configureOptions));

            return options;
        }

        /// <summary>
        ///     Adds a plugin that sends publishes events to different instances of the application
        ///     using an Azure Service Bus topic.
        /// </summary>
        /// <param name="options">The <see cref="EventsContext"/> options.</param>
        /// <param name="configuration">
        ///     A configuration section with the same structure of the <see cref="AzureTopicEventSenderOptions"/> type.
        /// </param>
        /// <returns>The same instance of <see cref="IEventsContextOptions"/> for chaining.</returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="options"/> is <see langword="null"/>.
        /// </exception>
        public static IEventsContextOptions UseAzureTopicEventSender(
            this IEventsContextOptions options,
            IConfiguration configuration
        )
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            options.AddPlugin(new AzureTopicEventSenderPlugin(configuration));

            return options;
        }

        /// <summary>
        ///     Adds a plugin that receives events published from other instances of the application
        ///     using an Azure Service Bus topic.
        /// </summary>
        /// <remarks>
        ///     The management connection string is required to dynamically create topic subscriptions.
        /// </remarks>
        /// <param name="options">The <see cref="EventsContext"/> options.</param>
        /// <param name="configureOptions">
        ///     An <see cref="Action"/> to configure the <see cref="AzureTopicEventReceiverOptions"/> for the receiving plugin.
        /// </param>
        /// <returns>The same instance of <see cref="IEventsContextOptions"/> for chaining.</returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="options"/> is <see langword="null"/>.
        /// </exception>
        public static IEventsContextOptions UseAzureTopicEventReceiver(
            this IEventsContextOptions options,
            Action<AzureTopicEventReceiverOptions> configureOptions
        )
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            options.AddPlugin(new AzureTopicEventReceiverPlugin(configureOptions));

            return options;
        }

        /// <summary>
        ///     Adds a plugin that receives events published from other instances of the application
        ///     using an Azure Service Bus topic.
        /// </summary>
        /// <remarks>
        ///     The management connection string is required to dynamically create topic subscriptions.
        /// </remarks>
        /// <param name="options">The <see cref="EventsContext"/> options.</param>
        /// <param name="configuration">
        ///     A configuration section with the same structure of the <see cref="AzureTopicEventReceiverOptions"/> type.
        /// </param>
        /// <returns>The same instance of <see cref="IEventsContextOptions"/> for chaining.</returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="options"/> is <see langword="null"/>.
        /// </exception>
        public static IEventsContextOptions UseAzureTopicEventReceiver(
            this IEventsContextOptions options,
            IConfiguration configuration
        )
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            options.AddPlugin(new AzureTopicEventReceiverPlugin(configuration));

            return options;
        }
    }
}
