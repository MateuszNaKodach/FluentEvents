using System;
using Microsoft.Azure.ServiceBus;

namespace FluentEvents.Azure.ServiceBus.Common
{
    internal static class ConnectionStringValidator
    {
        internal static string ValidateOrThrow(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ConnectionStringIsInvalidException();

            try
            {
                var serviceBusConnectionStringBuilder = new ServiceBusConnectionStringBuilder(
                    connectionString
                );
            }
            catch (ArgumentException e)
            {
                throw new ConnectionStringIsInvalidException(e);
            }

            return connectionString;
        }
    }
}
