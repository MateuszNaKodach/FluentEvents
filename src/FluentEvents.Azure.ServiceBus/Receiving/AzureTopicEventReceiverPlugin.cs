using System;
using FluentEvents.Infrastructure;
using FluentEvents.Plugins;
using FluentEvents.Transmission;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace FluentEvents.Azure.ServiceBus.Receiving
{
    internal class AzureTopicEventReceiverPlugin : IFluentEventsPlugin
    {
        private readonly IConfiguration m_Configuration;
        private readonly Action<AzureTopicEventReceiverConfig> m_ConfigureOptions;

        public AzureTopicEventReceiverPlugin(Action<AzureTopicEventReceiverConfig> configureOptions)
        {
            m_ConfigureOptions = configureOptions ?? throw new ArgumentNullException(nameof(configureOptions));
        }

        public AzureTopicEventReceiverPlugin(IConfiguration configuration)
        {
            m_Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public void ApplyServices(IServiceCollection services)
        {
            if (m_ConfigureOptions != null)
                services.Configure(m_ConfigureOptions);
            else
                services.Configure<AzureTopicEventReceiverConfig>(m_Configuration);

            services.AddSingleton<IValidableConfig>(x =>
                x.GetRequiredService<IOptions<AzureTopicEventReceiverConfig>>().Value
            );
            services.AddSingleton<ITopicSubscriptionsService, TopicSubscriptionsService>();
            services.AddSingleton<ISubscriptionClientFactory, SubscriptionClientFactory>();
            services.AddSingleton<IEventReceiver, AzureTopicEventReceiver>();
        }
    }
}