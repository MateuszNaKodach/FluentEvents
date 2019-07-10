

using Microsoft.Extensions.Options;

namespace FluentEvents.Azure.ServiceBus.Common
{
    internal class AzureServiceBusEventSenderConfigValidatorBase
    {
        protected ValidateOptionsResult Validate(AzureServiceBusEventSenderConfigBase options)
        {
            if (!ConnectionStringValidator.IsValid(options.SendConnectionString, nameof(options.SendConnectionString), out var errorMessage))
                return ValidateOptionsResult.Fail(errorMessage);

            return ValidateOptionsResult.Success;
        }
    }
}