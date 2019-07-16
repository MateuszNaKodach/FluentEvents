using Microsoft.Extensions.Options;

namespace FluentEvents.Azure.ServiceBus.Receiving
{
    internal class AzureTopicEventReceiverConfigValidator : IValidateOptions<AzureTopicEventReceiverConfig>
    {
        public ValidateOptionsResult Validate(string name, AzureTopicEventReceiverConfig options)
        {
            if (!ConnectionStringValidator.IsValid(
                    options.ReceiveConnectionString,
                    nameof(options.ReceiveConnectionString),
                    out var receiveConnectionStringErrorMessage
                )
            )
                return ValidateOptionsResult.Fail(receiveConnectionStringErrorMessage);

            if (!ConnectionStringValidator.IsValid(
                    options.ManagementConnectionString,
                    nameof(options.ManagementConnectionString),
                    out var managementConnectionStringErrorMessage
                )
            )
                return ValidateOptionsResult.Fail(managementConnectionStringErrorMessage);

            if (options.SubscriptionNameGenerator == null)
                return ValidateOptionsResult.Fail(
                    $"{nameof(AzureTopicEventReceiverConfig.SubscriptionNameGenerator)} is null"
                );

            if (string.IsNullOrWhiteSpace(options.TopicPath))
                return ValidateOptionsResult.Fail(
                    $"{nameof(AzureTopicEventReceiverConfig.TopicPath)} is null or empty"
                );

            return ValidateOptionsResult.Success;
        }
    }

}
