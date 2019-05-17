using System;
using FluentEvents.Infrastructure;
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
                services.Configure(_configureOptions);
            else
                services.Configure<AzureTopicEventSenderConfig>(_configuration);

            services.AddSingleton<IValidableConfig>(x =>
                x.GetRequiredService<IOptions<AzureTopicEventSenderConfig>>().Value
            );
            services.AddSingleton<ITopicClientFactory, TopicClientFactory>();
            services.AddSingleton<IEventSender, AzureTopicEventSender>();
        }
    }
}