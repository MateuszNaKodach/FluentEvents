using System.Net.Http;

namespace FluentEvents.Azure.SignalR.Client
{
    internal interface IHttpRequestFactory
    {
        HttpRequestMessage CreateHttpRequest(
            ConnectionString connectionString,
            string hubMethodName,
            object eventSender, 
            object eventArgs,
            string url
        );
    }
}