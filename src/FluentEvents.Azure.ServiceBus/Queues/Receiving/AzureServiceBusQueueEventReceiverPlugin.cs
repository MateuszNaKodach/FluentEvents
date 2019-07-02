using System;
using FluentEvents.Azure.ServiceBus.Queues.Common;
using FluentEvents.Plugins;
using FluentEvents.Transmission;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FluentEvents.Azure.ServiceBus.Queues.Receiving
{
    internal class AzureServiceBusQueueEventReceiverPlugin : IFluentEventsPlugin
    {
        private readonly IConfiguration _configuration;
        private readonly Action<AzureServiceBusQueueEventReceiverConfig> _configureOptions;

        public AzureServiceBusQueueEventReceiverPlugin(Action<AzureServiceBusQueueEventReceiverConfig> configureOptions)
        {
            _configureOptions = configureOptions ?? throw new ArgumentNullException(nameof(configureOptions));
        }

        public AzureServiceBusQueueEventReceiverPlugin(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public void ApplyServices(IServiceCollection services)
        {
            if (_configureOptions != null)
                services.Configure(_configureOptions);
            else
                services.Configure<AzureServiceBusQueueEventReceiverConfig>(_configuration);

            services.TryAddSingleton<IQueueClientFactory, QueueClientFactory>();
            services.AddSingleton<IEventReceiver, AzureServiceBusQueueEventReceiver>();
        }
    }
}