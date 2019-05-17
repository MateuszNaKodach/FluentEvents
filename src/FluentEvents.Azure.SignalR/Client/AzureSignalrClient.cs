using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace FluentEvents.Azure.SignalR.Client
{
    internal class AzureSignalRClient : IAzureSignalRClient
    {
        private static readonly string[] _nullReceiver = {null};

        private readonly HttpClient _httpClient;
        private readonly IUrlProvider _urlProvider;
        private readonly IHttpRequestFactory _httpRequestFactory;

        private readonly ConnectionString _connectionString;

        public AzureSignalRClient(
            IOptions<AzureSignalRServiceConfig> config,
            HttpClient httpClient,
            IUrlProvider urlProvider,
            IHttpRequestFactory httpRequestFactory
        )
        {
            _httpClient = httpClient;
            _httpRequestFactory = httpRequestFactory;
            _urlProvider = urlProvider;
            _connectionString = config.Value.ConnectionString;
        }

        public async Task SendEventAsync(
            PublicationMethod publicationMethod,
            string hubName,
            string hubMethodName,
            string[] receiverIds,
            object eventSender,
            object eventArgs
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
                    eventSender,
                    eventArgs,
                    url
                );

                var response = await _httpClient.SendAsync(request);
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
