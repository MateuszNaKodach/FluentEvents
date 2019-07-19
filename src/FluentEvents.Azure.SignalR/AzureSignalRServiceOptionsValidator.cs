using FluentEvents.Azure.SignalR.Client;
using Microsoft.Extensions.Options;

namespace FluentEvents.Azure.SignalR
{
    internal class AzureSignalRServiceOptionsValidator : IValidateOptions<AzureSignalRServiceOptions>
    {
        public ValidateOptionsResult Validate(string name, AzureSignalRServiceOptions options)
        {
            if (!ConnectionString.IsValid(
                    options.ConnectionString,
                    nameof(options.ConnectionString),
                    out var errorMessage
                )
            )
                return ValidateOptionsResult.Fail(errorMessage);

            return ValidateOptionsResult.Success;
        }
    }
}