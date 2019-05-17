using FluentEvents.Azure.SignalR.Client;
using FluentEvents.Infrastructure;

namespace FluentEvents.Azure.SignalR
{
    /// <summary>
    ///     The configuration of the Azure SignalR Service plugin.
    /// </summary>
    public class AzureSignalRServiceConfig : IValidableConfig
    {
        private string _connectionString;

        /// <summary>
        ///     The connection string of the Azure SignalR Service.
        /// </summary>
        public string ConnectionString
        {
            get => _connectionString;
            set => _connectionString = Client.ConnectionString.Validate(value);
        }

        void IValidableConfig.Validate()
        {
            if (ConnectionString == null)
                throw new ConnectionStringIsNullException();
        }
    }
}