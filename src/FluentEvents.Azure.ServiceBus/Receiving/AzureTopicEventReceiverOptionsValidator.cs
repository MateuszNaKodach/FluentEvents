using Microsoft.Extensions.Options;

namespace FluentEvents.Azure.ServiceBus.Receiving
{
    internal class AzureTopicEventReceiverOptionsValidator : IValidateOptions<AzureTopicEventReceiverOptions>
    {
        public ValidateOptionsResult Validate(string name, AzureTopicEventReceiverOptions options)
        {
            if (!ConnectionStringValidator.IsValid(
                    options.ReceiveConnectionString,
                    nameof(options.ReceiveConnectionString),
                    out var receiveConnectionStringErrorMessage
                )
            )
                return ValidateOptionsResult.Fail(receiveConnectionStringErrorMessage);

            if (options.IsSubscriptionCreationEnabled && !ConnectionStringValidator.IsValid(
                    options.ManagementConnectionString,
                    nameof(options.ManagementConnectionString),
                    out var managementConnectionStringErrorMessage
                )
            )
                return ValidateOptionsResult.Fail(managementConnectionStringErrorMessage);

            if (string.IsNullOrWhiteSpace(options.SubscriptionName) && options.SubscriptionNameProvider == null)
                return ValidateOptionsResult.Fail(
                    $"{nameof(options.SubscriptionName)} and {nameof(options.SubscriptionNameProvider)} are null or empty," +
                    $" please specify at least one of the parameters"
                );

            if (string.IsNullOrWhiteSpace(options.TopicPath))
                return ValidateOptionsResult.Fail(
                    $"{nameof(options.TopicPath)} is null or empty"
                );

            return ValidateOptionsResult.Success;
        }
    }

}
