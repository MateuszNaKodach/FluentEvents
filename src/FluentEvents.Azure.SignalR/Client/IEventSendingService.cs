using System.Threading.Tasks;

namespace FluentEvents.Azure.SignalR.Client
{
    internal interface IEventSendingService
    {
        Task SendEventAsync(
            PublicationMethod publicationMethod,
            string hubName,
            string hubMethodName,
            string[] receiverIds,
            object eventSender,
            object eventArgs
        );
    }
}