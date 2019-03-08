﻿using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace FluentEvents.Azure.SignalR.Client
{
    internal class AzureSignalRClient : IAzureSignalRClient
    {
        private readonly HttpClient m_HttpClient;
        private readonly IUrlProvider m_UrlProvider;
        private readonly IHttpRequestFactory m_HttpRequestFactory;

        private readonly ConnectionString m_ConnectionString;

        public AzureSignalRClient(
            IOptions<AzureSignalRClientConfig> config,
            IConnectionStringBuilder connectionStringBuilder,
            HttpClient httpClient,
            IUrlProvider urlProvider,
            IHttpRequestFactory httpRequestFactory
        )
        {
            m_HttpClient = httpClient;
            m_HttpRequestFactory = httpRequestFactory;
            m_UrlProvider = urlProvider;
            m_ConnectionString = connectionStringBuilder.ParseConnectionString(config.Value.ConnectionString);
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
            foreach (var receiverId in receiverIds)
            {
                var url = m_UrlProvider.GetUrl(
                    m_ConnectionString.Endpoint,
                    publicationMethod,
                    hubName,
                    receiverId
                );

                var request = m_HttpRequestFactory.CreateHttpRequest(
                    m_ConnectionString,
                    hubMethodName,
                    eventSender,
                    eventArgs,
                    url
                );

                var response = await m_HttpClient.SendAsync(request);

                if (response.StatusCode != HttpStatusCode.Accepted)
                    throw new AzureSignalRPublishingFailedException();
            }
        }
    }
}
