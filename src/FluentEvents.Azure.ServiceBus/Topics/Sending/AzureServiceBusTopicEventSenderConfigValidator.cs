using FluentEvents.Azure.ServiceBus.Common;
using Microsoft.Extensions.Options;

namespace FluentEvents.Azure.ServiceBus.Topics.Sending
{
    internal class AzureServiceBusTopicEventSenderConfigValidator
        : AzureServiceBusEventSenderConfigValidatorBase
            , IValidateOptions<AzureServiceBusTopicEventSenderConfig>
    {
        public ValidateOptionsResult Validate(string name, AzureServiceBusTopicEventSenderConfig options)
        {
            return Validate(options);
        }
    }
}