using System;
using FluentEvents.Azure.ServiceBus.Queues.Common;
using FluentEvents.Infrastructure;
using FluentEvents.Plugins;
using FluentEvents.Transmission;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace FluentEvents.Azure.ServiceBus.Queues.Sending
{
    internal class AzureServiceBusQueueEventSenderPlugin : IFluentEventsPlugin
    {
        private readonly IConfiguration _configuration;
        private readonly Action<AzureServiceBusQueueEventSenderConfig> _configureOptions;

        public AzureServiceBusQueueEventSenderPlugin(Action<AzureServiceBusQueueEventSenderConfig> configureOptions)
        {
            _configureOptions = configureOptions ?? throw new ArgumentNullException(nameof(configureOptions));
        }

        public AzureServiceBusQueueEventSenderPlugin(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public void ApplyServices(IServiceCollection services)
        {
            if (_configureOptions != null)
                services.Configure(_configureOptions);
            else
                services.Configure<AzureServiceBusQueueEventSenderConfig>(_configuration);

            services.AddSingleton<IValidableConfig>(x =>
                x.GetRequiredService<IOptions<AzureServiceBusQueueEventSenderConfig>>().Value
            );
            services.TryAddSingleton<IQueueClientFactory, QueueClientFactory>();
            services.AddSingleton<IEventSender, AzureServiceBusQueueEventSender>();
        }
    }
}