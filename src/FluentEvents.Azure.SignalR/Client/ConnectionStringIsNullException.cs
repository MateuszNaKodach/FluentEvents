﻿namespace FluentEvents.Azure.SignalR.Client
{
    /// <summary>
    ///     An exception thrown when <see cref="AzureSignalRServiceConfig.ConnectionString"/> is null.
    /// </summary>
    public class ConnectionStringIsNullException : FluentEventsException
    {
        internal ConnectionStringIsNullException()
            : base("The connection string is null")
        {
        }
    }
}