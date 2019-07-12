using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;

namespace FluentEvents.Azure.SignalR.Client
{
    internal class HttpRequestFactory : IHttpRequestFactory
    {
        private readonly string _mimeType = "application/json";
        private readonly Encoding _encoding = Encoding.UTF8;

        private readonly IAccessTokensService _accessTokensService;

        public HttpRequestFactory(IAccessTokensService accessTokensService)
        {
            _accessTokensService = accessTokensService;
        }

        public HttpRequestMessage CreateHttpRequest(ConnectionString connectionString,
            string hubMethodName,
            object domainEvent,
            string url
        )
        {
            return BuildRequest(
                connectionString,
                url,
                new PayloadMessage
                {
                    Arguments = new[] {domainEvent},
                    Target = hubMethodName
                }
            );
        }

        private HttpRequestMessage BuildRequest(
            ConnectionString connectionString,
            string url,
            PayloadMessage payloadMessage
        )
        {
            var request = new HttpRequestMessage(HttpMethod.Post, GetUri(url));

            request.Headers.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                _accessTokensService.GenerateAccessToken(connectionString, url)
            );
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(_mimeType));
            request.Content = new StringContent(JsonConvert.SerializeObject(payloadMessage), _encoding, _mimeType);

            return request;
        }

        private Uri GetUri(string baseUrl) => new UriBuilder(baseUrl).Uri;

        private class PayloadMessage
        {
            public string Target { get; set; }
            public object[] Arguments { get; set; }
        }
    }
}