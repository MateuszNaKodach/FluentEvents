using System;
using FluentEvents.Azure.SignalR.Client;
using FluentEvents.Infrastructure;
using Microsoft.Extensions.Options;

namespace FluentEvents.Azure.SignalR
{
    internal class AzureSignalRServiceConfigValidator : IValidateOptions<AzureSignalRServiceConfig>
    {
        public ValidateOptionsResult Validate(string name, AzureSignalRServiceConfig options)
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