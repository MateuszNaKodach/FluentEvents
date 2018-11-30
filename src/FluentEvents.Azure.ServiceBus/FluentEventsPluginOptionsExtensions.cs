using System;
using FluentEvents.Plugins;
using Microsoft.Extensions.Configuration;

namespace FluentEvents.Azure.ServiceBus
{
    public static class FluentEventsPluginOptionsExtensions
    {
        public static IFluentEventsPluginOptions AddAzureTopicSender(
            this IFluentEventsPluginOptions pluginOptions,
            Action<TopicEventSenderConfig> configureOptions
        )
        {
            pluginOptions.AddPlugin(new TopicSenderPlugin(configureOptions));
            return pluginOptions;
        }

        public static IFluentEventsPluginOptions AddAzureTopicSender(
            this IFluentEventsPluginOptions pluginOptions,
            IConfiguration configuration
        )
        {
            pluginOptions.AddPlugin(new TopicSenderPlugin(configuration));
            return pluginOptions;
        }

        public static IFluentEventsPluginOptions AddAzureTopicReceiver(
            this IFluentEventsPluginOptions pluginOptions,
            Action<TopicEventReceiverConfig> configureOptions
        )
        {
            pluginOptions.AddPlugin(new TopicReceiverPlugin(configureOptions));
            return pluginOptions;
        }

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
