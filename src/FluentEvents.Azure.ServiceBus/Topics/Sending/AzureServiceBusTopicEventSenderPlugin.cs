using System;
using FluentEvents.Infrastructure;
using FluentEvents.Plugins;
using FluentEvents.Transmission;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace FluentEvents.Azure.ServiceBus.Topics.Sending
{
    internal class AzureServiceBusTopicEventSenderPlugin : IFluentEventsPlugin
    {
        private readonly IConfiguration _configuration;
        private readonly Action<AzureServiceBusTopicEventSenderConfig> _configureOptions;

        public AzureServiceBusTopicEventSenderPlugin(Action<AzureServiceBusTopicEventSenderConfig> configureOptions)
        {
            _configureOptions = configureOptions ?? throw new ArgumentNullException(nameof(configureOptions));
        }

        public AzureServiceBusTopicEventSenderPlugin(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public void ApplyServices(IServiceCollection services)
        {
            if (_configureOptions != null)
                services.Configure(_configureOptions);
            else
                services.Configure<AzureServiceBusTopicEventSenderConfig>(_configuration);

            services.AddSingleton<ITopicClientFactory, TopicClientFactory>();
            services.AddSingleton<IEventSender, AzureServiceBusTopicEventSender>();
        }
    }
}