using System;
using FluentEvents.Azure.ServiceBus.Topics.Subscribing;
using FluentEvents.Plugins;
using FluentEvents.Transmission;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace FluentEvents.Azure.ServiceBus.Topics.Receiving
{
    internal class AzureTopicEventReceiverPlugin : IFluentEventsPlugin
    {
        private readonly IConfiguration _configuration;
        private readonly Action<TopicEventReceiverConfig> _configureOptions;

        public AzureTopicEventReceiverPlugin(Action<TopicEventReceiverConfig> configureOptions)
        {
            _configureOptions = configureOptions ?? throw new ArgumentNullException(nameof(configureOptions));
        }

        public AzureTopicEventReceiverPlugin(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public void ApplyServices(IServiceCollection services)
        {
            if (_configureOptions != null)
                services.AddOptions<TopicEventReceiverConfig>().Configure(_configureOptions);
            else
                services.AddOptions<TopicEventReceiverConfig>().Bind(_configuration);

            services.AddTransient<IValidateOptions<TopicEventReceiverConfig>, TopicEventReceiverConfigValidator>();
            services.AddSingleton<ITopicSubscriptionsService, TopicSubscriptionsService>();
            services.AddSingleton<ISubscriptionClientFactory, SubscriptionClientFactory>();
            services.AddSingleton<IEventReceiver, TopicEventReceiver>();
        }
    }
}