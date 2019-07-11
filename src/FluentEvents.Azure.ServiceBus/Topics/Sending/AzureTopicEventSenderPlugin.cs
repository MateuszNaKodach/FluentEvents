using System;
using FluentEvents.Plugins;
using FluentEvents.Transmission;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace FluentEvents.Azure.ServiceBus.Topics.Sending
{
    internal class AzureTopicEventSenderPlugin : IFluentEventsPlugin
    {
        private readonly IConfiguration _configuration;
        private readonly Action<TopicEventSenderConfig> _configureOptions;

        public AzureTopicEventSenderPlugin(Action<TopicEventSenderConfig> configureOptions)
        {
            _configureOptions = configureOptions ?? throw new ArgumentNullException(nameof(configureOptions));
        }

        public AzureTopicEventSenderPlugin(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public void ApplyServices(IServiceCollection services)
        {
            if (_configureOptions != null)
                services.AddOptions<TopicEventSenderConfig>().Configure(_configureOptions);
            else
                services.AddOptions<TopicEventSenderConfig>().Bind(_configuration);

            services.AddTransient<IValidateOptions<TopicEventSenderConfig>, TopicEventSenderConfigValidator>();
            services.AddSingleton<ITopicClientFactory, TopicClientFactory>();
            services.AddSingleton<IEventSender, TopicEventSender>();
        }
    }
}