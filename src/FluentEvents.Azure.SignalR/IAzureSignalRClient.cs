using System.Threading.Tasks;

namespace FluentEvents.Azure.SignalR
{
    internal interface IAzureSignalRClient
    {
        Task SendEventAsync(
            PublicationMethod publicationMethod,
            string hubName,
            string hubMethodName,
            string[] subjectIds,
            object eventSender,
            object eventArgs
        );
    }
}