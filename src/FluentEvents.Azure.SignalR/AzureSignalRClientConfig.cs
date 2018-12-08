namespace FluentEvents.Azure.SignalR
{
    internal class AzureSignalRClientConfig : IAzureSignalRClientConfig
    {
        public string ConnectionString { get; set; }
    }
}