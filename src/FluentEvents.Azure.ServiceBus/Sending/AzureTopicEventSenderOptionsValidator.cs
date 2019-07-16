using Microsoft.Extensions.Options;

namespace FluentEvents.Azure.ServiceBus.Sending
{
    internal class AzureTopicEventSenderOptionsValidator : IValidateOptions<AzureTopicEventSenderOptions>
    {
        public ValidateOptionsResult Validate(string name, AzureTopicEventSenderOptions options)
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
