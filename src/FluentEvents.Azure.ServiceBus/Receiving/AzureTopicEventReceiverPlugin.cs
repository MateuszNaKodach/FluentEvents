using System;
using FluentEvents.Plugins;
using FluentEvents.Transmission;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace FluentEvents.Azure.ServiceBus.Receiving
{
    internal class AzureTopicEventReceiverPlugin : IFluentEventsPlugin
    {
        private readonly IConfiguration _configuration;
        private readonly Action<AzureTopicEventReceiverConfig> _configureOptions;

        public AzureTopicEventReceiverPlugin(Action<AzureTopicEventReceiverConfig> configureOptions)
        {
            _configureOptions = configureOptions ?? throw new ArgumentNullException(nameof(configureOptions));
        }

        public AzureTopicEventReceiverPlugin(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public void ApplyServices(IServiceCollection services)
        {
            if (_configureOptions != null)
                services.AddOptions<AzureTopicEventReceiverConfig>().Configure(_configureOptions);
            else
                services.AddOptions<AzureTopicEventReceiverConfig>().Bind(_configuration);

            services.AddTransient<IValidateOptions<AzureTopicEventReceiverConfig>, AzureTopicEventReceiverConfigValidator>();
            services.AddSingleton<ITopicSubscriptionsService, TopicSubscriptionsService>();
            services.AddSingleton<ISubscriptionClientFactory, SubscriptionClientFactory>();
            services.AddSingleton<IEventReceiver, AzureTopicEventReceiver>();
        }
    }
}