using System;
using FluentEvents.Plugins;
using FluentEvents.Transmission;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FluentEvents.Azure.ServiceBus.Sending
{
    internal class TopicSenderPlugin : IFluentEventsPlugin
    {
        private readonly IConfiguration m_Configuration;
        private readonly Action<TopicEventSenderConfig> m_ConfigureOptions;

        public TopicSenderPlugin(Action<TopicEventSenderConfig> configureOptions)
        {
            m_ConfigureOptions = configureOptions ?? throw new ArgumentNullException(nameof(configureOptions));
        }

        public TopicSenderPlugin(IConfiguration configuration)
        {
            m_Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public void ApplyServices(IServiceCollection services)
        {
            if (m_ConfigureOptions != null)
                services.Configure(m_ConfigureOptions);
            else
                services.Configure<TopicEventSenderConfig>(m_Configuration);

            services.AddSingleton<ITopicClientFactory, TopicClientFactory>();
            services.AddSingleton<IEventSender, TopicEventSender>();
        }
    }
}