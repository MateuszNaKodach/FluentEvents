using System;
using FluentEvents.Azure.ServiceBus.Queues.Common;
using FluentEvents.Plugins;
using FluentEvents.Transmission;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace FluentEvents.Azure.ServiceBus.Queues.Receiving
{
    internal class AzureQueueEventReceiverPlugin : IFluentEventsPlugin
    {
        private readonly IConfiguration _configuration;
        private readonly Action<QueueEventReceiverConfig> _configureOptions;

        public AzureQueueEventReceiverPlugin(Action<QueueEventReceiverConfig> configureOptions)
        {
            _configureOptions = configureOptions ?? throw new ArgumentNullException(nameof(configureOptions));
        }

        public AzureQueueEventReceiverPlugin(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public void ApplyServices(IServiceCollection services)
        {
            if (_configureOptions != null)
                services.AddOptions<QueueEventReceiverConfig>().Configure(_configureOptions);
            else
                services.AddOptions<QueueEventReceiverConfig>().Bind(_configuration);

            services.AddTransient<IValidateOptions<QueueEventReceiverConfig>, QueueEventReceiverConfigValidator>();
            services.TryAddSingleton<IQueueClientFactory, QueueClientFactory>();
            services.AddSingleton<IEventReceiver, QueueEventReceiver>();
        }
    }
}