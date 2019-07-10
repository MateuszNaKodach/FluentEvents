using FluentEvents.Azure.ServiceBus.Common;
using Microsoft.Extensions.Options;

namespace FluentEvents.Azure.ServiceBus.Queues.Sending
{
    internal class AzureServiceBusQueueEventSenderConfigValidator : AzureServiceBusEventSenderConfigValidatorBase
        , IValidateOptions<AzureServiceBusQueueEventSenderConfig>
    {
        public ValidateOptionsResult Validate(string name, AzureServiceBusQueueEventSenderConfig options)
        {
            return Validate(options);
        }
    }
}
