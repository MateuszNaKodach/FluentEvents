using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;

namespace FluentEvents.Azure.SignalR.Client
{
    internal class HttpRequestFactory : IHttpRequestFactory
    {
        private readonly string m_MimeType = "application/json";
        private readonly Encoding m_Encoding = Encoding.UTF8;

        private readonly IAccessTokensService m_AccessTokensService;

        public HttpRequestFactory(IAccessTokensService accessTokensService)
        {
            m_AccessTokensService = accessTokensService;
        }

        public HttpRequestMessage CreateHttpRequest(
            ConnectionString connectionString,
            string hubMethodName,
            object eventSender, 
            object eventArgs,
            string url
        )
        {
            return BuildRequest(
                connectionString,
                url,
                new PayloadMessage
                {
                    Arguments = new[] {eventSender, eventArgs},
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
                m_AccessTokensService.GenerateAccessToken(connectionString, url)
            );
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(m_MimeType));
            request.Content = new StringContent(JsonConvert.SerializeObject(payloadMessage), m_Encoding, m_MimeType);

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