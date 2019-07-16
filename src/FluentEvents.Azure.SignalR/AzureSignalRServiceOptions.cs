using FluentEvents.Azure.SignalR.Client;
using FluentEvents.Infrastructure;

namespace FluentEvents.Azure.SignalR
{
    /// <summary>
    ///     The configuration of the Azure SignalR Service plugin.
    /// </summary>
    public class AzureSignalRServiceOptions
    {
        /// <summary>
        ///     The connection string of the Azure SignalR Service.
        /// </summary>
        public string ConnectionString { get; set; }
    }
}