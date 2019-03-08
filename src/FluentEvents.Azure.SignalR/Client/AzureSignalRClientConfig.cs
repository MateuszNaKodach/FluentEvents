namespace FluentEvents.Azure.SignalR.Client
{
    /// <inheritdoc />
    public class AzureSignalRClientConfig : IAzureSignalRClientConfig
    {
        /// <inheritdoc />
        public string ConnectionString { get; set; }
    }
}