using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace FluentEvents.Azure.SignalR.Client
{
    internal class AzureSignalRHttpClient : IAzureSignalRHttpClient
    {
        private readonly HttpClient _httpClient;

        public AzureSignalRHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage httpRequestMessage) 
            => _httpClient.SendAsync(httpRequestMessage);
    }
}
