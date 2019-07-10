using Microsoft.Extensions.Options;

namespace FluentEvents.Azure.ServiceBus.Common
{
    internal abstract class AzureServiceBusEventReceiverConfigValidatorBase
    {
        protected ValidateOptionsResult Validate(AzureServiceBusEventReceiverConfigBase options)
        {
            if (!ConnectionStringValidator.IsValid(
                    options.ReceiveConnectionString,
                    nameof(options.ReceiveConnectionString),
                    out var errorMessage
                )
            )
                return ValidateOptionsResult.Fail(errorMessage);

            return ValidateOptionsResult.Success;
        }
    }
}
