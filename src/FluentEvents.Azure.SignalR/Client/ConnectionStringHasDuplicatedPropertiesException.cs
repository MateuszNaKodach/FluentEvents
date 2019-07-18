namespace FluentEvents.Azure.SignalR.Client
{
    internal class ConnectionStringHasDuplicatedPropertiesException : FluentEventsAzureSignalRException
    {
        internal ConnectionStringHasDuplicatedPropertiesException(string key)
            : base($"Duplicate properties found in connection string: {key}.")
        {
            
        }
    }
}