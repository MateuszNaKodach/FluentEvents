namespace FluentEvents.Azure.SignalR.Client
{
    internal class ConnectionStringIsNullException : FluentEventsAzureSignalRException
    {
        internal ConnectionStringIsNullException()
            : base("The connection string is null")
        {
        }
    }
}