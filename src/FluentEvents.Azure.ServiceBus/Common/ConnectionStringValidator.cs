using System;
using Microsoft.Azure.ServiceBus;

namespace FluentEvents.Azure.ServiceBus.Common
{
    internal static class ConnectionStringValidator
    {
        internal static bool IsValid(string connectionString, string connectionStringName, out string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                errorMessage = $"{connectionStringName} is null or empty.";
                return false;
            }

            try
            {
                // ReSharper disable once UnusedVariable
                var serviceBusConnectionStringBuilder = new ServiceBusConnectionStringBuilder(
                    connectionString
                );
            }
            catch (ArgumentException e)
            {
                errorMessage = $"{connectionStringName} is invalid: {e.Message}";
                return false;
            }

            errorMessage = null;
            return true;
        }
    }
}
