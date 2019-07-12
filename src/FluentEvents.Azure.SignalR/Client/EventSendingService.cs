using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace FluentEvents.Azure.SignalR.Client
{
    internal class EventSendingService : IEventSendingService
    {
        private static readonly string[] _nullReceiver = {null};

        private readonly IAzureSignalRHttpClient _signalRHttpClient;
        private readonly IUrlProvider _urlProvider;
        private readonly IHttpRequestFactory _httpRequestFactory;

        private readonly ConnectionString _connectionString;

        public EventSendingService(
            IOptions<AzureSignalRServiceConfig> config,
            IAzureSignalRHttpClient signalRHttpClient,
            IUrlProvider urlProvider,
            IHttpRequestFactory httpRequestFactory
        )
        {
            _signalRHttpClient = signalRHttpClient;
            _httpRequestFactory = httpRequestFactory;
            _urlProvider = urlProvider;
            _connectionString = config.Value.ConnectionString;
        }

        public async Task SendEventAsync(PublicationMethod publicationMethod,
            string hubName,
            string hubMethodName,
            string[] receiverIds,
            object domainEvent
        )
        {
            foreach (var receiverId in receiverIds ?? _nullReceiver)
            {
                var url = _urlProvider.GetUrl(
                    _connectionString.Endpoint,
                    publicationMethod,
                    hubName,
                    receiverId
                );

                var request = _httpRequestFactory.CreateHttpRequest(
                    _connectionString,
                    hubMethodName,
                    domainEvent,
                    url
                );

                var response = await _signalRHttpClient.SendAsync(request).ConfigureAwait(false);
                try
                {
                    response.EnsureSuccessStatusCode();
                }
                catch (HttpRequestException e)
                {
                    throw new AzureSignalRPublishingFailedException(e);
                }

                if (response.StatusCode != HttpStatusCode.Accepted)
                    throw new AzureSignalRPublishingFailedException(response.StatusCode);
            }
        }
    }
}
