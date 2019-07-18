namespace FluentEvents.Azure.SignalR.Client
{
    internal class ConnectionStringHasMissingPropertiesException : FluentEventsAzureSignalRException
    {
        internal ConnectionStringHasMissingPropertiesException(string endpointProperty, string accessKeyProperty)
            : base($"The connection string has missing required properties {endpointProperty} and {accessKeyProperty}.")
        {
            
        }
    }
}