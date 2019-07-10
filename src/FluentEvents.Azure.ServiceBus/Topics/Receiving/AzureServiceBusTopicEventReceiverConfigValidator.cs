using FluentEvents.Azure.ServiceBus.Common;
using Microsoft.Extensions.Options;

namespace FluentEvents.Azure.ServiceBus.Topics.Receiving
{
    internal class AzureServiceBusTopicEventReceiverConfigValidator : AzureServiceBusEventReceiverConfigValidatorBase
        , IValidateOptions<AzureServiceBusTopicEventReceiverConfig>
    {
        public ValidateOptionsResult Validate(string name, AzureServiceBusTopicEventReceiverConfig options)
        {
            if (!ConnectionStringValidator.IsValid(
                    options.ManagementConnectionString,
                    nameof(options.ManagementConnectionString),
                    out var errorMessage
                )
            )
                return ValidateOptionsResult.Fail(errorMessage);

            if (options.SubscriptionNameGenerator == null)
                return ValidateOptionsResult.Fail(
                    $"{nameof(AzureServiceBusTopicEventReceiverConfig.SubscriptionNameGenerator)} is null"
                );

            if (string.IsNullOrWhiteSpace(options.TopicPath))
                return ValidateOptionsResult.Fail(
                    $"{nameof(AzureServiceBusTopicEventReceiverConfig.TopicPath)} is null or empty"
                );

            return Validate(options);
        }
    }
}
