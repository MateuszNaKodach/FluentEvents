using System;
using FluentEvents.Plugins;
using FluentEvents.Transmission;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FluentEvents.Azure.ServiceBus
{
    internal class TopicSenderPlugin : IFluentEventsPlugin
    {
        private readonly IConfiguration m_Configuration;
        private readonly Action<TopicEventSenderConfig> m_ConfigureOptions;

        public TopicSenderPlugin(Action<TopicEventSenderConfig> configureOptions)
        {
            m_ConfigureOptions = configureOptions;
        }

        public TopicSenderPlugin(IConfiguration configuration)
        {
            m_Configuration = configuration;
        }

        public void ApplyServices(IServiceCollection services, IServiceProvider appServiceProvider)
        {
            if (m_ConfigureOptions != null)
                services.Configure(m_ConfigureOptions);
            else
                services.Configure<TopicEventSenderConfig>(m_Configuration);

            services.AddSingleton<ITopicEventSender, TopicEventSender>();
            services.AddSingleton<IEventSender>(x => x.GetRequiredService<ITopicEventSender>());
        }
    }
}