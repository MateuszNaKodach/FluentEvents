namespace FluentEvents.Azure.SignalR.Client
{
    /// <inheritdoc />
    /// <summary>
    ///     An exception thrown when parsing a connection string with missing properties.
    /// </summary>
    public class ConnectionStringHasMissingPropertiesException : FluentEventsException
    {
        internal ConnectionStringHasMissingPropertiesException(string endpointProperty, string accessKeyProperty)
            : base($"The connection string has missing required properties {endpointProperty} and {accessKeyProperty}.")
        {
            
        }
    }
}