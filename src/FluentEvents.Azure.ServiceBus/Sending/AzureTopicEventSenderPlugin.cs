using System;
using FluentEvents.Plugins;
using FluentEvents.Transmission;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace FluentEvents.Azure.ServiceBus.Sending
{
    internal class AzureTopicEventSenderPlugin : IFluentEventsPlugin
    {
        private readonly IConfiguration _configuration;
        private readonly Action<AzureTopicEventSenderConfig> _configureOptions;

        public AzureTopicEventSenderPlugin(Action<AzureTopicEventSenderConfig> configureOptions)
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
                services.AddOptions<AzureTopicEventSenderConfig>().Configure(_configureOptions);
            else
                services.AddOptions<AzureTopicEventSenderConfig>().Bind(_configuration);

            services.AddTransient<IValidateOptions<AzureTopicEventSenderConfig>, AzureTopicEventSenderConfigValidator>();
            services.AddSingleton<ITopicClientFactory, TopicClientFactory>();
            services.AddSingleton<IEventSender, AzureTopicEventSender>();
        }
    }
}