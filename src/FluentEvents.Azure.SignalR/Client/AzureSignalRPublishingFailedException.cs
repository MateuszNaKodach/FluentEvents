namespace FluentEvents.Azure.SignalR.Client
{
    /// <summary>
    ///     An exception thrown when a request to an Azure SignalR Service API fails.
    /// </summary>
    public class AzureSignalRPublishingFailedException : FluentEventsException
    {
        internal AzureSignalRPublishingFailedException()
            : base("Failed to send a request to the Azure SignalR Service API")
        {
        }
    }
}