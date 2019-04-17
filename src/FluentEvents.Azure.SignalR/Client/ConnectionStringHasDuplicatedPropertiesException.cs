namespace FluentEvents.Azure.SignalR.Client
{
    /// <inheritdoc />
    /// <summary>
    ///     An exception thrown when parsing a connection string with duplicated properties.
    /// </summary>
    public class ConnectionStringHasDuplicatedPropertiesException : FluentEventsAzureSignalRException
    {
        internal ConnectionStringHasDuplicatedPropertiesException(string key)
            : base($"Duplicate properties found in connection string: {key}.")
        {
            
        }
    }
}