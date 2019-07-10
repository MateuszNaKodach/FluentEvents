using FluentEvents.Azure.ServiceBus.Common;
using Microsoft.Extensions.Options;

namespace FluentEvents.Azure.ServiceBus.Queues.Receiving
{
    internal class AzureServiceBusQueueEventReceiverConfigValidator : AzureServiceBusEventReceiverConfigValidatorBase
        , IValidateOptions<AzureServiceBusQueueEventReceiverConfig>
    {
        public ValidateOptionsResult Validate(string name, AzureServiceBusQueueEventReceiverConfig options)
        {
            return Validate(options);
        }
    }
}
