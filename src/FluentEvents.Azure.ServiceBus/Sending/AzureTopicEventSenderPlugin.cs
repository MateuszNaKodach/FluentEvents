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
        private readonly IConfiguration m_Configuration;
        private readonly Action<AzureTopicEventSenderConfig> m_ConfigureOptions;

        public AzureTopicEventSenderPlugin(Action<AzureTopicEventSenderConfig> configureOptions)
        {
            m_ConfigureOptions = configureOptions ?? throw new ArgumentNullException(nameof(configureOptions));
        }

        public AzureTopicEventSenderPlugin(IConfiguration configuration)
        {
            m_Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public void ApplyServices(IServiceCollection services)
        {
            if (m_ConfigureOptions != null)
                services.Configure(m_ConfigureOptions);
            else
                services.Configure<AzureTopicEventSenderConfig>(m_Configuration);

            services.AddSingleton<IValidableConfig>(x =>
                x.GetRequiredService<IOptions<AzureTopicEventSenderConfig>>().Value
            );
            services.AddSingleton<ITopicClientFactory, TopicClientFactory>();
            services.AddSingleton<IEventSender, AzureTopicEventSender>();
        }
    }
}