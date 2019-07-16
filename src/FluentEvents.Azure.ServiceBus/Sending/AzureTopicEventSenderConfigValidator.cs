using Microsoft.Extensions.Options;

namespace FluentEvents.Azure.ServiceBus.Sending
{
    internal class AzureTopicEventSenderConfigValidator : IValidateOptions<AzureTopicEventSenderConfig>
    {
        public ValidateOptionsResult Validate(string name, AzureTopicEventSenderConfig options)
        {
            if (!ConnectionStringValidator.IsValid(
                    options.SendConnectionString,
                    nameof(options.SendConnectionString),
                    out var errorMessage
                )
            )
                return ValidateOptionsResult.Fail(errorMessage);

            return ValidateOptionsResult.Success;
        }
    }
}
