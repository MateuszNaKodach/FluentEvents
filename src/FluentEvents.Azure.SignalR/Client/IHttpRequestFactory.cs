using System.Net.Http;

namespace FluentEvents.Azure.SignalR.Client
{
    internal interface IHttpRequestFactory
    {
        HttpRequestMessage CreateHttpRequest(ConnectionString connectionString,
            string hubMethodName,
            object domainEvent,
            string url);
    }
}