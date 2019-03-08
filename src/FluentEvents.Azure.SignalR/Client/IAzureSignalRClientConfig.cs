namespace FluentEvents.Azure.SignalR.Client
{
    /// <summary>
    ///     The configuration of the Azure SignalR Service plugin.
    /// </summary>
    public interface IAzureSignalRClientConfig
    {
        /// <summary>
        ///     The connection string of the Azure SignalR Service.
        /// </summary>
        string ConnectionString { get; }
    }
}