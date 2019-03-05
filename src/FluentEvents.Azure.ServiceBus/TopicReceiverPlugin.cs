using System;
using FluentEvents.Plugins;
using FluentEvents.Transmission;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FluentEvents.Azure.ServiceBus
{
    internal class TopicReceiverPlugin : IFluentEventsPlugin
    {
        private readonly IConfiguration m_Configuration;
        private readonly Action<TopicEventReceiverConfig> m_ConfigureOptions;

        public TopicReceiverPlugin(Action<TopicEventReceiverConfig> configureOptions)
        {
            m_ConfigureOptions = configureOptions ?? throw new ArgumentNullException(nameof(configureOptions));
        }

        public TopicReceiverPlugin(IConfiguration configuration)
        {
            m_Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public void ApplyServices(IServiceCollection services)
        {
            if (m_ConfigureOptions != null)
                services.Configure(m_ConfigureOptions);
            else
                services.Configure<TopicEventReceiverConfig>(m_Configuration);

            services.AddSingleton<IEventReceiver, TopicEventReceiver>();
        }
    }
}