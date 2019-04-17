using System.Net;
using System.Net.Http;

namespace FluentEvents.Azure.SignalR.Client
{
    /// <summary>
    ///     An exception thrown when a request to an Azure SignalR Service API fails.
    /// </summary>
    public class AzureSignalRPublishingFailedException : FluentEventsAzureSignalRException
    {
        internal AzureSignalRPublishingFailedException(HttpRequestException httpRequestException)
            : base("Failed to send a request to the Azure SignalR Service API", httpRequestException)
        {
        }

        internal AzureSignalRPublishingFailedException(HttpStatusCode statusCode)
            : base($"Failed to send a request to the Azure SignalR Service API," +
                   $" The expected Status code is: {HttpStatusCode.Accepted} but was: {statusCode}")
        {
        }
    }
}