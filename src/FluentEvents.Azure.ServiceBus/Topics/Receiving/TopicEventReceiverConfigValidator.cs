using FluentEvents.Azure.ServiceBus.Common;
using Microsoft.Extensions.Options;

namespace FluentEvents.Azure.ServiceBus.Topics.Receiving
{
    internal class TopicEventReceiverConfigValidator : EventReceiverConfigValidatorBase
        , IValidateOptions<TopicEventReceiverConfig>
    {
        public ValidateOptionsResult Validate(string name, TopicEventReceiverConfig options)
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
                    $"{nameof(TopicEventReceiverConfig.SubscriptionNameGenerator)} is null"
                );

            if (string.IsNullOrWhiteSpace(options.TopicPath))
                return ValidateOptionsResult.Fail(
                    $"{nameof(TopicEventReceiverConfig.TopicPath)} is null or empty"
                );

            return Validate(options);
        }
    }
}
