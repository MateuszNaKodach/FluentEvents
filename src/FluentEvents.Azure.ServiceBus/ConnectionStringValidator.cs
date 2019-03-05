using System;
using Microsoft.Azure.ServiceBus;

namespace FluentEvents.Azure.ServiceBus
{
    internal static class ConnectionStringValidator
    {
        internal static string ValidateOrThrow(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidConnectionStringException();

            try
            {
                var serviceBusConnectionStringBuilder = new ServiceBusConnectionStringBuilder(
                    connectionString
                );
            }
            catch (ArgumentException e)
            {
                throw new InvalidConnectionStringException(e);
            }

            return connectionString;
        }
    }
}
