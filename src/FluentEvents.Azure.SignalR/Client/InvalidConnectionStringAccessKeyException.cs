using System;

namespace FluentEvents.Azure.SignalR.Client
{
 
    internal class InvalidConnectionStringAccessKeyException : FluentEventsAzureSignalRException
    {
        internal InvalidConnectionStringAccessKeyException(Exception innerException) 
            : base("The connection string access key is invalid.", innerException)
        {
            
        }
    }
}