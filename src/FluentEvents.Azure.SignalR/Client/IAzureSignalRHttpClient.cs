using System.Net.Http;
using System.Threading.Tasks;

namespace FluentEvents.Azure.SignalR.Client
{
    internal interface IAzureSignalRHttpClient
    {
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage httpRequestMessage);
    }
}