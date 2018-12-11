using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace FluentEvents.Azure.SignalR
{
    internal class AzureSignalRClient : IAzureSignalRClient
    {
        private readonly HttpClient m_HttpClient;
        private readonly IAccessTokensService m_AccessTokensService;
        private readonly ConnectionString m_ConnectionString;

        protected internal string MimeType = "application/json";
        protected internal Encoding Encoding = Encoding.UTF8;

        public AzureSignalRClient(
            IOptions<AzureSignalRClientConfig> config,
            HttpClient httpClient,
            IConnectionStringBuilder connectionStringBuilder,
            IAccessTokensService accessTokensService
        )
        {
            m_HttpClient = httpClient;
            m_AccessTokensService = accessTokensService;
            m_ConnectionString = connectionStringBuilder.ParseConnectionString(config.Value.ConnectionString);
        }

        public async Task SendEventAsync(
            PublicationMethod publicationMethod,
            string hubName,
            string hubMethodName,
            string[] subjectIds,
            object eventSender,
            object eventArgs
        )
        {
            foreach (var subjectId in subjectIds)
            {
                string url;
                switch (publicationMethod)
                {
                    case PublicationMethod.User:
                        url = GetSendToUserUrl(hubName, subjectId);
                        break;
                    case PublicationMethod.Group:
                        url = GetSendToGroupUrl(hubName, subjectId);
                        break;
                    case PublicationMethod.Broadcast:
                        url = GetBroadcastUrl(hubName);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(publicationMethod));
                }

                var request = BuildRequest(url, new PayloadMessage
                {
                    Arguments = new[] { eventSender, eventArgs },
                    Target = hubMethodName
                });

                var response = await m_HttpClient.SendAsync(request);
                if (response.StatusCode != HttpStatusCode.Accepted)
                    throw new AzureSignalRPublishingFailedException();
            }
        }

        private HttpRequestMessage BuildRequest(string url, PayloadMessage payloadMessage)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, GetUrl(url));

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", m_AccessTokensService.GenerateAccessToken(m_ConnectionString, url));
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(MimeType));
            request.Content = new StringContent(JsonConvert.SerializeObject(payloadMessage), Encoding, MimeType);

            return request;
        }

        private Uri GetUrl(string baseUrl) => new UriBuilder(baseUrl).Uri;

        private string GetSendToUserUrl(string hubName, string userId) => $"{GetBaseUrl(hubName)}/user/{userId}";

        private string GetSendToGroupUrl(string hubName, string group) => $"{GetBaseUrl(hubName)}/group/{group}";

        private string GetBroadcastUrl(string hubName) => $"{GetBaseUrl(hubName)}";

        private string GetBaseUrl(string hubName) => $"{m_ConnectionString.Endpoint}/api/v1/hubs/{hubName.ToLower()}";

        private class PayloadMessage
        {
            public string Target { get; set; }
            public object[] Arguments { get; set; }
        }
    }
}
