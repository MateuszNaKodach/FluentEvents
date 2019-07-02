using System;
using FluentEvents.Azure.ServiceBus.Topics.Subscribing;
using FluentEvents.Infrastructure;
using FluentEvents.Plugins;
using FluentEvents.Transmission;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace FluentEvents.Azure.ServiceBus.Topics.Receiving
{
    internal class AzureServiceBusTopicEventReceiverPlugin : IFluentEventsPlugin
    {
        private readonly IConfiguration _configuration;
        private readonly Action<AzureServiceBusTopicEventReceiverConfig> _configureOptions;

        public AzureServiceBusTopicEventReceiverPlugin(Action<AzureServiceBusTopicEventReceiverConfig> configureOptions)
        {
            _configureOptions = configureOptions ?? throw new ArgumentNullException(nameof(configureOptions));
        }

        public AzureServiceBusTopicEventReceiverPlugin(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public void ApplyServices(IServiceCollection services)
        {
            if (_configureOptions != null)
                services.Configure(_configureOptions);
            else
                services.Configure<AzureServiceBusTopicEventReceiverConfig>(_configuration);

            services.AddSingleton<IValidableConfig>(x =>
                x.GetRequiredService<IOptions<AzureServiceBusTopicEventReceiverConfig>>().Value
            );
            services.AddSingleton<ITopicSubscriptionsService, TopicSubscriptionsService>();
            services.AddSingleton<ISubscriptionClientFactory, SubscriptionClientFactory>();
            services.AddSingleton<IEventReceiver, AzureServiceBusTopicEventReceiver>();
        }
    }
}