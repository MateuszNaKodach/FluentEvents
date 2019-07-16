namespace FluentEvents.Azure.SignalR.Client
{
    /// <summary>
    ///     An exception thrown when <see cref="AzureSignalRServiceOptions.ConnectionString"/> is null.
    /// </summary>
    public class ConnectionStringIsNullException : FluentEventsAzureSignalRException
    {
        internal ConnectionStringIsNullException()
            : base("The connection string is null")
        {
        }
    }
}