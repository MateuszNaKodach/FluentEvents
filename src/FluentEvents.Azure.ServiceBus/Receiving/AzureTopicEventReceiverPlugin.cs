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
        private readonly Action<AzureTopicEventReceiverOptions> _configureOptions;

        public AzureTopicEventReceiverPlugin(Action<AzureTopicEventReceiverOptions> configureOptions)
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
                services.AddOptions<AzureTopicEventReceiverOptions>().Configure(_configureOptions);
            else
                services.AddOptions<AzureTopicEventReceiverOptions>().Bind(_configuration);

            services.AddTransient<IValidateOptions<AzureTopicEventReceiverOptions>, AzureTopicEventReceiverOptionsValidator>();
            services.AddSingleton<ITopicSubscriptionsService, TopicSubscriptionsService>();
            services.AddSingleton<ISubscriptionClientFactory, SubscriptionClientFactory>();
            services.AddSingleton<IEventReceiver, AzureTopicEventReceiver>();
        }
    }
}